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
using androLib.Common.Globals;

namespace VacuumBags.Items
{
	[Autoload(false)]
	public class EssenceOfGathering : ModBag
	{
		public override string ModDisplayNameTooltip => "Stars Above";
		public override string LocalizationDisplayName => "Essence of Gathering";
		public override string Artist => "Stars Above";
		new public static int BagStorageID;
		public static SortedSet<int> Blacklist {
			get {
				if (blacklist == null)
					GetBlackList();

				return blacklist;
			}
		}
		private static SortedSet<int> blacklist = null;
		private static void GetBlackList() {
			blacklist = new() {

			};

			SortedSet<string> modItemBlacklist = new() {
				"StarsAbove/Starlight"
			};

			for (int i = ItemID.Count; i < ItemLoader.ItemCount; i++) {
				Item item = ContentSamples.ItemsByType[i];
				if (modItemBlacklist.Contains(item.ModFullName()))
					blacklist.Add(item.type);
			}
		}
		public static bool ItemAllowedToBeStored(Item item) => item?.ModItem != null && (item.ModItem is UnloadedItem unloadedItem ? unloadedItem.ModName : item.ModItem.Mod.Name) == AndroMod.starsAboveModName && !Blacklist.Contains(item.type);
		new public static Color PanelColor => new Color(152, 59, 137, androLib.Common.Configs.ConfigValues.UIAlpha);
		new public static Color ScrollBarColor => new Color(44, 60, 180, androLib.Common.Configs.ConfigValues.UIAlpha);
		new public static Color ButtonHoverColor => new Color(200, 75, 160, androLib.Common.Configs.ConfigValues.UIAlpha);
		public static void RegisterWithAndroLib(Mod mod) {
			BagStorageID = StorageManager.RegisterVacuumStorageClass(
				mod,//Mod
				typeof(EssenceOfGathering),//type 
				ItemAllowedToBeStored,//Is allowed function, Func<Item, bool>
				null,//Localization Key name.  Attempts to determine automatically by treating the type as a ModItem, or you can specify.
				200,//StorageSize
				true,//Can vacuum
				() => PanelColor, // Get color function. Func<using Microsoft.Xna.Framework.Color>
				() => ScrollBarColor, // Get Scroll bar color function. Func<using Microsoft.Xna.Framework.Color>
				() => ButtonHoverColor, // Get Button hover color function. Func<using Microsoft.Xna.Framework.Color>
				() => ModContent.ItemType<EssenceOfGathering>(),//Get ModItem type
				80,//UI Left
				675//UI Top
			);
		}
		public override void AddRecipes() {
			if (AndroMod.starsAboveEnabled) {
				if (!VacuumBags.serverConfig.HarderBagRecipes) {
					CreateRecipe()
					.AddTile(TileID.WorkBenches)
					.AddRecipeGroup($"{AndroMod.ModName}:{AndroModSystem.StarsAboveAnyKingSlimeEssence}", 1)
					.AddIngredient(ItemID.Glass, 10)
					.Register();
				}
				else {
					CreateRecipe()
					.AddTile(TileID.WorkBenches)
					.AddRecipeGroup($"{AndroMod.ModName}:{AndroModSystem.StarsAboveAnyKingSlimeEssence}", 1)
					.AddRecipeGroup($"{AndroMod.ModName}:{AndroModSystem.StarsAboveAnyEyeOfCthulhuEssence}", 1)
					.AddRecipeGroup($"{AndroMod.ModName}:{AndroModSystem.StarsAboveAnyEaterOfWorldsOrBrainOfCthulhuEssence}", 1)
					.AddRecipeGroup($"{AndroMod.ModName}:{AndroModSystem.StarsAboveAnyQueenBeeEssence}", 1)
					.AddRecipeGroup($"{AndroMod.ModName}:{AndroModSystem.StarsAboveAnySkeletronEssence}", 1)
					.AddIngredient(ItemID.Glass, 40)
					.Register();
				}
			}
		}
	}
}