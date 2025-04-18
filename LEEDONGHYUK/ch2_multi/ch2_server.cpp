// server.cpp

#include <iostream>
#include <unistd.h>
#include <arpa/inet.h>
#include <cstring>
#include <string>
#include <vector>
#include <algorithm>
#include <map>
#include <sstream>


struct Message {
    int id;
    int length;
    char data[1024];
};

std::map<int, int> client_map;// : 소켓 fd ↔ 닉네임

int c_id = 1; //

const int SERVER_ID = -2; //서버의 아이디
const int ID_REQUEST = -1; //ID_REQUEST의 아이디


int main(){
    
    int server_fd = socket(AF_INET, SOCK_STREAM, 0);

    int opt = 1;
    setsockopt(server_fd, SOL_SOCKET, SO_REUSEADDR, &opt, sizeof(opt));
    //서버 주소 구조체 설정
    sockaddr_in serv_addr{};
    serv_addr.sin_family = AF_INET;
    serv_addr.sin_addr.s_addr =INADDR_ANY;
    serv_addr.sin_port = htons(8888);

    //소켓에 주소 바인드
    bind(server_fd, (sockaddr*)&serv_addr, sizeof(serv_addr));

    //소켓이 수신 대기 상태로 전환 됨. 3개까지만 연결 가능(편의상).
    listen(server_fd, 3);
    std::cout << "wating for client....(listen finished)\n";
    std::cout << "Usage: type '<id> <message>' (e.g., '1 hello') to send to a client (IDs 1–9 only).\n";
    
    std::vector<int> client_fdset;
    //왜 얘는 vector로 쓸까? fd_set으로 안쓰고 -> 순회돌기도 불편하고, 단점이 많다고함
    
    //fd_set 선언, read_fdset, 
    //read_fdset :  select()호출하기 위한 임시 복사본이다.
    fd_set read_fdset; 

    while(true){
    /*
        ** FD관련 함수들 
        FD_ZERO(fd_set set) set을 0으로 초기화
        FD_SET(sockfd, &readfds); fd를 set에 추가한다. ->감시 대상에 포함하겠다. 
        FD_CLR(sockfd, &readfds); fd를 set에서 제거
        FD_ISSET(int fd, fd_set set) : 지정한 fd가 set에 포함인지 확인. find
    */

        FD_ZERO(&read_fdset);
        FD_SET(0, &read_fdset); //여기서 이 서버가 유저의 입력(stdin)을 받을 준비가 된 것임.
        FD_SET(server_fd, &read_fdset);
        //server_fd가 왜자꾸 튀어 나올까? : 새로운 클라이언트가 접속하려고 할때,select()를 통해서 감지
        //server_fd는 현관문, 여기를 감시하면 누가 들어올지 체크
        //client_fd는 집안에 있는 손님들. + 0을 체크하는것은 stdin을 체크하는것

        int max_fd = server_fd;

        for(int fd : client_fdset){
            FD_SET(fd, &read_fdset); //기존 클라이언트 소켓 감시 추가
            if (fd > max_fd) max_fd = fd;
        }
        
        //지금 client_fd는 accept 된놈이니까 3이상의 값을 가지고 잇을 것임
        //이것을 통해 read_fdset에 등록을 한것이고, 우리는 이것을 select로 감시할것임.
        
    /*
        select -> 이벤트 감지 
        max_fd+1 -> 검사할 최대 fd +1
        &read_fdset -> 읽기 검사 할 set
        nullptr -> 쓰기 감시 집합
        nullptr -> 예외 감시
        nullptr -> 타임아웃(nullptr이면 무한대기, {0,0}이면 바로끝내기?)
        이함수에서 쓰기와 예외처리는 안했는데 두개는 특수한 경우가 아니면 잘 사용하지 않는다.

    */
        int activity = select(max_fd +1, &read_fdset, nullptr, nullptr, nullptr);
        //왜 max_fd+1까지 인지 궁금했었는데, max_fd 는 i<N같은 느낌인듯, max_fd까지 검사하기 위해서는 
        //인덱스를 max_fd+1까지한다고함.
        if(activity<0){
            std::cerr << "select error\n" ;
            break;
        }
    /*
        위 시퀀스가 끝난 후 select는 read_fdset 를 검사하고, 이벤트 발생한놈만 다시 살려서 read_fdset에 보낸다.
        그 뒤 어떤 이벤트가 있었는지 fd_isset으로 확인함. 
    */

         //새로운 클라이언트가 연결 되었으면 FD_ISSET= TRUE
       
         if(FD_ISSET(server_fd, &read_fdset)){
                int new_fd = accept(server_fd, nullptr, nullptr);
                client_fdset.push_back(new_fd);
                //client_fdset에 푸시해줌(집안의 손님들에 추가하기기)
                std::cout <<"New client connected fd:" << new_fd << "\n";
         }
         
        //이건 서버가 전송하려는 것임. cin으로 입력
        if(FD_ISSET(0,&read_fdset)){
            //input받고
            std::string input;
            std::getline(std::cin, input);
            
            //targetid, stringstream시작.
            int target_id;
            std::string msg;
            std::istringstream iss(input);
            if(!(iss>>target_id) || !std::getline(iss, msg) || client_map.count(target_id)==0){
                // 정수가 아니면 // 공백 이후에 메시지가 없거나 getline 실패시 // id가 클라이언트맵에 없으면면
                std::cout << "Wrong format or unknwon ID" << std::endl;
            }
            else{
                if(!msg.empty() && msg[0] == ' ') msg = msg.substr(1);
                Message m;
                m.id = SERVER_ID;
                m.length = std::min((int)msg.length(), (int)sizeof(m.data) - 1);
                //여기서 메시지가 잘릴거 같긴한다. 
                strncpy(m.data, msg.c_str(), m.length);
                m.data[m.length] = '\0';
                    send(client_map[target_id],&m,sizeof(m), 0);
            }
            
        }

        std::vector<int> updated_fds;

        //이 행위 자체는 cliend_fdset에서 죽은놈과 살아있는놈 구별하기
        for(int fd: client_fdset){
            if(FD_ISSET(fd, &read_fdset)){
                Message m;
                //recv 잘되었는지를 검사함.
                int len = recv(fd,&m, sizeof(m), 0 );
                //만약 죽은놈이면 지워줌.
                if(len<=0){
                    std::cout <<"Client disconnected: " << fd <<"\n";
                    close(fd);
                    for(auto it = client_map.begin(); it != client_map.end(); ++it){
                        if(fd==it->second){
                            client_map.erase(it);
                            break;
                        }
                    }
                    continue;
                }
                //살아있는놈이면 메시지 받고 냅둠.
                else{
                    //m을 보낸 클라이언트의  아이디가 초기화 되지 않은 상태라면, 
                    if(m.id == ID_REQUEST){
                        m.id = c_id;
                        client_map[c_id] = fd;
                        m.data[m.length] = '\0';
                        std::cout << "[Client " << m.id << "] : " << m.data 
                        << " (id=" << m.id << ", len=" << m.length << ")\n";

                        std::string confirmMsg = "First connection! Your ID is created. ID : ";
                        confirmMsg += std::to_string(c_id++);
                        strncpy(m.data, confirmMsg.c_str(), sizeof(m.data) - 1);
                        m.data[sizeof(m.data) - 1] = '\0';  // null-terminate 안전 처리
                        m.length = confirmMsg.length();  // 전송 메시지 길이 설정
                        send(fd, &m,sizeof(m), 0);
                        
                    }
                    else{
                    m.data[m.length] = 0;
                    std::cout << "[Client " << m.id << "] : " << m.data 
                    << " (id=" << m.id << ", len=" << m.length << ")\n";
                    std::cout.flush();
                    }
                }
            }
            updated_fds.push_back(fd);
        }
        //살아있는놈으로 갱신. 
        client_fdset = std::move(updated_fds);
    }
    
    for (int fd : client_fdset) close(fd);
    close(server_fd);

}