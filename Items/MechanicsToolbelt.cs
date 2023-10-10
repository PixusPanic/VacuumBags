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

namespace VacuumBags.Items
{
    [Autoload(false)]
	public  class MechanicsToolbelt : BagModItem, INeedsSetUpAllowedList
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

		public static int BagStorageID;//Set this when registering with androLib.


		public static void RegisterWithAndroLib(Mod mod) {
			BagStorageID = StorageManager.RegisterVacuumStorageClass(
				mod,//Mod
				typeof(MechanicsToolbelt),//type 
				ItemAllowedToBeStored,//Is allowed function, Func<Item, bool>
				null,//Localization Key name.  Attempts to determine automatically by treating the type as a ModItem, or you can specify.
				100,//StorageSize
				true,//Can vacuum
				() => new Color(99, 63, 33, androLib.Common.Configs.ConfigValues.UIAlpha),    // Get color function. Func<using Microsoft.Xna.Framework.Color>
				() => new Color(155, 110, 45, androLib.Common.Configs.ConfigValues.UIAlpha),    // Get Scroll bar color function. Func<using Microsoft.Xna.Framework.Color>
				() => new Color(200, 140, 65, androLib.Common.Configs.ConfigValues.UIAlpha),    // Get Button hover color function. Func<using Microsoft.Xna.Framework.Color>
				() => ModContent.ItemType<MechanicsToolbelt>(),//Get ModItem type
				80,//UI Left
				675,//UI Top
				() => AllowedItems,
				false,
				() => {
					Player player = Main.LocalPlayer;
					if (WirePlacingTools.Contains(player.HeldItem.type)) {
						ChooseWireFromBelt(player);
					}
					else {
						ChoosePlacableItemFromBelt(player);
					}
				}
			);
		}
		public static bool ItemAllowedToBeStored(Item item) => AllowedItems.Contains(item.type);
		public static Item ChoosePlacableItemFromBelt(Player player) => ChooseFromBag(BagStorageID, item => item.createTile > -1 || item.IsBucket(), player);

		internal static void On_Player_PutItemInInventoryFromItemUsage(On_Player.orig_PutItemInInventoryFromItemUsage orig, Player self, int type, int theSelectedItem) {
			if (!TryPutItemInBagFromItemUsage(self, type, theSelectedItem))
				orig(self, type, theSelectedItem);
		}
		public static bool TryPutItemInBagFromItemUsage(Player player, int type, int theSelectedItem = -1) {
			if (!StorageManager.HasRequiredItemToUseStorageFromBagType(player, BagStorageID, out _))
				return false;

			Item contentSampleItem = type.CSI();
			if (!contentSampleItem.IsBucket())
				return false;

			if (!ItemAllowedToBeStored(contentSampleItem))
				return false;

			BagUI bagUI = StorageManager.BagUIs[BagStorageID];
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
			if (theSelectedItem >= 0 && (inventory[theSelectedItem].type == 0 || inventory[theSelectedItem].stack <= 0)) {
				inventory[theSelectedItem].SetDefaults(type);
				return true;
			}

			IEnumerable<KeyValuePair<int, Item>> indexItemsPairs = GetFirstFromBag(BagStorageID, ItemSets.IsBucket, player);
			int slotAfterBag = indexItemsPairs.Any() ? indexItemsPairs.First().Key + 1 : -1;

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
		public static Item ChooseWireFromBelt(Player player) => ChooseFromBag(BagStorageID, item => item.type == ItemID.Wire, player);
		internal static void OnItemCheck_UseWiringTools(ILContext il) {
			//IL_01e3: ldloc.2
			//IL_01e4: ldc.i4.0

			var c = new ILCursor(il);
			if (!c.TryGotoNext(MoveType.After,
				i => i.MatchLdloc(2),
				i => i.MatchLdcI4(0)
			)) { throw new Exception("Failed to find instructions OnItemCheck_UseWiringTools 1/1"); }

			c.Emit(OpCodes.Ldloc_2);
			c.Emit(OpCodes.Ldarg_0);
			c.Emit(OpCodes.Ldarg_1);
			c.Emit(OpCodes.Ldloc_0);
			c.Emit(OpCodes.Ldloc_1);


			c.EmitDelegate((int indexOfWireInInventory, Player player, Item heldItem, int tileTargetX, int tileTargetY) => {
				if (indexOfWireInInventory != -1)
					return;

				int mechanicsToolbeltItemType = ModContent.ItemType<MechanicsToolbelt>();
				if (!StorageManager.HasRequiredItemToUseStorageFromBagTypeSlow(Main.LocalPlayer, mechanicsToolbeltItemType))
					return;

				Item wireItem = ChooseWireFromBelt(player);
				if (wireItem == null)
					return;

				if (!WorldGen.PlaceWire(tileTargetX, tileTargetY))
					return;

				if (ItemLoader.ConsumeItem(wireItem, player))
					wireItem.stack--;

				if (wireItem.stack <= 0)
					wireItem.SetDefaults();

				player.ApplyItemTime(heldItem);
				NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 5, Player.tileTargetX, Player.tileTargetX);
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

		public static SortedSet<int> AllowedItems => AllowedItemsManager.AllowedItems;
		public static AllowedItemsManager AllowedItemsManager = new(ModContent.ItemType<MechanicsToolbelt>, () => BagStorageID, DevCheck, DevWhiteList, DevModWhiteList, DevBlackList, DevModBlackList, ItemGroups, EndWords, SearchWords);
		public AllowedItemsManager GetAllowedItemsManager => AllowedItemsManager;
		protected static bool? DevCheck(ItemSetInfo info, SortedSet<ItemGroup> itemGroups, SortedSet<string> endWords, SortedSet<string> searchWords) {
			if (info.Weapon || info.Armor)
				return false;

			if (ItemID.Sets.SortingPriorityWiring[info.Type] != -1)
				return true;

			return null;
		}
		protected static SortedSet<int> DevWhiteList() {
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

			return devWhiteList;
		}
		protected static SortedSet<string> DevModWhiteList() {
			SortedSet<string> devModWhiteList = new() {

			};

			return devModWhiteList;
		}
		protected static SortedSet<int> DevBlackList() {
			SortedSet<int> devBlackList = new() {
				ItemID.HoneyBucket,
				ItemID.BottomlessHoneyBucket,
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
				ItemGroup.Wiring
			};

			return itemGroups;
		}
		protected static SortedSet<string> EndWords() {
			SortedSet<string> endWords = new() {
				"fountain"
			};

			return endWords;
		}

		protected static SortedSet<string> SearchWords() {
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
