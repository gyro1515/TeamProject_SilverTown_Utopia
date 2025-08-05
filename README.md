# SilverTown Utopia

## 📖 목차
1. [프로젝트 소개](#-프로젝트-소개)
2. [팀소개](#-팀소개)
3. [프로젝트 계기](#-프로젝트-계기)
4. [주요기능](#-주요기능)
5. [개발기간](#-개발기간)
6. [기술스택](#-기술스택)
7. [와이어프레임](#-와이어프레임)
8. [프로젝트 파일 구조](#-프로젝트-파일-구조)
9. [Trouble Shooting](#️-trouble-shooting)

---

## 👨‍🏫 프로젝트 소개

<p align="center">
  <img src="https://raw.githubusercontent.com/gyro1515/TeamProject_SilverTown_Utopia/main/README_Images/1.png" alt="SilverTown Utopia 로고" width="600"/>
</p>

**SilverTown Utopia**는  
은퇴를 하고싶은 늙고 지친 용사의 실버타운 입주를 향한 마지막 모험담을 그린 로그라이크 액션 게임입니다.  
주요 기능인 **절차적 맵 생성**과 **다양한 몬스터 패턴**을 이용한 액션성을 강조하여  
**매번 새로운 전투 경험을 제공**하는 것이 특징입니다.

---

## 🧑‍🤝‍🧑 팀소개

- 팀장: 박용규  
- 프로그래머: 이원진, 문장원, 남상혁  
- 기획: 박기훈

---

## 💡 프로젝트 계기

> 궁수의 전설을 재해석하여, 절차적 맵 생성과 몬스터 회피 패턴을 차용한 역동적인 전투를 구현하고자 하였습니다.

- 플레이어와 몬스터가 모두 원거리 공격등)

- ---

## 💜 주요기능

### 1. 절차적 맵 생성
- 충돌 분산 알고리즘(Room Separation)과 MST(Minimum Spanning Tree)을 접목하여
- **매번 다른 형태의 맵**을 자동으로 생성하도록 구성

- <p align="center">
  <img src="https://raw.githubusercontent.com/gyro1515/TeamProject_SilverTown_Utopia/main/README_Images/Mapping.gif" alt="SilverTown Utopia 로고" width="600"/>
</p>

### 2. Behavior Tree 기반 AI
- 몬스터가 **고정된 공격만 하지 않고**, 상황에 따라 패턴을 달리함
- 다양한 공격 루트를 통해 **회피 중심의 전투 지양**

### 3. ScriptableObject 기반 스킬 관리
- 스킬/패턴 데이터를 모듈화하여 **인스펙터에서 직접 수정 가능**
- 빠른 밸런싱 및 확장성을 확보
Mapping.gif

---

## ⏲️ 개발기간

- **2025.07.29(월) ~ 2025.08.06(화)**

---

## 📚 기술스택

### ✔️ Language
- C#

### ✔️ Version Control
- Git
- GitHub

### ✔️ Framework / Engine
- Unity 2022.3 LTS

### ✔️ IDE
- Unity Editor
- Visual Studio 2022

### ✔️ 협업 툴
- Notion (기획 및 문서)
- Figma (UI 설계)

---

## 🖼 와이어프레임

- [📎 BoardMix 와이어프레임 링크](https://boardmix.com/app/share/CAE.CMXKjwEgASoQEYmqKFI-jfIxBupJiGf00TAGQAE/wYtfPh)

---

## 📁 프로젝트 파일 구조

Assets/
├── Main/
│   ├── Animations/           # 애니메이션 (Enemy, Player, Skill, Weapon)
│   │   ├── Enemy/
│   │   ├── Player/
│   │   ├── Skill/
│   │   └── Weapon/
│   ├── Artwork/              # 배경/타일셋
│   │   ├── Cave/
│   │   ├── RockCave/
│   │   └── TestTile/
│   ├── Fonts/
│   ├── Input/
│   ├── Materials/
│   ├── Prefabs/              # 프리팹 모음
│   │   ├── Collidables/
│   │   ├── Enemy/
│   │   ├── Item/
│   │   ├── MeleeAttack/
│   │   ├── Obstacle/
│   │   ├── Player/
│   │   ├── Room/
│   │   └── Skill/
│   ├── Resources/
│   ├── Scene/
│   ├── ScriptableObjects/    # 스크립터블 오브젝트 모음
│   ├── Scripts/              # 스크립트 전체
│   │   ├── BehaviorTree/
│   │   ├── Cam/
│   │   ├── CameraEffect/
│   │   ├── Card/
│   │   ├── Character/
│   │   ├── Collidables/
│   │   ├── Enum/
│   │   ├── Item/
│   │   ├── Manager/
│   │   ├── Map/
│   │   ├── Pattern/
│   │   ├── Skills/
│   │   └── UI/
│   └── Sounds/               # 사운드
│       ├── BGM/
│       ├── Interact/
│       ├── Monster/
│       └── Players/
├── TestAssets/               # 테스트용 애셋 (개발용)
│   ├── Animations/
│   │   ├── Enemy/
│   │   ├── Player/
│   │   ├── Skill/
│   │   └── Weapon/
│   ├── Artworks/
│   │   └── Tilemap/
│   ├── ExternalAssets/
│   │   ├── Effect/
│   │   ├── Item/
│   │   ├── Monster/
│   │   ├── Player/
│   │   ├── Tilemap/
│   │   ├── UI/
│   │   └── Weapon Skill/
│   ├── Prefabs/
│   │   ├── Enemy/
│   │   ├── MeleeAttack/
│   │   ├── Parrying/
│   │   ├── Player/
│   │   └── Skill/
│   └── Resources/
│       ├── Sprite/
│       └── Tilemap/
├── TextMesh Pro/
└── Unity.VisualScripting.Generated/

---

## 🛠️ Trouble Shooting

몬스터 대시 문제
배경
몬스터는 플레이어 위치를 기준으로 고정된 방향으로 대시
Collider를 사용하여 부피를 가진 상태로 충돌 처리
문제 상황
플레이어가 벽 끝에 붙어 있을 경우, 해당 위치가 몬스터의 목표 지점이 되나, 물리적으로 접근이 불가능
부피가 있는 콜라이더가 벽에 먼저 닿아 멈추지만, 목표에 도달하지 못했기 때문에 대시가 무한지속

시도 - 단순 충돌 중단 처리
“대시 중 벽에 닿으면 멈춘다”는 조건을 추가

새로운 문제 발생:
몬스터와 플레이어가 같은 벽에 가까이 있을 경우, 대시 시작 직후 즉시 멈추는 현상 발생
결과적으로 정상적인 대시조차 불가능해짐

최종 해결 전략
레이캐스트를 활용한 전방 충돌 탐지

Collision2D 대신 Raycast로 정확한 충돌면 판별
벽 모서리와 같이 판정이 애매한 경우에도 안정적으로 감지 가능

법선과 이동 방향 각도 계산
스치는 충돌은 슬라이딩하며 180도에 가까운 정면 충돌만 대시 중단되게 변경

슬라이딩 방향 계산
정면 충돌이 아닌 경우, moveDirection의 수직 성분을 제거하여 벽을 따라 미끄러지듯 이동

대시 목표 위치 보정
기존 목표(prePlayerPos) 대신, 현재 위치에서 슬라이딩 방향으로 남은 거리만큼 이동한 위치로 보정

결과 요약
도달 불가능한 지점에 대해 무한 대시 반복 문제 해결
충돌이 일어나도 벽을 따라 자연스럽게 슬라이딩
시작 지점과 목표 지점이 같은 벽에 붙어 있어도 정상 작동

