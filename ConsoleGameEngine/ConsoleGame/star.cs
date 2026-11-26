using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

// ==========================================
// 1. 최상위 문 (Main 함수 역할 - 파일 최상단 위치 필수)
// ==========================================
MyGameEngine game = new MyGameEngine();
game.Run();

Console.Clear();
Console.WriteLine("게임이 종료되었습니다!");


// ==========================================
// 2. 클래스 선언부
// ==========================================
abstract class BaseGameEngine
{
    // 입력기능     Input
    // 갱신기능     Update
    // 렌더링기능   Rendering
    abstract public void Input();
    abstract public void Update();
    abstract public void Rendering();
}

class MyGameEngine : BaseGameEngine
{
    private int _width = 15;
    private int _height = 15;
    private int _playerX = 7;
    private int _playerY = 13;
    private bool _isRunning = true;
    private bool _isGameOver = false;

    // 게임 요소 (점수, 적, 타이머)
    private int _score = 0;
    private int _tickCount = 0;
    private Random _rand = new Random();
    private List<(int x, int y)> _enemies = new List<(int x, int y)>();

    // 깜빡임 해결을 위한 문자열 버퍼
    private StringBuilder _frameBuffer = new StringBuilder();

    public MyGameEngine()
    {
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

            // 프레임 속도 조절 (약 30 FPS)
            Thread.Sleep(33);
        }
    }

    public override void Input()
    {
        if (!Console.KeyAvailable) return;

        ConsoleKeyInfo keyInfo = Console.ReadKey(true);

        switch (keyInfo.Key)
        {
            // 좌우 이동으로 변경
            case ConsoleKey.A:
            case ConsoleKey.LeftArrow:
                if (!_isGameOver && _playerX > 1) _playerX--;
                break;
            case ConsoleKey.D:
            case ConsoleKey.RightArrow:
                if (!_isGameOver && _playerX < _width - 2) _playerX++;
                break;
            // 게임 오버 시 R키로 재시작
            case ConsoleKey.R:
                if (_isGameOver) ResetGame();
                break;
            case ConsoleKey.Escape:
                _isRunning = false;
                break;
        }
    }

    public override void Update()
    {
        if (_isGameOver) return;

        _tickCount++;

        // 4프레임마다(약 0.13초) 적 이동 및 생성
        if (_tickCount % 4 == 0)
        {
            // 1. 기존 적들 아래로 1칸씩 이동
            for (int i = _enemies.Count - 1; i >= 0; i--)
            {
                int nextY = _enemies[i].y + 1;

                // 바닥에 닿으면 제거 및 점수 획득
                if (nextY >= _height - 1)
                {
                    _enemies.RemoveAt(i);
                    _score += 10;
                }
                else
                {
                    _enemies[i] = (_enemies[i].x, nextY);
                }
            }

            // 2. 새로운 적 생성 (벽 안쪽 1 ~ _width - 2 사이 랜덤)
            int spawnX = _rand.Next(1, _width - 1);
            _enemies.Add((spawnX, 1));
        }

        // 3. 플레이어와 적 충돌 체크 (매 프레임 검사)
        foreach (var enemy in _enemies)
        {
            if (enemy.x == _playerX && enemy.y == _playerY)
            {
                _isGameOver = true;
                break;
            }
        }
    }

    public override void Rendering()
    {
        Console.SetCursorPosition(0, 0);
        _frameBuffer.Clear();

        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                // 모든 타일을 정확히 2칸 너비로 통일하여 밀림 방지
                if (x == _playerX && y == _playerY)
                {
                    _frameBuffer.Append(_isGameOver ? "※" : "★"); // 피격 시 모양 변경
                }
                else if (_enemies.Contains((x, y)))
                {
                    _frameBuffer.Append("▼"); // 떨어지는 장애물 (2칸 문자)
                }
                else if (y == 0 || y == _height - 1 || x == 0 || x == _width - 1)
                {
                    _frameBuffer.Append("■"); // 벽 (2칸 문자)
                }
                else
                {
                    _frameBuffer.Append("  "); // 2칸 공백
                }
            }
            _frameBuffer.AppendLine();
        }

        // UI 출력 (잔상 방지를 위해 공백으로 줄 끝을 채움)
        _frameBuffer.AppendLine($"점수: {_score}점                    ");
        if (_isGameOver)
        {
            _frameBuffer.AppendLine("GAME OVER! 재시작: R | 종료: ESC  ");
        }
        else
        {
            _frameBuffer.AppendLine("이동: A, D / 좌우 방향키 | 종료: ESC");
        }

        Console.Write(_frameBuffer.ToString());
    }

    private void ResetGame()
    {
        _playerX = _width / 2;
        _enemies.Clear();
        _score = 0;
        _tickCount = 0;
        _isGameOver = false;
    }
}
