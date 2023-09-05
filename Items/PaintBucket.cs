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
	public  class PaintBucket : VBModItem, ISoldByWitch {
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
				675//UI Top
			);
		}
		public static bool ItemAllowedToBeStored(Item item) => AllowedItems.Contains(item.type);
		public static void RegisterWithGadgetGalore() {
			if (!VacuumBags.gadgetGaloreEnabled)
				return;

			VacuumBags.GadgetGalore.Call("RegisterPaintInventory", () => GetPaintsFromCan().ToArray());
		}

		internal static Item OnFindPaintOrCoating(On_Player.orig_FindPaintOrCoating orig, Player self) {
			Item item = orig(self);
			int AmmoBagID = ModContent.ItemType<AmmoBag>();
			if (!self.HasItem(AmmoBagID))
				return item;

			Item fromBag = ChoosePaintFromCan(self);
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

		private static IEnumerable<Item> GetPaintsFromCan() {
			IEnumerable<Item> items = StorageManager.GetItems(BagStorageID).Where(item => !item.NullOrAir() && item.stack > 0 && item.PaintOrCoating);
			if (!items.Any())
				return new Item[0];

			if (items.AnyFavoritedItem())
				items = items.Where(item => item.favorited);

			return items;
		}
		private static Item ChoosePaintFromCan(Player player) {
			if (player.whoAmI != Main.myPlayer)
				return null;

			IEnumerable<Item> items = GetPaintsFromCan();
			if (!items.Any())
				return null;

			return items.First();
		}

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
			};

			SortedSet<string> endWords = new() {
				"paint",
				"coating"
			};

			SortedSet<string> searchWords = new() {
				
			};

			SortedSet<string> modItems = new() {
				"GadgetGalore/BucketOfPaintTools",
				"GadgetGalore/GhostlyPainter",
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

				ItemGroupAndOrderInGroup group = new ItemGroupAndOrderInGroup(item);
				if (group.Group == ItemGroup.Paint) {
					allowedItems.Add(item.type);
					continue;
				}

				if (i < ItemID.Count)
					continue;

				if (modItems.Contains(item.ModFullName())) {
					allowedItems.Add(item.type);
					continue;
				}
			}

			foreach (int blackListItemType in BlackList) {
				allowedItems.Remove(blackListItemType);
			}
		}
		public static SortedSet<int> BlackList {
			get {
				if (blackList == null)
					GetBlackList();

				return blackList;
			}
		}
		private static SortedSet<int> blackList = null;
		private static void GetBlackList() {
			blackList = new() {
				ModContent.ItemType<PaintBucket>(),
			};
		}

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

		#region AndroModItem attributes that you don't need.

		public virtual SellCondition SellCondition => SellCondition.Never;
		public virtual float SellPriceModifier => 1f;
		public override List<WikiTypeID> WikiItemTypes => new() { WikiTypeID.Storage };
		public override string LocalizationTooltip =>
			$"Automatically stores paint\n" +
			$"When in your inventory, the contents of the bag are available for crafting.\n" +
			$"Right click to open the bag.\n" +
			$"Paint in the can is used if the Paint Can is in the first paint item found.\n" +
			$"If any paint in the can that can be used by your paint tool is favorited, only favorited paints will be used.";
		public override string Artist => "@kingjoshington";
		public override string Designer => "@kingjoshington";

		#endregion
	}
}
