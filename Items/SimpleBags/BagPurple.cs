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
	public class BagPurple : SimpleBag {
		public static BagModItem Instance {
			get {
				if (instance == null)
					instance = new BagPurple();

				return instance;
			}
		}
		private static BagModItem instance;
		public override int GetBagType() => ModContent.ItemType<BagPurple>();
		public override int MyTileType => ModContent.TileType<Tiles.BagPurple>();
		protected override SortedSet<int> GetDefaultBlacklist() {
			return new() {
				ModContent.ItemType<BagPurple>(),
				ModContent.ItemType<PackPurple>(),
			};
		}

		public override Color PanelColor => new Color(80, 10, 80, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ScrollBarColor => new Color(90, 10, 90, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ButtonHoverColor => new Color(120, 0, 120, androLib.Common.Configs.ConfigValues.UIAlpha);

		public override void AddRecipes() {
			if (!VacuumBags.serverConfig.HarderBagRecipes) {
				CreateRecipe()
				.AddTile(TileID.WorkBenches)
				.AddIngredient(ItemID.Silk, 2)
				.AddIngredient(ItemID.VioletHusk, 1)
				.AddIngredient(ItemID.WhiteString, 1)
				.Register();
			}
			else {
				CreateRecipe()
				.AddTile(TileID.WorkBenches)
				.AddIngredient(ItemID.Silk, 10)
				.AddIngredient(ItemID.VioletHusk, 1)
				.AddIngredient(ItemID.PurpleString, 1)
				.AddIngredient(ItemID.PurpleMucos, 2)
				.AddIngredient(ItemID.Plum, 2)
				.AddIngredient(ItemID.Amethyst, 15)
				.Register();
			}
		}
	}
}