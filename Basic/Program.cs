// 2026 11 18
// C# 프로그래밍 입문
// 학습 목표 : 화면에 내가 원하는 글자를 출력하고 싶다.

// 컴퓨터 : 작성한 글자를 어떻게 읽어야 해?
// 입력과 출력
// 내가 어떤 크기에 어떤 물건을 넣어야 할지 딱 맞는 상자를 알려줄게 -> type 형태

// 변수 
// 정수 int
// 실수 float
// 문자열 string
// 불 bool

// [데이터 + 로직] =>
//변수
// 로직 : 어떻게 사용을 해야 진짜 게임이 끝난 것 처럼 표현이 될까요

// 두번째 학습 목표 : 변수를 변형해서 원하는 흐름을 얻기

// 슬라임hp
// 용사hp
//슬라임atk
//용사atk
// '전투' : [슬라임의 턴]. 슬라임이 용사의 체력을 슬라임의 공격력으로 공격합니다

// 조건문을 활용해서 아주 간단한 승리 조건을 표현해보세요

namespace Basic
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("일어나세요 용사여. 아직 당신은 끝나지 않았습니다.");
            Console.WriteLine("당신의 이름은 무엇인가요?");

            int hp = 100;
            hp = hp - 100; //
            float version = 0.01f;
            bool gameClear = false;

            Console.WriteLine(hp);
            Console.WriteLine(version);
            Console.WriteLine(gameClear);

            string name = Console.ReadLine();
            Console.WriteLine(name);

            // 그러면 당신의 이름이 뭔가요?

            // 슬라임의 체력 변수로 정의해보세요
            int slimeHP = 100;
            // 용사의 체력
            int p_HP = 100;
            // 슬라임의 공격력
            int s_atk = 1;
            // 용사의 공격력
            int p_atk = 20;

            Console.WriteLine("[슬라임 턴] ");
            p_HP = p_HP - s_atk;
            Console.WriteLine($"용사의 체력은 : {p_HP}다");
            // 슬라임의 용사의 체력을 슬라임의 공격력으로 공격합니다.

            // 어떻게 해야 특정 경우에만 실행할 수 있게 해줄건가?

            // 조건문과 반복문

            // 게임을 클리어가 아되었을 때는 무엇을 해야 하나요?
            if(gameClear)
            {
                // 게임이 클리어를 안했을 때 해야할 일이 얼마나 있나요? 많이 있습니다. 1개가 아닐 수도 있습니다.
                Console.WriteLine("출력이 되었나요?");
            }
        }
    }
}
