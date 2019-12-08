﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace Project_PV
{
    class musuh
    {
        public int hp { get; set; }
        public List<Skill> skill { get; set; }
        public string tipe { get; set; }
        public string tipe_gerak { get; set; }
        public int tipe_gerak_ke { get; set; }

        public musuh(int hp, List<Skill> skill, string tipe, string tipe_gerak, int tipe_gerak_ke)
        {
            this.hp = hp;
            this.skill = skill;
            this.tipe = tipe;
            this.tipe_gerak = tipe_gerak;
            this.tipe_gerak_ke = tipe_gerak_ke;
        }

        public void getImage(Graphics g,int x)
        {
            try
            {
                gambar(g,x);
            }
            catch (Exception)
            {
                tipe_gerak_ke = 1;
                gambar(g,x);
            }
        }
        public void gambar(Graphics g,int x)
        {
            object O = Properties.Resources.ResourceManager.GetObject(tipe + "_" + tipe_gerak + "___" + tipe_gerak_ke+ "_");
            Image img = (Image)O;
            g.DrawImage(img, x, 250, 100, 150);
        }

    }

    class yeti : musuh
    {
        public yeti()
            : base(100,new List<Skill>(),"yeti","idle",1)
        {
            skill.Add(new yeti1());
            skill.Add(new yeti2());
            skill.Add(new yeti3());
            skill.Add(new yeti4());
        }
    }
}
