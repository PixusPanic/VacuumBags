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
	public class BagPink : SimpleBag {
		public static IBagModItem Instance {
			get {
				if (instance == null)
					instance = new BagPink();

				return instance;
			}
		}
		private static IBagModItem instance;
		public override int GetBagType() => ModContent.ItemType<BagPink>();
		public override int MyTileType => ModContent.TileType<Tiles.BagPink>();
		protected override SortedSet<int> GetDefaultBlacklist() {
			return new() {
				ModContent.ItemType<BagPink>(),
				ModContent.ItemType<PackPink>(),
			};
		}

		public override Color PanelColor => new Color(255, 192, 203, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ScrollBarColor => new Color(255, 182, 193, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ButtonHoverColor => new Color(255, 105, 180, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override void AddRecipes() {
			if (!VacuumBags.serverConfig.HarderBagRecipes) {
				CreateRecipe()
				.AddTile(TileID.WorkBenches)
				.AddIngredient(ItemID.Silk, 2)
				.AddIngredient(ItemID.PinkPricklyPear, 1)
				.AddIngredient(ItemID.WhiteString, 1)
				.Register();
			}
			else {
				CreateRecipe()
				.AddTile(TileID.WorkBenches)
				.AddIngredient(ItemID.Silk, 10)
				.AddIngredient(ItemID.PinkPricklyPear, 3)
				.AddIngredient(ItemID.PinkString, 1)
				.AddIngredient(ItemID.PinkGel, 100)
				.AddIngredient(ItemID.ArmoredCavefish, 5)
				.AddIngredient(ItemID.Salmon, 5)
				.Register();
			}
		}
	}
}