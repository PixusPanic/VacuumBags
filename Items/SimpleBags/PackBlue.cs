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
	public class PackBlue : BagBlue {
		new public static IBagModItem Instance {
			get {
				if (instance == null)
					instance = new PackBlue();

				return instance;
			}
		}
		private static IBagModItem instance;
		public override int BagStorageID { get => BagBlue.Instance.BagStorageID; set => BagBlue.Instance.BagStorageID = value; }
		public override int GetBagType() => ModContent.ItemType<PackBlue>();
		public override int MyTileType => ModContent.TileType<Tiles.PackBlue>();
		public override void AddRecipes() {
			base.AddRecipes();

			CreateRecipe()
				.AddTile(TileID.Loom)
				.AddIngredient(ModContent.ItemType<BagBlue>())
				.Register();

			Recipe.Create(ModContent.ItemType<BagBlue>())
				.AddIngredient(Type)
				.AddTile(TileID.Loom)
				.Register();
		}
	}
}