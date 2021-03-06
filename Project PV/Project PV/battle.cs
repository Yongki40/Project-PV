﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;

namespace Project_PV
{
    
    class battle : GameState
    {
        public Image background{ get; set; }
        public List<musuh> musuh { get; set; }
        public List<karakter> player { get; set; }
        GameStateManager gsm;

        public List<int> locSkill { get; set; }

        string aktif = "inv";

        int pilih_attack = 0;
        bool serang = false;
        bool serang_musuh = false;
        bool delay_aktif = false;
        bool ganti_posisi = false;

        int pilihHero = 0;
        int targetHero = 0;
        int pilihMusuh= 0;
        int targetMusuh= 0;
        int zoom = 0;
        int zoom_bkg = 0;

        int timer_attack = 0;

        MediaPlayer sfx = new MediaPlayer();

        public List<Inventory> battleInv { get; set; }
        List<turn> turnAttack = new List<turn>();
        int turn_ke = 0;
        dungeon thisDungeon;
        public battle(GameStateManager gsm,Image back,dungeon dgn)
        {
            thisDungeon = dgn;



            this.gsm = gsm;
            battleInv = thisDungeon.battleInv;
            player = new List<karakter>(); ;
            for (int i = 0; i < 4; i++)
            {
                try
                {
                    player.Add(gsm.player.currentCharacters[i]);
                }
                catch (Exception)
                {

                }
            }
			Random rand = new Random();
            musuh = new List<musuh>();
			int a = rand.Next(1, 3);
		
            for (int i = 0; i < a; i++)
            {
				int b = rand.Next(1, 8);
				if (b == 1)
				{
					musuh.Add(new yeti(700 + i * 100));
				} else if(b == 2)
				{
					musuh.Add(new Gargoyle(700 + i * 100));
				}else if (b == 3)
                {
                    musuh.Add(new Gargoyle(700 + i * 100));
                }
                else if(b== 4)
				{
					musuh.Add(new Boarman(700 + i * 100));
				} else if(b== 5)
				{
					musuh.Add(new Larry(700 + i * 100));
				} else if(b== 6)
				{
					musuh.Add(new FlameEater(700 + i * 100));
				}
				else
				{
					musuh.Add(new GiantGoblin(700 + i * 100));
				}
                turnAttack.Add(new turn(0, i, musuh[i].speed));
            }


            locSkill = new List<int>();
            locSkill.Add(310);
            locSkill.Add(365);
            locSkill.Add(420);
            locSkill.Add(476);

            for (int i = 0; i < player.Count; i++)
            {
                player[i].x = 350 - 100 * i;
                turnAttack.Add(new turn(1, i, player[i].speed));
            }

            for (int i = 0; i < turnAttack.Count-1; i++)
            {
                for (int j = i; j < turnAttack.Count - i-1; j++)
                {
                    if (turnAttack[i].speed < turnAttack[j].speed)
                    {
                        turn temp = turnAttack[i];
                        turnAttack[i] = turnAttack[j];
                        turnAttack[j] = temp;
                    }
                }
            }
            background = back;
            imgpPlayer = (Image)Properties.Resources.ResourceManager.GetObject("panel_player2");
            imgpInv = (Image)Properties.Resources.ResourceManager.GetObject("panel_inventory");

            if (turnAttack[0].jenis == 1)
            {
                pilihHero = turnAttack[0].ke;
            }
            else
            {
                pilihMusuh = turnAttack[0].ke;
                delay_aktif = true;
            }


            dmg_min = player[pilihHero].skills[pilih_attack].status_skill.dmg_min + "";
            dmg_max = player[pilihHero].skills[pilih_attack].status_skill.dmg_max + "";
            acc = player[pilihHero].skills[pilih_attack].status_skill.acc + "";
            crit = player[pilihHero].skills[pilih_attack].status_skill.crit + "%";
            prot = player[pilihHero].skills[pilih_attack].status_skill.def + "";
        }
        Image imgpPlayer;
        Image imgpInv;
        public void readInventory()
        {
            battleInv = thisDungeon.battleInv;
        }
        

