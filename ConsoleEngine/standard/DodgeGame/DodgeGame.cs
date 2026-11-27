using ConsoleEngine;

namespace DodgeGame2
{

    // 게임 v1.0 (태그 game-v1.0.0) — docs/system-plan.md의 규칙 ↔ 구현 대응표를 따른다
    class DodgeGame : GameBase
    {
        private const int Width = 30;             // 규칙 1: 30×20칸
        private const int Height = 20;
        private const float SpawnInterval = 0.3f; // 규칙 3
        private const float RockSpeed = 10f;
        private const float GoalSeconds = 60f;    // 최종 목표

        private readonly List<Rock> _rocks = new List<Rock>();
        private int _playerX = Width / 2;         // 맨 아래 줄 가운데
        private float _spawnTimer;
        private float _time;
        private GameState _state = GameState.Playing;

        private bool _wasd;                       // 조작: 화살표 ↔ A/D

        public DodgeGame() : base(Width, Height)
        {
            Keys.Bind(ConsoleKey.LeftArrow, new MoveCommand(this, -1));   // v0.4.0
            Keys.Bind(ConsoleKey.RightArrow, new MoveCommand(this, +1));
        }

        /// <summary>MoveCommand가 부른다. 진행 중일 때만, 화면 안에서만 움직인다.</summary>
        public void MovePlayer(int dx)
        {
            if (_state != GameState.Playing) return;
            int x = _playerX + dx;
            if (x >= 0 && x < Width) _playerX = x;          // 규칙 2
        }
        protected override void Input(ConsoleKey key)    // 묶이지 않은 키만 온다
        {
            if (key == ConsoleKey.Escape) Quit();            // 규칙 6
            if (key == ConsoleKey.K) SwapControls();
        }

        private void SwapControls()
        {
            if (_wasd)
            {
                Keys.Rebind(ConsoleKey.A, ConsoleKey.LeftArrow);
                Keys.Rebind(ConsoleKey.D, ConsoleKey.RightArrow);
            }
            else
            {
                Keys.Rebind(ConsoleKey.LeftArrow, ConsoleKey.A);
                Keys.Rebind(ConsoleKey.RightArrow, ConsoleKey.D);
            }
            _wasd = !_wasd;
        }



        private void Restart()
        {
            _rocks.Clear();
            _playerX = Width / 2;
            _spawnTimer = 0f;
            _time = 0f;
            _state = GameState.Playing;
        }

        protected override void Update(float deltaTime)
        {
            if (_state != GameState.Playing)
            {
                if (Keyboard.WasPressed(ConsoleKey.R)) Restart();   // 이번 프레임에 R?
                return;
            }

            if (_state != GameState.Playing) return;
            _time += deltaTime;
            if (_time >= GoalSeconds)
            {
                _state = GameState.Clear;
                return;
            }
            _spawnTimer -= deltaTime;
            if (_spawnTimer <= 0f)
            {
                _rocks.Add(new Rock(Random.Shared.Next(Width)));
                _spawnTimer = SpawnInterval;
            }
            for (int i = _rocks.Count - 1; i >= 0; i--)
            {
                Rock rock = _rocks[i];
                rock.Y += RockSpeed * deltaTime;             // 초당 10칸
                if ((int)rock.Y == Height - 1 && rock.X == _playerX)
                    _state = GameState.GameOver;              // 규칙 4
                if (rock.Y >= Height) _rocks.RemoveAt(i);
            }
        }

        protected override void Render(Screen screen)
        {
            foreach (Rock rock in _rocks)
                screen.Draw(rock.X, (int)rock.Y, '*');
            screen.Draw(_playerX, Height - 1, '@');
            screen.DrawText(0, 0, $"TIME {(int)_time}/{(int)GoalSeconds}");
            if (_state == GameState.GameOver)
                screen.DrawText(5, 9, "GAME OVER - R: retry");
            if (_state == GameState.Clear)
                screen.DrawText(6, 9, "CLEAR! - R: retry");

            screen.DrawText(0, 1, _wasd ? "MOVE A/D (K)" : "MOVE <-/-> (K)");

        }
    }
}