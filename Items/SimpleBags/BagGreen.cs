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
	public class BagGreen : SimpleBag {
		public static BagModItem Instance {
			get {
				if (instance == null)
					instance = new BagGreen();

				return instance;
			}
		}
		private static BagModItem instance;
		public override int GetBagType() => ModContent.ItemType<BagGreen>();
		public override int MyTileType => ModContent.TileType<Tiles.BagGreen>();
		protected override SortedSet<int> GetDefaultBlacklist() {
			return new() {
				ModContent.ItemType<BagGreen>(),
				ModContent.ItemType<PackGreen>(),
			};
		}

		public override Color PanelColor => new Color(10, 80, 10, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ScrollBarColor => new Color(10, 90, 10, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ButtonHoverColor => new Color(0, 120, 0, androLib.Common.Configs.ConfigValues.UIAlpha);

		public override void AddRecipes() {
			if (!VacuumBags.serverConfig.HarderBagRecipes) {
				CreateRecipe()
				.AddTile(TileID.WorkBenches)
				.AddIngredient(ItemID.Silk, 2)
				.AddIngredient(ItemID.GreenMushroom, 1)
				.AddIngredient(ItemID.WhiteString, 1)
				.Register();
			}
			else {
				CreateRecipe()
				.AddTile(TileID.WorkBenches)
				.AddIngredient(ItemID.Silk, 10)
				.AddIngredient(ItemID.GreenMushroom, 1)
				.AddIngredient(ItemID.GreenThread, 1)
				.AddIngredient(ItemID.Seaweed, 2)
				.AddIngredient(ItemID.JungleSpores, 5)
				.Register();
			}
		}
	}
}