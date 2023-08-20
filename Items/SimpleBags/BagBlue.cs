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
	public class BagBlue : SimpleBag {
		protected override IDictionary<int, int> recipeIngredients => new Dictionary<int, int>() {
			{ ItemID.CyanHusk, 1 },
			{ ItemID.WhiteString, 1 }
		};
		new public static void RegisterWithAndroLib(Mod mod) {
			BagStorageID = StorageManager.RegisterVacuumStorageClass(
				mod,//Mod
				typeof(BagBlue),//type 
				ItemAllowedToBeStored,//Is allowed function, Func<Item, bool>
				null,//Localization Key name.  Attempts to determine automatically by treating the type as a ModItem, or you can specify.
				40,//StorageSize
				null,//Can vacuum
				() => new Color(10, 10, 80, androLib.Common.Configs.ConfigValues.UIAlpha), // Get color function. Func<using Microsoft.Xna.Framework.Color>
				() => new Color(10, 10, 90, androLib.Common.Configs.ConfigValues.UIAlpha), // Get Scroll bar color function. Func<using Microsoft.Xna.Framework.Color>
				() => new Color(0, 0, 120, androLib.Common.Configs.ConfigValues.UIAlpha), // Get Button hover color function. Func<using Microsoft.Xna.Framework.Color>
				() => ModContent.ItemType<BagBlue>(),//Get ModItem type
				80,//UI Left
				675//UI Top
			);
		}

		protected override void EditRecipe(ref Recipe recipe) { }
	}
}