#include <stdio.h>
#include <winsock2.h>
#pragma comment(lib, "ws2_32.lib")
#define BUF_SIZE 100

int main(int argc, char* argv[])
{
	WSADATA wsaData;
	SOCKET server, client;
	SOCKADDR_IN server_addr, client_addr;
	char msg_buf[BUF_SIZE];
	
	WSAStartup(MAKEWORD(2, 2), &wsaData);

	// 소켓 생성 및 주소 초기화 설정 및 바인딩
	server = socket(PF_INET, SOCK_STREAM, 0);
	memset(&server_addr, 0, sizeof(server_addr));
	server_addr.sin_family = AF_INET;
	server_addr.sin_addr.s_addr = htonl(INADDR_ANY);
	server_addr.sin_port = htons(atoi(argv[1]));
	bind(server, &server_addr, sizeof(server_addr));

	// listen으로 소켓 listen
	listen(server, 5);
	// client가 종료되어도 server는 종료되지않고 다른 client의 연락을 받기 위해 반복문 선언
	while (1)
	{
		printf("start listening...\n");
		int clnt_addr_sz = sizeof(client_addr);
		client = accept(server, &client_addr, &clnt_addr_sz);
		printf("client %d번 연결\n", (int)client);
		// client는 사용자가 종료하기 전까지 server로 메세지를 전달
		while (1)
		{
			int len = recv(client, msg_buf, BUF_SIZE-1, 0);
			if (len == 0)
				break;
			msg_buf[len] = '\0';
			printf("수신된 메세지: %s\n", msg_buf);
		}
		// 메세지 전송이 종료되면 클라이언트 연결 종료
		printf("%d번 클라이언트 연결 종료\n\n", (int)client);
		closesocket(client);
	}

	// 소켓 닫기
	closesocket(server);
	WSACleanup();
}
