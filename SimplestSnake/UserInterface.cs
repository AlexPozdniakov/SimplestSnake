using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alex_hw6_snake
{
    interface UserInterface
    {
        void DrawField(Field field);

        void YouWin();
        void YouLose();
    }
}
