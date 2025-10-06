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
	public class BagOrange : SimpleBag {
		public static IBagModItem Instance {
			get {
				if (instance == null)
					instance = new BagOrange();

				return instance;
			}
		}
		private static IBagModItem instance;
		public override int GetBagType() => ModContent.ItemType<BagOrange>();
		public override int MyTileType => ModContent.TileType<Tiles.BagOrange>();
		protected override SortedSet<int> GetDefaultBlacklist() {
			return new() {
				ModContent.ItemType<BagOrange>(),
				ModContent.ItemType<PackOrange>(),
			};
		}

		public override Color PanelColor => new Color(120, 60, 10, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ScrollBarColor => new Color(130, 70, 10, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ButtonHoverColor => new Color(150, 80, 0, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override void AddRecipes() {
			if (!VacuumBags.serverConfig.HarderBagRecipes) {
				CreateRecipe()
				.AddTile(TileID.WorkBenches)
				.AddIngredient(ItemID.Silk, 2)
				.AddIngredient(ItemID.OrangeBloodroot, 1)
				.AddIngredient(ItemID.WhiteString, 1)
				.Register();
			}
			else {
				CreateRecipe()
				.AddTile(TileID.WorkBenches)
				.AddIngredient(ItemID.Silk, 10)
				.AddIngredient(ItemID.OrangeBloodroot, 1)
				.AddIngredient(ItemID.OrangeString, 1)
				.AddIngredient(ItemID.Grapefruit, 2)
				.AddIngredient(ItemID.OrangeDragonfly, 1)
				.AddIngredient(ItemID.Amber, 5)
				.Register();
			}
		}
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<BagToggle>().BagsAndPacks;
		}
	}
}