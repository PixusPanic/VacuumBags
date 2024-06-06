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
	public class PackYellow : BagYellow {
		new public static IBagModItem Instance {
			get {
				if (instance == null)
					instance = new PackYellow();

				return instance;
			}
		}
		private static IBagModItem instance;
		public override int BagStorageID { get => BagYellow.Instance.BagStorageID; set => BagYellow.Instance.BagStorageID = value; }
		public override int GetBagType() => ModContent.ItemType<PackYellow>();
		public override int MyTileType => ModContent.TileType<Tiles.PackYellow>();
		public override void AddRecipes() {
			base.AddRecipes();

			CreateRecipe()
				.AddTile(TileID.Loom)
				.AddIngredient(ModContent.ItemType<BagYellow>())
				.Register();

			Recipe.Create(ModContent.ItemType<BagYellow>())
				.AddIngredient(Type)
				.AddTile(TileID.Loom)
				.Register();
		}
	}
}