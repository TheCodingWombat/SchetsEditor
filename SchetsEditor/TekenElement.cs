using System;
using System.Drawing;

namespace SchetsEditor
{
    class TekenElement
    {
        public Point beginPunt, eindPunt;
        public Brush kwast; //moet allemaal nog private worden
        public ISchetsTool tool;

        public TekenElement(ISchetsTool tool, Point beginPunt, Point eindPunt, Brush kwast)
        {
            this.beginPunt = beginPunt;
            this.eindPunt = eindPunt;
            this.kwast = kwast;
            this.tool = tool;
        }

        public bool Contains(Point p)
        {
            switch (tool.ToString())
            {
                case "vlak":
                    if (beginPunt.X <= p.X + 5 && eindPunt.X >= p.X - 5 && beginPunt.Y <= p.Y + 5 && eindPunt.Y >= p.Y - 5) //Extra 5 pixel margin toegevoegd!
                        return true;
                    return false;
                case "rondje":
                    if ((p.X < ((beginPunt.X + eindPunt.X) / 2) + (eindPunt.X - beginPunt.X)/2 ) && p.X > ((beginPunt.X + eindPunt.X) / 2) - (eindPunt.X - beginPunt.X)/2 )
                        if ((p.Y < ((beginPunt.Y + eindPunt.Y) / 2) + (eindPunt.Y - beginPunt.Y)/2 ) && p.Y > ((beginPunt.Y + eindPunt.Y) / 2) - (eindPunt.Y - beginPunt.Y)/2 )
                            return true;
                    return false;
                case "lijn":
                    if (Math.Abs((eindPunt.X - beginPunt.X) * (beginPunt.Y - p.Y) - (beginPunt.X - p.X) * (eindPunt.Y - beginPunt.Y)) / //Met behulp van http://www.java2s.com/Code/CSharp/Development-Class/DistanceFromPointToLine.htm
                    Math.Sqrt(Math.Pow(eindPunt.X - beginPunt.X, 2) + Math.Pow(eindPunt.Y - beginPunt.Y, 2)) < 5)
                        return true;
                    return false;
                case "cirkel":
                    if((p.X < ((beginPunt.X + eindPunt.X) / 2) + (eindPunt.X - beginPunt.X) / 2) && p.X > ((beginPunt.X + eindPunt.X) / 2) - (eindPunt.X - beginPunt.X) / 2)
                        if ((p.Y < ((beginPunt.Y + eindPunt.Y) / 2) + (eindPunt.Y - beginPunt.Y) / 2) && p.Y > ((beginPunt.Y + eindPunt.Y) / 2) - (eindPunt.Y - beginPunt.Y) / 2)
                        return true;
                    return false;

                case "kader":
                    if ((beginPunt.Y - 5 <= p.Y  && beginPunt.Y + 5 >= p.Y ) && ((beginPunt.X <= p.X && eindPunt.X >= p.X ) || (beginPunt.X >= p.X + 5 || eindPunt.X <= p.X - 5)))
                        return true;
                    if ((eindPunt.Y - 5 <= p.Y && eindPunt.Y + 5 >= p.Y) && ((beginPunt.X <= p.X && eindPunt.X >= p.X) || (beginPunt.X >= p.X + 5 || eindPunt.X <= p.X - 5)))
                        return true;
                    if((beginPunt.Y <= p.Y + 5 && eindPunt.Y >= p.Y + 5) && (beginPunt.X <= p.X + 5 && beginPunt.X >= p.X - 5))
                        return true;
                    if ((beginPunt.Y <= p.Y + 5 && eindPunt.Y >= p.Y + 5) && (eindPunt.X <= p.X + 5 && eindPunt.X >= p.X - 5))
                        return true;
                    return false;
                default:
                    return false;
            }
            
        }
    }
}