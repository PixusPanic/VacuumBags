using androLib.Common.Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using static Terraria.ID.ContentSamples.CreativeHelper;
using androLib.Common.Utility;

namespace VacuumBags.Items
{
	public interface INeedsSetUpAllowedList
	{
		public AllowedItemsManager GetAllowedItemsManager { get; }
	}

	public class AllowedItemsManager {
		public int OwningBagItemType => getOwningBagItemType();
		public Func<int> getOwningBagItemType;

		public SortedSet<int> AllowedItems = null;

		private Func<ItemSetInfo, SortedSet<ItemGroup>, SortedSet<string>, SortedSet<string>, bool?> devCheck = null;
		private Func<SortedSet<int>> GetDevWhiteList = null;
		private Func<SortedSet<string>> GetDevModWhiteList = null;
		private Func<SortedSet<int>> GetDevBlackList = null;
		private Func<SortedSet<string>> GetDevModBlackList = null;
		private Func<SortedSet<ItemGroup>> GetDevItemGroups = null;
		private Func<SortedSet<string>> GetDevEndWords = null;
		private Func<SortedSet<string>> GetDevSearchWords = null;

		private SortedSet<int> playerWhiteList = null;
		private SortedSet<string> playerModWhiteList = null;
		private SortedSet<int> playerBlackList = null;
		private SortedSet<string> playerModBlackList = null;

		//Setup Only
		private SortedSet<int> devWhiteList = null;
		private SortedSet<string> devModWhiteList = null;
		private SortedSet<int> devBlackList = null;
		private SortedSet<string> devModBlackList = null;

		private SortedSet<int> playerWhiteListCopy = null;
		private SortedSet<string> playerModWhiteListCopy = null;
		private SortedSet<int> playerBlackListCopy = null;
		private SortedSet<string> playerModBlackListCopy = null;
		private SortedSet<int> playerWhiteListForLog = null;
		private SortedSet<string> playerModWhiteListForLog = null;
		private SortedSet<int> playerBlackListForLog = null;
		private SortedSet<string> playerModBlackListForLog = null;

