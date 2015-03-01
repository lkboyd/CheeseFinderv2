using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheeseFinder
{

    class Program
    {
        public static Random rng = new Random();
        static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Would you like to play a game?");
            System.Threading.Thread.Sleep(1000);
            Console.WriteLine("Lets see how many pieces of cheese you can get before the cats get you!");
            System.Threading.Thread.Sleep(1000);
            Console.WriteLine("Use the arrow keys to move your Mouse up and down and \n side to side to try and get the cheese!");
            System.Threading.Thread.Sleep(5000);
                      
            new CheeseFinder().PlayGame();

               Console.ReadKey();
            
        }

        /// <summary>
        /// Point class, setting information for the mouse, cheese, cat, and when the mouse get caught! as well as information for setting the grid
        /// </summary>
        public class Point
        {

            public int X { get; set; }
            public int Y { get; set; }
            public PointStatus status { get; set; }



            public Point(int x, int y)
            {
                this.X = x; this.Y = y;
                this.status = PointStatus.Empty;
            }
            public enum PointStatus
            {
                Empty = 1,
                Mouse,
                Cheese,
                Cat,
                CatAndCheese
            }

        }
        public class Mouse
        {
            public int Energy { get; set; }
            public Point Position { get; set; }
            public bool HasBeenPouncedOn { get; set; }
            public Mouse()
            {
                this.HasBeenPouncedOn = false;
                this.Energy = 50;
            }
            public void EatCheese()
            {
                this.Energy += 10;
            }
        }
        public class Cat
        {
            public Point Position { get; set; }
        }
        public class CheeseFinder
        {
            public Point[,] Grid { get; set; }
            public Mouse Mouse { get; set; }
            public Point Cheese { get; set; }
            public List<Cat> Cats { get; set; }
            public int CheeseCount { get; set; }


            public CheeseFinder()
            {
                this.Cats = new List<Cat>();
                this.CheeseCount = 0;
                this.Mouse = new Mouse();
                this.Grid = new Point[10, 10];


                for (int x = 0; x < 10; x++)
                {
                    for (int y = 0; y < 10; y++)
                    {
                        this.Grid[x, y] = new Point(x, y);

                    }

                }
                // Putting the mouse on the grid.
                this.Mouse.Position = Grid[rng.Next(0, 10), rng.Next(0, 10)];
                this.Mouse.Position.status = Point.PointStatus.Mouse;

                // Putting the cheese on the grid as well.  
                PlaceCheese();
            }

            public void DrawGrid()
            {
                // Building the grid :) in green!  
                Console.Clear();

                for (int x = 0; x < 10; x++)
                {
                    for (int y = 0; y < 10; y++)
                    {
                        Point aPoint = this.Grid[x, y];
                        Console.BackgroundColor = ConsoleColor.Gray;
                        switch (aPoint.status)
                        {
                            case Point.PointStatus.Empty:
                                Console.ForegroundColor = ConsoleColor.DarkGreen;
                                Console.Write("[ ]");
                                break;
                            case Point.PointStatus.Cheese:
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.Write("[>]");
                                break;
                            case Point.PointStatus.Mouse:
                                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                                Console.Write("[M]");
                                break;
                            case Point.PointStatus.Cat:
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write("[C]");
                                break;
                        }
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                    }
                    Console.WriteLine();
                }
                Console.WriteLine("Mouse Energy: " + Mouse.Energy);
            }

            public void GetUserMove()
            {
                // Includes validation of user input.
                this.Mouse.Energy--;
                int newMouseX = this.Mouse.Position.X;
                int newMouseY = this.Mouse.Position.Y;
                ConsoleKeyInfo input = Console.ReadKey();


                if (input.Key != ConsoleKey.LeftArrow && input.Key != ConsoleKey.RightArrow && input.Key != ConsoleKey.UpArrow && input.Key != ConsoleKey.DownArrow)
                        {
                    Console.WriteLine("Invalid Move...are you sure you hit an arrow?  Try Again!");
                    System.Threading.Thread.Sleep(1500);
                        }

                
                if (input.Key == ConsoleKey.LeftArrow || input.Key == ConsoleKey.RightArrow || input.Key == ConsoleKey.UpArrow || input.Key == ConsoleKey.DownArrow)
                {
                    // inputs that move the mouse (includes the ability to go from the top of the screen to the bottom and side to side...:)  )
                    switch (input.Key)
                    {
                        case ConsoleKey.LeftArrow:
                            newMouseY--;
                            if (newMouseY == -1)
                            {
                                newMouseY = 9;
                            }
                            break;
                        case ConsoleKey.RightArrow:
                            newMouseY++;
                            if (newMouseY == 10)
                            {
                                newMouseY = 0;
                            }
                            break;
                        case ConsoleKey.UpArrow:
                            newMouseX--;
                            if (newMouseX == -1)
                            {
                                newMouseX = 9;
                            }
                            break;
                        case ConsoleKey.DownArrow:
                            newMouseX++;
                            if (newMouseX == 10)
                            {
                                newMouseX = 0;
                            }
                            break;
                                        
                    }

                    Point newMousePosition = this.Grid[newMouseX, newMouseY];
                    if (newMousePosition.status == Point.PointStatus.Cheese)
                    {
                        //got the cheese, increase cheese count
                        this.CheeseCount++;
                        this.Mouse.EatCheese();
                        //place new cheese
                        PlaceCheese();

                        //check to add a new cat
                        if (this.CheeseCount % 2 == 0)
                        {
                            AddCat();
                        }
                        newMousePosition.status = Point.PointStatus.Mouse;
                    }
                    else if (newMousePosition.status == Point.PointStatus.Cat)
                    {
                        this.Mouse.HasBeenPouncedOn = true;
                        newMousePosition.status = Point.PointStatus.Cat;
                    }
                    else
                    {
                        newMousePosition.status = Point.PointStatus.Mouse;
                    }

                    //move the mouse
                    this.Mouse.Position.status = Point.PointStatus.Empty;
                    this.Mouse.Position = newMousePosition;
                }
            }
            public void PlayGame()
            {
                while (this.Mouse.Energy > 0 && !this.Mouse.HasBeenPouncedOn)
                {
                    DrawGrid();
                    GetUserMove();
                    foreach (Cat cat in this.Cats)
                    {
                        MoveCat(cat);
                    }
                }
                Console.WriteLine("Oh no!  You died!");


                if (Mouse.HasBeenPouncedOn)
                {
                    Console.WriteLine("The cat got you!  Yum Yum Yum...");
                }

                Console.WriteLine("Cheese consumed: " + CheeseCount);
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();
            }

            private void PlaceCheese()
            {
                bool placeOK = false;
                while (!placeOK)
                {
                    //get a random ppoint
                    Point point = this.Grid[rng.Next(0, 10), rng.Next(0, 10)];
                    if (point.status == Point.PointStatus.Empty)
                    {
                        //status is empty, place the cheese
                        placeOK = true;
                        point.status = Point.PointStatus.Cheese;
                        this.Cheese = point;
                    }
                }
            }

            private void AddCat()
            {
                Cat newCat = new Cat();
                PlaceCat(newCat);
                this.Cats.Add(newCat);
            }
            private void PlaceCat(Cat cat)
            {
                bool placeOK = false;
                while (!placeOK)
                {
                    //get a random point
                    Point point = this.Grid[rng.Next(0, 10), rng.Next(0, 10)];
                    if (point.status == Point.PointStatus.Empty)
                    {
                        //status is empty, place the cheese
                        placeOK = true;
                        point.status = Point.PointStatus.Cat;
                        cat.Position = point;
                    }
                }
            }
            private void MoveCat(Cat cat)
            {
                //cat has 80% chance to move
                if (rng.Next(6) > 1)
                {
                    int x = this.Mouse.Position.X - cat.Position.X;
                    int y = this.Mouse.Position.Y - cat.Position.Y;
                    bool validMove = false;
                    Point targetLocation = cat.Position;
                    bool tryLeft = (x < 0);
                    bool tryRight = (x > 0);
                    bool tryUp = (y < 0);
                    bool tryDown = (y > 0);

                    while (!validMove && (tryLeft || tryRight || tryUp || tryDown))
                    {
                        int targetX = cat.Position.X;
                        int targetY = cat.Position.Y;

                        if (tryRight)
                        {
                            targetLocation = Grid[++targetX, targetY];
                            tryRight = false;
                        }
                        else if (tryLeft)
                        {
                            targetLocation = Grid[--targetX, targetY];
                            tryLeft = false;
                        }
                        else if (tryDown)
                        {
                            targetLocation = Grid[targetX, ++targetY];
                            tryDown = false;
                        }
                        else if (tryUp)
                        {
                            targetLocation = Grid[targetX, --targetY];
                            tryUp = false;
                        }
                        validMove = IsValidCatMove(targetLocation);
                    }
                    //leaving space checks
                    if (cat.Position.status == Point.PointStatus.CatAndCheese)
                    {
                        cat.Position.status = Point.PointStatus.Cheese;
                    }
                    else
                    {
                        cat.Position.status = Point.PointStatus.Empty;
                    }
                    //new space check
                    if (targetLocation.status == Point.PointStatus.Mouse)
                    {
                        this.Mouse.HasBeenPouncedOn = true;
                        targetLocation.status = Point.PointStatus.Cat;
                    }
                    else if (targetLocation.status == Point.PointStatus.Cheese)
                    {
                        targetLocation.status = Point.PointStatus.CatAndCheese;
                    }
                    else
                    {
                        targetLocation.status = Point.PointStatus.Cat;
                    }
                    cat.Position = targetLocation;
                }
            }
            private bool IsValidCatMove(Point targetLocation)
            {
                return (targetLocation.status == Point.PointStatus.Empty || targetLocation.status == Point.PointStatus.Mouse);
            }
        }
    }
    
    
}