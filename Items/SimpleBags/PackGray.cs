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
	public class PackGray : BagGray {
		new public static BagModItem Instance {
			get {
				if (instance == null)
					instance = new PackGray();

				return instance;
			}
		}
		private static BagModItem instance;
		public override int BagStorageID { get => BagGray.Instance.BagStorageID; set => BagGray.Instance.BagStorageID = value; }
		public override int GetBagType() => ModContent.ItemType<PackGray>();
		public override int MyTileType => ModContent.TileType<Tiles.PackGray>();
		public override void AddRecipes() {
			base.AddRecipes();

			CreateRecipe()
				.AddTile(TileID.Loom)
				.AddIngredient(ModContent.ItemType<BagGray>())
				.Register();

			Recipe.Create(ModContent.ItemType<BagGray>())
				.AddIngredient(Type)
				.AddTile(TileID.Loom)
				.Register();
		}
	}
}