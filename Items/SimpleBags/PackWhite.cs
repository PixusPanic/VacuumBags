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
	public class PackWhite : BagWhite {
		new public static BagModItem Instance {
			get {
				if (instance == null)
					instance = new PackWhite();

				return instance;
			}
		}
		private static BagModItem instance;
		public override int BagStorageID { get => BagWhite.Instance.BagStorageID; set => BagWhite.Instance.BagStorageID = value; }
		public override int GetBagType() => ModContent.ItemType<PackWhite>();
		public override int MyTileType => ModContent.TileType<Tiles.PackWhite>();
		public override void AddRecipes() {
			base.AddRecipes();

			CreateRecipe()
				.AddTile(TileID.Loom)
				.AddIngredient(ModContent.ItemType<BagWhite>())
				.Register();

			Recipe.Create(ModContent.ItemType<BagWhite>())
				.AddIngredient(Type)
				.AddTile(TileID.Loom)
				.Register();
		}
	}
}