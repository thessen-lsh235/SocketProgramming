#include <iostream>
#include <unistd.h>
#include <arpa/inet.h>
#include <cstring>
#include <sys/select.h>

int main(){

    //1.소켓 생성
    int sock = socket(AF_INET, SOCK_STREAM,0);
    if (sock < 0) { perror("socket"); return 1; }

    //서버 구조체 생성
    sockaddr_in serv_addr{};
    serv_addr.sin_family = AF_INET;
    serv_addr.sin_port = htons(8888);
    inet_pton(AF_INET, "127.0.0.1", &serv_addr.sin_addr);
    
    //다 만들었으면 서버에 연결을 요청
    if(connect(sock, (sockaddr*)&serv_addr, sizeof(serv_addr))<0){
        std::cerr << "Connection failed\n";
        return 1;
    };
    std::cout << "Connected to server!\n";


    // 4.select용 fd_set 선언
    fd_set read_fdset;
    char buffer[1024];

    while(true){
        FD_ZERO(&read_fdset); //fdset 초기화
        FD_SET(0, &read_fdset); // stdin감지
        FD_SET(sock, &read_fdset); // 서버 소켓 감지 등록

        int max_fd = sock > 0 ? sock : 0;

        // 5. select 대기
        int activity = select(max_fd+1, &read_fdset, nullptr, nullptr, nullptr);
        if(activity == -1){
            std::cerr << "select error\n";
            break;
        }
        // 6. 사용자가 입력한 경우(stdin)
        if(FD_ISSET(0, &read_fdset)){
            memset(buffer, 0, sizeof(buffer));
            if(!fgets(buffer, sizeof(buffer), stdin)) break;
            send(sock, buffer, strlen(buffer), 0);
        }

        //7. 서버에서 메시지가 온 경우
        if(FD_ISSET(sock, &read_fdset)){
            // memset(buffer, 0, sizeof(buffer)); //buffer 받을 공간 할당해놓고
            int len = recv(sock, buffer, sizeof(buffer)-1,  0);
            if(len<=0){
                std::cout <<"server disconnected\n";
                break;
            }
            buffer[len] = '\0';  // ⭐ 추가
            std::cout <<"[Server]: " << buffer << std::endl;
        }
    }

    close(sock);
    return 0;

}