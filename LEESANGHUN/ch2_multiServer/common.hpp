#pragma once

#include <cstdint>
#include <string>
#include <cstring>

constexpr int kBufferSize = 1024;
constexpr int kPort = 9000;

struct Message
{
	uint32_t id;
	char text[256];

	Message() : id(0)
	{
		std::memset(text, 0, sizeof(text));
	}
};
