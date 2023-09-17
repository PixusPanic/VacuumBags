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
using static Terraria.ID.ContentSamples.CreativeHelper;
using System;
using androLib.UI;
using System.Reflection;

namespace VacuumBags.Items
{
	[Autoload(false)]
	public  class FishingBelt : BagModItem, ISoldByWitch, INeedsSetUpAllowedList
	{
		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
		public override void SetDefaults() {
            Item.maxStack = 1;
            Item.value = 100000;
			Item.rare = ItemRarityID.Blue;
			Item.width = 32;
            Item.height = 26;
			Item.ammo = Type;
		}
        public override void AddRecipes() {
			if (!VacuumBags.serverConfig.HarderBagRecipes) {
				CreateRecipe()
				.AddTile(TileID.Loom)
				.AddIngredient(ItemID.Silk, 2)
				.AddIngredient(ItemID.Rope, 5)
				.AddIngredient(ItemID.WhiteString, 1)
				.AddIngredient(ItemID.WoodFishingPole, 1)
				.Register();
			}
			else {
				CreateRecipe()
				.AddTile(TileID.Loom)
				.AddIngredient(ItemID.Silk, 5)
				.AddIngredient(ItemID.Rope, 30)
				.AddIngredient(ItemID.WhiteString, 3)
				.AddIngredient(ItemID.WoodFishingPole, 1)
				.AddIngredient(ItemID.Hook, 1)
				.Register();
			}
		}

