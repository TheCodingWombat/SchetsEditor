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
                    if (beginPunt.X < p.X && eindPunt.X > p.X && beginPunt.Y < p.Y && eindPunt.Y > p.Y)
                        return true;
                    return false;
                default:
                    return false;
            }
            
        }
    }
}