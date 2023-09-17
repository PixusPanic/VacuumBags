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

namespace VacuumBags.Items
{
	[Autoload(false)]
	public  class PaintBucket : BagModItem, ISoldByWitch, INeedsSetUpAllowedList
	{
		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
		public override void SetDefaults() {
            Item.maxStack = 1;
            Item.value = 100000;
			Item.rare = ItemRarityID.Blue;
			Item.width = 32;
            Item.height = 28;
			Item.ammo = Type;
		}
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

		public static int BagStorageID;//Set this when registering with androLib.


		public static void RegisterWithAndroLib(Mod mod) {
			BagStorageID = StorageManager.RegisterVacuumStorageClass(
				mod,//Mod
				typeof(PaintBucket),//type 
				ItemAllowedToBeStored,//Is allowed function, Func<Item, bool>
				null,//Localization Key name.  Attempts to determine automatically by treating the type as a ModItem, or you can specify.
				100,//StorageSize
				true,//Can vacuum
				() => new Color(245, 245, 220, androLib.Common.Configs.ConfigValues.UIAlpha),  // Get color function. Func<using Microsoft.Xna.Framework.Color>
				() => new Color(255, 255, 230, androLib.Common.Configs.ConfigValues.UIAlpha),  // Get Scroll bar color function. Func<using Microsoft.Xna.Framework.Color>
				() => new Color(255, 250, 240, androLib.Common.Configs.ConfigValues.UIAlpha),  // Get Button hover color function. Func<using Microsoft.Xna.Framework.Color>
				() => ModContent.ItemType<PaintBucket>(),//Get ModItem type
				80,//UI Left
				675,//UI Top
				() => ChoosePaintFromBucket(Main.LocalPlayer)
			);
		}
		public static bool ItemAllowedToBeStored(Item item) => AllowedItems.Contains(item.type);
		public static void RegisterWithGadgetGalore() {
			if (!VacuumBags.gadgetGaloreEnabled)
				return;

			VacuumBags.GadgetGalore.Call("RegisterPaintInventory", () => StorageManager.GetItems(BagStorageID).Where(item => item.NullOrAir()));
		}

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
		private static Item ChoosePaintFromBucket(Player player) => ChooseFromBag(BagStorageID, (Item item) => item.PaintOrCoating, player, selectItems: false);
		internal static void OnItemCheck_CheckCanUse(ILContext il) {
			var c = new ILCursor(il);
			/*
	// if (sItem.type == 1071 || sItem.type == 1072)
	IL_02ae: ldarg.1
	IL_02af: ldfld int32 Terraria.Item::'type'
	IL_02b4: ldc.i4 1071
	IL_02b9: beq.s IL_02c8

	IL_02bb: ldarg.1
	IL_02bc: ldfld int32 Terraria.Item::'type'
	IL_02c1: ldc.i4 1072
	IL_02c6: bne.un.s IL_02f7

	// bool flag2 = false;
	IL_02c8: ldc.i4.0
	IL_02c9: stloc.s 9
	// for (int i = 0; i < 58; i++)
	IL_02cb: ldc.i4.0
	IL_02cc: stloc.s 10
	// if (this.inventory[i].PaintOrCoating)
	IL_02ce: br.s IL_02eb 
			*/
			if (!c.TryGotoNext(MoveType.Before,
				i => i.MatchLdcI4(0),
				i => i.MatchStloc(9)
			)) { throw new Exception("Failed to find instructions PaintBucket.OnItemCheck_CheckCanUse()"); }
			c.Index++;

			c.EmitDelegate((bool hasPaint) => {
				int PaintBucketID = ModContent.ItemType<AmmoBag>();
				if (!Main.LocalPlayer.HasItem(PaintBucketID))
					return hasPaint;

				foreach (Item item in StorageManager.GetItems(BagStorageID)) {
					if (!item.NullOrAir() && item.stack > 0 && item.PaintOrCoating)
						return true;
				}

				return hasPaint;
			});
		}
		
		public static SortedSet<int> AllowedItems => AllowedItemsManager.AllowedItems;
		public static AllowedItemsManager AllowedItemsManager = new(DevCheck, DevWhiteList, DevModWhiteList, DevBlackList, DevModBlackList, ItemGroups, EndWords, SearchWords);
		public AllowedItemsManager GetAllowedItemsManager => AllowedItemsManager;
		protected static bool? DevCheck(ItemSetInfo info, SortedSet<ItemGroup> itemGroups, SortedSet<string> endWords, SortedSet<string> searchWords) {
			if (info.Equipment)
				return false;

			return null;
		}
		protected static SortedSet<int> DevWhiteList() {
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
		protected static SortedSet<string> DevModWhiteList() {
			SortedSet<string> devModWhiteList = new() {
				"GadgetGalore/BucketOfPaintTools",
				"GadgetGalore/GhostlyPainter",
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
				ItemGroup.Paint,
				ItemGroup.Dye,
				ItemGroup.DyeMaterial
			};

			return itemGroups;
		}
		protected static SortedSet<string> EndWords() {
			SortedSet<string> endWords = new() {
				"paint",
				"coating",
				"dye"
			};

			return endWords;
		}

		protected static SortedSet<string> SearchWords() {
			SortedSet<string> searchWords = new() {
				"paintbrush",
				"paintroller",
				"paintscraper"
			};

			return searchWords;
		}

		#region AndroModItem attributes that you don't need.

		public virtual SellCondition SellCondition => SellCondition.Never;
		public virtual float SellPriceModifier => 1f;
		public override List<WikiTypeID> WikiItemTypes => new() { WikiTypeID.Storage };
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
