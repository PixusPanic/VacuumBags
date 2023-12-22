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
	public class PackOrange : BagOrange {
		new public static BagModItem Instance {
			get {
				if (instance == null)
					instance = new PackOrange();

				return instance;
			}
		}
		private static BagModItem instance;
		public override int BagStorageID { get => BagOrange.Instance.BagStorageID; set => BagOrange.Instance.BagStorageID = value; }
		public override int GetBagType() => ModContent.ItemType<PackOrange>();
		public override int MyTileType => ModContent.TileType<Tiles.PackOrange>();
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