﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections;

namespace SchetsEditor
{   public class SchetsControl : UserControl
    {   private Schets schets;
        private Color penkleur;
        private int countteken, countundo;
        


        private ArrayList tekenElementen = new ArrayList();
        private ArrayList undoElementen = new ArrayList();

        public ArrayList TekenElementen
        {
            get
            {
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
            schets.Changed = false; countteken = 0; countundo = 0;
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }
        private void teken(object o, PaintEventArgs pea)
        {  schets.Teken(pea.Graphics); 
           if(tekenElementen.Count == 0 || schets.Saved == true)
           {
                schets.Changed = false; schets.Saved = false;
           }
           else if(tekenElementen.Count > countteken && undoElementen.Count == countundo) 
                    undoElementen.Clear(); schets.Saved = false;
           countteken = tekenElementen.Count; 
           countundo = undoElementen.Count;
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
        {   schets.Bitmap = new Bitmap(1,1);
            schets.VeranderAfmeting(new Size(this.ClientSize.Width, this.ClientSize.Height ));
            schets.Schoon();
            this.Invalidate();
            foreach (TekenElement e in tekenElementen)
            {
                if (e.tool.ToString() == "tekst")
                    ((TekstTool)(e.tool)).TekenLetterOpnieuw(this, e.c, e.beginPunt, e.kwast, e.rotatie);
                else
                    (e.tool).Compleet(MaakBitmapGraphics(), e.beginPunt, e.eindPunt, e.kwast);
            }

            
        }
        public void Undo(object o, EventArgs ea)
        {
            if(tekenElementen.Count >= 1)
                foreach (TekenElement e in tekenElementen)
                        if(tekenElementen.IndexOf(e) == tekenElementen.Count -1)
                        {
                            undoElementen.Add(new TekenElement(e.tool, e.beginPunt, e.eindPunt, e.kwast, e.c));
                            tekenElementen.RemoveAt(tekenElementen.Count - 1);
                            break;
                        } 
            TekenBitmapOpnieuw();
        }
        public void Redo(object o, EventArgs ea)
        {
            if(undoElementen.Count >= 1)
                foreach(TekenElement e in undoElementen)
                    if(undoElementen.IndexOf(e) == undoElementen.Count -1)
                        {
                        tekenElementen.Add(new TekenElement(e.tool, e.beginPunt, e.eindPunt, e.kwast, e.c));
                        undoElementen.RemoveAt(undoElementen.Count - 1);
                        break;
                        }
            TekenBitmapOpnieuw();
        }
        public void Schoon(object o, EventArgs ea)
        {   tekenElementen.Clear();
            schets.Schoon();
            this.Invalidate();
        }
        public void Roteer(object o, EventArgs ea)
        {
            schets.VeranderAfmeting(new Size(this.ClientSize.Height, this.ClientSize.Width));
            foreach (TekenElement e in tekenElementen)
            {
                Point newbegin, neweind;
                newbegin = BerekenRotatie(new Point(e.beginPunt.X - (ClientSize.Width/2), e.beginPunt.Y - (ClientSize.Height/2)));
                e.beginPunt.X = newbegin.X + (ClientSize.Width / 2);
                e.beginPunt.Y = newbegin.Y + (ClientSize.Height / 2); 
                neweind = BerekenRotatie(new Point(e.eindPunt.X - (ClientSize.Width/2), e.eindPunt.Y - (ClientSize.Height/2)));
                e.eindPunt.X = neweind.X + (ClientSize.Width / 2); 
                e.eindPunt.Y = neweind.Y + (ClientSize.Height / 2); 
                e.rotatie = (e.rotatie + 90) % 360;
            }
            TekenBitmapOpnieuw();
            schets.Changed = true;
            this.Invalidate();
        }
        private Point BerekenRotatie(Point e)
        {
            int newX, newY;
            Point newPoint = new Point(0, 0);
            newX = e.X * (int)Math.Cos((Math.PI / 2)) - e.Y * (int)Math.Sin((Math.PI / 2));
            newY = e.X * (int)Math.Sin((Math.PI / 2)) + e.Y * (int)Math.Cos((Math.PI / 2));
            newPoint.X = newX; newPoint.Y = newY;
            return newPoint;
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
