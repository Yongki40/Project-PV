﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project_PV
{
    class BattleState : GameState
    {
        public GameStateManager gsm { get; set; }
        public List<int> gambar { get; set; }
        public int x { get; set; }
        karakter player = new ninja("ninnin", 50, new equip[5], new List<string>(), 5, 5);
        public string url { get; set; }
        public BattleState(GameStateManager gsm,Form1 form)
        {
            Random r = new Random();
            this.gsm = gsm;
            gambar = new List<int>();
            gambar.Add(r.Next(24) + 1);
            gambar.Add(r.Next(24) + 1);
            gambar.Add(r.Next(24) + 1);
            gambar.Add(r.Next(24) + 1);
            gambar.Add(r.Next(24) + 1);
            imgBg1 = (Image)background1;
            imgBg2 = (Image)background2;
            imgBg3 = (Image)background3;
            imgLast = (Image)last;
            imgDoor = (Image)door;
            x = 0;
            img = (Image)O;

            //for (int i = 0; i < gambar.Count; i++)
            //{
            //    PictureBox pb = new PictureBox();
            //    pb.Width = 450;
            //    pb.Height = 450;
            //    pb.Location = new Point(0, 0);
            //     Properties.Resources.ResourceManager.GetObject("courtyard_random___" + gambar[i] + "_");
            //    Image img = (Image)O;
            //    pb.Image = img;
            //    form.Controls.Add(pb);
            //}

        }

        public override void init()
        {
            throw new NotImplementedException();
        }
        object background1 = Properties.Resources.ResourceManager.GetObject("courtyard_backgroundcoba___1_");
        object background2 = Properties.Resources.ResourceManager.GetObject("courtyard_backgroundcoba___2_");
        object background3 = Properties.Resources.ResourceManager.GetObject("courtyard_backgroundcoba___3_");
        object last = Properties.Resources.ResourceManager.GetObject("courtyard_last"); 
        object door = Properties.Resources.ResourceManager.GetObject("courtyard_door"); 
        Image imgLast;
        Image imgDoor;
        Image imgBg1;
        Image imgBg2;
        Image imgBg3;
        object O = Properties.Resources.ResourceManager.GetObject("lala");
        Image img ;
        public override void draw(Graphics g)
        {

            //g.DrawImage(imgBg2, x, 20, 360, 400);
            //g.DrawImage(imgLast, x+220, 20, -220,400);
            //g.DrawImage(imgBg1, x, 0, 360, 100);
            //g.DrawImage(imgBg3, x, 320, 360, 100);

            //g.DrawImage(imgBg2, x + 140, 20, 360, 400);
            //g.DrawImage(imgDoor, x+140, 20, 360, 400);
            //g.DrawImage(imgBg1, x + 140, 0, 360, 100);
            //g.DrawImage(imgBg3, x + 140, 320, 360, 100);
            for (int i = 0; i < gambar.Count; i++)
            {
                object O = Properties.Resources.ResourceManager.GetObject("courtyard_randomcoba___4_");
                //object O = Properties.Resources.ResourceManager.GetObject("lala2");
                Image img = (Image)O;
                g.DrawImage(imgBg2, x + 340 + i * 360, 20, 450, 450);
                g.DrawImage(img, x +340+ i * 360, 0, 450, 450);
                g.DrawImage(imgBg1, x + 340 + i * 360, 0, 450, 100);
                g.DrawImage(imgBg3, x + 340 + i * 360, 380, 450, 100);
            }

            //g.DrawImage(imgBg2, x + 340 + 5 * 360, 20, 360, 400);
            //g.DrawImage(imgDoor, x + 340 + 5 * 360, 20, 360, 400);
            //g.DrawImage(imgBg1, x + 340 + 5 * 360, 0, 360, 100);
            //g.DrawImage(imgBg3, x + 340 + 5 * 360, 320, 360, 100);

            player.getImage(g);
            player.hero_move_now++;
        }

        public override void mouse_click(object sender, MouseEventArgs e)
        {

        }
        public override void key_keydown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.D)
            {
                x -= 10;
                player.hero_move = "run";
            }
            if (e.KeyData == Keys.A&&x<0)
            {
                x += 10;
                player.hero_move = "run";
            }
        }
    }
}
