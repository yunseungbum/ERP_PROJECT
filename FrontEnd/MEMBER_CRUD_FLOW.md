# 회원정보 CRUD 흐름

## 권한

| 기능 | 허용 역할 |
|---|---|
| 목록·단건 조회 | 로그인한 모든 사용자 |
| 추가·수정 | President, Director, Coach, Treasurer, InventoryManager |
| 삭제 | President |
| 일반 Member와 guest | 조회만 가능 |

프론트엔드는 권한에 따라 버튼을 표시하지만 최종 권한 검사는 백엔드의
JWT 역할 검사가 담당한다.

## API

```text
GET    /api/members             회원 목록
GET    /api/members/{memberId}  회원 단건
POST   /api/members             회원 추가
PUT    /api/members/{memberId}  회원 수정
DELETE /api/members/{memberId}  회원 비활성화
```

모든 요청에는 다음 헤더가 자동으로 포함된다.

```http
Authorization: Bearer {accessToken}
```

## 수정 흐름

```text
목록에서 수정 클릭
→ /members/{memberId}/edit
→ GET /api/members/{memberId}
→ 기존 회원정보를 수정 폼의 initialValues로 전달
→ 사용자가 내용을 변경하고 저장
→ PUT /api/members/{memberId}
→ 목록으로 이동하여 DB 데이터 다시 조회
```

## 삭제 흐름

삭제 버튼은 회장에게만 보인다. 삭제 확인 후 `DELETE` API를 호출하며 DB 행을
제거하지 않고 `is_active = false`로 변경한다. 따라서 `memberId`와 다른 업무
데이터의 연결은 유지된다.

## 회원 활동 상태

`isActive`는 소프트 삭제 여부이며 활동 상태와 구분한다.

| 값 | 의미 | 신규 월 회비 |
|---|---|---|
| `memberStatus = Active` | 현재 활동 중 | 청구 대상 |
| `memberStatus = Paused` | 활동 중단 | 청구 제외 |
| `isActive = false` | 삭제된 회원 | 모든 기본 조회에서 제외 |

상태를 `Paused`로 변경해도 상태 변경 전에 이미 생성된 회비와 미납금은
삭제하거나 취소하지 않는다. 회비 일괄 생성 시에는 삭제되지 않은 활동 회원만
대상으로 조회한다.

```csharp
member.IsActive &&
member.MemberStatus == MemberStatusCodes.Active
```
