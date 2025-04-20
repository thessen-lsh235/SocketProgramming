#pragma once
#include <cstring>

#define MAX_MSG_LEN 256

struct Message {
    int id; // 목적지 클라이언트 ID
    char content[MAX_MSG_LEN];

    Message() : id(0) {
        memset(content, 0, MAX_MSG_LEN);
    }
};
