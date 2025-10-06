using androLib;
using androLib.Common.Utility;
using androLib.UI;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using VacuumBags.Common.Configs;
using VacuumBags.Common.Globals;
using VacuumBags.Items;

namespace VacuumBags
{
	public class BagPlayer : ModPlayer {

		#region General Overrides

		public override void Load() {
			#region Loading toggles
			// This is so the game doesn't crash whenever it's trying to access something that's not loaded
			if (ModContent.GetInstance<BagToggle>().BuildersBox)
				choseFromBagFunctions.Add(new KeyValuePair<Func<int>, Func<Player, Item>>(ModContent.ItemType<BuildersBox>, BuildersBox.ChooseItemFromBox));
			if (ModContent.GetInstance<BagToggle>().WallEr)
				choseFromBagFunctions.Add(new KeyValuePair<Func<int>, Func<Player, Item>>(ModContent.ItemType<WallEr>, WallEr.ChooseItemFromWallEr));
			if (ModContent.GetInstance<BagToggle>().JarOfDirt)
				choseFromBagFunctions.Add(new(ModContent.ItemType<JarOfDirt>, JarOfDirt.ChooseItemFromJar));
			if (ModContent.GetInstance<BagToggle>().SlayersSack)
			{
				choseFromBagFunctions.Add(new(ModContent.ItemType<SlayersSack>, SlayersSack.ChooseRopeFromSack));
				chooseFromBagForQuickSelectFunctions.Add(new KeyValuePair<Func<int>, Func<Player, Func<Item, bool>, Item>>(ModContent.ItemType<SlayersSack>, SlayersSack.ChooseTorchFromSack));
			}
			if (ModContent.GetInstance<BagToggle>().MechanicsToolbelt)
				choseFromBagFunctions.Add(new(ModContent.ItemType<MechanicsToolbelt>, MechanicsToolbelt.ChoosePlacableItemFromBelt));
			#endregion
			
			AndroMod.OnResetGameCounter += () => {
				honeyWetResetTime = 0;
				nextFullCheckTime = 0;
			};
		}
		
		public override void Unload()
		{
			choseFromBagFunctions.Clear();
			chooseFromBagForQuickSelectFunctions.Clear();
		}
		
		public override void PostUpdateMiscEffects() {
			if (ModContent.GetInstance<BagToggle>().BannerBag)
				BannerBag.PostUpdateMiscEffects(Player);
		}
		public override void ResetEffects() {
			bagPlaceItem = null;
			bagItemChecked = false;
			bagSmartSelectItem = null;
			bagSmartSelectItemChecked = false;
			inventoryFoundIn = null;
			foundInventoryIndex = Storage.ItemNotFound;
			bagFoundIn = -1;
			TouchingStation = false;
		}
		public override void PreUpdate() {
			LastTouchingStation = TouchingStation;
			TouchingStation = false;
		}
		public override void PostUpdateBuffs() {
			if (Main.netMode == NetmodeID.Server || !ModContent.GetInstance<BagToggle>().PortableStation
			    && !ModContent.GetInstance<BagToggle>().ExquisitePotionFlask)
				return;

			if (Player.honeyWet)
				honeyWetResetTime = Main.GameUpdateCount + HoneyBuffTime;

			if (ModContent.GetInstance<BagToggle>().PortableStation)
				UpdateHoneyBuff();
			if (ModContent.GetInstance<BagToggle>().ExquisitePotionFlask && ModContent.GetInstance<BagToggle>().PotionFlask)
				ExquisitePotionFlask.PostUpdateBuffs(Player);
		}
		public override void PostUpdate() {
			if (ModContent.GetInstance<BagToggle>().TrashCan)
				TrashCan.TrashCheck();
		}
		public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource) {
			honeyWetResetTime = 0;
			if (ModContent.GetInstance<BagToggle>().ExquisitePotionFlask && ModContent.GetInstance<BagToggle>().PotionFlask)
				ExquisitePotionFlask.OnKilled(Player);
		}
		public override void OnRespawn() {
			if (ModContent.GetInstance<BagToggle>().ExquisitePotionFlask && ModContent.GetInstance<BagToggle>().PotionFlask)
				ExquisitePotionFlask.OnRespawn(Player);
		}

		#endregion

		#region Bag/Item Swap

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
		private static IList<Item> inventoryFoundIn = null;
		private static int foundInventoryIndex = Storage.ItemNotFound;
		private static int bagFoundIn = -1;

		private static List<KeyValuePair<Func<int>, Func<Player, Item>>> choseFromBagFunctions = [];
		private static List<KeyValuePair<Func<int>, Func<Player, Func<Item, bool>, Item>>> chooseFromBagForQuickSelectFunctions =
			[];

