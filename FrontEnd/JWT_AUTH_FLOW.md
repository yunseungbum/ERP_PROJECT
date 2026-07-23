# JWT Access Token 흐름

## 현재 구현 범위

로그인 성공 시 백엔드는 30분 동안 유효한 JWT Access Token을 발급한다.

- 로그인 응답: `userId`, `name`, `roles`, `accessToken`, `expiresAt`
- 프론트엔드 보관 위치: React 애플리케이션 메모리
- API 전달 방식: `Authorization: Bearer {accessToken}`
- 로그아웃: 메모리에 저장된 Access Token 제거
- 백엔드 검증: 서명, 발급자, 대상, 만료 시간
- 보호 API 확인용 경로: `GET /api/auth/me`

## 보안 규칙

- JWT 서명키는 코드나 `appsettings.json`에 저장하지 않는다.
- 서명키는 백엔드 프로젝트의 User Secrets `Jwt:SigningKey`에만 저장한다.
- 토큰에는 비밀번호와 비밀번호 해시를 넣지 않는다.
- 화면의 버튼 숨김은 사용자 편의 기능이다.
- 실제 데이터 보호는 백엔드의 `[Authorize]` 및 역할 권한 검증이 담당한다.

## 현재 의도된 제한

Access Token은 메모리에만 저장하므로 브라우저를 새로고침하면 로그인 정보가 사라진다.
Refresh Token, 새로고침 후 로그인 유지, 서버 로그아웃은 다음 인증 단계에서 구현한다.

## 공개 체험 계정

- 아이디: `guest`
- 비밀번호: `1234`
- 역할 코드: `Member`
- 권한: 일반 팀원과 동일한 읽기 전용

공개 체험 계정의 비밀번호는 로그인 화면에 안내하지만 DB에는 평문이 아닌
ASP.NET `PasswordHasher` 해시만 저장한다. 운영진 역할은 부여하지 않는다.
