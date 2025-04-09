![image](https://github.com/user-attachments/assets/cf24b90f-2992-4fcb-a748-71d5d82aa8fe)

세계수를 성장시켜 거대하게 키우고,

다양한 꽃을 구매해 화려한 꽃밭을 만들고,

나만의 동물을 배치하여 감상해 보세요!

[`홍보영상`](https://youtu.be/eV8vvz2bWnE?si=fTQnZzfiAFHHJ3tz)
## 게임 소개
|게임명|세계수 키우기|
|:---:|:---:|
|장르|`방치형` `힐링` `시뮬레이션`|
|개발엔진|`Unity(2022.3.17.f1)`|
|협업툴|`Discord` `figma` `google sheets`|
|플랫폼|`PC` `Web` `Android`|
|개발 기간|`2024.6.27` ~ `2024.8.22`|
## 목차
- [팀원 소개](#팀원-소개)
- [구현 기능](#구현-기능)
- [게임 플레이 링크](게임-플레이-링크)
- [사용 에셋](#사용-에셋)
## 팀원 소개
- 팀장: `허정`
  - 게임 기획
  - 맵 제작
  - 데이터 테이블 작성
- 부팀장: `이영대`
  - UI 제작
  - 카메라 기능
  - 사운드 기능
  - 서버 구축
- 팀원: `김보근`
  - 세계수 기능
  - 식물 기능
  - 스킬
  - 방치 보상 기능
  - 포그(안개)
- 팀원: `박도현`
  - 동물 기능
  - 도감 기능
  - 도전과제 기능
  - 빌드 및 리펙토링

## 구현 기능
### [세계수](https://github.com/codingskywhale/WorldTreeClassic/wiki/%EC%84%B8%EA%B3%84%EC%88%98)
- 세계수 레벨업 시 세계수의 외형이 변한다.
- 세계수 일정 레벨 도달 시 동물과 식물이 순차적으로 해금된다.
- 세계수 레벨업 시 동물 버블 생명력 생산량이 증가한다.
- 25n레벨마다 버블 생명력 생산량이 2배씩 증가한다.
### [식물](https://github.com/codingskywhale/WorldTreeClassic/wiki/%EC%8B%9D%EB%AC%BC)
- 식물 해금 및 레벨업 시 맵에 식물이 배치된다.
- 식물 해금 및 레벨업 시 초당 생명력 생산량이 증가한다.
- 식물 해금 시 맵에 배치 가능한 동물이 증가한다.
- 25n 레벨마다 식물의 초당 생명력 생산량이 2배 증가한다.
### [동물](https://github.com/codingskywhale/WorldTreeClassic/wiki/%EB%8F%99%EB%AC%BC)
- 동물 해금 및 구매 시 초당 생명력 생산량이 2배 증가한다.
- 구매한 동물은 맵에 배치된다.
### [동물가방](https://github.com/codingskywhale/WorldTreeClassic/wiki/%EB%8F%99%EB%AC%BC-%EA%B0%80%EB%B0%A9)
- 동물 해금 시 해당 동물의 세부도감이 활성화 된다.
- 세부 도감에서 맵에 배치한 동물을 가방에 보관할 수 있다.
- 세부 도감에서 가방에 보관한 동물을 맵에 배치할 수 있다.
- 스토리 도감에서 동물 스토리를 확인할 수 있다.
### [따라가기](https://github.com/codingskywhale/WorldTreeClassic/wiki/%EB%94%B0%EB%9D%BC%EA%B0%80%EA%B8%B0)
- 따라가기 버튼 클릭 시 세계수를 중심으로 카메라를 회전시킬 수 있다.
- 따라가기 버튼 클릭 후 동물 클릭 시 해당 동물을 중심으로 카메라를 회전시킬 수 있다.
- 따라가기 버튼 다시 클릭 시 카메라가 기존 위치로 돌아와 고정된다.
### [배경음악](https://github.com/codingskywhale/WorldTreeClassic/wiki/%EB%B0%B0%EA%B2%BD%EC%9D%8C%EC%95%85)
- 음악 버튼 클릭 시 배경음악을 변경할 수 있다.
### [대기화면](https://github.com/codingskywhale/WorldTreeClassic/wiki/%EB%B0%A9%EC%B9%98-%EB%B3%B4%EC%83%81)
- 10초 이상 게임 조작이 없을 시 대기화면으로 넘어가며, 화면상 UI를 비활성화한다.
- 대기화면에서 화면 클릭 시 다시 화면으로 넘어온다.
### [방치보상](https://github.com/codingskywhale/WorldTreeClassic/wiki/%EB%B0%A9%EC%B9%98-%EB%B3%B4%EC%83%81)
- 게임 종료 후 재접속 시 게임 방치 보상을 획득할 수 있다.
### [광고](https://github.com/codingskywhale/WorldTreeClassic/wiki/%EA%B4%91%EA%B3%A0-%EB%B3%B4%EC%83%81)
- 광고 시청 시 보상을 획득할 수 있다.

## 게임 플레이 링크
[PC 플레이](https://pdhyeon.itch.io/worldtreeclassic)

[모바일 다운로드](https://drive.google.com/file/d/1CdF-V_yx7O-nsc4Y3gh_RkqgGPTJRk8m/view?usp=drive_link)

## 사용 에셋
- 식물: [Low Poly Trees Pack - Flowers](https://assetstore.unity.com/packages/3d/vegetation/flowers/low-poly-trees-pack-flowers-178576)
- 동물: [Low Poly Animated Animals](https://assetstore.unity.com/packages/3d/characters/animals/low-poly-animated-animals-93089)
- 세계수/맵: [Grasslands - Stylized Nature](https://assetstore.unity.com/packages/3d/environments/fantasy/grasslands-stylized-nature-287353)
- 세계수2: [Lemon Trees](https://assetstore.unity.com/packages/3d/vegetation/trees/lemon-trees-200372)
