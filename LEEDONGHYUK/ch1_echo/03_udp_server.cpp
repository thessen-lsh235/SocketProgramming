/*
    this is udp_server
    1. 구조체, 디스크럽터 선언
    2. 소켓 생성
    3. 서버 주소 설정
    4. 바인드
    5. 클라이언트로 부터 데이터 수신(recv from)
    6. 응답 보내기(optional)
    7. 반복 루프
    8. close socket

*/

/*

    OSI 계층	담당 범위	코드 상 해당 부분
    7. 응용 계층	사용자 입력/출력, 응용 로직	std::cin.getline, std::cout, printf, 메시지 내용 자체
    6. 표현 계층	데이터 포맷 변환, 인코딩	(이 예제에서는 없음) – 문자열 그대로 전송
    5. 세션 계층	연결 유지, 세션 제어	(UDP는 무연결) → 세션 계층 기능 없음
    4. 전송 계층	송수신 포트, 오류 확인, 흐름 제어	sendto, recvfrom, htons(PORT) 등 UDP 소켓 관련 전송 API
    3. 네트워크 계층	IP 주소 지정, 라우팅	inet_pton, server_addr.sin_addr, AF_INET, sockaddr_in
    2. 데이터 링크 계층	프레임 구성, MAC 주소 처리	운영체제/네트워크 드라이버가 처리 (코드에는 없음)
    1. 물리 계층	실제 전기 신호 전송	네트워크 장치가 담당 (코드에는 없음)
    
*/

#include <iostream>
#include <cstring>
#include <arpa/inet.h>
#include <unistd.h>

#define PORT 8888
#define BUFFER_SIZE 1024

int main(){

    // 🌎 1. 구조체, 디스크럽터들 선언 

    int sockfd; 
    // 소켓을 담을 파일 디스크럽터
    // 리눅스에서는 모든 것을 '파일로' 관리한다. 
    // 그렇기 때문에 소켓도 파일디스크립터로 관리한다고 생각

    struct sockaddr_in server_addr, client_addr;
    //서버와 클라이언트의 주소를 담기 위해서
    // 비 연결적인 특성때문에, 오히려 구현할때, 두개 다 고려 해주어야 함.
    // 그래서 처음 부터 두개 다 선언해놓고 시작 함.

    // 🌎 2. 소켓 생성

    sockfd = socket(AF_INET, SOCK_DGRAM, 0);
    //SOCK_DGRAM : UDP 소켓을 생성하기 위한 매크로
    //SOCK_STREAM : TCP 소켓을 생성하기 위한 매크로

    if(sockfd < 0){
        perror("socket creation failed");
        return 1;
    }

    // 🌎 3. 서버 주소 설정

    memset(&server_addr, 0, sizeof(server_addr)); //    주소 구조체 초기화
    // 왜 memory set을 할까? 
    // 1. server_addr 이라는 구조체를 선언하고, 그애에 대해서 메모리의 주소를 부여해주는 것 같다.
    // 사이즈 지정은 sizeof로 해주고 0은? 모든 바이트를 0으로 채우는 것 <- 쓰레기 값 방지 해줌 .
    // 그럼 왜 socket은 memset을 안해줄까 ?    
    // ->이건 마치 우리가 int a = bfs(); 이런식으로 하듯이. 굳이 초기화 해줄 필요가 없음. 
    // 그럼 여기서 또.. 왜 memset을 해주지? -> 단순히 파일디스크럽터의 인트 개념이 아니라. 여러 구조체가 
    // 정의 된 형태이므로 memset을 안 해줄 경우 쓰레기 값이 들어가 있을 수 있음. 

    server_addr.sin_family = AF_INET;   //IPv4 주소 체계
    server_addr.sin_addr.s_addr = INADDR_ANY; //모든 IP 주소에서 수신
    server_addr.sin_port = htons(PORT); //포트 번호 설정 
    // htons : 호스트 바이트 순서를 네트워크 바이트 순서로 변환
    // 호스트도 바이트 순서가 있겠지. 근데 네트워크 바이트 순서랑 안꼬이게 변환함
    // 받을때는 noths();

    // 🌎 4. 바인드

    if(bind(sockfd, (const struct sockaddr *)&server_addr, sizeof(server_addr)) < 0){
        perror("bind failed");
        close(sockfd);
        return 1;
    }

    // 🌎 5. 메시지 수신 및 응답(무한 루프)

    char buffer[BUFFER_SIZE];               // 이제 메시지를 받야아 하니 버퍼를 생성함. 
    socklen_t len = sizeof(client_addr);    // 클라이언트 주소 길이 근데 왜 socklen_t에 담을까? 던져줄때 형식을 맞춰줘야하나?

    while(1){
        // 클라이언트로부터 데이터 수신
        int n = recvfrom(sockfd, buffer, BUFFER_SIZE -1 , 0, (struct sockaddr *)&client_addr, &len );

        if( n < 0 ){
            perror("recvfrom failed");
            continue;
        }

        buffer[n] = '\0'; // 마지막에 0을 써줘라. 
        printf("클라이언트로 부터 받은 메시지 : %s\n", buffer);
        

        const char *response = "서버 응답 : 메시지 잘 받음!" ;
        sendto(sockfd,  response, strlen(response) , 0 ,  (struct sockaddr *)&client_addr, len);
    }

    // 🌎 8. 소켓 닫기( 사실상 루프 안 끝나면 실행 안됨)

    close(sockfd);

    return 0;


}

