namespace ConsoleShooter;

/// <summary>
/// 발사 후 위쪽 직선 방향으로 날아가는 총알 클래스
/// </summary>
public class Bullet(int x, int y)
{
    public int X { get; private set; } = x;
    public int Y { get; private set; } = y;

    public void Update()
    {
        Y--; // 직선(위쪽)으로 이동
    }

    public bool IsOutOfBounds()
    {
        return Y < 0;
    }
}
