using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Resources;
using System.IO;
using System.Drawing.Imaging;
using System.Collections;

namespace SchetsEditor
{
    public class SchetsWin : Form
    {   
        MenuStrip menuStrip;
        SchetsControl schetscontrol;
        ISchetsTool huidigeTool;
        Panel paneel;
        bool vast;
        public Schets schets; //New!
        string Tekst;           //New!
        public string Bestandsnaam = "";       //New!
        int formaat;            //New!!
        public int OpslagFormaat;       //New!
        ResourceManager resourcemanager
            = new ResourceManager("SchetsEditor.Properties.Resources"
                                 , Assembly.GetExecutingAssembly()
                                 );



    private void veranderAfmeting(object o, EventArgs ea)
        {
            schetscontrol.Size = new Size ( this.ClientSize.Width  - 70
                                          , this.ClientSize.Height - 50);
            paneel.Location = new Point(64, this.ClientSize.Height - 30);
        }

        private void klikToolMenu(object obj, EventArgs ea)
        {
            this.huidigeTool = (ISchetsTool)((ToolStripMenuItem)obj).Tag;
        }

        private void klikToolButton(object obj, EventArgs ea)
        {
            this.huidigeTool = (ISchetsTool)((RadioButton)obj).Tag;
        }
        private bool vraagafsluiten() //NEW
        {
            DialogResult antwoord = MessageBox.Show("Weet u het zeker? \nEr zijn onopgeslagen wijzigingen aangebracht!", 
                "Zeker weten?", MessageBoxButtons.OKCancel);
            if (antwoord == DialogResult.OK) { return true; }else { return false; }  
        }
        public void afsluiten(object sender, EventArgs e) // NEW
        {
            if (schets.Changed)
                if (vraagafsluiten())
                    this.Close();
            else { this.Close(); }
        }
        private void afsluitenx(object sender, FormClosingEventArgs e) //NEW
        {
            if (schets.Changed)
                if (!vraagafsluiten())
                    e.Cancel = true;
            else { }
        }
        private void opslaanals(object obj, EventArgs ea) //New method!
        {
            SaveFileDialog dialoog = new SaveFileDialog();
            dialoog.Filter = "Png|*.png|Jpeg|*.jpeg|Bmp|*.bmp|Alle files|*.*";
            dialoog.Title = "Opslaan als ...";
            if(dialoog.ShowDialog() == DialogResult.OK)
            {
                Bitmap saved = schets.Bitmap;
                this.Tekst = dialoog.FileName;
                schrijfNaarFile(saved);
                formaat = dialoog.FilterIndex;
                Bestandsnaam = this.Tekst;
                OpslagFormaat = formaat;
            }
        }
        private void opslaan(object obj, EventArgs ea) //New method!
        {
            if (Bestandsnaam != "") { this.Tekst = Bestandsnaam; formaat = OpslagFormaat; Bitmap saved = schets.Bitmap; schrijfNaarFile(saved); }
            else opslaanals(obj, ea);
        }
        private void schrijfNaarFile(Bitmap Saved)      //New method!
        {
            //MemoryStream writer = new MemoryStream(); //Voor opslaan als naar bitmap die weer te gebruiken is ... punt 5 ofzo
            Console.WriteLine(this.Tekst);
            Console.WriteLine(formaat);
            switch (formaat)
            {
                case 0:
                    Saved.Save(this.Tekst, ImageFormat.Png);
                    break;
                case 1:
                    Saved.Save(this.Tekst, ImageFormat.Jpeg);
                    break;
                case 2:
                    Saved.Save(this.Tekst, ImageFormat.Bmp);
                    break;
                case 3:
                    Saved.Save(this.Tekst);
                    break;
            }
            //writer.Close();
        }
        public SchetsWin()
        {

            ISchetsTool[] deTools = { new PenTool() 
                                    , new LijnTool()
                                    , new RechthoekTool()
                                    , new VolRechthoekTool()
                                    , new CirkelTool()
                                    , new VolCirkelTool()
                                    , new TekstTool()
                                    , new GumTool()
                                    };

            String[] deKleuren = { "Black", "Red", "Green", "Blue"
                                 , "Yellow", "Magenta", "Cyan" 
                                 };

            this.ClientSize = new Size(700, 500);
            huidigeTool = deTools[0];

            schetscontrol = new SchetsControl();
            schetscontrol.Location = new Point(64, 10);
            schets = schetscontrol.Schets; //New!
            this.FormClosing += afsluitenx; //NEW!
            schetscontrol.MouseDown += (object o, MouseEventArgs mea) =>
                                       {   vast=true;  
                                           huidigeTool.MuisVast(schetscontrol, mea.Location); 
                                       };
            schetscontrol.MouseMove += (object o, MouseEventArgs mea) =>
                                       {   if (vast)
                                           huidigeTool.MuisDrag(schetscontrol, mea.Location); 
                                       };
            schetscontrol.MouseUp   += (object o, MouseEventArgs mea) =>
                                       {   if (vast)
                                           huidigeTool.MuisLos (schetscontrol, mea.Location);
                                           vast = false; 
                                       };
            schetscontrol.KeyPress +=  (object o, KeyPressEventArgs kpea) => 
                                       {   huidigeTool.Letter  (schetscontrol, kpea.KeyChar); 
                                       };
            this.Controls.Add(schetscontrol);

            menuStrip = new MenuStrip();
            menuStrip.Visible = false;
            this.Controls.Add(menuStrip);
            this.maakFileMenu();
            this.maakToolMenu(deTools);
            this.maakAktieMenu(deKleuren);
            this.maakToolButtons(deTools);
            this.maakAktieButtons(deKleuren);
            this.Resize += this.veranderAfmeting;
            this.veranderAfmeting(null, null);
        }

