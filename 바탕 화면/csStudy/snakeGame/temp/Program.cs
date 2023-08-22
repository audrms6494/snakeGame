namespace temp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static ThirdWeekHomeworkGameOne;

    public class Program
    {
        public static void Main(string[] args)
        {
            int x = Console.WindowWidth;
            int y = Console.WindowHeight;
            // 여유를 줘야 글씨가 보인다.
            ThirdWeekHomeworkGameOne game = new ThirdWeekHomeworkGameOne(x - 3, y - 5);
            game.GameStart();
            Console.ReadLine();
        }
    }

    /// <summary>
    /// 스네이크
    /// </summary>
    public class ThirdWeekHomeworkGameOne
    {
        protected int maxX; protected int maxY;
        public ThirdWeekHomeworkGameOne(int x, int y)
        {
            maxX = x; maxY = y;
        }

        public void GameStart()
        {
            Snake snake = new Snake(new Point(10, 10, 'o'), 4, Direction.RIGHT, 'x');
            snake.Draw();

            FoodCreator foodCreator = new FoodCreator(maxX, maxY, '*');

            // 벽 그리기
            Wall wall = new Wall(maxX, maxY);
            wall.Draw();

            bool counterFlag = false;
            Point food = foodCreator.CreateFood(snake);
            food.Draw();

            while (counterFlag == false)
            {
                if (snake.IsHit(food))
                {
                    snake.EatFood(food);
                    food = foodCreator.CreateFood(snake);
                    food.Draw();
                }
                // 키 입력에 따라 뱀 방향 정하기
                // 입력이 없는 경우에 자동으로 방향 값 정하기
                snake.ReadWait();

                // 이동하기 전에 부딪히는지 체크하기
                counterFlag = snake.CheckAhead(maxX, maxY);
                if (counterFlag == true) break;

                // 뱀 이동하기
                snake.Move();

                // 뱀의 상태 출력하기(길이, 먹은 음식의 수)
                Console.SetCursorPosition(10, maxY + 1);
                Console.Write($"snake length={snake.Length}\tfood count={snake.FoodCount}");
            }
            Console.SetCursorPosition(10, maxY + 2);
            Console.Write($"Game Over!");
        }

        public class Point
        {
            public int x { get; set; }
            public int y { get; set; }
            public char sym { get; set; }

            public Point(int _x, int _y, char _sym)
            {
                x = _x;
                y = _y;
                sym = _sym;
            }

            /// <summary>
            /// 위치 정보에 따라 글씨 그리기
            /// </summary>
            public virtual void Draw()
            {
                Console.SetCursorPosition(x, y);
                Console.Write(sym);
            }

            /// <summary>
            /// 글씨만 지우는 걸로 변경함.
            /// </summary>
            public virtual void Clear()
            {
                char temp = sym;
                sym = ' ';
                Draw();
                sym = temp;
            }

            /// <summary>
            /// 주어진 위치와 위치가 똑같은지 확인
            /// </summary>
            public virtual bool IsHit(Point p)
            {
                return p.x == x && p.y == y;
            }
        }

        public enum Direction
        {
            LEFT,
            RIGHT,
            UP,
            DOWN
        }

        public class Snake : Point
        {
            public int Length { get; set; }
            public Direction Direc { get => direc; set { direc = value; } }
            private Direction direc;
            private List<Point> body;
            private char bodyChar;
            public int FoodCount { get; private set; }

            public Snake(Point _head, int _length, Direction _direc, char _body) : base(_head.x, _head.y, _head.sym)
            {
                Length = _length;
                direc = _direc;
                bodyChar = _body;

                int bodyX = 0, bodyY = 0;
                switch (direc)
                {
                    case Direction.LEFT:
                        bodyX = +1;
                        break;
                    case Direction.RIGHT:
                        bodyX = -1;
                        break;
                    case Direction.UP:
                        bodyY = -1;
                        break;
                    case Direction.DOWN:
                        bodyY = 1;
                        break;
                }
                body = new List<Point>();
                for (int i = 1; i <= Length; ++i)
                {
                    body.Add(new Point(x + bodyX * i, y + bodyY * i, _body));
                }
            }

            public void Move()
            {
                // 뱀 지우기
                Clear();
                // 몸통 위치 정보 앞에걸로 한칸씩 옮기기
                // 먹이를 먹은 경우에 먹이의 위치가 꼬리 맨끝으로 이동하게 된다.
                for (int i = Length - 1; i > 0; --i)
                {
                    body[i].x = body[i - 1].x;
                    body[i].y = body[i - 1].y;
                }
                body[0].x = x;
                body[0].y = y;

                // 방향에 따라 머리 위치 수정
                switch (Direc)
                {
                    case Direction.LEFT:
                        --x;
                        break;
                    case Direction.RIGHT:
                        ++x;
                        break;
                    case Direction.UP:
                        --y;
                        break;
                    case Direction.DOWN:
                        ++y;
                        break;
                }
                // 뱀 그리기
                Draw();
            }

            public override void Draw()
            {
                // 머리 그리기
                base.Draw();
                // 몸통 그리기
                foreach (Point p in body)
                {
                    p.Draw();
                }
            }

            public override void Clear()
            {
                // 머리 지우기
                base.Clear();
                // 몸통 지우기
                foreach (Point p in body)
                {
                    p.Clear();
                }
            }

            public override bool IsHit(Point p)
            {
                bool ret = false;

                // 머리 지점 체크
                ret = base.IsHit(p);
                if (ret) { return ret; }
                // 몸통 지점 체크
                foreach (Point p2 in body)
                {
                    ret = p2.IsHit(p);
                    if (ret) { return ret; }
                }
                return ret;
            }

            public bool CheckAhead(int _maxX, int _maxY)
            {
                bool ret = false;
                Point tempPoint = new Point(x, y, sym);
                switch (direc)
                {
                    case Direction.LEFT:
                        --tempPoint.x;
                        break;
                    case Direction.RIGHT:
                        ++tempPoint.x;
                        break;
                    case Direction.UP:
                        --tempPoint.y;
                        break;
                    case Direction.DOWN:
                        ++tempPoint.y;
                        break;
                }
                if (tempPoint.x <= 0 || tempPoint.x >= _maxX || tempPoint.y <= 0 || tempPoint.y >= _maxY)
                    return true;
                ret = IsHit(tempPoint);
                return ret;
            }

            public void EatFood(Point _food)
            {
                ++Length;
                _food.sym = bodyChar;
                body.Add(_food);
                ++FoodCount;
            }

            public void ReadWait()
            {
                int count = 0;
                while (count < 10)
                {
                    if (Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(true).Key;
                        switch (key)
                        {
                            case ConsoleKey.W:
                            case ConsoleKey.UpArrow:
                                if (Direc != Direction.DOWN) Direc = Direction.UP;
                                else continue;
                                break;
                            case ConsoleKey.S:
                            case ConsoleKey.DownArrow:
                                if (Direc != Direction.UP) Direc = Direction.DOWN;
                                else continue;
                                break;
                            case ConsoleKey.D:
                            case ConsoleKey.RightArrow:
                                if (Direc != Direction.LEFT) Direc = Direction.RIGHT;
                                else continue;
                                break;
                            case ConsoleKey.A:
                            case ConsoleKey.LeftArrow:
                                if (Direc != Direction.RIGHT) Direc = Direction.LEFT;
                                else continue;
                                break;
                        }
                        break;
                    }
                    else
                    {
                        ++count;
                        Thread.Sleep(10);
                    }
                }
            }
        }

        public class FoodCreator
        {
            private char sym;
            private Random rnd;
            private int maxX;
            private int maxY;
            public FoodCreator(int _maxX, int _maxY, char _sym)
            {
                sym = _sym;
                rnd = new Random();
                maxX = _maxX;
                maxY = _maxY;
            }

            /// <summary>
            /// 뱀이 있는 지점에 먹이를 생성하지 않기 위해서 뱀을 변수로 받는다.
            /// </summary>
            /// <param name="_snake">뱀</param>
            /// <returns>생성된 먹이</returns>
            public Point CreateFood(Snake _snake)
            {
                Point ret;
                do
                {
                    ret = new Point(rnd.Next(1, maxX), rnd.Next(1, maxY), sym);
                }
                while (_snake.IsHit(ret));

                return ret;
            }
        }

        public class Wall
        {
            private Dictionary<Point, char> walls;
            public Wall(int _xLimit, int _yLimit)
            {
                // 중복으로 들어갈 수도 있어 Dictionary로 구성
                // 중복된 키는 추가되지 않는다
                walls = new Dictionary<Point, char>();
                for (int i = 0; i <= _yLimit; ++i)
                {
                    for (int j = 0; j <= _xLimit; ++j)
                    {
                        // 첫줄 벽
                        if (i == 0)
                        {
                            walls.Add(new Point(j, i, '='), '=');
                        }
                        // 수직 앞쪽 벽
                        else if (j == 0)
                        {
                            walls.Add(new Point(j, i, '='), '=');
                        }
                        // 수직 뒷쪽 벽
                        else if (j == _xLimit)
                        {
                            walls.Add(new Point(j, i, '='), '=');
                        }
                        // 마지막줄 벽
                        else if (i == _yLimit)
                        {
                            walls.Add(new Point(j, i, '='), '=');
                        }
                    }
                }
            }

            public void Draw()
            {
                foreach (KeyValuePair<Point, char> pair in walls)
                {
                    pair.Key.Draw();
                }
            }
        }
    }
}