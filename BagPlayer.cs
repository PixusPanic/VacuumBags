using androLib;
using androLib.Common.Utility;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using VacuumBags.Items;

namespace VacuumBags
{
	public class BagPlayer : ModPlayer {
		private static bool bagItemChecked = false;
		private static bool bagSmartSelectItemChecked = false;
		private static int lastToolStrategyID(Player player) => player != null ? (int)(typeof(Player).GetField("_lastSmartCursorToolStrategy", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(player) ?? -1) : -1;
		public static ref Item BagPlaceItem(Player player) {
			if (!bagItemChecked)
				bagPlaceItem = GetBagPlaceItem(player);

			return ref bagPlaceItem;
		}
		private static Item bagPlaceItem = null;
		public static ref Item BagSmartSelectItem(Player player, Func<Item, bool> itemCondition) {
			if (!bagSmartSelectItemChecked)
				bagSmartSelectItem = GetBagSmartSelectItem(player, itemCondition);

			return ref bagSmartSelectItem;
		}

		private static Item bagSmartSelectItem = null;
		private static int bagInventoryIndex = Storage.RequiredItemNotFound;

		private static List<KeyValuePair<Func<int>, Func<Player, Item>>> choseFromBagFunctions = new() {
			new(ModContent.ItemType<BuildersBox>, BuildersBox.ChooseItemFromBox),
			new(ModContent.ItemType<WallEr>, WallEr.ChooseItemFromWallEr),
			new(ModContent.ItemType<JarOfDirt>, JarOfDirt.ChooseItemFromJar),
			new(ModContent.ItemType<SlayersSack>, SlayersSack.ChooseRopeFromSack),
			new(ModContent.ItemType<MechanicsToolbelt>, MechanicsToolbelt.ChoosePlacableItemFromBelt)
		};
		private static List<KeyValuePair<Func<int>, Func<Player, Func<Item, bool>, Item>>> chooseFromBagForQuickSelectFunctions = new() {
			new(ModContent.ItemType<SlayersSack>, SlayersSack.ChooseTorchFromSack)
		};

		private static Item GetBagPlaceItem(Player player) {
			bagItemChecked = true;
			int bagType = Main.LocalPlayer.HeldItem.type;
			foreach (KeyValuePair<Func<int>, Func<Player, Item>> choseFromBagPair in choseFromBagFunctions) {
				if (choseFromBagPair.Key() == bagType)
					return choseFromBagPair.Value(player);
			}

			return null;
		}
		private static Item GetBagSmartSelectItem(Player player, Func<Item, bool> itemCondition) {
			bagSmartSelectItemChecked = true;
			foreach (KeyValuePair<Func<int>, Func<Player, Func<Item, bool>, Item>> choseFromBagPair in chooseFromBagForQuickSelectFunctions) {
				int bagType = choseFromBagPair.Key();
				if (StorageManager.HasRequiredItemToUseStorageFromBagType(player, bagType, out bagInventoryIndex)) {
					if (bagInventoryIndex > Storage.RequiredItemNotFound)
						return choseFromBagPair.Value(player, itemCondition);
				}
			}

			return null;
		}
		public override void PostUpdateMiscEffects() {
			BannerBag.PostUpdateMiscEffects(Player);
		}
		public override void ResetEffects() {
			bagPlaceItem = null;
			bagItemChecked = false;
			bagSmartSelectItem = null;
			bagSmartSelectItemChecked = false;
			bagInventoryIndex = Storage.RequiredItemNotFound;
		}

		internal static void OnItemCheck_Inner(On_Player.orig_ItemCheck_Inner orig, Player self) {
			Player player = self;
			DoBagItemSwap(player, () => orig(self), player.selectedItem == player.inventory.Length - 1);
		}
		internal static void On_Main_DrawInterface_40_InteractItemIcon(On_Main.orig_DrawInterface_40_InteractItemIcon orig, Main self) {
			Player player = Main.LocalPlayer;
			DoBagItemSwap(player, () => orig(self));
		}
		internal static void On_SmartCursorHelper_SmartCursorLookup(On_SmartCursorHelper.orig_SmartCursorLookup orig, Player player) {
			DoBagItemSwap(player, () => orig(player));
		}
		internal static void On_PlayerDrawLayers_DrawPlayer_27_HeldItem(On_PlayerDrawLayers.orig_DrawPlayer_27_HeldItem orig, ref PlayerDrawSet drawinfo) {
			Player player = Main.LocalPlayer;
			drawinfo.heldItem = player.HeldItem;
			PlayerDrawSet l = drawinfo;
			DoBagItemSwap(player, () => {
				l.heldItem = player.HeldItem;

				orig(ref l);
			});

			drawinfo = l;
		}
		private static void DoBagItemSwap(Player player, Action orig, bool swapMouseItem = false) {
			bool temp = !player.controlTorch && player.nonTorch > -1 && player.itemAnimation == 0;
			int lastToolStrategy = lastToolStrategyID(player);
			if (lastToolStrategy > ToolStrategyID.None) {
				int tempnonTorch = player.nonTorch;
				int tempselectedItem = player.selectedItem;

				bool swapSmartSelect = BagSmartSelectItem(player, ToolStrategyID.ToolStrategyConditions[lastToolStrategy]) != null;
				if (!swapSmartSelect && lastToolStrategy == ToolStrategyID.Light)
					swapSmartSelect = BagSmartSelectItem(player, ToolStrategyID.ToolStrategyConditions[ToolStrategyID.Light2ndPassGlowStickOnly]) != null;

				if (swapSmartSelect && bagInventoryIndex > Storage.RequiredItemNotFound) {
					if (player.nonTorch == -1)
						player.nonTorch = player.selectedItem;

					player.selectedItem = bagInventoryIndex;
					ref Item beingReplaced = ref player.inventory[player.selectedItem];
					ref Item toReplace = ref bagSmartSelectItem;
					SwapAndCallOriginal(ref beingReplaced, ref toReplace, () => orig(), swapMouseItem);
					return;
				}
			}

			bool swap = BagPlaceItem(player) != null;
			if (swap) {
				ref Item beingReplaced = ref player.inventory[player.selectedItem];
				ref Item toReplace = ref bagPlaceItem;
				SwapAndCallOriginal(ref beingReplaced, ref toReplace, () => orig(), swapMouseItem);
				return;
			}

			orig();
		}

		public static void SwapAndCallOriginal(ref Item beingReplaced, ref Item toReplace, Action orig, bool swapMouseItem = false) {
			Utils.Swap(ref beingReplaced, ref toReplace);

			orig();

			Utils.Swap(ref beingReplaced, ref toReplace);
			if (swapMouseItem && Main.LocalPlayer.itemAnimation == 0)
				Main.mouseItem = beingReplaced.Clone();
		}
	}
}
