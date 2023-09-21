using androLib;
using androLib.Common.Utility;
using androLib.Items;
using androLib.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using VacuumBags.Localization;

namespace VacuumBags.Items
{
	public abstract class BagModItem : AndroModItem
	{
		protected override Action<ModItem, string, string> AddLocalizationTooltipFunc => VacuumBagsLocalizationDataStaticMethods.AddLocalizationTooltip;
		private static IEnumerable<KeyValuePair<int, Item>> GetFirstItemTypePairsXFromBag(int storageID, Func<Item, bool> itemCondition, Player player, int firstXItems, Func<Item, bool> doesntCountTowardsTotal = null) {
			if (Main.netMode == NetmodeID.MultiplayerClient && player.whoAmI != Main.myPlayer)
				return null;

			IEnumerable<KeyValuePair<int, Item>> indexItemsPairs = StorageManager.GetItems(storageID).Select((Item item, int index) => new KeyValuePair<int, Item>(index, item)).Where(p => !p.Value.NullOrAir() && p.Value.stack > 0 && itemCondition(p.Value));
			if (!indexItemsPairs.Any())
				return null;

			int itemsCount = indexItemsPairs.Count();
			if (firstXItems == FirstXItemsChooseAllItems || itemsCount <= firstXItems)
				return indexItemsPairs;

			//returnFunc is to minimize the effect of doesntCountTowardsTotal when it is null which is most calls.
			//This call makes it do nothing when doesntCountTowardsTotal is null.
			Func<IEnumerable<KeyValuePair<int, Item>>, IEnumerable<KeyValuePair<int, Item>>> returnFunc = (iIP) => iIP;
			if (doesntCountTowardsTotal != null) {
				IEnumerable<KeyValuePair<int, Item>> itemsThatDontCount = indexItemsPairs.Where(p => doesntCountTowardsTotal(p.Value));
				if (itemsThatDontCount.Any()) {
					returnFunc = (iIP) => iIP.Concat(itemsThatDontCount);

					indexItemsPairs = indexItemsPairs.Where(p => !doesntCountTowardsTotal(p.Value));
					itemsCount = indexItemsPairs.Count();

					if (itemsCount <= firstXItems)
						return returnFunc(indexItemsPairs);
				}
			}
			
			if (!indexItemsPairs.AnyFavoritedItem(doesntCountTowardsTotal))
				return returnFunc(indexItemsPairs.Take(firstXItems));

			IEnumerable<KeyValuePair<int, Item>> favoritedIndexItemsPairs = indexItemsPairs.Where(p => p.Value.favorited);
			int favoritedItemsCount = favoritedIndexItemsPairs.Count();
			if (favoritedItemsCount >= firstXItems) {
				if (favoritedItemsCount == firstXItems) {
					return returnFunc(favoritedIndexItemsPairs);
				}
				else {
					return returnFunc(favoritedIndexItemsPairs.Take(firstXItems));
				}
			}

			return returnFunc(favoritedIndexItemsPairs.Concat(indexItemsPairs.Where(p => !p.Value.favorited).Take(firstXItems - favoritedItemsCount)));
		}
		public const int FirstXItemsChooseAllItems = -1;
		private static IEnumerable<Item> SelectAndGetItems(IEnumerable<KeyValuePair<int, Item>> indexItemsPairs, int storageID, int context, bool selectItems = true) {
			if (selectItems) {
				foreach (int key in indexItemsPairs.Select(p => p.Key)) {
					StorageManager.BagUIs[storageID].AddSelectedItemSlot(key, context);
				}
			}

			return indexItemsPairs.Select(p => p.Value);
		}
		public static IEnumerable<Item> GetFirstXFromBag(int storageID, Func<Item, bool> itemCondition, Player player, int firstXItems, Func<Item, bool> doesntCountTowardsTotal = null, int context = ItemSlotContextID.YellowSelected, bool selectItems = true) {
			IEnumerable<KeyValuePair<int, Item>> indexItemsPairs = GetFirstItemTypePairsXFromBag(storageID, itemCondition, player, firstXItems, doesntCountTowardsTotal);
			if (indexItemsPairs == null)
				return new List<Item>();

			return SelectAndGetItems(indexItemsPairs, storageID, context, selectItems);
		}
		public static IEnumerable<KeyValuePair<int, Item>> GetFirstFromBag(int storageID, Func<Item, bool> itemCondition, Player player) {
			return GetFirstItemTypePairsXFromBag(storageID, itemCondition, player, 1);
		}
		public static Item ChooseFromBag(int storageID, Func<Item, bool> itemCondition, Player player, int context = ItemSlotContextID.YellowSelected, bool selectItems = true) {
			IEnumerable<KeyValuePair<int, Item>> indexItemsPairs = GetFirstFromBag(storageID, itemCondition, player);
			if (indexItemsPairs == null)
				return null;

			return SelectAndGetItems(indexItemsPairs, storageID, context, selectItems).First();
		}
		public static IEnumerable<Item> GetAllFromBag(int storageID, Func<Item, bool> itemCondition, Player player, int context = ItemSlotContextID.YellowSelected, bool selectItems = true) {
			return GetFirstXFromBag(storageID, itemCondition, player, FirstXItemsChooseAllItems, null, context, selectItems);
		}
		public static Item ChooseFromBagOnlyIfFirstInInventory(Item item, Player player, int storageID, Func<Item, bool> itemCondition, int context = ItemSlotContextID.YellowSelected, bool selectItems = true) {
			if (!player.TryGetModPlayer(out StoragePlayer storagePlayer))
				return null;

			if (!storagePlayer.Storages[storageID].HasRequiredItemToUseStorage(storagePlayer.Player, out _, out int bagIndex) || bagIndex == Storage.RequiredItemNotFound)
				return null;

			IEnumerable<KeyValuePair<int, Item>> indexItemsPairs = GetFirstFromBag(storageID, itemCondition, player);
			if (indexItemsPairs == null)
				return null;

			int tempCount = indexItemsPairs.Count();
			Func<Item> fromBag = () => SelectAndGetItems(indexItemsPairs, storageID, context, selectItems).First();

			int startOfAmmoIndex = 54;
			if (bagIndex == Storage.ReuiredItemInABagStartingIndex || bagIndex == startOfAmmoIndex)
				return fromBag();

			if (item == null)
				return fromBag();

			if (fromBag == null)
				return null;

			Item[] inventory = player.inventory;
			if (bagIndex >= startOfAmmoIndex) {
				for (int j = startOfAmmoIndex; j < bagIndex; j++) {
					if (inventory[j].stack > 0 && itemCondition(inventory[j]))
						return null;
				}
			}

			for (int k = 0; k < bagIndex; k++) {
				if (inventory[k].stack > 0 && itemCondition(inventory[k]))
					return null;
			}

			return fromBag();
		}

