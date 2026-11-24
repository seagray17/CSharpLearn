/*
 *  클래스의 생성자까지 사용 문법 실습한다.
 *  
 *  -게임 폴더
 *      -Program.cs
 *      -Unit.cs
 *      
 * AI코드를 하나의 Main 작성을 하면 가독성이 매우 떨어집니다. AI가 무엇을 만들었는지 1도 알아볼 수가 없다.
 * 이해 가능한 범위까지 코드를 줄이면됩니다.
 * 1번째로. Main으로만 게임 만들어. -> 공통된 속성과 기능을 찾아서 클래스로 표현하고 그 클래스를 분할시켜라.
 * */
  using System;

namespace TextRpgGame
{
    class Program
    {
        static void Main()
        {
            // 초기화 (Init)
            Unit player = new Unit("전사", 100, 20);
            Unit enemy = new Unit("고블린", 80, 12);

            bool isRunning = true;
            string battleLog = "전투가 시작되었습니다! 행동을 선택하세요.";

            // 초기 화면 렌더링
            Render(player, enemy, battleLog);

            // 게임 루프 (Game Loop)
            while (isRunning)
            {
                // 1. 입력 (Input)
                ConsoleKey input = GetInput();

                // 2. 업데이트 (Update)
                Update(input, player, enemy, ref isRunning, ref battleLog);

                // 3. 렌더링 (Render)
                Render(player, enemy, battleLog);
            }
        }

        static ConsoleKey GetInput()
        {
            return Console.ReadKey(true).Key;
        }

        static void Update(ConsoleKey input, Unit player, Unit enemy, ref bool isRunning, ref string battleLog)
        {
            if (input == ConsoleKey.D1 || input == ConsoleKey.NumPad1)
            {
                player.Attack(enemy);
                battleLog = $"{player.Name}의 공격! {enemy.Name}에게 {player.AttackPower}의 피해를 입혔습니다.\n";

                if (!enemy.IsDead)
                {
                    enemy.Attack(player);
                    battleLog += $"{enemy.Name}의 반격! {player.Name}에게 {enemy.AttackPower}의 피해를 입혔습니다.";
                }
            }
            else if (input == ConsoleKey.D2 || input == ConsoleKey.NumPad2)
            {
                int healAmount = 15;
                player.Heal(healAmount);
                battleLog = $"{player.Name}이(가) 체력을 {healAmount} 회복했습니다.\n";

                enemy.Attack(player);
                battleLog += $"{enemy.Name}의 공격! {player.Name}에게 {enemy.AttackPower}의 피해를 입혔습니다.";
            }
            else if (input == ConsoleKey.Q)
            {
                isRunning = false;
                battleLog = "게임을 종료합니다.";
            }
            else
            {
                battleLog = "잘못된 입력입니다. 1, 2, 또는 Q를 눌러주세요.";
            }

            // 승패 판정
            if (player.IsDead)
            {
                battleLog += $"\n\n[패배] {player.Name}이(가) 쓰러졌습니다...";
                isRunning = false;
            }
            else if (enemy.IsDead)
            {
                battleLog += $"\n\n[승리] {enemy.Name}을(를) 처치했습니다!";
                isRunning = false;
            }
        }

        static void Render(Unit player, Unit enemy, string log)
        {
            Console.Clear();
            Console.WriteLine("========================================");
            Console.WriteLine($" [{player.Name}] HP: {player.CurrentHp}/{player.MaxHp} | 공격력: {player.AttackPower}");
            Console.WriteLine($" [{enemy.Name}]  HP: {enemy.CurrentHp}/{enemy.MaxHp} | 공격력: {enemy.AttackPower}");
            Console.WriteLine("========================================");
            Console.WriteLine();
            Console.WriteLine(log);
            Console.WriteLine();
            Console.WriteLine("----------------------------------------");
            Console.WriteLine(" [1] 공격   [2] 회복   [Q] 종료");
            Console.WriteLine("----------------------------------------");
        }
    }
}