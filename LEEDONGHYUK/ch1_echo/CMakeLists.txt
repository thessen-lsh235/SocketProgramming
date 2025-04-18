cmake_minimum_required(VERSION 3.10)
project(SocketProgramming)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin)

# 하위 디렉토리 개별 등록
add_executable(01_tcp_client_Basic 01_tcp_client_Basic.cpp)
add_executable(01_tcp_server_Basic 01_tcp_server_Basic.cpp)
add_executable(04_tcp_server_final 04_tcp_server_final.cpp)
add_executable(04_tcp_client1_final 04_tcp_client1_final.cpp)
