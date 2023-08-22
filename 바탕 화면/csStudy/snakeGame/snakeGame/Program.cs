using System.ComponentModel;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace snakeGame
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int x = Console.WindowWidth - 3;
            int y = Console.WindowHeight - 5;
            GameSnake game = new GameSnake(x, y);
            game.Start();
            Console.ReadLine();
        }
    }

    public class GameSnake
    {
        protected int maxX; protected int maxY;
        public GameSnake(int x, int y)
        {
            maxX = x; maxY = y;
        }

        public void Start()
        {
            bool gameOver = false;

            // 배경화면 설정
            for (int i = 0; i <= maxX; i ++)
            {
                Console.SetCursorPosition(i, 0);
                Console.WriteLine('◇');
                Console.SetCursorPosition(i, maxY);
                Console.WriteLine('◇');
            }

            for (int j = 0; j <= maxY; j ++)
            {
                Console.SetCursorPosition(0, j);
                Console.WriteLine('◇');
                Console.SetCursorPosition(maxX, j);
                Console.WriteLine('◇');
            }

            // 머리 및 몸통, 방향 초기화
            Point startHead = new Point(10, 10, '■');
            List <Point> startBody = new List<Point>();

            for (int i = 1; i <= 5; i ++ )
            {
                startBody.Add(new Point(10 - i, 10, '□'));
            }

            // Snake 생성
            Snake snake = new Snake(startHead, startBody, Direction.Right);
            snake.Draw();

            // 먹이 생성
            Point food = CreateFood(snake);

            while (gameOver == false)
            {
                snake.ReadWait();
                snake.Move();

                if (snake.IsHit(food))
                {
                    Point temp = snake.body[snake.body.Count - 1];
                    Point newPoint = new Point(temp.x, temp.y, temp.sym);
                    snake.body.Add(newPoint);
                    food = CreateFood(snake);
                }

                if (snake.x <= 0 || snake.y <= 0 || snake.x>= maxX || snake.y>= maxY)
                {
                    break;
                }

                foreach (Point p in snake.body)
                {
                    if (snake.IsHit(p))
                    {
                        gameOver = true;
                    }
                }
                Console.SetCursorPosition(5, maxY + 1);
                Console.Write($"snake length={snake.body.Count}\tfood count={snake.body.Count-5}");
            }
            Console.SetCursorPosition(5, maxY + 2);
            Console.Write("Game Over!");
        }

        public enum Direction
        {
            Right,
            Left,
            Up,
            Down
        }

        public class Point
        {

            public int x { get; set; }
            public int y { get; set; }
            public char sym { get; set; }

            public Point (int _x, int _y, char _sym)
            {
                x = _x;
                y = _y;
                sym = _sym;
            }

            public virtual void Draw()
            {
                Console.SetCursorPosition(x, y);
                Console.WriteLine(sym);
            }

            public virtual void Clear()
            {
                Console.SetCursorPosition(x, y);
                Console.WriteLine(' ');
            }

            public virtual bool IsHit(Point p)
            {
                return (p.x == x && p.y == y);
            }
        }


        public class Snake : Point
        {
            public Point head { get; set; }
            public List<Point> body { get; set; }
            public Direction Direc { get => direc; set { direc = value; } }
            private Direction direc { get; set; }


            public Snake(Point _head, List<Point> _body, Direction _direc) : base (_head.x, _head.y, _head.sym)
            {
                head = _head;
                body = _body;
                direc = _direc;
            }

            public override void Draw()
            {
                // 왜 head로 하면 안될까요?
                base.Draw();
                foreach (Point p in body)
                {
                    p.Draw();
                }
            }

            public override void Clear()
            {
                base.Clear();
                foreach (Point p in body)
                {
                    p.Clear();
                }
            }

            public void Move()
            {
                Clear();

                for (int i = body.Count - 1; i > 0; i--)
                {
                    body[i].x = body[i - 1].x;
                    body[i].y = body[i - 1].y;
                }
                body[0].x = x;
                body[0].y = y;

                switch (Direc)
                {
                    case Direction.Left: x--; break;
                    case Direction.Right: x++; break;
                    case Direction.Up: y--; break;
                    case Direction.Down: y++; break;
                }

                Draw();
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
                                if (Direc != Direction.Down) Direc = Direction.Up;
                                else continue;
                                break;
                            case ConsoleKey.S:
                            case ConsoleKey.DownArrow:
                                if (Direc != Direction.Up) Direc = Direction.Down;
                                else continue;
                                break;
                            case ConsoleKey.D:
                            case ConsoleKey.RightArrow:
                                if (Direc != Direction.Left) Direc = Direction.Right;
                                else continue;
                                break;
                            case ConsoleKey.A:
                            case ConsoleKey.LeftArrow:
                                if (Direc != Direction.Right) Direc = Direction.Left;
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

        public Point CreateFood(Snake _snake)
        {
            Random rnd = new Random();
            Point newFood;
            bool crash = false;
            do
            {
                newFood = new Point(rnd.Next(1, maxX), rnd.Next(1, maxY), '○');
                foreach (Point p in _snake.body)
                {
                    if (newFood.IsHit(p))
                    {
                        crash = true;
                        break;
                    }
                }
            }
            while (crash);

            newFood.Draw();
            return newFood;
        }
    }


}