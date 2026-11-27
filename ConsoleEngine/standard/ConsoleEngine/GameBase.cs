using System.Diagnostics;

namespace ConsoleEngine;

/// <summary>콘솔 게임의 뼈대. 상속해서 Input·Update·Render를 채운다.</summary>
public abstract class GameBase
{
    private readonly Screen _screen;
    private readonly int _frameMs;
    private readonly InputState _keyboard = new InputState();     // v0.4.0
    private readonly KeyMap _keys = new KeyMap();                 // v0.4.0
    private bool _quitRequested;

    /// <summary>이번 프레임에 눌린 키. Update에서 물어본다. (v0.4.0)</summary>
    protected InputState Keyboard
    {
        get { return _keyboard; }
    }

    /// <summary>키 → 명령 표. 묶인 키는 Input 대신 명령이 처리한다. (v0.4.0)</summary>
    protected KeyMap Keys
    {
        get { return _keys; }
    }

    protected GameBase(int width, int height, int framesPerSecond = 20)
    {
        _screen = new Screen(width, height);
        _frameMs = 1000 / framesPerSecond;
    }

    public void Run()
    {
        Console.CursorVisible = false;
        Console.Clear();
        var watch = new Stopwatch();
        float deltaTime = _frameMs / 1000f;      // 고정 프레임 시간(초)
        while (true)
        {
            _keyboard.BeginFrame();                    // ① 입력: 지난 프레임 키를 비우고
            while (Console.KeyAvailable)
            {
                ConsoleKey key = Console.ReadKey(true).Key;
                _keyboard.Add(key);                    // 기록하고
                if (!_keys.Handle(key)) Input(key);    // 명령이 없으면 Input으로
            }
            if (_quitRequested) break;
            Update(deltaTime);                     // ② 갱신
            _screen.Clear();                       // ③ 렌더링
            Render(_screen);
            _screen.Present();
            int wait = _frameMs - (int)watch.ElapsedMilliseconds;
            if (wait > 0) Thread.Sleep(wait);      // 프레임 속도 맞추기
        }
        Console.CursorVisible = true;
    }

    protected void Quit() { _quitRequested = true; }

    /// <summary>키가 눌릴 때마다 엔진이 부른다. 한 프레임에 여러 번 올 수 있다.</summary>
    /// <param name="key">눌린 키. 예: ConsoleKey.LeftArrow</param>
    protected virtual void Input(ConsoleKey key) { }

    /// <summary>매 프레임 한 번. 게임 상태(위치·점수·충돌)를 바꾼다.</summary>
    /// <param name="deltaTime">한 프레임의 시간(초). 기본 0.05</param>
    protected abstract void Update(float deltaTime);

    /// <summary>매 프레임 한 번. 지금 상태를 screen에 그린다.</summary>
    /// <param name="screen">이번 프레임의 화면 버퍼</param>
    protected abstract void Render(Screen screen);
}