		public static void PostSetupRecipes() {
			SetupAllAllowedItemManagers();
		}

		public static readonly bool PrintDevOnlyAllowedItemListInfo = Debugger.IsAttached && VacuumBags.clientConfig.LogAllPlayerWhiteAndBlackLists;
		private static void SetupAllAllowedItemManagers() {
			List<AllowedItemsManager> allowedItemManagers = StorageManager.AllBagTypes.Select(t => ContentSamples.ItemsByType[t].ModItem).OfType<INeedsSetUpAllowedList>().Select(b => b.GetAllowedItemsManager).ToList();
			SortedDictionary<int, SortedSet<int>> enchantedItemsAllowedInBags = new();
			foreach (AllowedItemsManager allowedItemsManager in allowedItemManagers) {
				allowedItemsManager.Load();
				allowedItemsManager.PostLoadSetup();
				if (PrintDevOnlyAllowedItemListInfo)
					enchantedItemsAllowedInBags.Add(allowedItemsManager.OwningBagItemType, new());
			}

			List<int> itemsNotAdded = new List<int>();

			for (int i = 0; i < ItemLoader.ItemCount; i++) {
				ItemSetInfo info = new(i);
				if (info.NullOrAir())
					continue;

				if (StorageManager.AllBagTypesSorted.Contains(info.Type))
					continue;

				bool forWhitelistOnlyCheck = false;
				foreach (AllowedItemsManager allowedItemsManager in allowedItemManagers) {
					if (allowedItemsManager.TryAddToAllowedItems(info, forWhitelistOnlyCheck)) {
						forWhitelistOnlyCheck = true;
						if (PrintDevOnlyAllowedItemListInfo && ContentSamples.ItemsByType[i].IsEnchantable())
							enchantedItemsAllowedInBags[allowedItemsManager.OwningBagItemType].Add(info.Type);
					}
				}

				if (!forWhitelistOnlyCheck)
					itemsNotAdded.Add(info.Type);
			}

			if (PrintDevOnlyAllowedItemListInfo) {
				IEnumerable<Item> itemsNotSelected = itemsNotAdded.Select(t => ContentSamples.ItemsByType[t]);
				//itemsNotSelected.Select(i => i.S()).S("All Items that don't fit in bags:").LogSimple();
				itemsNotSelected.Where(i => !i.IsEnchantable()).Select(i => i.S()).S("All Non-Enchantable Items that don't fit in bags:").LogSimple();
				SortedSet<int> sortedItemsNotSelected = new (itemsNotAdded);
				IEnumerable<Item> allOtherItems = ContentSamples.ItemsByType.Select(p => p.Value).Where(i => !sortedItemsNotSelected.Contains(i.type));
				enchantedItemsAllowedInBags.Select(p => p.Value.Select(i => ContentSamples.ItemsByType[i].S()).S(ContentSamples.ItemsByType[p.Key].Name)).S("All Enchantable Items that fit in bags:").LogSimple();
			}

			foreach (AllowedItemsManager allowedItemsManager in allowedItemManagers) {
				allowedItemsManager.ClearSetupLists();
			}
		}
	}
}
