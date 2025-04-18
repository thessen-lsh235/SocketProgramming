cmake_minimum_required(VERSION 3.10)
project(SocketProgramming)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin)

# 하위 디렉토리 개별 등록

add_executable(ch2_client_select ch2_client_select.cpp)
add_executable(ch2_server_select ch2_server_select.cpp)
