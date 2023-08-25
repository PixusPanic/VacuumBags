using androLib.Common.Utility;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;
using Microsoft.Xna.Framework;
using androLib.Items;
using androLib.Common.Globals;
using androLib;
using System;

namespace VacuumBags.Items
{
	public class PackGreen : BagGreen {
		public override int MyTileType => ModContent.TileType<Tiles.PackGreen>();
		public static void RegisterWithAndroLibItemTypeOnly() {
			StorageManager.RegisterVacuumStorageClassItemTypeOnly(() => ModContent.ItemType<PackGreen>(), BagStorageID);
		}
		public override void AddRecipes() {
			base.AddRecipes();

			CreateRecipe()
				.AddTile(TileID.Loom)
				.AddIngredient(ModContent.ItemType<BagGreen>())
				.Register();

			Recipe.Create(ModContent.ItemType<BagGreen>())
				.AddIngredient(Type)
				.AddTile(TileID.Loom)
				.Register();
		}
	}
}