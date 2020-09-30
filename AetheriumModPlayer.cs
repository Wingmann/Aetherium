using Aetherium.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aetherium
{
    public class AetheriumModPlayer : ModPlayer
    {
        public bool aetherWisp;
        public bool furyOfTheStorm;
        public bool manaLeech;
        public bool bonePlating;
        public bool arcaneComet;
        public bool wickedScythe;
        public bool prescenceOfMind;
        public bool harumachiClover;

        int furyOfTheStormTimer, furyOfTheStormDamage, furyOfTheStormProcs, furyOfTheStormCooldown, furyOfTheStormLastEnemyType = 0;
        int bonePlatingProcs, bonePlatingTimer, bonePlatingCooldown = 0;
        int arcaneCometCooldown = 0;
        int prescenceOfMindTimer, prescenceOfMindStacks = 0;


        public override void ResetEffects()
        {
            aetherWisp = false;
            furyOfTheStorm = false;
            manaLeech = false;
            bonePlating = false;
            arcaneComet = false;
            wickedScythe = false;
            prescenceOfMind = false;
            harumachiClover = false;
        }

        public override void SetupStartInventory(IList<Item> items)
        {
            Item wisp = new Item();
            wisp.SetDefaults(mod.ItemType("Aether_Wisp"));
            wisp.stack = 1;
            items.Add(wisp);
        }

        public override void PostUpdateEquips()
        {
            if(furyOfTheStorm)
            {
                if(furyOfTheStormCooldown != 0)
                {
                    furyOfTheStormCooldown--;
                }
                else if(furyOfTheStormProcs > 0)
                {
                    furyOfTheStormTimer++;
                }
                else
                {
                    furyOfTheStormTimer = 0;
                }
                if(furyOfTheStormTimer>90)
                {
                    furyOfTheStormProcs = 0;
                    furyOfTheStormTimer = 0;
                    furyOfTheStormDamage = 0;
                }
            }
            else
            {
                furyOfTheStormTimer = 0;
                furyOfTheStormDamage = 0;
                furyOfTheStormProcs = 0;
                furyOfTheStormCooldown = 0;
            }
            if(bonePlating)
            {
                if(bonePlatingCooldown==0) // Not on cooldown
                {
                    if (bonePlatingProcs > 0 && bonePlatingTimer != 0)
                    {
                        bonePlatingTimer--;
                        if (Main.rand.Next(6) == 0)
                        {
                            Dust.NewDust(player.position, player.width, player.height, DustID.GrassBlades);
                        }
                        if (Main.rand.Next(6) == 0)
                        {
                            Dust.NewDust(player.position, player.width, player.height, 31);
                        }
                    }
                    if (bonePlatingTimer == 0 && bonePlatingProcs > 0)
                    {
                        bonePlatingProcs = 0;
                        bonePlatingCooldown = 720;
                        bonePlatingProcs = 0;
                    }
                    else
                    {
                        if(Main.rand.Next(90)==0)
                        {
                            Dust.NewDust(player.position, player.width, player.height, 31);
                        }
                        if (Main.rand.Next(45) == 0)
                        {
                            Dust.NewDust(player.position, player.width, player.height, DustID.GrassBlades);
                        }
                    }
                }
                else if(bonePlatingCooldown != 0)
                {
                    bonePlatingCooldown--;
                }
            }
            if(arcaneComet)
            {
                if(arcaneCometCooldown!= 0)
                {
                    arcaneCometCooldown--;
                }
            }
            if(prescenceOfMind)
            {
                if(prescenceOfMindTimer != 0)
                {
                    player.statManaMax2 += 20 * prescenceOfMindStacks;
                    prescenceOfMindTimer--;
                }
                if(prescenceOfMindTimer == 0)
                {
                    prescenceOfMindStacks = 0;
                }
            }
        }

        public override void PostUpdate()
        {

        }

        public override void PreUpdateMovement()
        {

        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (furyOfTheStorm)
            {
                if (furyOfTheStormProcs >= 2 && furyOfTheStormCooldown == 0)
                {
                    if (furyOfTheStormTimer < 90 && furyOfTheStormLastEnemyType == target.type)
                    {
                        furyOfTheStormDamage += damage;
                        target.StrikeNPC(furyOfTheStormDamage + damage, 0, proj.direction); // Electrocute the enemy
                        for (int i = 0; i < 10; i++)
                        {
                            Dust.NewDust(target.position, target.width, target.height, DustID.Electric);
                        }
                        Main.PlaySound(SoundID.Item94, target.position);
                        furyOfTheStormTimer = 0;
                        furyOfTheStormDamage = 0;
                        furyOfTheStormProcs = 0;
                        furyOfTheStormCooldown = 300;
                    }
                    else
                    {
                        furyOfTheStormTimer = 0;
                        furyOfTheStormDamage = 0;
                        furyOfTheStormProcs = 0;
                    }
                    furyOfTheStormLastEnemyType = target.type;
                }
                else if (furyOfTheStormCooldown == 0)
                {
                    if (furyOfTheStormTimer < 90)
                    {
                        if (furyOfTheStormProcs > 0 && furyOfTheStormLastEnemyType != target.type)
                        {
                            furyOfTheStormTimer = 0;
                            furyOfTheStormDamage = 0;
                            furyOfTheStormProcs = 0;
                        }
                        else
                        {
                            furyOfTheStormProcs++;
                            furyOfTheStormDamage += damage;
                        }
                        furyOfTheStormLastEnemyType = target.type;
                    }
                }
            }
            if (manaLeech && target.life < 1 && target.lifeMax > 1 && target.damage > 1)
            {
                player.statMana += (int)((player.statManaMax - player.statMana) * 0.4f);
                player.ManaEffect((int)((player.statManaMax - player.statMana) * 0.4f));
            }
            if (arcaneComet)
            {
                if(arcaneCometCooldown==0)
                {
                    Vector2 dir = player.DirectionTo(proj.position);
                    dir.Normalize();
                    Projectile.NewProjectile(player.position, dir * 200f, ModContent.ProjectileType<Arcane_Comet>(), (int)(damage * 1.1f), 0, player.whoAmI);
                    Main.PlaySound(new Terraria.Audio.LegacySoundStyle(2, 28), player.position);
                    arcaneCometCooldown = 240;
                }
            }
            if(wickedScythe)
            {
                if(target.life < target.lifeMax*0.5 && (int)(damage*0.15f) > 0)
                {
                    target.StrikeNPC((int)(damage * 0.15f), 0, proj.direction);
                }
            }
            if (prescenceOfMind && target.life < 1 && target.lifeMax > 1 && target.damage > 1 && prescenceOfMindStacks < 6)
            {
                prescenceOfMindTimer = 600;
                prescenceOfMindStacks++;
            }
            if (harumachiClover && target.life < 1 && target.lifeMax > 1 && target.damage > 1 && proj.type != ModContent.ProjectileType<Sakura_Petal>())
            {
                for(double a = 0; a < Math.PI*2; a += Math.PI/6f)
                {
                    Projectile.NewProjectile(target.position, new Vector2((float)Math.Cos(a), (float)Math.Sin(a)) * 50f, ModContent.ProjectileType<Sakura_Petal>(), (int)(damage*0.30f), 0, player.whoAmI);
                }
            }
        }

        
        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            if (furyOfTheStorm)
            {
                if (furyOfTheStormProcs >= 2 && furyOfTheStormCooldown == 0)
                {
                    if (furyOfTheStormTimer < 90 && furyOfTheStormLastEnemyType == target.type)
                    {
                        furyOfTheStormDamage += damage;
                        target.StrikeNPC(furyOfTheStormDamage + damage, 0, item.direction); // Electrocute the enemy
                        for (int i = 0; i < 10; i++)
                        {
                            Dust.NewDust(target.position, target.width, target.height, DustID.Electric);
                        }
                        Main.PlaySound(SoundID.Item94, target.position);
                        furyOfTheStormTimer = 0;
                        furyOfTheStormDamage = 0;
                        furyOfTheStormProcs = 0;
                        furyOfTheStormCooldown = 300;
                    }
                    else
                    {
                        furyOfTheStormTimer = 0;
                        furyOfTheStormDamage = 0;
                        furyOfTheStormProcs = 0;
                    }
                    furyOfTheStormLastEnemyType = target.type;
                }
                else if (furyOfTheStormCooldown == 0)
                {
                    if (furyOfTheStormTimer < 90)
                    {
                        if (furyOfTheStormProcs > 0 && furyOfTheStormLastEnemyType != target.type)
                        {
                            furyOfTheStormTimer = 0;
                            furyOfTheStormDamage = 0;
                            furyOfTheStormProcs = 0;
                        }
                        else
                        {
                            furyOfTheStormProcs++;
                            furyOfTheStormDamage += damage;
                        }
                        furyOfTheStormLastEnemyType = target.type;
                    }
                }
            }
            if (manaLeech && target.life < 1 && target.lifeMax > 1 && target.damage > 1)
            {
                player.statMana += (int)((player.statManaMax - player.statMana) * 0.4f);
                player.ManaEffect((int)((player.statManaMax - player.statMana) * 0.4f));
            }
            if (wickedScythe)
            {
                if (target.life < target.lifeMax * 0.5 && (int)(damage * 0.15f) > 0)
                {
                    target.StrikeNPC((int)(damage * 0.15f), 0, item.direction);
                }
            }
            if (prescenceOfMind && target.life < 1 && target.lifeMax > 1 && target.damage > 1 && prescenceOfMindStacks < 5)
            {
                prescenceOfMindTimer = 600;
                prescenceOfMindStacks++;
            }
        }

        public override void Hurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit)
        { 
            if(bonePlatingProcs>0)
            {
                bonePlatingProcs--;
                player.statLife += (int)(damage * 0.4f);
                //player.HealEffect((int)(damage * 0.4f));
                if (Main.myPlayer == player.whoAmI)
                {
                    Main.PlaySound(new Terraria.Audio.LegacySoundStyle(3,2), player.position);
                }
                if(bonePlatingProcs<1)
                {
                    bonePlatingCooldown = 720;
                    bonePlatingTimer = 0;
                }
            }
            else if (bonePlatingCooldown == 0 && bonePlatingProcs == 0)
            {
                bonePlatingProcs = 3;
                bonePlatingTimer = 180;
            }
        }

        public override void OnHitByNPC(NPC npc, int damage, bool crit)
        {

        }

        public override void OnConsumeAmmo(Item weapon, Item ammo)
        {

        }
    }
}