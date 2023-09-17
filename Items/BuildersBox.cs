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
using System;
using static Terraria.ModLoader.PlayerDrawLayer;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using static Terraria.ID.ContentSamples.CreativeHelper;

namespace VacuumBags.Items
{
	[Autoload(false)]
	public  class BuildersBox : BagModItem, ISoldByWitch, INeedsSetUpAllowedList
	{
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
				.AddIngredient(ItemID.RedBrick, 50)
				.AddRecipeGroup($"{AndroMod.ModName}:{AndroModSystem.AnyCommonGem}", 5)
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
				675,//UI Top
				() => ChooseItemFromBox(Main.LocalPlayer)
			);
		}
		public static bool ItemAllowedToBeStored(Item item) => AllowedItems.Contains(item.type);
		public static void RegisterWithGadgetGalore() {
			if (!VacuumBags.gadgetGaloreEnabled)
				return;

			VacuumBags.GadgetGalore.Call("RegisterBuildInventory", () => StorageManager.GetItems(BagStorageID).Where(item => item.NullOrAir()));
		}
		public static Item ChooseItemFromBox(Player player) => ChooseFromBag(BagStorageID, (Item item) => item.createTile > -1, player);

		public static SortedSet<int> AllowedItems => AllowedItemsManager.AllowedItems;
		public static AllowedItemsManager AllowedItemsManager = new(DevCheck, DevWhiteList, DevModWhiteList, DevBlackList, DevModBlackList, ItemGroups, EndWords, SearchWords);
		public AllowedItemsManager GetAllowedItemsManager => AllowedItemsManager;
		protected static bool? DevCheck(ItemSetInfo info, SortedSet<ItemGroup> itemGroups, SortedSet<string> endWords, SortedSet<string> searchWords) {
			if (info.Equipment)
				return false;

			if (!info.CreateTile)
				return false;

			if (info.RequiredTile)
				return false;

			return null;
		}
		protected static SortedSet<int> DevWhiteList() {
			SortedSet<int> devWhiteList = new() {
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
				ItemID.LesionBlock,
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
				ItemID.LavaMossBlock,
				ItemID.ArgonMossBlock,
				ItemID.KryptonMossBlock,
				ItemID.XenonMossBlock,
				ItemID.VioletMossBlock,
				ItemID.RainbowMossBlock,
				ItemID.GrayBrick,
				ItemID.RedBrick,
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
				ItemID.SmoothSandstone,
				ItemID.RedDynastyShingles,
				ItemID.BlueDynastyShingles,
				ItemID.Glass,
				ItemID.Sign,
				ItemID.Piano,
				ItemID.Bench,
				ItemID.TikiTorch,
				ItemID.TrashCan,
				ItemID.PinkVase,
				ItemID.Throne,
				ItemID.Mannequin,
				ItemID.CandyCaneBlock,
				ItemID.GreenCandyCaneBlock,
				ItemID.BlueLight,
				ItemID.RedLight,
				ItemID.GreenLight,
				ItemID.RichMahogany,
				ItemID.BoneBlock,
				ItemID.Cannon,
				ItemID.SnowballLauncher,
				ItemID.BunnyCannon,
				ItemID.Cog,
				ItemID.DungeonShelf,
				ItemID.BubbleMachine,
				ItemID.Hay,
				ItemID.Jackelier,
				ItemID.PineTreeBlock,
				ItemID.ChristmasTree,
				ItemID.Womannquin,
				ItemID.CoralstoneBlock,
				ItemID.FireworksBox,
				ItemID.FireworkFountain,
				ItemID.MartianConduitPlating,
				ItemID.SmokeBlock,
				ItemID.Bubble,
				ItemID.ItemFrame,
				ItemID.Fireplace,
				ItemID.Chimney,
				ItemID.ConfettiCannon,
				ItemID.PortalGunStation,
				ItemID.PixelBox,
				ItemID.LesionBlock,
				ItemID.AntiPortalBlock,
				ItemID.FoodPlatter,
				ItemID.PlasmaLamp,
				ItemID.FogMachine,
			};

			foreach (int itemType in RecipeGroup.recipeGroups[RecipeGroupID.Wood].ValidItems) {
				devWhiteList.Add(itemType);
			}

			return devWhiteList;
		}
		protected static SortedSet<string> DevModWhiteList() {
			SortedSet<string> devModWhiteList = new() {

			};

			return devModWhiteList;
		}
		protected static SortedSet<int> DevBlackList() {
			SortedSet<int> devBlackList = new() {

			};

			return devBlackList;
		}
		protected static SortedSet<string> DevModBlackList() {
			SortedSet<string> devModBlackList = new() {
				"CalamityMod/ThrowingBrick"
			};

			return devModBlackList;
		}
		protected static SortedSet<ItemGroup> ItemGroups() {
			SortedSet<ItemGroup> itemGroups = new() {
				ItemGroup.PlacableObjects
			};

			return itemGroups;
		}
		protected static SortedSet<string> EndWords() {
			SortedSet<string> endWords = new() {
				
			};

			return endWords;
		}
		protected static SortedSet<string> SearchWords() {
			SortedSet<string> searchWords = new() {

			};

			return searchWords;
		}

		#region AndroModItem attributes that you don't need.

		public virtual SellCondition SellCondition => SellCondition.Never;
		public virtual float SellPriceModifier => 1f;
		public override List<WikiTypeID> WikiItemTypes => new() { WikiTypeID.Storage };
		public override string LocalizationTooltip =>
		$"Automatically stores building materials (bricks, craftable blocks, etc.)\n" +
		$"When in your inventory, the contents of the bag are available for crafting.\n" +
		$"Right click to open the bag.\n" +
		$"Items can be placed from the box by left clicking with the box.  If any items in the box are favorited, only favorited items will be used.";
		public override string Artist => "anodomani";
		public override string Designer => "@kingjoshington";

		#endregion
	}
}
