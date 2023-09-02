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
	public class BagBrown : SimpleBag {
		new public static int BagStorageID;
		public override int MyTileType => ModContent.TileType<Tiles.BagBrown>();
		public static void CloseBag() => StorageManager.CloseBag(BagStorageID);
		new public static SortedSet<int> Blacklist {
			get {
				if (blacklist == null) {
					blacklist = new() {
						ModContent.ItemType<BagBrown>(),
						ModContent.ItemType<PackBrown>(),
					};
				}

				return blacklist;
			}
		}
		private static SortedSet<int> blacklist = null;
		public static bool ItemAllowedToBeStored(Item item) => !Blacklist.Contains(item.type);
		new public static Color PanelColor => new Color(25, 10, 3, androLib.Common.Configs.ConfigValues.UIAlpha);
		new public static void RegisterWithAndroLib(Mod mod) {
			BagStorageID = StorageManager.RegisterVacuumStorageClass(
				mod,//Mod
				typeof(BagBrown),//type 
				ItemAllowedToBeStored,//Is allowed function, Func<Item, bool>
				null,//Localization Key name.  Attempts to determine automatically by treating the type as a ModItem, or you can specify.
				20,//StorageSize
				null,//Can vacuum
				() => PanelColor,//Get color function. Func<using Microsoft.Xna.Framework.Color>
				() => new Color(30, 10, 1, androLib.Common.Configs.ConfigValues.UIAlpha),//Get Scroll bar color function. Func<using Microsoft.Xna.Framework.Color>
				() => new Color(50, 20, 6, androLib.Common.Configs.ConfigValues.UIAlpha),//Get Button hover color function. Func<using Microsoft.Xna.Framework.Color>
				() => ModContent.ItemType<BagBrown>(),//Get ModItem type
				80,//UI Left
				675//UI Top
			);
		}
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
	}
}