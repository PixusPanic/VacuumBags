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
using VacuumBags.Common.Configs;

namespace VacuumBags.Items
{
	[Autoload(false)]
	public  class PortableStation : BagModItem, ISoldByWitch, INeedsSetUpAllowedList
	{
		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
		public override void SetDefaults() {
            Item.maxStack = 1;
            Item.value = 100000;
			Item.rare = ItemRarityID.Blue;
			Item.width = 32;
            Item.height = 30;
			Item.createTile = ModContent.TileType<Tiles.PortableStation>();
			Item.consumable = true;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 10;
			Item.useAnimation = 15;
			Item.useTurn = true;
		}
        public override void AddRecipes() {
			if (!VacuumBags.serverConfig.HarderBagRecipes) {
				CreateRecipe()
				.AddRecipeGroup($"{AndroMod.ModName}:{AndroModSystem.Workbenches}", 1)
				.AddIngredient(ItemID.Chest, 1)
				.AddIngredient(ItemID.Rope, 20)
				.Register();
			}
			else {
				CreateRecipe()
				.AddTile(TileID.Anvils)
				.AddIngredient(ItemID.HeavyWorkBench, 1)
				.AddIngredient(ItemID.Chain, 5)
				.AddIngredient(ItemID.GoldChest, 2)
				.Register();
			}
		}

		public static int BagStorageID;//Set this when registering with androLib.
		public static void CloseBag() => StorageManager.CloseBag(BagStorageID);
		new public static Color PanelColor => new Color(127, 92, 69, androLib.Common.Configs.ConfigValues.UIAlpha);
		new public static Color ScrollBarColor => new Color(30, 40, 102, androLib.Common.Configs.ConfigValues.UIAlpha);
		new public static Color ButtonHoverColor => new Color(160, 110, 80, androLib.Common.Configs.ConfigValues.UIAlpha);
		public static void RegisterWithAndroLib(Mod mod) {
			BagStorageID = StorageManager.RegisterVacuumStorageClass(
				mod,//Mod
				typeof(PortableStation),//type 
				ItemAllowedToBeStored,//Is allowed function, Func<Item, bool>
				null,//Localization Key name.  Attempts to determine automatically by treating the type as a ModItem, or you can specify.
				100,//StorageSize
				true,//Can vacuum
				() => PanelColor, // Get color function. Func<using Microsoft.Xna.Framework.Color>
				() => ScrollBarColor, // Get Scroll bar color function. Func<using Microsoft.Xna.Framework.Color>
				() => ButtonHoverColor, // Get Button hover color function. Func<using Microsoft.Xna.Framework.Color>
				() => ModContent.ItemType<PortableStation>(),//Get ModItem type
				80,//UI Left
				675,//UI Top
				() => UpdateAllSelectedFromBag()
			);
		}
		public static bool ItemAllowedToBeStored(Item item) => AllowedItems.Contains(item.type);

		private static void UpdateStationsFromHeldPortableStation(Player player) {
			if (ActiveStationsFromPortableStation.Any())
				return;

			if (StorageManager.HasRequiredItemToUseStorageFromBagType(player, ModContent.ItemType<PortableStation>(), out _))
				ApplyFirstXStationTiles(player, VacuumBags.serverConfig.PortableStationNumberOfStationsInInventory);
		}
		public static IEnumerable<Item> GetStations(Player player, int firstXBanners) {
			return GetFirstXFromBag(
				BagStorageID,
				(Item item) => item.createTile > -1 && item.createTile < player.adjTile.Length,
				player, firstXBanners);
		}
		public static void ApplyFirstXStationTiles(Player player, int firstXStationTiles, bool fromTileNearbyEffects = false) {
			if (firstXStationTiles == 0)
				return;

			IEnumerable<int> stations = GetStations(player, firstXStationTiles).Select(item => item.createTile);
			if (fromTileNearbyEffects || !ActiveStationsFromPortableStation.Any())
				ActiveStationsFromPortableStation = stations.ToList();

			foreach (int station in stations) {
				if (!player.adjTile[station]) {
					player.adjTile[station] = true;
					Recipe.FindRecipes(true);
				}
			}

			UpdateAllSelectedFromBag();
		}
		public static void UpdateAllSelectedFromBag() {
			Player player = Main.LocalPlayer;
			Item[] items = StorageManager.GetItems(BagStorageID);
			BagUI bagUI = StorageManager.BagUIs[BagStorageID];
			for (int i = 0; i < items.Length; i++) {
				Item item = items[i];
				if (item.NullOrAir())
					continue;

				int station = item.createTile;
				if (station < 0 || station >= player.adjTile.Length)
					continue;

				if (player.adjTile[station]) {
					int context = ActiveStationsFromPortableStation.Contains(station) ? ItemSlotContextID.YellowSelected : ItemSlotContextID.Purple;
					bagUI.AddSelectedItemSlot(i, context);
				}
			}
		}
		public static List<int> ActiveStationsFromPortableStation = new();
		internal static void OnAdjTiles(On_Player.orig_AdjTiles orig, Player self) {
			ActiveStationsFromPortableStation = new();
			orig(self);
			UpdateStationsFromHeldPortableStation(self);
		}

