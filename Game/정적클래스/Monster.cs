namespace ConsoleShooter;

/// <summary>
/// 웨이브마다 생성되는 몬스터 클래스
/// </summary>
public class Monster(int x, int y)
{
    public int X { get; private set; } = x;
    public int Y { get; private set; } = y;
}
