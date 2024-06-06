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
	public class BagYellow : SimpleBag {
		public static IBagModItem Instance {
			get {
				if (instance == null)
					instance = new BagYellow();

				return instance;
			}
		}
		private static IBagModItem instance;
		public override int GetBagType() => ModContent.ItemType<BagYellow>();
		public override int MyTileType => ModContent.TileType<Tiles.BagYellow>();
		protected override SortedSet<int> GetDefaultBlacklist() {
			return new() {
				ModContent.ItemType<BagYellow>(),
				ModContent.ItemType<PackYellow>(),
			};
		}

		public override Color PanelColor => new Color(120, 120, 10, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ScrollBarColor => new Color(130, 130, 10, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ButtonHoverColor => new Color(150, 150, 0, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override void AddRecipes() {
			if (!VacuumBags.serverConfig.HarderBagRecipes) {
				CreateRecipe()
				.AddTile(TileID.WorkBenches)
				.AddIngredient(ItemID.Silk, 2)
				.AddIngredient(ItemID.YellowMarigold, 1)
				.AddIngredient(ItemID.WhiteString, 1)
				.Register();
			}
			else {
				CreateRecipe()
				.AddTile(TileID.WorkBenches)
				.AddIngredient(ItemID.Silk, 10)
				.AddIngredient(ItemID.YellowMarigold, 1)
				.AddIngredient(ItemID.YellowString, 1)
				.AddIngredient(ItemID.Banana, 2)
				.AddIngredient(ItemID.Sunflower, 10)
				.AddIngredient(ItemID.Topaz, 15)
				.Register();
			}
		}
	}
}