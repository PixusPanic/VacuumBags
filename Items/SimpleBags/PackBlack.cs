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
	public class PackBlack : BagBlack {
		new public static IBagModItem Instance {
			get {
				if (instance == null)
					instance = new PackBlack();

				return instance;
			}
		}
		private static IBagModItem instance;
		public override int BagStorageID { get => BagBlack.Instance.BagStorageID; set => BagBlack.Instance.BagStorageID = value; }
		public override int GetBagType() => ModContent.ItemType<PackBlack>();
		public override int MyTileType => ModContent.TileType<Tiles.PackBlack>();
		public override void AddRecipes() {
			base.AddRecipes();

			CreateRecipe()
				.AddTile(TileID.Loom)
				.AddIngredient(ModContent.ItemType<BagBlack>())
				.Register();

			Recipe.Create(ModContent.ItemType<BagBlack>())
				.AddIngredient(Type)
				.AddTile(TileID.Loom)
				.Register();
		}
		
	}
}