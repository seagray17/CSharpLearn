// 배열

// 왜 쓸까요? 
// 반복문, 갯수가 정해졌을 때 쓰면 'for'

// 어떻게 배열을 사용하면 n개를 하나의 코드로 표현할 수 있을까요?
// 배열의 구현 원리를 알면 설명할 수 있다. -> 자료구조.

using System.Reflection;

int[] numbers = new int[10];

for (int i = 0; i < 10; i++)
{
    numbers[i] = i;
    Console.WriteLine($"현재 값은 : {numbers[i]} ");
}

// 배열은 같은 자료형을 한번에 표현할 수 있다.
// 갯수를 정하면 시작부터 끝까지 데이터를 저장하고 언제든지 꺼내서 쓸 수 있다.

// 가격은 가격인데 xx 가격너무 많은데 price 마음것 써도 되나?

const int ITEM_MAX_COUNT = 10;

string[] names = new string[ITEM_MAX_COUNT]
{
            "녹슨 단검", "가죽 투구", "나무 방패", "체력 물약", "마나 물약",
            "철제 장검", "사슬 갑옷", "강철 부츠", "화염의 지팡이", "드래곤 슬레이어"
};

int[] prices = new int[ITEM_MAX_COUNT];

// 한글 너비 보정 출력용 로컬 함수
string PadRightKorean(string text, int totalWidth)
{
    int koreanCount = 0;
    foreach (char c in text)
    {
        if (c >= 0xAC00 && c <= 0xD7A3) koreanCount++; // 한글 음절 체크
    }
    return text.PadRight(totalWidth - koreanCount);
}

Console.WriteLine("=========================================");
Console.WriteLine($"| {PadRightKorean("번호", 4)} | {PadRightKorean("아이템 이름", 16)} | {"가격",7} |");
Console.WriteLine("=========================================");

for (int i = 0; i < ITEM_MAX_COUNT; i++)
{
    prices[i] = 10 + 10 * i;

    string colNum = (i + 1).ToString().PadRight(4);
    string colName = PadRightKorean(names[i], 16);
    string colPrice = $"{prices[i],4} G";

    Console.WriteLine($"| {colNum} | {colName} | {colPrice,8} |");
}

Console.WriteLine("=========================================");