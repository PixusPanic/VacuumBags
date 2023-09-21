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

namespace VacuumBags.Items
{
	[Autoload(false)]
	public  class PotionFlask : BagModItem, ISoldByWitch, INeedsSetUpAllowedList
	{
		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
		public override void SetDefaults() {
            Item.maxStack = 1;
            Item.value = 100000;
			Item.rare = ItemRarityID.Blue;
			Item.width = 32;
            Item.height = 32;
        }
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

		public static int BagStorageID;//Set this when registering with androLib.


		public static void RegisterWithAndroLib(Mod mod) {
			BagStorageID = StorageManager.RegisterVacuumStorageClass(
				mod,//Mod
				typeof(PotionFlask),//type 
				ItemAllowedToBeStored,//Is allowed function, Func<Item, bool>
				null,//Localization Key name.  Attempts to determine automatically by treating the type as a ModItem, or you can specify.
				100,//StorageSize
				true,//Can vacuum
				() => new Color(80, 10, 80, androLib.Common.Configs.ConfigValues.UIAlpha),    // Get color function. Func<using Microsoft.Xna.Framework.Color>
				() => new Color(90, 10, 90, androLib.Common.Configs.ConfigValues.UIAlpha),    // Get Scroll bar color function. Func<using Microsoft.Xna.Framework.Color>
				() => new Color(120, 0, 120, androLib.Common.Configs.ConfigValues.UIAlpha),   // Get Button hover color function. Func<using Microsoft.Xna.Framework.Color>
				() => ModContent.ItemType<PotionFlask>(),//Get ModItem type
				80,//UI Left
				675//UI Top
			);
		}
		public static bool ItemAllowedToBeStored(Item item) => AllowedItems.Contains(item.type);
		public static Item OnQuickBuff_PickBestFoodItem(On_Player.orig_QuickBuff_PickBestFoodItem orig, Player self) {
			Item item = orig(self);

			int potionFlaskID = ModContent.ItemType<PotionFlask>();
			if (!self.HasItem(potionFlaskID))
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
		private static Item PickBestFoodItemFromFlask(int nextHighestBuffNum, Item foundFoodItem, Player player) {
			if (player.whoAmI != Main.myPlayer)
				return null;

			IEnumerable<Item> foodItems = StorageManager.GetItems(BagStorageID).Where(item => QuickBuff_FindFoodPriority(item.buffType) > 0);
			bool anyFavoritedFood = foodItems.AnyFavoritedItem();
			foreach (Item foodItem in (anyFavoritedFood ? foodItems.Where(item => item.favorited) : foodItems)) {
				if (!foodItem.NullOrAir())
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
			orig(self);

			if (self.whoAmI != Main.myPlayer)
				return;

			if (self.cursed || self.CCed || self.dead)
				return;

			if (self.CountBuffs() == Player.MaxBuffs)
				return;

			int potionFlaskID = ModContent.ItemType<PotionFlask>();
			if (!self.HasItem(potionFlaskID))
				return;

			MethodInfo itemCheck_CheckCanUse = typeof(Player).GetMethod("ItemCheck_CheckCanUse", BindingFlags.NonPublic | BindingFlags.Instance);
			IEnumerable<Item> nonFoodBuffItems = StorageManager.GetItems(BagStorageID).Where(
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
			if (!self.HasItem(potionFlaskID))
				return foundHealItem;

			IEnumerable<Item> healItems = StorageManager.GetItems(BagStorageID).Where(item =>
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
			if (!self.HasItem(potionFlaskID))
				return foundManaItem;

			IEnumerable<Item> manaItems = StorageManager.GetItems(BagStorageID).Where(item =>
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

		public static SortedSet<int> AllowedItems => AllowedItemsManager.AllowedItems;
		public static AllowedItemsManager AllowedItemsManager = new(ModContent.ItemType<PotionFlask>, DevCheck, DevWhiteList, DevModWhiteList, DevBlackList, DevModBlackList, ItemGroups, EndWords, SearchWords);
		public AllowedItemsManager GetAllowedItemsManager => AllowedItemsManager;
		protected static bool? DevCheck(ItemSetInfo info, SortedSet<ItemGroup> itemGroups, SortedSet<string> endWords, SortedSet<string> searchWords) {
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
		protected static SortedSet<int> DevWhiteList() {
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
				ItemGroup.BuffPotion,
				ItemGroup.LifePotions,
				ItemGroup.ManaPotions,
				ItemGroup.Flask,
				ItemGroup.Food
			};
			
			return itemGroups;
		}
		protected static SortedSet<string> EndWords() {
			SortedSet<string> endWords = new() {
				"potion"
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
			$"Automatically stores potions, food and drink.\n" +
			$"When in your inventory, the contents of the bag are available for crafting.\n" +
			$"Right click to open the bag.\n" +
			$"Quick Buff will use food items from the bag.  If any food items in the bag are favorited, only favorited food items will be used.\n" +
			$"Quick Buff will use favorited potions from the bag.\n" +
			$"Quick Heal will use healing items from the bag.  If any healing items in the bag are favorited, only favorited healing items will be used.\n" +
			$"Quick Mana will use mana items from the bag.  If any mana items in the bag are favorited, only favorited mana items will be used.";
		public override string Artist => "anodomani";
		public override string Designer => "@kingjoshington";

		#endregion
	}
}