        public override void draw(Graphics g)
        {
            if (!serang&&!serang_musuh)
            {
                g.DrawImage(background, 0, 0, 1300, 450);
            }
            else
            {
                g.DrawImage(background, 0 - zoom_bkg / 2, 0 - zoom_bkg, 1300 + zoom_bkg, 450 + zoom_bkg);
            }

            if (!serang && !serang_musuh)
            {
                for (int i = 0; i < musuh.Count; i++)
                {
                    musuh[i].getImage(g);
                    musuh[i].musuh_move_now++;
                }
            }
            else
            {
                
                if (serang )
                {
                    for (int i = 0; i < musuh.Count; i++)
                    {
                        if (targetMusuh != i)
                        {
                            musuh[i].getImage(g);
                            musuh[i].musuh_move_now++;
                        }
                    }

                    musuh[targetMusuh].getImageAttack(g, zoom);
                }
                if(serang_musuh)
                {
                    for (int i = 0; i < musuh.Count; i++)
                    {
                        if (pilihMusuh != i)
                        {
                            musuh[i].getImage(g);
                            musuh[i].musuh_move_now++;
                        }
                    }
                    musuh[pilihMusuh].getImageAttack(g, zoom);
                }
            }

            if (!serang && !serang_musuh)
            {
                for (int i = 0; i < player.Count; i++)
                {
                    player[i].getImage(g);
                    player[i].hero_move_now++;
                }
            }
            else
            {
                if (serang)
                {
                    for (int i = 0; i < player.Count; i++)
                    {
                        if (turnAttack[turn_ke].ke != i)
                        {
                            player[i].getImage(g);
                            player[i].hero_move_now++;
                        }
                    }

                    player[turnAttack[turn_ke].ke].getImageAttack(g, zoom);
                }
                if (serang_musuh)
                {

                    for (int i = 0; i < player.Count; i++)
                    {
                        if (targetHero != i)
                        {
                            player[i].getImage(g);
                            player[i].hero_move_now++;
                        }
                    }
                    player[targetHero].getImageAttack(g, zoom);
                }
            }



            g.DrawImage((Image)Properties.Resources.ResourceManager.GetObject("side_decor"), 0, 420, 120, 270);
            g.DrawImage(imgpPlayer, 70 + 22, 420, 528, 100);
            g.DrawImage((Image)Properties.Resources.ResourceManager.GetObject(player[pilihHero].hero + "_icon"), 135, 440, 68, 68);

            for (int i = 0; i < player[pilihHero].skills.Count; i++)
            {
                g.DrawImage(player[pilihHero].skills[i].icon, 308 + 55 * i, 447, 52, 52);
            }
            g.DrawImage((Image)Properties.Resources.pilihSkill, 308 + 55 * pilih_attack, 447, 52, 52);


            g.DrawImage((Image)Properties.Resources.ResourceManager.GetObject("ability_move"), 308 + 55 * 4, 447, 52, 52);
            g.DrawImage((Image)Properties.Resources.ResourceManager.GetObject("ability_pass"), 308 + 55 * 5, 447, 10, 52);


            g.DrawImage((Image)Properties.Resources.ResourceManager.GetObject("panel_stat"), 70 + 50, 520, 500, 170);
            g.DrawImage(imgpInv, 70 + 550, 420, 550, 270);

            if (turnAttack[turn_ke].jenis == 1 && zoom == 0)
            {
                g.DrawImage((Image)Properties.Resources.pilihhero, player[turnAttack[turn_ke].ke].x - gerak_icon + 20, 330, (gerak_icon * 2) + 50, 100);
                for (int i = 0; i < 4; i++)
                {
                    if (player[pilihHero].skills[pilih_attack].target[i] == 1 && i < musuh.Count)
                    {
                        if (musuh[i].hp > 0)
                        {
                            g.DrawImage((Image)Properties.Resources.target, musuh[i].x - gerak_icon + 20, 330, (gerak_icon * 2) + 50, 100);
                        }
                    }
                }
            }
            ////MessageBox.Show((Image)imgpInv+"");
            Font font = new Font("Arial", 15.0f);

            System.Drawing.Brush br = new SolidBrush(System.Drawing.Color.White);
            if (aktif == "inv")
            {
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if ((i * 8) + j < battleInv.Count)
                        {
                            if (battleInv[(i * 8) + j] is Inventory)
                            {
                                g.DrawImage(battleInv[(i * 8) + j].gambar, (float)(640 + j * 61.5), 440 + i * 120, 50, 110);
                                g.DrawString(battleInv[(i * 8) + j].jumlah + "", font,br, (float)(640 + j * 61.5), 445 + i * 120);
                            }
                        }
                    }
                }
            }
            g.DrawImage((Image)Properties.Resources.ResourceManager.GetObject("side_decor"), 1285, 420, -120, 270);

