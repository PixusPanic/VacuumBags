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
using System;

namespace VacuumBags.Items
{
	[Autoload(false)]
	public abstract class SimpleBag : BagModItem_VB {
		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
		public abstract int MyTileType { get; }
		public static int BagSize => -40;
		public override bool? CanVacuum => VacuumBags.clientConfig.SimpleBagsVacuumAllItems ? true : null;
		public override void SetDefaults() {
			Item.createTile = MyTileType;
			Item.consumable = true;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 10;
			Item.useAnimation = 15;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.maxStack = 1;
			Item.value = 100000;
			Item.rare = ItemRarityID.Blue;
			Item.width = 24;
			Item.height = 24;
		}

		private static Dictionary<int, SortedSet<int>> blacklists = new();
		private static Dictionary<int, SortedSet<int>> vacuumWhitelists = new();
		public static void ClearAllowedLists() {
			blacklists.Clear();
			vacuumWhitelists.Clear();
		}
		private SortedSet<int> Blacklist {
			get {
				if (!blacklists.TryGetValue(BagStorageID, out SortedSet<int> blacklist)) {
					blacklist = GetDefaultBlacklist();
					blacklist.UnionWith(StorageManager.GetPlayerBlackListSortedSet(BagStorageID));
					blacklists.Add(BagStorageID, blacklist);
				}

				return blacklist;
			}
		}
		private SortedSet<int> VacuumWhitelist {
			get {
				if (!vacuumWhitelists.TryGetValue(BagStorageID, out SortedSet<int> vacuumWhitelist)) {
					vacuumWhitelist = StorageManager.GetPlayerWhiteListSortedSet(BagStorageID);
					vacuumWhitelists.Add(BagStorageID, vacuumWhitelist);
				}

				return vacuumWhitelist;
			}
		}
		protected abstract SortedSet<int> GetDefaultBlacklist();
		public override Func<Item, bool> CanVacuumItemFunc => CanVacuumItem;
		private bool CanVacuumItem(Item item) => VacuumWhitelist.Contains(item.type);
		public override void UpdateAllowedList(int item, bool add) {
			if (add) {
				VacuumWhitelist.Add(item);
				Blacklist.Remove(item);
			}
			else {
				if (!VacuumWhitelist.Remove(item))
					Blacklist.Add(item);
			}
		}
		public override bool ItemAllowedToBeStored(Item item) => !Blacklist.Contains(item.type);
		public override void RegisterWithAndroLib(Mod mod) {
			if (Main.netMode == NetmodeID.Server)
				return;

			BagStorageID = StorageManager.RegisterVacuumStorageClass(
				mod,//Mod
				GetType(),//type
				ItemAllowedToBeStored,//Is allowed function, Func<Item, bool>
				null,//Localization Key name.  Attempts to determine automatically by treating the type as a ModItem, or you can specify.
				BagSize,//StorageSize
				CanVacuum,//Can vacuum
				() => PanelColor, // Get color function. Func<using Microsoft.Xna.Framework.Color>
				() => ScrollBarColor, // Get Scroll bar color function. Func<using Microsoft.Xna.Framework.Color>
				() => ButtonHoverColor, // Get Button hover color function. Func<using Microsoft.Xna.Framework.Color>
				() => GetBagType(),//Get ModItem type
				80,//UI Left
				675,//UI Top
				UpdateAllowedList,
				false,
				canVacuumItem: CanVacuumItem
			);
		}


		#region AndroModItem attributes that you don't need.

		public override string LocalizationTooltip =>
			$"Automatically stores items already contained in the bag.\n" +
			$"When in your inventory, the contents of the bag are available for crafting.\n" +
			$"Right click to open the bag.";
		public override string LocalizationDisplayName => Name.AddSpaces().Replace(" ", " (") + ")";
		public override string Artist => "@kingjoshington";
		public override string Designer => "@kingjoshington";

		#endregion
	}
}