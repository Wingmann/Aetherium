﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Aetherium.Items.Weapons
{
    public class Aetherium_Railgun : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aetherium Railgun");
            Tooltip.SetDefault("Converts ammo into high-velocity piercing bullets\nHas a high critical strike chance");
        }

        public override void SetDefaults()
        {
            item.damage = 140;
            item.width = 70;
            item.height = 22;
            item.useTime = 40;
            item.useAnimation = 40;
            item.useStyle = 5;
            item.noMelee = true;
            item.crit = 40;
            item.knockBack = 5;
            item.value = 10000;
            item.rare = 9;
            item.UseSound = new Terraria.Audio.LegacySoundStyle(2, 40);
            item.autoReuse = true;
            item.shoot = 10;
            item.shootSpeed = 40f;
            item.ammo = AmmoID.Bullet;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
{
        type = mod.ProjectileType<Projectiles.Rail>(); // or ProjectileID.FireArrow;

    return true; // return true to allow tmodloader to call Projectile.NewProjectile as normal
}

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod.ItemType<Crafting.True_Aetherium_Bar>(),12);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
