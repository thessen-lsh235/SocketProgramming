# cppSocket 하위 디렉토리용
cmake_minimum_required(VERSION 3.15)
project(cppSocket)

set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# 서버 빌드 (기본 버전)
add_executable(echo_server server.cpp)

# 추가 서버 버전 빌드
# add_executable(echo_server_v2 default_socket_server/server2.cpp)
# add_executable(echo_server_v3 default_socket_server/server3.cpp)

# 클라이언트 빌드
add_executable(echo_client client.cpp)

