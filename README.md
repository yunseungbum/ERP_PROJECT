# Buddy FC ERP

Buddy FC 운영진이 회원, 회비, 재정, 경기 일정과 물품을 관리하기 위한 ERP 프로젝트입니다.

## 프로젝트 구조

```text
ERP_PROJECT/
├─ FrontEnd/   # React + TypeScript + Vite
└─ BackEnd/    # ASP.NET Core Web API
```

## 개발 환경 실행

### 백엔드

```powershell
cd BackEnd\BuddyErp.Api
dotnet run --launch-profile http
```

백엔드 주소: `http://localhost:5080`

### 프론트엔드

```powershell
cd FrontEnd
npm.cmd install
npm.cmd run dev
```

프론트엔드 주소: `http://localhost:5173`

두 서버를 모두 실행한 뒤 프론트 화면의 **API 연결 확인** 버튼을 누르면
`GET /api/health` 요청으로 연결 상태를 확인할 수 있습니다.

## 현재 개발 단계

- [x] React 프로젝트 생성
- [x] ASP.NET Core Web API 프로젝트 생성
- [x] CORS 개발 정책 설정
- [x] Health API 및 프론트 연결 화면
- [ ] 로그인 API 명세
- [ ] 사용자·역할 데이터 모델
- [ ] Access Token·Refresh Token 인증
- [ ] React 로그인 화면