        private void maakFileMenu()
        {   
            ToolStripMenuItem menu = new ToolStripMenuItem("File");
            menu.MergeAction = MergeAction.MatchOnly;
            menu.DropDownItems.Add("Sluiten", null, this.afsluiten);
            menu.DropDownItems.Add("Opslaan als..", null, this.opslaanals);  //New!
            menu.DropDownItems.Add("Opslaan", null, this.opslaan); //NEW!
            menuStrip.Items.Add(menu);
        }

        private void maakToolMenu(ICollection<ISchetsTool> tools)
        {   
            ToolStripMenuItem menu = new ToolStripMenuItem("Tool");
            foreach (ISchetsTool tool in tools)
            {   ToolStripItem item = new ToolStripMenuItem();
                item.Tag = tool;
                item.Text = tool.ToString();
                item.Image = (Image)resourcemanager.GetObject(tool.ToString());
                item.Click += this.klikToolMenu;
                menu.DropDownItems.Add(item);
            }
            menuStrip.Items.Add(menu);
        }

        private void maakAktieMenu(String[] kleuren)
        {   
            ToolStripMenuItem menu = new ToolStripMenuItem("Aktie");
            menu.DropDownItems.Add("Clear", null, schetscontrol.Schoon );
            menu.DropDownItems.Add("Roteer", null, schetscontrol.Roteer );
            ToolStripMenuItem submenu = new ToolStripMenuItem("Kies kleur");
            foreach (string k in kleuren)
                submenu.DropDownItems.Add(k, null, schetscontrol.VeranderKleurViaMenu);
            menu.DropDownItems.Add(submenu);
            menuStrip.Items.Add(menu);
        }

        private void maakToolButtons(ICollection<ISchetsTool> tools)
        {
            int t = 0;
            foreach (ISchetsTool tool in tools)
            {
                RadioButton b = new RadioButton();
                b.Appearance = Appearance.Button;
                b.Size = new Size(45, 62);
                b.Location = new Point(10, 10 + t * 62);
                b.Tag = tool;
                b.Text = tool.ToString();
                b.Image = (Image)resourcemanager.GetObject(tool.ToString());
                b.TextAlign = ContentAlignment.TopCenter;
                b.ImageAlign = ContentAlignment.BottomCenter;
                b.Click += this.klikToolButton;
                this.Controls.Add(b);
                if (t == 0) b.Select();
                t++;
            }
        }

        private void maakAktieButtons(String[] kleuren)
        {   
            paneel = new Panel();
            paneel.Size = new Size(600, 24);
            this.Controls.Add(paneel);
            
            Button b; Label l; ComboBox cbb;
            b = new Button(); 
            b.Text = "Clear";  
            b.Location = new Point(  0, 0); 
            b.Click += schetscontrol.Schoon; 
            paneel.Controls.Add(b);
            
            b = new Button(); 
            b.Text = "Rotate"; 
            b.Location = new Point( 80, 0); 
            b.Click += schetscontrol.Roteer; 
            paneel.Controls.Add(b);
            
            l = new Label();  
            l.Text = "Penkleur:"; 
            l.Location = new Point(180, 3); 
            l.AutoSize = true;               
            paneel.Controls.Add(l);
            
            cbb = new ComboBox(); cbb.Location = new Point(240, 0); 
            cbb.DropDownStyle = ComboBoxStyle.DropDownList; 
            cbb.SelectedValueChanged += schetscontrol.VeranderKleur;
            foreach (string k in kleuren)
                cbb.Items.Add(k);
            cbb.SelectedIndex = 0;
            paneel.Controls.Add(cbb);
        }
    }
}
