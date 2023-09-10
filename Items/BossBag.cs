using androLib.Common.Utility;
using androLib;
using androLib.Items;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Terraria.ID.ContentSamples.CreativeHelper;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using androLib.ModIntegration;
using Terraria.GameContent.LootSimulation;
using androLib.Common.Globals;

namespace VacuumBags.Items
{
	public class BossBag : VBModItem, ISoldByWitch
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
				.AddRecipeGroup("VacuumBags:AnyBossTrophy", 1)
				.AddRecipeGroup("VacuumBags:AnyBossBag", 1)
				.AddIngredient(ItemID.GoldChest, 1)
				.Register();
			}
			else {
				CreateRecipe()
				.AddTile(TileID.HeavyWorkBench)
				.AddIngredient(ItemID.SkeletronBossBag)
				.AddIngredient(ItemID.EyeofCthulhuTrophy)
				.AddIngredient(ItemID.DeadMansChest, 1)
				.Register();
			}
		}

		public static int BagStorageID;//Set this when registering with androLib.

		public static void RegisterWithAndroLib(Mod mod) {
			BagStorageID = StorageManager.RegisterVacuumStorageClass(
				mod,//Mod
				typeof(BossBag),//type 
				ItemAllowedToBeStored,//Is allowed function, Func<Item, bool>
				null,//Localization Key name.  Attempts to determine automatically by treating the type as a ModItem, or you can specify.
				200,//StorageSize
				true,//Can vacuum
				() => new Color(121, 92, 18, androLib.Common.Configs.ConfigValues.UIAlpha),   // Get color function. Func<using Microsoft.Xna.Framework.Color>
				() => new Color(155, 130, 20, androLib.Common.Configs.ConfigValues.UIAlpha),   // Get Scroll bar color function. Func<using Microsoft.Xna.Framework.Color>
				() => new Color(147, 125, 30, androLib.Common.Configs.ConfigValues.UIAlpha), // Get Button hover color function. Func<using Microsoft.Xna.Framework.Color>
				() => ModContent.ItemType<BossBag>(),//Get ModItem type
				80,//UI Left
				675//UI Top
			);
		}
		public static bool ItemAllowedToBeStored(Item item) => AllowedItems.Contains(item.type);

		public static SortedSet<int> AllowedItems {
			get {
				if (allowedItems == null)
					GetAllowedItems();

				return allowedItems;
			}
		}
		private static SortedSet<int> allowedItems = null;

		private static void GetAllowedItems() {
			allowedItems = new() {
				
			};

			SortedSet<string> endWords = new() {
				
			};

			SortedSet<string> searchWords = new() {

			};

			foreach (int bossTrophyType in BossTrophies) {
				allowedItems.Add(bossTrophyType);
			}

			foreach (int bossBagType in BossBagsData.BossBags) {
				allowedItems.Add(bossBagType);
			}

			for (int i = 0; i < ItemLoader.ItemCount; i++) {
				if (allowedItems.Contains(i))
					continue;

				Item item = ContentSamples.ItemsByType[i];
				if (item.NullOrAir())
					continue;

				if (ItemID.Sets.SortingPriorityBossSpawns[i] > -1
					&& item.useStyle == ItemUseStyleID.HoldUp
					&& item.consumable
					&& item.useAnimation == 45
					&& item.useTime == 45
					) {
					allowedItems.Add(item.type);
					continue;
				}

				string lowerName = item.GetItemInternalName().ToLower();
				bool added = false;
				foreach (string endWord in endWords) {
					if (lowerName.EndsWith(endWord)) {
						allowedItems.Add(item.type);
						added = true;
						break;
					}
				}

				if (added)
					continue;

				foreach (string searchWord in searchWords) {
					if (lowerName.Contains(searchWord)) {
						allowedItems.Add(item.type);
						added = true;
						break;
					}
				}

				if (added)
					continue;

				ItemGroupAndOrderInGroup group = new ItemGroupAndOrderInGroup(item);
				if (group.Group == ItemGroup.BossBags || group.Group == ItemGroup.BossSpawners || group.Group == ItemGroup.EventItem || group.Group == ItemGroup.BossItem) {
					allowedItems.Add(item.type);
					continue;
				}

				if (lowerName.Contains("relic")
					&& item.useStyle == ItemUseStyleID.Swing
					&& item.useTurn
					&& item.autoReuse
					&& item.consumable
					) {
					allowedItems.Add(item.type);
					continue;
				}

				if (BossChecklistIntegration.BossInfos != null) {
					foreach (BossChecklistBossInfo bossChecklistInfo in BossChecklistIntegration.BossInfos.Select(p => p.Value)) {
						foreach (int bossSummonType in bossChecklistInfo.spawnItem) {
							allowedItems.Add(bossSummonType);
						}
					}
					foreach (BossChecklistBossInfo bossChecklistInfo in BossChecklistIntegration.BossInfos.Select(p => p.Value)) {
						foreach (int itemType in bossChecklistInfo.loot) {
							if (allowedItems.Contains(itemType))
								continue;

							Item lootItem = ContentSamples.ItemsByType[itemType];

							if (lootItem.vanity) {
								allowedItems.Add(itemType);
								continue;
							}

							if (lootItem.damage > 0)
								continue;

							if (lootItem.defense > 0)
								continue;

							if (lootItem.accessory)
								continue;

							if (lootItem.material && lootItem.createTile < 0 && lootItem.createWall < 0 && (lootItem.consumable || lootItem.shoot <= ProjectileID.None && lootItem.buffType <= 0))
								continue;

							if (lootItem.potion)
								continue;

							if (lootItem.ammo > 0)
								continue;

							if (lootItem.consumable && lootItem.createTile < 0 && lootItem.createWall < 0)
								continue;

							allowedItems.Add(itemType);
						}
					}
				}
			}

			foreach (int blackListItemType in BlackList) {
				allowedItems.Remove(blackListItemType);
			}
		}
		public static SortedSet<int> BossTrophies {
			get {
				if (bossTrophies == null)
					GetBossTrophies();

				return bossTrophies;
			}
		}

		public static SortedSet<int> bossTrophies = null;
		private static void GetBossTrophies() {
			bossTrophies = new() {
				
			};

			int bossTrophyValue = Item.sellPrice(0, 1);
			for (int i = 0; i < ItemLoader.ItemCount; i++) {
				Item item = ContentSamples.ItemsByType[i];
				string lowerName = item.GetItemInternalName().ToLower();
				if (lowerName.EndsWith("trophy")
					&& item.useStyle == ItemUseStyleID.Swing
					&& item.useTurn == true
					&& item.autoReuse == true
					&& item.consumable == true
					&& item.createTile > -1
					&& item.value == bossTrophyValue
					&& item.rare == ItemRarityID.Blue
					) {
					bossTrophies.Add(item.type);
				}
			}
		}

		public static SortedSet<int> BlackList {
			get {
				if (blackList == null)
					GetBlackList();

				return blackList;
			}
		}
		private static SortedSet<int> blackList = null;
		private static void GetBlackList() {
			blackList = new() {
				
			};

			SortedSet<string> modItemBlacklist = new() {
				"WeaponEnchantments/PowerBooster",
				"WeaponEnchantments/UltraPowerBooster",
				"StarsAbove/Starlight"
			};

			for (int i = ItemID.Count; i < ItemLoader.ItemCount; i++) {
				Item item = ContentSamples.ItemsByType[i];
				if (modItemBlacklist.Contains(item.ModFullName()))
					blackList.Add(item.type);
			}
		}

		#region AndroModItem attributes that you don't need.

		public virtual SellCondition SellCondition => SellCondition.Never;
		public virtual float SellPriceModifier => 1f;
		public override List<WikiTypeID> WikiItemTypes => new() { WikiTypeID.Storage };
		public override string LocalizationTooltip =>
			$"Automatically stores boss bags, trophies, boss statues, and cosmetic boss drops.\n" +
			$"When in your inventory, the contents of the bag are available for crafting.\n" +
			$"Right click to open the bag.\n" +
			$"Uses Boss Checklist to add modded items to the allowed items if it is enabled.";
		public override string Artist => "@kingjoshington";
		public override string Designer => "andro951";

		#endregion
	}
}
