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
	public class FargosMementos : ModBag
	{
		public override string ModDisplayNameTooltip => "Fargos";
		public override string LocalizationDisplayName => "Fargo's Mementos";
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
		public static bool ItemAllowedToBeStored(Item item) => item?.ModItem != null && (item.ModItem is UnloadedItem unloadedItem ? unloadedItem.ModName : item.ModItem.Mod.Name).StartsWith(AndroMod.fargosModName) && !Blacklist.Contains(item.type);
		new public static Color PanelColor => new Color(226, 109, 0, androLib.Common.Configs.ConfigValues.UIAlpha);
		new public static Color ScrollBarColor => new Color(170, 40, 40, androLib.Common.Configs.ConfigValues.UIAlpha);
		new public static Color ButtonHoverColor => new Color(255, 140, 50, androLib.Common.Configs.ConfigValues.UIAlpha);
		public static void RegisterWithAndroLib(Mod mod) {
			BagStorageID = StorageManager.RegisterVacuumStorageClass(
				mod,//Mod
				typeof(FargosMementos),//type 
				ItemAllowedToBeStored,//Is allowed function, Func<Item, bool>
				null,//Localization Key name.  Attempts to determine automatically by treating the type as a ModItem, or you can specify.
				200,//StorageSize
				true,//Can vacuum
				() => PanelColor, // Get color function. Func<using Microsoft.Xna.Framework.Color>
				() => ScrollBarColor, // Get Scroll bar color function. Func<using Microsoft.Xna.Framework.Color>
				() => ButtonHoverColor, // Get Button hover color function. Func<using Microsoft.Xna.Framework.Color>
				() => ModContent.ItemType<FargosMementos>(),//Get ModItem type
				80,//UI Left
				675//UI Top
			);
		}
		public override void AddRecipes() {
			if (AndroMod.fargosEnabled) {
				if (AndroMod.fargosMod.TryFind("SuspiciousEye", out ModItem eyeThatCouldBeSeeAsSuspicious)
					&& AndroMod.fargosMod.TryFind("SlimyCrown", out ModItem slimyCrown)
					&& AndroMod.fargosMod.TryFind("AutoHouse", out ModItem autoHouse)
					) {
					if (!VacuumBags.serverConfig.HarderBagRecipes) {
						CreateRecipe()
						.AddTile(TileID.WorkBenches)
						.AddIngredient(eyeThatCouldBeSeeAsSuspicious.Type, 1)
						.AddIngredient(slimyCrown.Type, 1)
						.AddIngredient(autoHouse.Type, 1)
						.Register();
					}
					else {
						if (AndroMod.fargosMod.TryFind("SiblingPylon", out ModItem siblingPylon)
							&& AndroMod.fargosMod.TryFind("WormSnack", out ModItem wormSnack)
							&& AndroMod.fargosMod.TryFind("AttractiveOre", out ModItem attractiveOre)
							) {
							CreateRecipe()
							.AddTile(TileID.WorkBenches)
							.AddIngredient(eyeThatCouldBeSeeAsSuspicious.Type, 1)
							.AddIngredient(slimyCrown.Type, 1)
							.AddIngredient(autoHouse.Type, 1)
							.AddIngredient(siblingPylon.Type, 1)
							.AddIngredient(wormSnack.Type, 1)
							.AddIngredient(attractiveOre.Type, 1)
							.Register();
						}
					}
				}
			}
		}
	}
}