		private SortedSet<ItemGroup> itemGroups = null;
		private SortedSet<string> endWords = null;
		private SortedSet<string> searchWords = null;
		public AllowedItemsManager(
			Func<int> GetBagItemType,
			Func<ItemSetInfo, SortedSet<ItemGroup>, SortedSet<string>, SortedSet<string>, bool?> DevCheck,
			Func<SortedSet<int>> DevWhiteList = null,
			Func<SortedSet<string>> DevModWhiteList = null,
			Func<SortedSet<int>> DevBlackList = null,
			Func<SortedSet<string>> DevModBlackList = null,
			Func<SortedSet<ItemGroup>> DevItemGroups = null,
			Func<SortedSet<string>> DevEndWords = null,
			Func<SortedSet<string>> DevSearchWords = null
			) {

			getOwningBagItemType = GetBagItemType;

			AllowedItems = new();

			devCheck = DevCheck;
			GetDevWhiteList = DevWhiteList;
			GetDevModWhiteList = DevModWhiteList;
			GetDevBlackList = DevBlackList;
			GetDevModBlackList = DevModBlackList;
			GetDevItemGroups = DevItemGroups;
			GetDevEndWords = DevEndWords;
			GetDevSearchWords = DevSearchWords;
		}
		public void Load() {
			playerWhiteList = new();
			playerModWhiteList = new();
			playerBlackList = new();
			playerModBlackList = new();

			if (VacuumBags.clientConfig.LogAllPlayerWhiteAndBlackLists) {
				playerWhiteListForLog = new();
				playerModWhiteListForLog = new();
				playerBlackListForLog = new();
				playerModBlackListForLog = new();
			}
		}
		public void Save() {

		}
		public void PostLoadSetup() {
			devWhiteList = GetDevWhiteList?.Invoke() ?? new();
			devModWhiteList = GetDevModWhiteList?.Invoke() ?? new();
			devBlackList = GetDevBlackList?.Invoke() ?? new();
			devModBlackList = GetDevModBlackList?.Invoke() ?? new();
			itemGroups = GetDevItemGroups?.Invoke() ?? new();
			endWords = GetDevEndWords?.Invoke() ?? new();
			searchWords = GetDevSearchWords?.Invoke() ?? new();
			playerWhiteListCopy = new(playerWhiteList);
			playerModWhiteListCopy = new(playerModWhiteList);
			playerBlackListCopy = new(playerBlackList);
			playerModBlackListCopy = new(playerModBlackList);
		}
		public static readonly bool PrintFullBagWhitelists = true;
		public bool TryAddToAllowedItems(ItemSetInfo info, bool whitelistCheckOnly) {
			bool vanillaItem = info.Type < ItemID.Count;
			bool playerBlackListed = vanillaItem ? playerBlackListCopy.Remove(info.Type) : info.CheckModFullName(ref playerModBlackListCopy);
			bool playerWhiteListed = vanillaItem ? playerWhiteListCopy.Remove(info.Type) : info.CheckModFullName(ref playerModWhiteListCopy);
			bool? result = null;

			if (devWhiteList.Remove(info.Type) || !vanillaItem && info.CheckModFullName(ref devModWhiteList))
				result = true;

			if (result == null && whitelistCheckOnly)
				result = false;

			if (result == null) {
				if (vanillaItem) {
					if (devBlackList.Remove(info.Type))
						result = false;
				}
				else {
					if (info.CheckModFullName(ref devModBlackList))
						result = false;
				}
			}

			if (result == null)
				result = devCheck != null ? devCheck(info, itemGroups, endWords, searchWords) : null;

			if (result == null && info.CheckItemGroup(itemGroups))
				result = true;

			if (result == null && info.CheckEndsWith(endWords))
				result = true;

			if (result == null && info.CheckContains(searchWords))
				result = true;

			bool devWhitelisted = result == true;
			bool shouldAddItem = devWhitelisted && !playerBlackListed || playerWhiteListed;
			if (shouldAddItem) {
				if (playerWhiteListed) {
					if (devWhitelisted) {
						if (vanillaItem) {
							playerWhiteList.Remove(info.Type);
						}
						else {
							playerModWhiteList.Remove(info.ModFullName);
						}
					}
					else if (VacuumBags.clientConfig.LogAllPlayerWhiteAndBlackLists) {
						if (vanillaItem) {
							playerWhiteListForLog.Add(info.Type);
						}
						else {
							playerModWhiteListForLog.Add(info.ModFullName);
						}
					}
					
				}

				AllowedItems.Add(info.Type);
			}
			else {
				if (playerBlackListed) {
					if (!devWhitelisted) {
						if (vanillaItem) {
							playerBlackList.Remove(info.Type);
						}
						else {
							playerModBlackList.Remove(info.ModFullName);
						}
					}
					else {
						if (vanillaItem) {
							playerBlackListForLog.Add(info.Type);
						}
						else {
							playerModBlackListForLog.Add(info.ModFullName);
						}
					}
				}
			}

			return devWhitelisted;
		}
		public void ClearSetupLists() {
			if (VacuumBags.clientConfig.LogAllPlayerWhiteAndBlackLists) {
				string bagName = ContentSamples.ItemsByType[OwningBagItemType].ModItem?.Name;
				if (playerWhiteListForLog.Count() > 0)
					playerWhiteListForLog.Select(t => t.GetItemIDOrName()).S($"Player Whitelist ({bagName})");

				if (playerModWhiteListForLog.Count() > 0)
					playerModWhiteListForLog.S($"Player Mod Whitelist ({bagName})");

				if (playerBlackListForLog.Count() > 0)
					playerBlackListForLog.Select(t => t.GetItemIDOrName()).S($"Player Blacklist ({bagName})");

				if (playerModBlackListForLog.Count() > 0)
					playerModBlackListForLog.S($"Player Mod Blacklist ({bagName})");
			}

			devCheck = null;
			devModWhiteList = null;
			devBlackList = null;
			devModBlackList = null;
			playerWhiteListCopy = null;
			playerModWhiteListCopy = null;
			playerBlackListCopy = null;
			playerModBlackListCopy = null;
			itemGroups = null;
			endWords = null;
			searchWords = null;
			if (VacuumBags.clientConfig.LogAllPlayerWhiteAndBlackLists) {
				playerWhiteListForLog = null;
				playerModWhiteListForLog = null;
				playerBlackListForLog = null;
				playerModBlackListForLog = null;
			}
		}
		public void AddPlayerWhiteList(Item item) {
			AllowedItems.Add(item.type);
			if (item.type < ItemID.Count) {
				playerWhiteList.Add(item.type);
				playerBlackList.Remove(item.type);
			}
			else {
				string modFullName = item.ModFullName();
				playerModWhiteList.Add(modFullName);
				playerModBlackList.Remove(modFullName);
			}
		}
		public void AddPlayerBlackList(Item item) {
			AllowedItems.Remove(item.type);
			if (item.type < ItemID.Count) {
				playerWhiteList.Remove(item.type);
				playerBlackList.Add(item.type);
			}
			else {
				string modFullName = item.ModFullName();
				playerModWhiteList.Remove(modFullName);
				playerModBlackList.Add(modFullName);
			}
		}
	}

