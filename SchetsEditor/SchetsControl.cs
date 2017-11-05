using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections;

namespace SchetsEditor
{   public class SchetsControl : UserControl
    {   private Schets schets;
        private Color penkleur;


        private ArrayList tekenElementen = new ArrayList();
        private ArrayList undoElementen = new ArrayList();

        public ArrayList TekenElementen
        {
            get
            {
                Console.WriteLine(tekenElementen.Count);
                return tekenElementen;
            }
            set { tekenElementen = value; }
        }

        public Color PenKleur
        { get { return penkleur; }
        }
        public Schets Schets
        { get { return schets;   }
        }
        public SchetsControl()
        {   this.BorderStyle = BorderStyle.Fixed3D;
            this.schets = new Schets();
            this.Paint += this.teken;
            this.Resize += this.veranderAfmeting;
            this.veranderAfmeting(null, null);
            penkleur = Color.Black;
            schets.Changed = false;
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }
        private void teken(object o, PaintEventArgs pea)
        {   schets.Teken(pea.Graphics); 
            undoElementen.Clear();
           if(tekenElementen.Count == 0){schets.Changed = false; /*Console.WriteLine("Falsed teken"); */}
        }       
        private void veranderAfmeting(object o, EventArgs ea)
        {   schets.VeranderAfmeting(this.ClientSize);
            this.Invalidate();
        }
        public Graphics MaakBitmapGraphics()
        {   Graphics g = schets.BitmapGraphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            return g;
        }

        public void TekenBitmapOpnieuw()
        {
            //Console.WriteLine("tesdft");
            schets.Schoon();
            this.Invalidate();
            foreach (TekenElement e in tekenElementen)
            {
                Console.WriteLine("TBO:Redrawing element" + e.tool.ToString() + " met " + e.kwast);
                //if(e.tool.ToString() != "tekst")
                    (e.tool).Compleet(MaakBitmapGraphics(), e.beginPunt, e.eindPunt, e.kwast);
                //else Tools.TekstTool.MaakLetter(this, MaakBitmapGraphics(), e.beginPunt, e.eindPunt, e.kwast);
            }
            
        }
        public void Undo(object o, EventArgs ea)
        {
            if(tekenElementen.Count >= 1)
                {
                foreach (TekenElement e in tekenElementen)
                    {
                         //Console.WriteLine("Testing foreach");                                    
                    if(tekenElementen.IndexOf(e) == tekenElementen.Count -1)
                        {
                            undoElementen.Add(new TekenElement(e.tool, e.beginPunt, e.eindPunt, e.kwast));
                            tekenElementen.RemoveAt(tekenElementen.Count - 1);
                            break;
                        } // :)
                    }
                }
            Console.WriteLine("TekenElementenCount: "+tekenElementen.Count);
            Console.WriteLine("UndoElementenCount: "+undoElementen.Count);
            TekenBitmapOpnieuw();
        }
        public void Redo(object o, EventArgs ea)
        {
            if(undoElementen.Count >= 1)
                foreach(TekenElement e in undoElementen)
                    if(undoElementen.IndexOf(e) == undoElementen.Count -1)
                        {
                        tekenElementen.Add(new TekenElement(e.tool, e.beginPunt, e.eindPunt, e.kwast));
                        undoElementen.RemoveAt(undoElementen.Count - 1);
                        break;
                        }
            Console.WriteLine("TekenElementenCount: "+tekenElementen.Count);
            Console.WriteLine("UndoElementenCount: "+undoElementen.Count);
            TekenBitmapOpnieuw();
        }
        public void Schoon(object o, EventArgs ea)
        {   tekenElementen.Clear();
            schets.Schoon();
            this.Invalidate();
        }
        public void Roteer(object o, EventArgs ea)
        {   schets.VeranderAfmeting(new Size(this.ClientSize.Height, this.ClientSize.Width));
            schets.Roteer();
            this.Invalidate();
        }
        public void VeranderKleur(object obj, EventArgs ea)
        {   ColorDialog dialoog = new ColorDialog();
            dialoog.AllowFullOpen = true;
            dialoog.Color = penkleur;
            if(dialoog.ShowDialog() == DialogResult.OK)
                penkleur = dialoog.Color;
        }
        public void VeranderKleurViaMenu(object obj, EventArgs ea)
        {   VeranderKleur(obj, ea);
        }
    }
}
