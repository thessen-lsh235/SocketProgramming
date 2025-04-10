#include <iostream>
#include <cstring>
#include <sys/socket.h>
#include <arpa/inet.h>
#include <unistd.h>

#define PORT 8080
#define BUFFER_SIZE 1024




int main()
{
	int sock = 0;
	struct sockaddr_in serv_addr;
	char buffer[BUFFER_SIZE] = {0};

	// 소켓 생성
	if ((sock = socket(AF_INET, SOCK_STREAM, 0)) < 0)
	{
		perror("소켓 생성 실패");
		return -1;
	}

	// 서버 주소 설정
	serv_addr.sin_family = AF_INET;
	serv_addr.sin_port = htons(PORT);

	// 서버 IP 주소 변환 및 설정 (127.0.0.1은 로컬호스트)
	if (inet_pton(AF_INET, "127.0.0.1", &serv_addr.sin_addr) <= 0)
	{
		perror("유효하지 않은 주소 또는 주소 변환 실패");
		return -1;
	}

	// 서버에 연결
	if (connect(sock, (struct sockaddr *)&serv_addr, sizeof(serv_addr)) < 0)
	{
		perror("서버 연결 실패");
		return -1;
	}

	std::cout << "서버에 연결되었습니다!" << std::endl;

	while (true)
	{
		// 사용자 입력
		std::cout << "서버로 보낼 메시지를 입력하세요 (종료하려면 'exit' 입력): ";
		std::string input;
		std::getline(std::cin, input);

		// 종료 조건
		if (input == "exit")
		{
			std::cout << "클라이언트를 종료합니다." << std::endl;
			break;
		}

		// 서버로 메시지 전송
		send(sock, input.c_str(), input.size(), 0);
		std::cout << "서버로 메시지를 전송했습니다." << std::endl;

		// 서버로부터 응답 수신
		int valread = read(sock, buffer, BUFFER_SIZE);
		if (valread > 0)
		{
			std::cout << "서버로부터 받은 메시지: " << buffer << std::endl;
		}

		// 버퍼 초기화
		memset(buffer, 0, BUFFER_SIZE);
	}

	// 소켓 종료
	close(sock);

	return 0;
}
