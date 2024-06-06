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
using static androLib.Items.IBagModItem;

namespace VacuumBags.Items
{
    [Autoload(false)]
	public class BannerBag : AllowedListBagModItem_VB {
		public static BannerBag Instance {
			get {
				if (instance == null)
					instance = new BannerBag();

				return instance;
			}
		}
		private static BannerBag instance;
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
		public override int GetBagType() => ModContent.ItemType<BannerBag>();
		public override void AddRecipes() {
			if (!VacuumBags.serverConfig.HarderBagRecipes) {
				CreateRecipe()
				.AddTile(TileID.Loom)
				.AddRecipeGroup($"{VacuumBags.ModName}:{VacuumBagSystem.AnyBanner}", 1)
				.AddIngredient(ItemID.Silk, 5)
				.AddRecipeGroup($"{AndroMod.ModName}:{AndroModSystem.GoldOrPlatinumBar}", 2)
				.Register();
			}
			else {
				CreateRecipe()
				.AddTile(TileID.Loom)
				.AddRecipeGroup($"{VacuumBags.ModName}:{VacuumBagSystem.AnyBanner}", 3)
				.AddIngredient(ItemID.Silk, 10)
				.AddRecipeGroup($"{AndroMod.ModName}:{AndroModSystem.GoldOrPlatinumBar}", 10)
				.Register();
			}
		}
		public override Color PanelColor => new Color(140, 140, 160, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ScrollBarColor => new Color(100, 70, 15, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ButtonHoverColor => new Color(240, 240, 240, androLib.Common.Configs.ConfigValues.UIAlpha);
		protected override Action SelectItemForUIOnly => UpdateAllSelectedFromBag;

		public static void UpdateBannersFromHeldBag(ref SceneMetrics sceneMetrics, Player player) {
			if (ActiveBannersFromTileNearbyEffects.Any())
				return;

			if (StorageManager.HasRequiredItemToUseStorageFromBagTypeSlow(player, ModContent.ItemType<BannerBag>()))
				ApplyFirstXBanners(ref sceneMetrics, player, VacuumBags.serverConfig.BannerBagNumberOfBannersInInventory);
		}
		public static IEnumerable<Item> GetBanners(Player player, int firstXBanners) {
			return GetFirstXFromBag(
				Instance.BagStorageID, 
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
			Item[] items = StorageManager.GetItems(Instance.BagStorageID);
			BagUI bagUI = StorageManager.BagUIs[Instance.BagStorageID];
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

		public override SortedSet<int> DevWhiteList() {
			SortedSet<int> devWhiteList = new(ItemSets.AllBanners);

			return devWhiteList;
		}

		#region AndroModItem attributes that you don't need.

		public override string SummaryOfFunction => "Enemy Banners";
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
