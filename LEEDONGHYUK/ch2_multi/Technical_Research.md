## Techincal_Research

---

# ✅ 구조체 기반 메시지란?

- 단순히 `char buffer[1024]`로 문자열만 주고받는 것이 아님
- `id`, `length`, `payload` 등을 포함한 `Message` 구조체 단위로 송수신

# ✅ message를 구조체로 담아 던질때 문제점?

- > `send`가 레거시 함수이기 때문에 `send(sock, (char*)&a, sizeof(a), 0)`;
이런식으로 보낼때 알아서 못받아 먹음. 구조체는 여러가지 변수가 있기에

**→일단 패딩에서 문제가 됨(int -> char 순이랑 char->int순이랑 달라 질수가 있다.)**

- 내가 알고 있는 기존의 구조체
->그냥 `struct` 안에 `int a`와 `string b`를 받고 그것을 던지면 된다고 생각
- 현실적으로 고려해야 할 점 →send 가 레거시 스타일이기에,
**1.** 개발자가 바이트 단위 전송 범위를 정확히 계산.
**2.** 구조체 내부 padding도 고려해주어야함.
**3.** 유동길이, 몇 바이트까지 `msg[]`를 읽어야 할지 모름
**4.** `recv`도 마찬가지
- > 이런것들을 고려해서 나온것이 TLV(1Byte type, 4Byte Legnth, + Payload, UART에서 사용)
- > 내가 구현해야 할 것은 TLV까지 가보기 보다

### 1. Length-prefixed 구조

```cpp
struct Packet{
int length;
char data[MAXBUFFER];
}
```

### 2. Endianness (바이트 순서) 처리 → `ntohl()`, `htonl()`

- int size;
char msg[]; // 길이가 정해지지 않은 payload
이 경우에는 동적할당 해서 구조체의 길이도 가변적이다.

---

# ✅ 비동기/동기  차이점

- 동기 (blocking)	함수가 리턴될 때까지 CPU가 기다림 (recv에서 멈춤)
- 비동기 (non-blocking)	함수는 즉시 리턴, CPU는 계속 다른 일 하다가 이벤트 오면 처리

이해한 바 : TCP는 listen 후 send recv하면서 계속 기다려야하는 것(즉 연결 지향적인 느낌이라면), udp는 원할때 포트번호 맞춰서 던지는 놈임.
이런식의 udp장점을 tcp에서 끌어다가 쓰려는것이 아닐까?

---

# ✅ 기존 TCP Client-Server는 다중 처리 불가능

- 이를 해결하기 위한 3가지 방식 존재

---

## 1. 통신시간 줄이기 (Non-blocking IO, timeout)

- 병렬 처리 ❌
- 빠르게 처리 후 다시 대기 상태로 복귀
- 구조는 단순하지만 확장성 낮음

---

## 2. 쓰레드 생성 (pthread, fork)

- 각각의 작업을 동시에 처리하는 병렬 구조

### pthread

- 같은 프로세스 내 여러 스레드 생성
- 예: `pthread_create()`, `pthread_join()`

### fork

- 현재 프로세스를 복사 (쌍둥이 프로세스)
- 서로 완전히 독립된 실행 공간

---

## 3. Socket 입출력 모드 사용 (select, poll)

- 이벤트 감시 기반 처리 모델
- 여러 소켓을 감시하다가, 데이터 있는 소켓만 처리
- 예: `select(fd+1, &readfds, NULL, NULL, &timeout)`

---

# ✅ 기본 처리 로직 예시

1. `pthread_create(&tid, nullptr, recv_handler, info);` → 쓰레드 생성
2. `int len = recv(sock, &m, sizeof(m), 0);` → 메시지 수신
3. `client_map[id] = client_sock;` → 클라이언트 등록

> 흐름: accept → recv(ID) → 쓰레드 생성 → recv_handler 무한 수신
> 

---

# ✅ 왜 프로세스가 아니라 쓰레드를 쓸까?

- **fork**
    - 장점: 안정성, 독립성
    - 단점: 리소스 오버헤드 큼
- **pthread**
    - 장점: 빠르고 효율적
    - 단점: 동기화 관리 어려움, 충돌 가능성 있음

---

# ✅ 리눅스와 쓰레드

- **Process**는 `/proc` 통해 파일처럼 접근 가능
- *Thread(pthread_t)**는 **식별자**일 뿐, I/O 객체가 아님 → 파일로 관리되지 않음

---

# ✅ 쓰레드 수 많아지면 어떻게?

1. **쓰레드 개수 제한 + task queue**
    
    → 대부분 이 방식으로 충분함
    
2. **프로세스 분산 (fork)**
    
    → 아예 프로세스를 나누어 병렬 처리
    
3. **멀티 머신 로드밸런싱**
    
    → 서버 자체를 여러 대로 분산 (대형 서비스에 적합)
    

---

# ✅ `listen(tcp_socket, 3)` 의미

- **Backlog queue** 최대 3개까지 허용
- 초과 요청은 커널이 **drop**할 수 있음 → `accept()`에 도달 못 함

---

# ✅ Mutex & Condition Variable

- Mutex는 **동시 접근 방지용**이지 **순서 보장용** 아님
- 순서 제어 필요 시 → **조건 변수 사용**
    - 선언: `pthread_cond_t`
    - 초기화: `pthread_cond_init`
    - 대기: `pthread_cond_wait()`
    - 알림: `pthread_cond_signal()` / `pthread_cond_broadcast()`

---

# ✅ 넌블로킹 소켓 & `fcntl`

```cpp
cpp
복사편집
int flag = 1;
fcntl(sockfd, F_SETFL, O_NONBLOCK); // 비블로킹 설정
```

- **블로킹 소켓**: I/O 없으면 CPU 점유 X
- **넌블로킹 소켓**: 데이터 없으면 루프 반복 → CPU 점유율 증가 가능
    
    → 해결: `select`, `epoll` 등 사용
    

---

# ✅ `select()`

```cpp
cpp
복사편집
int select(int nfds, fd_set *readfds, fd_set *writefds, fd_set *exceptfds, struct timeval *timeout);
```

- `timeout == NULL`: 무한 대기
- `{0, 0}`: 즉시 리턴
- 양수: 대기 시간 설정
- **1 thread : N socket** 모델 구성 가능
- `readfds` → `recv()`
- `writefds` → `send()`