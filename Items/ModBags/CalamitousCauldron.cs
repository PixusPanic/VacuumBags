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
	public class CalamitousCauldron : ModBag {
		public static IBagModItem Instance {
			get {
				if (instance == null)
					instance = new CalamitousCauldron();

				return instance;
			}
		}
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<BagToggle>().CalamitousCauldron && ModContent.GetInstance<BagToggle>().ModBags &&
			       (AndroMod.calamityEnabled || !ModContent.GetInstance<BagToggle>().RequireModsForModBags);
		}
		
		private static IBagModItem instance;
		public override string ModDisplayNameTooltip => "Calamity";
		public override string LocalizationDisplayName => "Calamitous Cauldron";
		public override int GetBagType() => ModContent.ItemType<CalamitousCauldron>();
		public override Color PanelColor => new Color(45, 25, 33, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ScrollBarColor => new Color(166, 32, 53, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ButtonHoverColor => new Color(89, 50, 70, androLib.Common.Configs.ConfigValues.UIAlpha);
		protected override SortedSet<string> ModNames => modNames;
		private static SortedSet<string> modNames = new() { AndroMod.calamityModName };
		public override void AddRecipes() {
			if (AndroMod.calamityEnabled) {
				if (AndroMod.calamityMod.TryFind("WulfrumMetalScrap", out ModItem wulfrumMetalcrap)
						&& AndroMod.calamityMod.TryFind("EnergyCore", out ModItem energyCore)
						&& AndroMod.calamityMod.TryFind("SeaPrism", out ModItem seaPrism)
						) {
					if (!VacuumBags.serverConfig.HarderBagRecipes) {
						CreateRecipe()
						.AddTile(TileID.WorkBenches)
						.AddIngredient(wulfrumMetalcrap.Type, 10)
						.AddIngredient(energyCore.Type, 2)
						.AddIngredient(seaPrism.Type, 10)
						.Register();
					}
					else {
						if (AndroMod.calamityMod.TryFind("SulphurousSand", out ModItem sulphurousSand)
							&& AndroMod.calamityMod.TryFind("Acidwood", out ModItem acidwood)
							&& AndroMod.calamityMod.TryFind("AerialiteOre", out ModItem aerialiteOre)
							) {
							CreateRecipe()
							.AddTile(TileID.WorkBenches)
							.AddIngredient(wulfrumMetalcrap.Type, 50)
							.AddIngredient(energyCore.Type, 5)
							.AddIngredient(seaPrism.Type, 50)
							.AddIngredient(sulphurousSand.Type, 50)
							.AddIngredient(acidwood.Type, 20)
							.AddIngredient(aerialiteOre.Type, 5)
							.Register();
						}
					}
				}
			}
		}
	}
}