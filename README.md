# 🖥️ Kiosk Client (WPF + MVVM)

## 📌 프로젝트 소개
WPF 기반 키오스크 클라이언트 애플리케이션입니다.  
ASP.NET Core Web API 서버와 통신하여 상품 조회 및 CRUD 기능을 수행하며,  
MVVM 패턴을 적용하여 UI와 로직을 분리했습니다.

---

## 🛠️ 기술 스택

- **Language**: C#
- **Framework**: .NET (WPF)
- **Architecture**: MVVM Pattern
- **Communication**: HTTP (REST API)
- **Data Format**: JSON

---

## 🧱 프로젝트 구조
```
KioskClient
├── Models          # DTO 및 데이터 모델
├── Services        # API 통신 (HttpClient)
├── ViewModels      # UI 상태 및 로직
├── Commands        # ICommand 구현 (RelayCommand)
├── Views           # XAML UI
└── MainWindow.xaml
```
## 🏗️ 아키텍처
View (XAML)
```
↓ Binding
ViewModel
↓
Service
↓
ASP.NET Core API
↓
Database (SQLite)
```

## ⚙️ 주요 기능

### 1. 상품 조회 (GET)
- 서버에서 상품 목록 조회
- ObservableCollection을 통해 UI 자동 갱신

### 2. 상품 추가 (POST)
- 입력값 검증 후 서버에 전송
- 성공 시 목록 갱신

### 3. 상품 수정 (PUT)
- 선택된 상품 수정
- ViewModel 상태 기반 처리

### 4. 상품 삭제 (DELETE)
- 선택된 상품 삭제
- 확인 UI 제공

---

## 🔄 데이터 흐름
사용자 입력
→ ViewModel (상태 관리)
→ Service (API 호출)
→ Server
→ 결과 반환
→ ViewModel 갱신
→ UI 자동 반영

---

## 🎯 MVVM 적용 내용
---
- Code-behind 최소화
- Command 패턴 적용 (RelayCommand)
- Data Binding 사용
- ObservableCollection으로 UI 자동 업데이트
- INotifyPropertyChanged 구현
---

## 🧩 주요 코드 예시

### Command 바인딩
```xml
<Button Content="상품 추가"
        Command="{Binding CreateProductCommand}" />
```
### 데이터 바인딩
```xml
<TextBox Text="{Binding ProductName}" />

<ListBox ItemsSource="{Binding Products}"
         SelectedItem="{Binding SelectedProduct}" />
```

##🚀 실행 방법
ASP.NET Core API 서버 실행
WPF 클라이언트 실행
상품 조회 및 CRUD 기능 사용
---

⚠️ 트러블 슈팅
```
404 오류 → API URL 오타
HTTPS 문제 → HttpClientHandler 설정
CS0051 → DTO 접근 제한자 문제 (public 필요)
인터페이스 오류 → 메서드 구현 누락
```
---
📈 향후 개선 방향
---
- 키오스크 UI (버튼 기반 UX)
- 장바구니 기능
- 주문(Order) 시스템 추가
- 결제 기능 연동
- 상태 관리 (View 전환)
---
