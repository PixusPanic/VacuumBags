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
	public class BagBrown : SimpleBag {
		public static IBagModItem Instance {
			get {
				if (instance == null)
					instance = new BagBrown();

				return instance;
			}
		}
		private static IBagModItem instance;
		public override int GetBagType() => ModContent.ItemType<BagBrown>();
		public override int MyTileType => ModContent.TileType<Tiles.BagBrown>();
		protected override SortedSet<int> GetDefaultBlacklist() {
			return new() {
				ModContent.ItemType<BagBrown>(),
				ModContent.ItemType<PackBrown>(),
			};
		}

		public override Color PanelColor => new Color(25, 10, 3, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ScrollBarColor => new Color(30, 10, 1, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ButtonHoverColor => new Color(50, 20, 6, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override void AddRecipes() {
			if (!VacuumBags.serverConfig.HarderBagRecipes) {
				CreateRecipe()
				.AddTile(TileID.WorkBenches)
				.AddIngredient(ItemID.Silk, 2)
				.AddIngredient(ItemID.Leather, 5)
				.AddIngredient(ItemID.WhiteString, 1)
				.Register();
			}
			else {
				CreateRecipe()
				.AddTile(TileID.WorkBenches)
				.AddIngredient(ItemID.Silk, 10)
				.AddIngredient(ItemID.Leather, 10)
				.AddIngredient(ItemID.BrownString, 1)
				.AddIngredient(ItemID.Coconut, 2)
				.AddIngredient(ItemID.BrownMoss, 1)
				.AddIngredient(ItemID.FossilOre, 3)
				.Register();
			}
		}
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<BagToggle>().BagsAndPacks;
		}
	}
}