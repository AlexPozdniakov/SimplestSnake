using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alex_hw6_snake
{
    enum Direction
    {
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3
    }

    struct OrientedPoint
    {
        public Direction Direction;
        public int X;
        public int Y;

        public OrientedPoint(int x, int y, Direction direction)
        {
            X = x;
            Y = y;
            Direction = direction;
        }

        public static bool operator ==(OrientedPoint obj1, OrientedPoint obj2)
        {
            return obj1.X == obj2.X && obj1.Y == obj2.Y && obj1.Direction == obj2.Direction;
        }
        public static bool operator !=(OrientedPoint obj1, OrientedPoint obj2)
        {
            return !(obj1 == obj2);
        }
    }



    class Snake
    {
        private List<OrientedPoint> body;

        
        public Snake()
        {
            body = new List<OrientedPoint> { new OrientedPoint(0, 0, Direction.Right) };
        }

        public void SetStartPosition(int x, int y, Direction direction) 
                 => SetStartPosition(new OrientedPoint(x, y, direction));

        public void SetStartPosition(OrientedPoint pt)
        {
            body.Clear();
            body.Add(pt);
        }


        public delegate void VoidDelegate();

        public event VoidDelegate snakeMoved;
        public event VoidDelegate checkNextMove;
        public event VoidDelegate foodEaten;
        

        public int Length
        {
            get { return body.Count; }
        }

        public OrientedPoint Head
        {
            get { return body.Last(); }
        }

        public OrientedPoint Tail
        {
            get { return body.First(); }
        }

        public Direction NextMove
        {
            get { return Head.Direction; }
        }
        
        public OrientedPoint this[int i]
        {
            get { return body[i]; }
        }

        

        public static OrientedPoint getNextPoint(OrientedPoint previous)
        {
            switch(previous.Direction)
            {
                case Direction.Up:
                    previous.Y--;
                    break;

                case Direction.Down:
                    previous.Y++;
                    break;

                case Direction.Left:
                    previous.X--;
                    break;

                case Direction.Right:
                    previous.X++;
                    break;
            }
            return previous;
        }



        private bool checkSnake()
        {
            OrientedPoint pt = Tail;

            foreach (OrientedPoint nextChain in body)
            {
                if (nextChain == Head)
                    break;

                pt = getNextPoint(pt);

                if (pt.X != nextChain.X || pt.Y != nextChain.Y)
                {
                    throw new Exception("Error: wrong structure of snake!");
                    return false;
                }

                pt.Direction = nextChain.Direction;
            }
            return true;
        }
        

            
        public void BeginMove() => checkNextMove();
        

        public void Move()
        {
            for (int i = 0; i < body.Count; i++)
            {
                OrientedPoint nextPoint = getNextPoint(body[i]);

                if(i != body.Count - 1)
                    nextPoint.Direction = body[i + 1].Direction;

                body[i] = nextPoint;
            }

            snakeMoved();
        }
        

        public void Eat()
        {
            body.Add(getNextPoint(Head));

            snakeMoved();

            foodEaten();
        }


        public void ChangeDirection(Direction direction)
        {
            if (direction < Direction.Up || direction > Direction.Left)
                throw new ArgumentOutOfRangeException("Wrong argument of direction");

            OrientedPoint head = new OrientedPoint(Head.X, Head.Y, direction);

            if(body.Count > 1)
            {
                OrientedPoint nextPoint = getNextPoint(head);
                OrientedPoint neck = body[Length - 2];

                    // don't allow to move in opposite direction
                if(nextPoint.X != neck.X || nextPoint.Y != neck.Y)
                    body[Length - 1] = head;
            }
            else
                body[Length - 1] = head;
        }

    } // class Snake
} // namespace Alex_hw6_snake