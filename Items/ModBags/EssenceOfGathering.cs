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
using androLib.Common.Globals;
using androLib.Items;
using static Terraria.ID.ContentSamples.CreativeHelper;

namespace VacuumBags.Items
{
	[Autoload(false)]
	public class EssenceOfGathering : ModBag, INeedsSetUpAllowedList
	{
		public override string ModDisplayNameTooltip => "Stars Above";
		public override string LocalizationDisplayName => "Essence of Gathering";
		public override string Artist => "Stars Above";
		new public static int BagStorageID;
		new public static Color PanelColor => new Color(152, 59, 137, androLib.Common.Configs.ConfigValues.UIAlpha);
		new public static Color ScrollBarColor => new Color(44, 60, 180, androLib.Common.Configs.ConfigValues.UIAlpha);
		new public static Color ButtonHoverColor => new Color(200, 75, 160, androLib.Common.Configs.ConfigValues.UIAlpha);
		protected static int DefaultBagSize => 200;
		public static void RegisterWithAndroLib(Mod mod) {
			BagStorageID = StorageManager.RegisterVacuumStorageClass(
				mod,//Mod
				typeof(EssenceOfGathering),//type 
				ItemAllowedToBeStored,//Is allowed function, Func<Item, bool>
				null,//Localization Key name.  Attempts to determine automatically by treating the type as a ModItem, or you can specify.
				-DefaultBagSize,//StorageSize
				true,//Can vacuum
				() => PanelColor, // Get color function. Func<using Microsoft.Xna.Framework.Color>
				() => ScrollBarColor, // Get Scroll bar color function. Func<using Microsoft.Xna.Framework.Color>
				() => ButtonHoverColor, // Get Button hover color function. Func<using Microsoft.Xna.Framework.Color>
				() => ModContent.ItemType<EssenceOfGathering>(),//Get ModItem type
				80,//UI Left
				675,//UI Top
				UpdateAllowedList
			);
		}
		public override void AddRecipes() {
			if (AndroMod.starsAboveEnabled) {
				if (!VacuumBags.serverConfig.HarderBagRecipes) {
					CreateRecipe()
					.AddTile(TileID.WorkBenches)
					.AddRecipeGroup($"{AndroMod.ModName}:{AndroModSystem.StarsAboveAnyKingSlimeEssence}", 1)
					.AddIngredient(ItemID.Glass, 10)
					.Register();
				}
				else {
					CreateRecipe()
					.AddTile(TileID.WorkBenches)
					.AddRecipeGroup($"{AndroMod.ModName}:{AndroModSystem.StarsAboveAnyKingSlimeEssence}", 1)
					.AddRecipeGroup($"{AndroMod.ModName}:{AndroModSystem.StarsAboveAnyEyeOfCthulhuEssence}", 1)
					.AddRecipeGroup($"{AndroMod.ModName}:{AndroModSystem.StarsAboveAnyEaterOfWorldsOrBrainOfCthulhuEssence}", 1)
					.AddRecipeGroup($"{AndroMod.ModName}:{AndroModSystem.StarsAboveAnyQueenBeeEssence}", 1)
					.AddRecipeGroup($"{AndroMod.ModName}:{AndroModSystem.StarsAboveAnySkeletronEssence}", 1)
					.AddIngredient(ItemID.Glass, 40)
					.Register();
				}
			}
		}

		private static void UpdateAllowedList(int item, bool add) {
			if (add) {
				AllowedItems.Add(item);
			}
			else {
				AllowedItems.Remove(item);
			}
		}
		public static bool ItemAllowedToBeStored(Item item) => AllowedItems.Contains(item.type) || item.ModItem is UnloadedItem unloadedItem && unloadedItem.ModName == AndroMod.starsAboveModName;
		public static SortedSet<int> AllowedItems => AllowedItemsManager.AllowedItems;
		public static AllowedItemsManager AllowedItemsManager = new(ModContent.ItemType<EssenceOfGathering>, () => BagStorageID, DevCheck, DevWhiteList, DevModWhiteList, DevBlackList, DevModBlackList, ItemGroups, EndWords, SearchWords);
		public AllowedItemsManager GetAllowedItemsManager => AllowedItemsManager;
		protected static bool? DevCheck(ItemSetInfo info, SortedSet<ItemGroup> itemGroups, SortedSet<string> endWords, SortedSet<string> searchWords) {
			return null;
		}
		protected static SortedSet<int> DevWhiteList() {
			SortedSet<int> devWhiteList = AndroMod.starsAboveEnabled ? new(AndroMod.starsAboveMod.GetContent<ModItem>().Where(m => m != null).Select(m => m.Type)) : new();

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