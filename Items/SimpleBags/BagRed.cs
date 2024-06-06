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
	public class BagRed : SimpleBag {
		public static IBagModItem Instance {
			get {
				if (instance == null)
					instance = new BagRed();

				return instance;
			}
		}
		private static IBagModItem instance;
		public override int GetBagType() => ModContent.ItemType<BagRed>();
		public override int MyTileType => ModContent.TileType<Tiles.BagRed>();
		protected override SortedSet<int> GetDefaultBlacklist() {
			return new() {
				ModContent.ItemType<BagRed>(),
				ModContent.ItemType<PackRed>(),
			};
		}

		public override Color PanelColor => new Color(80, 10, 10, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ScrollBarColor => new Color(90, 10, 10, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ButtonHoverColor => new Color(120, 0, 0, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override void AddRecipes() {
			if (!VacuumBags.serverConfig.HarderBagRecipes) {
				CreateRecipe()
				.AddTile(TileID.WorkBenches)
				.AddIngredient(ItemID.Silk, 2)
				.AddIngredient(ItemID.RedHusk, 1)
				.AddIngredient(ItemID.WhiteString, 1)
				.Register();
			}
			else {
				CreateRecipe()
				.AddTile(TileID.WorkBenches)
				.AddIngredient(ItemID.Silk, 10)
				.AddIngredient(ItemID.RedHusk, 1)
				.AddIngredient(ItemID.RedString, 1)
				.AddIngredient(ItemID.Apple, 5)
				.AddIngredient(ItemID.Ruby, 1)
				.AddIngredient(ItemID.BloodMoonStarter, 1)
				.Register();
			}
		}
	}
}