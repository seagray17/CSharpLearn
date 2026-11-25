using System;
using System.Collections.Generic;
using System.Threading;

namespace StarCraftOOP
{
    #region 1. 열거형 및 인터페이스 (약속)
    public enum MoveType { Ground, Air }
    public enum UnitAttribute { Biological, Mechanical }

    // 인터페이스: 종족을 초월한 특수 기능 약속
    public interface ICloakable
    {
        bool IsCloaked { get; }
        void ToggleCloak();
    }

    public interface IDetector
    {
        void Detect();
    }

    public interface ITransport
    {
        void LoadUnit(Unit unit);
        void UnloadAll();
    }

    public interface IWorker
    {
        void GatherResource();
    }

    public interface IBurrowable
    {
        bool IsBurrowed { get; }
        void ToggleBurrow();
    }
    #endregion

    #region 2. 최상위 추상 클래스 (Unit)
    public abstract class Unit
    {
        public string Name { get; protected set; }
        public int Hp { get; protected set; }
        public int MaxHp { get; protected set; }
        public MoveType MoveType { get; protected set; }
        public UnitAttribute Attribute { get; protected set; }
        public int X { get; protected set; }

        protected Unit(string name, int maxHp, MoveType moveType, UnitAttribute attribute)
        {
            Name = name;
            MaxHp = maxHp;
            Hp = maxHp;
            MoveType = moveType;
            Attribute = attribute;
            X = 0;
        }

        // 공통 기능: 이동 (다형성 활용 가능)
        public virtual void Move(int distance)
        {
            X += distance;
            Console.WriteLine($"[{Name}] ({MoveType}/{Attribute}) 위치 {X}로 이동했습니다.");
        }

        // 데미지 입기 (종족별 오버라이딩을 위해 virtual 선언)
        public virtual void TakeDamage(int damage)
        {
            Hp = Math.Max(0, Hp - damage);
            Console.WriteLine($"[{Name}] {damage}의 피해를 입음! (남은 체력: {Hp}/{MaxHp})");
        }

        // 매 턴(프레임) 갱신될 종족별 고유 패시브
        public abstract void UpdateTurn();

        // 상태 출력 (렌더링)
        public virtual string GetStatus()
        {
            return $"[{Name}] HP: {Hp}/{MaxHp} | 위치: {X} | 타입: {MoveType}/{Attribute}";
        }
    }
    #endregion

    #region 3. 종족별 추상 클래스 (테란, 저그, 프로토스)
    public abstract class TerranUnit : Unit
    {
        protected TerranUnit(string name, int maxHp, MoveType moveType, UnitAttribute attribute)
            : base(name, maxHp, moveType, attribute) { }

        public override void UpdateTurn()
        {
            // 테란은 기본 자동 재생이 없음 (생체는 메딕 치료, 기계는 SCV 수리 필요)
        }
    }

    public abstract class ZergUnit : Unit
    {
        protected ZergUnit(string name, int maxHp, MoveType moveType)
            : base(name, maxHp, moveType, UnitAttribute.Biological) { } // 저그는 모두 생체

        public override void UpdateTurn()
        {
            // 저그 고유 특성: 매 턴 체력 자동 재생
            if (Hp > 0 && Hp < MaxHp)
            {
                Hp = Math.Min(MaxHp, Hp + 2);
            }
        }
    }

    public abstract class ProtossUnit : Unit
    {
        public int Shield { get; protected set; }
        public int MaxShield { get; protected set; }

        protected ProtossUnit(string name, int maxHp, int maxShield, MoveType moveType, UnitAttribute attribute)
            : base(name, maxHp, moveType, attribute)
        {
            MaxShield = maxShield;
            Shield = maxShield;
        }

        // 프로토스 고유 특성: 쉴드가 먼저 피해를 흡수
        public override void TakeDamage(int damage)
        {
            if (Shield >= damage)
            {
                Shield -= damage;
                Console.WriteLine($"[{Name}] 쉴드가 {damage} 피해를 흡수! (남은 쉴드: {Shield}/{MaxShield})");
            }
            else
            {
                int remainDamage = damage - Shield;
                Shield = 0;
                base.TakeDamage(remainDamage);
            }
        }

