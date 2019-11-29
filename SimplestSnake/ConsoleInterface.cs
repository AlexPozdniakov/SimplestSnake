using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alex_hw6_snake
{
    class ConsoleInterface : UserInterface
    {

        private int left_margin = 4;
        private int top_margin = 2;
        private char border = '#';


        public void YouLose() => TextMessage("You lose!");
        public void YouWin() => TextMessage("You win!");

        private void TextMessage(string message)
        {
            int width = (message.Length / 2) + 1;

            DrawBorder(width, 3);

            int left_indent = (left_margin + 1) * 2;
            int top_indent = top_margin + 1;

            Console.SetCursorPosition(left_indent, top_indent);
            Console.Write(message);
            Console.ReadKey(true);
        }


        public void DrawField(Field field)
        {
            if (field == null)
                throw new NullReferenceException();

            DrawBorder(field.Width, field.Height);
            Draw(field);
        }


        private void DrawBorder(int FieldWidth, int FieldHeight)
        {
            Console.Clear();

            for (int i = 0; i < this.top_margin; i++)
                Console.WriteLine();

            string l_margin = new string(' ', 2 * this.left_margin);
            string border = new string(this.border, 1) + ' ';
            string empty = new string(' ', 2);

            l_margin += border;

            Console.Write(l_margin);
            for (int j = 0; j < FieldWidth; j++)
                Console.Write(border);
            Console.WriteLine(border);

            for (int i = 0; i < FieldHeight; i++)
            {
                Console.Write(l_margin);

                for(int j = 0; j < FieldWidth; j++)
                    Console.Write(empty);
                
                Console.WriteLine(border);
            }

            Console.Write(l_margin);
            for (int j = 0; j < FieldWidth; j++)
                Console.Write(border);
            Console.WriteLine(border);

        }

        
        private void Draw(Field field)
        {
            int left_indent = (left_margin + 1) * 2;
            int top_indent = top_margin + 1;

            for (int y = 0; y < field.Height; y++)
            {
                Console.SetCursorPosition(left_indent, top_indent + y);

                for (int x = 0; x < field.Width; x++)
                {
                    FieldState cell = field[x, y];  // order x y is important

                    if (cell == FieldState.Empty)
                        Console.BackgroundColor = ConsoleColor.Black;
                    else if (cell == FieldState.Food)
                        Console.BackgroundColor = ConsoleColor.Red;
                    else if (cell == FieldState.Wall)
                        Console.BackgroundColor = ConsoleColor.Gray;
                    else if (cell == FieldState.Body || cell == FieldState.Tail)
                        Console.BackgroundColor = ConsoleColor.DarkMagenta;
                    else if (cell == FieldState.Head)
                        Console.BackgroundColor = ConsoleColor.DarkYellow;
                    
                    Console.Write("  ");
                }
                Console.BackgroundColor = ConsoleColor.Black;
            }
        }
        
    } // class ConsoleInterface
} // namespace Alex_hw6_snake
