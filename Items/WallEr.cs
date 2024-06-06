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
using System;
using static androLib.Items.IBagModItem;

namespace VacuumBags.Items
{
    [Autoload(false)]
	public class WallEr : AllowedListBagModItem_VB {
		public static IBagModItem Instance {
			get {
				if (instance == null)
					instance = new WallEr();

				return instance;
			}
		}
		private static IBagModItem instance;
		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
		public override void SetDefaults() {
			Item.maxStack = 1;
			Item.value = 100000;
			Item.rare = ItemRarityID.Blue;
			Item.width = 30;
			Item.height = 32;
		}
		public override int GetBagType() => ModContent.ItemType<WallEr>();
		public override void AddRecipes() {
			if (!VacuumBags.serverConfig.HarderBagRecipes) {
				CreateRecipe()
				.AddTile(TileID.Anvils)
				.AddIngredient(ItemID.CopperBar, 10)
				.AddIngredient(ItemID.WoodenBeam, 20)
				.AddIngredient(ItemID.IronFence, 20)
				.Register();
			}
			else {
				CreateRecipe()
				.AddTile(TileID.Anvils)
				.AddIngredient(ItemID.CopperBar, 50)
				.AddIngredient(ItemID.Obsidian, 10)
				.AddIngredient(ItemID.DynastyWood, 100)
				.AddIngredient(ItemID.BorealWood, 50)
				.AddIngredient(ItemID.PalmWood, 50)
				.AddIngredient(ItemID.RichMahogany, 50)
				.Register();
			}
		}
		public override Color PanelColor => new(120, 60, 10, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ScrollBarColor => new(130, 70, 10, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ButtonHoverColor => new(150, 80, 0, androLib.Common.Configs.ConfigValues.UIAlpha);
		protected override Action SelectItemForUIOnly => () => ChooseItemFromWallEr(Main.LocalPlayer);

		public static Item ChooseItemFromWallEr(Player player) => ChooseFromBag(Instance.BagStorageID, (Item item) => item.createWall > -1, player);

		public override bool? DevCheck(ItemSetInfo info, SortedSet<ItemGroup> itemGroups, SortedSet<string> endWords, SortedSet<string> searchWords) {
			return info.CreateWall;
		}
		public override SortedSet<int> DevWhiteList() {
			SortedSet<int> devWhiteList = new() {
				ItemID.WoodenBeam,
				ItemID.AdamantiteBeam,
				ItemID.BorealBeam,
				ItemID.RichMahoganyBeam,
				ItemID.MushroomBeam,
				ItemID.GraniteColumn,
				ItemID.SandstoneColumn,
				ItemID.PalladiumColumn,
				ItemID.MarbleColumn,
				ItemID.WoodenFence,
				ItemID.LeadFence,
				ItemID.EbonwoodFence,
				ItemID.RichMahoganyFence,
				ItemID.PearlwoodFence,
				ItemID.ShadewoodFence,
				ItemID.IronFence,
				ItemID.BorealWoodFence,
				ItemID.PalmWoodFence,
				ItemID.WroughtIronFence,
				ItemID.BambooFence,
				ItemID.AshWoodFence,
				ItemID.StoneWall,
				ItemID.DirtWall,
				ItemID.WoodWall,
				ItemID.GrayBrickWall,
				ItemID.RedBrickWall,
				ItemID.BlueBrickWall,
				ItemID.GreenBrickWall,
				ItemID.PinkBrickWall,
				ItemID.GoldBrickWall,
				ItemID.SilverBrickWall,
				ItemID.CopperBrickWall,
				ItemID.ObsidianBrickWall,
				ItemID.GlassWall,
				ItemID.PearlstoneBrickWall,
				ItemID.IridescentBrickWall,
				ItemID.MudstoneBrickWall,
				ItemID.CobaltBrickWall,
				ItemID.MythrilBrickWall,
				ItemID.PlankedWall,
				ItemID.PalladiumColumnWall,
				ItemID.AdamantiteBeamWall,
				ItemID.CandyCaneWall,
				ItemID.GreenCandyCaneWall,
				ItemID.SnowBrickWall,
				ItemID.DemoniteBrickWall,
				ItemID.SandstoneBrickWall,
				ItemID.EbonstoneBrickWall,
				ItemID.RedStuccoWall,
				ItemID.YellowStuccoWall,
				ItemID.GreenStuccoWall,
				ItemID.GrayStuccoWall,
				ItemID.EbonwoodWall,
				ItemID.RichMahoganyWall,
				ItemID.PearlwoodWall,
				ItemID.RainbowBrickWall,
				ItemID.TinBrickWall,
				ItemID.TungstenBrickWall,
				ItemID.PlatinumBrickWall,
				ItemID.GrassWall,
				ItemID.JungleWall,
				ItemID.FlowerWall,
				ItemID.CactusWall,
				ItemID.CloudWall,
				ItemID.MushroomWall,
				ItemID.BoneBlockWall,
				ItemID.SlimeBlockWall,
				ItemID.FleshBlockWall,
				ItemID.DiscWall,
				ItemID.IceBrickWall,
				ItemID.ShadewoodWall,
				ItemID.LihzahrdBrickWall,
				ItemID.HiveWall,
				ItemID.BlueSlabWall,
				ItemID.BlueTiledWall,
				ItemID.PinkSlabWall,
				ItemID.PinkTiledWall,
				ItemID.GreenSlabWall,
				ItemID.GreenTiledWall,
				ItemID.PalladiumColumnWall,
				ItemID.BubblegumBlockWall,
				ItemID.TitanstoneBlockWall,
				ItemID.LivingWoodWall,
				ItemID.PumpkinWall,
				ItemID.HayWall,
				ItemID.SpookyWoodWall,
				ItemID.ChristmasTreeWallpaper,
				ItemID.OrnamentWallpaper,
				ItemID.CandyCaneWallpaper,
				ItemID.FestiveWallpaper,
				ItemID.StarsWallpaper,
				ItemID.SquigglesWallpaper,
				ItemID.SnowflakeWallpaper,
				ItemID.KrampusHornWallpaper,
				ItemID.BluegreenWallpaper,
				ItemID.GrinchFingerWallpaper,
				ItemID.FancyGreyWallpaper,
				ItemID.IceFloeWallpaper,
				ItemID.MusicWallpaper,
				ItemID.PurpleRainWallpaper,
				ItemID.RainbowWallpaper,
				ItemID.SparkleStoneWallpaper,
				ItemID.StarlitHeavenWallpaper,
				ItemID.BubbleWallpaper,
				ItemID.CopperPipeWallpaper,
				ItemID.DuckyWallpaper,
				ItemID.WaterfallWall,
				ItemID.LavafallWall,
				ItemID.WhiteDynastyWall,
				ItemID.BlueDynastyWall,
				ItemID.ArcaneRuneWall,
				ItemID.CopperPlatingWall,
				ItemID.StoneSlabWall,
				ItemID.BorealWoodWall,
				ItemID.PalmWoodWall,
				ItemID.AmberGemsparkWall,
				ItemID.AmberGemsparkWallOff,
				ItemID.AmethystGemsparkWall,
				ItemID.AmethystGemsparkWallOff,
				ItemID.DiamondGemsparkWall,
				ItemID.DiamondGemsparkWallOff,
				ItemID.EmeraldGemsparkWall,
				ItemID.EmeraldGemsparkWallOff,
				ItemID.RubyGemsparkWall,
				ItemID.RubyGemsparkWallOff,
				ItemID.SapphireGemsparkWall,
				ItemID.SapphireGemsparkWallOff,
				ItemID.TopazGemsparkWall,
				ItemID.TopazGemsparkWallOff,
				ItemID.TinPlatingWall,
				ItemID.ConfettiWall,
				ItemID.ConfettiWallBlack,
				ItemID.HoneyfallWall,
				ItemID.ChlorophyteBrickWall,
				ItemID.CrimtaneBrickWall,
				ItemID.ShroomitePlatingWall,
				ItemID.MartianConduitWall,
				ItemID.HellstoneBrickWall,
				ItemID.MarbleWall,
				ItemID.MarbleBlockWall,
				ItemID.GraniteWall,
				ItemID.GraniteBlockWall,
				ItemID.MeteoriteBrickWall,
				ItemID.CrystalBlockWall,
				ItemID.SandstoneWall,
				ItemID.HardenedSandWall,
				ItemID.CorruptHardenedSandWall,
				ItemID.CrimsonHardenedSandWall,
				ItemID.HallowHardenedSandWall,
				ItemID.CorruptSandstoneWall,
				ItemID.CrimsonSandstoneWall,
				ItemID.HallowSandstoneWall,
				ItemID.DesertFossilWall,
				ItemID.LunarBrickWall,
				ItemID.LivingLeafWall,
				ItemID.CogWall,
				ItemID.SandFallWall,
				ItemID.SnowFallWall,
				ItemID.SillyBalloonPinkWall,
				ItemID.SillyBalloonPurpleWall,
				ItemID.SillyBalloonGreenWall,
				ItemID.IronBrickWall,
				ItemID.LeadBrickWall,
				ItemID.LesionBlockWall,
				ItemID.CrimstoneBrickWall,
				ItemID.SmoothSandstoneWall,
				ItemID.SpiderWall,
				ItemID.SolarBrickWall,
				ItemID.VortexBrickWall,
				ItemID.NebulaBrickWall,
				ItemID.StardustBrickWall,
				ItemID.GoldStarryGlassWall,
				ItemID.BlueStarryGlassWall,
				ItemID.MudWallEcho,
				ItemID.SnowWallEcho,
				ItemID.CaveWall1Echo,
				ItemID.CaveWall2Echo,
				ItemID.LargeBambooBlockWall,
				ItemID.BambooBlockWall,
				ItemID.AmberStoneWallEcho,
				ItemID.AshWoodWall,
				ItemID.EchoWall,
				ItemID.ReefWall,
				ItemID.SpiderWallUnsafe,
				ItemID.BlueBrickWallUnsafe,
				ItemID.BlueSlabWallUnsafe,
				ItemID.BlueTiledWallUnsafe,
				ItemID.PinkBrickWallUnsafe,
				ItemID.PinkSlabWallUnsafe,
				ItemID.PinkTiledWallUnsafe,
				ItemID.GreenBrickWallUnsafe,
				ItemID.GreenSlabWallUnsafe,
				ItemID.GreenTiledWallUnsafe,
				ItemID.SandstoneWallUnsafe,
				ItemID.HardenedSandWallUnsafe,
				ItemID.LihzahrdWallUnsafe,
				ItemID.PoopWall,
				ItemID.ShimmerWall,
				ItemID.ShimmerBrickWall,
				ItemID.LunarRustBrickWall,
				ItemID.DarkCelestialBrickWall,
				ItemID.AstraBrickWall,
				ItemID.CosmicEmberBrickWall,
				ItemID.CryocoreBrickWall,
				ItemID.MercuryBrickWall,
				ItemID.StarRoyaleBrickWall,
				ItemID.HeavenforgeBrickWall,
				ItemID.AncientBlueDungeonBrickWall,
				ItemID.AncientGreenDungeonBrickWall,
				ItemID.AncientPinkDungeonBrickWall,
				ItemID.AncientGoldBrickWall,
				ItemID.AncientSilverBrickWall,
				ItemID.AncientCopperBrickWall,
				ItemID.AncientCobaltBrickWall,
				ItemID.AncientMythrilBrickWall,
				ItemID.AncientObsidianBrickWall,
				ItemID.AncientHellstoneBrickWall,
				ItemID.LavaMossBlockWall,
				ItemID.ArgonMossBlockWall,
				ItemID.KryptonMossBlockWall,
				ItemID.XenonMossBlockWall,
				ItemID.VioletMossBlockWall,
				ItemID.RainbowMossBlockWall,
			};

			return devWhiteList;
		}
		public override SortedSet<ItemGroup> ItemGroups() {
			SortedSet<ItemGroup> itemGroups = new() {
				ItemGroup.Walls
			};

			return itemGroups;
		}
		public override SortedSet<string> EndWords() {
			SortedSet<string> endWords = new() {
				"wall",
				"wallunsafe",
				"wallpaper",
				"echo",
				"beam",
				"column",
				"fence",
			};

			return endWords;
		}

		public override SortedSet<string> SearchWords() {
			SortedSet<string> searchWords = new() {
				"slabwall",
			};

			return searchWords;
		}

		#region AndroModItem attributes that you don't need.

		public override string SummaryOfFunction => "Place-able Wall Items";
		public override string LocalizationTooltip =>
			$"Automatically stores walling materials (walls, fences, beams, etc.)\n" +
			$"When in your inventory, the contents of the bag are available for crafting.\n" +
			$"Right click to open the bag.\n" +
			$"Items can be placed from Wall-Er by left clicking with Wall-Er.  If any items in Wall-Er are favorited, only favorited items will be used.";

		public override string LocalizationDisplayName => "Wall-Er";
		public override string Artist => "@kingjoshington";
		public override string Designer => "@kingjoshington";

		#endregion
	}
}