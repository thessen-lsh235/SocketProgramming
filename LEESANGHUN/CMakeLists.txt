cmake_minimum_required(VERSION 3.15)
project(SocketProgramming)

# 공통 C++ 표준 설정
set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# 하위 디렉토리 추가
add_subdirectory(ch1_1_1_echoServer/cppSocket)
add_subdirectory(ch2_multiServer)
