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
	public class LokisTesseract : ModBag {
		public static IBagModItem Instance {
			get {
				if (instance == null)
					instance = new LokisTesseract();

				return instance;
			}
		}
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<BagToggle>().LokisTesseract && ModContent.GetInstance<BagToggle>().ModBags &&
			       (AndroMod.thoriumEnabled || !ModContent.GetInstance<BagToggle>().RequireModsForModBags);
		}
		
		private static IBagModItem instance;
		public override string ModDisplayNameTooltip => "Thorium";
		public override string LocalizationDisplayName => "Loki's Tesseract";
		public override int GetBagType() => ModContent.ItemType<LokisTesseract>();
		public override Color PanelColor => new Color(4, 189, 189, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ScrollBarColor => new Color(180, 76, 0, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ButtonHoverColor => new Color(150, 255, 255, androLib.Common.Configs.ConfigValues.UIAlpha);
		protected override SortedSet<string> ModNames => modNames;
		private static SortedSet<string> modNames = new() { AndroMod.thoriumModName };
		public override void AddRecipes() {
			if (AndroMod.thoriumEnabled) {
				if (AndroMod.thoriumMod.TryFind("SmoothCoal", out ModItem smoothCoal)
					&& AndroMod.thoriumMod.TryFind("LifeQuartz", out ModItem lifeQuartz)
					&& AndroMod.thoriumMod.TryFind("Blood", out ModItem blood)
					) {
					if (!VacuumBags.serverConfig.HarderBagRecipes) {
						if (AndroMod.thoriumMod.TryFind("ThoriumOre", out ModItem thoriumOre))
						CreateRecipe()
						.AddTile(TileID.Anvils)
						.AddIngredient(smoothCoal.Type, 3)
						.AddIngredient(lifeQuartz.Type, 5)
						.AddIngredient(blood.Type, 2)
						.AddIngredient(thoriumOre.Type, 5)
						.Register();
					}
					else {
						if (AndroMod.thoriumMod.TryFind("ThoriumBar", out ModItem thoriumBar)
							&& AndroMod.thoriumMod.TryFind("BloodPotion", out ModItem bloodPotion)
							&& AndroMod.thoriumMod.TryFind("LivingLeaf", out ModItem livingLeaf)
							&& AndroMod.thoriumMod.TryFind("YewWood", out ModItem yewWood)
							&& AndroMod.thoriumMod.TryFind("ThoriumAnvil", out ModTile thoriumAnvil)
							) {
							CreateRecipe()
							.AddTile(thoriumAnvil.Type)
							.AddIngredient(smoothCoal.Type, 5)
							.AddIngredient(lifeQuartz.Type, 25)
							.AddIngredient(bloodPotion.Type, 2)
							.AddIngredient(thoriumBar.Type, 5)
							.AddIngredient(livingLeaf.Type, 2)
							.AddIngredient(yewWood.Type, 20)
							.Register();
						}
					}
				}
			}
		}
	}
}