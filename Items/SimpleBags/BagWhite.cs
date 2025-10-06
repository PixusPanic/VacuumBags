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
using VacuumBags.Common.Configs;

namespace VacuumBags.Items
{
	[Autoload(false)]
	public class BagWhite : SimpleBag {
		public static IBagModItem Instance {
			get {
				if (instance == null)
					instance = new BagWhite();

				return instance;
			}
		}
		private static IBagModItem instance;
		public override int GetBagType() => ModContent.ItemType<BagWhite>();
		public override int MyTileType => ModContent.TileType<Tiles.BagWhite>();
		protected override SortedSet<int> GetDefaultBlacklist() {
			return new() {
				ModContent.ItemType<BagWhite>(),
				ModContent.ItemType<PackWhite>(),
			};
		}

		public override Color PanelColor => new Color(255, 255, 255, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ScrollBarColor => new Color(245, 245, 245, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ButtonHoverColor => new Color(240, 240, 240, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override void AddRecipes() {
			if (!VacuumBags.serverConfig.HarderBagRecipes) {
				CreateRecipe()
				.AddTile(TileID.WorkBenches)
				.AddIngredient(ItemID.Silk, 2)
				.AddIngredient(ItemID.BrightSilverDye, 1)
				.AddIngredient(ItemID.WhiteString, 1)
				.Register();
			}
			else {
				CreateRecipe()
				.AddTile(TileID.WorkBenches)
				.AddIngredient(ItemID.Silk, 10)
				.AddIngredient(ItemID.BrightSilverDye, 1)
				.AddIngredient(ItemID.WhiteString, 1)
				.AddIngredient(ItemID.Diamond, 3)
				.AddIngredient(ItemID.WhitePearl, 1)
				.AddIngredient(ItemID.MilkCarton, 1)
				.AddIngredient(ItemID.FrostMinnow, 3)
				.Register();
			}
		}
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<BagToggle>().BagsAndPacks;
		}
	}
}