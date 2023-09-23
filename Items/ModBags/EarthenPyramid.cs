using androLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ModLoader.Default;

namespace VacuumBags.Items
{
	[Autoload(false)]
	public class EarthenPyramid : ModBag {
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();
			Item.width = 30;
		}
		public override string ModDisplayNameTooltip => "Secrets of the Shadows";
		public override string Designer => "@level12lobster";
		public override string Artist => "@level12lobster";
		new public static int BagStorageID;
		public static SortedSet<int> Blacklist {
			get {
				if (blacklist == null) {
					blacklist = new() {

					};
				}

				return blacklist;
			}
		}
		private static SortedSet<int> blacklist = null;
		public static bool ItemAllowedToBeStored(Item item) => item?.ModItem != null && (item.ModItem is UnloadedItem unloadedItem ? unloadedItem.ModName : item.ModItem.Mod.Name) == AndroMod.secretsOfTheShadowsName && !Blacklist.Contains(item.type);
		new public static Color PanelColor => new Color(210, 175, 50, androLib.Common.Configs.ConfigValues.UIAlpha);
		new public static Color ScrollBarColor => new Color(40, 0, 80, androLib.Common.Configs.ConfigValues.UIAlpha);
		new public static Color ButtonHoverColor => new Color(255, 230, 120, androLib.Common.Configs.ConfigValues.UIAlpha);
		public static void RegisterWithAndroLib(Mod mod) {
			BagStorageID = StorageManager.RegisterVacuumStorageClass(
				mod,//Mod
				typeof(EarthenPyramid),//type 
				ItemAllowedToBeStored,//Is allowed function, Func<Item, bool>
				null,//Localization Key name.  Attempts to determine automatically by treating the type as a ModItem, or you can specify.
				200,//StorageSize
				true,//Can vacuum
				() => PanelColor, // Get color function. Func<using Microsoft.Xna.Framework.Color>
				() => ScrollBarColor, // Get Scroll bar color function. Func<using Microsoft.Xna.Framework.Color>
				() => ButtonHoverColor, // Get Button hover color function. Func<using Microsoft.Xna.Framework.Color>
				() => ModContent.ItemType<EarthenPyramid>(),//Get ModItem type
				80,//UI Left
				675//UI Top
			);
		}
		public override void AddRecipes() {
			if (AndroMod.secretsOfTheShadowsEnabled) {
				if (AndroMod.secretsOfTheShadowsMod.TryFind("DissolvingEarth", out ModItem dissolvingEarth)
						&& AndroMod.secretsOfTheShadowsMod.TryFind("FragmentOfEarth", out ModItem fragmentOfEarth)
						) {
					if (!VacuumBags.serverConfig.HarderBagRecipes) {
						CreateRecipe()
						.AddTile(TileID.WorkBenches)
						.AddIngredient(dissolvingEarth.Type, 1)
						.AddIngredient(fragmentOfEarth.Type, 4)
						.AddIngredient(ItemID.SandstoneBrick, 10)
						.Register();
					}
					else {
						if (AndroMod.secretsOfTheShadowsMod.TryFind("PyramidRubble", out ModItem pyramidRubble)
							&& AndroMod.secretsOfTheShadowsMod.TryFind("PyramidSlab", out ModItem pyramidSlab)
							) {
							CreateRecipe()
							.AddTile(TileID.WorkBenches)
							.AddIngredient(dissolvingEarth.Type, 1)
							.AddIngredient(fragmentOfEarth.Type, 4)
							.AddIngredient(pyramidRubble.Type, 10)
							.AddIngredient(pyramidSlab.Type, 10)
							.Register();
						}
					}
				}
			}
		}
	}
}