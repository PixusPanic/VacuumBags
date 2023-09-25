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
using System.Diagnostics;
using Terraria.Localization;
using VacuumBags.Common.Configs;
using androLib.UI;

namespace VacuumBags.Items
{
    [Autoload(false)]
	public class BannerBag : BagModItem, ISoldByWitch, INeedsSetUpAllowedList
	{
		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
		public override void SetDefaults() {
            Item.maxStack = 1;
            Item.value = 100000;
			Item.rare = ItemRarityID.Blue;
			Item.width = 20;
            Item.height = 48;
			Item.createTile = ModContent.TileType<Tiles.BannerBag>();
			Item.consumable = true;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 10;
			Item.useAnimation = 15;
			Item.useTurn = true;
		}
        public override void AddRecipes() {
			if (!VacuumBags.serverConfig.HarderBagRecipes) {
				CreateRecipe()
				.AddTile(TileID.Loom)
				.AddRecipeGroup($"{VacuumBags.ModName}:{VacuumBagSystem.AnyBanner}", 1)
				.AddIngredient(ItemID.Silk, 5)
				.AddIngredient(ItemID.GoldBar, 2)
				.Register();
			}
			else {
				CreateRecipe()
				.AddTile(TileID.Loom)
				.AddRecipeGroup($"{VacuumBags.ModName}:{VacuumBagSystem.AnyBanner}", 3)
				.AddIngredient(ItemID.Silk, 10)
				.AddIngredient(ItemID.GoldBar, 10)
				.Register();
			}
		}

		public static int BagStorageID;//Set this when registering with androLib.
		public static void CloseBag() => StorageManager.CloseBag(BagStorageID);
		new public static Color PanelColor => new Color(140, 140, 160, androLib.Common.Configs.ConfigValues.UIAlpha);
		new public static Color ScrollBarColor => new Color(100, 70, 15, androLib.Common.Configs.ConfigValues.UIAlpha);
		new public static Color ButtonHoverColor => new Color(240, 240, 240, androLib.Common.Configs.ConfigValues.UIAlpha);
		public static void RegisterWithAndroLib(Mod mod) {
			BagStorageID = StorageManager.RegisterVacuumStorageClass(
				mod,//Mod
				typeof(BannerBag),//type 
				ItemAllowedToBeStored,//Is allowed function, Func<Item, bool>
				null,//Localization Key name.  Attempts to determine automatically by treating the type as a ModItem, or you can specify.
				100,//StorageSize
				true,//Can vacuum
				() => PanelColor, // Get color function. Func<using Microsoft.Xna.Framework.Color>
				() => ScrollBarColor, // Get Scroll bar color function. Func<using Microsoft.Xna.Framework.Color>
				() => ButtonHoverColor, // Get Button hover color function. Func<using Microsoft.Xna.Framework.Color>
				() => ModContent.ItemType<BannerBag>(),//Get ModItem type
				80,//UI Left
				675,//UI Top
				() => AllowedItems,
				false,
				() => UpdateAllSelectedFromBag()
			);
		}
		public static bool ItemAllowedToBeStored(Item item) => AllowedItems.Contains(item.type);

