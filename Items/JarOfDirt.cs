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
using Ionic.Zlib;

namespace VacuumBags.Items
{
    [Autoload(false)]
	public  class JarOfDirt : BagModItem, ISoldByWitch, INeedsSetUpAllowedList
	{
		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
		public override void SetDefaults() {
            Item.maxStack = 1;
            Item.value = 100000;
			Item.rare = ItemRarityID.Blue;
			Item.width = 24;
            Item.height = 32;
        }
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

		public static int BagStorageID;//Set this when registering with androLib.


		public static void RegisterWithAndroLib(Mod mod) {
			BagStorageID = StorageManager.RegisterVacuumStorageClass(
				mod,//Mod
				typeof(JarOfDirt),//type 
				ItemAllowedToBeStored,//Is allowed function, Func<Item, bool>
				null,//Localization Key name.  Attempts to determine automatically by treating the type as a ModItem, or you can specify.
				100,//StorageSize
				true,//Can vacuum
				() => new Color(42, 28, 1, androLib.Common.Configs.ConfigValues.UIAlpha),//Get color function. Func<using Microsoft.Xna.Framework.Color>
				() => new Color(33, 19, 0, androLib.Common.Configs.ConfigValues.UIAlpha),//Get Scroll bar color function. Func<using Microsoft.Xna.Framework.Color>
				() => new Color(92, 71, 5, androLib.Common.Configs.ConfigValues.UIAlpha),//Get Button hover color function. Func<using Microsoft.Xna.Framework.Color>
				() => ModContent.ItemType<JarOfDirt>(),//Get ModItem type
				80,//UI Left
				675,//UI Top
				() => AllowedItems,
				false,
				() => ChooseItemFromJar(Main.LocalPlayer)
			);
		}
		public static bool ItemAllowedToBeStored(Item item) => AllowedItems.Contains(item.type);
		public static void RegisterWithGadgetGalore() {
			if (!VacuumBags.gadgetGaloreEnabled)
				return;

			VacuumBags.GadgetGalore.Call("RegisterBuildInventory", () => StorageManager.GetItems(BagStorageID));
		}
		public static Item ChooseItemFromJar(Player player) => ChooseFromBag(BagStorageID, (Item item) => item.createTile > -1, player);

		public static SortedSet<int> AllowedItems => AllowedItemsManager.AllowedItems;
		public static AllowedItemsManager AllowedItemsManager = new(ModContent.ItemType<JarOfDirt>, () => BagStorageID, DevCheck, DevWhiteList, DevModWhiteList, DevBlackList, DevModBlackList, ItemGroups, EndWords, SearchWords);
		public AllowedItemsManager GetAllowedItemsManager => AllowedItemsManager;
		protected static bool? DevCheck(ItemSetInfo info, SortedSet<ItemGroup> itemGroups, SortedSet<string> endWords, SortedSet<string> searchWords) {
			if (!info.CreateTile)
				return false;
			
			if (info.Equipment)
				return false;

			if (info.Extractable)
				return true;

			return null;
		}
		protected static SortedSet<int> DevWhiteList() {
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

			};

			return devModBlackList;
		}
		protected static SortedSet<ItemGroup> ItemGroups() {
			SortedSet<ItemGroup> itemGroups = new() {

			};

			return itemGroups;
		}
		protected static SortedSet<string> EndWords() {
			SortedSet<string> endWords = new() {
				"iceblock",
				"moss"
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
