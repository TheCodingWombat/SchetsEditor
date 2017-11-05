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
        public int Opslagformaat;       //New!
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
            if (antwoord == DialogResult.OK) { return true; }
            else { return false; }  
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
            dialoog.Filter = "Png|*.png|Jpeg|*.jpeg|Bmp|*.bmp|Project File|*.txt|Alle files|*.*";
            dialoog.Title = "Opslaan als ...";
            if(dialoog.ShowDialog() == DialogResult.OK)
            {
                Bitmap saved = new Bitmap(schets.Bitmap);
                this.Tekst = dialoog.FileName;
                Console.WriteLine(Path.GetExtension(this.Tekst).ToLower());
                Console.WriteLine(dialoog.FilterIndex + "dialoog.FilterIndex");
                formaat = dialoog.FilterIndex;
                schrijfNaarFile(saved);
                Bestandsnaam = this.Tekst;
            }
        }
        public int OpslagFormaat //New Method!
        {
            set { Opslagformaat = value;}
        }
        private void opslaan(object obj, EventArgs ea) //New method!
        {
            if (Bestandsnaam != "") { this.Tekst = Bestandsnaam; Bitmap saved = new Bitmap(schets.Bitmap); schets.Bitmap.Dispose(); System.IO.File.Delete(this.Tekst); formaat = Opslagformaat;  schrijfNaarFile(saved); }
            else opslaanals(obj, ea);
        }
        private void schrijfNaarFile(Bitmap Saved)      //New method!
        {
            //MemoryStream writer = new MemoryStream(); //Voor opslaan als naar bitmap die weer te gebruiken is ... punt 5 ofzo
            Console.WriteLine(this.Tekst);
            Console.WriteLine("formaat:" + formaat);
            Console.WriteLine(formaat);
            switch (formaat)
            {
                case 1:
                    Saved.Save(this.Tekst, ImageFormat.Png);
                    Console.WriteLine("PNG!");
                    break;
                case 2:
                    Saved.Save(this.Tekst, ImageFormat.Jpeg);
                    Console.WriteLine("JPG!");
                    break;
                case 3:
                    Saved.Save(this.Tekst, ImageFormat.Bmp);
                    Console.WriteLine("BMP!");
                    break;
                case 4:
                    Console.WriteLine("JSON!"); schrijfNaarJson(); Console.WriteLine("JSON!");
                    break;
                case 5:
                    Saved.Save(this.Tekst);
                    Console.WriteLine("ALLE FILES");
                    break;
            }
            schets.Changed = false;
            //writer.Close();
        }
        private void schrijfNaarJson()
        {
            Console.WriteLine("Schrijf naar JSON");
            
            ArrayList elementen = schetscontrol.TekenElementen;
            StreamWriter writer= new StreamWriter(this.Tekst);
            foreach(TekenElement e in elementen){
                Console.WriteLine(new Pen(e.kwast).Color.R);

                    writer.WriteLine(e.tool.ToString() + " " + e.beginPunt.X + " " + e.beginPunt.Y + " " + e.eindPunt.X + " " + e.eindPunt.Y + " " + new Pen(e.kwast).Color.A + " " + new Pen(e.kwast).Color.R + " " + new Pen(e.kwast).Color.G + " " + new Pen(e.kwast).Color.B);
                //writer.WriteLine("Mission failed!");
            }
            writer.Close();
        }
        public void OpenJson(string File)
        {
            int n = 0; //
            string regel;
            StreamReader sr = new StreamReader(File);
            schetscontrol.TekenElementen = null;
            schetscontrol.TekenElementen = new ArrayList();
            while((regel = sr.ReadLine()) != null)
            {
                n++;
                string [] r = regel.Split(' ');
                ISchetsTool tool;
                if(r[0] == "vlak") tool = new VolRechthoekTool();
                else if(r[0] == "kader") tool = new RechthoekTool();
                else if(r[0] == "rondje") tool = new VolCirkelTool();
                else if(r[0] == "cirkel") tool = new CirkelTool();
                else if(r[0] == "lijn") tool = new LijnTool();
                else if(r[0] == "pen") tool = new PenTool();
                else if(r[0] == "tekst") tool = new TekstTool();
                else tool = null;
                

                Point beginpunt = new Point(int.Parse(r[1]), int.Parse(r[2]));
                Point eindpunt = new Point(int.Parse(r[3]), int.Parse(r[4]));
                Brush brush = new SolidBrush(Color.FromArgb(int.Parse(r[5]),int.Parse(r[6]),int.Parse(r[7]),int.Parse(r[8])));
                Console.WriteLine("De brush is" +brush + " met color: "+ Color.FromArgb(int.Parse(r[5]),int.Parse(r[6]),int.Parse(r[7]),int.Parse(r[8])));
                Console.WriteLine(tool.ToString() + " " + beginpunt + " " + eindpunt + " " + brush);
                schetscontrol.TekenElementen.Add(new TekenElement(tool, beginpunt, eindpunt, brush));
                schetscontrol.TekenBitmapOpnieuw(); Console.WriteLine("TekenBitMapOpnieuw Succes voor nummer: " + n);
            }    

        }
        public SchetsWin()
        {
            ISchetsTool[] deTools = { new PenTool() 
                                    , new LijnTool()
                                    , new RechthoekTool()
                                    , new VolRechthoekTool()
                                    , new CirkelTool() //
                                    , new VolCirkelTool() //
                                    , new TekstTool()
                                    , new GumTool()
                                    };

           // String[] deKleuren = { "Black", "Red", "Green", "Blue"
             //                    , "Yellow", "Magenta", "Cyan" 
               //                  };

            this.ClientSize = new Size(700, 500);
            huidigeTool = deTools[0];

            schetscontrol = new SchetsControl();
            schetscontrol.Location = new Point(64, 10);
            schets = schetscontrol.Schets; //New!
            schets.Changed = false; //new
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
            this.maakAktieMenu(); //:)
            this.maakToolButtons(deTools);
            this.maakAktieButtons(); // :)
            this.Resize += this.veranderAfmeting;
            this.veranderAfmeting(null, null);
        }

        private void maakFileMenu()
        {   
            ToolStripMenuItem menu = new ToolStripMenuItem("File");
            menu.MergeAction = MergeAction.MatchOnly;
            menu.DropDownItems.Add("Sluiten", null, this.afsluiten);
            menu.DropDownItems.Add("Opslaan", null, this.opslaan); //NEW!
            menu.DropDownItems.Add("Opslaan als..", null, this.opslaanals);  //New!
            (menu.DropDownItems[1] as ToolStripMenuItem).ShortcutKeys = (Keys)(Keys.Control | Keys.S);
            (menu.DropDownItems[2] as ToolStripMenuItem).ShortcutKeys = (Keys)(Keys.Control | Keys.Shift | Keys.S);
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

        private void maakAktieMenu()
        {   
            ToolStripMenuItem menu = new ToolStripMenuItem("Aktie");
            
            //ToolStripDropDownButton Undo = new ToolStripDropDownButton("Undo",null,schetscontrol.Undo);
            menu.DropDownItems.Add("Clear", null, schetscontrol.Schoon );
            menu.DropDownItems.Add("Roteer", null, schetscontrol.Roteer );
            menu.DropDownItems.Add("Kleur", null, schetscontrol.VeranderKleur);
            menu.DropDownItems.Add("Undo", null, schetscontrol.Undo);
            menu.DropDownItems.Add("Redo", null, schetscontrol.Redo);
            (menu.DropDownItems[3] as ToolStripMenuItem).ShortcutKeys = (Keys)(Keys.Control | Keys.Z);
            (menu.DropDownItems[4] as ToolStripMenuItem).ShortcutKeys = (Keys)(Keys.Control | Keys.Y);
            

            //ToolStripMenuItem submenu = new ToolStripMenuItem("Kies kleur");
            //foreach (string k in kleuren)
              //  submenu.DropDownItems.Add(k, null, schetscontrol.VeranderKleurViaMenu);
            //menu.DropDownItems.Add(submenu);
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

        private void maakAktieButtons()
        {   
            paneel = new Panel();
            paneel.Size = new Size(600, 24);
            this.Controls.Add(paneel);
            
            Button b; //Label l; ComboBox cbb;
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
            
            //l = new Label();  
            //l.Text = "Penkleur:"; 
            //l.Location = new Point(180, 3); 
            //l.AutoSize = true;               
            //paneel.Controls.Add(l);
            
            b = new Button(); b.Location = new Point(160, 0);
            b.Text = "Kleur";
            b.Click += schetscontrol.VeranderKleur;
            paneel.Controls.Add(b);
            //cbb = new ComboBox(); cbb.Location = new Point(240, 0); //dit zal een button worden
            //cbb.DropDownStyle = ComboBoxStyle.DropDownList; 
            //cbb.SelectedValueChanged += schetscontrol.VeranderKleur;
            //foreach (string k in kleuren)
              //  cbb.Items.Add(k);
            //cbb.SelectedIndex = 0;
            //paneel.Controls.Add(cbb);
            b = new Button(); b.Location = new Point(240, 0);
            b.Text = "Undo teken";
            b.Click += schetscontrol.Undo;
            paneel.Controls.Add(b);

            b = new Button(); b.Location = new Point(320, 0);
            b.Text = "Redo teken";
            b.Click += schetscontrol.Redo;
            paneel.Controls.Add(b);
        }
    }
}
