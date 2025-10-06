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
	public class FargosMementos : ModBag {
		public static IBagModItem Instance {
			get {
				if (instance == null)
					instance = new FargosMementos();

				return instance;
			}
		}
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<BagToggle>().FargosMementos && ModContent.GetInstance<BagToggle>().ModBags &&
			       (AndroMod.fargosEnabled || !ModContent.GetInstance<BagToggle>().RequireModsForModBags);
		}
		
		private static IBagModItem instance;
		public override string ModDisplayNameTooltip => "Fargos";
		public override string LocalizationDisplayName => "Fargo's Mementos";
		public override int GetBagType() => ModContent.ItemType<FargosMementos>();
		public override Color PanelColor => new Color(226, 109, 0, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ScrollBarColor => new Color(170, 40, 40, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ButtonHoverColor => new Color(255, 140, 50, androLib.Common.Configs.ConfigValues.UIAlpha);
		protected override SortedSet<string> ModNames => modNames;
		private static SortedSet<string> modNames = new() { AndroMod.fargosModName, AndroMod.fargosSoulsModName };
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