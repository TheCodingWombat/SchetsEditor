using System;
using System.Drawing;
using System.Drawing.Drawing2D;

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
                    TweepuntTool.Punten2Rechthoek(beginPunt, eindPunt);
                    Rectangle myRectangle = TweepuntTool.Punten2Rechthoek(beginPunt, eindPunt);
                    return myRectangle.Contains(p);
                case "rondje":
                    Rectangle myEllipse = TweepuntTool.Punten2Rechthoek(beginPunt, eindPunt);
                    double h = (Math.Abs(eindPunt.X+beginPunt.X) / 2);
                    double k = (Math.Abs(eindPunt.Y+beginPunt.Y) / 2);
                    double rx = myEllipse.Width/2;
                    double ry = myEllipse.Height/2;
                    double berekening = ((((p.X - h)*(p.X - h)) / (rx*rx)) + (((p.Y - k)*(p.Y - k)) / (ry*ry))) ;
                    if(berekening <= 1.1)
                        return true;
                    return false;
                case "lijn":
                    if (Math.Abs((eindPunt.X - beginPunt.X) * (beginPunt.Y - p.Y) - (beginPunt.X - p.X) * (eindPunt.Y - beginPunt.Y)) / //Met behulp van http://www.java2s.com/Code/CSharp/Development-Class/DistanceFromPointToLine.htm
                    Math.Sqrt(Math.Pow(eindPunt.X - beginPunt.X, 2) + Math.Pow(eindPunt.Y - beginPunt.Y, 2)) < 5)
                        return true;
                    return false;
                case "cirkel":

                    Rectangle myEllipseOpen = TweepuntTool.Punten2Rechthoek(beginPunt, eindPunt);
                    h = (Math.Abs(eindPunt.X+beginPunt.X) / 2);
                    k = (Math.Abs(eindPunt.Y+beginPunt.Y) / 2);
                    rx = myEllipseOpen.Width/2;
                    ry = myEllipseOpen.Height/2;
                    berekening = ((((p.X - h)*(p.X - h)) / (rx*rx)) + (((p.Y - k)*(p.Y - k)) / (ry*ry))) ;
                    Console.WriteLine("r_x " + rx + " r_y " + ry + " k " + k + " h " + h);
                    Console.WriteLine(((((p.X - h)*(p.X - h)) / (rx*rx)) + (((p.Y - k)*(p.Y - k)) / (ry*ry)) == 1));

                    if(berekening < 1.5 && berekening > 0.5 )
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
                case "pen":
                    if (Math.Abs((eindPunt.X - beginPunt.X) * (beginPunt.Y - p.Y) - (beginPunt.X - p.X) * (eindPunt.Y - beginPunt.Y)) / //Met behulp van http://www.java2s.com/Code/CSharp/Development-Class/DistanceFromPointToLine.htm
                        Math.Sqrt(Math.Pow(eindPunt.X - beginPunt.X, 2) + Math.Pow(eindPunt.Y - beginPunt.Y, 2)) < 5)
                        return true;
                    return false;
                
                default:
                    return false;
            }
            
        }
    }
}