		public static int BagStorageID;//Set this when registering with androLib.
		new public static Color PanelColor => new Color(38, 38, 67, androLib.Common.Configs.ConfigValues.UIAlpha);
		new public static Color ScrollBarColor => new Color(46, 31, 18, androLib.Common.Configs.ConfigValues.UIAlpha);
		new public static Color ButtonHoverColor => new Color(92, 122, 173, androLib.Common.Configs.ConfigValues.UIAlpha);
		public static void RegisterWithAndroLib(Mod mod) {
			BagStorageID = StorageManager.RegisterVacuumStorageClass(
				mod,//Mod
				typeof(FishingBelt),//type 
				ItemAllowedToBeStored,//Is allowed function, Func<Item, bool>
				null,//Localization Key name.  Attempts to determine automatically by treating the type as a ModItem, or you can specify.
				100,//StorageSize
				true,//Can vacuum
				() => PanelColor, // Get color function. Func<using Microsoft.Xna.Framework.Color>
				() => ScrollBarColor, // Get Scroll bar color function. Func<using Microsoft.Xna.Framework.Color>
				() => ButtonHoverColor, // Get Button hover color function. Func<using Microsoft.Xna.Framework.Color>
				() => ModContent.ItemType<FishingBelt>(),//Get ModItem type
				80,//UI Left
				675,//UI Top
				() => typeof(Player).GetMethod("Fishing_GetBait", BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(Main.LocalPlayer, new object[] { null }),
				true//Update info accessories in bag
			);
		}

		internal static void OnFishing_GetBait(On_Player.orig_Fishing_GetBait orig, Player self, out Item bait) {
			orig(self, out bait);
			if (Main.LocalPlayer.HeldItem.fishingPole <= 0)
				return;

			Item bagBait = ChooseFromBagOnlyIfFirstInInventory(
				bait,
				self,
				BagStorageID,
				(Item item) => item.bait > 0
			);

			if (bagBait != null)
				bait = bagBait;
		}

		public static bool ItemAllowedToBeStored(Item item) => AllowedItems.Contains(item.type);

		public static SortedSet<int> AllowedItems => AllowedItemsManager.AllowedItems;
		public static AllowedItemsManager AllowedItemsManager = new(DevCheck, DevWhiteList, DevModWhiteList, DevBlackList, DevModBlackList, ItemGroups, EndWords, SearchWords);
		public AllowedItemsManager GetAllowedItemsManager => AllowedItemsManager;
		protected static bool? DevCheck(ItemSetInfo info, SortedSet<ItemGroup> itemGroups, SortedSet<string> endWords, SortedSet<string> searchWords) {
			if (ItemID.Sets.CanFishInLava[info.Type]
				|| ItemID.Sets.IsLavaBait[info.Type]
				|| ItemID.Sets.IsFishingCrate[info.Type]
				|| ItemID.Sets.IsFishingCrateHardmode[info.Type]) {
				return true;
			}

			return null;
		}
		protected static SortedSet<int> DevWhiteList() {
			SortedSet<int> devWhiteList = new() {
				ItemID.FishBowl,
				ItemID.Fish,
				ItemID.OldShoe,
				ItemID.FishingSeaweed,
				ItemID.TinCan,
				ItemID.AnglerHat,
				ItemID.AnglerVest,
				ItemID.AnglerPants,
				ItemID.HighTestFishingLine,
				ItemID.AnglerEarring,
				ItemID.TackleBox,
				ItemID.FuzzyCarrot,
				ItemID.ScalyTruffle,
				ItemID.LifePreserver,
				ItemID.ShipsWheel,
				ItemID.CompassRose,
				ItemID.WallAnchor,
				ItemID.GoldfishTrophy,
				ItemID.BunnyfishTrophy,
				ItemID.SwordfishTrophy,
				ItemID.SharkteethTrophy,
				ItemID.ShipInABottle,
				ItemID.TreasureMap,
				ItemID.SeaweedPlanter,
				ItemID.PillaginMePixels,
				ItemID.FishCostumeMask,
				ItemID.FishCostumeShirt,
				ItemID.FishCostumeFinskirt,
				ItemID.GingerBeard,
				ItemID.FishFinder,
				ItemID.WeatherRadio,
				ItemID.LockBox,
				ItemID.ObsidianLockbox,
				ItemID.Sextant,
				ItemID.FishermansGuide,
				ItemID.CellPhone,
				ItemID.PDA,
				ItemID.FishFinder,
				ItemID.AnglerTackleBag,
				ItemID.CanOfWorms,
				ItemID.Oyster,
				ItemID.ShuckedOyster,
				ItemID.WhitePearl,
				ItemID.BlackPearl,
				ItemID.PinkPearl,
				ItemID.SharkBait,
				ItemID.ChumBucket,
			};

			foreach (int bugType in RecipeGroup.recipeGroups[RecipeGroupID.Bugs].ValidItems) {
				devWhiteList.Add(bugType);
			}

			foreach (int itemType in RecipeGroup.recipeGroups[RecipeGroupID.Scorpions].ValidItems) {
				devWhiteList.Add(itemType);
			}

			foreach (int itemType in RecipeGroup.recipeGroups[RecipeGroupID.Butterflies].ValidItems) {
				devWhiteList.Add(itemType);
			}

			foreach (int itemType in RecipeGroup.recipeGroups[RecipeGroupID.Fireflies].ValidItems) {
				devWhiteList.Add(itemType);
			}

			foreach (int itemType in RecipeGroup.recipeGroups[RecipeGroupID.Snails].ValidItems) {
				devWhiteList.Add(itemType);
			}

			foreach (int itemType in RecipeGroup.recipeGroups[RecipeGroupID.Dragonflies].ValidItems) {
				devWhiteList.Add(itemType);
			}

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
				ItemGroup.Fish,
				ItemGroup.FishingBait,
				ItemGroup.FishingRods,
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


		#region AndroModItem attributes that you don't need.

		public virtual SellCondition SellCondition => SellCondition.Never;
		public virtual float SellPriceModifier => 1f;
		public override List<WikiTypeID> WikiItemTypes => new() { WikiTypeID.Storage };
		public override string LocalizationTooltip =>
			$"Automatically stores fishing related items such as fish, bait and angler rewards.\n" +
			$"When in your inventory, the contents of the belt are available for crafting.\n" +
			$"Right click to open the belt.\n" +
			$"Bait in the belt is used if the fishing belt is in the first bait item found.\n" +
			$"If any bait in the belt that can be used is favorited, only favorited baits will be used.\n" +
			$"Information accessories that are favorited will show their info as if they are in your inventory.";
		public override string Artist => "anodomani";
		public override string Designer => "andro951";

		#endregion
	}
}
