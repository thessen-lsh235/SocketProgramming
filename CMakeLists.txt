cmake_minimum_required(VERSION 3.10)
project(SocketProgramming)

set(CMAKE_CXX_STANDARD 17)

# 제외할 디렉토리
set(EXCLUDED_DIR "SEONGYUNSEOK")

# 현재 디렉토리 내 모든 하위 디렉토리 가져오기
file(GLOB CHILD_DIRS RELATIVE ${CMAKE_SOURCE_DIR} */)

foreach(dir ${CHILD_DIRS})
    if(NOT dir STREQUAL ${EXCLUDED_DIR})
        if(EXISTS "${CMAKE_SOURCE_DIR}/${dir}/CMakeLists.txt")
            add_subdirectory(${dir})
        endif()
    endif()
endforeach()

