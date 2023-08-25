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
	public class PackYellow : BagYellow {
		public override int MyTileType => ModContent.TileType<Tiles.PackYellow>();
		public static void RegisterWithAndroLibItemTypeOnly() {
			StorageManager.RegisterVacuumStorageClassItemTypeOnly(() => ModContent.ItemType<PackYellow>(), BagStorageID);
		}
		public override void AddRecipes() {
			base.AddRecipes();

			CreateRecipe()
				.AddTile(TileID.Loom)
				.AddIngredient(ModContent.ItemType<BagYellow>())
				.Register();

			Recipe.Create(ModContent.ItemType<BagYellow>())
				.AddIngredient(Type)
				.AddTile(TileID.Loom)
				.Register();
		}
	}
}