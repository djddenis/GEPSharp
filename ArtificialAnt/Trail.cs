using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ArtificialAnt
{
    public enum LocationStates
    {
        Ant,
        Empty,
        Food
    }

    public enum Directions
    {
        Up,
        Down,
        Left,
        Right
    }

    public class Trail
    {
        public static readonly int BoardEdgeLength = 32;
        public static readonly HashSet<Point> StartFoodLocations;
        public static readonly Point AntStart;
        public static readonly Directions StartDirection;

        public Point Ant { get; private set; }
        public Directions AntDirection { get; set; }
        public HashSet<Point> FoodLocations { get; private set; }

        static Trail()
        {
            StartFoodLocations = new HashSet<Point>();
            string[] lines = File.ReadAllLines("Board.txt");
            for (int i = 0; i < BoardEdgeLength; i++)
            {
                for (int j = 0; j < BoardEdgeLength; j++)
                {
                    
                    if (lines[i][j] == '*')
                        StartFoodLocations.Add(new Point(j, i));
                    else if (lines[i][j] != '0')
                    {
                        AntStart = new Point(i, j);
                        switch (lines[i][j])
                        {
                            case('R'):
                                StartDirection = Directions.Right;
                                break;
                            case ('L'):
                                StartDirection = Directions.Left;
                                break;
                            case ('U'):
                                StartDirection = Directions.Up;
                                break;
                            case ('D'):
                                StartDirection = Directions.Down;
                                break;
                        }
                    }
                }
            }
        }

        public Trail()
        {
            Ant = new Point(AntStart.x, AntStart.y);
            AntDirection = StartDirection;
            FoodLocations = new HashSet<Point>();
            foreach (Point p in StartFoodLocations)
                FoodLocations.Add(p);
        }

        public void TurnLeft()
        {
            switch (AntDirection)
            {
                case (Directions.Right):
                    AntDirection = Directions.Up;
                    break;
                case (Directions.Left):
                    AntDirection = Directions.Down;
                    break;
                case (Directions.Up):
                    AntDirection = Directions.Left;
                    break;
                case (Directions.Down):
                    AntDirection = Directions.Right;
                    break;
            }
        }

        public void TurnRight()
        {
            switch (AntDirection)
            {
                case (Directions.Right):
                    AntDirection = Directions.Down;
                    break;
                case (Directions.Left):
                    AntDirection = Directions.Up;
                    break;
                case (Directions.Up):
                    AntDirection = Directions.Right;
                    break;
                case (Directions.Down):
                    AntDirection = Directions.Left;
                    break;
            }
        }

        public bool Move()
        {
            switch (AntDirection)
            {
                case (Directions.Right):
                    Ant.x = Wrap(Ant.x + 1);
                    break;
                case (Directions.Left):
                    Ant.x = Wrap(Ant.x - 1);
                    break;
                case (Directions.Up):
                    Ant.y = Wrap(Ant.y - 1);
                    break;
                case (Directions.Down):
                    Ant.y = Wrap(Ant.y + 1);
                    break;
            }
            return FoodLocations.Remove(Ant);
        }

        public bool FoodAhead()
        {
            switch (AntDirection)
            {
                case (Directions.Right):
                    return FoodLocations.Contains(new Point(Wrap(Ant.x + 1), Ant.y));
                case (Directions.Left):
                    return FoodLocations.Contains(new Point(Wrap(Ant.x - 1), Ant.y));
                case (Directions.Up):
                    return FoodLocations.Contains(new Point(Ant.x, Wrap(Ant.y - 1)));
                case (Directions.Down):
                    return FoodLocations.Contains(new Point(Ant.x, Wrap(Ant.y + 1)));
                default:
                    throw new ArgumentException();
            }
        }

        private int Wrap(int n)
        {
            if (n >= BoardEdgeLength)
                return n - BoardEdgeLength;
            if (n < 0)
                return n + BoardEdgeLength;
            return n;
        }
    }
}
