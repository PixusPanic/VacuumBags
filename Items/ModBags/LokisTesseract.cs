using androLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ModLoader.Default;
using androLib.Items;
using static Terraria.ID.ContentSamples.CreativeHelper;

namespace VacuumBags.Items
{
	[Autoload(false)]
	public class LokisTesseract : ModBag, INeedsSetUpAllowedList
	{
		public override string ModDisplayNameTooltip => "Thorium";
		public override string LocalizationDisplayName => "Loki's Tesseract";
		new public static int BagStorageID;
		new public static Color PanelColor => new Color(4, 189, 189, androLib.Common.Configs.ConfigValues.UIAlpha);
		new public static Color ScrollBarColor => new Color(180, 76, 0, androLib.Common.Configs.ConfigValues.UIAlpha);
		new public static Color ButtonHoverColor => new Color(150, 255, 255, androLib.Common.Configs.ConfigValues.UIAlpha);
		protected static int DefaultBagSize => 200;
		public static void RegisterWithAndroLib(Mod mod) {
			BagStorageID = StorageManager.RegisterVacuumStorageClass(
				mod,//Mod
				typeof(LokisTesseract),//type 
				ItemAllowedToBeStored,//Is allowed function, Func<Item, bool>
				null,//Localization Key name.  Attempts to determine automatically by treating the type as a ModItem, or you can specify.
				-DefaultBagSize,//StorageSize
				true,//Can vacuum
				() => PanelColor, // Get color function. Func<using Microsoft.Xna.Framework.Color>
				() => ScrollBarColor, // Get Scroll bar color function. Func<using Microsoft.Xna.Framework.Color>
				() => ButtonHoverColor, // Get Button hover color function. Func<using Microsoft.Xna.Framework.Color>
				() => ModContent.ItemType<LokisTesseract>(),//Get ModItem type
				80,//UI Left
				675,//UI Top
				() => AllowedItems
			);
		}
		public override void AddRecipes() {
			if (AndroMod.thoriumEnabled) {
				if (AndroMod.thoriumMod.TryFind("SmoothCoal", out ModItem smoothCoal)
					&& AndroMod.thoriumMod.TryFind("LifeQuartz", out ModItem lifeQuartz)
					&& AndroMod.thoriumMod.TryFind("Blood", out ModItem blood)
					) {
					if (!VacuumBags.serverConfig.HarderBagRecipes) {
						if (AndroMod.thoriumMod.TryFind("ThoriumOre", out ModItem thoriumOre))
						CreateRecipe()
						.AddTile(TileID.Anvils)
						.AddIngredient(smoothCoal.Type, 3)
						.AddIngredient(lifeQuartz.Type, 5)
						.AddIngredient(blood.Type, 2)
						.AddIngredient(thoriumOre.Type, 5)
						.Register();
					}
					else {
						if (AndroMod.thoriumMod.TryFind("ThoriumBar", out ModItem thoriumBar)
							&& AndroMod.thoriumMod.TryFind("BloodPotion", out ModItem bloodPotion)
							&& AndroMod.thoriumMod.TryFind("LivingLeaf", out ModItem livingLeaf)
							&& AndroMod.thoriumMod.TryFind("YewWood", out ModItem yewWood)
							&& AndroMod.thoriumMod.TryFind("ThoriumAnvil", out ModTile thoriumAnvil)
							) {
							CreateRecipe()
							.AddTile(thoriumAnvil.Type)
							.AddIngredient(smoothCoal.Type, 5)
							.AddIngredient(lifeQuartz.Type, 25)
							.AddIngredient(bloodPotion.Type, 2)
							.AddIngredient(thoriumBar.Type, 5)
							.AddIngredient(livingLeaf.Type, 2)
							.AddIngredient(yewWood.Type, 20)
							.Register();
						}
					}
				}
			}
		}

		public static bool ItemAllowedToBeStored(Item item) => AllowedItems.Contains(item.type) || item.ModItem is UnloadedItem unloadedItem && unloadedItem.ModName == AndroMod.thoriumModName;
		public static SortedSet<int> AllowedItems => AllowedItemsManager.AllowedItems;
		public static AllowedItemsManager AllowedItemsManager = new(ModContent.ItemType<LokisTesseract>, () => BagStorageID, DevCheck, DevWhiteList, DevModWhiteList, DevBlackList, DevModBlackList, ItemGroups, EndWords, SearchWords);
		public AllowedItemsManager GetAllowedItemsManager => AllowedItemsManager;
		protected static bool? DevCheck(ItemSetInfo info, SortedSet<ItemGroup> itemGroups, SortedSet<string> endWords, SortedSet<string> searchWords) {
			return null;
		}
		protected static SortedSet<int> DevWhiteList() {
			SortedSet<int> devWhiteList = AndroMod.thoriumEnabled ? new(AndroMod.thoriumMod.GetContent<ModItem>().Where(m => m != null).Select(m => m.Type)) : new();

			return devWhiteList;
		}
		protected static SortedSet<string> DevModWhiteList() {
			SortedSet<string> devModWhiteList = new() {

			};

			return devModWhiteList;
		}
		protected static SortedSet<int> DevBlackList() {
			SortedSet<int> devBlackList = new() {

			};

			return devBlackList;
		}
		protected static SortedSet<string> DevModBlackList() {
			SortedSet<string> devModBlackList = new() {

			};

			return devModBlackList;
		}
		protected static SortedSet<ItemGroup> ItemGroups() {
			SortedSet<ItemGroup> itemGroups = new() {

			};

			return itemGroups;
		}
		protected static SortedSet<string> EndWords() {
			SortedSet<string> endWords = new() {

			};

			return endWords;
		}

		protected static SortedSet<string> SearchWords() {
			SortedSet<string> searchWords = new() {

			};

			return searchWords;
		}
	}
}