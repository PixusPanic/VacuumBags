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
using androLib.Items;
using VacuumBags.Common.Configs;
using static Terraria.ID.ContentSamples.CreativeHelper;

namespace VacuumBags.Items
{
	[Autoload(false)]
	public class SpookyGourd : ModBag {
		public static IBagModItem Instance {
			get {
				if (instance == null)
					instance = new SpookyGourd();

				return instance;
			}
		}
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<BagToggle>().SpookyGourd && ModContent.GetInstance<BagToggle>().ModBags &&
			       (AndroMod.spookyModEnabled || !ModContent.GetInstance<BagToggle>().RequireModsForModBags);
		}
		
		private static IBagModItem instance;
		public override string ModDisplayNameTooltip => "Spooky Mod";
		public override string Designer => "@level12lobster";
		public override string Artist => "@level12lobster";
		public override int GetBagType() => ModContent.ItemType<SpookyGourd>();
		public override Color PanelColor => new Color(252, 134, 21, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ScrollBarColor => new Color(0, 90, 65, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ButtonHoverColor => new Color(255, 180, 40, androLib.Common.Configs.ConfigValues.UIAlpha);
		protected override SortedSet<string> ModNames => modNames;
		private static SortedSet<string> modNames = new() { AndroMod.spookyModName };
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