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
using VacuumOreBag.Items;

namespace VacuumBags.Items {
	public abstract class BagModItem_VB : AndroModItem, IBagModItem {
		protected override Action<ModItem, string, string> AddLocalizationTooltipFunc => VacuumBagsLocalizationDataStaticMethods.AddLocalizationTooltip;
		public virtual int BagStorageID { get; set; }
		public abstract Color PanelColor { get; }
		public abstract Color ScrollBarColor { get; }
		public abstract Color ButtonHoverColor { get; }
		public abstract int GetBagType();
		public abstract bool ItemAllowedToBeStored(Item item);
		public abstract void UpdateAllowedList(int item, bool add);
		public virtual int DefaultBagSize => 100;
		public virtual bool? CanVacuum => true;
		public virtual bool BlackListOnly => false;
		public virtual Func<Item, bool> CanVacuumItemFunc => null;
		public virtual void RegisterWithAndroLib(Mod mod) {
			((IBagModItem)this).RegisterWithAndroLibIBagModItem(mod);
		}
		public override List<WikiTypeID> WikiItemTypes => new() { WikiTypeID.Storage };
		public virtual Action SelectItemForUIOnly => null;
		public virtual bool ShouldUpdateInfoAccessories => false;
	}
}
