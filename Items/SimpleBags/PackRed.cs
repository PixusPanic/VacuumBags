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
	public class PackRed : BagRed {
		new public static IBagModItem Instance {
			get {
				if (instance == null)
					instance = new PackRed();

				return instance;
			}
		}
		private static IBagModItem instance;
		public override int BagStorageID { get => BagRed.Instance.BagStorageID; set => BagRed.Instance.BagStorageID = value; }
		public override int GetBagType() => ModContent.ItemType<PackRed>();
		public override int MyTileType => ModContent.TileType<Tiles.PackRed>();
		public override void AddRecipes() {
			base.AddRecipes();

			CreateRecipe()
				.AddTile(TileID.Loom)
				.AddIngredient(ModContent.ItemType<BagRed>())
				.Register();

			Recipe.Create(ModContent.ItemType<BagRed>())
				.AddIngredient(Type)
				.AddTile(TileID.Loom)
				.Register();
		}
	}
}