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
	public class PackPink : BagPink {
		new public static IBagModItem Instance {
			get {
				if (instance == null)
					instance = new PackPink();

				return instance;
			}
		}
		private static IBagModItem instance;
		public override int BagStorageID { get => BagPink.Instance.BagStorageID; set => BagPink.Instance.BagStorageID = value; }
		public override int GetBagType() => ModContent.ItemType<PackPink>();
		public override int MyTileType => ModContent.TileType<Tiles.PackPink>();
		public override void AddRecipes() {
			base.AddRecipes();

			CreateRecipe()
				.AddTile(TileID.Loom)
				.AddIngredient(ModContent.ItemType<BagPink>())
				.Register();

			Recipe.Create(ModContent.ItemType<BagPink>())
				.AddIngredient(Type)
				.AddTile(TileID.Loom)
				.Register();
		}
	}
}