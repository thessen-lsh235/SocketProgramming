/*
    this is udp_client
    1. 구조체 및 디스크립터 선언
    2. 소켓 생성
    3. 서버 주소 설정
    4. 메시지 입력 받기
    5. 서버로 메시지 전송(sendto)
    6. 서버로부터 응답 수신 (recvfrom)
    7. 결과 출력
    8. 소켓 닫기
*/

#include <iostream>
#include <cstring>
#include <arpa/inet.h>
#include <unistd.h>

#define SERVER_PORT 8888
#define BUFFER_SIZE 1024
#define ADDRESS "127.0.0.1"
int main(){

    /*
    OSI 계층	담당 범위	코드 상 해당 부분
    7. 응용 계층	사용자 입력/출력, 응용 로직	std::cin.getline, std::cout, prheintf, 메시지 내용 자체
    6. 표현 계층	데이터 포맷 변환, 인코딩	(이 예제에서는 없음) – 문자열 그대로 전송
    5. 세션 계층	연결 유지, 세션 제어	(UDP는 무연결) → 세션 계층 기능 없음
    4. 전송 계층	송수신 포트, 오류 확인, 흐름 제어	sendto, recvfrom, htons(PORT) 등 UDP 소켓 관련 전송 API
    3. 네트워크 계층	IP 주소 지정, 라우팅	inet_pton, server_addr.sin_addr, AF_INET, sockaddr_in
    2. 데이터 링크 계층	프레임 구성, MAC 주소 처리	운영체제/네트워크 드라이버가 처리 (코드에는 없음)
    1. 물리 계층	실제 전기 신호 전송	네트워크 장치가 담당 (코드에는 없음)
    */
    

    //  🌎 1. 디스크립터와 구조체 선언

    int sockfd;
    struct sockaddr_in server_addr; 
    char buffer[BUFFER_SIZE];

    
    // 🌎 2. 소켓 생성 (UDP 방식)

    sockfd = socket(AF_INET, SOCK_DGRAM,0); //IPv4(AF_INET), UDP(SOCK_DGRAM)
    if(sockfd < 0){ // 소켓 생성 실패
        perror("socket creation failed");   // 에러 메시지 출력
        return 1; 
    }

    
    // 🌎 3. 서버 주소 설정 

    memset(&server_addr, 0, sizeof(server_addr)); 
    server_addr.sin_family = AF_INET;              
    server_addr.sin_port = htons(SERVER_PORT);

    // IP 주소 루프백(LocalHost) 로 설정 : localhost-> 내 자신의 주소 
    if(inet_pton(AF_INET, ADDRESS, &server_addr.sin_addr) <= 0 ) {
        perror("invalid server address");
        return 1;
        // 왜 클라이언트가 ip주소를 루프백?으로 설정할까 루프백은 뭘까 :
        // 127.0.0.1로 보내면 패킷은 밖으로 나가지 않고 OS 내부에서 바로 반송됨
    }

    // 🌎 4. 사용자 입력 받기 

    std::cout << "서버에 보낼 메시지를 입력하세요: "; // 입력 안내 출력
    std::cin.getline(buffer, BUFFER_SIZE); //사용자로부터 한 줄 입력 받아 buffer에 저장.
 

    // 🌎 5. 서버에 메시지 전송

    sendto(sockfd, buffer, strlen(buffer), 0, (struct sockaddr *)&server_addr, sizeof(server_addr));
    // socket 정보와 버퍼에 담아서 보냄
    // 0 <- flag인데 이건 뭘까? MSG_DONTWAIT , MSG_CONFIRM , MSG_MORE, MSG_NOSIGANL;


    // 🌎 6. 서버로부터 응답 수신

    socklen_t len = sizeof(server_addr); // 서버 주소 길이 설정
    int n = recvfrom( sockfd, buffer, BUFFER_SIZE - 1, 0, (struct sockaddr *)&server_addr, &len );
    // 서버로부터 메시지를 수신
    // 서버의 응답이 담김

    if(n < 0){
        perror("recvfrom failed!!"); //에러 메시지 출력
        close(sockfd);
        return 1;
    }

    // 🌎 7. 소켓 닫기.

    close(sockfd); // 열린 소켓 자원 반납 (연결 종료)

    return 0;
}
