// 아이템은 어떤 종류가 있을 까요? 열거형



// 갯수는 몇개가 있을까요?
// 이를 콘솔에서 어떻게 보여지게 할거에요?

// '조건문, 반복문, 배열, 열거형'

// 어떤 입력을 해서 무엇이 변경해야하고 그것을 어떻게 보여줘야 한다.
// 숫자를 1~10까지를 클릭하고 예약되어 있는 명령이 실행이 된다.


using System;

class Program
{
    static void Main()
    {
        // ==========================================
        // 1. 초기 데이터 설정 (Init)
        // ==========================================
        const int ITEM_MAX_COUNT = 10;

        string[] names = new string[ITEM_MAX_COUNT]
        {
            "녹슨 단검", "가죽 투구", "나무 방패", "체력 물약", "마나 물약",
            "철제 장검", "사슬 갑옷", "강철 부츠", "화염의 지팡이", "드래곤 슬레이어"
        };

        int[] prices = new int[ITEM_MAX_COUNT];
        bool[] isPurchased = new bool[ITEM_MAX_COUNT]; // 구매 여부 상태

        for (int i = 0; i < ITEM_MAX_COUNT; i++)
        {
            prices[i] = 10 + 10 * i;
        }

        int playerGold = 300;                        // 초기 보유 골드
        string systemMessage = "구매할 아이템 번호를 입력하세요."; // 안내 메시지
        string userInput = "";                       // 입력값 저장
        bool isRunning = true;                       // 루프 실행 여부

        // 한글 너비 보정 로컬 함수
        string PadRightKorean(string text, int totalWidth)
        {
            int koreanCount = 0;
            foreach (char c in text)
            {
                if (c >= 0xAC00 && c <= 0xD7A3) koreanCount++;
            }
            return text.PadRight(totalWidth - koreanCount);
        }

        // ==========================================
        // 게임 루프: 최초 화면 렌더링 후 입력 -> 업데이트 -> 렌더링 반복
        // ==========================================
        while (isRunning)
        {
            // ------------------------------------------
            // [3. 렌더링 (Render)] - 현재 상태 화면 출력
            // ------------------------------------------
            Console.Clear();
            Console.WriteLine("===================================================");
            Console.WriteLine($"| {PadRightKorean("번호", 4)} | {PadRightKorean("아이템 이름", 16)} | {"가격",7} | {PadRightKorean("상태", 8)} |");
            Console.WriteLine("===================================================");

            for (int i = 0; i < ITEM_MAX_COUNT; i++)
            {
                string colNum = (i + 1).ToString().PadRight(4);
                string colName = PadRightKorean(names[i], 16);
                string colPrice = $"{prices[i],4} G";
                string statusText = isPurchased[i] ? "구매완료" : "판매중";
                string colStatus = PadRightKorean(statusText, 8);

                Console.WriteLine($"| {colNum} | {colName} | {colPrice,8} | {colStatus} |");
            }

            Console.WriteLine("===================================================");
            Console.WriteLine($" 보유 골드 : {playerGold} G");
            Console.WriteLine($" 알림      : {systemMessage}");
            Console.WriteLine("===================================================");

            // ------------------------------------------
            // [1. 입력 (Input)] - 사용자 명령 받기
            // ------------------------------------------
            Console.Write("번호 입력 (1~10, 종료는 0) > ");
            userInput = Console.ReadLine();

            // ------------------------------------------
            // [2. 업데이트 (Update)] - 입력에 따른 데이터 및 상태 갱신
            // ------------------------------------------
            if (int.TryParse(userInput, out int selectedNumber))
            {
                if (selectedNumber == 0)
                {
                    isRunning = false;
                    Console.WriteLine("상점을 종료합니다.");
                }
                else if (selectedNumber >= 1 && selectedNumber <= ITEM_MAX_COUNT)
                {
                    int index = selectedNumber - 1;

                    if (isPurchased[index])
                    {
                        systemMessage = $"[{names[index]}]은(는) 이미 구매한 아이템입니다.";
                    }
                    else if (playerGold < prices[index])
                    {
                        systemMessage = $"골드가 부족합니다! (필요: {prices[index]} G)";
                    }
                    else
                    {
                        playerGold -= prices[index];
                        isPurchased[index] = true;
                        systemMessage = $"[{names[index]}] 구매 완료! (-{prices[index]} G)";
                    }
                }
                else
                {
                    systemMessage = "1부터 10 사이의 번호를 입력해주세요.";
                }
            }
            else
            {
                systemMessage = "잘못된 입력입니다. 숫자만 입력해주세요.";
            }
        }
    }
}