            if (player[pilihHero].hero_equip[0].type != "zonk")
            {
                g.DrawImage(player[pilihHero].hero_equip[0].img, 310, 562, 45, 105);
            }
            if (player[pilihHero].hero_equip[1].type != "zonk")
            {
                g.DrawImage(player[pilihHero].hero_equip[1].img, 373, 562, 45, 105);
            }

            for (int i = 0; i < 4; i++)
            {
                if (player[pilihHero].skills[pilih_attack].rank[i] == 1)
                {
                    g.DrawImage((Image)Properties.Resources.Yellow_dot, 145 + 10 * i, 575, 10, 10);
                }
                else
                {
                    g.DrawImage((Image)Properties.Resources.Grey_dot, 145 + 10 * i, 575, 10, 10);
                }

                if (player[pilihHero].skills[pilih_attack].target[i] == 1)
                {
                    g.DrawImage((Image)Properties.Resources.Red_dot, 200 + 10 * i, 575, 10, 10);
                }
                else
                {
                    g.DrawImage((Image)Properties.Resources.Grey_dot, 200 + 10 * i, 575, 10, 10);
                }
            }

            g.DrawString("ACC        ", new Font("Arial", 12, FontStyle.Regular), br, 145, 587);
            g.DrawString("CRIT       ", new Font("Arial", 12, FontStyle.Regular), br, 145, 602);
            g.DrawString("DMG        ", new Font("Arial", 12, FontStyle.Regular), br, 145, 617);
            g.DrawString("DODGE      ", new Font("Arial", 12, FontStyle.Regular), br, 145, 632);
            g.DrawString("PROT       ", new Font("Arial", 12, FontStyle.Regular), br, 145, 647);

            g.DrawString(acc, new Font("Arial", 12, FontStyle.Regular), br, 210, 587);
            g.DrawString(crit, new Font("Arial", 12, FontStyle.Regular), br, 210, 602);
            g.DrawString(dmg_min + "-" + dmg_max, new Font("Arial", 12, FontStyle.Regular), br, 210, 617);
            g.DrawString(dodge, new Font("Arial", 12, FontStyle.Regular), br, 210, 632);
            g.DrawString(prot, new Font("Arial", 12, FontStyle.Regular), br, 210, 647);

            g.DrawString(player[pilihHero].hp + "/" + player[pilihHero].maxHp, new Font("Arial", 12, FontStyle.Regular), new SolidBrush(System.Drawing.Color.DarkRed), 178, 530);
            g.DrawString(player[pilihHero].hero_stress.stress_point + "/" + 200, new Font("Arial", 12, FontStyle.Regular), br, 178, 550);

            g.DrawString(player[pilihHero].nama, new Font("Arial", 15, FontStyle.Regular), br, 200, 445);
            g.DrawString(player[pilihHero].type, new Font("Arial", 13, FontStyle.Regular), br, 200, 475);

