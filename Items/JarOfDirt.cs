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
using Terraria.Localization;
using static Terraria.ID.ContentSamples.CreativeHelper;
using static androLib.Items.IBagModItem;

namespace VacuumBags.Items
{
    [Autoload(false)]
	public  class JarOfDirt : AllowedListBagModItem_VB {
		public static IBagModItem Instance {
			get {
				if (instance == null)
					instance = new JarOfDirt();

				return instance;
			}
		}
		private static IBagModItem instance;
		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
		public override void SetDefaults() {
            Item.maxStack = 1;
            Item.value = 100000;
			Item.rare = ItemRarityID.Blue;
			Item.width = 24;
            Item.height = 32;
		}
		public override int GetBagType() => ModContent.ItemType<JarOfDirt>();
		public override void AddRecipes() {
			if (!VacuumBags.serverConfig.HarderBagRecipes) {
				CreateRecipe()
				.AddIngredient(ItemID.Glass, 10)
				.AddIngredient(ItemID.DirtBlock, 20)
				.AddIngredient(ItemID.LifeCrystal, 1)
				.Register();
			}
			else {
				CreateRecipe()
				.AddIngredient(ItemID.Glass, 20)
				.AddIngredient(ItemID.DirtBlock, 100)
				.AddIngredient(ItemID.CrimsonHeart, 1)
				.Register();

				CreateRecipe()
				.AddIngredient(ItemID.Glass, 20)
				.AddIngredient(ItemID.DirtBlock, 100)
				.AddIngredient(ItemID.ShadowOrb, 1)
				.Register();
			}
		}
		public override Color PanelColor => new Color(42, 28, 1, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ScrollBarColor => new Color(33, 19, 0, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ButtonHoverColor => new Color(92, 71, 5, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Action SelectItemForUIOnly => () => ChooseItemFromJar(Main.LocalPlayer);
		
		public static Item ChooseItemFromJar(Player player) => ChooseFromBag(Instance.BagStorageID, (Item item) => item.createTile > -1, player);

		public override bool? DevCheck(ItemSetInfo info, SortedSet<ItemGroup> itemGroups, SortedSet<string> endWords, SortedSet<string> searchWords) {
			if (!info.CreateTile)
				return false;
			
			if (info.Equipment)
				return false;

			if (info.Extractable)
				return true;

			return null;
		}
		public override SortedSet<int> DevWhiteList() {
			SortedSet<int> devWhiteList = new() {
				ItemID.DirtBlock,
				ItemID.StoneBlock,
				ItemID.LifeCrystal,
				ItemID.Cobweb,
				ItemID.EbonstoneBlock,
				ItemID.ClayBlock,
				ItemID.SandBlock,
				ItemID.AshBlock,
				ItemID.MudBlock,
				ItemID.Coral,
				ItemID.Cactus,
				ItemID.EbonsandBlock,
				ItemID.PearlsandBlock,
				ItemID.PearlstoneBlock,
				ItemID.MudstoneBlock,
				ItemID.SiltBlock,
				ItemID.SnowBlock,
				ItemID.Cloud,
				ItemID.RainCloud,
				ItemID.CrimstoneBlock,
				ItemID.SlushBlock,
				ItemID.Hive,
				ItemID.HoneyBlock,
				ItemID.CrispyHoneyBlock,
				ItemID.CrimsandBlock,
				ItemID.LifeFruit,
				ItemID.Seashell,
				ItemID.Starfish,
				ItemID.Marble,
				ItemID.Granite,
				ItemID.Sandstone,
				ItemID.HardenedSand,
				ItemID.CorruptHardenedSand,
				ItemID.CrimsonHardenedSand,
				ItemID.CorruptSandstone,
				ItemID.CrimsonSandstone,
				ItemID.HallowHardenedSand,
				ItemID.HallowSandstone,
				ItemID.SnowCloudBlock,
				ItemID.JunoniaShell,
				ItemID.LightningWhelkShell,
				ItemID.LightningWhelkShell,
				ItemID.ShellPileBlock,
				ItemID.EucaluptusSap,
				ItemID.ThinIce,
				ItemID.PoopBlock,
				ItemID.CrimsonHeart,
				ItemID.DemonHeart,
				ItemID.HeartHairpin
			};

			foreach (int itemType in RecipeGroup.recipeGroups[RecipeGroupID.Sand].ValidItems) {
				devWhiteList.Add(itemType);
			}

			return devWhiteList;
		}
		public override SortedSet<string> EndWords() {
			SortedSet<string> endWords = new() {
				"iceblock",
				"moss"
			};

			return endWords;
		}


		#region AndroModItem attributes that you don't need.

		public override string SummaryOfFunction => "Place-able Items";
		public override string LocalizationDisplayName => "Jar of Dirt";
		public override string LocalizationTooltip =>
		$"Automatically stores dirt and other natural blocks (dirt, mud, clay, sand, stone, ice, etc.)\n" +
		$"When in your inventory, the contents of the jar are available for crafting.\n" +
		$"Right click to open the jar.\n" +
		$"Items can be placed from the jar by left clicking with the jar.  If any items in the jar are favorited, only favorited items will be used.\n" +
			$"\n" +
			$"..........guess what's inside it....";
		public override string Artist => "andro951";
		public override string Designer => "andro951";

		#endregion
	}
}