	public struct ItemSetInfo
	{
		public Item Item;
		public int Type;
		public ItemSetInfo(int itemTpye) {
			Type = itemTpye;
			Item = ContentSamples.ItemsByType[itemTpye];
		}
		public ItemSetInfo(Item item) {
			Type = item.type;
			Item = item;
		}
		public bool NullOrAir() => Item.NullOrAir();

		public string ModFullName {
			get {
				if (modFullName == null)
					modFullName = Item.ModFullName();

				return modFullName;
			}
		}
		public string modFullName = null;
		public bool CheckModFullName(ref SortedSet<string> modFullNames) => modFullNames.Remove(modFullName);

		public string LowerInternalName {
			get {
				if (lowerInternalName == null)
					lowerInternalName = Item.GetItemInternalName().ToLower();

				return lowerInternalName;
			}
		}
		public string lowerInternalName = null;
		public bool CheckEndsWith(SortedSet<string> endWords) {
			foreach (string endWord in endWords) {
				if (LowerInternalName.EndsWith(endWord))
					return true;
			}

			return false;
		}
		public bool CheckContains(SortedSet<string> searchWords) {
			foreach (string word in searchWords) {
				if (LowerInternalName.Contains(word))
					return true;
			}

			return false;
		}

		bool itemGroupChecked = false;
		public ItemGroup ItemGroup {
			get {
				if (!itemGroupChecked) {
					itemGroupChecked = true;
					itemGroup = new ItemGroupAndOrderInGroup(Item).Group;
				}

				return itemGroup;
			}
		}

		public ItemGroup itemGroup = ItemGroup.Coin;

		public bool CheckItemGroup(SortedSet<ItemGroup> itemGroups) => itemGroups.Contains(ItemGroup);
		public bool CheckItemGroup(ItemGroup itemGroup) => ItemGroup == itemGroup;

		public bool Weapon {
			get {
				if (weapon == null)
					weapon = Item.IsWeaponItem();

				return weapon.Value;
			}
		}
		private bool? weapon = null;
		public bool Accessory {
			get {
				if (accessory == null)
					accessory = Item.IsAccessoryItem();

				return accessory.Value;
			}
		}
		private bool? accessory = null;
		public bool Armor {
			get {
				if (armor == null)
					armor = Item.IsArmorItem();

				return armor.Value;
			}
		}
		private bool? armor = null;
		public bool Tool {
			get {
				if (tool == null)
					tool = Item.IsTool();

				return tool.Value;
			}
		}
		private bool? tool = null;

		public bool FishingPole {
			get {
				if (fishingPole == null)
					fishingPole = Item.IsFishingPole();

				return fishingPole.Value;
			}
		}
		private bool? fishingPole = null;

		public bool Equipment {
			get {
				if (equipment == null)
					equipment = Weapon || Armor || Accessory || Tool || FishingPole;

				return equipment.Value;
			}
		}
		private bool? equipment = null;
		public bool Banner => Type.IsBannerItem();
		public bool CreateTile => Item.createTile > -1;
		public bool CreateWall => Item.createWall > -1;
		public bool Ammo => Item.ammo > AmmoID.None;
		public bool ConsumableWeapon => Item.consumable && Weapon;
		public bool Bomb => ItemID.Sets.ItemsThatCountAsBombsForDemolitionistToSpawn[Type];
		public bool BossTrophyOrRelic {
			get {
				if (bossTrophyOrRelic == null)
					bossTrophyOrRelic = Item.IsBossTrophy() || Item.IsBossRelic(LowerInternalName);

				return bossTrophyOrRelic.Value;
			}
		}
		private bool? bossTrophyOrRelic = null;
		public bool BossSpawner => Item.IsBossSpawner();
		public bool Vanity => Item.vanity;
		public bool Potion => Item.potion;
		public bool Material => Item.material;
		public bool Consumable => Item.consumable;
		public bool CanShoot => Item.shoot > ProjectileID.None;
		public bool HasBuff => Item.buffType > 0;
		public bool GrassSeeds => ItemID.Sets.GrassSeeds[Type];
		public bool FlowerPacket => ItemID.Sets.flowerPacketInfo[Type] != null;
		public bool Extractable => ItemID.Sets.SortingPriorityExtractibles[Type] != -1;
		public bool DyeMaterial => ItemID.Sets.ExoticPlantsForDyeTrade[Type] || CheckItemGroup(ItemGroup.DyeMaterial);
		public bool RequiredTile => Item.IsRequiredTile();
		public bool Food => ItemID.Sets.IsFood[Type];
		public bool Rope => Item.IsRope();
		public bool Torch => Item.IsTorch();
		public bool WaterTorch => Item.IsWaterTorch();
		public bool Glowstick => Item.IsGlowstick();
		public bool FlairGun => Item.ISFlareGun();
		public bool Coin => Item.IsACoin;
	}
}