            for (int i = 0; i < musuh.Count; i++)
            {
                if (musuh[i].hp >= 0)
                {
                    int temp_hp = musuh[i].maxHp / musuh[i].hp;
                    int hitung_panjang = (int)(80 / temp_hp);
                    g.DrawImage((Image)Properties.Resources.health_pip_full, musuh[i].x + 5, 420, hitung_panjang, 10);
                }
            }
            for (int i = 0; i < player.Count; i++)
            {
                if (player[i].hero_buff == efek.bleed)
                {
                    g.DrawImage((Image)Properties.Resources.skill_attribute_bleed, player[i].x + 15, 410, 30, 30);
                }
                if (player[i].hero_buff == efek.blight)
                {
                    g.DrawImage((Image)Properties.Resources.skill_attribute_disease, player[i].x + 15, 410, 30, 30);
                }
                if (player[i].hero_buff == efek.marked)
                {
                    g.DrawImage((Image)Properties.Resources.skill_attribute_tag, player[i].x + 15, 410, 30, 30);
                }
                if (player[i].hero_buff == efek.stun)
                {
                    g.DrawImage((Image)Properties.Resources.skill_attribute_stun, player[i].x + 15, 410, 30, 30);
                }
            }
        }

        string dmg_min =   "10";
        string dmg_max =   "10";
        string acc =   "10";
        string crit =  "10";
        string dodge = "0";
        string prot =  "10";

        bool gerak = false;
        int gerak_icon = 0;
        public override void init()
        {
            
        }

        public override void key_keydown(object sender, KeyEventArgs e)
        {
            
        }

        public override void key_KeyUp(object sender, KeyEventArgs e)
        {
            
        }
        int pilihInv = -1;

        public override void mouse_click(object sender, MouseEventArgs e)
        {
            //MessageBox.Show(e.X + "" + e.Y);
            int x = e.X;
            int y = e.Y;
            Rectangle mouse = new Rectangle(x, y, 1, 1);
            for (int i = 0; i < locSkill.Count; i++)
            {
                Rectangle skill = new Rectangle(locSkill[i], 448, 45, 45);
                if (mouse.IntersectsWith(skill))
                {
                    pilih_attack = i;
                    dmg_min = player[pilihHero].skills[i].status_skill.dmg_min+"";
                    dmg_max = player[pilihHero].skills[i].status_skill.dmg_max+"";
                    acc =  player[pilihHero].   skills[i].status_skill.acc + "";
                    crit = player[pilihHero].   skills[i].status_skill.crit + "%";
                    prot = player[pilihHero].   skills[i].status_skill.def + "";
                }
            }

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if ((i * 8) + j < battleInv.Count)
                    {
                        if (battleInv[(i * 8) + j] is Inventory)
                        {
                            Rectangle recInv = new Rectangle((int)(640 + j * 61.5), 440 + i * 120, 50, 110);
                            if (recInv.IntersectsWith(mouse))
                            {
                                pilihInv = (i * 8) + j;
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < musuh.Count; i++)
            {
                Rectangle recMusuh = new Rectangle(musuh[i].x, 250, 100, 150);
                if (mouse.IntersectsWith(recMusuh))
                {
                    targetMusuh = i;
                    if (pilih_attack != -1 && turnAttack[turn_ke].jenis==1&&turnAttack[turn_ke].ke==pilihHero)
                    {
                        if (player[turnAttack[turn_ke].ke].skills[pilih_attack].target[turnAttack[turn_ke].ke] ==1)
                        {
                            if (player[turnAttack[turn_ke].ke].skills[pilih_attack].target[targetMusuh] == 1)
                            {
                                player[turnAttack[turn_ke].ke].getDamage(pilih_attack, targetMusuh, musuh);
                                serang = true;
                                player[turnAttack[turn_ke].ke].x = 520;
                                player[turnAttack[turn_ke].ke].hero_move = "skill" + (pilih_attack + 1);
                                player[turnAttack[turn_ke].ke].hero_move_now = 1;
                                //sfx.Stop();
                                //string RunningPath = AppDomain.CurrentDomain.BaseDirectory;
                                //string FileName = string.Format("{0}Resources\\char_share_imp_sword.wav", Path.GetFullPath(Path.Combine(RunningPath, @"..\..\")));
                                //sfx.Open(new System.Uri(FileName));
                                //sfx.Play();
                            }
                            else
                            {
                                MessageBox.Show("attack tidak sesuai target");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Posisi tidak pas");
                        }
                    }else if(turnAttack[turn_ke].ke != pilihHero)
                    {
                        MessageBox.Show("salah pilih hero");
                    }
                }
            }

            for (int i = 0; i < player.Count; i++)
            {
                Rectangle recPlayer = new Rectangle(player[i].x, 250, 100, 150);
                if (recPlayer.IntersectsWith(mouse))
                {
                    if (ganti_posisi)
                    {
                        targetHero = i;
                        gerak_geser = 10 * Math.Abs(pilihHero - targetHero);
                        gerak_geser_max = 100 * Math.Abs(pilihHero - targetHero);
                        timer_geser = true;
                    }else if (pilihInv!=-1&&battleInv[pilihInv].item==itemUse.bisa)
                    {
                        battleInv[pilihInv].getEffect(battleInv[pilihInv], player[i]);
                        battleInv[pilihInv].jumlah--;
                        if (battleInv[pilihInv].jumlah <= 0)
                        {
                            battleInv.RemoveAt(pilihInv);
                        }

                        pilihInv = -1;
                        gantiTurn();
                    }
                    else if (turnAttack[turn_ke].jenis==1&& player[turnAttack[turn_ke].ke].skills[pilih_attack].skill_efek == efek.heal)
                    {
                            player[turnAttack[turn_ke].ke].skills[pilih_attack].getDamageSkill(i, player);
                            gantiTurn();
                    }
                    else
                    {
                        pilihHero = i;
                    }
                }
            }

            Rectangle recPilih = new Rectangle(308 + 55 * 4, 447, 52, 52);
            if (mouse.IntersectsWith(recPilih))
            {
                ganti_posisi = true;
            }
        }
        int gerak_geser = 0;
        int gerak_geser_max = 0;
        bool timer_geser = false;
        public override void mouse_hover(object sender, MouseEventArgs e)
        {

        }

        public override void mouse_leave(object sender, MouseEventArgs e)
        {

        }
        Random r = new Random();

        public override void update()
        {

            if (!gerak)
            {
                gerak_icon+=2;
            }

            else
            {
                gerak_icon -= 2;
            }
            if (gerak_icon > 20 && !gerak)
            {
                gerak = true;
            }
            if (gerak_icon < 0 && gerak)
            {
                gerak = false;
            }

            if (zoom <= 80 && (serang == true || serang_musuh == true))
            {
                zoom += 30;
                zoom_bkg += 70;
            }

            if (serang)
            {
                timer_attack++;
                player[pilihHero].x += 1;
            }

            if (serang_musuh)
            {
                timer_attack++;
                musuh[pilihMusuh].x -= 1;
            }
            if (timer_attack == 30 && serang == true)
            {
                timer_attack = 0;
                zoom = 0;
                zoom_bkg = 0;
                player[pilihHero].x = 350 - 100 * pilihHero;
                player[pilihHero].hero_move = "idle";
                player[pilihHero].hero_move_now = 1;
                serang = false;
                if (musuh[targetMusuh].hp<=0)
                {
                    try
                    {
                        for (int i = 0; i < turnAttack.Count; i++)
                        {
                            if (turnAttack[i].jenis == 0)
                            {
                                if (turnAttack[i].ke == musuh.Count)
                                {
                                    turnAttack.RemoveAt(i);
                                    i--;
                                }
                                else if (turnAttack[i].ke > targetMusuh)
                                {
                                    turnAttack[i].ke--;
                                    musuh[turnAttack[i].ke].x -= 100;
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {

                    }
                    MessageBox.Show("mati");
                    musuh.RemoveAt(targetMusuh);
                }
                winCondition();
                gantiTurn();
            }

            if (timer_attack == 30 && serang_musuh == true)
            {
                timer_attack = 0;
                zoom = 0;
                zoom_bkg = 0;
                musuh[pilihMusuh].x = 700 + pilihMusuh * 100;
                musuh[pilihMusuh].musuh_move_now = 1;
                musuh[pilihMusuh].musuh_move = "idle";
                serang_musuh = false;
                player[targetHero].x = 350 - 100 * targetHero;

                if (player[targetHero].hp <= 0)
                {
                    for (int i = 0; i < turnAttack.Count; i++)
                    {
                        if (turnAttack[i].jenis == 1)
                        {
                            if (turnAttack[i].ke == player.Count)
                            {
                                turnAttack.RemoveAt(i);
                                i--;
                            }
                            else if (turnAttack[i].ke > targetHero)
                            {
                                player[turnAttack[i].ke].x += 100;
                                turnAttack[i].ke--;
                            }
                        }
                    }
                    player.RemoveAt(targetHero);
                    if (player.Count == 0)
                    {
                        MessageBox.Show("Mati Semua");
                        for (int i = 0; i < gsm.player.currentCharacters.Count; i++)
                        {
                            if (gsm.player.currentCharacters[i].hp <= 0)
                            {
                                gsm.player.currentCharacters.RemoveAt(i);
                                i--;
                            }
                        }
                        gsm.stage = Stage.mainMenu;
                        gsm.loadState(gsm.stage);
                    }
                }

                gantiTurn();
            }

            if (delay_aktif)
            {
                timer_attack++;
                if (timer_attack == 30)
                {
                    
                    targetHero = r.Next(player.Count);
                    int pilih_attack_musuh = r.Next(1, 4);
                    try
                    {
                        pilihMusuh = turnAttack[turn_ke].ke;
                        musuh[pilihMusuh].skill[pilih_attack_musuh].getDamageSkill(targetHero, player);
                        musuh[pilihMusuh].x = 650;
                        player[targetHero].x = 450;
                        musuh[pilihMusuh].musuh_move_now = 1;
                        musuh[pilihMusuh].musuh_move = "skill" + pilih_attack_musuh;
                    }
                    catch (Exception)
                    {
                        pilihMusuh = turnAttack[turn_ke].ke;
                        pilihMusuh = 0;
                        musuh[pilihMusuh].skill[pilih_attack_musuh].getDamageSkill(targetHero, player);
                        musuh[pilihMusuh].x = 650;
                        player[targetHero].x = 450;
                        musuh[pilihMusuh].musuh_move_now = 1;
                        musuh[pilihMusuh].musuh_move = "skill" + pilih_attack_musuh;
                    }
                    delay_aktif = false;
                    timer_attack = 0;
                    serang_musuh = true;
                    //sfx.Stop();
                    //string RunningPath = AppDomain.CurrentDomain.BaseDirectory;
                    //string FileName = string.Format("{0}Resources\\yeti_attack_sfx_1.wav", Path.GetFullPath(Path.Combine(RunningPath, @"..\..\")));
                    //sfx.Open(new System.Uri(FileName));
                    //sfx.Play();

                }
            }

            if (timer_geser)
            {
                if (0 == gerak_geser_max)
                {
                    timer_geser = false;
                    ganti_posisi = false;


                    karakter swap_char = player[pilihHero];
                    player[pilihHero] = player[targetHero];
                    player[targetHero] = swap_char;



                    int tmp_ind = turnAttack[turn_ke].ke;//0
                    for (int i = 0; i < turnAttack.Count; i++)
                    {
                        if (turnAttack[i].jenis == 1 && turnAttack[i].ke == targetHero)
                        {
                            turnAttack[turn_ke].ke = turnAttack[i].ke;
                            turnAttack[i].ke = tmp_ind;
                        }
                    }

                    tmp_ind = pilihHero;
                    pilihHero = targetHero;
                    targetHero = tmp_ind;
                    gantiTurn();
                }
                else
                {
                    if (pilihHero >= targetHero)
                    {
                        player[pilihHero].x +=gerak_geser;
                        player[targetHero].x -=gerak_geser;
                    }
                    else
                    {
                        player[pilihHero].x -= gerak_geser;
                        player[targetHero].x += gerak_geser;
                    }
                    gerak_geser_max -= gerak_geser;
                }
            }
        }

        public void winCondition()
        {
            if (musuh.Count == 0)
            {
                gsm.player.currentCharacters = player;
                gsm.dungeon.myLoc = location.area;
                gsm.dungeon.Area_besar[gsm.dungeon.ke].battle = true;
            }
        }
        bool gerak_attack = false;
        public void gantiTurn()
        {
            gerak_attack = false;
            try
            {
                if (gerak_attack == false)
                {
                    turn_ke++;
                    if (turnAttack[turn_ke].jenis == 1)
                    {
                        gerak_attack = true;
                        pilihHero = turnAttack[turn_ke].ke;

                        player[turnAttack[turn_ke].ke].hero_buff_turn--;
                        if (player[turnAttack[turn_ke].ke].hero_buff == efek.bleed)
                            {
                                player[turnAttack[turn_ke].ke].hp -= player[pilihHero].hp / 10;
                                player[turnAttack[turn_ke].ke].hero_stress.stress_point += 10;
                                MessageBox.Show(player[pilihHero].nama + " terkena Bleed " + player[turnAttack[turn_ke].ke].hero_buff_turn);
                            }
                            if (player[turnAttack[turn_ke].ke].hero_buff == efek.blight)
                            {
                                player[turnAttack[turn_ke].ke].hp -= (player[pilihHero].maxHp / 10);
                                player[turnAttack[turn_ke].ke].hero_stress.stress_point += 10;
                                MessageBox.Show(player[pilihHero].nama + " terkena Blight " + player[turnAttack[turn_ke].ke].hero_buff_turn);
                            }
                            if (player[turnAttack[turn_ke].ke].hero_buff == efek.stun)
                            {
                                gantiTurn();
                                player[turnAttack[turn_ke].ke].hero_stress.stress_point += 20;
                                MessageBox.Show(player[turnAttack[turn_ke].ke].nama + " terkena Stun " + player[turnAttack[turn_ke].ke].hero_buff_turn);
                            }
                            if (player[turnAttack[turn_ke].ke].hero_buff_turn == -3 || (player[turnAttack[turn_ke].ke].hero_stress.stress_level == stress_stage.depresi && player[pilihHero].hero_stress.stress_point >= 100))
                            {
                                player[pilihHero].hero_buff_turn = 0;
                                player[pilihHero].hero_buff = efek.none;

                                for (int j = 0; j < player.Count; j++)
                                {
                                    if (j != pilihHero)
                                    {
                                        player[j].hero_stress.stress_point += 20;
                                    }
                                }
                            }
                            else
                            {
                                if (player[pilihHero].hero_stress.stress_point >= 100)
                                {
                                    player[pilihHero].hero_stress.stress_level = stress_stage.depresi;
                                    player[pilihHero].hero_stress.stress_point = 0;
                                    for (int j = 0; j < player.Count; j++)
                                    {
                                        if (j != pilihHero)
                                        {
                                            player[j].hero_stress.stress_point += 10;
                                        }
                                    }
                                }
                            }

                        
                    }
                    else
                    {
                        gerak_attack = true;
                        delay_aktif = true;
                        pilihMusuh = turnAttack[turn_ke].ke;
                        musuh[pilihMusuh].marked = false;
                        for (int i = 0; i < musuh[pilihMusuh].musuh_buff.Count; i++)
                        {
                            if (musuh[pilihMusuh].musuh_buff[i] == efek.bleed)
                            {
                                musuh[pilihMusuh].hp -= musuh[pilihMusuh].hp / 10;
                            }
                            if (musuh[pilihMusuh].musuh_buff[i] == efek.blight)
                            {
                                musuh[pilihMusuh].hp -= (musuh[pilihMusuh].maxHp / 10);
                            }
                            if (musuh[pilihMusuh].musuh_buff[i] == efek.marked)
                            {
                                musuh[pilihMusuh].marked = true;
                            }
                            if (musuh[pilihMusuh].musuh_buff[i] == efek.stun)
                            {
                                gantiTurn();
                            }
                            musuh[pilihMusuh].musuh_buff_turn[i]--;
                            if (musuh[pilihMusuh].musuh_buff_turn[i] == -3)
                            {
                                musuh[pilihMusuh].musuh_buff_turn.RemoveAt(i);
                                musuh[pilihMusuh].musuh_buff.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
                else
                {

                    MessageBox.Show("diem1");
                }
            }
            catch (Exception)
            {
                try
                {
                    if (gerak_attack == false)
                    {
                        turn_ke = 0;
                        if (turnAttack[turn_ke].jenis == 1)
                        {
                            gerak_attack = true;
                            pilihHero = turnAttack[turn_ke].ke;

                            player[turnAttack[turn_ke].ke].hero_buff_turn--;
                            if (player[turnAttack[turn_ke].ke].hero_buff == efek.bleed)
                            {
                                player[turnAttack[turn_ke].ke].hp -= player[pilihHero].hp / 10;
                                player[turnAttack[turn_ke].ke].hero_stress.stress_point += 10;
                                MessageBox.Show(player[pilihHero].nama + " terkena Bleed " + player[turnAttack[turn_ke].ke].hero_buff_turn);
                            }
                            if (player[turnAttack[turn_ke].ke].hero_buff == efek.blight)
                            {
                                player[turnAttack[turn_ke].ke].hp -= (player[pilihHero].maxHp / 10);
                                player[turnAttack[turn_ke].ke].hero_stress.stress_point += 10;
                                MessageBox.Show(player[pilihHero].nama + " terkena Blight " + player[turnAttack[turn_ke].ke].hero_buff_turn);
                            }
                            if (player[turnAttack[turn_ke].ke].hero_buff == efek.stun)
                            {
                                gantiTurn();
                                player[turnAttack[turn_ke].ke].hero_stress.stress_point += 20;
                                MessageBox.Show(player[turnAttack[turn_ke].ke].nama + " terkena Stun " + player[turnAttack[turn_ke].ke].hero_buff_turn);
                            }
                            if (player[turnAttack[turn_ke].ke].hero_buff_turn == -3 || (player[turnAttack[turn_ke].ke].hero_stress.stress_level == stress_stage.depresi && player[pilihHero].hero_stress.stress_point >= 100))
                            {
                                player[pilihHero].hero_buff_turn = 0;
                                player[pilihHero].hero_buff = efek.none;

                                for (int j = 0; j < player.Count; j++)
                                {
                                    if (j != pilihHero)
                                    {
                                        player[j].hero_stress.stress_point += 20;
                                    }
                                }
                            }
                            else
                            {
                                if (player[pilihHero].hero_stress.stress_point >= 100)
                                {
                                    player[pilihHero].hero_stress.stress_level = stress_stage.depresi;
                                    player[pilihHero].hero_stress.stress_point = 0;
                                    for (int j = 0; j < player.Count; j++)
                                    {
                                        if (j != pilihHero)
                                        {
                                            player[j].hero_stress.stress_point += 10;
                                        }
                                    }
                                }
                            }


                        }
                        else
                        {
                            gerak_attack = true;
                            MessageBox.Show("musuh2");
                            delay_aktif = true;
                            pilihMusuh = turnAttack[turn_ke].ke;
                            musuh[pilihMusuh].marked = false;
                            for (int i = 0; i < musuh[pilihMusuh].musuh_buff.Count; i++)
                            {
                                if (musuh[pilihMusuh].musuh_buff[i] == efek.bleed)
                                {
                                    musuh[pilihMusuh].hp -= musuh[pilihMusuh].hp / 10;
                                }
                                if (musuh[pilihMusuh].musuh_buff[i] == efek.blight)
                                {
                                    musuh[pilihMusuh].hp -= (musuh[pilihMusuh].maxHp / 10);
                                }
                                if (musuh[pilihMusuh].musuh_buff[i] == efek.marked)
                                {
                                    musuh[pilihMusuh].marked = true;
                                }
                                if (musuh[pilihMusuh].musuh_buff[i] == efek.stun)
                                {
                                    gantiTurn();
                                }
                                musuh[pilihMusuh].musuh_buff_turn[i]--;
                                if (musuh[pilihMusuh].musuh_buff_turn[i] == -3)
                                {
                                    musuh[pilihMusuh].musuh_buff_turn.RemoveAt(i);
                                    musuh[pilihMusuh].musuh_buff.RemoveAt(i);
                                    i--;
                                }
                            }
                        }
                    }
                    else
                    {
                        pilihHero = turnAttack[0].ke;
                        gerak_attack = false;
                        MessageBox.Show("diem2");
                    }
                }
                catch (Exception)
                {

                }
                
            }
        }
    }
    
    class turn
    {
        public int jenis { get; set; }
        public int ke { get; set; }
        public int speed { get; set; }
        public turn(int jenis, int ke,int speed)
        {
            this.jenis = jenis;
            this.ke = ke;
            this.speed = speed;
        }
    }
}
