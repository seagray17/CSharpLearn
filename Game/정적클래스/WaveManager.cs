namespace ConsoleShooter;

/// <summary>
/// 몬스터의 생존 수를 관리하고 전멸 시 다음 Wave를 발생시키는 정적 클래스
/// </summary>
public static class WaveManager
{
    public static int CurrentWave { get; private set; } = 0;
    public static int AliveCount { get; private set; } = 0;
    public static bool IsWaveCleared => AliveCount <= 0;

    /// <summary>
    /// 다음 Wave를 시작하고 n마리의 몬스터를 생성합니다.
    /// </summary>
    public static void SpawnWave(List<Monster> monsters, int mapWidth)
    {
        CurrentWave++;
        int spawnCount = CurrentWave + 2; // Wave 1: 3마리, Wave 2: 4마리 ... (n마리)

        monsters.Clear();
        Random random = new();

        for (int i = 0; i < spawnCount; i++)
        {
            int x = random.Next(1, mapWidth - 1);
            int y = random.Next(1, 6);
            monsters.Add(new Monster(x, y));
        }

        AliveCount = monsters.Count;
    }

    /// <summary>
    /// 몬스터가 사망할 때 호출되어 생존 수를 감소시킵니다.
    /// </summary>
    public static void OnMonsterKilled()
    {
        if (AliveCount > 0)
        {
            AliveCount--;
        }
    }
}
