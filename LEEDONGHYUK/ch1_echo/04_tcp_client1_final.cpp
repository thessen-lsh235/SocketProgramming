// this is tcp client

/*
    1.socket()
    2.bind()
    3.listen()
    4.accept()
    ---loop---
    |5.recv() |
    |6.send() |
    ----------
    7.closesocket()
    8.close()

*/

#include <iostream>
#include <cstdlib>
#include <cstring>

#include <sys/socket.h> // socket관련 헤더
#include <arpa/inet.h>  // socket관련 헤더
#include <netinet/in.h> //sockaddr_in 구조체 선언

#include <unistd.h>
#include <string.h>
#include <stdio.h>

#define PORT 12345 // 서버 포트
#define MAXBUF 4
#define SERVER_IP "127.0.0.1" // 서버 IP (로컬호스트 예시)

using namespace std; 

int main()
{

    // 🌍 1. 소켓 생성 : TCP 소켓을 클라이언트도 만든다.

    int tcp_socket = socket(PF_INET, SOCK_STREAM, IPPROTO_TCP);
    // SOCK_STREAM : 스트림 기반 소켓(TCP) == 연속된 바이트 흐름 처럼 데이터를 주고 받는 형식
    // 스트림기반 == tcp== 바이트 스트림 == 연결지향, 메시지 기반 ==udp == 메시지 단위 == 비연결지향
    // IPPROTO_TCP : TCP프로토콜임을 명시한다. SOCK_STREAM과 같이 사용-> type = tcp 으로 이해
    if (tcp_socket < 0)
    {
        cerr << "Client : 소켓 생성 실패(1)" << endl;
        return 1;
    }

    // 🌍 2. 서버 주소 구조체 설정

    struct sockaddr_in server_addr;
    memset(&server_addr, 0, sizeof(server_addr));
    server_addr.sin_family = AF_INET; // IPV4

    if (inet_pton(AF_INET, SERVER_IP, &server_addr.sin_addr) <= 0)
    {
        
        // GPT : inet_pton() 함수는 문자 형태의 IP 주소(예: "127.0.0.1")를
        // 이진 데이터 형식(네트워크 바이트 순서의 정수)으로 변환합니다.
        cerr << "Client : 유효 하지 않은 서버 IP 주소(2)" << endl;
        close(tcp_socket);
        return 1;
    }

    server_addr.sin_port = htons(PORT);

    // 🌍 3. 서버에 연결을 요청
    if (connect(tcp_socket, (struct sockaddr *)&server_addr, sizeof(server_addr)) < 0)
    {
        cerr << "Client : 서버 연결 실패(3)" << endl;
        close(tcp_socket);
        return 1;
    }
    std::cout << "Client : 서버에 연결 되었습니다(3) " << endl;

    // 🌍 4. 서버로 데이터 전송하기

    std::cout << "Client : 서버에 보낼 메시지 입력하세요(종료하려면 exit)" << endl;
    string input;
    do{
        getline(cin, input);

        if(input == "exit"){
            const char* exit_msg = "클라이언트가 종료를 요청";
            send(tcp_socket, exit_msg, strlen(exit_msg), 0);

            string confirm;
            cout << "want exit? (y/n)";
            getline(cin, confirm);

            if(confirm == "y" || confirm =="Y") break;
            else{
                const char* retry_msg = "재접속 시도 중....";
                send(tcp_socket, retry_msg, strlen(retry_msg),0);
                continue;
            }

        }
        string temp = "client 1 says: ";
        input.insert(0,temp);

        send(tcp_socket, input.c_str(), input.length(), 0);
        //왜멍청하게 input으로 안던지고 c.str()으로 던질까?
        //send는 고인물이라 str같은거 모르고 c_str()으로 받아서 던줘주어야함. 즉 const void*타입의 메모리 주소를 요구함.
        // 나는 그럼 input이라는 string객체로 일단 받아놓고. 그놈의 맨앞 주소를 받아서 tcp_socket한 테 던져준다?
        // 왜냐면 어차피 cpp에서 string이라는것은 char[]이며, 이것의 주소를 c_str()으로 던져준다고 이해?
        
        char buffer[MAXBUF];
        int readsize = recv(tcp_socket, buffer, sizeof(buffer) -1 , 0);
        if(readsize<=0){
            cerr <<"Client : 서버 끊김 또는 오류" << endl;
        }
        buffer[readsize] = 0;
        cout <<"client: 서버 응답: " << buffer << endl;
        
    }while(true);
    // 🌍 6. 닫기

    close(tcp_socket);

    return 0;
}