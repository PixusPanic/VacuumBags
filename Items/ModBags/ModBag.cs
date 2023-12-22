using androLib;
using androLib.Common.Utility;
using androLib.Items;
using Microsoft.Xna.Framework;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;

namespace VacuumBags.Items
{
	public abstract class ModBag : AllowedListBagModItem_VB {
		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
		public abstract string ModDisplayNameTooltip { get; }
		public override void SetDefaults() {
			Item.maxStack = 1;
			Item.value = 100000;
			Item.rare = ItemRarityID.Blue;
			Item.width = 32;
			Item.height = 32;
		}
		public override bool ItemAllowedToBeStored(Item item) => AllowedItems.Contains(item.type) || item.ModItem is UnloadedItem unloadedItem && ModNames.Contains(unloadedItem.ModName);
		protected abstract SortedSet<string> ModNames { get; }
		public override SortedSet<int> DevWhiteList() {
			SortedSet<int> devWhiteList = new();
			foreach (string modName in ModNames) {
				if (!ModLoader.TryGetMod(modName, out Mod mod))
					continue;

				devWhiteList = new(devWhiteList.Concat(mod.GetContent<ModItem>().Where(m => m != null).Select(m => m.Type)));
			}

			return devWhiteList;
		}

		#region AndroModItem attributes that you don't need.

		public override string LocalizationTooltip =>
			$"Automatically stores items from {ModDisplayNameTooltip}.\n" +
			$"When in your inventory, the contents of the bag are available for crafting.\n" +
			$"Right click to open the bag.";
		public override string Artist => "@kingjoshington";
		public override string Designer => "andro951";


		#endregion
	}
}
