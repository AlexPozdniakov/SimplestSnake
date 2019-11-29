using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alex_hw6_snake
{
    enum FieldState { Empty, Food, Wall, Head, Tail, Body}

    public enum WallSetting { None, High, Medium, Low };

    class Field
    {
        private static int minWidth = 6;
        private static int minHeight = 6;

        private Random rand = new Random();
        private FieldState[,] field;    // [X, Y]   first number is column, second is row

        public int Width { get; private set; }
        public int Height { get; private set; }

        private Snake snake;

        private WallSetting wallsetting;

        public event Snake.VoidDelegate YouWin;
        public event Snake.VoidDelegate YouLose;

        
        public Field(int width, int height, Snake snake, WallSetting settings)
        {
            setField(width, height);

            wallsetting = settings;
            SetWalls(wallsetting);

            this.snake = snake;
            setSnake(snake);

            GenerateFood();

            snake.checkNextMove += CheckNextMove;
            snake.snakeMoved += OnSnakeMoved;
            snake.foodEaten += FoodEaten;
        }
        
        public Field(int width, int height, Snake snake) : this(width, height, snake, WallSetting.Medium) { }

        public Field(Snake snake) : this(10, 10, snake) { }

        public Field() : this(10, 10, new Snake()) { }

        public Field(Snake snake, Settings settings) 
            : this(settings.fieldHeight, settings.fieldHeight, snake, settings.wallsetting) { }
        

        public int SnakeLength
        {
            get { return snake.Length; }
        }
        
        public FieldState this[int i, int j]
        {
            get
            {
                if (inFieldRange(i, j))
                    return field[i, j];
                else
                    throw new IndexOutOfRangeException("Invalid access to field by indexator");
            }
        }

        private bool inFieldRange(int i, int j)
        {
            return i >= 0 && j >= 0 && i < Width && j < Height;
        }

        private bool isFieldFull()
        {
            foreach (FieldState s in field)
            {
                if (s == FieldState.Empty)
                    return false;
            }
            return true;
        }


        private void setField(int w, int h)
        {
            Width = w < minWidth ? minWidth : w;
            Height = h < minHeight ? minHeight : h;

            field = new FieldState[Width, Height];
        }


        public void setNewField(int w, int h)
        {
            setField(w, h);
            setSnake(snake);
            SetWalls(wallsetting);
            GenerateFood();
        }

        public void setNewField(Settings settings)
        {
            this.wallsetting = settings.wallsetting;
            setNewField(settings.fieldWidth, settings.fieldHeight);
        }

        public void setNewWallSetting(WallSetting ws)
        {
            this.wallsetting = ws;
            setNewField(Width, Height);
        }


        private void setSnake(Snake snake)
        {
            bool canSetSnake = false;

            int x;
            int y;
            Direction d;

            do
            {
                x = rand.Next(1, Width - 1);
                y = rand.Next(1, Height - 1);
                d = (Direction)rand.Next(0, 4);

                FieldState wall = FieldState.Wall;

                if (field[x, y] != wall)
                {
                    canSetSnake = canMove(new OrientedPoint(x, y, d));
                }

            } while (!canSetSnake);


            field[x, y] = FieldState.Head;

            snake.SetStartPosition(x, y, d);
        }


        private struct Point
        {
            public int x;
            public int y;
        }

        private void SetWalls(WallSetting settings)
        {
            int iteration_times = 100;

            switch (settings)
            {
                case WallSetting.None:
                    return;

                case WallSetting.Low:
                    iteration_times = 50;
                    break;

                case WallSetting.Medium:
                    iteration_times = 100;
                    break;

                case WallSetting.High:
                    iteration_times = 200;
                    break;
            }

            int x1 = 1;
            int x2 = Width - 1;
            int y1 = 1;
            int y2 = Height - 1;

            int maxLength = Width - 2;
            
            for(int iter = 0; iter < iteration_times; iter++)
            {
                int x = rand.Next(x1, x2);
                int y = rand.Next(y1, y2);
                int L = rand.Next(2, maxLength + 1);

                Direction d = (Direction) rand.Next(0, 4);

                Point[] cells = new Point[L];

                switch(d)
                {
                    case Direction.Up:
                        for(int i = 0; i < L; i++)
                        {
                            cells[i].x = x;
                            cells[i].y = y - i;
                        }
                        break;

                    case Direction.Down:
                        for(int i = 0; i < L; i++)
                        {
                            cells[i].x = x;
                            cells[i].y = y + i;
                        }
                        break;

                    case Direction.Left:
                        for (int i = 0; i < L; i++)
                        {
                            cells[i].x = x - i;
                            cells[i].y = y;
                        }
                        break;

                    case Direction.Right:
                        for (int i = 0; i < L; i++)
                        {
                            cells[i].x = x + i;
                            cells[i].y = y;
                        }
                        break;
                }

                bool possibleWall = true;

                foreach(Point pt in cells)
                {
                    if (    !inFieldRange(pt.x, pt.y) ||
                         existNearbyWalls(pt.x, pt.y) ||
                         atEdge(pt.x, pt.y)
                       )
                    {
                        possibleWall = false;
                        break;
                    }
                }

                if(possibleWall)
                {
                    foreach (Point pt in cells)
                        field[pt.x, pt.y] = FieldState.Wall;
                }

            } // end for( ... iter < iteration_times ... )

        } // void SetWalls()


        private bool atEdge(int x, int y)
        {
            return x == 0 || y == 0 || x == Width - 1 || y == Height - 1;
        }

        private bool existNearbyWalls(int x, int y)
        {
            int distance = 2;

            for (int i = x - distance; i <= x + distance; i++)
            {
                for (int j = y - distance; j <= y + distance; j++)
                {
                    if (inFieldRange(i, j) && field[i, j] == FieldState.Wall)
                        return true;
                }
            }
            return false;
        }



        public void CheckNextMove()
        {
            if (!canMove(snake.Head))
            {
                YouLose();
                return;
            }

            OrientedPoint pt = Snake.getNextPoint(snake.Head);

            if (field[pt.X, pt.Y] == FieldState.Food)
                snake.Eat();
            else
                snake.Move();
        }


        public bool canMove(OrientedPoint head)
        {
            if (!inFieldRange(head.X, head.Y))
                return false;

            OrientedPoint nextPt = Snake.getNextPoint(head);

            if (!inFieldRange(nextPt.X, nextPt.Y))
                return false;

            FieldState state = field[nextPt.X, nextPt.Y];

            if (state == FieldState.Empty || state == FieldState.Food)
                return true;
            else
                return false;
        }


        public void OnSnakeMoved() => Refresh(snake);
        
        public void Refresh(Snake snake)
        {
            Clean();
            
            for (int i = 0; i < snake.Length; i++)
            {
                int x = snake[i].X;
                int y = snake[i].Y;

                if (i == snake.Length - 1)
                    field[x, y] = FieldState.Head;
                else if (i == 0)
                    field[x, y] = FieldState.Tail;
                else
                    field[x, y] = FieldState.Body;
            }
        }


        private void Clean()
        {
            for (int i = 0; i < field.GetLength(0); i++)
            {
                for (int j = 0; j < field.GetLength(1); j++)
                {
                    FieldState cell = field[i, j];

                    if (cell == FieldState.Body || cell == FieldState.Head || cell == FieldState.Tail)
                        field[i, j] = FieldState.Empty;
                }
            }
        }
        

        public void FoodEaten()
        {
            if (!GenerateFood())
                YouWin();
        }

        public bool GenerateFood()
        {
            if (isFieldFull())
                return false;

            int x;
            int y;

            do
            {
                x = rand.Next(0, Width);
                y = rand.Next(0, Height);

            } while (field[x, y] != FieldState.Empty);

            field[x, y] = FieldState.Food;

            return true;
        }

    } // class Field
} // namespace Alex_hw6_snake