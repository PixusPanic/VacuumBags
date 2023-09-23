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
	public class SpookyGourd : ModBag {
		public override string ModDisplayNameTooltip => "Spooky Mod";
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
		public static bool ItemAllowedToBeStored(Item item) => item?.ModItem != null && (item.ModItem is UnloadedItem unloadedItem ? unloadedItem.ModName : item.ModItem.Mod.Name) == AndroMod.spookyModName && !Blacklist.Contains(item.type);
		new public static Color PanelColor => new Color(252, 134, 21, androLib.Common.Configs.ConfigValues.UIAlpha);
		new public static Color ScrollBarColor => new Color(0, 90, 65, androLib.Common.Configs.ConfigValues.UIAlpha);
		new public static Color ButtonHoverColor => new Color(255, 180, 40, androLib.Common.Configs.ConfigValues.UIAlpha);
		public static void RegisterWithAndroLib(Mod mod) {
			BagStorageID = StorageManager.RegisterVacuumStorageClass(
				mod,//Mod
				typeof(SpookyGourd),//type 
				ItemAllowedToBeStored,//Is allowed function, Func<Item, bool>
				null,//Localization Key name.  Attempts to determine automatically by treating the type as a ModItem, or you can specify.
				200,//StorageSize
				true,//Can vacuum
				() => PanelColor, // Get color function. Func<using Microsoft.Xna.Framework.Color>
				() => ScrollBarColor, // Get Scroll bar color function. Func<using Microsoft.Xna.Framework.Color>
				() => ButtonHoverColor, // Get Button hover color function. Func<using Microsoft.Xna.Framework.Color>
				() => ModContent.ItemType<SpookyGourd>(),//Get ModItem type
				80,//UI Left
				675//UI Top
			);
		}
		public override void AddRecipes() {
			if (AndroMod.spookyModEnabled) {
				if (AndroMod.spookyMod.TryFind("RottenSeed", out ModItem rottenSeed)
						&& AndroMod.spookyMod.TryFind("SpookySeedsOrange", out ModItem spookySeedsOrange)
						&& AndroMod.spookyMod.TryFind("SpookySeedsGreen", out ModItem spookySeedsGreen)
						&& AndroMod.spookyMod.TryFind("RottenChunk", out ModItem moldyChunks)
						) {
					if (!VacuumBags.serverConfig.HarderBagRecipes) {
						CreateRecipe()
						.AddTile(TileID.WorkBenches)
						.AddIngredient(rottenSeed.Type, 1)
						.AddIngredient(spookySeedsOrange.Type, 3)
						.AddIngredient(spookySeedsGreen.Type, 3)
						.AddIngredient(moldyChunks.Type, 5)
						.Register();
					}
					else {
						if (AndroMod.spookyMod.TryFind("EyeChocolate", out ModItem eyeChocolate)
							&& AndroMod.spookyMod.TryFind("CreepyChunk", out ModItem creepyChunk)
							) {
							CreateRecipe()
							.AddTile(TileID.WorkBenches)
							.AddIngredient(rottenSeed.Type, 1)
							.AddIngredient(spookySeedsOrange.Type, 3)
							.AddIngredient(spookySeedsGreen.Type, 3)
							.AddIngredient(moldyChunks.Type, 5)
							.AddIngredient(eyeChocolate.Type, 1)
							.AddIngredient(creepyChunk.Type, 5)
							.Register();
						}
					}
				}
			}
		}
	}
}