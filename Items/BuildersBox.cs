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

namespace VacuumBags.Items
{
	public  class BuildersBox : AndroModItem, ISoldByWitch {
		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
		public override void SetDefaults() {
            Item.maxStack = 99;
            Item.value = 100000;
			Item.rare = ItemRarityID.Blue;
			Item.width = 32;
            Item.height = 32;
        }
        public override void AddRecipes() {
			if (!VacuumBags.serverConfig.HarderBagRecipes) {
				CreateRecipe()
				.AddTile(TileID.WorkBenches)
				.AddIngredient(ItemID.RedBrick, 50)
				.AddRecipeGroup("androLib:CommonGems", 5)
				.Register();
			}
			else {
				CreateRecipe()
				.AddTile(TileID.HeavyWorkBench)
				.AddIngredient(ItemID.RedBrick, 50)
				.AddIngredient(ItemID.Diamond, 5)
				.AddIngredient(ItemID.Ruby, 5)
				.AddIngredient(ItemID.Amber, 5)
				.AddIngredient(ItemID.Cloud, 25)
				.Register();
			}
		}

		public static int BagStorageID;//Set this when registering with androLib.


		public static void RegisterWithAndroLib(Mod mod) {
			BagStorageID = StorageManager.RegisterVacuumStorageClass(
				mod,//Mod
				typeof(BuildersBox),//type 
				ItemAllowedToBeStored,//Is allowed function, Func<Item, bool>
				null,//Localization Key name.  Attempts to determine automatically by treating the type as a ModItem, or you can specify.
				100,//StorageSize
				true,//Can vacuum
				() => new Color(80, 10, 10, androLib.Common.Configs.ConfigValues.UIAlpha),//Get color function. Func<using Microsoft.Xna.Framework.Color>
				() => new Color(90, 10, 10, androLib.Common.Configs.ConfigValues.UIAlpha),//Get Scroll bar color function. Func<using Microsoft.Xna.Framework.Color>
				() => new Color(120, 0, 0, androLib.Common.Configs.ConfigValues.UIAlpha),//Get Button hover color function. Func<using Microsoft.Xna.Framework.Color>
				() => ModContent.ItemType<BuildersBox>(),//Get ModItem type
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
				ItemID.Hay,
				ItemID.CopperPlating,
				ItemID.TinPlating,
				ItemID.ShroomitePlating,
				ItemID.MartianConduitPlating,
				ItemID.AccentSlab,
				ItemID.StoneSlab,
				ItemID.SandstoneSlab,
				ItemID.RedStucco,
				ItemID.YellowStucco,
				ItemID.GreenStucco,
				ItemID.GrayStucco,
				ItemID.GraniteBlock,
				ItemID.MarbleBlock,
				ItemID.MudstoneBlock,
				ItemID.PinkSlimeBlock,
				ItemID.SlimeBlock,
				ItemID.FrozenSlimeBlock,
				ItemID.FleshBlock,
				ItemID.AsphaltBlock,
				ItemID.SunplateBlock,
				ItemID.BubblegumBlock,
				ItemID.TitanstoneBlock,
				ItemID.AmethystGemsparkBlock,
				ItemID.TopazGemsparkBlock,
				ItemID.SapphireGemsparkBlock,
				ItemID.EmeraldGemsparkBlock,
				ItemID.RubyGemsparkBlock,
				ItemID.DiamondGemsparkBlock,
				ItemID.AmberGemsparkBlock,
				ItemID.CoralstoneBlock,
				ItemID.WaterfallBlock,
				ItemID.LavafallBlock,
				ItemID.ConfettiBlock,
				ItemID.LivingFireBlock,
				ItemID.LivingCursedFireBlock,
				ItemID.LivingDemonFireBlock,
				ItemID.LivingFrostFireBlock,
				ItemID.LivingIchorBlock,
				ItemID.LivingUltrabrightFireBlock,
				ItemID.HoneyfallBlock,
				ItemID.CrystalBlock,
				ItemID.SandFallBlock,
				ItemID.SnowFallBlock,
				ItemID.SnowCloudBlock,
				ItemID.LesionBlock,
				ItemID.ShellPileBlock,
				ItemID.SpiderBlock,
				ItemID.GoldStarryGlassBlock,
				ItemID.BlueStarryGlassBlock,
				ItemID.EchoBlock,
				ItemID.LargeBambooBlock,
				ItemID.BambooBlock,
				ItemID.AmethystStoneBlock,
				ItemID.TopazStoneBlock,
				ItemID.SapphireStoneBlock,
				ItemID.EmeraldStoneBlock,
				ItemID.RubyStoneBlock,
				ItemID.DiamondStoneBlock,
				ItemID.AmberStoneBlock,
				ItemID.ReefBlock,
				ItemID.PoopBlock,
				ItemID.LavaMossBlock,
				ItemID.ArgonMossBlock,
				ItemID.KryptonMossBlock,
				ItemID.XenonMossBlock,
				ItemID.VioletMossBlock,
				ItemID.RainbowMossBlock,
				ItemID.GrayBrick,
				ItemID.RedBrick,
				ItemID.ClayBlock,
				ItemID.BlueBrick,
				ItemID.ChainLantern,
				ItemID.GreenBrick,
				ItemID.PinkBrick,
				ItemID.GoldBrick,
				ItemID.SilverBrick,
				ItemID.CopperBrick,
				ItemID.ObsidianBrick,
				ItemID.HellstoneBrick,
				ItemID.PearlstoneBrick,
				ItemID.IridescentBrick,
				ItemID.CobaltBrick,
				ItemID.MythrilBrick,
				ItemID.DemoniteBrick,
				ItemID.SnowBrick,
				ItemID.SandstoneBrick,
				ItemID.EbonstoneBrick,
				ItemID.RainbowBrick,
				ItemID.TinBrick,
				ItemID.TungstenBrick,
				ItemID.PlatinumBrick,
				ItemID.IceBrick,
				ItemID.LihzahrdBrick,
				ItemID.ChlorophyteBrick,
				ItemID.CrimtaneBrick,
				ItemID.MeteoriteBrick,
				ItemID.LunarBrick,
				ItemID.IronBrick,
				ItemID.LeadBrick,
				ItemID.CrimstoneBrick,
				ItemID.SolarBrick,
				ItemID.VortexBrick,
				ItemID.NebulaBrick,
				ItemID.StardustBrick,
				ItemID.CrackedBlueBrick,
				ItemID.CrackedGreenBrick,
				ItemID.CrackedPinkBrick,
				ItemID.ShimmerBrick,
				ItemID.LunarRustBrick,
				ItemID.DarkCelestialBrick,
				ItemID.AstraBrick,
				ItemID.CosmicEmberBrick,
				ItemID.CryocoreBrick,
				ItemID.MercuryBrick,
				ItemID.StarRoyaleBrick,
				ItemID.HeavenforgeBrick,
				ItemID.AncientBlueDungeonBrick,
				ItemID.AncientGreenDungeonBrick,
				ItemID.AncientPinkDungeonBrick,
				ItemID.AncientGoldBrick,
				ItemID.AncientSilverBrick,
				ItemID.AncientCopperBrick,
				ItemID.AncientCobaltBrick,
				ItemID.AncientMythrilBrick,
				ItemID.AncientObsidianBrick,
				ItemID.AncientHellstoneBrick,
			};

			SortedSet<string> endWords = new() {
				"plating",
				"slab",
				"stucco",
				"brick",
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
			}
		}

		#region AndroModItem attributes that you don't need.

		public virtual SellCondition SellCondition => SellCondition.Never;
		public virtual float SellPriceModifier => 1f;
		public override List<WikiTypeID> WikiItemTypes => new() { WikiTypeID.Storage };
		public override string LocalizationTooltip =>
			$"Automatically stores building materials (bricks, craftable blocks, etc.)\n" +
			$"When in your inventory, the contents of the bag are available for crafting.\n" +
			$"Right click to open the bag.";
		public override string Artist => "andro951";
		public override string Designer => "@kingjoshington";

		#endregion
	}
}
