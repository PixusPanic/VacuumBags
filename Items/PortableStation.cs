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
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace VacuumBags.Items
{
    [Autoload(false)]
	public  class PortableStation : BagModItem, INeedsSetUpAllowedList
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
		protected static int DefaultBagSize => 100;
		public static void RegisterWithAndroLib(Mod mod) {
			BagStorageID = StorageManager.RegisterVacuumStorageClass(
				mod,//Mod
				typeof(PortableStation),//type 
				ItemAllowedToBeStored,//Is allowed function, Func<Item, bool>
				null,//Localization Key name.  Attempts to determine automatically by treating the type as a ModItem, or you can specify.
				-DefaultBagSize,//StorageSize
				true,//Can vacuum
				() => PanelColor, // Get color function. Func<using Microsoft.Xna.Framework.Color>
				() => ScrollBarColor, // Get Scroll bar color function. Func<using Microsoft.Xna.Framework.Color>
				() => ButtonHoverColor, // Get Button hover color function. Func<using Microsoft.Xna.Framework.Color>
				() => ModContent.ItemType<PortableStation>(),//Get ModItem type
				80,//UI Left
				675,//UI Top
				UpdateAllowedList,
				false,
				() => UpdateAllSelectedFromBag(Main.SceneMetrics)
			);
		}
		public static bool ItemAllowedToBeStored(Item item) => AllowedItems.Contains(item.type);
		public static void UpdateAllSelectedFromBag(SceneMetrics sceneMetrics) {
			Player player = Main.LocalPlayer;
			Item[] items = StorageManager.GetItems(BagStorageID);
			BagUI bagUI = StorageManager.BagUIs[BagStorageID];
			for (int i = 0; i < items.Length; i++) {
				Item item = items[i];
				if (item.NullOrAir())
					continue;

				if (item.IsRequiredTile() && item.createTile > -1 && item.createTile < player.adjTile.Length && player.adjTile[item.createTile]) {
					int context = ActiveStationsFromPortableStation.Contains(item.createTile) ? ItemSlotContextID.YellowSelected : ItemSlotContextID.Purple;
					bagUI.AddSelectedItemSlot(i, context);
				}

				if (item.PassiveBuffTileIsActive(sceneMetrics)) {
					int context = ActiveBuffsFromTileNearbyEffects.ContainsKey(item.type) ? ItemSlotContextID.BrightGreenSelected : ItemSlotContextID.Purple;
					bagUI.AddSelectedItemSlot(i, context);
				}

				if (item.IsActiveBuffTileAndHasBuff(player))
					bagUI.AddSelectedItemSlot(i, ItemSlotContextID.BrightGreenSelected);
			}
		}

		#region Crafting Stations

		private static void UpdateStationsFromHeldPortableStation(Player player) {
			if (!Main.playerInventory || ActiveStationsFromPortableStation.Any())
				return;

			if (StorageManager.HasRequiredItemToUseStorageFromBagTypeSlow(player, ModContent.ItemType<PortableStation>()))
				ApplyFirstXStationTiles(player, VacuumBags.serverConfig.PortableStationNumberOfStationsInInventory);
		}
		public static IEnumerable<Item> GetStations(Player player, int firstXBanners) {
			return GetFirstXFromBag(
				BagStorageID,
				(Item item) => item.IsRequiredTile() && item.createTile < player.adjTile.Length,
				player, firstXBanners);
		}
		public static void ApplyFirstXStationTiles(Player player, int firstXStationTiles, bool fromTileNearbyEffects = false) {
			if (firstXStationTiles == 0)
				return;

			IEnumerable<int> stations = GetStations(player, firstXStationTiles).Select(item => item.createTile);
			SortedSet<int> adjTiles = new(stations);
			foreach (int station in stations) {
				if (station < TileID.Count)
					continue;

				ModTile modTile = TileLoader.GetTile(station);
				if (modTile is null)
					continue;

				int[] tiles = modTile.AdjTiles;
				for (int i = 0; i < tiles.Length; i++) {
					adjTiles.Add(tiles[i]);
				}
			}

			if (fromTileNearbyEffects || !ActiveStationsFromPortableStation.Any())
				ActiveStationsFromPortableStation = adjTiles;

			foreach (int station in adjTiles) {
				if (!player.adjTile[station]) {
					player.adjTile[station] = true;
					Recipe.FindRecipes(true);
				}
			}
		}
		public static SortedSet<int> ActiveStationsFromPortableStation = new();
		internal static void OnAdjTiles(ILContext il) {
			var c = new ILCursor(il);

			c.EmitDelegate(() => { ActiveStationsFromPortableStation = new(); return; });

			//IL_03bc: ldsfld bool Terraria.Main::playerInventory
			//IL_03c1: brtrue.s IL_03c4

			if (!c.TryGotoNext(MoveType.Before,
				i => i.MatchLdsfld(typeof(Main), nameof(Main.playerInventory)),
				i => i.MatchBrtrue(out _)
			)) { throw new Exception("Failed to find instructions OnAdjTiles 2/2"); }

			c.Emit(OpCodes.Ldarg_0);

			c.EmitDelegate((Player player) => {
				UpdateStationsFromHeldPortableStation(player);
			});
		}

		#endregion

		#region Passive Buff Tiles

		public static void UpdatePassiveBuffsFromHeldBag(ref SceneMetrics sceneMetrics, Player player) {
			if (ActiveBuffsFromTileNearbyEffects.Any())
				return;

			if (StorageManager.HasRequiredItemToUseStorageFromBagTypeSlow(player, ModContent.ItemType<PortableStation>()))
				ApplyFirstXPassiveBuffs(ref sceneMetrics, player, VacuumBags.serverConfig.PortableStationNumberOfPassiveBuffStationsInInventory);
		}
		public static IEnumerable<Item> GetPassiveBuffsStations(SceneMetrics sceneMetrics, Player player, int firstXBuffs) {
			return GetFirstXFromBag(
				BagStorageID,
				(Item item) => {
					if (!item.IsPassiveBuffTile())
						return false;

					if (item.IsPassiveBuffCandle() && !item.favorited)
						return false;

					if (VacuumBags.clientConfig.PortableStationPassiveBuffsOnlyActiveIfFavorited && !item.favorited)
						return false;

					bool alreadyActive = item.PassiveBuffTileIsActive(sceneMetrics);
					bool fromLast = ActiveBuffsFromTileNearbyEffects.ContainsKey(item.type);
					bool result = !alreadyActive || fromLast;
					return result;
				},
				player,
				firstXBuffs,
				(Item item) => item.IsPassiveBuffCandle(),
				context: ItemSlotContextID.BrightGreenSelected
			);
		}
		public static void ApplyFirstXPassiveBuffs(ref SceneMetrics sceneMetrics, Player player, int firstXPassiveBuffs) {
			if (firstXPassiveBuffs == 0)
				return;
			
			IEnumerable<KeyValuePair<int, Action<SceneMetrics>>> passiveBuffs = GetPassiveBuffsStations(sceneMetrics, player, firstXPassiveBuffs).Where(item => item.IsPassiveBuffTile())
				.Select(item => new KeyValuePair<int, Action<SceneMetrics>>(item.type, ItemSets.PassiveBuffTileEffects[item.type]));
			foreach (KeyValuePair<int, Action<SceneMetrics>> pair in passiveBuffs) {
				ActiveBuffsFromTileNearbyEffects.TryAdd(pair.Key, pair.Value);
			}
		}
		public static SortedDictionary<int, Action<SceneMetrics>> ActiveBuffsFromTileNearbyEffects = new ();
		public static bool UpdateFromPlacedTile = false;
		internal static void PreScanAndExportToMain() {
			ActiveBuffsFromTileNearbyEffects.Clear();
			if (Main.LocalPlayer.TryGetModPlayer(out BagPlayer bagPlayer))
				bagPlayer.NearPortableStation = false;
		}
		internal static void PostScanAndExportToMain(ref SceneMetrics sceneMetrics) {
			if (UpdateFromPlacedTile) {
				ApplyFirstXPassiveBuffs(ref sceneMetrics, Main.LocalPlayer, VacuumBags.serverConfig.PortableStationNumberOfPassiveBuffStationsWhenPlaced);
				UpdateFromPlacedTile = false;
			}
			else {
				UpdatePassiveBuffsFromHeldBag(ref sceneMetrics, Main.LocalPlayer);
			}

			foreach (Action<SceneMetrics> passiveBuff in ActiveBuffsFromTileNearbyEffects.Select(p => p.Value)) {
				passiveBuff(sceneMetrics);
			}
		}

		#endregion

		#region Active Buff Tiles

		public static void OnRightClickTile() {
			if (!VacuumBags.serverConfig.PortableStationsActivateActiveBuffsWhenOpened)
				return;

			Player player = Main.LocalPlayer;
			IEnumerable<Item> activeBuffStationItems = GetAllFromBag(
				BagStorageID,
				ItemSets.IsActiveBuffTile,
				player,
				ItemSlotContextID.BrightGreenSelected
			);

			foreach (Item item in activeBuffStationItems) {
				if (item.IsActiveBuffTile(out Action<Player> buff)) {
					if (!VacuumBags.clientConfig.SilencePortableStationActiveBuffs && !item.HasActiveTileBuff(player))
						item.PlayActiveBuffTileSound(player);

					buff(player);
				}
			}
		}

		#endregion


		private static void UpdateAllowedList(int item, bool add) {
			if (add) {
				AllowedItems.Add(item);
			}
			else {
				AllowedItems.Remove(item);
			}
		}
		public static SortedSet<int> AllowedItems => AllowedItemsManager.AllowedItems;
		public static AllowedItemsManager AllowedItemsManager = new(ModContent.ItemType<PortableStation>, () => BagStorageID, DevCheck, DevWhiteList, DevModWhiteList, DevBlackList, DevModBlackList, ItemGroups, EndWords, SearchWords);
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
				ItemID.ShadowCandle,
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

		public override string SummaryOfFunction => "Crafting Stations, Passive and Active buff stations";
		public override string LocalizationTooltip =>
			$"Automatically stores crafting stations and buff stations.\n" +
			$"When in your inventory, the contents of the station are available for crafting.\n" +
			$"Right click to open the station.\n" +
			$"When in your inventory, the Portable Station provides up to {BagsServerConfig.PortableStationNumberOfCraftingStationsInInventoryDefault} station for crafting by default.\n" +
			$"When placed, it provides all of your stations for crafting by default, but in the normal tile range.  Each Portable Station shares it's inventory.\n" +
			$"The background of stations is yellow when they are being provided for crafting by the Portable Station.\n" +
			$"The background is purple if there is already that type of station nearby, however it will not automatically skip stations you are near when selecting\n" +
			$"stations from the Portable Station because more is not necessarily better with crafting recipes.\n\n" +
			$"When in your inventory, the Portable station provides up to {BagsServerConfig.PortableStationNumberOfPassiveBuffStationsInInventoryDefault} buff station buffs to you by default.\n" +
			$"When placed, it provides all of your passive buff station buffs by default, but in the normal buff range.\n" +
			$"Items that are providing their buff will be bright green.  Buffs that you already have from nearby tiles will be skipped and have a purple background.\n\n" +
			$"Opening the table while it is placed will make you interact with all of the activate buff stations, causing you to gain their buffs.";
		public override string Artist => "@kingjoshington";
		public override string Designer => "andro951";

		#endregion
	}
}
