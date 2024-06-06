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
	public class BagBlue : SimpleBag {
		public static IBagModItem Instance {
			get {
				if (instance == null)
					instance = new BagBlue();

				return instance;
			}
		}
		private static IBagModItem instance;
		public override int GetBagType() => ModContent.ItemType<BagBlue>();
		public override int MyTileType => ModContent.TileType<Tiles.BagBlue>();
		protected override SortedSet<int> GetDefaultBlacklist() {
			return new() {
				ModContent.ItemType<BagBlue>(),
				ModContent.ItemType<PackBlue>(),
			};
		}
		public override Color PanelColor => new Color(10, 10, 80, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ScrollBarColor => new Color(10, 10, 90, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ButtonHoverColor => new Color(0, 0, 120, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override void AddRecipes() {
			if (!VacuumBags.serverConfig.HarderBagRecipes) {
				CreateRecipe()
				.AddTile(TileID.WorkBenches)
				.AddIngredient(ItemID.Silk, 2)
				.AddIngredient(ItemID.CyanHusk, 1)
				.AddIngredient(ItemID.WhiteString, 1)
				.Register();
			}
			else {
				CreateRecipe()
				.AddTile(TileID.WorkBenches)
				.AddIngredient(ItemID.Silk, 10)
				.AddIngredient(ItemID.CyanHusk, 1)
				.AddIngredient(ItemID.BlueString, 1)
				.AddIngredient(ItemID.GlowingMushroom, 50)
				.AddIngredient(ItemID.Sapphire, 1)
				.AddIngredient(ItemID.JojaCola, 1)
				.Register();
			}
		}
	}
}