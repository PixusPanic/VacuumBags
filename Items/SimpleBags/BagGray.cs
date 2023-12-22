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
	public class BagGray : SimpleBag {
		public static BagModItem Instance {
			get {
				if (instance == null)
					instance = new BagGray();

				return instance;
			}
		}
		private static BagModItem instance;
		public override int GetBagType() => ModContent.ItemType<BagGray>();
		public override int MyTileType => ModContent.TileType<Tiles.BagGray>();
		protected override SortedSet<int> GetDefaultBlacklist() {
			return new() {
				ModContent.ItemType<BagGray>(),
				ModContent.ItemType<PackGray>(),
			};
		}

		public override Color PanelColor => new Color(30, 30, 30, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ScrollBarColor => new Color(60, 60, 60, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ButtonHoverColor => new Color(90, 90, 90, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override void AddRecipes() {
			if (!VacuumBags.serverConfig.HarderBagRecipes) {
				CreateRecipe()
				.AddTile(TileID.WorkBenches)
				.AddIngredient(ItemID.Silk, 2)
				.AddIngredient(ItemID.GrayStucco, 10)
				.AddIngredient(ItemID.WhiteString, 1)
				.Register();
			}
			else {
				CreateRecipe()
				.AddTile(TileID.WorkBenches)
				.AddIngredient(ItemID.Silk, 10)
				.AddIngredient(ItemID.SiltBlock, 200)
				.AddIngredient(ItemID.WhiteString, 1)
				.AddIngredient(ItemID.GrayBrick, 100)
				.AddIngredient(ItemID.ShuckedOyster, 3)
				.AddIngredient(ItemID.Bass, 1)
				.Register();
			}
		}
	}
}