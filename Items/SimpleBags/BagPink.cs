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
	public class BagPink : SimpleBag {
		new public static int BagStorageID;
		public override int MyTileType => ModContent.TileType<Tiles.BagPink>();
		public static void CloseBag() => StorageManager.CloseBag(BagStorageID);
		new public static SortedSet<int> Blacklist {
			get {
				if (blacklist == null) {
					blacklist = new() {
						ModContent.ItemType<BagPink>(),
						ModContent.ItemType<PackPink>(),
					};
				}

				return blacklist;
			}
		}
		private static SortedSet<int> blacklist = null;
		public static bool ItemAllowedToBeStored(Item item) => !Blacklist.Contains(item.type);
		new public static Color PanelColor => new Color(255, 192, 203, androLib.Common.Configs.ConfigValues.UIAlpha);
		new public static void RegisterWithAndroLib(Mod mod) {
			BagStorageID = StorageManager.RegisterVacuumStorageClass(
				mod,//Mod
				typeof(BagPink),//type 
				ItemAllowedToBeStored,//Is allowed function, Func<Item, bool>
				null,//Localization Key name.  Attempts to determine automatically by treating the type as a ModItem, or you can specify.
				BagSize,//StorageSize
				IsVacuumBag,//Can vacuum
				() => PanelColor, // Get color function. Func<using Microsoft.Xna.Framework.Color>
				() => new Color(255, 182, 193, androLib.Common.Configs.ConfigValues.UIAlpha), // Get Scroll bar color function. Func<using Microsoft.Xna.Framework.Color>
				() => new Color(255, 105, 180, androLib.Common.Configs.ConfigValues.UIAlpha), // Get Button hover color function. Func<using Microsoft.Xna.Framework.Color>
				() => ModContent.ItemType<BagPink>(),//Get ModItem type
				80,//UI Left
				675//UI Top
			);
		}
		public override void AddRecipes() {
			if (!VacuumBags.serverConfig.HarderBagRecipes) {
				CreateRecipe()
				.AddTile(TileID.WorkBenches)
				.AddIngredient(ItemID.Silk, 2)
				.AddIngredient(ItemID.PinkPricklyPear, 1)
				.AddIngredient(ItemID.WhiteString, 1)
				.Register();
			}
			else {
				CreateRecipe()
				.AddTile(TileID.WorkBenches)
				.AddIngredient(ItemID.Silk, 10)
				.AddIngredient(ItemID.PinkPricklyPear, 3)
				.AddIngredient(ItemID.PinkString, 1)
				.AddIngredient(ItemID.PinkGel, 100)
				.AddIngredient(ItemID.ArmoredCavefish, 5)
				.AddIngredient(ItemID.Salmon, 5)
				.Register();
			}
		}
	}
}