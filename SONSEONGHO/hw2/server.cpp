#include <iostream>
#include <thread>
#include <mutex>
#include <map>
#include <vector>
#include <unistd.h>
#include <arpa/inet.h>
#include <sys/socket.h>
#include <sys/select.h>
#include <cstring>
#include "message.h"

std::map<int, int> idToSock;
std::mutex mapMutex;
fd_set recvFds;
int fdMax = 0;

void recvThread() {
    while (true) {
        fd_set tmpFds;
        FD_ZERO(&tmpFds);

        {
            std::lock_guard<std::mutex> lock(mapMutex);
            tmpFds = recvFds;
        }

        timeval timeout{1, 0};
        int ret = select(fdMax + 1, &tmpFds, nullptr, nullptr, &timeout);
        if (ret < 0) 
        {
            perror("select()");
            break;
        }

        if (ret == 0) continue;

        std::lock_guard<std::mutex> lock(mapMutex);
        for (const auto& [id, sock] : idToSock) 
        {
            if (FD_ISSET(sock, &tmpFds)) 
            {
                Message msg;
                ssize_t len = recv(sock, &msg, sizeof(msg), 0);
                msg.content[len] = 0;
                if (len <= 0) 
                {
                    std::cout << "[클라이언트 " << id << "번] 연결 종료\n";
                    close(sock);
                    FD_CLR(sock, &recvFds);
                    idToSock.erase(id);
                    break;  // map 변경 중에는 반복 종료
                } 
                else {
                    std::cout << "---------------------------------\n[클라이언트 " << id << "번 메세지]: " << msg.content 
                    << "\n---------------------------------\n" 
                    << "[메세지 전송 형식: <id> <message>\n>> " << std::flush;
                }
            }
        }
    }
}

void sendThread() 
{
    while (true) 
    {
        int targetId;
        char input[MAX_MSG_LEN];
        std::cout << "[메세지 전송 형식: <id> <message>\n>> ";
        std::cin >> targetId;
        std::cin.ignore();
        std::cin.getline(input, MAX_MSG_LEN);

        std::lock_guard<std::mutex> lock(mapMutex);
        if (idToSock.count(targetId)) 
        {
            Message msg;
            msg.id = targetId;
            strcpy(msg.content, input);
            send(idToSock[targetId], &msg, sizeof(msg), 0);
        } 
        else 
        {
            std::cout << "[Error] No such client ID\n";
        }
    }
}

int main(int argc, char* argv[]) {
    int serverSock = socket(AF_INET, SOCK_STREAM, 0);
    sockaddr_in servAddr;
    servAddr.sin_family = AF_INET;
    servAddr.sin_addr.s_addr = INADDR_ANY;
    servAddr.sin_port = htons(atoi(argv[1]));

    bind(serverSock, (sockaddr*)&servAddr, sizeof(servAddr));
    listen(serverSock, 100);
    std::cout << "[Server Listening on Port " << argv[1] << "]\n";

    FD_ZERO(&recvFds);
    FD_SET(serverSock, &recvFds);
    fdMax = serverSock;

    int nextClientId = 1;

    std::thread(recvThread).detach();
    std::thread(sendThread).detach();

    while (true) {
        fd_set tmpFds = recvFds;
        timeval timeout{1, 0};
        int ret = select(fdMax + 1, &tmpFds, nullptr, nullptr, &timeout);
        if (ret < 0) break;

        if (FD_ISSET(serverSock, &tmpFds)) {
            sockaddr_in clntAddr;
            socklen_t addrLen = sizeof(clntAddr);
            int clntSock = accept(serverSock, (sockaddr*)&clntAddr, &addrLen);

            {
                std::lock_guard<std::mutex> lock(mapMutex);
                idToSock[nextClientId] = clntSock;
                FD_SET(clntSock, &recvFds);
                if (clntSock > fdMax) fdMax = clntSock;
            }

            std::cout << "[클라이언트 " << nextClientId << "번] 연결 (fd: " << clntSock << ")\n>> ";
            nextClientId++;
        }
    }

    close(serverSock);
    return 0;

}
