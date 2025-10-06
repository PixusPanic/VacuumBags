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
using System.Numerics;
using MonoMod.Cil;
using System;
using Mono.Cecil.Cil;
using androLib.UI;
using VacuumBags.Common.Configs;
using static androLib.Items.IBagModItem;

namespace VacuumBags.Items
{
    [Autoload(false)]
	public  class MechanicsToolbelt : AllowedListBagModItem_VB {
		public static IBagModItem Instance {
			get {
				if (instance == null)
					instance = new MechanicsToolbelt();

				return instance;
			}
		}
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<BagToggle>().MechanicsToolbelt;
		}
		
		private static IBagModItem instance;
		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
		public override void SetDefaults() {
            Item.maxStack = 1;
            Item.value = 100000;
			Item.rare = ItemRarityID.Blue;
			Item.width = 32;
            Item.height = 26;
			Item.ammo = Type;
		}
		public override int GetBagType() => ModContent.ItemType<MechanicsToolbelt>();
		public override void AddRecipes() {
			if (!VacuumBags.serverConfig.HarderBagRecipes) {
				CreateRecipe()
				.AddTile(TileID.WorkBenches)
				.AddIngredient(ItemID.Leather, 2)
				.AddIngredient(ItemID.GoldBar, 2)
				.AddIngredient(ItemID.DartTrap, 1)
				.Register();
			}
			else {
				CreateRecipe()
				.AddTile(TileID.HeavyWorkBench)
				.AddIngredient(ItemID.Leather, 5)
				.AddIngredient(ItemID.GoldBar, 10)
				.AddIngredient(ItemID.GoldChest, 1)
				.AddIngredient(ItemID.DartTrap, 1)
				.AddIngredient(ItemID.Wire, 10)
				.Register();
			}
		}
		public override Color PanelColor => new Color(99, 63, 33, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ScrollBarColor => new Color(155, 110, 45, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ButtonHoverColor => new Color(200, 140, 65, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Action SelectItemForUIOnly => () => {
			Player player = Main.LocalPlayer;
			if (WirePlacingTools.Contains(player.HeldItem.type)) {
				ChooseWireFromBelt(player);
			}
			else {
				ChoosePlacableItemFromBelt(player);
			}
		};
		public override bool ShouldUpdateInfoAccessories => true;

		public static Item ChoosePlacableItemFromBelt(Player player) => ChooseFromBag(Instance.BagStorageID, item => item.createTile > -1 || item.IsBucket(), player);

		internal static void On_Player_PutItemInInventoryFromItemUsage(On_Player.orig_PutItemInInventoryFromItemUsage orig, Player self, int type, int theSelectedItem) {
			if (!TryPutItemInBagFromItemUsage(self, type, theSelectedItem))
				orig(self, type, theSelectedItem);
		}
		public static bool TryPutItemInBagFromItemUsage(Player player, int type, int theSelectedItem = -1) {
			int bagItemType = ModContent.ItemType<MechanicsToolbelt>();
			Item bagPlaceItem = BagPlayer.BagPlaceItem(player);
			bool swapped = !bagPlaceItem.NullOrAir() && bagPlaceItem.type == bagItemType;
			if (!swapped && !StorageManager.HasRequiredItemToUseStorageFromBagType(player, bagItemType, out _, out _, out _))
				return false;

			Item contentSampleItem = type.CSI();
			if (!contentSampleItem.IsBucket())
				return false;

			if (!Instance.ItemAllowedToBeStored(contentSampleItem))
				return false;

			BagUI bagUI = StorageManager.BagUIs[Instance.BagStorageID];
			if (!bagUI.CanVacuumItem(contentSampleItem, player, true))
				return false;

			Item[] inv = bagUI.MyStorage.Items;
			for (int i = 0; i < inv.Length; i++) {
				Item item = inv[i];
				if (!item.NullOrAir() && item.stack > 0 && item.type == type && item.stack < item.maxStack) {
					item.stack++;
					return true;
				}
			}

			Item[] inventory = player.inventory;
			if (theSelectedItem >= 0 && (inventory[theSelectedItem].type == ItemID.None || inventory[theSelectedItem].stack <= 0)) {
				inventory[theSelectedItem].SetDefaults(type);
				return true;
			}

			IEnumerable<KeyValuePair<int, Item>> indexItemsPairs = GetFirstFromBag(Instance.BagStorageID, ItemSets.IsBucket, player);
			int slotAfterBag = indexItemsPairs?.Any() == true ? indexItemsPairs.First().Key + 1 : -1;

			int start = slotAfterBag >= 0 && slotAfterBag < inv.Length ? slotAfterBag : 0;
			for (int i = start; i < inv.Length; i++) {
				Item item = inv[i];
				if (item.NullOrAir()) {
					item.SetDefaults(type);
					return true;
				}
			}

			return false;
		}
		public static Item ChooseWireFromBelt(Player player) => ChooseFromBag(Instance.BagStorageID, item => item.type == ItemID.Wire, player);
		public static Item ChooseFromBelt(Player player, int itemType) => ChooseFromBag(Instance.BagStorageID, item => item.type == itemType, player);
		internal static void OnItemCheck_UseWiringTools(ILContext il) {
			//IL_01e3: ldloc.2
			//IL_01e4: ldc.i4.0

			var c = new ILCursor(il);

			//note to self: If need to log the rest of the IL instructions, use this:
			//while (c.Next != null) {
			//	bool catchingExceptions = true;
			//	$"c.Index: {c.Index}, Instruction: {c.Next}".LogSimple();
			//	while (catchingExceptions) {
			//		c.Index++;
			//		try {
			//			if (c.Next != null) {
			//				string tempString = c.Next.ToString();
			//			}

			//			catchingExceptions = false;
			//		}
			//		catch (Exception e) {
			//			$"c.Index: {c.Index}, exception: {e.ToString().Substring(0, 20)}".LogSimple();
			//		}
			//	}
			//}

			if (!c.TryGotoNext(MoveType.After,
				i => i.MatchLdloc(2),
				i => i.MatchLdcI4(0)
			)) { throw new Exception("Failed to find instructions OnItemCheck_UseWiringTools 1/4"); }

			c.Emit(OpCodes.Ldloc_2);
			c.Emit(OpCodes.Ldarg_0);
			c.Emit(OpCodes.Ldarg_1);
			c.Emit(OpCodes.Ldloc_0);
			c.Emit(OpCodes.Ldloc_1);
			c.EmitDelegate((int indexOfWireInInventory, Player player, Item heldItem, int tileTargetX, int tileTargetY) =>
				CheckPlaceWireFromBag(indexOfWireInInventory, player, heldItem, tileTargetX, tileTargetY, WorldGen.PlaceWire));


			//IL_02b7: ldloc.s 4
			//IL_02b9: ldc.i4.0

			if (!c.TryGotoNext(MoveType.After,
				i => i.MatchLdloc(5)//,
				//i => i.MatchLdcI4(0)
			)) { throw new Exception("Failed to find instructions OnItemCheck_UseWiringTools 2/4"); }


			c.Emit(OpCodes.Ldloc, 5);
			c.Emit(OpCodes.Ldarg_0);
			c.Emit(OpCodes.Ldarg_1);
			c.Emit(OpCodes.Ldloc_0);
			c.Emit(OpCodes.Ldloc_1);
			c.EmitDelegate((int indexOfWireInInventory, Player player, Item heldItem, int tileTargetX, int tileTargetY) =>
				CheckPlaceWireFromBag(indexOfWireInInventory, player, heldItem, tileTargetX, tileTargetY, WorldGen.PlaceWire2));


			//IL_0372: ldloc.s 6
			//IL_0374: ldc.i4.0

			if (!c.TryGotoNext(MoveType.After,
				i => i.MatchLdloc(7),
				i => i.MatchLdcI4(0)
			)) { throw new Exception("Failed to find instructions OnItemCheck_UseWiringTools 3/4"); }

			c.Emit(OpCodes.Ldloc, 7);
			c.Emit(OpCodes.Ldarg_0);
			c.Emit(OpCodes.Ldarg_1);
			c.Emit(OpCodes.Ldloc_0);
			c.Emit(OpCodes.Ldloc_1);
			c.EmitDelegate((int indexOfWireInInventory, Player player, Item heldItem, int tileTargetX, int tileTargetY) =>
				CheckPlaceWireFromBag(indexOfWireInInventory, player, heldItem, tileTargetX, tileTargetY, WorldGen.PlaceWire3));


			//IL_042d: ldloc.s 8
			//IL_042f: ldc.i4.0

			if (!c.TryGotoNext(MoveType.After,
				i => i.MatchLdloc(9),
				i => i.MatchLdcI4(0)
			)) { throw new Exception("Failed to find instructions OnItemCheck_UseWiringTools 4/4"); }

			c.Emit(OpCodes.Ldloc, 9);
			c.Emit(OpCodes.Ldarg_0);
			c.Emit(OpCodes.Ldarg_1);
			c.Emit(OpCodes.Ldloc_0);
			c.Emit(OpCodes.Ldloc_1);
			c.EmitDelegate((int indexOfWireInInventory, Player player, Item heldItem, int tileTargetX, int tileTargetY) =>
				CheckPlaceWireFromBag(indexOfWireInInventory, player, heldItem, tileTargetX, tileTargetY, WorldGen.PlaceWire4));
		}
		private static void CheckPlaceWireFromBag(int indexOfWireInInventory, Player player, Item heldItem, int tileTargetX, int tileTargetY, Func<int, int, bool> placeWire) {
			if (indexOfWireInInventory != -1)
				return;

			int mechanicsToolbeltItemType = ModContent.ItemType<MechanicsToolbelt>();
			if (!StorageManager.HasRequiredItemToUseStorageFromBagTypeSlow(Main.LocalPlayer, mechanicsToolbeltItemType))
				return;

			Item wireItem = ChooseWireFromBelt(player);
			if (wireItem == null)
				return;

			if (!placeWire(tileTargetX, tileTargetY))
				return;

			if (ItemLoader.ConsumeItem(wireItem, player))
				wireItem.stack--;

			if (wireItem.stack <= 0)
				wireItem.SetDefaults();

			player.ApplyItemTime(heldItem);
			NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 5, Player.tileTargetX, Player.tileTargetX);
		}
		internal static void OnMassWireOperation(ILContext il) {
			var c = new ILCursor(il);

			// int num = wireCount;
			//IL_0061: ldloc.0

			if (!c.TryGotoNext(MoveType.Before,
				i => i.MatchLdloc(0),
				i => i.MatchLdloc(1)
			)) { throw new Exception("Failed to find instructions OnMassWireOperation 1/4"); }

			c.Emit(OpCodes.Ldloca, 0);
			c.Emit(OpCodes.Ldloca, 1);
			c.EmitDelegate((ref int wireCount, ref int actuatorCount) => {
				int mechanicsToolbeltItemType = ModContent.ItemType<MechanicsToolbelt>();
				if (!StorageManager.HasRequiredItemToUseStorageFromBagTypeSlow(Main.LocalPlayer, mechanicsToolbeltItemType, out _, out _, out _))
					return;

				foreach (Item item in StorageManager.GetItems(Instance.BagStorageID)) {
					if (!item.NullOrAir()) {
						if (item.type == ItemID.Wire) {
							wireCount += item.stack;
						}
						else if (item.type == ItemID.Actuator) {
							actuatorCount += item.stack;
						}
					}
				}
			});
		}

		/// <summary>
		/// From ItemSlot Draw(SpriteBatch spriteBatch, Item[] inv, int context, int slot, Vector2 position, Color lightColor = default(Color)), context 13
		/// </summary>
		public static SortedSet<int> WirePlacingTools = new() {
			ItemID.Wrench,
			ItemID.BlueWrench,
			ItemID.GreenWrench,
			ItemID.YellowWrench,
			ItemID.MulticolorWrench,
			ItemID.WireKite
		};


		public override bool? DevCheck(ItemSetInfo info, SortedSet<ItemGroup> itemGroups, SortedSet<string> endWords, SortedSet<string> searchWords) {
			if (info.Weapon || info.Armor)
				return false;

			if (ItemID.Sets.SortingPriorityWiring[info.Type] != -1)
				return true;

			return null;
		}
		public static List<Func<int>> AdditonalDevWhitelistItems = new();
		public override SortedSet<int> DevWhiteList() {
			SortedSet<int> devWhiteList = new() {
				ItemID.Spike,
				ItemID.Wrench,
				ItemID.WireCutter,
				ItemID.ActiveStoneBlock,
				ItemID.InactiveStoneBlock,
				ItemID.Lever,
				ItemID.Wire,
				ItemID.Switch,
				ItemID.Explosives,
				ItemID.InletPump,
				ItemID.OutletPump,
				ItemID.Actuator,
				ItemID.BlueWrench,
				ItemID.GreenWrench,
				ItemID.LandMine,
				ItemID.WoodenSpike,
				ItemID.Teleporter,
				ItemID.BoosterTrack,
				ItemID.MinecartTrack,
				ItemID.Trapdoor,
				ItemID.TallGate,
				ItemID.Detonator,
				ItemID.ConveyorBeltLeft,
				ItemID.ConveyorBeltRight,
				ItemID.WireKite,
				ItemID.YellowWrench,
				ItemID.WirePipe,
				ItemID.AnnouncementBox,
				ItemID.AnnouncementBox,
				ItemID.ActuationRod,
				ItemID.ActuationAccessory,
				ItemID.MulticolorWrench,
				ItemID.WireBulb,
				ItemID.ProjectilePressurePad,
				ItemID.Grate,
			};

			foreach (int itemType in RecipeGroup.recipeGroups[RecipeGroupID.PressurePlate].ValidItems) {
				devWhiteList.Add(itemType);
			}

			devWhiteList.UnionWith(ItemSets.Buckets);

			devWhiteList.UnionWith(WirePlacingTools);

			devWhiteList.UnionWith(AdditonalDevWhitelistItems.Select(a => a()));

			return devWhiteList;
		}
		public override SortedSet<int> DevBlackList() {
			SortedSet<int> devBlackList = new() {
				ItemID.HoneyBucket,
				ItemID.BottomlessHoneyBucket,
			};

			return devBlackList;
		}
		public override SortedSet<ItemGroup> ItemGroups() {
			SortedSet<ItemGroup> itemGroups = new() {
				ItemGroup.Wiring
			};

			return itemGroups;
		}
		public override SortedSet<string> EndWords() {
			SortedSet<string> endWords = new() {
				"fountain"
			};

			return endWords;
		}

		public override SortedSet<string> SearchWords() {
			SortedSet<string> searchWords = new() {
				"statue",
				"pressureplate",
				"trap",
				"boulder",
				"pump",
				"timer",
				"sponge",
				"logicgate",
				"logicsensor",
				"logicsensor"
			};

			return searchWords;
		}


		#region AndroModItem attributes that you don't need.

		public override string SummaryOfFunction => "Place-able Items, Buckets and Wire";
		public override string LocalizationDisplayName => "Mechanic's Toolbelt";
		public override string LocalizationTooltip =>
			$"Automatically stores wiring related items such as traps and statues.\n" +
			$"When in your inventory, the contents of the belt are available for crafting.\n" +
			$"Right click to open the belt.\n" +
			$"Items can be placed from the belt by left clicking with the belt.  If any items in the belt are favorited, only favorited items will be used.\n" +
			$"Wire in the belt can be used by wiring tools as if it were in your inventory.";
		public override string Artist => "@kingjoshington";
		public override string Designer => "andro951";

		#endregion
	}
}
