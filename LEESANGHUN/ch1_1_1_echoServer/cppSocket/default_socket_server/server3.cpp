#include <iostream>
#include <cstring>
#include <sys/socket.h>
#include <netinet/in.h>
#include <unistd.h>

#define PORT 8080
#define BUFFER_SIZE 1024

int main()
{
	int server_fd, new_socket;
	struct sockaddr_in address;
	int addrlen = sizeof(address);
	char buffer[BUFFER_SIZE] = {0};

	// 소켓 생성
	/*
	 server_fd 실제로 서버에서 client에 대한 연결이나 데이터를 수신할 fd
	 SOCK_STREAM은 TCP연결을 뜻함
	 UDP일 경우에는 SOCK_DGRAM
	*/
	if ((server_fd = socket(AF_INET, SOCK_STREAM, 0)) == 0)
	{
		perror("소켓 생성 실패");
		exit(EXIT_FAILURE);
	}

	// 주소 설정
	/*
	설정할 ip에 대한 설정들
	*/
	address.sin_family = AF_INET;		  // IPV4
	address.sin_addr.s_addr = INADDR_ANY; // 0.0.0.0으로 open
	address.sin_port = htons(PORT);		  // port를 네트워크 바이트로 변환환

	// 소켓 바인딩
	/*
	생성한 소켓을 특정 주소와  포트에 연결하도록 한다.
	*/
	if (bind(server_fd, (struct sockaddr *)&address, sizeof(address)) < 0)
	{
		// 포트가 이미 사용중이거나, 소켓이 제대롯 생성되지 않았을때
		perror("바인딩 실패");
		close(server_fd);
		exit(EXIT_FAILURE);
	}

	// 연결 대기
	/*
	3 이 의미하는 것은 연결대기 요청을 최대 3개로 설정 하겠다는 뜻.
	*/
	if (listen(server_fd, 3) < 0)
	{
		perror("연결 대기 실패");
		close(server_fd);
		exit(EXIT_FAILURE);
	}

	std::cout << "서버가 포트 " << PORT << "에서 대기 중입니다..." << std::endl;

	while (true)
	{
		// 클라이언트 연결 수락
		if ((new_socket = accept(server_fd, (struct sockaddr *)&address, (socklen_t *)&addrlen)) < 0)
		{
			perror("연결 수락 실패");
			close(server_fd);
			exit(EXIT_FAILURE);
		}

		std::cout << "클라이언트가 연결되었습니다!" << std::endl;

		// 클라이언트와의 통신 루프
		while (true)
		{
			// 메시지 수신
			int valread = read(new_socket, buffer, BUFFER_SIZE);
			// 보통 0일 경우 종료
			if (valread <= 0) // 클라이언트가 연결을 종료하거나 에러 발생
			{
				std::cout << "클라이언트 연결 종료" << std::endl;
				break;
			}

			std::cout << "클라이언트로부터 받은 메시지: " << buffer << std::endl;

			// 메시지 전송 (에코 서버처럼 클라이언트에게 받은 메시지를 다시 보냄)
			send(new_socket, buffer, valread, 0);
			std::cout << "클라이언트로 메시지를 전송했습니다." << std::endl;

			// 버퍼 초기화
			memset(buffer, 0, BUFFER_SIZE);
		}

		// 클라이언트 소켓 종료
		close(new_socket);
	}

	close(server_fd);

	return 0;
}