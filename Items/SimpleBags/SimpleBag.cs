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
	public abstract class SimpleBag : BagModItem {

		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
		public abstract int MyTileType { get; }
		public static Color PanelColor => new Color(255, 255, 255, androLib.Common.Configs.ConfigValues.UIAlpha);
		public static int BagSize => -40;
		public static bool? IsVacuumBag = VacuumBags.clientConfig.SimpleBagsVacuumAllItems ? true : null;
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

		public static int BagStorageID;//Set this when registering with androLib.

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