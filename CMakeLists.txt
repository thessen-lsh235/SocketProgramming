cmake_minimum_required(VERSION 3.10)
project(SocketProgramming)

set(CMAKE_CXX_STANDARD 17)

# 제외할 디렉토리
set(EXCLUDED_DIR "SEONGYUNSEOK")

# 현재 디렉토리 내 모든 하위 디렉토리 가져오기
file(GLOB CHILD_DIRS RELATIVE ${CMAKE_SOURCE_DIR} */)

# 필터링 후 빌드 타겟 생성
foreach(dir ${CHILD_DIRS})
    if(NOT dir STREQUAL ${EXCLUDED_DIR})
        if(EXISTS "${dir}/client.cpp")
            add_executable(${dir}_client ${dir}/client.cpp)
        endif()
        if(EXISTS "${dir}/server.cpp")
            add_executable(${dir}_server ${dir}/server.cpp)
        endif()
    endif()
endforeach()

