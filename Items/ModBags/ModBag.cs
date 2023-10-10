using androLib.Common.Utility;
using androLib.Items;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace VacuumBags.Items
{
	public abstract class ModBag : BagModItem
	{
		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
		public abstract string ModDisplayNameTooltip { get; }
		public static Color PanelColor => new Color(255, 255, 255, androLib.Common.Configs.ConfigValues.UIAlpha);
		public static Color ScrollBarColor => new Color(255, 255, 255, androLib.Common.Configs.ConfigValues.UIAlpha);
		public static Color ButtonHoverColor => new Color(255, 255, 255, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override void SetDefaults() {
			Item.maxStack = 1;
			Item.value = 100000;
			Item.rare = ItemRarityID.Blue;
			Item.width = 32;
			Item.height = 32;
		}

		public static int BagStorageID;//Set this when registering with androLib.

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
