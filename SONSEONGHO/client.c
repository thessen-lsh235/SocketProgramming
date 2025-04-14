#define _CRT_SECURE_NO_WARNINGS
#define _WINSOCK_DEPRECATED_NO_WARNINGS
#include <stdio.h>
#include <winsock2.h>
#include <string.h>
#pragma comment(lib, "ws2_32.lib")
#define BUF_SIZE 100

int main(int argc, char* argv[])
{
	WSADATA wsaData;
	SOCKET client;
	SOCKADDR_IN server_addr;
	char msg_buf[BUF_SIZE];

	WSAStartup(MAKEWORD(2, 2), &wsaData);

	// 소켓 생성 및 주소 초기화 설정 및 바인딩
	client = socket(PF_INET, SOCK_STREAM, 0);
	memset(&server_addr, 0, sizeof(server_addr));
	server_addr.sin_family = AF_INET;
	server_addr.sin_addr.s_addr = inet_addr(argv[1]);
	server_addr.sin_port = htons(atoi(argv[2]));

	// 서버에 연결 시도
	connect(client, (struct sockaddr*)&server_addr, sizeof(server_addr));

	// 연결 종료(q 입력) 할 때까지 반복적으로 메세지 전송
	printf("서버에 보낼 메세지를 입력하세요 (종료: q)\n");
	while (1)
	{
		scanf(" %s", msg_buf);
		// 연결 종료 시 socket 닫기
		if (strcmp(msg_buf, "q")==0)
		{
			printf("연결 종료\n");
			closesocket(client);
			break;
		}
		send(client, msg_buf, strlen(msg_buf), 0);
	}
	WSACleanup();
}
