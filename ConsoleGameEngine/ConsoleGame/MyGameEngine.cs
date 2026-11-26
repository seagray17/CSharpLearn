// 같은 질문을 했는데

// AI 임의로 구성한 내용을 파악하고 이 부분을 AI로 만들어서 만들어지는 결과를 일정한 수준으로 맞추게 유도하는 작업.

// # ㅁ -> 가로행이 밀려서 그림이 그려진다

// '상속'
// 내가 만들고 싶은 것을 클래스 만들어주세요.
// abstract 추상 : 기능을 구현하지는 않을거지만 너가 이 내용을 무조건 구현해야해.

using System.Text;

class MyGameEngine : BaseGameEngine
{
    private int _width = 15;
    private int _height = 10;
    private int _playerX = 5;
    private int _playerY = 5;
    private bool _isRunning = true;

    // 1. 깜빡임 해결을 위한 문자열 버퍼 (매 프레임 새로 할당 방지)
    private StringBuilder _frameBuffer = new StringBuilder();

    public MyGameEngine()
    {
        // 커서 숨기기 (깜빡임 최소화)
        Console.CursorVisible = false;
        Console.Clear();
    }

    public void Run()
    {
        while (_isRunning)
        {
            Input();
            Update();
            Rendering();

            // 프레임 속도 조절 (약 30~60 FPS)
            Thread.Sleep(33);
        }
    }

    public override void Input()
    {
        // 키 입력이 있을 때만 처리 (블로킹 방지)
        if (!Console.KeyAvailable) return;

        ConsoleKeyInfo keyInfo = Console.ReadKey(true);

        switch (keyInfo.Key)
        {
            case ConsoleKey.W:
            case ConsoleKey.UpArrow:
                if (_playerY > 1) _playerY--;
                break;
            case ConsoleKey.S:
            case ConsoleKey.DownArrow:
                if (_playerY < _height - 2) _playerY++;
                break;
            case ConsoleKey.A:
            case ConsoleKey.LeftArrow:
                if (_playerX > 1) _playerX--;
                break;
            case ConsoleKey.D:
            case ConsoleKey.RightArrow:
                if (_playerX < _width - 2) _playerX++;
                break;
            case ConsoleKey.Escape:
                _isRunning = false;
                break;
        }
    }

    public override void Update()
    {
        // 게임 로직, 충돌 체크, AI 이동 등을 여기에 구현
    }

    public override void Rendering()
    {
        // [핵심 1] 깜빡임 해결: Console.Clear() 대신 커서만 (0, 0)으로 이동해 덮어쓰기
        Console.SetCursorPosition(0, 0);
        _frameBuffer.Clear();

        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                // [핵심 2] 화면 2칸 밀림 해결: 모든 타일을 정확히 2바이트(2칸) 너비로 통일
                if (x == _playerX && y == _playerY)
                {
                    _frameBuffer.Append("★"); // 2칸 문자
                }
                else if (y == 0 || y == _height - 1 || x == 0 || x == _width - 1)
                {
                    _frameBuffer.Append("■"); // 2칸 문자 (벽)
                }
                else
                {
                    _frameBuffer.Append("  "); // 빈 공간도 1칸(" ")이 아닌 2칸("  ") 공백 사용!
                }
            }
            _frameBuffer.AppendLine();
        }

        _frameBuffer.AppendLine("이동: 방향키 / WASD | 종료: ESC");

        // 한 번의 Write 호출로 전체 화면 출력 (I/O 병목 및 깜빡임 제거)
        Console.Write(_frameBuffer.ToString());
    }
}

// MyGameEngine : 키밀림X , 화면이 깜빡이지도 않는 3요소가 있는