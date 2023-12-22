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
	[Autoload(false)]
	public class PackBrown : BagBrown {
		public static BagModItem Instance {
			get {
				if (instance == null)
					instance = new PackBrown();

				return instance;
			}
		}
		private static BagModItem instance;
		public override int BagStorageID { get => BagBrown.Instance.BagStorageID; set => BagBrown.Instance.BagStorageID = value; }
		public override int GetBagType() => ModContent.ItemType<PackBrown>();
		public override int MyTileType => ModContent.TileType<Tiles.PackBrown>();
		public override void AddRecipes() {
			base.AddRecipes();

			CreateRecipe()
				.AddTile(TileID.Loom)
				.AddIngredient(ModContent.ItemType<BagBrown>())
				.Register();

			Recipe.Create(ModContent.ItemType<BagBrown>())
				.AddIngredient(Type)
				.AddTile(TileID.Loom)
				.Register();
		}
	}
}