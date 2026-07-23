# Data

이 폴더는 데이터베이스 연결과 저장 관련 코드만 관리합니다.

향후 다음 파일이 추가될 예정입니다.

- `AppDbContext.cs`: Entity Framework Core의 DB 연결 지점
- `Configurations/`: Entity별 테이블 설정
- `Migrations/`: 데이터베이스 변경 이력
- `Seed/`: 최초 역할과 관리자 계정 데이터

Controller에서는 이 폴더의 코드를 직접 사용하지 않고 Service를 통해 접근합니다.