        public override void UpdateTurn()
        {
            // 프로토스 고유 특성: 매 턴 쉴드 자동 재생
            if (Hp > 0 && Shield < MaxShield)
            {
                Shield = Math.Min(MaxShield, Shield + 3);
            }
        }

        public override string GetStatus()
        {
            return $"[{Name}] HP: {Hp}/{MaxHp} | Shield: {Shield}/{MaxShield} | 위치: {X} | 타입: {MoveType}/{Attribute}";
        }
    }
    #endregion

    #region 4. 구체적 유닛 클래스 (상속 + 인터페이스 조립)
    // 테란 일꾼: IWorker 구현
    public class SCV : TerranUnit, IWorker
    {
        public SCV() : base("SCV", 60, MoveType.Ground, UnitAttribute.Mechanical) { }
        public void GatherResource() => Console.WriteLine($"[{Name}] 미네랄을 채취합니다.");
    }

    // 테란 공중 유닛: ICloakable 구현
    public class Wraith : TerranUnit, ICloakable
    {
        public bool IsCloaked { get; private set; }
        public Wraith() : base("레이스", 120, MoveType.Air, UnitAttribute.Mechanical) { }

        public void ToggleCloak()
        {
            IsCloaked = !IsCloaked;
            Console.WriteLine($"[{Name}] 클로킹 모드: {(IsCloaked ? "활성화(은신)" : "비활성화")}");
        }
    }

    // 저그 지상 유닛: IBurrowable 구현
    public class Zergling : ZergUnit, IBurrowable
    {
        public bool IsBurrowed { get; private set; }
        public Zergling() : base("저글링", 35, MoveType.Ground) { }

        public void ToggleBurrow()
        {
            IsBurrowed = !IsBurrowed;
            Console.WriteLine($"[{Name}] 버로우(잠복): {(IsBurrowed ? "땅속으로 숨음" : "지상으로 나옴")}");
        }

        public override void Move(int distance)
        {
            if (IsBurrowed)
            {
                Console.WriteLine($"[{Name}] 버로우 상태에서는 이동할 수 없습니다!");
                return;
            }
            base.Move(distance);
        }
    }

    // 저그 공중 유닛: ITransport, IDetector 다중 구현
    public class Overlord : ZergUnit, ITransport, IDetector
    {
        private List<Unit> cargo = new List<Unit>();
        public Overlord() : base("오버로드", 200, MoveType.Air) { }

        public void Detect() => Console.WriteLine($"[{Name}] 주변의 은신/잠복 유닛을 탐지합니다.");

        public void LoadUnit(Unit unit)
        {
            if (unit.MoveType == MoveType.Ground)
            {
                cargo.Add(unit);
                Console.WriteLine($"[{Name}] {unit.Name} 탑승 완료. (현재 탑승: {cargo.Count}기)");
            }
            else
            {
                Console.WriteLine($"[{Name}] 공중 유닛은 태울 수 없습니다.");
            }
        }

        public void UnloadAll()
        {
            Console.WriteLine($"[{Name}] 탑승 유닛 {cargo.Count}기를 모두 하차시켰습니다.");
            cargo.Clear();
        }
    }

    // 프로토스 공중 유닛: ICloakable, IDetector 다중 구현
    public class Observer : ProtossUnit, ICloakable, IDetector
    {
        public bool IsCloaked { get; private set; } = true; // 옵저버는 영구 은신
        public Observer() : base("옵저버", 40, 20, MoveType.Air, UnitAttribute.Mechanical) { }

        public void ToggleCloak() => Console.WriteLine($"[{Name}] 패시브 영구 은신 상태입니다.");
        public void Detect() => Console.WriteLine($"[{Name}] 정밀 감지기 가동! 은신 유닛을 찾아냅니다.");
    }
    #endregion

