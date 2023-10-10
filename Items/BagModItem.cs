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
using VacuumOreBag.Items;

namespace VacuumBags.Items
{
    public abstract class BagModItem : AndroModItem, ISoldByNPC {
		protected override Action<ModItem, string, string> AddLocalizationTooltipFunc => VacuumBagsLocalizationDataStaticMethods.AddLocalizationTooltip;
		public virtual string SummaryOfFunction => SummaryOfFunctionDefault;
		public const string SummaryOfFunctionDefault = "N/A";
		private static IEnumerable<KeyValuePair<int, Item>> GetFirstXItemTypePairsFromBag(int storageID, Func<Item, bool> itemCondition, Player player, int firstXItemTypes, Func<Item, bool> doesntCountTowardsTotal = null) {
			if (Main.netMode == NetmodeID.MultiplayerClient && player.whoAmI != Main.myPlayer)
				return null;

			Item[] inv = StorageManager.GetItems(storageID);

			//First Pass through
			List<KeyValuePair<int, Item>> indexItemsPairs = new();
			SortedSet<int> sortedItemTypes = new();
			List<int> itemTypes = new();
			for (int i = 0; i < inv.Length; i++) {
				Item item = inv[i];
				if (item.NullOrAir() || item.stack < 1)
					continue;

				if (!itemCondition(item))
					continue;

				if (!sortedItemTypes.Contains(item.type)) {
					sortedItemTypes.Add(item.type);
					itemTypes.Add(item.type);
				}

				indexItemsPairs.Add(new KeyValuePair<int, Item>(i, item));
			}

			if (!indexItemsPairs.Any())
				return null;

			int itemsCount = sortedItemTypes.Count;
			if (firstXItemTypes == FirstXItemsChooseAllItems || itemsCount <= firstXItemTypes)
				return indexItemsPairs;

			//returnFunc is to minimize the effect of doesntCountTowardsTotal when it is null which is most calls.
			//This call makes it do nothing when doesntCountTowardsTotal is null.
			Func<IEnumerable<KeyValuePair<int, Item>>, SortedSet<int>, IEnumerable<KeyValuePair<int, Item>>> returnFunc = (iIP, chosen) => iIP.Where(p => chosen.Contains(p.Value.type));
			if (doesntCountTowardsTotal != null) {
				List<KeyValuePair<int, Item>> pairsThatDontCount = new();
				SortedSet<int> typesThatDontCount = new();
				List<int> indexesToRemove = new();

				for (int i = 0; i < indexItemsPairs.Count; i++) {
					Item item = indexItemsPairs[i].Value;
					if (doesntCountTowardsTotal(item)) {
						typesThatDontCount.Add(item.type);
						itemTypes.Remove(item.type);
						sortedItemTypes.Remove(item.type);
						indexesToRemove.Add(i);
						pairsThatDontCount.Add(indexItemsPairs[i]);
					}
				}

				for (int i = indexesToRemove.Count - 1; i >= 0; i--) {
					indexItemsPairs.RemoveAt(indexesToRemove[i]);
				}

				if (typesThatDontCount.Count > 0) {
					returnFunc = (iIP, chosen) => {
						chosen.UnionWith(typesThatDontCount);
						return iIP.Concat(pairsThatDontCount).Where(p => chosen.Contains(p.Value.type));
					};

					itemsCount = itemTypes.Count;
					if (itemsCount <= firstXItemTypes)
						return returnFunc(indexItemsPairs, sortedItemTypes);
				}
			}

			//Favorited Items
			SortedSet<int> sortedFavoritedTypes = new();
			List<int> favoritedTypes = new();
			for (int i = 0; i < indexItemsPairs.Count; i++) {
				Item item = indexItemsPairs[i].Value;
				if (item.favorited) {
					if (!sortedFavoritedTypes.Contains(item.type)) {
						sortedFavoritedTypes.Add(item.type);
						favoritedTypes.Add(item.type);
					}
				}
			}

			if (sortedFavoritedTypes.Count < 1) {
				SortedSet<int> chosenTypes = new(itemTypes.Take(firstXItemTypes));
				return returnFunc(indexItemsPairs, chosenTypes);
			}

			int favoritedItemsCount = sortedFavoritedTypes.Count;
			if (favoritedItemsCount >= firstXItemTypes) {
				if (favoritedItemsCount == firstXItemTypes) {
					return returnFunc(indexItemsPairs, sortedFavoritedTypes);
				}
				else {
					SortedSet<int> chosenTypes = new(favoritedTypes.Take(firstXItemTypes));
					return returnFunc(indexItemsPairs, chosenTypes);
				}
			}

			foreach (int favoritedItemType in sortedFavoritedTypes) {
				itemTypes.Remove(favoritedItemType);
			}

			SortedSet<int> chosenTypes2 = new(sortedFavoritedTypes.Concat(itemTypes.Take(firstXItemTypes - sortedFavoritedTypes.Count)));
			return returnFunc(indexItemsPairs, chosenTypes2);
		}
		private static IEnumerable<KeyValuePair<int, Item>> GetFirstItemTypePairFromBag(int storageID, Func<Item, bool> itemCondition, Player player) {
			if (Main.netMode == NetmodeID.MultiplayerClient && player.whoAmI != Main.myPlayer)
				return null;

			Item[] inv = StorageManager.GetItems(storageID);
			Item chosenItem = null;
			int index = -1;
			for (int i = 0; i < inv.Length; i++) {
				Item item = inv[i];
				if (item.NullOrAir() || item.stack < 1)
					continue;

				if (!itemCondition(item))
					continue;

				if (chosenItem == null || item.favorited) {
					chosenItem = item;
					index = i;
				}
				else {
					continue;
				}

				if (chosenItem.favorited)
					break;
			}

			return chosenItem == null ? null : new List<KeyValuePair<int, Item>>() { new KeyValuePair<int, Item>(index, chosenItem) };
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
			IEnumerable<KeyValuePair<int, Item>> indexItemsPairs = GetFirstXItemTypePairsFromBag(storageID, itemCondition, player, firstXItems, doesntCountTowardsTotal);
			if (indexItemsPairs == null)
				return new List<Item>();

			return SelectAndGetItems(indexItemsPairs, storageID, context, selectItems);
		}
		public static IEnumerable<KeyValuePair<int, Item>> GetFirstFromBag(int storageID, Func<Item, bool> itemCondition, Player player) {
			return GetFirstItemTypePairFromBag(storageID, itemCondition, player);
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

				return fromBag();
			}

			for (int k = 0; k < bagIndex; k++) {
				if (inventory[k].stack > 0 && itemCondition(inventory[k]))
					return null;
			}

			return fromBag();
		}

		public Func<int> SoldByNPCNetID => null;
		public virtual SellCondition SellCondition => SellCondition.Never;
		public virtual float SellPriceModifier => 1f;
		public override List<WikiTypeID> WikiItemTypes => new() { WikiTypeID.Storage };
	}
}