		public static void UpdateBannersFromHeldBag(ref SceneMetrics sceneMetrics, Player player) {
			if (ActiveBannersFromTileNearbyEffects.Any())
				return;

			if (StorageManager.HasRequiredItemToUseStorageFromBagTypeSlow(player, ModContent.ItemType<BannerBag>()))
				ApplyFirstXBanners(ref sceneMetrics, player, VacuumBags.serverConfig.BannerBagNumberOfBannersInInventory);
		}
		public static IEnumerable<Item> GetBanners(Player player, int firstXBanners) {
			return GetFirstXFromBag(
				BagStorageID, 
				(Item item) => {
					if (!item.IsBanner(out int banner))
						return false;

					return !Main.SceneMetrics.NPCBannerBuff[banner] || ActiveBannersFromTileNearbyEffects.Contains(banner);
				},
				player, firstXBanners);
		}
		public static void ApplyFirstXBanners(ref SceneMetrics sceneMetrics, Player player, int firstXBanners, bool fromTileNearbyEffects = false) {
			if (firstXBanners == 0)
				return;
			
			IEnumerable<int> banners = GetBanners(player, firstXBanners).Where(item => item.IsBanner()).Select(item => ItemSets.ItemToBanner[item.type]);
			if (fromTileNearbyEffects || !ActiveBannersFromTileNearbyEffects.Any())
				ActiveBannersFromTileNearbyEffects = banners.ToList();

			foreach (int banner in banners) {
				sceneMetrics.NPCBannerBuff[banner] = true;
				sceneMetrics.hasBanner = true;
			}
		}
		private static void UpdateAllSelectedFromBag() {
			Item[] items = StorageManager.GetItems(BagStorageID);
			BagUI bagUI = StorageManager.BagUIs[BagStorageID];
			for (int i = 0; i < items.Length; i++) {
				Item item = items[i];
				if (item.NullOrAir())
					continue;

				if (item.IsBanner(out int banner) && Main.SceneMetrics.NPCBannerBuff[banner]) {
					int context = ActiveBannersFromTileNearbyEffects.Contains(banner) ? ItemSlotContextID.YellowSelected : ItemSlotContextID.Purple;
					bagUI.AddSelectedItemSlot(i, context);
				}
			}
		}
		public static void PostUpdateMiscEffects(Player player) {
			if (ActiveBannersFromTileNearbyEffects.Any())
				player.AddBuff(BuffID.MonsterBanner, -1);
		}
		public static List<int> ActiveBannersFromTileNearbyEffects = new();
		public static bool UpdateFromPlacedTile = false;
		internal static void PreScanAndExportToMain() {
			ActiveBannersFromTileNearbyEffects = new();
		}
		internal static void PostScanAndExportToMain(ref SceneMetrics sceneMetrics) {
			if (UpdateFromPlacedTile) {
				ApplyFirstXBanners(ref sceneMetrics, Main.LocalPlayer, VacuumBags.serverConfig.BannerBagNumberOfBannersWhenPlaced, true);
				UpdateFromPlacedTile = false;
			}
			else {
				UpdateBannersFromHeldBag(ref sceneMetrics, Main.LocalPlayer);
			}
		}

		public static SortedSet<int> AllowedItems => AllowedItemsManager.AllowedItems;
		public static AllowedItemsManager AllowedItemsManager = new(ModContent.ItemType<BannerBag>, () => BagStorageID, DevCheck, DevWhiteList, DevModWhiteList, DevBlackList, DevModBlackList, ItemGroups, EndWords, SearchWords);
		public AllowedItemsManager GetAllowedItemsManager => AllowedItemsManager;
		protected static bool? DevCheck(ItemSetInfo info, SortedSet<ItemGroup> itemGroups, SortedSet<string> endWords, SortedSet<string> searchWords) {
			return null;
		}
		protected static SortedSet<int> DevWhiteList() {
			SortedSet<int> devWhiteList = new(ItemSets.AllBanners);

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

		#region AndroModItem attributes that you don't need.

		public virtual SellCondition SellCondition => SellCondition.Never;
		public virtual float SellPriceModifier => 1f;
		public override List<WikiTypeID> WikiItemTypes => new() { WikiTypeID.Storage };
		public override string LocalizationTooltip =>
			$"Automatically stores banners.\n" +
			$"When in your inventory, the contents of the bag are available for crafting.\n" +
			$"Right click to open the bag.\n" +
			$"The bag provides up to {BagsServerConfig.BannerBagNumberOfBannersInInventoryDefault} banner effects by default when in your inventory.\n" +
			$"When placed, it provides all of your banner effects by default, but in the normal banner range.  Each banner bag shares it's inventory.\n" +
			$"When selecting banner buffs to give you, banners will be ignored if you already have them nearby.\n" +
			$"The background of banners is yellow when there buff is being granted by the banner bag.\n" +
			$"The background is purple if you already have the banner buff from another source.";
		public override string Artist => "@kingjoshington";
		public override string Designer => "andro951";

		#endregion
	}
}
