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
using MonoMod.Cil;
using VacuumBags.Common.Configs;
using static androLib.Items.IBagModItem;

namespace VacuumBags.Items
{
    [Autoload(false)]
	public  class PaintBucket : AllowedListBagModItem_VB {
		public static IBagModItem Instance {
			get {
				if (instance == null)
					instance = new PaintBucket();

				return instance;
			}
		}
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<BagToggle>().PaintBucket;
		}
		
		private static IBagModItem instance;
		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
		public override void SetDefaults() {
            Item.maxStack = 1;
            Item.value = 100000;
			Item.rare = ItemRarityID.Blue;
			Item.width = 32;
            Item.height = 28;
			Item.ammo = Type;
		}
		public override int GetBagType() => ModContent.ItemType<PaintBucket>();
		public override void AddRecipes() {
			if (!VacuumBags.serverConfig.HarderBagRecipes) {
				CreateRecipe()
				.AddTile(TileID.WorkBenches)
				.AddIngredient(ItemID.EmptyBucket, 1)
				.AddIngredient(ItemID.Paintbrush, 1)
				.AddIngredient(ItemID.PaintRoller, 1)
				.AddIngredient(ItemID.FallenStar, 10)
				.Register();
			}
			else {
				CreateRecipe()
				.AddTile(TileID.HeavyWorkBench)
				.AddIngredient(ItemID.EmptyBucket, 1)
				.AddIngredient(ItemID.PaintSprayer, 1)
				.AddIngredient(ItemID.FallenStar, 30)
				.AddIngredient(ItemID.RainbowDye, 1)
				.Register();
			}
		}

		public override Color PanelColor => new Color(245, 245, 220, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ScrollBarColor => new Color(255, 255, 230, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ButtonHoverColor => new Color(255, 250, 240, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Action SelectItemForUIOnly => () => ChoosePaintFromBucket(Main.LocalPlayer);

		internal static Item OnFindPaintOrCoating(On_Player.orig_FindPaintOrCoating orig, Player self) {
			Item item = orig(self);
			int AmmoBagID = ModContent.ItemType<AmmoBag>();
			if (!self.HasItem(AmmoBagID))
				return item;

			Item fromBag = ChoosePaintFromBucket(self);
			if (item == null) {
				return fromBag;
			}
			else {
				if (fromBag == null || self.whoAmI != Main.myPlayer)
					return item;

				Item[] inventory = self.inventory;
				for (int j = 54; j < 58; j++) {
					if (inventory[j].type == AmmoBagID)
						return fromBag;

					if (inventory[j].stack > 0 && item.PaintOrCoating) {
						return item;
					}
				}

				for (int k = 0; k < 54; k++) {
					if (inventory[k].type == AmmoBagID)
						return fromBag;

					if (inventory[k].stack > 0 && item.PaintOrCoating) {
						return item;
					}
				}
			}

			return item;
		}
		private static Item ChoosePaintFromBucket(Player player) => ChooseFromBag(Instance.BagStorageID, (Item item) => item.PaintOrCoating, player);

		public override bool? DevCheck(ItemSetInfo info, SortedSet<ItemGroup> itemGroups, SortedSet<string> endWords, SortedSet<string> searchWords) {
			if (info.Equipment)
				return false;

			return null;
		}
		public override SortedSet<int> DevWhiteList() {
			SortedSet<int> devWhiteList = new() {
				ItemID.GlowPaint,
				ItemID.RedPaint,
				ItemID.OrangePaint,
				ItemID.YellowPaint,
				ItemID.LimePaint,
				ItemID.GreenPaint,
				ItemID.TealPaint,
				ItemID.CyanPaint,
				ItemID.SkyBluePaint,
				ItemID.BluePaint,
				ItemID.PurplePaint,
				ItemID.VioletPaint,
				ItemID.PinkPaint,
				ItemID.DeepRedPaint,
				ItemID.DeepOrangePaint,
				ItemID.DeepYellowPaint,
				ItemID.DeepLimePaint,
				ItemID.DeepGreenPaint,
				ItemID.DeepTealPaint,
				ItemID.DeepCyanPaint,
				ItemID.DeepSkyBluePaint,
				ItemID.DeepBluePaint,
				ItemID.DeepPurplePaint,
				ItemID.DeepVioletPaint,
				ItemID.DeepPinkPaint,
				ItemID.BlackPaint,
				ItemID.WhitePaint,
				ItemID.GrayPaint,
				ItemID.BrownPaint,
				ItemID.ShadowPaint,
				ItemID.NegativePaint,
				ItemID.EchoCoating,
				ItemID.TealMushroom,
				ItemID.GreenMushroom,
				ItemID.SkyBlueFlower,
				ItemID.YellowMarigold,
				ItemID.BlueBerries,
				ItemID.LimeKelp,
				ItemID.PinkPricklyPear,
				ItemID.OrangeBloodroot,
				ItemID.RedHusk,
				ItemID.CyanHusk,
				ItemID.VioletHusk,
				ItemID.BlackInk,
				ItemID.StrangePlant1,
				ItemID.StrangePlant2,
				ItemID.StrangePlant3,
				ItemID.StrangePlant4,
			};

			foreach (int itemType in ItemID.Sets.NonColorfulDyeItems) {
				devWhiteList.Add(itemType);
			}

			return devWhiteList;
		}
		public override SortedSet<string> DevModWhiteList() {
			SortedSet<string> devModWhiteList = new() {
				"GadgetGalore/BucketOfPaintTools",
				"GadgetGalore/GhostlyPainter",
			};

			return devModWhiteList;
		}
		public override SortedSet<ItemGroup> ItemGroups() {
			SortedSet<ItemGroup> itemGroups = new() {
				ItemGroup.Paint,
				ItemGroup.Dye,
				ItemGroup.DyeMaterial
			};

			return itemGroups;
		}
		public override SortedSet<string> EndWords() {
			SortedSet<string> endWords = new() {
				"paint",
				"coating",
				"dye"
			};

			return endWords;
		}

		public override SortedSet<string> SearchWords() {
			SortedSet<string> searchWords = new() {
				"paintbrush",
				"paintroller",
				"paintscraper"
			};

			return searchWords;
		}

		#region AndroModItem attributes that you don't need.

		public override string SummaryOfFunction => "Paints";
		public override string LocalizationTooltip =>
			$"Automatically stores paint\n" +
			$"When in your inventory, the contents of the bucket are available for crafting.\n" +
			$"Right click to open the bucket.\n" +
			$"Paint in the bucket is used if the paint bucket is in the first paint item found.\n" +
			$"If any paint in the bucket that can be used by your paint tool is favorited, only favorited paints will be used.";
		public override string Artist => "@kingjoshington";
		public override string Designer => "@kingjoshington";

		#endregion
	}
}
