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
using static Terraria.ID.ContentSamples.CreativeHelper;

namespace VacuumBags.Items
{
	[Autoload(false)]
	public class EarthenPyramid : ModBag {
		public static IBagModItem Instance {
			get {
				if (instance == null)
					instance = new EarthenPyramid();

				return instance;
			}
		}
		private static IBagModItem instance;
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();
			Item.width = 30;
		}
		public override string ModDisplayNameTooltip => "Secrets of the Shadows";
		public override string Designer => "@level12lobster";
		public override string Artist => "@level12lobster";
		public override int GetBagType() => ModContent.ItemType<EarthenPyramid>();
		public override Color PanelColor => new Color(210, 175, 50, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ScrollBarColor => new Color(40, 0, 80, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ButtonHoverColor => new Color(255, 230, 120, androLib.Common.Configs.ConfigValues.UIAlpha);
		protected override SortedSet<string> ModNames => modNames;
		private static SortedSet<string> modNames = new() { AndroMod.secretsOfTheShadowsName };
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