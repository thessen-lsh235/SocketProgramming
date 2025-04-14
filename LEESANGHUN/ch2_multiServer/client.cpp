#include "common.hpp"

#include <iostream>
#include <thread>
#include <unistd.h>
#include <arpa/inet.h>
#include <cstring>

void RecvThread(int socket)
{
	while (true)
	{
		Message msg;
		ssize_t bytes = recv(socket, &msg, sizeof(msg), 0);
		if (bytes <= 0)
		{
			std::cout << "Disconnected from server." << std::endl;
			break;
		}
		std::cout << "[Recv] From server: ID=" << msg.id << ", Text=" << msg.text << std::endl;
	}
}

void SendThread(int socket)
{
	while (true)
	{
		Message msg;
		msg.id = 1;

		std::string input;
		std::getline(std::cin, input);
		std::snprintf(msg.text, sizeof(msg.text), "%s", input.c_str());

		send(socket, &msg, sizeof(msg), 0);
		std::cout << "[Send] To server: ID=" << msg.id << ", Text=" << msg.text << std::endl;
	}
}

int main()
{
	int sock = socket(AF_INET, SOCK_STREAM, 0);

	sockaddr_in server_addr{};
	server_addr.sin_family = AF_INET;
	server_addr.sin_port = htons(kPort);
	inet_pton(AF_INET, "127.0.0.1", &server_addr.sin_addr);

	if (connect(sock, reinterpret_cast<sockaddr *>(&server_addr), sizeof(server_addr)) < 0)
	{
		std::cerr << "Connection failed." << std::endl;
		return 1;
	}

	std::cout << "Connected to server." << std::endl;

	std::thread recv_thread(RecvThread, sock);
	std::thread send_thread(SendThread, sock);

	recv_thread.join();
	send_thread.join();

	close(sock);
	return 0;
}
