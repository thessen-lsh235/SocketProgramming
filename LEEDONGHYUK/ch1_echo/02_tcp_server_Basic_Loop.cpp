//this is tcp server

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
#include <netinet/in.h>
#include <unistd.h>
#include <string.h>
#include <stdio.h>

#define PORT 12345 //서버 포트
#define MAXBUF 1024

int main(){

    // 🌍 1. 소켓 생성 : TCP 소켓을 판다.

    int tcp_socket = socket(PF_INET, SOCK_STREAM, IPPROTO_TCP);
    // PF_INET : IPv4 주소체계,SOCK_STREAM : 스트림 기반 소켓(TCP)
    // IPPROTO_TCP : TCP프로토콜임을 명시한다. SOCK_STREAM과 같이 사용-> type = tcp 으로 이해
   if (tcp_socket < 0) {
    std::cerr << "Server : 소켓 생성 실패(1)" << std::endl;
    return 1;
    }
    //이건 그냥 파일 입출력에서도 자주 하듯이, 예외처리 cerr는 cout 과 다르니, 디스크립터 파일번호 2번썼었나? 



    // 🌍 2. 서버 주소 구조체 설정

    struct sockaddr_in server_addr;
    std::memset(&server_addr, 0 , sizeof(server_addr));
    //서버 주소에 대한 구조체를 0으로 초기화 한다.
    server_addr.sin_family = AF_INET; //IPV4 주소체계
    server_addr.sin_addr.s_addr = INADDR_ANY;
    //sin_addr 주소 값을 담는 구조체, sin_zero가 패딩을 담아줌(0)
    ///s_addr -> 실제 주소, INADDR_ANY -> 0.0.0.0 으로 특수한 주소 , 모든 주소를 받기
    server_addr.sin_port = htons(PORT);
    //HOST의 바이트 순서를 네트워크 바이트로 변환한다(ntohs는 반대)
    //86과 같은 경우 리틀엔디언을 쓴다. 리틀엔디언 -> 하드웨어 친화

    std::cout << "Server :서버 구조체 생성 완료(2)" << std::endl;
    //🌍 3.  소켓을 포트에 바인딩: 소켓에 ip와 포트 정보를 태우고, 어떤 포트에서 통신할지 정한다.

    //(struct sockaddr*)&server_addr: sockaddr_in 구조체의 주소를 sockaddr 포인터로 형변환
    if(bind(tcp_socket, (struct sockaddr*)&server_addr, sizeof(server_addr)) <0){
        //tcp_socket은 소켓에 대한 정보를 담고 있다. iterator처럼 동작하여 몇번 소켓의 인덱스인지
        //tcp_socket의 상태를 확인하고(cpu는 이것을 소켓디스크립터로 관리 중임)
        //server_addr 의 sin_port, sin_addr.s_addr이 누군가 사용중이라면 bind() -> 음수로 반환됨
        // 이게 유효하지 않거나 닫힌 소켓이거나 이미 바인딩 되어있으면 버림
        std::cerr << "Server :소켓 바인딩 실패(3)" <<std::endl;
        close(tcp_socket);
        return 1;
    }


    //🌍 4. 클라이언트 연결 대기: listen() 호출로 소켓 수신상태를 변경한다.

    // 3은 대기열의 최대 길이인데, 왜 3으로 지정해놨는지는 잘 모르겠음.
    if(listen(tcp_socket,3) < 0){
            std::cerr <<"Server :소켓 리스닝 실패(4)" << std::endl;
            close(tcp_socket);
            return 1;
    }
    //여긴 소켓 리스닝이 성공했을 경우
    std::cout << "Server :클라이언트 연결 대기 중...(4)" << std::endl;
    //

    //🌍 5. 클라이언트 연결 수락: 연결 요청을 받아들인다.

    int client_socket; // 클라이언트와 통신하기 위해서 새로운 소켓 생성
    struct sockaddr_in client_addr;
    socklen_t addr_len = sizeof(client_addr);
    // 클라이언트도 sockaddr_in 구조체를 받아서 만들어줌. 
    // 마찬가지로 주소체계(IPv4),실제주소(패딩 처리해서 받아줌), htons등의 처리를 해줘야겟네

    client_socket = accept(tcp_socket, (struct sockaddr*)&client_addr, &addr_len);
    //서버단에서 구조체를 세팅하고 만들었듯이, 클라이언트도 구조체로 만들어서 던져줄 것임. 그것을 받으면 됨.
    if(client_socket <0){
        std::cerr << "Server :클라이언트 연결 수락 실패(5)" << std::endl;
        close(tcp_socket);
        return 1;
    }
    std::cout << "Server :클라이언트 연결됨(5)" << std::endl;

    //🌍 6. 클라이언트 연결 수락: 연결 요청을 받아들인다.
    while(true){
        char buffer[MAXBUF];
        int read_size = recv(client_socket, buffer, sizeof(buffer)-1 ,0);
        // 여기까지 보니까, 대부분의 socket 통신 함수들은 첫번째 인자는 소켓이고, 두번째 인자는 주소나 버퍼, 
        // 마지막은 사이즈 인 것 같다. 여기서 0은 통신관련 옵션인데, 없을 경우 0 MSG_WAITALL, MSG_PEEK 등등..
        // 쓸일 있을지 모르겠음
        if (read_size <= 0) {
            std::cerr << "Server :데이터 수신 실패" << std::endl;
            close(client_socket); //이젠 얘도 닫아주어야함
            close(tcp_socket);
            return 1;
        }
        // 받은 데이터의 끝에 널(문자열 종료)을 추가하여 문자열 처리 가능하게 함
        buffer[read_size] = '\0'; 
        std::cout << "Server :클라이언트로부터 받은 메시지: " << buffer << std::endl;
        const char* message = "Server :메시지를 받았습니다!" ;
        send(client_socket, message, strlen(message),0);
    }


    //🌍 7. 클라이언트에게 응답 보내기


    //🌍 8. 소켓 종료: 사용이 끝난 소켓들을 닫아서 자원을 해제합니다.
    close(client_socket);   // 클라이언트와의 소켓 종료
    close(tcp_socket);      // 서버 소켓 종료


}


