using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace ConsoleTetris
{
    public class TetrisGame
    {
        // 1. 기본 블록 클래스
        public class 블록
        {
            public string shape = "[]";

            public 블록(string shape)
            {
                this.shape = shape;
            }
        }

        // 2. 시간 제한이 있어서 일정 시간 후에 파괴되는 블록 클래스
        public class TimerBlock : 블록
        {
            public int deathTimer = 5;
            public double remainingTime;
            public int targetStopY; // 생성 시 미리 정해지는 목표 고정 Y좌표

            public TimerBlock(string shape, int targetStopY, int deathTimer = 5) : base(shape)
            {
                this.targetStopY = targetStopY;
                this.deathTimer = deathTimer;
                this.remainingTime = deathTimer;
            }

            // 시간이 지났을 때 없어지는 기능
            public bool TimeDeath(double deltaTime)
            {
                remainingTime -= deltaTime;
                return remainingTime <= 0;
            }
        }

        // 3. 떨어지다가 중간에 멈춰서 조종이 잠기는 블록 클래스
        public class StopBlock : 블록
        {
            public int stopTriggerY;

            public StopBlock(string shape, int stopTriggerY) : base(shape)
            {
                this.stopTriggerY = stopTriggerY;
            }
        }

        // 현재 조종 중이거나 공중에 정지해 있는 테트로미노 조각 정보
        private class ActivePiece
        {
            public int[,] Shape;
            public int X;
            public int Y;
            public 블록 BlockType;
            public bool IsSuspended;

            public bool IsStopBlock => BlockType is StopBlock;
            public bool IsTimerBlock => BlockType is TimerBlock;

            public ActivePiece(int[,] shape, int x, int y, 블록 blockType)
            {
                Shape = shape;
                X = x;
                Y = y;
                BlockType = blockType;
                IsSuspended = false;
            }

            public ActivePiece Clone()
            {
                int rows = Shape.GetLength(0);
                int cols = Shape.GetLength(1);
                int[,] copy = new int[rows, cols];
                Array.Copy(Shape, copy, Shape.Length);
                ActivePiece p = new ActivePiece(copy, X, Y, BlockType);
                p.IsSuspended = this.IsSuspended;
                return p;
            }
        }

        // 게임 보드 크기 설정
        private const int Width = 10;
        private const int Height = 20;

        private 블록[,] board = new 블록[Height, Width];
        private ActivePiece currentPiece;
        private ActivePiece suspendedPiece; // 공중에 멈춰 대기 중인 StopBlock

        private Random random = new Random();
        private bool isGameOver = false;
        private int score = 0;
        private string statusMessage = "일반 낙하 중";

        // 타이머 및 낙하 속도 관리
        private Stopwatch stopwatch = new Stopwatch();
        private double lastFrameTime = 0;
        private double fallAccumulator = 0;
        private double fallInterval = 0.5;

        // 테트로미노 7종 모양 정의
        private static readonly List<int[,]> TetrominoShapes = new List<int[,]>
        {
            new int[,] { { 1, 1, 1, 1 } },                         // I
            new int[,] { { 1, 1 }, { 1, 1 } },                       // O
            new int[,] { { 0, 1, 0 }, { 1, 1, 1 } },                 // T
            new int[,] { { 1, 0, 0 }, { 1, 1, 1 } },                 // J
            new int[,] { { 0, 0, 1 }, { 1, 1, 1 } },                 // L
            new int[,] { { 0, 1, 1 }, { 1, 1, 0 } },                 // S
            new int[,] { { 1, 1, 0 }, { 0, 1, 1 } }                  // Z
        };

        // 프로그램 진입점
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.CursorVisible = false;
            TetrisGame game = new TetrisGame();
            game.Run();
        }

        // 메인 게임 루프 (입력 -> 갱신 -> 렌더링)
        public void Run()
        {
            SpawnNewPiece();
            stopwatch.Start();
            lastFrameTime = stopwatch.Elapsed.TotalSeconds;

            while (!isGameOver)
            {
                double currentTime = stopwatch.Elapsed.TotalSeconds;
                double deltaTime = currentTime - lastFrameTime;
                lastFrameTime = currentTime;

                ProcessInput();
                Update(deltaTime);
                Render();

                Thread.Sleep(30);
            }

            Console.SetCursorPosition(0, Height + 5);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"게임 오버! 최종 점수: {score}점                    ");
            Console.ResetColor();
            Console.CursorVisible = true;
        }

        // 1. 입력 처리 (Input)
        private void ProcessInput()
        {
            while (Console.KeyAvailable)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                switch (keyInfo.Key)
                {
                    case ConsoleKey.LeftArrow:
                        TryMove(-1, 0);
                        break;
                    case ConsoleKey.RightArrow:
                        TryMove(1, 0);
                        break;
                    case ConsoleKey.UpArrow:
                        TryRotate();
                        break;
                    case ConsoleKey.DownArrow:
                        // StopBlock은 아래 화살표 가속 제한, TimerBlock 및 일반 블록은 허용
                        if (!currentPiece.IsStopBlock)
                        {
                            StepDown();
                        }
                        break;
                    case ConsoleKey.Spacebar:
                        // StopBlock은 스페이스 즉시 낙하 제한, TimerBlock은 지정된 Y좌표까지만 낙하
                        if (!currentPiece.IsStopBlock)
                        {
                            HardDrop();
                        }
                        break;
                }
            }
        }

        // 2. 게임 상태 갱신 (Update)
        private void Update(double deltaTime)
        {
            // 보드에 완전히 안착(고정)된 TimerBlock만 시간 경과 및 파괴 처리
            UpdateBoardTimerBlocks(deltaTime);

            // 현재 떨어지는 StopBlock이 목표 높이에 도달했는지 확인
            if (TrySuspendCurrentStopBlock())
            {
                return;
            }

            // 현재 떨어지는 TimerBlock이 미리 지정된 목표 Y좌표에 도달했는지 확인
            if (TryLockTimerBlockAtTargetY())
            {
                return;
            }

            // 블록 자동 하강 타이머
            fallAccumulator += deltaTime;
            if (fallAccumulator >= fallInterval)
            {
                fallAccumulator = 0;
                StepDown();
            }
        }

        // 3. 화면 출력 (Render) - 모든 셀을 반각 2칸(2 bytes)으로 고정해 테두리 밀림 방지
        private void Render()
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine($"=== C# 테트리스 (점수: {score}) ===          ");
            Console.WriteLine($"상태: {statusMessage,-42}");

            for (int y = 0; y < Height; y++)
            {
                string rowStr = "<!";
                for (int x = 0; x < Width; x++)
                {
                    string cell = GetCellDisplay(x, y);
                    rowStr += cell;
                }
                rowStr += "!>";
                Console.WriteLine(rowStr + "  ");
            }

            Console.WriteLine("<!====================!>  ");
            Console.WriteLine("조작: ←/→ 이동 | ↑ 회전 | ↓ 소프트드롭 | Space 하드드롭        ");
            Console.WriteLine("※ TimerBlock(숫자)은 지정된 높이에 도달하면 고정되며 그때부터 타이머가 줄어듭니다.");
            Console.WriteLine("※ StopBlock(정)은 공중 정지 후 일반 블록과 합체 가능 (TimerBlock과는 합체 불가)   ");
        }

        // 각 좌표별 2칸 고정 문자열 반환 (반각 2글자 = 전각 1글자 폭)
        private string GetCellDisplay(int x, int y)
        {
            // 1. 현재 조종 중인 블록 확인
            if (IsPointInPiece(currentPiece, x, y))
            {
                if (currentPiece.BlockType is TimerBlock activeTb)
                {
                    // 떨어지는 중에는 아직 타이머가 흐르지 않으므로 초기 시간 표시
                    return $"{activeTb.deathTimer} ";
                }
                if (currentPiece.BlockType is StopBlock)
                {
                    return "정";
                }
                return "[]";
            }

            // 2. 공중에 정지해 대기 중인 StopBlock 확인
            if (suspendedPiece != null && IsPointInPiece(suspendedPiece, x, y))
            {
                return "정";
            }

            // 3. 보드에 고정된 블록 확인
            if (board[y, x] != null)
            {
                if (board[y, x] is TimerBlock tb)
                {
                    int displaySec = Math.Max(1, (int)Math.Ceiling(tb.remainingTime));
                    return $"{displaySec} ";
                }
                return "[]";
            }

            return ". ";
        }

        private bool IsPointInPiece(ActivePiece piece, int x, int y)
        {
            if (piece == null) return false;
            int rows = piece.Shape.GetLength(0);
            int cols = piece.Shape.GetLength(1);
            int py = y - piece.Y;
            int px = x - piece.X;
            return py >= 0 && py < rows && px >= 0 && px < cols && piece.Shape[py, px] != 0;
        }

        // 한 칸 아래로 이동 시도
        private void StepDown()
        {
            // TimerBlock이 이미 지정된 목표 Y좌표에 도달해 있다면 즉시 고정
            if (TryLockTimerBlockAtTargetY())
            {
                return;
            }

            if (TryMove(0, 1))
            {
                // 이동 직후 StopBlock 정지 높이 또는 TimerBlock 목표 Y좌표 도달 체크
                if (!TrySuspendCurrentStopBlock())
                {
                    TryLockTimerBlockAtTargetY();
                }
            }
            else
            {
                // 바닥이나 다른 블록에 닿아 고정
                LockCurrentPiece();
                ClearFullLines();
                HandleNextBlockTurn();
            }
        }

        // 스페이스바 즉시 낙하 (TimerBlock은 지정된 Y좌표까지만 낙하하고 멈춤)
        private void HardDrop()
        {
            if (currentPiece.IsTimerBlock && currentPiece.BlockType is TimerBlock tb)
            {
                while (currentPiece.Y < tb.targetStopY && TryMove(0, 1))
                {
                }
                LockCurrentPiece();
                ClearFullLines();
                HandleNextBlockTurn();
                return;
            }

            while (TryMove(0, 1))
            {
            }
            LockCurrentPiece();
            ClearFullLines();
            HandleNextBlockTurn();
        }

        // TimerBlock이 생성 시 정해진 targetStopY에 도달했는지 확인 후 공중/해당 위치에 고정
        private bool TryLockTimerBlockAtTargetY()
        {
            if (currentPiece != null && currentPiece.BlockType is TimerBlock tb)
            {
                if (currentPiece.Y >= tb.targetStopY)
                {
                    LockCurrentPiece();
                    ClearFullLines();
                    HandleNextBlockTurn();
                    return true;
                }
            }
            return false;
        }

        // 이동 시도 (공중에 멈춘 StopBlock과 부딪히면 일반 블록끼리만 합체)
        private bool TryMove(int dx, int dy)
        {
            int newX = currentPiece.X + dx;
            int newY = currentPiece.Y + dy;

            if (!IsValidBoardPosition(currentPiece.Shape, newX, newY))
            {
                return false;
            }

            // 공중에 멈춘 StopBlock과 겹치는지 확인
            if (suspendedPiece != null && IsOverlappingPiece(currentPiece.Shape, newX, newY, suspendedPiece))
            {
                // TimerBlock(터지는 블록)과 StopBlock(멈추는 블록)은 서로 붙지 않도록 이동 차단
                if (currentPiece.IsTimerBlock)
                {
                    return false;
                }

                currentPiece.X = newX;
                currentPiece.Y = newY;
                MergeWithSuspendedPiece();
                return true;
            }

            currentPiece.X = newX;
            currentPiece.Y = newY;
            return true;
        }

        // 회전 시도 (회전 시 공중에 멈춘 StopBlock과 닿으면 일반 블록끼리만 합체)
        private void TryRotate()
        {
            int rows = currentPiece.Shape.GetLength(0);
            int cols = currentPiece.Shape.GetLength(1);
            int[,] rotated = new int[cols, rows];

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    rotated[c, rows - 1 - r] = currentPiece.Shape[r, c];
                }
            }

            if (!IsValidBoardPosition(rotated, currentPiece.X, currentPiece.Y))
            {
                return;
            }

            if (suspendedPiece != null && IsOverlappingPiece(rotated, currentPiece.X, currentPiece.Y, suspendedPiece))
            {
                // TimerBlock은 회전 시에도 StopBlock과 합체하지 않음
                if (currentPiece.IsTimerBlock)
                {
                    return;
                }

                currentPiece.Shape = rotated;
                MergeWithSuspendedPiece();
                return;
            }

            currentPiece.Shape = rotated;
        }

        // StopBlock이 지정된 높이에 도달하면 공중에 멈추고 일반 블록으로 전환 대기
        private bool TrySuspendCurrentStopBlock()
        {
            if (currentPiece != null && currentPiece.BlockType is StopBlock stopBlock && !currentPiece.IsSuspended)
            {
                if (currentPiece.Y >= stopBlock.stopTriggerY)
                {
                    currentPiece.IsSuspended = true;
                    // 한 번 멈춘 뒤에는 일반 블록으로 전환
                    currentPiece.BlockType = new 블록("[]");
                    suspendedPiece = currentPiece;
                    statusMessage = "StopBlock 공중 정지! 새 블록으로 합체 또는 회피 가능";
                    SpawnNewPiece(allowSpecialBlock: false);
                    return true;
                }
            }
            return false;
        }

        // 멈춘 블록과 떨어지는 일반 블록이 만났을 때 일반 블록("[]") 하나로 합체
        private void MergeWithSuspendedPiece()
        {
            int minX = Math.Min(currentPiece.X, suspendedPiece.X);
            int minY = Math.Min(currentPiece.Y, suspendedPiece.Y);
            int maxX = Math.Max(
                currentPiece.X + currentPiece.Shape.GetLength(1) - 1,
                suspendedPiece.X + suspendedPiece.Shape.GetLength(1) - 1
            );
            int maxY = Math.Max(
                currentPiece.Y + currentPiece.Shape.GetLength(0) - 1,
                suspendedPiece.Y + suspendedPiece.Shape.GetLength(0) - 1
            );

            int mergedRows = maxY - minY + 1;
            int mergedCols = maxX - minX + 1;
            int[,] mergedShape = new int[mergedRows, mergedCols];

            for (int r = 0; r < currentPiece.Shape.GetLength(0); r++)
            {
                for (int c = 0; c < currentPiece.Shape.GetLength(1); c++)
                {
                    if (currentPiece.Shape[r, c] != 0)
                    {
                        mergedShape[currentPiece.Y + r - minY, currentPiece.X + c - minX] = 1;
                    }
                }
            }

            for (int r = 0; r < suspendedPiece.Shape.GetLength(0); r++)
            {
                for (int c = 0; c < suspendedPiece.Shape.GetLength(1); c++)
                {
                    if (suspendedPiece.Shape[r, c] != 0)
                    {
                        mergedShape[suspendedPiece.Y + r - minY, suspendedPiece.X + c - minX] = 1;
                    }
                }
            }

            currentPiece = new ActivePiece(mergedShape, minX, minY, new 블록("[]"));
            suspendedPiece = null;
            fallAccumulator = 0;
            statusMessage = "블록 합체 완료! 일반 블록으로 함께 낙하 중";
        }

        // 블록 안착 후 다음 차례 결정
        private void HandleNextBlockTurn()
        {
            if (suspendedPiece != null)
            {
                currentPiece = suspendedPiece;
                currentPiece.IsSuspended = false;
                suspendedPiece = null;
                fallAccumulator = 0;
                statusMessage = "멈춰 있던 블록(일반 블록 전환됨) 조종 재개!";
            }
            else
            {
                SpawnNewPiece();
            }
        }

        // 보드 경계 및 이미 쌓인 블록과의 충돌 검사
        private bool IsValidBoardPosition(int[,] shape, int startX, int startY)
        {
            int rows = shape.GetLength(0);
            int cols = shape.GetLength(1);

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (shape[r, c] == 0) continue;

                    int boardX = startX + c;
                    int boardY = startY + r;

                    if (boardX < 0 || boardX >= Width || boardY < 0 || boardY >= Height)
                        return false;

                    if (board[boardY, boardX] != null)
                        return false;
                }
            }
            return true;
        }

        // 두 블록 조각이 겹치는지 검사
        private bool IsOverlappingPiece(int[,] shape, int startX, int startY, ActivePiece other)
        {
            int rows = shape.GetLength(0);
            int cols = shape.GetLength(1);

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (shape[r, c] == 0) continue;

                    int boardX = startX + c;
                    int boardY = startY + r;

                    if (IsPointInPiece(other, boardX, boardY))
                        return true;
                }
            }
            return false;
        }

        // 현재 조종 블록을 보드에 고정 (TimerBlock은 이 시점부터 시간이 흐르기 시작함)
        private void LockCurrentPiece()
        {
            int rows = currentPiece.Shape.GetLength(0);
            int cols = currentPiece.Shape.GetLength(1);

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (currentPiece.Shape[r, c] == 0) continue;

                    int boardX = currentPiece.X + c;
                    int boardY = currentPiece.Y + r;

                    if (boardY >= 0 && boardY < Height && boardX >= 0 && boardX < Width)
                    {
                        if (currentPiece.BlockType is TimerBlock tb)
                        {
                            // 고정되는 순간부터 온전히 deathTimer(5초) 카운트 시작
                            board[boardY, boardX] = new TimerBlock(tb.shape, tb.targetStopY, tb.deathTimer);
                        }
                        else
                        {
                            board[boardY, boardX] = new 블록("[]");
                        }
                    }
                }
            }
        }

        // 보드에 고정된 TimerBlock 시간 경과 및 파괴 처리
        private void UpdateBoardTimerBlocks(double deltaTime)
        {
            bool anyDestroyed = false;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (board[y, x] is TimerBlock timerBlock)
                    {
                        if (timerBlock.TimeDeath(deltaTime))
                        {
                            board[y, x] = null;
                            anyDestroyed = true;
                        }
                    }
                }
            }

            if (anyDestroyed)
            {
                ApplyGravityToFloatingBlocks();
                ClearFullLines();
            }
        }

        // 파괴된 블록 위의 블록들을 아래 빈 공간으로 자동 하강
        private void ApplyGravityToFloatingBlocks()
        {
            for (int x = 0; x < Width; x++)
            {
                int writeY = Height - 1;

                for (int readY = Height - 1; readY >= 0; readY--)
                {
                    if (board[readY, x] != null)
                    {
                        블록 temp = board[readY, x];
                        board[readY, x] = null;
                        board[writeY, x] = temp;
                        writeY--;
                    }
                }
            }
        }

        // 가득 찬 가로줄 제거 및 점수 획득
        private void ClearFullLines()
        {
            int linesCleared = 0;

            for (int y = Height - 1; y >= 0; y--)
            {
                bool isFull = true;
                for (int x = 0; x < Width; x++)
                {
                    if (board[y, x] == null)
                    {
                        isFull = false;
                        break;
                    }
                }

                if (isFull)
                {
                    linesCleared++;
                    for (int moveY = y; moveY > 0; moveY--)
                    {
                        for (int x = 0; x < Width; x++)
                        {
                            board[moveY, x] = board[moveY - 1, x];
                        }
                    }
                    for (int x = 0; x < Width; x++)
                    {
                        board[0, x] = null;
                    }
                    y++;
                }
            }

            if (linesCleared > 0)
            {
                score += linesCleared * 100;
            }
        }

        // 새 블록 생성
        private void SpawnNewPiece(bool allowSpecialBlock = true)
        {
            int[,] shape = TetrominoShapes[random.Next(TetrominoShapes.Count)];
            int startX = (Width - shape.GetLength(1)) / 2;
            int startY = 0;

            블록 blockType;
            int roll = random.Next(100);

            if (allowSpecialBlock && roll < 30)
            {
                // 30% 확률로 StopBlock 생성 (Y좌표 6~10 사이에서 공중 정지)
                int triggerY = random.Next(6, 11);
                blockType = new StopBlock("정", triggerY);
                statusMessage = $"StopBlock 등장! (Y={triggerY}에서 정지 예정)";
            }
            else if (roll < 65)
            {
                // 35% 확률로 TimerBlock 생성 (Y좌표 8~16 사이에서 미리 정해진 위치에 멈춤)
                int targetY = random.Next(8, 17);
                blockType = new TimerBlock("[]", targetY, 5);
                statusMessage = $"TimerBlock 등장! (Y={targetY}에서 고정 후 5초 뒤 파괴)";
            }
            else
            {
                blockType = new 블록("[]");
                if (suspendedPiece == null)
                {
                    statusMessage = "일반 블록 낙하 중";
                }
            }

            currentPiece = new ActivePiece(shape, startX, startY, blockType);
            fallAccumulator = 0;

            if (!IsValidBoardPosition(currentPiece.Shape, currentPiece.X, currentPiece.Y) ||
                (suspendedPiece != null && IsOverlappingPiece(currentPiece.Shape, currentPiece.X, currentPiece.Y, suspendedPiece)))
            {
                isGameOver = true;
            }
        }
    }
}