    #region 5. 게임 엔진 (입력 - 갱신 - 렌더링 루프)
    class Program
    {
        static List<Unit> army = new List<Unit>();
        static bool isRunning = true;
        static int turn = 1;

        static void Main(string[] args)
        {
            InitGame();

            // 게임 루프 (Game Loop)
            while (isRunning)
            {
                Render();   // 1. 화면 출력
                Input();    // 2. 사용자 입력 처리
                Update();   // 3. 상태 갱신 (종족 패시브 등)
            }
        }

        static void InitGame()
        {
            // 다형성: 서로 다른 유닛들을 하나의 Unit 리스트로 관리
            army.Add(new SCV());
            army.Add(new Wraith());
            army.Add(new Zergling());
            army.Add(new Overlord());
            army.Add(new Observer());

            // 테스트를 위해 일부 유닛에게 초기 데미지 부여 (재생 확인용)
            army[2].TakeDamage(15); // 저글링 체력 감소
            army[4].TakeDamage(15); // 옵저버 쉴드 감소
            Console.Clear();
        }

        // [렌더링] 현재 모든 유닛의 상태를 화면에 그림
        static void Render()
        {
            Console.WriteLine("=====================================================================");
            Console.WriteLine($" [Turn {turn}] 스타크래프트 다형성 시뮬레이터");
            Console.WriteLine("=====================================================================");
            for (int i = 0; i < army.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {army[i].GetStatus()}");
            }
            Console.WriteLine("---------------------------------------------------------------------");
            Console.WriteLine("명령어: [1] 전체 이동  [2] 은신 토글(ICloakable)  [3] 잠복 토글(IBurrowable)");
            Console.WriteLine("        [4] 탐지 가동(IDetector)  [5] 자원 채취(IWorker)  [6] 수송 테스트(ITransport)");
            Console.WriteLine("        [7] 전체 피격 테스트(10 데미지)  [0] 종료");
            Console.Write("입력 > ");
        }

        // [입력] 유저의 키 입력을 받아 다형성/인터페이스 기반으로 명령 수행
        static void Input()
        {
            string input = Console.ReadLine();
            Console.WriteLine();

            switch (input)
            {
                case "1":
                    // 상속의 다형성: 모든 유닛이 각자의 Move() 규칙에 따라 이동
                    foreach (var unit in army) unit.Move(5);
                    break;

                case "2":
                    // 인터페이스의 다형성: ICloakable 자격증이 있는 유닛만 은신 실행
                    foreach (var unit in army)
                        if (unit is ICloakable cloakable) cloakable.ToggleCloak();
                    break;

                case "3":
                    foreach (var unit in army)
                        if (unit is IBurrowable burrowable) burrowable.ToggleBurrow();
                    break;

                case "4":
                    foreach (var unit in army)
                        if (unit is IDetector detector) detector.Detect();
                    break;

                case "5":
                    foreach (var unit in army)
                        if (unit is IWorker worker) worker.GatherResource();
                    break;

                case "6":
                    // 오버로드(ITransport)를 찾아 지상 유닛 태우기
                    Overlord overlord = army.Find(u => u is ITransport) as Overlord;
                    if (overlord != null)
                    {
                        foreach (var unit in army)
                            if (unit != overlord) overlord.LoadUnit(unit);
                    }
                    break;

                case "7":
                    foreach (var unit in army) unit.TakeDamage(10);
                    break;

                case "0":
                    isRunning = false;
                    Console.WriteLine("게임을 종료합니다.");
                    return;

                default:
                    Console.WriteLine("잘못된 입력입니다. 턴만 경과합니다.");
                    break;
            }

            Console.WriteLine("\n(다음 턴으로 넘어가려면 Enter를 누르세요...)");
            Console.ReadLine();
        }

        // [갱신] 매 턴마다 각 종족의 패시브(체력/쉴드 재생) 연산
        static void Update()
        {
            turn++;
            foreach (var unit in army)
            {
                unit.UpdateTurn(); // 저그는 체력 회복, 프로토스는 쉴드 회복
            }
            Console.Clear();
        }
    }
    #endregion
}
