using GameData;

// '구현 전에 허락을 맡아라'

//
// 게임이 되려면 무엇이 필요할까?
// 1. 입력
// 2. 렌더링 (Rendering)
// 3. Update 


class Program
{
    static void Main(string[] args)
    {
        // 초기 데이터 세팅
        MonsterType myMonster = MonsterType.FIRE;
        Tile currentTile = Tile.EARTH; // 시작 타일
        int hp = 100;
        int attack = 20;
        string statusMessage = "게임을 시작합니다. 이동할 타일 번호를 입력하세요.";
        bool isRunning = true;

        // 게임 루프
        while (isRunning)
        {
            // ==========================================
            // 1. 렌더링 (Rendering) - 현재 상태 화면 출력
            // ==========================================
            Console.Clear();
            Console.WriteLine("=========================================");
            Console.WriteLine($" 몬스터 속성 : {myMonster}");
            Console.WriteLine($" 현재 상태   : 체력 {hp} | 공격력 {attack}");
            Console.WriteLine($" 현재 타일   : {currentTile}");
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine($" 알림: {statusMessage}");
            Console.WriteLine("=========================================");
            Console.WriteLine("밟을 타일 선택 -> [0] 불(FIRE)  [1] 물(WATER)  [2] 땅(EARTH)  [Q] 종료");
            Console.Write("입력: ");

            // ==========================================
            // 2. 입력 (Input) - 사용자 키 입력 받기
            // ==========================================
            string input = Console.ReadLine();

            // ==========================================
            // 3. Update - 입력에 따른 게임 로직 및 상태 갱신
            // ==========================================
            if (input == "q" || input == "Q")
            {
                isRunning = false;
                Console.WriteLine("게임을 종료합니다.");
                break;
            }

            // 입력값에 따라 현재 밟은 타일 변경
            if (input == "0")
            {
                currentTile = Tile.FIRE;
            }
            else if (input == "1")
            {
                currentTile = Tile.WATER;
            }
            else if (input == "2")
            {
                currentTile = Tile.EARTH;
            }
            else
            {
                statusMessage = "잘못된 입력입니다. 0, 1, 2, Q 중에서 입력해주세요.";
                continue;
            }

            // 몬스터 속성(FIRE)과 밟은 타일 판정 로직
            if (myMonster == MonsterType.FIRE)
            {
                switch (currentTile)
                {
                    case Tile.FIRE:
                        attack += 5;
                        statusMessage = "불 타일을 밟아 화력이 강해졌습니다! (공격력 +5)";
                        break;

                    case Tile.WATER:
                        hp -= 20;
                        statusMessage = "물 타일을 밟아 화상을 입었습니다! (체력 -20)";
                        break;

                    case Tile.EARTH:
                        statusMessage = "땅 타일 위입니다. 아무런 효과가 없습니다.";
                        break;
                }
            }

            // 게임 오버 체크
            if (hp <= 0)
            {
                Console.Clear();
                Console.WriteLine("체력이 0이 되어 몬스터가 쓰러졌습니다! GAME OVER");
                isRunning = false;
            }
        }
    }
}