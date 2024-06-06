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
	public class PackPurple : BagPurple {
		new public static IBagModItem Instance {
			get {
				if (instance == null)
					instance = new PackPurple();

				return instance;
			}
		}
		private static IBagModItem instance;
		public override int BagStorageID { get => BagPurple.Instance.BagStorageID; set => BagPurple.Instance.BagStorageID = value; }
		public override int GetBagType() => ModContent.ItemType<PackPurple>();
		public override int MyTileType => ModContent.TileType<Tiles.PackPurple>();
		public override void AddRecipes() {
			base.AddRecipes();

			CreateRecipe()
				.AddTile(TileID.Loom)
				.AddIngredient(ModContent.ItemType<BagPurple>())
				.Register();

			Recipe.Create(ModContent.ItemType<BagPurple>())
				.AddIngredient(Type)
				.AddTile(TileID.Loom)
				.Register();
		}
	}
}