		private static Item GetBagPlaceItem(Player player) {
			bagItemChecked = true;
			int bagType = Main.LocalPlayer.HeldItem.type;
			foreach (KeyValuePair<Func<int>, Func<Player, Item>> choseFromBagPair in choseFromBagFunctions) {
				if (choseFromBagPair.Key() == bagType)
					return choseFromBagPair.Value(player);
			}

			return null;
		}
		private static bool ValidateStoredBagInfo => bagFoundIn == -1 && foundInventoryIndex > Storage.ItemNotFound && inventoryFoundIn != null && ReferenceEquals(inventoryFoundIn?[foundInventoryIndex], Main.LocalPlayer.inventory[foundInventoryIndex]);
		private static Item GetBagSmartSelectItem(Player player, Func<Item, bool> itemCondition) {
			bagSmartSelectItemChecked = true;
			foreach (KeyValuePair<Func<int>, Func<Player, Func<Item, bool>, Item>> choseFromBagPair in chooseFromBagForQuickSelectFunctions) {
				int bagType = choseFromBagPair.Key();
				if (StorageManager.HasRequiredItemToUseStorageFromBagType(player, bagType, out inventoryFoundIn, out foundInventoryIndex, out bagFoundIn)) {
					if (ValidateStoredBagInfo)
						return choseFromBagPair.Value(player, itemCondition);
				}
			}

			return null;
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
			Player player = drawinfo.drawPlayer;
			if (player.whoAmI != Main.myPlayer || Main.gameMenu) {
				orig(ref drawinfo);
				return;
			}

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

				if (swapSmartSelect && ValidateStoredBagInfo) {
					if (player.nonTorch == -1)
						player.nonTorch = player.selectedItem;

					player.selectedItem = foundInventoryIndex;
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

		#endregion

		#region Honey Buff

		public const int HoneyBuffTime = 1800;
		public const int AOEHoneyBuffTime = 2;
		public const int TicksPerSecond = 60;

		public bool TouchingStation = false;
		public bool LastTouchingStation = false;
		public bool NearPortableStation = false;
		private uint honeyWetResetTime = 0;
		private int lastHoneyBucketLocation = -1;
		private uint nextFullCheckTime = 0;
		private void UpdateNextHoneyFullCheckTime() {
			nextFullCheckTime = Main.GameUpdateCount + TicksPerSecond;
		}
		private void UpdatePlayerPortableStorageOverlap() {
			if (lastHoneyBucketLocation == -1)
				return;

			Player player = Player;
			Point p = new Point(0, 2);
			Vector2 vector = p.ToVector2();
			List<Point> tilesIn2 = Collision.GetTilesIn(player.TopLeft, player.BottomRight + vector);
			for (int j = 0; j < tilesIn2.Count; j++) {
				Point point2 = tilesIn2[j];
				Tile tile2 = Main.tile[point2.X, point2.Y];
				if (tile2.HasTile && tile2.TileType == GlobalBagTile.PortableStationType) {
					if (!LastTouchingStation)
						SoundEngine.PlaySound(SoundID.SplashWeak);

					TouchingStation = true;
					break;
				}
			}
		}
		public void UpdateHoneyBuff() {
			UpdatePlayerPortableStorageOverlap();
			int honeyBuffIndex = Player.FindBuffIndex(BuffID.Honey);
			bool hadBuff = honeyBuffIndex != -1;
			bool? fromStation = HoneyCheck();
			if (!hadBuff) {
				honeyWetResetTime = 0;
				honeyBuffIndex = Player.FindBuffIndex(BuffID.Honey);
			}

			bool hasHoney = honeyBuffIndex != -1;
			if (!hasHoney || lastHoneyBucketLocation < 0)
				return;
			 
			int context = fromStation == true || honeyWetResetTime < Main.GameUpdateCount || fromStation == null && Player.buffTime[honeyBuffIndex] <= AOEHoneyBuffTime ? ItemSlotContextID.BrightGreenSelected : ItemSlotContextID.Purple;
			StorageManager.BagUIs[PortableStation.Instance.BagStorageID].AddSelectedItemSlot(lastHoneyBucketLocation, context);
		}
		private bool? HoneyCheck() {
			if (!VacuumBags.serverConfig.PortableStationCanGiveHoneyBuff)
				return false;

			if (honeyWetResetTime > Main.GameUpdateCount + HoneyBuffTime && !TouchingStation)
				return false;

			if (!Player.TryGetModPlayer(out StoragePlayer storagePlayer))
				return false;

			bool doOnTouchBuff = false;
			bool doAOEBuff = false;
			bool bucketsActSame = !VacuumBags.serverConfig.PortableStationMustBeTouchedToGetHoneyBuff;
			Item[] inv = storagePlayer.Storages[PortableStation.Instance.BagStorageID].Items;
			bool doFullCheck = nextFullCheckTime <= Main.GameUpdateCount;
			if (!doFullCheck) {
				if (lastHoneyBucketLocation > -1) {
					Item item = inv[lastHoneyBucketLocation];
					if (item.type == ItemID.HoneyBucket) {
						doOnTouchBuff = true;
						if (bucketsActSame)
							doAOEBuff = true;
					}
					else if (item.type == ItemID.BottomlessHoneyBucket) {
						doOnTouchBuff = true;
						doAOEBuff = true;
					}
					else {
						doFullCheck = true;
					}

					if (doAOEBuff)
						UpdateNextHoneyFullCheckTime();
				}
			}

			if (doFullCheck) {
				//Full check
				lastHoneyBucketLocation = -1;
				for (int i = 0; i < inv.Length; i++) {
					Item item = inv[i];
					if (item.type == ItemID.HoneyBucket && lastHoneyBucketLocation == -1) {
						lastHoneyBucketLocation = i;
						doOnTouchBuff = true;
						if (bucketsActSame) {
							doAOEBuff = true;
							break;
						}
					}
					else if (item.type == ItemID.BottomlessHoneyBucket) {
						doOnTouchBuff = true;
						doAOEBuff = true;
						lastHoneyBucketLocation = i;
						break;
					}
				}

				UpdateNextHoneyFullCheckTime();
			}

			if (!doOnTouchBuff)
				return false;

			if (TouchingStation) {
				Player.AddBuff(BuffID.Honey, HoneyBuffTime);
				honeyWetResetTime = 0;
				return true;
			}

			if (!doAOEBuff)
				return false;

			if (NearPortableStation) {
				Player.AddBuff(BuffID.Honey, AOEHoneyBuffTime);
				return null;
			}

			return false;
		}

		#endregion
	}
}
