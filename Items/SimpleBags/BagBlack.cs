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
	public class BagBlack : SimpleBag {
		public static IBagModItem Instance {
			get {
				if (instance == null)
					instance = new BagBlack();

				return instance;
			}
		}
		private static IBagModItem instance;
		public override int GetBagType() => ModContent.ItemType<BagBlack>();
		public override int MyTileType => ModContent.TileType<Tiles.BagBlack>();
		protected override SortedSet<int> GetDefaultBlacklist() {
			return new() {
				ModContent.ItemType<BagBlack>(),
				ModContent.ItemType<PackBlack>(),
			};
		}

		public override Color PanelColor => new Color(20, 20, 20, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ScrollBarColor => new Color(30, 30, 30, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ButtonHoverColor => new Color(40, 40, 40, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override void AddRecipes() {
			if (!VacuumBags.serverConfig.HarderBagRecipes) {
				CreateRecipe()
				.AddTile(TileID.WorkBenches)
				.AddIngredient(ItemID.Silk, 2)
				.AddIngredient(ItemID.BlackInk, 1)
				.AddIngredient(ItemID.WhiteString, 1)
				.Register();
			}
			else {
				CreateRecipe()
				.AddTile(TileID.WorkBenches)
				.AddIngredient(ItemID.Silk, 10)
				.AddIngredient(ItemID.BlackInk, 1)
				.AddIngredient(ItemID.BlackString, 1)
				.AddIngredient(ItemID.CoffeeCup, 1)
				.AddIngredient(ItemID.TopHat, 1)
				.AddIngredient(ItemID.TuxedoShirt, 1)
				.AddIngredient(ItemID.TuxedoPants, 1)
				.AddIngredient(ItemID.BlackPearl, 1)
				.Register();
			}
		}
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<BagToggle>().BagsAndPacks;
		}
	}
}