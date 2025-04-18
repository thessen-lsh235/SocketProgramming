cmake_minimum_required(VERSION 3.10)
project(LEEDONGHYUK)

set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin)

# ch1_echo/ 안에 있는 .cpp 전부 가져오기
file(GLOB CH1_ECHO_SOURCES CONFIGURE_DEPENDS "ch1_echo/*.cpp")

# 각 cpp를 독립된 실행파일로 만들기
foreach(src ${CH1_ECHO_SOURCES})
    get_filename_component(exec_name ${src} NAME_WE)
    add_executable(${exec_name} ${src})
endforeach()
