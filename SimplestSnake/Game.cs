using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alex_hw6_snake
{
    public struct Settings
    {
        public int fieldWidth;
        public int fieldHeight;
        public WallSetting wallsetting;
    }

    class Game
    {
        UserInterface UI;
        Timer timer;
        Field field;
        Snake snake;
        Settings settings;

        bool paused = false;

        public Game()
        {
            timer = new Timer();
            timer.Interval = 700;
            timer.Elapsed += OnTimer;

            settings = new Settings()
            {
                fieldWidth = 10,
                fieldHeight = 10,
                wallsetting = WallSetting.Medium
            };

            UI = new ConsoleInterface();
            snake = new Snake();
            field = new Field(snake, settings);

            ChangedDirection += snake.ChangeDirection;

            field.YouLose += timer.Stop;
            field.YouWin += timer.Stop;

            field.YouLose += UI.YouLose;
            field.YouWin += UI.YouWin;

            field.YouLose += () => Environment.Exit(0);
            field.YouWin += () => Environment.Exit(0);
        }
        

        public delegate void ChangeDirection(Direction direction);
        public event ChangeDirection ChangedDirection;


        private void SpeedUp()
        {
            if (timer.Interval > 200)
                timer.Interval -= 100;
        }

        private void SpeedDown()
        {
            if (timer.Interval < 800)
                timer.Interval += 100;
        }

        private void PauseResume()
        {
            if(paused)
            {
                paused = false;
                timer.Start();
            }
            else
            {
                paused = true;
                timer.Stop();
            }
        }
        

        public void Run()
        {
            timer.Start();

            UI.DrawField(field);

            Console.CursorVisible = false;
            const bool dontPrintKeys = true;
            ConsoleKey key;

            do
            {
                key = Console.ReadKey(dontPrintKeys).Key;

                switch(key)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        ChangedDirection(Direction.Up);
                        break;

                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        ChangedDirection(Direction.Down);
                        break;

                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.A:
                        ChangedDirection(Direction.Left);
                        break;

                    case ConsoleKey.RightArrow:
                    case ConsoleKey.D:
                        ChangedDirection(Direction.Right);
                        break;

                    case ConsoleKey.OemPlus:
                        SpeedUp();
                        break;

                    case ConsoleKey.OemMinus:
                        SpeedDown();
                        break;

                    case ConsoleKey.P:
                        PauseResume();
                        break;
                }

            } while (key != ConsoleKey.Escape);

        } // void Run()


        private void OnTimer(object sender, ElapsedEventArgs arg)
        {
            snake.BeginMove();
            
            UI.DrawField(field);
        }

    } // class Game
} // namespace Alex_hw6_snake