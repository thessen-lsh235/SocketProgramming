cmake_minimum_required(VERSION 3.15)
project(multiServer)

set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# 공통 헤더 포함
include_directories(${CMAKE_CURRENT_SOURCE_DIR})

# 서버 빌드
add_executable(multi_server server.cpp)

# 클라이언트 빌드
add_executable(multi_client client.cpp)

