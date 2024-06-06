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
using androLib.Items;
using static Terraria.ID.ContentSamples.CreativeHelper;

namespace VacuumBags.Items
{
	[Autoload(false)]
	public class EssenceOfGathering : ModBag {
		public static IBagModItem Instance {
			get {
				if (instance == null)
					instance = new EssenceOfGathering();

				return instance;
			}
		}
		private static IBagModItem instance;
		public override string ModDisplayNameTooltip => "Stars Above";
		public override string LocalizationDisplayName => "Essence of Gathering";
		public override string Artist => "Stars Above";
		public override int GetBagType() => ModContent.ItemType<EssenceOfGathering>();
		public override Color PanelColor => new Color(152, 59, 137, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ScrollBarColor => new Color(44, 60, 180, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ButtonHoverColor => new Color(200, 75, 160, androLib.Common.Configs.ConfigValues.UIAlpha);
		protected override SortedSet<string> ModNames => modNames;
		private static SortedSet<string> modNames = new() { AndroMod.starsAboveModName };
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