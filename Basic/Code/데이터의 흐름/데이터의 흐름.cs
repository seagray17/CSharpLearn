using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// 반드시 내가 주석으로 지정한 표현 방식쓰고 너가 추가적으로 기능을 구현하지는 말아라.
// 만약에 그 상태에서 실행이 안된다면 안되는 이유를 나에게 말해라. C#으로
// 숫자 야구 게임을 조건문으로 만들때 조건을 내가 만든다
// 상대가 숫자를 0 ~ 100까지 정한다
// 나는 상대가 정한 숫자를 1가지 말한다
// 그 답이 틀리면 내가 말한 숫자가 상대가 정한 숫자보다 큰지 작은지를 말한다.
// 내가 말한 숫자가 상대가 정한 숫자와 같으면 당신이 이겼다를 말하고 종료한다.





namespace Basic
{
    internal class 데이터의_흐름 
    {
        // 언더테일
        // 너 몇번쨰 죽었구나.
        // 너 죽음 수 1번일때 2번일때 3번일때 4번일때 deathcount == 1 2 3 4 5 6
        static void Main()
        {
            // if ,else if, if
            bool GameFInish = true;
            int Score = 105; // 0 ~ 100

            if (GameFInish) 
            {
                // 60점 이상 80점 미만 B를 출력하라
                if (60 <= Score && 80 > Score)
                {
                    Console.WriteLine("B");
                }

                // 80점 이상 100점 미만 A를 출력하라
                else if (80 <= Score && 100 > Score)
                {
                    Console.WriteLine("A");
                }
                else
                {
                    Console.WriteLine("미정의");
                }
            }

            string name = "거북이";
            // switch 스위치
            switch (name)
            {
                case "파이리":
                    Console.WriteLine("파이리");
                    break;

                case "거북이":
                    Console.WriteLine("거북이");
                    break;

            }
        }


    }
}
