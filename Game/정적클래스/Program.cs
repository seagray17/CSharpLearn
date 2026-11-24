using ConsoleShooter;

Console.CursorVisible = false;

const int Width = 40;
const int Height = 20;

Player player = new(Width / 2, Height - 2);
List<Monster> monsters = [];
List<Bullet> bullets = [];
bool isRunning = true;

// 첫 번째 웨이브 시작 (정적 클래스 WaveManager가 몬스터 생성 및 생존 수 관리)
WaveManager.SpawnWave(monsters, Width);

// 게임 루프: 입력(Input) -> 업데이트(Update) -> 렌더링(Render)
while (isRunning)
{
    ProcessInput();
    Update();
    Render();
    Thread.Sleep(50);
}

Console.Clear();
Console.CursorVisible = true;
Console.WriteLine($"게임 종료! 최종 도달 Wave: {WaveManager.CurrentWave}");

// 1. 입력 (Input)
void ProcessInput()
{
    while (Console.KeyAvailable)
    {
        ConsoleKey key = Console.ReadKey(intercept: true).Key;

        switch (key)
        {
            case ConsoleKey.W: player.Move(0, -1, Width, Height); break;
            case ConsoleKey.S: player.Move(0, 1, Width, Height); break;
            case ConsoleKey.A: player.Move(-1, 0, Width, Height); break;
            case ConsoleKey.D: player.Move(1, 0, Width, Height); break;
            case ConsoleKey.Spacebar:
                bullets.Add(player.Shoot());
                break;
            case ConsoleKey.Escape:
                isRunning = false;
                break;
        }
    }
}

// 2. 업데이트 (Update)
void Update()
{
    // 총알 직선 이동 및 화면 이탈 제거
    for (int i = bullets.Count - 1; i >= 0; i--)
    {
        bullets[i].Update();
        if (bullets[i].IsOutOfBounds())
        {
            bullets.RemoveAt(i);
        }
    }

    // 총알과 몬스터 충돌 판정
    for (int i = bullets.Count - 1; i >= 0; i--)
    {
        for (int j = monsters.Count - 1; j >= 0; j--)
        {
            if (bullets[i].X == monsters[j].X && bullets[i].Y == monsters[j].Y)
            {
                bullets.RemoveAt(i);
                monsters.RemoveAt(j);
                WaveManager.OnMonsterKilled(); // 정적 클래스에 몬스터 사망 보고
                break;
            }
        }
    }

    // 몬스터가 전부 사망하면 다음 Wave 발생
    if (WaveManager.IsWaveCleared)
    {
        WaveManager.SpawnWave(monsters, Width);
    }
}

// 3. 렌더링 (Render)
void Render()
{
    Console.SetCursorPosition(0, 0);
    Console.WriteLine($"WAVE: {WaveManager.CurrentWave} | 남은 몬스터: {WaveManager.AliveCount}마리   ");
    Console.WriteLine("이동: WASD | 발사: Space | 종료: ESC");

    char[,] board = new char[Height, Width];
    for (int y = 0; y < Height; y++)
        for (int x = 0; x < Width; x++)
            board[y, x] = ' ';

    foreach (var monster in monsters)
        if (monster.Y >= 0 && monster.Y < Height && monster.X >= 0 && monster.X < Width)
            board[monster.Y, monster.X] = 'M';

    foreach (var bullet in bullets)
        if (bullet.Y >= 0 && bullet.Y < Height && bullet.X >= 0 && bullet.X < Width)
            board[bullet.Y, bullet.X] = '|';

    board[player.Y, player.X] = 'A';

    Console.WriteLine("+" + new string('-', Width) + "+");
    for (int y = 0; y < Height; y++)
    {
        Console.Write('|');
        for (int x = 0; x < Width; x++)
            Console.Write(board[y, x]);
        Console.WriteLine('|');
    }
    Console.WriteLine("+" + new string('-', Width) + "+");
}
