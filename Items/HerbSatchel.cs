using androLib.Common.Utility;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;
using Microsoft.Xna.Framework;
using androLib.Items;
using androLib.Common.Globals;
using androLib;
using static Terraria.ID.ContentSamples.CreativeHelper;

namespace VacuumBags.Items
{
	public  class HerbSatchel : VBModItem, ISoldByWitch {
		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
		public override void SetDefaults() {
            Item.maxStack = 1;
            Item.value = 100000;
			Item.rare = ItemRarityID.Blue;
			Item.width = 32;
            Item.height = 32;
        }
        public override void AddRecipes() {
			if (!VacuumBags.serverConfig.HarderBagRecipes) {
				CreateRecipe()
				.AddTile(TileID.WorkBenches)
				.AddIngredient(ItemID.Leather, 2)
				.AddIngredient(ItemID.GrassSeeds, 3)
				.AddIngredient(ItemID.DaybloomSeeds, 3)
				.AddIngredient(ItemID.MoonglowSeeds, 3)
				.Register();
			}
			else {
				CreateRecipe()
				.AddTile(TileID.HeavyWorkBench)
				.AddIngredient(ItemID.Leather, 5)
				.AddIngredient(ItemID.Sunflower, 10)
				.AddIngredient(ItemID.GrassSeeds, 10)
				.AddIngredient(ItemID.MushroomGrassSeeds, 10)
				.AddIngredient(ItemID.JungleGrassSeeds, 10)
				.AddIngredient(ItemID.DaybloomSeeds, 10)
				.AddIngredient(ItemID.MoonglowSeeds, 10)
				.AddIngredient(ItemID.BlinkrootSeeds, 10)
				.AddIngredient(ItemID.DeathweedSeeds, 10)
				.AddIngredient(ItemID.WaterleafSeeds, 10)
				.AddIngredient(ItemID.FireblossomSeeds, 10)
				.AddIngredient(ItemID.ShiverthornSeeds, 10)
				.Register();
			}
		}

		public static int BagStorageID;//Set this when registering with androLib.


		public static void RegisterWithAndroLib(Mod mod) {
			BagStorageID = StorageManager.RegisterVacuumStorageClass(
				mod,//Mod
				typeof(HerbSatchel),//type 
				ItemAllowedToBeStored,//Is allowed function, Func<Item, bool>
				null,//Localization Key name.  Attempts to determine automatically by treating the type as a ModItem, or you can specify.
				100,//StorageSize
				true,//Can vacuum
				() => new Color(10, 80, 10, androLib.Common.Configs.ConfigValues.UIAlpha),    // Get color function. Func<using Microsoft.Xna.Framework.Color>
				() => new Color(10, 90, 10, androLib.Common.Configs.ConfigValues.UIAlpha),    // Get Scroll bar color function. Func<using Microsoft.Xna.Framework.Color>
				() => new Color(0, 120, 0, androLib.Common.Configs.ConfigValues.UIAlpha),    // Get Button hover color function. Func<using Microsoft.Xna.Framework.Color>
				() => ModContent.ItemType<HerbSatchel>(),//Get ModItem type
				80,//UI Left
				675//UI Top
			);
		}
		public static bool ItemAllowedToBeStored(Item item) => AllowedItems.Contains(item.type);

		public static SortedSet<int> AllowedItems {
			get {
				if (allowedItems == null)
					GetAllowedItems();

				return allowedItems;
			}
		}
		private static SortedSet<int> allowedItems = null;

		private static void GetAllowedItems() {
			allowedItems = new() {
				ItemID.GrassSeeds,
				ItemID.CorruptSeeds,
				ItemID.CrimsonSeeds,
				ItemID.MushroomGrassSeeds,
				ItemID.JungleGrassSeeds,
				ItemID.HallowedSeeds,
				ItemID.AshGrassSeeds,
				ItemID.DaybloomSeeds,
				ItemID.MoonglowSeeds,
				ItemID.BlinkrootSeeds,
				ItemID.DeathweedSeeds,
				ItemID.WaterleafSeeds,
				ItemID.FireblossomSeeds,
				ItemID.ShiverthornSeeds,
				ItemID.PumpkinSeed,
				ItemID.DayBloomPlanterBox,
				ItemID.MoonglowPlanterBox,
				ItemID.CorruptPlanterBox,
				ItemID.CrimsonPlanterBox,
				ItemID.BlinkrootPlanterBox,
				ItemID.WaterleafPlanterBox,
				ItemID.ShiverthornPlanterBox,
				ItemID.FireBlossomPlanterBox,
				ItemID.Acorn,
				ItemID.Sunflower,
				ItemID.VileMushroom,
				ItemID.Mushroom,
				ItemID.GlowingMushroom,
				ItemID.Pumpkin,
				ItemID.StrangePlant1,
				ItemID.StrangePlant2,
				ItemID.StrangePlant3,
				ItemID.StrangePlant4,
				ItemID.FlowerPacketBlue,
				ItemID.FlowerPacketMagenta,
				ItemID.FlowerPacketPink,
				ItemID.FlowerPacketRed,
				ItemID.FlowerPacketYellow,
				ItemID.FlowerPacketViolet,
				ItemID.FlowerPacketWhite,
				ItemID.FlowerPacketTallGrass,
			};

			SortedSet<string> endWords = new() {
				
			};

			SortedSet<string> searchWords = new() {
				
			};

			for (int i = 0; i < ItemLoader.ItemCount; i++) {
				Item item = ContentSamples.ItemsByType[i];
				if (item.NullOrAir())
					continue;

				string lowerName = item.Name.ToLower();
				bool added = false;
				foreach (string endWord in endWords) {
					if (lowerName.EndsWith(endWord)) {
						allowedItems.Add(item.type);
						added = true;
						break;
					}
				}

				if (added)
					continue;

				foreach (string searchWord in searchWords) {
					if (lowerName.Contains(searchWord)) {
						allowedItems.Add(item.type);
						added = true;
						break;
					}
				}

				if (ItemID.Sets.GrassSeeds[item.type]) {
					allowedItems.Add(item.type);
					continue;
				}

				if (ItemID.Sets.flowerPacketInfo[item.type] != null) {
					allowedItems.Add(item.type);
					continue;
				}

				if (ItemID.Sets.ExoticPlantsForDyeTrade[item.type]) {
					allowedItems.Add(item.type);
					continue;
				}

				ItemGroupAndOrderInGroup group = new ItemGroupAndOrderInGroup(item);
				if (group.Group == ItemGroup.AlchemyPlants || group.Group == ItemGroup.AlchemySeeds || group.Group == ItemGroup.DyeMaterial) {
					allowedItems.Add(item.type);
					continue;
				}
			}
		}

		#region AndroModItem attributes that you don't need.

		public virtual SellCondition SellCondition => SellCondition.Never;
		public virtual float SellPriceModifier => 1f;
		public override List<WikiTypeID> WikiItemTypes => new() { WikiTypeID.Storage };
		public override string LocalizationTooltip =>
			$"Automatically stores seeds, herbs, flowers, etc.\n" +
			$"When in your inventory, the contents of the bag are available for crafting.\n" +
			$"Right click to open the bag.";
		public override string Artist => "andro951";
		public override string Designer => "@kingjoshington";

		#endregion
	}
}
