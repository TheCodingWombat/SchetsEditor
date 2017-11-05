using System;
using System.Collections.Generic;
using System.Drawing;

namespace SchetsEditor
{
    public class Schets
    {
        public Bitmap bitmap;      //Nu public en static!
        private bool changed;        //New!
        
        public Schets()
        {
            bitmap = new Bitmap(1, 1);
            changed = false;
        }
        public Graphics BitmapGraphics
        {
            get { return Graphics.FromImage(bitmap); }
        }
        public bool Changed         //New!
        {
            get { return changed; }
            set { changed = value;}
        }
        public Bitmap Bitmap
        {
            get { return bitmap; }
            set { bitmap = value; }
        }
        public void VeranderAfmeting(Size sz)
        {
            if (sz.Width > bitmap.Size.Width || sz.Height > bitmap.Size.Height)
            {
                Bitmap nieuw = new Bitmap( Math.Max(sz.Width,  bitmap.Size.Width)
                                         , Math.Max(sz.Height, bitmap.Size.Height)
                                         );
                Graphics gr = Graphics.FromImage(nieuw);
                gr.FillRectangle(Brushes.White, 0, 0, sz.Width, sz.Height);
                gr.DrawImage(bitmap, 0, 0);
                bitmap = nieuw;
            }
            //changed = true; //Console.WriteLine("Verander afmeting changed");//New!
        }
        public void Teken(Graphics gr)
        {
            gr.DrawImage(bitmap, 0, 0);
            changed = true; //Console.WriteLine("Teken changed");//New! 
            //Console.WriteLine("Teken in schets.cs, changed:"+changed);
        }
        public virtual void Schoon()
        {
            Graphics gr = Graphics.FromImage(bitmap);
            gr.FillRectangle(Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
            changed = true; //Console.WriteLine("Schoon changed");//New!
        }
        public void Roteer()
        {
            bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
            changed = true; //Console.WriteLine("Roteer changed"); //New!
        }
    }
}
