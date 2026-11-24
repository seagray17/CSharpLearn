namespace ConsoleShooter;

/// <summary>
/// WASD로 이동하고 스페이스바로 직선 총알을 발사하는 플레이어 클래스
/// </summary>
public class Player(int startX, int startY)
{
    public int X { get; private set; } = startX;
    public int Y { get; private set; } = startY;

    public void Move(int dx, int dy, int maxWidth, int maxHeight)
    {
        X = Math.Clamp(X + dx, 0, maxWidth - 1);
        Y = Math.Clamp(Y + dy, 0, maxHeight - 1);
    }

    public Bullet Shoot()
    {
        return new Bullet(X, Y - 1);
    }
}
