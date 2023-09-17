using androLib;
using androLib.Common.Utility;
using androLib.Items;
using androLib.UI;
using System;
using System.Collections.Generic;
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
		private static IEnumerable<KeyValuePair<int, Item>> GetFirstXFromBag(int storageID, Func<Item, bool> itemCondition, Player player, int firstXItems) {
			if (Main.netMode == NetmodeID.MultiplayerClient && player.whoAmI != Main.myPlayer)
				return null;

			IEnumerable<KeyValuePair<int, Item>> indexItemsPairs = StorageManager.GetItems(storageID).Select((Item item, int index) => new KeyValuePair<int, Item>(index, item)).Where(p => !p.Value.NullOrAir() && p.Value.stack > 0 && itemCondition(p.Value));
			if (!indexItemsPairs.Any())
				return null;

			int itemsCount = indexItemsPairs.Count();
			if (firstXItems == FirstXItemsChooseAllItems || itemsCount <= firstXItems)
				return indexItemsPairs;

			if (!indexItemsPairs.AnyFavoritedItem())
				return indexItemsPairs.Take(firstXItems);

			IEnumerable<KeyValuePair<int, Item>> favoritedIndexItemsPairs = indexItemsPairs.Where(p => p.Value.favorited);
			int favoritedItemsCount = favoritedIndexItemsPairs.Count();
			if (favoritedItemsCount >= firstXItems) {
				if (favoritedItemsCount == firstXItems) {
					return favoritedIndexItemsPairs;
				}
				else {
					return favoritedIndexItemsPairs.Take(firstXItems);
				}
			}

			return favoritedIndexItemsPairs.Concat(indexItemsPairs.Where(p => !p.Value.favorited).Take(firstXItems - favoritedItemsCount));
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
		public static IEnumerable<Item> GetFirstXFromBag(int storageID, Func<Item, bool> itemCondition, Player player, int firstXItems, int context = ItemSlotContextID.YellowSelected, bool selectItems = true) {
			IEnumerable<KeyValuePair<int, Item>> indexItemsPairs = GetFirstXFromBag(storageID, itemCondition, player, firstXItems);
			if (indexItemsPairs == null)
				return new Item[0];

			return SelectAndGetItems(indexItemsPairs, storageID, context, selectItems);
		}
		private static IEnumerable<KeyValuePair<int, Item>> GetFirstFromBag(int storageID, Func<Item, bool> itemCondition, Player player) {
			return GetFirstXFromBag(storageID, itemCondition, player, 1);
		}
		public static Item ChooseFromBag(int storageID, Func<Item, bool> itemCondition, Player player, int context = ItemSlotContextID.YellowSelected, bool selectItems = true) {
			IEnumerable<KeyValuePair<int, Item>> indexItemsPairs = GetFirstFromBag(storageID, itemCondition, player);
			if (indexItemsPairs == null)
				return null;

			return SelectAndGetItems(indexItemsPairs, storageID, context, selectItems).First();
		}
		public static IEnumerable<Item> GetAllFromBag(int storageID, Func<Item, bool> itemCondition, Player player, int context = ItemSlotContextID.YellowSelected, bool selectItems = true) {
			return GetFirstXFromBag(storageID, itemCondition, player, FirstXItemsChooseAllItems, context, selectItems);
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

		private static void SetupAllAllowedItemManagers() {
			List<AllowedItemsManager> allowedItemManagers = StorageManager.AllBagTypes.Select(t => ContentSamples.ItemsByType[t].ModItem).OfType<INeedsSetUpAllowedList>().Select(b => b.GetAllowedItemsManager).ToList();
			foreach (AllowedItemsManager allowedItemsManager in allowedItemManagers) {
				allowedItemsManager.Load();
				allowedItemsManager.PostLoadSetup();
			}

			for (int i = 0; i < ItemLoader.ItemCount; i++) {
				ItemSetInfo info = new(i);
				if (info.NullOrAir())
					continue;

				if (StorageManager.AllBagTypesSorted.Contains(info.Type))
					continue;

				bool forWhitelistOnlyCheck = false;
				foreach (AllowedItemsManager allowedItemsManager in allowedItemManagers) {
					if (allowedItemsManager.TryAddToAllowedItems(info, forWhitelistOnlyCheck))
						forWhitelistOnlyCheck = true;
				}
			}

			foreach (AllowedItemsManager allowedItemsManager in allowedItemManagers) {
				allowedItemsManager.ClearSetupLists();
			}
		}
	}
}
