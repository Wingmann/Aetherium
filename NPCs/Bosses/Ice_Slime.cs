﻿using Aetherium.Items.Armor;
using Aetherium.Items.Consumables;
using Aetherium.Items.Crafting;
using Aetherium.Items.Weapons;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aetherium.NPCs.Bosses
{
    public class Ice_Slime : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Icy Elemental Slime");
            Main.npcFrameCount[npc.type] = 2;
        }

        public override void SetDefaults()
        {
            npc.width = 120;
            npc.height = 76;
            npc.damage = 11;
            npc.defense = 6;
            npc.knockBackResist = 0f;
            npc.lifeMax = 1400;
            npc.npcSlots = 10f;
            npc.HitSound = new Terraria.Audio.LegacySoundStyle(SoundID.NPCHit4.SoundId, 1);
            npc.DeathSound = new Terraria.Audio.LegacySoundStyle(SoundID.NPCDeath4.SoundId, 1);
            npc.value = 600f;
            npc.aiStyle = -1;
            banner = 0;
            npc.boss = true;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return 0f;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.5f;
            return null;
        }

        private const int jumpDelay = 200;
        public override void AI()
        {
            npc.TargetClosest();
            npc.ai[1]++;
            if (npc.velocity.Y == 0)
            {
                if(Main.player[npc.target].position.Y + Main.player[npc.target].height > npc.position.Y + npc.height)
                {
                    if(!Collision.SolidTiles(npc.position.ToTileCoordinates().X, (int)(npc.position.ToTileCoordinates().X + npc.width/16f), (int)(npc.position.ToTileCoordinates().Y + npc.height/16f)+1, (int)(npc.position.ToTileCoordinates().Y + npc.height / 16f)+1))
                    {
                        npc.noTileCollide = true;
                    }
                }
                npc.velocity.X *= 0.7f; 
                if(npc.ai[0]==jumpDelay)
                {
                    npc.ai[3]++;
                    npc.velocity = npc.ai[3] % 3 == 0 ? new Microsoft.Xna.Framework.Vector2(npc.direction * 5, -16) : new Microsoft.Xna.Framework.Vector2(npc.direction * 5, -11);
                    npc.netUpdate = true;
                }
                else if(npc.ai[0]== 85 || npc.ai[0] == 170)
                {
                    Main.PlaySound(new LegacySoundStyle(2, 28), npc.Center);
                    int proj = Projectile.NewProjectile(npc.Center, npc.DirectionTo(Main.player[npc.target].Center) * 5, ProjectileID.Blizzard, 20, 2);
                    Main.projectile[proj].friendly = false;
                    Main.projectile[proj].hostile = true;
                    Main.projectile[proj].tileCollide = false;
                }
                npc.ai[0]++;
                npc.ai[2] = 0;
            }
            else
            {
                npc.noTileCollide = false;
                npc.ai[0] = 0;
                if(npc.ai[2] > 5)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        Dust.NewDust(npc.position, npc.width, npc.height, 20);
                    }
                    int proj = Projectile.NewProjectile(npc.position + new Microsoft.Xna.Framework.Vector2((float)Math.Sin(npc.velocity.Y) * npc.width / 2f + npc.width / 2f, npc.height), new Microsoft.Xna.Framework.Vector2(0, 15), ProjectileID.Blizzard, 20, 2);
                    Main.projectile[proj].friendly = false;
                    Main.projectile[proj].hostile = true;
                    npc.ai[2] = 0;
                }
                else
                {
                    npc.ai[2]++;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (npc.velocity.Y != 0)
            {
                npc.frame.Y = 1 * frameHeight;
            }
            else if (npc.ai[0]> jumpDelay - 30)
            {
                npc.frame.Y = (int)(npc.ai[1] / 2f % 2) * frameHeight;
            }
            else if (npc.ai[0] > jumpDelay - 60)
            {
                npc.frame.Y = (int)(npc.ai[1] / 4f % 2) * frameHeight;
            }
            else
            {
                npc.frame.Y = (int)(npc.ai[1] / 45f % 2) * frameHeight;
            }
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.625f * bossLifeScale);
            npc.damage = (int)(npc.damage * 0.6f);
        }
        public override void NPCLoot()
        {
            if(!NPC.AnyNPCs(ModContent.NPCType<Fire_Slime>()) && !NPC.AnyNPCs(ModContent.NPCType<Desert_Slime>()) && !NPC.AnyNPCs(ModContent.NPCType<Earth_Slime>()))
            {
                if(Main.expertMode)
                {
                    npc.DropItemInstanced(npc.position, new Vector2(npc.width, npc.height), ModContent.ItemType<Elemental_Slimes_Bag>(), 1, true);
                }
                else
                {
                    int[] items = { ModContent.ItemType<Desert_Rose>(), ModContent.ItemType<Ice_Staff>(), ModContent.ItemType<Molten_Edge>(), ModContent.ItemType<Bee_Swarm_Staff>() };
                    int choice1 = Main.rand.Next(items);
                    Item.NewItem(npc.getRect(), choice1);
                    int choice2 = Main.rand.Next(items);
                    while (choice2 == choice1)
                    {
                        choice2 = Main.rand.Next(items);
                    }
                    Item.NewItem(npc.getRect(), choice2);
                }
                if(!AetheriumWorld.downedElementalSlimes)
                {
                    AetheriumWorld.downedElementalSlimes = true;
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendData(MessageID.WorldData); // Immediately inform clients of new world state.
                    }
                }
            }
            for (int i = 0; i < 7; i++)
            {
                int dustType = 20;
                int dustIndex = Dust.NewDust(npc.position, npc.width, npc.height, dustType);
                Dust dust = Main.dust[dustIndex];
                dust.velocity.X = dust.velocity.X + Main.rand.Next(-50, 51) * 0.01f;
                dust.velocity.Y = dust.velocity.Y + Main.rand.Next(-50, 51) * 0.01f;
                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
            }
            Item.NewItem(npc.getRect(), ItemID.Gel, Main.rand.Next(1, 4));
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int i = 0; i < 4; i++)
            {
                int dustType = 20;
                int dustIndex = Dust.NewDust(npc.position, npc.width, npc.height, dustType);
                Dust dust = Main.dust[dustIndex];
                dust.velocity.X = dust.velocity.X + Main.rand.Next(-50, 51) * 0.01f;
                dust.velocity.Y = dust.velocity.Y + Main.rand.Next(-50, 51) * 0.01f;
                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
            }
        }
    }
}
