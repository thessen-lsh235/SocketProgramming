#include "common.hpp"

#include <iostream>
#include <thread>
#include <mutex>
#include <vector>
#include <map>
#include <atomic>
#include <sstream>
#include <netinet/in.h>
#include <unistd.h>
#include <arpa/inet.h>

struct ClientInfo
{
	int socket_fd;
	sockaddr_in addr;
};

std::mutex clients_mutex;
std::map<int, ClientInfo> clients; // client_id -> ClientInfo
std::atomic<int> client_id_seq{1};

void HandleRecv(int client_id, int client_socket)
{
	while (true)
	{
		Message msg;
		ssize_t bytes = recv(client_socket, &msg, sizeof(msg), 0);
		if (bytes <= 0)
		{
			std::lock_guard<std::mutex> lock(clients_mutex);
			std::cout << "Client " << client_id << " disconnected.\n";
			clients.erase(client_id);
			close(client_socket);
			break;
		}
		std::cout << "[Recv] Client " << client_id << ": ID=" << msg.id << ", Text=" << msg.text << std::endl;
	}
}

void HandleAdminSend()
{
	while (true)
	{
		std::string input_line;
		std::getline(std::cin, input_line);

		if (input_line.empty())
			continue;

		std::istringstream iss(input_line);
		std::string target;
		std::string message_text;
		iss >> target;
		std::getline(iss, message_text);
		if (!message_text.empty() && message_text.front() == ' ')
		{
			message_text.erase(0, 1);
		}

		Message msg;
		msg.id = 0;
		std::snprintf(msg.text, sizeof(msg.text), "%s", message_text.c_str());

		std::lock_guard<std::mutex> lock(clients_mutex);

		if (target == "broadcast")
		{
			for (const auto &[id, info] : clients)
			{
				send(info.socket_fd, &msg, sizeof(msg), 0);
				std::cout << "[Send] Broadcast to Client " << id << ": " << msg.text << std::endl;
			}
		}
		else
		{
			int target_id = std::stoi(target);
			if (clients.find(target_id) != clients.end())
			{
				send(clients[target_id].socket_fd, &msg, sizeof(msg), 0);
				std::cout << "[Send] To Client " << target_id << ": " << msg.text << std::endl;
			}
			else
			{
				std::cout << "Client " << target_id << " not found.\n";
			}
		}
	}
}

int main()
{
	int server_socket = socket(AF_INET, SOCK_STREAM, 0);

	// [추가] 소켓 재사용 설정
	int opt = 1;
	setsockopt(server_socket, SOL_SOCKET, SO_REUSEADDR, &opt, sizeof(opt));
	setsockopt(server_socket, SOL_SOCKET, SO_REUSEPORT, &opt, sizeof(opt));

	sockaddr_in server_addr{};
	server_addr.sin_family = AF_INET;
	server_addr.sin_addr.s_addr = INADDR_ANY;
	server_addr.sin_port = htons(kPort);

	bind(server_socket, reinterpret_cast<sockaddr *>(&server_addr), sizeof(server_addr));
	listen(server_socket, SOMAXCONN);

	std::cout << "Server listening on port " << kPort << std::endl;

	std::thread admin_thread(HandleAdminSend);
	admin_thread.detach();

	while (true)
	{
		sockaddr_in client_addr{};
		socklen_t addr_len = sizeof(client_addr);
		int client_socket = accept(server_socket, reinterpret_cast<sockaddr *>(&client_addr), &addr_len);

		int new_client_id = client_id_seq++;

		{
			std::lock_guard<std::mutex> lock(clients_mutex);
			clients[new_client_id] = {client_socket, client_addr};
		}

		std::cout << "New client connected: ID=" << new_client_id
				  << ", socket=" << client_socket << std::endl;

		std::thread recv_thread(HandleRecv, new_client_id, client_socket);
		recv_thread.detach();
	}

	close(server_socket);
	return 0;
}
