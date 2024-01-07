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
using System.Reflection;
using Terraria.Audio;
using System.Diagnostics.CodeAnalysis;
using androLib.UI;
using Terraria.UI;
using Microsoft.Xna.Framework.Input;

namespace VacuumBags.Items
{
    [Autoload(false)]
	public  class PotionFlask : AllowedListBagModItem_VB {
		public static BagModItem Instance {
			get {
				if (instance == null)
					instance = new PotionFlask();

				return instance;
			}
		}
		private static BagModItem instance;
		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
		public override void SetDefaults() {
            Item.maxStack = 1;
            Item.value = 100000;
			Item.rare = ItemRarityID.Blue;
			Item.width = 32;
            Item.height = 32;
		}
		public override int GetBagType() => ModContent.ItemType<PotionFlask>();
		public override void AddRecipes() {
			if (!VacuumBags.serverConfig.HarderBagRecipes) {
				CreateRecipe()
				.AddTile(TileID.WorkBenches)
				.AddIngredient(ItemID.Glass, 100)
				.AddIngredient(ItemID.Fireblossom, 10)
				.AddIngredient(ItemID.Moonglow, 10)
				.AddIngredient(ItemID.Shiverthorn, 10)
				.Register();
			}
			else {
				CreateRecipe()
				.AddTile(TileID.AlchemyTable)
				.AddIngredient(ItemID.Glass, 100)
				.AddIngredient(ItemID.LesserHealingPotion, 100)
				.AddIngredient(ItemID.Fireblossom, 20)
				.AddIngredient(ItemID.Moonglow, 20)
				.AddIngredient(ItemID.Shiverthorn, 20)
				.AddIngredient(ItemID.Blinkroot, 20)
				.AddIngredient(ItemID.Daybloom, 20)
				.AddIngredient(ItemID.Deathweed, 20)
				.AddIngredient(ItemID.Waterleaf, 20)
				.Register();
			}
		}
		public override Color PanelColor => new Color(80, 10, 80, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ScrollBarColor => new Color(90, 10, 90, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ButtonHoverColor => new Color(120, 0, 120, androLib.Common.Configs.ConfigValues.UIAlpha);

		private static bool HasAndCanUsePotionFlask(Player player, bool onlyCheckRegular = false) => !VacuumBags.clientConfig.TurnOffRegularPotionFlask && StorageManager.HasRequiredItemToUseStorageFromBagType(player, PotionFlaskType, out _) || !onlyCheckRegular && StorageManager.HasRequiredItemToUseStorageFromBagType(player, ExquisitePotionFlaskType, out _, true);

		#region QuickBuff/Heal

		public static Item OnQuickBuff_PickBestFoodItem(On_Player.orig_QuickBuff_PickBestFoodItem orig, Player self) {
			Item item = orig(self);

			if (!HasAndCanUsePotionFlask(self))
				return item;

			int highestBuffNum = item != null ? QuickBuff_FindFoodPriority(item.buffType) : 0;
			if (highestBuffNum < 1) {
				//No food found by Vanilla, so need to check buffs again.
				for (int i = 0; i < Player.MaxBuffs; i++) {
					if (self.buffTime[i] >= 1) {
						int num2 = QuickBuff_FindFoodPriority(self.buffType[i]);
						if (highestBuffNum <= num2)
							highestBuffNum = num2 + 1;
					}
				}
			}

			if (highestBuffNum >= 4)
				return item;

			Item item2 = PickBestFoodItemFromFlask(highestBuffNum, item, self);
			if (item2 != null)
				return item2;

			return item;
		}
		protected static Item PickBestFoodItemFromFlask(int nextHighestBuffNum, Item foundFoodItem, Player player) {
			if (player.whoAmI != Main.myPlayer)
				return null;

			IEnumerable<Item> foodItems = StorageManager.GetItems(Instance.BagStorageID).Where(item => QuickBuff_FindFoodPriority(item.buffType) > 0);
			bool anyFavoritedFood = foodItems.AnyFavoritedItem();
			foreach (Item foodItem in (anyFavoritedFood ? foodItems.Where(item => item.favorited) : foodItems)) {
				if (foodItem.NullOrAir())
					continue;

				int buffNum = QuickBuff_FindFoodPriority(foodItem.buffType);
				if (buffNum >= nextHighestBuffNum) {
					if (buffNum > nextHighestBuffNum) {
						nextHighestBuffNum = buffNum;
						foundFoodItem = foodItem;
					}
					else if (foundFoodItem == null || foodItem.buffTime > foundFoodItem.buffTime) {
						foundFoodItem = foodItem;
					}
				}
			}

			return foundFoodItem;
		}
		public static int QuickBuff_FindFoodPriority(int buffType) {//Copied from Player.cs QuickBuff_FindFoodPriority()
			switch (buffType) {
				case BuffID.WellFed:
					return 1;
				case BuffID.WellFed2:
					return 2;
				case BuffID.WellFed3:
					return 3;
				default:
					return 0;
			}
		}
		public static void OnQuickBuff(On_Player.orig_QuickBuff orig, Player self) {//Copied/edited from Player.cs QuickBuff()
			QuickBuff_Unpause(self);

			orig(self);

			if (self.whoAmI != Main.myPlayer)
				return;

			if (self.cursed || self.CCed || self.dead)
				return;

			if (self.CountBuffs() == Player.MaxBuffs)
				return;

			if (!HasAndCanUsePotionFlask(self))
				return;

			MethodInfo itemCheck_CheckCanUse = typeof(Player).GetMethod("ItemCheck_CheckCanUse", BindingFlags.NonPublic | BindingFlags.Instance);
			IEnumerable<Item> nonFoodBuffItems = StorageManager.GetItems(Instance.BagStorageID).Where(
				item => !item.NullOrAir() &&
				item.favorited &&
				item.stack > 0 &&
				item.buffType > 0 &&
				QuickBuff_FindFoodPriority(item.buffType) < 1 &&
				!item.CountsAsClass(DamageClass.Summon) &&
				(bool)itemCheck_CheckCanUse.Invoke(self, new object[] { item })
			);

			if (!nonFoodBuffItems.Any())
				return;

			SoundStyle? legacySoundStyle = null;
			MethodInfo quickBuff_ShouldBotherUsingThisBuff = typeof(Player).GetMethod("QuickBuff_ShouldBotherUsingThisBuff", BindingFlags.NonPublic | BindingFlags.Instance);
			foreach (Item item in nonFoodBuffItems) {
				int buffType = item.buffType;
				bool shouldBotherUsingThisBuff = (bool)quickBuff_ShouldBotherUsingThisBuff.Invoke(self, new object[] { buffType });
				if (item.mana > 0 && shouldBotherUsingThisBuff) {
					if (self.CheckMana(item, -1, true, true)) {
						self.manaRegenDelay = (int)self.maxRegenDelay;
					}
					else {
						shouldBotherUsingThisBuff = false;
					}
				}

				if (item.type == ItemID.Carrot && !Main.runningCollectorsEdition)
					shouldBotherUsingThisBuff = false;

				if (item.buffType == BuffID.FairyBlue) {
					buffType = Main.rand.Next(3);
					if (buffType == 0)
						buffType = BuffID.FairyBlue;

					if (buffType == 1)
						buffType = BuffID.FairyRed;

					if (buffType == 2)
						buffType = BuffID.FairyGreen;
				}

				if (!shouldBotherUsingThisBuff)
					continue;

				ItemLoader.UseItem(item, self);

				legacySoundStyle = item.UseSound;
				int buffTime = item.buffTime;
				if (buffTime == 0)
					buffTime = 3600;

				self.AddBuff(buffType, buffTime);
				if (item.consumable && ItemLoader.ConsumeItem(item, self)) {
					item.stack--;

					if (item.stack <= 0)
						item.TurnToAir(true);
				}

				if (self.CountBuffs() == Player.MaxBuffs)
					break;
			}

			if (legacySoundStyle != null) {
				SoundEngine.PlaySound(legacySoundStyle, self.position);
				Recipe.FindRecipes();
			}
		}
		public static Item OnQuickHeal_GetItemToUse(On_Player.orig_QuickHeal_GetItemToUse orig, Player self) {
			Item foundHealItem = orig(self);

			if (self.whoAmI != Main.myPlayer)
				return foundHealItem;

			int potionFlaskID = ModContent.ItemType<PotionFlask>();
			if (!HasAndCanUsePotionFlask(self))
				return foundHealItem;

			IEnumerable<Item> healItems = StorageManager.GetItems(Instance.BagStorageID).Where(item =>
				!item.NullOrAir() &&
				item.stack > 0 &&
				item.potion &&
				item.healLife > 0 &&
				CombinedHooks.CanUseItem(self, item)
			);

			if (!healItems.Any())
				return foundHealItem;

			if (healItems.AnyFavoritedItem())
				healItems = healItems.Where(item => item.favorited);

			int lifeThatCanBeHealed = self.statLifeMax2 - self.statLife;
			int negativeMaxLife = -self.statLifeMax2;
			foreach (Item healItem in healItems) {
				int remainingLifeThatCanBeHealed = self.GetHealLife(healItem, true) - lifeThatCanBeHealed;
				if (healItem.type == ItemID.RestorationPotion && remainingLifeThatCanBeHealed < 0) {
					remainingLifeThatCanBeHealed += 30;
					if (remainingLifeThatCanBeHealed > 0)
						remainingLifeThatCanBeHealed = 0;
				}

				if (negativeMaxLife < 0) {
					if (remainingLifeThatCanBeHealed > negativeMaxLife) {
						foundHealItem = healItem;
						negativeMaxLife = remainingLifeThatCanBeHealed;
					}
				}
				else if (remainingLifeThatCanBeHealed < negativeMaxLife && remainingLifeThatCanBeHealed >= 0) {
					foundHealItem = healItem;
					negativeMaxLife = remainingLifeThatCanBeHealed;
				}
			}

			return foundHealItem;
		}
		public static Item OnQuickMana_GetItemToUse(On_Player.orig_QuickMana_GetItemToUse orig, Player self) {
			Item foundManaItem = orig(self);

			if (self.whoAmI != Main.myPlayer)
				return foundManaItem;

			int potionFlaskID = ModContent.ItemType<PotionFlask>();
			if (!HasAndCanUsePotionFlask(self))
				return foundManaItem;

			IEnumerable<Item> manaItems = StorageManager.GetItems(Instance.BagStorageID).Where(item =>
				!item.NullOrAir() &&
				item.stack > 0 &&
				(!item.potion || self.potionDelay == 0) &&
				item.healMana > 0 &&
				CombinedHooks.CanUseItem(self, item)
			);

			if (!manaItems.Any())
				return foundManaItem;

			if (manaItems.AnyFavoritedItem())
				manaItems = manaItems.Where(item => item.favorited);

			return manaItems.First();

			//int manaThatCanBeHealed = self.statManaMax2 - self.statMana;
			//int negativeMaxMana = -self.statManaMax2;
			//foreach (Item manaItem in manaItems) {
			//	int remainingManaThatCanBeHealed = self.GetHealMana(manaItem, true) - manaThatCanBeHealed;
			//	if (negativeMaxMana < 0) {
			//		if (remainingManaThatCanBeHealed > negativeMaxMana) {
			//			foundManaItem = manaItem;
			//			negativeMaxMana = remainingManaThatCanBeHealed;
			//		}
			//	}
			//	else if (remainingManaThatCanBeHealed < negativeMaxMana && remainingManaThatCanBeHealed >= 0) {
			//		foundManaItem = manaItem;
			//		negativeMaxMana = remainingManaThatCanBeHealed;
			//	}
			//}

			//return foundManaItem;
		}

		#endregion

		#region Buff Pausing and Duration

		private static int scanIndex = 0;
		private static int PersistantDuration = 2;
		private static SortedDictionary<int, int> trackedBuffIndexes = new();//buff type, buff index
		private static SortedDictionary<int, int> trackedItemIndexes = new();//buff type, item index
		private static SortedDictionary<int, BuffInfo> Buffs = new();
		private static int ExquisitePotionFlaskType {
			get {
				if (exquisitePotionFlaskType == -1)
					exquisitePotionFlaskType = ModContent.ItemType<ExquisitePotionFlask>();

				return exquisitePotionFlaskType;
			}
		}
		private static int exquisitePotionFlaskType = -1;
		private static int PotionFlaskType {
			get {
				if (potionFlaskType == -1)
					potionFlaskType = ModContent.ItemType<PotionFlask>();

				return potionFlaskType;
			}
		}
		private static int potionFlaskType = -1;
		private static uint ticksAdded = 0;
		private static bool lastHasPotionFlask = false;
		private static bool hasPotionFlask = false;
		private static bool hasExquisiteFlask = false;
		public static bool firstCheckSinceLoad = true;
		private static int maxBuffs = Player.MaxBuffs;
		private static int nextOpen = 0;
		public static void OnKilled(Player player) {
			List<int> toRemove = new();
			foreach (KeyValuePair<int, BuffInfo> pair in Buffs) {
				if (pair.Value.OnKilledShouldRemove())
					toRemove.Add(pair.Key);
			}

			foreach (int key in toRemove) {
				Buffs.Remove(key);
			}
		}
		public static void OnRespawn(Player player) {
			if (!hasPotionFlask)
				return;

			nextOpen = 0;
			SortedSet<int> toRemove = new();
			foreach (BuffInfo info in Buffs.Values) {
				if (!info.Paused) {
					if (!hasExquisiteFlask && VacuumBags.serverConfig.PotionFlaskSavesBuffsOnDeath) {
						info.GiveBuffAtNextAvailable(player);
					}
					else {
						if (!player.HasBuff(info.Type))
							toRemove.Add(info.Type);
					}
				}
			}

			foreach (int key in toRemove) {
				Buffs.Remove(key);
			}
		}
		internal static void OnDelBuff(On_Player.orig_DelBuff orig, Player self, int b) {
			if (ItemSlot.ShiftInUse && Main.mouseRight && Main.mouseRightRelease && Main.HoverItem.NullOrAir()) {
				int type = self.buffType[b];
				Buffs.Remove(type);
			}

			orig(self, b);
			
			nextOpen = 0;
			foreach (BuffInfo info in Buffs.Values) {
				if (!info.Paused && info.BuffIndex >= b && info.BuffIndex > 0)
					info.BuffIndex--;
			}
		}
		internal static void OnAddBuff(On_Player.orig_AddBuff orig, Player self, int type, int timeToAdd, bool quiet, bool foodHack) {
			Item[] inv = null;
			bool shouldCheckBuffs = Main.netMode != NetmodeID.Server;
			if (shouldCheckBuffs) {
				shouldCheckBuffs = self.TryGetModPlayer(out StoragePlayer storagePlayer);
				if (shouldCheckBuffs) {
					inv = storagePlayer.Storages[Instance.BagStorageID].Items;
					if (hasPotionFlask) {
						if (Buffs.TryGetValue(type, out BuffInfo infoToPause)) {
							infoToPause.Pause(inv);
							infoToPause.RemoveBuff(self);
						}
					}
				}
			}

			orig(self, type, timeToAdd, quiet, foodHack);

			//Fixes AlchemistNPC Alchemist Charm buff duration issue because it adds the same buff 3 times.
			//Maybe add a check for AlchemistNPCLite here instead of removing it?
			//if (!shouldCheckBuffs)
			//	return;

			//if (!hasPotionFlask)
			//	return;

			//if (Buffs.TryGetValue(type, out BuffInfo info)) {
			//	int buffIndex = self.FindBuffIndex(type);
			//	if (buffIndex > -1)
			//		info.CheckIndexTryCombineTime(self, inv, buffIndex);
			//}
		}
		internal static void OnTryRemovingBuff(On_Main.orig_TryRemovingBuff orig, int i, int b) {
			if (hasPotionFlask && Main.LocalPlayer.TryGetModPlayer(out StoragePlayer storagePlayer)) {
				nextOpen = 0;
				Item[] inv = storagePlayer.Storages[Instance.BagStorageID].Items;
				foreach (KeyValuePair<int, BuffInfo> pair in Buffs) {
					if (pair.Key == b)
						pair.Value.Pause(inv);
				}
			}

			orig(i, b);
		}
		public static void QuickBuff_Unpause(Player player) {
			if (!hasPotionFlask)
				return;

			bool playSound = false;
			Item[] inv = StoragePlayer.LocalStoragePlayer.Storages[Instance.BagStorageID].Items;
			foreach (BuffInfo info in Buffs.Values) {
				if (info.Paused) {
					info.UnPause(inv);
					info.GiveBuffAtNextAvailable(player);
					playSound = true;
				}
			}

			if (playSound)
				SoundEngine.PlaySound(SoundID.Item3);
		}
		internal static void PreSaveAndQuit() {
			if (VacuumBags.clientConfig.TurnOffRegularPotionFlask && !hasExquisiteFlask)
				return;

			Player player = Main.LocalPlayer;
			nextOpen = 0;
			RefreshTrackedBuffsAndItemIndexes(player);
			foreach (BuffInfo info in Buffs.Values) {
				if (!trackedBuffIndexes.ContainsKey(info.Type))
					info.GiveBuffAtNextAvailable(player);
			}

			firstCheckSinceLoad = true;
			lastHasPotionFlask = false;
			Buffs.Clear();
			trackedBuffIndexes.Clear();
			trackedItemIndexes.Clear();
		}
		internal static void PostUpdateBuffs(Player player) {
			hasExquisiteFlask = StorageManager.HasRequiredItemToUseStorageFromBagTypeSlow(player, ExquisitePotionFlaskType, out _, true);
			if (hasExquisiteFlask) {
				hasPotionFlask = true;
			}
			else {
				hasPotionFlask = HasAndCanUsePotionFlask(player, false);
			}
			
			if (!hasPotionFlask && (Buffs.Count == 0 || VacuumBags.clientConfig.TurnOffRegularPotionFlask)) {
				lastHasPotionFlask = false;
				return;
			}

			//foreach (BuffInfo info in Buffs.Values) {
			//	player.ClearBuff(info.Type);
			//}

			//Buffs.Clear();

			nextOpen = 0;
			maxBuffs = Player.MaxBuffs;
			Item[] inv = StoragePlayer.LocalStoragePlayer.Storages[Instance.BagStorageID].Items;
			if (!lastHasPotionFlask) {
				hasPotionFlask = true;
				scanIndex = 0;
				ticksAdded = Main.GameUpdateCount / 2;
				while (scanIndex < inv.Length) {
					ScanForPotions(inv);
				}

				RefreshTrackedBuffsAndItemIndexes(player);
				UpdateBuffsAndBuffTimes(player, inv);
			}
			else {
				if (scanIndex == inv.Length) {
					scanIndex = -1;
					RefreshTrackedBuffsAndItemIndexes(player);
				}
				else if (scanIndex == -1) {
					UpdateBuffsAndBuffTimes(player, inv);
				}
				else {
					ScanForPotions(inv);
				}
			}

			int ticksToAdd = (int)(Main.GameUpdateCount / 2 - ticksAdded);
			ticksAdded += (uint)ticksToAdd;
			UpdatePlayerBuffs(player, inv, ticksToAdd);

			if (hasPotionFlask) {
				foreach (BuffInfo info in Buffs.Values) {
					info.PostUpdate(player, inv);
				}

				lastHasPotionFlask = true;
			}

			firstCheckSinceLoad = false;
		}
		private static void ScanForPotions(Item[] inv) {
			int scansPerTick = inv.Length.CeilingDivide(4);//Always finish in 4 ticks
			int endIndex = (int)Math.Min(scanIndex + scansPerTick, inv.Length);
			for (; scanIndex < endIndex; scanIndex++) {
				Item item = inv[scanIndex];
				int buffType = item.buffType;
				if (buffType > 0) {
					if (!trackedItemIndexes.TryAdd(buffType, scanIndex)) {
						Item currentItem = inv[trackedItemIndexes[buffType]];
						if (!currentItem.favorited && item.favorited)
							trackedItemIndexes[buffType] = scanIndex;
					}
				}
			}
		}
		private static void RefreshTrackedBuffsAndItemIndexes(Player player) {
			trackedBuffIndexes = new();
			for (int i = 0; i < maxBuffs; i++) {
				int buffType = player.buffType[i];
				if (buffType > 0 && ItemSets.IsPotionBuff(buffType))
					trackedBuffIndexes.TryAdd(player.buffType[i], i);
			}

			foreach (BuffInfo info in Buffs.Values) {
				if (trackedItemIndexes.TryGetValue(info.Type, out int inventoryIndex))
					info.ItemIndex = inventoryIndex;
			}
		}
		private static void UpdateBuffsAndBuffTimes(Player player, Item[] inv) {
			scanIndex = 0;

			//Check all buffs against items and reset items
			int[] keys = trackedBuffIndexes.Keys.ToArray();
			for (int i = 0; i < keys.Length; i++) {
				int buffType = keys[i];
				int buffIndex = trackedBuffIndexes[buffType];
				if (Buffs.TryGetValue(buffType, out BuffInfo info)) {
					//Extra check since trackedBuffIndexes could have changed in the last tick.
					if (hasPotionFlask && info.Paused && buffType == player.buffType[buffIndex])
						info.CheckIndexTryCombineTime(player, inv, buffIndex);
				}
				else {
					if (!trackedItemIndexes.TryGetValue(buffType, out int inventoryIndex))
						inventoryIndex = -1;

					bool wasFavorited = firstCheckSinceLoad;
					if (inventoryIndex > -1) {
						if (!firstCheckSinceLoad)
							inv[inventoryIndex].favorited = true;

						wasFavorited |= inv[inventoryIndex].favorited;
					}

					Buffs.Add(buffType, new(buffType, buffIndex, player.buffTime[buffIndex], inventoryIndex, wasFavorited));
				}
			}

			foreach (BuffInfo info in Buffs.Values) {
				info.CheckUpdateTrackedBuffs(player);
			}

			trackedBuffIndexes.Clear();
			trackedItemIndexes.Clear();
		}
		private static void UpdatePlayerBuffs(Player player, Item[] inv, int ticksToAdd) {
			List<BuffInfo> missed = new();
			foreach (BuffInfo info in Buffs.Values) {
				if (!info.TryGiveBuff(player, inv, ticksToAdd))
					missed.Add(info);
			}

			if (missed.Count > 0) {
				if (scanIndex == inv.Length)
					scanIndex = -1;

				RefreshTrackedBuffsAndItemIndexes(player);
				foreach (BuffInfo info in missed) {
					info.TryFindOrPause(player, inv);
				}
			}
		}
		public class BuffInfo {
			public int Type;
			public int BuffIndex;
			public int Time {
				get => time;
				set {
					time = hasExquisiteFlask && (0 <= value && value <= PersistantDuration) ? PersistantDuration : value;
				}
			}
			private int time;
			public int ItemIndex;
			public bool Paused = false;
			public bool WasFavorited;
			public bool lastHasItem = false;
			public BuffInfo(int type, int buffIndex, int time, int itemIndex, bool wasFavorited) {
				Type = type;
				BuffIndex = buffIndex;
				Time = time;
				ItemIndex = itemIndex;
				WasFavorited = wasFavorited;
				lastHasItem = itemIndex >= 0;
			}
			public bool HasItem(Item[] inv) => 0 <= ItemIndex && ItemIndex < inv.Length && inv[ItemIndex].buffType == Type;
			public void CheckUpdateFavoritedAndPaused(Player player, Item[] inv) {
				bool hasItem = HasItem(inv);

				bool favorited = hasItem && inv[ItemIndex].favorited;
				if (hasItem) {
					if (lastHasItem) {
						if (Paused) {
							if (favorited) {
								if (!WasFavorited) {
									UnPause(inv);
									TryFindNextOpenAndGiveBuff(player, inv);
								}
							}
						}
						else {
							if (!favorited) {
								if (WasFavorited) {
									if (AndroModSystem.FavoriteKeyDown) {
										Pause(inv);
										RemoveBuff(player);
									}
									else {
										//Manually clicking a new potion item that isn't favorited onto the existing favorited one unfavorites it and pauses the buff.
										inv[ItemIndex].favorited = true;
									}
								}
							}
						}
					}
					else {
						if (Paused) {
							if (favorited) {
								UnPause(inv);
								TryFindNextOpenAndGiveBuff(player, inv);
							}
						}
						else {
							if (firstCheckSinceLoad) {
								if (!favorited) {
									Pause(inv);
									RemoveBuff(player);
								}
							}
							else {
								if (!favorited) {
									UnPause(inv);
									TryFindNextOpenAndGiveBuff(player, inv);
								}
							}
						}
					}
				}
			}
			public bool TryGiveBuff(Player player, Item[] inv, int ticksToAdd) {
				CheckUpdateFavoritedAndPaused(player, inv);
				if (Paused)
					return true;

				return TryGiveBuffFromPreviousIndex(player, ticksToAdd);
			}
			public bool TryGiveBuffFromPreviousIndex(Player player, int ticksToAdd) {
				int buffType = player.buffType[BuffIndex];
				if (buffType == Type) {
					ref int buffTime = ref player.buffTime[BuffIndex];
					if (hasExquisiteFlask) {
						if (buffTime >= 0) {
							if (buffTime <= PersistantDuration) {
								buffTime = PersistantDuration;
							}
							else if (ticksToAdd > 0) {
								buffTime += ticksToAdd;
							}
						}
					}

					Time = buffTime;

					return true;
				}

				return false;
			}
			public bool TryFindNextOpenAndGiveBuff(Player player, Item[] inv) {
				CheckUpdateFavoritedAndPaused(player, inv);

				//Check newly updated trackedBuffs
				if (trackedBuffIndexes.TryGetValue(Type, out int newIndex)) {
					BuffIndex = newIndex;
					Time = player.buffTime[newIndex];
					if (TryGiveBuffFromPreviousIndex(player, 0))
						return true;
				}

				return GiveBuffAtNextAvailable(player);
			}
			public bool TryFindOrPause(Player player, Item[] inv) {
				CheckUpdateFavoritedAndPaused(player, inv);

				//Check newly updated trackedBuffs
				if (trackedBuffIndexes.TryGetValue(Type, out int newIndex)) {
					BuffIndex = newIndex;
					Time = player.buffTime[newIndex];
					if (TryGiveBuffFromPreviousIndex(player, 0))
						return true;
				}

				if (!hasExquisiteFlask && Time <= 0) {
					Buffs.Remove(Type);
				}
				else {
					Pause(inv);
				}
				
				return false;
			}
			public bool GiveBuffAtNextAvailable(Player player) {
				//Find the next available buff slot
				while (nextOpen < maxBuffs && player.buffType[nextOpen] > 0) {
					nextOpen++;
				}

				if (nextOpen >= maxBuffs)
					return false;

				player.buffType[nextOpen] = Type;
				ref int newBuffTime = ref player.buffTime[nextOpen];
				newBuffTime = Time;
				return true;
			}
			public void CheckUpdateTrackedBuffs(Player player) {
				int playerBuffType = player.buffType[BuffIndex];
				if (Type != playerBuffType) {
					if (trackedBuffIndexes.TryGetValue(Type, out int newIndex)) {
						if (BuffIndex != newIndex)
							CombineNoAdd(player, newIndex);
					}
				}
			}
			public bool OnKilledShouldRemove() => Time <= PersistantDuration;
			public void RemoveBuff(Player player) {
				if (player.buffType[BuffIndex] == Type) {
					player.DelBuff(BuffIndex);
				}
				else {
					player.ClearBuff(Type);
				}
			}
			public void Pause(Item[] inv) {
				Paused = true;
				for (int i = 0; i < inv.Length; i++) {
					Item item = inv[i];
					if (item.NullOrAir())
						continue;

					if (item.buffType == Type) {
						item.favorited = false;
						if (item.TryGetGlobalItem(out VacuumToStorageItem globalItem))
							globalItem.favorited = false;

						WasFavorited = false;
					}
				}
			}
			public void UnPause(Item[] inv) {
				Paused = false;
				for (int i = 0; i < inv.Length; i++) {
					Item item = inv[i];
					if (item.NullOrAir())
						continue;

					if (item.buffType == Type) {
						ItemIndex = i;
						item.favorited = true;
						WasFavorited = true;
						break;
					}
				}
			}
			public void CheckIndexTryCombineTime(Player player, Item[] inv, int buffIndex) {
				BuffIndex = buffIndex;
				if (Paused)
					UnPause(inv);

				ref int buffTime = ref player.buffTime[buffIndex];
				buffTime += Time;
				Time = buffTime;
			}
			public void CombineNoAdd(Player player, int buffIndex) {
				BuffIndex = buffIndex;
				ref int buffTime = ref player.buffTime[buffIndex];
				buffTime = Math.Max(buffTime, Time);
				Time = buffTime;
			}
			internal void PostUpdate(Player player, Item[] inv) {
				if (hasExquisiteFlask) {
					if (Time < PersistantDuration) {
						Time = PersistantDuration;
						if (!Paused) {
							if (player.buffType[Type] != Type)
								TryFindNextOpenAndGiveBuff(player, inv);
						}
					}
				}

				if (ItemIndex >= 0 && inv[ItemIndex].buffType == Type && Time > 0)
					StorageManager.BagUIs[Instance.BagStorageID].AddSelectedItemSlot(ItemIndex, !Paused ? ItemSlotContextID.BrightGreenSelected : ItemSlotContextID.YellowSelected);

				lastHasItem = HasItem(inv);
			}
		}

		#endregion


		public override bool? DevCheck(ItemSetInfo info, SortedSet<ItemGroup> itemGroups, SortedSet<string> endWords, SortedSet<string> searchWords) {
			if (info.Equipment)
				return false;

			if (info.Food)
				return true;

			if (info.Potion)
				return true;

			if (info.Consumable && info.HasBuff)
				return true;

			return false;
		}
		public override SortedSet<int> DevWhiteList() {
			SortedSet<int> devWhiteList = new() {
				ItemID.RecallPotion,
				ItemID.PotionOfReturn,
				ItemID.LesserHealingPotion,
				ItemID.HealingPotion,
				ItemID.GreaterHealingPotion,
				ItemID.LesserRestorationPotion,
				ItemID.RestorationPotion,
				ItemID.LesserManaPotion,
				ItemID.ManaPotion,
				ItemID.GreaterManaPotion,
				ItemID.SuperManaPotion,
				ItemID.WormholePotion,
				ItemID.SuperHealingPotion,
				ItemID.ObsidianSkinPotion,
				ItemID.RegenerationPotion,
				ItemID.SwiftnessPotion,
				ItemID.GillsPotion,
				ItemID.IronskinPotion,
				ItemID.ManaRegenerationPotion,
				ItemID.MagicPowerPotion,
				ItemID.FeatherfallPotion,
				ItemID.SpelunkerPotion,
				ItemID.InvisibilityPotion,
				ItemID.ShinePotion,
				ItemID.NightOwlPotion,
				ItemID.BattlePotion,
				ItemID.ThornsPotion,
				ItemID.WaterWalkingPotion,
				ItemID.ArcheryPotion,
				ItemID.HunterPotion,
				ItemID.GravitationPotion,
				ItemID.RedPotion,
				ItemID.MiningPotion,
				ItemID.HeartreachPotion,
				ItemID.CalmingPotion,
				ItemID.BuilderPotion,
				ItemID.TitanPotion,
				ItemID.FlipperPotion,
				ItemID.SummoningPotion,
				ItemID.TrapsightPotion,
				ItemID.AmmoReservationPotion,
				ItemID.LifeforcePotion,
				ItemID.EndurancePotion,
				ItemID.RagePotion,
				ItemID.InfernoPotion,
				ItemID.WrathPotion,
				ItemID.TeleportationPotion,
				ItemID.LovePotion,
				ItemID.StinkPotion,
				ItemID.FishingPotion,
				ItemID.SonarPotion,
				ItemID.CratePotion,
				ItemID.WarmthPotion,
				ItemID.GenderChangePotion,
				ItemID.LuckPotionLesser,
				ItemID.LuckPotion,
				ItemID.LuckPotionGreater,
				ItemID.BiomeSightPotion,
			};

			return devWhiteList;
		}
		public override SortedSet<ItemGroup> ItemGroups() {
			SortedSet<ItemGroup> itemGroups = new() {
				ItemGroup.BuffPotion,
				ItemGroup.LifePotions,
				ItemGroup.ManaPotions,
				ItemGroup.Flask,
				ItemGroup.Food
			};
			
			return itemGroups;
		}
		public override SortedSet<string> EndWords() {
			SortedSet<string> endWords = new() {
				"potion"
			};

			return endWords;
		}

		#region AndroModItem attributes that you don't need.

		public override string SummaryOfFunction => "Potions and food";
		public override string LocalizationTooltip =>
			$"Automatically stores potions, food and drink.\n" +
			$"When in your inventory, the contents of the bag are available for crafting.\n" +
			$"Right click to open the bag.\n" +
			$"Quick Buff will use food items from the bag.  If any food items in the bag are favorited, only favorited food items will be used.\n" +
			$"Quick Buff will use favorited potions from the bag.\n" +
			$"Quick Heal will use healing items from the bag.  If any healing items in the bag are favorited, only favorited healing items will be used.\n" +
			$"Quick Mana will use mana items from the bag.  If any mana items in the bag are favorited, only favorited mana items will be used.\n" +
			$"Potion buffs can be paused by right clicking on the buff like you normally would to remove it or unfavoriting the potion.\n" +
			$"Potions providing a buff will have a green background.  Paused potions will have a yellow background.\n" +
			$"Paused buffs can be resumed with quick buff or by favoriting the potion in the flask.\n" +
			$"Potion effects are kept on death at the same duration they were before dying.\n" +
			$"Gaining a potion buff while you already have the buff will add their durations together instead of setting it to the new effect's duration.\n" +
			$"Removing a buff with the Shift key held will delete the buff instead of pausing it.";
		public override string Artist => "anodomani";
		public override string Designer => "@kingjoshington";

		#endregion
	}
}
