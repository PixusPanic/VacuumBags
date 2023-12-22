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
	public class HoiPoiCapsule : ModBag {
		public static BagModItem Instance {
			get {
				if (instance == null)
					instance = new HoiPoiCapsule();

				return instance;
			}
		}
		private static BagModItem instance;
		public override string ModDisplayNameTooltip => "Dragon Ball Terraria";
		public override string LocalizationDisplayName => "Hoi-Poi Capsule";
		public override string Designer => "@_godslayer";
		public override string Artist => "@mountainybear49";
		public override int GetBagType() => ModContent.ItemType<HoiPoiCapsule>();
		public override Color PanelColor => new Color(230, 230, 255, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ScrollBarColor => new Color(60, 180, 255, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ButtonHoverColor => new Color(10, 20, 30, androLib.Common.Configs.ConfigValues.UIAlpha);
		protected override SortedSet<string> ModNames => modNames;
		private static SortedSet<string> modNames = new() { AndroMod.dbzTerrariaName };
		public override void AddRecipes() {
			if (AndroMod.dbzTerrariaEnabled) {
				if (AndroMod.dbzTerrariaMod.TryFind("ScrapMetal", out ModItem scrapMetal)
						&& AndroMod.dbzTerrariaMod.TryFind("StableKiCrystal", out ModItem stableKiCrystal)
						&& AndroMod.dbzTerrariaMod.TryFind("ZTable", out ModTile zTable)
						) {
					if (!VacuumBags.serverConfig.HarderBagRecipes) {
						CreateRecipe()
						.AddTile(zTable.Type)
						.AddIngredient(scrapMetal.Type, 10)
						.AddIngredient(stableKiCrystal.Type, 10)
						.Register();
					}
					else {
						if (AndroMod.dbzTerrariaMod.TryFind("CalmKiCrystal", out ModItem calmKiCrystal)
							&& AndroMod.dbzTerrariaMod.TryFind("EarthenShard", out ModItem earthenShard)
							) {
							CreateRecipe()
							.AddTile(zTable.Type)
							.AddIngredient(scrapMetal.Type, 40)
							.AddIngredient(stableKiCrystal.Type, 40)
							.AddIngredient(calmKiCrystal.Type, 20)
							.AddIngredient(earthenShard.Type, 15)
							.Register();
						}
					}
				}
			}
		}
	}
}