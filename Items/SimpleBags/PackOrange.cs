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
	public class PackOrange : BagOrange {
		public override int MyTileType => ModContent.TileType<Tiles.PackOrange>();
		public static void RegisterWithAndroLibItemTypeOnly() {
			StorageManager.RegisterVacuumStorageClassItemTypeOnly(() => ModContent.ItemType<PackOrange>(), BagStorageID);
		}
		public override void AddRecipes() {
			base.AddRecipes();

			CreateRecipe()
				.AddTile(TileID.Loom)
				.AddIngredient(ModContent.ItemType<BagOrange>())
				.Register();

			Recipe.Create(ModContent.ItemType<BagOrange>())
				.AddIngredient(Type)
				.AddTile(TileID.Loom)
				.Register();
		}
	}
}