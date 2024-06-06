using androLib;
using androLib.Common.Utility;
using androLib.Items;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using VacuumBags.Localization;
using static Terraria.ID.ContentSamples.CreativeHelper;

namespace VacuumBags.Items {
	public abstract class AllowedListBagModItem_VB : AndroModItem, IBagModItem, INeedsSetUpAllowedList {
		protected override Action<ModItem, string, string> AddLocalizationTooltipFunc => VacuumBagsLocalizationDataStaticMethods.AddLocalizationTooltip;
		public virtual int BagStorageID { get; set; }
		public abstract Color PanelColor { get; }
		public abstract Color ScrollBarColor { get; }
		public abstract Color ButtonHoverColor { get; }
		public virtual Func<int> SoldByNPCNetID => null;
		public virtual SellCondition SellCondition => SellCondition.Never;
		public virtual float SellPriceModifier => 1f;
		public abstract int GetBagType();
		protected virtual Action SelectItemForUIOnly => null;
		protected virtual bool ShouldUpdateInfoAccessories => false;
		public virtual void RegisterWithAndroLib(Mod mod) {
			((IBagModItem)this).RegisterWithAndroLibIBagModItem(mod);

			INeedsSetUpAllowedList.RegisterAllowedItemsManager(((IBagModItem)this).BagStorageID, CreateAllowedItemsManager);
		}
		public virtual bool ItemAllowedToBeStored(Item item) => AllowedItems.Contains(item.type);
		public virtual void UpdateAllowedList(int item, bool add) {
			if (add) {
				AllowedItems.Add(item);
			}
			else {
				AllowedItems.Remove(item);
			}
		}
		public SortedSet<int> AllowedItems => GetAllowedItemsManager.AllowedItems;
		public virtual AllowedItemsManager GetAllowedItemsManager => INeedsSetUpAllowedList.AllowedItemsManagers[((IBagModItem)this).BagStorageID];
		public virtual Func<AllowedItemsManager> CreateAllowedItemsManager => () => new(GetBagType, () => ((IBagModItem)this).BagStorageID, DevCheck, DevWhiteList, DevModWhiteList, DevBlackList, DevModBlackList, ItemGroups, EndWords, SearchWords, PostSetup);
		public virtual void PostSetup() { }
		public virtual bool? DevCheck(ItemSetInfo info, SortedSet<ItemGroup> itemGroups, SortedSet<string> endWords, SortedSet<string> searchWords) {
			return null;
		}
		public virtual SortedSet<int> DevWhiteList() {
			SortedSet<int> devWhiteList = new();

			return devWhiteList;
		}
		public virtual SortedSet<string> DevModWhiteList() {
			SortedSet<string> devModWhiteList = new() {

			};

			return devModWhiteList;
		}
		public virtual SortedSet<int> DevBlackList() {
			SortedSet<int> devBlackList = new() {

			};

			return devBlackList;
		}
		public virtual SortedSet<string> DevModBlackList() {
			SortedSet<string> devModBlackList = new() {

			};

			return devModBlackList;
		}
		public virtual SortedSet<ItemGroup> ItemGroups() {
			SortedSet<ItemGroup> itemGroups = new() {

			};

			return itemGroups;
		}
		public virtual SortedSet<string> EndWords() {
			SortedSet<string> endWords = new() {

			};

			return endWords;
		}
		public virtual SortedSet<string> SearchWords() {
			SortedSet<string> searchWords = new() {

			};

			return searchWords;
		}
		public override List<WikiTypeID> WikiItemTypes => new() { WikiTypeID.Storage };
		public virtual string SummaryOfFunction => "";
	}
}