		public static SortedSet<int> AllowedItems => AllowedItemsManager.AllowedItems;
		public static AllowedItemsManager AllowedItemsManager = new(DevCheck, DevWhiteList, DevModWhiteList, DevBlackList, DevModBlackList, ItemGroups, EndWords, SearchWords);
		public AllowedItemsManager GetAllowedItemsManager => AllowedItemsManager;
		protected static bool? DevCheck(ItemSetInfo info, SortedSet<ItemGroup> itemGroups, SortedSet<string> endWords, SortedSet<string> searchWords) {
			if (info.Equipment)
				return false;

			if (info.RequiredTile)
				return true;

			return null;
		}
		protected static SortedSet<int> DevWhiteList() {
			SortedSet<int> devWhiteList = new() {
				ItemID.Furnace,
				ItemID.IronAnvil,
				ItemID.WorkBench,
				ItemID.Tombstone,
				ItemID.Loom,
				ItemID.Sawmill,
				ItemID.TinkerersWorkshop,
				ItemID.CrystalBall,
				ItemID.AdamantiteForge,
				ItemID.MythrilAnvil,
				ItemID.FleshGrinder,
				ItemID.MeatGrinder,
				ItemID.Extractinator,
				ItemID.Solidifier,
				ItemID.BlendOMatic,
				ItemID.DyeVat,
				ItemID.Obelisk,
				ItemID.LihzahrdAltar,
				ItemID.ImbuingStation,
				ItemID.Autohammer,
				ItemID.Cauldron,
				ItemID.HeavyWorkBench,
				ItemID.AmmoBox,
				ItemID.BewitchingTable,
				ItemID.Campfire,
				ItemID.HeartLantern,
				ItemID.Sundial,
				ItemID.SharpeningStation,
				ItemID.TargetDummy,
				ItemID.CatBast,
				ItemID.GardenGnome,
				ItemID.WarTable,
				ItemID.StarinaBottle,
				ItemID.PeaceCandle,
				ItemID.WaterCandle,
				ItemID.HoneyBucket,
				ItemID.BottomlessHoneyBucket,
				ItemID.Sunflower,
				ItemID.SliceOfCake,
				ItemID.SliceOfCake
			};

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
				ItemGroup.CraftingObjects
			};

			return itemGroups;
		}
		protected static SortedSet<string> EndWords() {
			SortedSet<string> endWords = new() {
				"campfire"
			};

			return endWords;
		}

		protected static SortedSet<string> SearchWords() {
			SortedSet<string> searchWords = new() {
				"anvil",
				"furnace",
				"workbench",
				"bookcase",
				"gravemarker",
				"headstone",
				"gravestone",
			};

			return searchWords;
		}


		#region AndroModItem attributes that you don't need.

		public virtual SellCondition SellCondition => SellCondition.Never;
		public virtual float SellPriceModifier => 1f;
		public override List<WikiTypeID> WikiItemTypes => new() { WikiTypeID.Storage };
		public override string LocalizationTooltip =>
			$"Automatically stores crafting stations and buff stations.\n" +
			$"When in your inventory, the contents of the station are available for crafting.\n" +
			$"Right click to open the station.\n" +
			$"When in your inventory, the Portable Station provinces up to {BagsServerConfig.BannerBagNumberOfBannersInInventoryDefault} station for crafting by default when in your inventory.\n" +
			$"When placed, it provides all of your stations for crafting by default, but in the normal tile range.  Each Portable Station shares it's inventory.\n" +
			$"The background of stations is yellow when they are being provided for crafting by the Portable Station.\n" +
			$"The background is purple if there is already that type of station nearby, however it will not automatically skip stations you are near when selecting\n" +
			$"stations from the Portable Station because more is not necessarily better with crafting recipes. you already have the banner buff from another source.";
		public override string Artist => "@kingjoshington";
		public override string Designer => "andro951";

		#endregion
	}
}
