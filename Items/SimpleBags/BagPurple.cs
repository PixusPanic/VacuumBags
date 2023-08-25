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
	public class BagPurple : SimpleBag {
		new public static int BagStorageID;
		public override int MyTileType => ModContent.TileType<Tiles.BagPurple>();
		public static void CloseBag() => StorageManager.CloseBag(BagStorageID);
		new public static SortedSet<int> Blacklist {
			get {
				if (blacklist == null) {
					blacklist = new() {
						ModContent.ItemType<BagPurple>(),
						ModContent.ItemType<PackPurple>(),
					};
				}

				return blacklist;
			}
		}
		private static SortedSet<int> blacklist = null;
		new public static Color PanelColor => new Color(80, 10, 80, androLib.Common.Configs.ConfigValues.UIAlpha);
		new public static void RegisterWithAndroLib(Mod mod) {
			BagStorageID = StorageManager.RegisterVacuumStorageClass(
				mod,//Mod
				typeof(BagPurple),//type 
				ItemAllowedToBeStored,//Is allowed function, Func<Item, bool>
				null,//Localization Key name.  Attempts to determine automatically by treating the type as a ModItem, or you can specify.
				20,//StorageSize
				null,//Can vacuum
				() => PanelColor,    // Get color function. Func<using Microsoft.Xna.Framework.Color>
				() => new Color(90, 10, 90, androLib.Common.Configs.ConfigValues.UIAlpha),    // Get Scroll bar color function. Func<using Microsoft.Xna.Framework.Color>
				() => new Color(120, 0, 120, androLib.Common.Configs.ConfigValues.UIAlpha),   // Get Button hover color function. Func<using Microsoft.Xna.Framework.Color>
				() => ModContent.ItemType<BagPurple>(),//Get ModItem type
				80,//UI Left
				675//UI Top
			);
		}


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