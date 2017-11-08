using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace SchetsEditor
{
    public class Hoofdscherm : Form
    {
        MenuStrip menuStrip;
        SchetsWin s; 

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
            menu.DropDownItems.Add("Openen..", null, this.open);  
            menu.DropDownItems.Add("Exit", null, this.afsluiten);
            (menu.DropDownItems[0] as ToolStripMenuItem).ShortcutKeys = (Keys)(Keys.Control | Keys.N);
            (menu.DropDownItems[1] as ToolStripMenuItem).ShortcutKeys = (Keys)(Keys.Control | Keys.O);
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
        {   s = new SchetsWin();               
            s.MdiParent = this;
            s.Show();
            s.schets.Changed = false; 
        }
        private void afsluiten(object sender, EventArgs e) 
        {
            this.Close(); s = null; 
        }
        private void open(object obj, EventArgs ea)     
        {
            OpenFileDialog dialoog = new OpenFileDialog();
            dialoog.Filter = "Png|*.png|Jpeg|*.jpeg|Bmp|*.bmp|Project File|*.txt|Alle files|*.*";
            dialoog.Title = "Openen ...";
            if (dialoog.ShowDialog() == DialogResult.OK)
            {
                switch(dialoog.FilterIndex)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 5:
                        if(s != null) s.afsluiten(obj, ea);
                        nieuw(obj, ea);
                        s.schets.Bitmap = new Bitmap(dialoog.FileName, true);
                        s.Bestandsnaam = dialoog.FileName;
                        s.OpslagFormaat = dialoog.FilterIndex -1;
                        break;
                    case 4:
                        if(s != null) s.afsluiten(obj, ea);
                        nieuw(obj, ea);
                        s.Bestandsnaam = dialoog.FileName;
                        s.OpslagFormaat = dialoog.FilterIndex;
                        s.OpenJson(dialoog.FileName);
                        break;
                }
            }
        }
    }
}
