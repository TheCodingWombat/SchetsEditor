using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace SchetsEditor
{
    public class Hoofdscherm : Form
    {
        MenuStrip menuStrip;
        SchetsWin s; //New!

        public Hoofdscherm()
        {   this.ClientSize = new Size(800, 600);
            menuStrip = new MenuStrip();
            this.Controls.Add(menuStrip);
            this.maakFileMenu();
            this.maakHelpMenu();
            this.Text = "Schets editor";
            this.IsMdiContainer = true;
            this.MainMenuStrip = menuStrip;
        }
        private void maakFileMenu()
        {   ToolStripDropDownItem menu;
            menu = new ToolStripMenuItem("File");
            menu.DropDownItems.Add("Nieuw", null, this.nieuw);
            menu.DropDownItems.Add("Openen..", null, this.open);  //New!
            menu.DropDownItems.Add("Exit", null, this.afsluiten);
            menuStrip.Items.Add(menu);
        }
        private void maakHelpMenu()
        {   ToolStripDropDownItem menu;
            menu = new ToolStripMenuItem("Help");
            menu.DropDownItems.Add("Over \"Schets\"", null, this.about);
            menuStrip.Items.Add(menu);
        }
        private void about(object o, EventArgs ea)
        {   MessageBox.Show("Schets versie 1.0\n(c) UU Informatica 2010"
                           , "Over \"Schets\""
                           , MessageBoxButtons.OK
                           , MessageBoxIcon.Information
                           );
        }

        private void nieuw(object sender, EventArgs e)
        {   s = new SchetsWin();                //Er stond: SchetsWin s = new SchetsWin(); !
            s.MdiParent = this;
            s.Show();
        }
        private void afsluiten(object sender, EventArgs e) 
        {
            this.Close(); s = null; //Er stond alleen close!
        }
        private void open(object obj, EventArgs ea)     //New!
        {
            OpenFileDialog dialoog = new OpenFileDialog();
            dialoog.Filter = "Png|*.png|Jpeg|*.jpeg|Bmp|*.bmp|Alle files|*.*";
            dialoog.Title = "Openen ...";
            if (dialoog.ShowDialog() == DialogResult.OK)
            {
                if(s != null) s.afsluiten(obj, ea);
                nieuw(obj, ea);
                s.schets.bitmap = new Bitmap(dialoog.FileName, true);
                s.Bestandsnaam = dialoog.FileName;
                s.OpslagFormaat = dialoog.FilterIndex -1;
                Console.WriteLine("open formaat:"+Path.GetExtension(dialoog.FileName).ToLower());
            }
        }
    }
}
