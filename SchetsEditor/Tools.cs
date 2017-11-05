using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SchetsEditor
{
    public interface ISchetsTool
    {
        void MuisVast(SchetsControl s, Point p);
        void MuisDrag(SchetsControl s, Point p);
        void MuisLos(SchetsControl s, Point p);
        void Letter(SchetsControl s, char c);
        void Compleet(Graphics g, Point p1, Point p2, Brush kwast);
    }

    public abstract class StartpuntTool : ISchetsTool
    {
        protected Point startpunt;
        protected Brush kwast;
        protected SchetsControl s; //NIEUW

        public virtual void MuisVast(SchetsControl s, Point p)
        {   startpunt = p;
            this.s = s;
        }
        public virtual void MuisLos(SchetsControl s, Point p)
        {   kwast = new SolidBrush(s.PenKleur); Console.WriteLine(s.PenKleur);
        }
        public abstract void MuisDrag(SchetsControl s, Point p);
        public abstract void Letter(SchetsControl s, char c);

        public virtual void Compleet(Graphics g, Point p1, Point p2, Brush kwast) { }
        public virtual void MaakLetter(SchetsControl s, Graphics g, Point p1, Point p2, Brush kwast) { }
    }

    public class TekstTool : StartpuntTool
    {
        public override string ToString() { return "tekst"; }

        public override void MuisDrag(SchetsControl s, Point p) { }

        public override void MaakLetter(SchetsControl s, Graphics g, Point p1, Point p2, Brush kwast)
        {
            this.startpunt = p1;
            char c = (char)(p2.X);
            this.kwast = kwast;
            Letter(s, c);
        }

        public override void Letter(SchetsControl s, char c)
        {
            if (c >= 32)
            {
                Graphics gr = s.MaakBitmapGraphics();
                Font font = new Font("Tahoma", 40);
                string tekst = c.ToString();
                SizeF sz = 
                gr.MeasureString(tekst, font, this.startpunt, StringFormat.GenericTypographic);
                gr.DrawString   (tekst, font, kwast, 
                                              this.startpunt, StringFormat.GenericTypographic);
                // gr.DrawRectangle(Pens.Black, startpunt.X, startpunt.Y, sz.Width, sz.Height);
                startpunt.X += (int)sz.Width;
                s.TekenElementen.Add(new TekenElement(this, this.startpunt, new Point((int)c,0), kwast));
                s.Invalidate();
            }
        }
    }

    public abstract class TweepuntTool : StartpuntTool
    {
        public static Rectangle Punten2Rechthoek(Point p1, Point p2)
        {   return new Rectangle( new Point(Math.Min(p1.X,p2.X), Math.Min(p1.Y,p2.Y))
                                , new Size (Math.Abs(p1.X-p2.X), Math.Abs(p1.Y-p2.Y))
                                );
        }
        public static Pen MaakPen(Brush b, int dikte)
        {   Pen pen = new Pen(b, dikte);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            return pen;
        }
        public override void MuisVast(SchetsControl s, Point p)
        {   base.MuisVast(s, p);
            kwast = Brushes.Gray;
        }
        public override void MuisDrag(SchetsControl s, Point p)
        {   s.Refresh();
            this.Bezig(s.CreateGraphics(), this.startpunt, p);
        }
        public override void MuisLos(SchetsControl s, Point p)
        {   base.MuisLos(s, p);
            this.Compleet(s.MaakBitmapGraphics(), this.startpunt, p, kwast);
            s.TekenElementen.Add(new TekenElement(this, this.startpunt, p, kwast)); Console.WriteLine(kwast);
            s.Invalidate();
        }
        public override void Letter(SchetsControl s, char c)
        {
        }
        public abstract void Bezig(Graphics g, Point p1, Point p2);
        
        public override void Compleet(Graphics g, Point p1, Point p2, Brush kwast)
        { 
           this.Bezig(g, p1, p2);
        }
    }

    public class RechthoekTool : TweepuntTool
    {
        public override string ToString() { return "kader"; }

        public override void Bezig(Graphics g, Point p1, Point p2)
        {   Console.WriteLine("Drawing kader");
            g.DrawRectangle(MaakPen(kwast,3), TweepuntTool.Punten2Rechthoek(p1, p2));
        }
        public override void Compleet(Graphics g, Point p1, Point p2, Brush kwast) //Compleet toegevoegd ivm opslaan crash
        {   Console.WriteLine("Drawing kader");
            g.DrawRectangle(MaakPen(kwast,3), TweepuntTool.Punten2Rechthoek(p1, p2));
        }
    }
    public class CirkelTool : TweepuntTool              //New!
    {
        public override string ToString() { return "cirkel"; }

        public override void Bezig(Graphics g, Point p1, Point p2)
        {
            Console.WriteLine("Drawing cirkel met kwast:" +kwast);
            g.DrawEllipse(MaakPen(kwast, 3), TweepuntTool.Punten2Rechthoek(p1, p2));
        }
        public override void Compleet(Graphics g, Point p1, Point p2, Brush kwast) //Compleet toegevoegd ivm opslaan crash, dit is gekopiëerd moet beter kunnen
        {
            Console.WriteLine("Drawing cirkel met kwast:" +kwast);
            g.DrawEllipse(MaakPen(kwast, 3), TweepuntTool.Punten2Rechthoek(p1, p2));
        }
    }

    public class VolRechthoekTool : RechthoekTool
    {
        public override string ToString() { return "vlak"; }

        public override void Compleet(Graphics g, Point p1, Point p2, Brush kwast)
        {
            Console.WriteLine("drawing vlak");
            g.FillRectangle(kwast, TweepuntTool.Punten2Rechthoek(p1, p2));
        }
    }
    public class VolCirkelTool : CirkelTool         //New!
    {
        public override string ToString() { return "rondje"; }

        public override void Compleet(Graphics g, Point p1, Point p2, Brush kwast)
        {
            Console.WriteLine("drawing rondje");
            g.FillEllipse(kwast, TweepuntTool.Punten2Rechthoek(p1, p2));
        }
    }

    public class LijnTool : TweepuntTool
    {
        public override string ToString() { return "lijn"; }

        public override void Compleet(Graphics g, Point p1, Point p2, Brush kwast)
        {   g.DrawLine(MaakPen(kwast,3), p1, p2);
        }
        public override void Bezig(Graphics g, Point p1, Point p2)
        {   g.DrawLine(MaakPen(this.kwast,3),p1,p2);
        }
    }

    public class PenTool : LijnTool
    {
        public override string ToString() { return "pen"; }

        public override void MuisDrag(SchetsControl s, Point p)
        {   this.MuisLos(s, p);
            this.MuisVast(s, p);
        }
    }
    
    public class GumTool : StartpuntTool
    {
        public override void Letter(SchetsControl s, char c)
        {
        }

        public override void MuisDrag(SchetsControl s, Point p)
        {
        }

        public override void MuisLos(SchetsControl s, Point p)
        {
            ArrayList tekenElementen = s.TekenElementen;

            for (int i = tekenElementen.Count - 1; i >= 0; i--)
            {
                TekenElement e = ((TekenElement)tekenElementen[i]);
                if (e.Contains(startpunt))
                {
                    Console.WriteLine(i);
                    tekenElementen.Remove(e); //remove e from TekenElementen
                    i++;
                    s.TekenBitmapOpnieuw(); //Redraw bitmap from TekenElement ArrayList, en daar zit nu e niet meer in
                    break;

                }
            }
        }

        public override string ToString() { return "gum"; }
    }
}
