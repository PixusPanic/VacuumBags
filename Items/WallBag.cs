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
using System.Text.Json.Serialization;

namespace VacuumBags.Items
{
	//public class WallBag : AndroModItem, ISoldByWitch
	//{
	//	public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
	//	public override void SetDefaults() {
	//		Item.maxStack = 99;
	//		Item.value = 100000;
	//		Item.rare = ItemRarityID.Blue;
	//		Item.width = 32;
	//		Item.height = 32;
	//	}
	//	public override void AddRecipes() {
	//		Recipe recipe = CreateRecipe();
	//		recipe.AddTile(TileID.WorkBenches);
	//		recipe.AddIngredient(ItemID.TatteredCloth);
	//		recipe.AddIngredient(ItemID.WhiteString);
	//		recipe.Register();
	//	}

	//	public static int BagStorageID;//Set this when registering with androLib.


	//	public static void RegisterWithAndroLib(Mod mod) {
	//		BagStorageID = StorageManager.RegisterVacuumStorageClass(
	//			mod,//Mod
	//			typeof(WallBag),//type 
	//			(Item item) => ItemAllowedToBeStored(item),//Is allowed function, Func<Item, bool>
	//			null,//Localization Key name.  Attempts to determine automatically by treating the type as a ModItem, or you can specify.
	//			100,//StorageSize
	//			true,//Can vacuum
	//			() => new Color(25, 10, 3, androLib.Common.Configs.ConfigValues.UIAlpha),//Get color function. Func<using Microsoft.Xna.Framework.Color>
	//			() => new Color(30, 10, 1, androLib.Common.Configs.ConfigValues.UIAlpha),//Get Scroll bar color function. Func<using Microsoft.Xna.Framework.Color>
	//			() => new Color(50, 20, 6, androLib.Common.Configs.ConfigValues.UIAlpha),//Get Button hover color function. Func<using Microsoft.Xna.Framework.Color>
	//			() => ModContent.ItemType<WallBag>(),//Get ModItem type
	//			80,//UI Left
	//			675//UI Top
	//		);
	//	}

	//	public static bool ItemAllowedToBeStored(Item item) => AllowedItems.Contains(item.type);

	//	public static SortedSet<int> AllowedItems {
	//		get {
	//			if (allowedItems == null)
	//				GetAllowedItems();

	//			return allowedItems;
	//		}
	//	}
	//	private static SortedSet<int> allowedItems = null;

	//	private static void GetAllowedItems() {
	//		allowedItems = new() {

	//		};

	//		SortedSet<string> endWords = new() {
	//			"wall",
	//			"wallunsafe",
	//			"wallpaper",
	//			"echo",
	//		};

	//		SortedSet<string> searchWords = new() {
	//			"slabwall",
	//		};

	//		for (int i = 0; i < ItemLoader.ItemCount; i++) {
	//			Item item = ContentSamples.ItemsByType[i];
	//			if (item.NullOrAir())
	//				continue;

	//			string lowerName = item.Name.ToLower();
	//			bool added = false;
	//			foreach (string endWord in endWords) {
	//				if (lowerName.EndsWith(endWord)) {
	//					allowedItems.Add(item.type);
	//					added = true;
	//					break;
	//				}
	//			}

	//			if (added)
	//				continue;

	//			foreach (string searchWord in searchWords) {
	//				if (lowerName.Contains(searchWord)) {
	//					allowedItems.Add(item.type);
	//					added = true;
	//					break;
	//				}
	//			}
	//		}
	//	}

	//	#region AndroModItem attributes that you don't need.

	//	public virtual SellCondition SellCondition => SellCondition.Never;
	//	public virtual float SellPriceModifier => 1f;
	//	public override List<WikiTypeID> WikiItemTypes => new() { WikiTypeID.Storage };
	//	public override string LocalizationTooltip =>
	//		$"Automatically stores building materials (bricks, craftable blocks, etc.)\n" +
	//		$"When in your inventory, the contents of the bag are available for crafting.\n" +
	//		$"Right click to open the bag.";
	//	public override string Artist => "andro951";
	//	public override string Designer => "@kingjoshington";

	//	#endregion
	//}
}