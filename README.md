# 덧뺄


### (1) 개요
서버 1명 , 클라이언트 2명 , 디자인 2명 으로 이루어진 총 5명의 인원으로 이루어진 팀으로 개발을 하게된 게임입니다.  
현재 존재하는 카드 게임이 아니며 친구들과 여행을 가서 자주하던 게임을 코로나로 인해 가지 못하게 되어 집에서도 서로 게임을 하기 위해 직접 멤버를 모집하여 개발하게 된 게임입니다.  
쉽게 구할 수 있는 트럼프 카드를 통해 즐길 수 있습니다.  
현재 존재하는 포커, 홀덤, 블랙잭과 같은 베팅이 존재하며 카드 족보가 따로 존재하지 않아 쉽게 배울 수 있습니다. 


### (2) 게임 설명
1. 최소 2명~ 최대 5명의 인원 참여 가능합니다.
2. 플레이어들은 총 3장의 카드를 소지한 후 한장의 카드를 본인만 확인할 수 있습니다.
3. 왼쪽 가운데 오른쪽 카드 순서에 따라 왼쪽 + 오른쪽, 왼쪽 * 2, 왼쪽 - 오른쪽 등과 같은 조합이 이루어 질 수 있습니다.
4. 왼쪽 카드 공개전 베팅과 오른쪽 카드 공개 전 베팅이 존재합니다. 즉, 총 2번의 베팅이 존재합니다.
5. 가장 높은 숫자를 가진 플레이어가 승리하게 되며 판돈을 가져가게 됩니다.


### (3) 게임 규칙 (카드 규칙)
* 참여한 플레이어들은 3장의 카드를 뒷면이 보이도록 받습니다.  
* 3장의 카드 중 1장의 카드 숫자를 본인만 확인 할 수 있습니다.  
* 플레이어들은 확인한 카드를 다시 뒤집어 3장의 카드가 뒷면이 되도록 배치하며 같은 높이가 되도록 배치합니다.  
<img src="https://user-images.githubusercontent.com/49023743/124597181-1dbc2280-de9e-11eb-925a-f43f58f0d69f.PNG">
* 플레이어들이 카드를 확인 한 후 3장의 카드 순서를 임의로 배치 할 수 있습니다.  
* 카드 순서에 따라 왼쪽 + 오른쪽 카드 , 왼쪽 - 오른쪽 카드 , 왼쪽 * 2 계산되므로 배치를 가능한 높은 숫자가 되도록 해야 합니다.  
* 가운데 카드 숫자 홀수 : - (마이너스)  
* 가운데 카드 숫자 짝수 : + (플러스)  
* 3장의 카드 숫자가 동일할 경우 (트리플) : 왼쪽 or 오른쪽 카드 숫자 *2  
* 조커 카드 가운데 위치 : 왼쪽 * 오른쪽 (곱셈)  
* 조커 카드 왼쪽 또는 오른쪽 위치 : 반대편에 존재하는 숫자와 동일하게 취급, 단 남은 2장의 카드 숫자가 동일할 경우 트리플로 취급합니다.  
* 조커 카드 2장 왼쪽 과 오른쪽 위치 : 카드 3장의 합산 결과는 0으로 취급합니다.  
* 조커 카드 2장 왼쪽 또는 오른쪽 + 가운데 위치 : 남은 카드의 숫자 * 숫자  
* 조커 카드를 소지하고 Die를 하게 된다면 기본 판돈의 * 2 만큼 베팅 금액을 지불하게 됩니다.  

### (4) 게임 방법 (순서)


