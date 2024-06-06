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
	public class PackGreen : BagGreen {
		new public static IBagModItem Instance {
			get {
				if (instance == null)
					instance = new PackGreen();

				return instance;
			}
		}
		private static IBagModItem instance;
		public override int BagStorageID { get => BagGreen.Instance.BagStorageID; set => BagGreen.Instance.BagStorageID = value; }
		public override int GetBagType() => ModContent.ItemType<PackGreen>();
		public override int MyTileType => ModContent.TileType<Tiles.PackGreen>();
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