#include <iostream>
#include <thread>
#include <unistd.h>
#include <arpa/inet.h>
#include <sys/socket.h>
#include <cstring>
#include "message.h"

void recvThread(int sock) 
{
    while (true) 
    {
        Message msg;
        ssize_t len = recv(sock, &msg, sizeof(msg), 0);
        if (len <= 0) 
        {
            std::cout << "[서버와 연결 종료됨]\n";
            close(sock);
            break;
        }
        std::cout << "\n---------------------------------\n[서버 메세지] " << msg.content << "\n---------------------------------\n" << "전송 메세지 입력\n>> " << std::flush;
    }
}

void sendThread(int sock) 
{
    while (true) 
    {
        char input[MAX_MSG_LEN];
        std::cout << "전송 메세지 입력\n>> ";
        std::cin.getline(input, MAX_MSG_LEN);

        Message msg;
        msg.id = 0; // 서버에서 실제 목적지 id 설정
        strcpy(msg.content, input);

        ssize_t sent = send(sock, &msg, sizeof(msg), 0);
        if (sent <= 0) 
        {
            std::cerr << "[메시지 전송 실패]\n";
            break;
        }
    }
}

int main(int argc, char* argv[]) 
{
    int sock = socket(AF_INET, SOCK_STREAM, 0);
    if (sock == -1) 
    {
        std::cerr << "소켓 생성 실패\n";
        return -1;
    }

    sockaddr_in servAddr;
    servAddr.sin_family = AF_INET;
    servAddr.sin_port = htons(atoi(argv[2])); 
    inet_pton(AF_INET, argv[1], &servAddr.sin_addr); 

    if (connect(sock, (sockaddr*)&servAddr, sizeof(servAddr)) == -1) 
    {
        std::cerr << "서버 연결 실패\n";
        return -1;
    }

    std::cout << "[서버에 연결되었습니다]\n";

    std::thread(recvThread, sock).detach();
    std::thread(sendThread, sock).join();

    return 0;
}
