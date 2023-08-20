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
using static Terraria.ID.ContentSamples.CreativeHelper;

namespace VacuumBags.Items
{
	public class WallEr : AndroModItem, ISoldByWitch
	{
		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
		public override void SetDefaults() {
			Item.maxStack = 99;
			Item.value = 100000;
			Item.rare = ItemRarityID.Blue;
			Item.width = 30;
			Item.height = 32;
		}
		public override void AddRecipes() {
			if (!VacuumBags.serverConfig.HarderBagRecipes) {
				CreateRecipe()
				.AddTile(TileID.Anvils)
				.AddIngredient(ItemID.CopperBar, 10)
				.AddIngredient(ItemID.WoodenBeam, 20)
				.AddIngredient(ItemID.IronFence, 20)
				.Register();
			}
			else {
				CreateRecipe()
				.AddTile(TileID.Anvils)
				.AddIngredient(ItemID.CopperBar, 50)
				.AddIngredient(ItemID.Obsidian, 10)
				.AddIngredient(ItemID.DynastyWood, 100)
				.AddIngredient(ItemID.BorealWood, 50)
				.AddIngredient(ItemID.PalmWood, 50)
				.AddIngredient(ItemID.RichMahogany, 50)
				.Register();
			}
		}

		public static int BagStorageID;//Set this when registering with androLib.


		public static void RegisterWithAndroLib(Mod mod) {
			BagStorageID = StorageManager.RegisterVacuumStorageClass(
				mod,//Mod
				typeof(WallEr),//type 
				(Item item) => ItemAllowedToBeStored(item),//Is allowed function, Func<Item, bool>
				null,//Localization Key name.  Attempts to determine automatically by treating the type as a ModItem, or you can specify.
				100,//StorageSize
				true,//Can vacuum
				() => new Color(120, 60, 10, androLib.Common.Configs.ConfigValues.UIAlpha),    // Get color function. Func<using Microsoft.Xna.Framework.Color>
				() => new Color(130, 70, 10, androLib.Common.Configs.ConfigValues.UIAlpha),    // Get Scroll bar color function. Func<using Microsoft.Xna.Framework.Color>
				() => new Color(150, 80, 0, androLib.Common.Configs.ConfigValues.UIAlpha),     // Get Button hover color function. Func<using Microsoft.Xna.Framework.Color>
				() => ModContent.ItemType<WallEr>(),//Get ModItem type
				80,//UI Left
				675//UI Top
			);
		}

		public static bool ItemAllowedToBeStored(Item item) => AllowedItems.Contains(item.type);

		public static SortedSet<int> AllowedItems {
			get {
				if (allowedItems == null)
					GetAllowedItems();

				return allowedItems;
			}
		}
		private static SortedSet<int> allowedItems = null;

		private static void GetAllowedItems() {
			allowedItems = new() {

			};

			SortedSet<string> endWords = new() {
				"wall",
				"wallunsafe",
				"wallpaper",
				"echo",
			};

			SortedSet<string> searchWords = new() {
				"slabwall",
			};

			for (int i = 0; i < ItemLoader.ItemCount; i++) {
				Item item = ContentSamples.ItemsByType[i];
				if (item.NullOrAir())
					continue;

				string lowerName = item.Name.ToLower();
				bool added = false;
				foreach (string endWord in endWords) {
					if (lowerName.EndsWith(endWord)) {
						allowedItems.Add(item.type);
						added = true;
						break;
					}
				}

				if (added)
					continue;

				foreach (string searchWord in searchWords) {
					if (lowerName.Contains(searchWord)) {
						allowedItems.Add(item.type);
						added = true;
						break;
					}
				}

				ItemGroupAndOrderInGroup group = new ItemGroupAndOrderInGroup(item);
				if (group.Group == ItemGroup.Walls) {
					allowedItems.Add(item.type);
					continue;
				}
			}
		}

		#region AndroModItem attributes that you don't need.

		public virtual SellCondition SellCondition => SellCondition.Never;
		public virtual float SellPriceModifier => 1f;
		public override List<WikiTypeID> WikiItemTypes => new() { WikiTypeID.Storage };
		public override string LocalizationTooltip =>
			$"Automatically stores walling materials (walls, fences, beams, etc.)\n" +
			$"When in your inventory, the contents of the bag are available for crafting.\n" +
			$"Right click to open the bag.";
		public override string Artist => "andro951";
		public override string Designer => "@kingjoshington";

		#endregion
	}
}