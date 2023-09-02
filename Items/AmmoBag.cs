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
using MonoMod.Cil;
using System;
using Mono.Cecil.Cil;

namespace VacuumBags.Items
{
	public class AmmoBag : VBModItem, ISoldByWitch {
		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
		public override void SetDefaults() {
            Item.maxStack = 1;
            Item.value = ContentSamples.ItemsByType[ItemID.AmmoBox].value;
			Item.rare = ItemRarityID.Blue;
			Item.width = 28;
            Item.height = 30;
			Item.ammo = Type;
        }
        public override void AddRecipes() {
			if (!VacuumBags.serverConfig.HarderBagRecipes) {
				CreateRecipe()
				.AddTile(TileID.WorkBenches)
				.AddIngredient(ItemID.Leather, 2)
				.AddIngredient(ItemID.WoodenArrow, 100)
				.AddIngredient(ItemID.Shuriken, 100)
				.Register();
			}
			else {
				CreateRecipe()
				.AddTile(TileID.AmmoBox)
				.AddIngredient(ItemID.Leather, 10)
				.AddIngredient(ItemID.JestersArrow, 100)
				.AddIngredient(ItemID.Flare, 50)
				.AddIngredient(ItemID.BoneDagger, 75)
				.AddIngredient(ItemID.FrostDaggerfish, 75)
				.AddIngredient(ItemID.MusketBall, 100)
				.Register();
			}
        }

		public static int BagStorageID;//Set this when registering with androLib.

		public static void RegisterWithAndroLib(Mod mod) {
			BagStorageID = StorageManager.RegisterVacuumStorageClass(
				mod,//Mod
				typeof(AmmoBag),//type 
				ItemAllowedToBeStored,//Is allowed function, Func<Item, bool>
				null,//Localization Key name.  Attempts to determine automatically by treating the type as a ModItem, or you can specify.
				100,//StorageSize
				true,//Can vacuum
				() => new Color(80, 80, 80, androLib.Common.Configs.ConfigValues.UIAlpha),   // Get color function. Func<using Microsoft.Xna.Framework.Color>
				() => new Color(90, 90, 90, androLib.Common.Configs.ConfigValues.UIAlpha),   // Get Scroll bar color function. Func<using Microsoft.Xna.Framework.Color>
				() => new Color(120, 120, 120, androLib.Common.Configs.ConfigValues.UIAlpha), // Get Button hover color function. Func<using Microsoft.Xna.Framework.Color>
				() => ModContent.ItemType<AmmoBag>(),//Get ModItem type
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
				ItemID.EmptyBullet,
				ItemID.WoodenArrow,
				ItemID.FlamingArrow,
				ItemID.UnholyArrow,
				ItemID.JestersArrow,
				ItemID.HellfireArrow,
				ItemID.HolyArrow,
				ItemID.CursedArrow,
				ItemID.FrostburnArrow,
				ItemID.ChlorophyteArrow,
				ItemID.IchorArrow,
				ItemID.VenomArrow,
				ItemID.BoneArrow,
				ItemID.MoonlordArrow,
				ItemID.ShimmerArrow,
				ItemID.MeteorShot,
				ItemID.MusketBall,
				ItemID.SilverBullet,
				ItemID.CrystalBullet,
				ItemID.CursedBullet,
				ItemID.ChlorophyteBullet,
				ItemID.HighVelocityBullet,
				ItemID.IchorBullet,
				ItemID.VenomBullet,
				ItemID.PartyBullet,
				ItemID.NanoBullet,
				ItemID.ExplodingBullet,
				ItemID.GoldenBullet,
				ItemID.MoonlordBullet,
				ItemID.TungstenBullet,
				ItemID.RocketI,
				ItemID.RocketII,
				ItemID.RocketIII,
				ItemID.RocketIV,
				ItemID.RedRocket,
				ItemID.GreenRocket,
				ItemID.BlueRocket,
				ItemID.YellowRocket,
				ItemID.ClusterRocketI,
				ItemID.ClusterRocketII,
				ItemID.WetRocket,
				ItemID.LavaRocket,
				ItemID.HoneyRocket,
				ItemID.DryRocket,
				ItemID.MiniNukeI,
				ItemID.MiniNukeII,
				ItemID.Flare,
				ItemID.BlueFlare,
				ItemID.SpelunkerFlare,
				ItemID.CursedFlare,
				ItemID.RainbowFlare,
				ItemID.ShimmerFlare,
				ItemID.PoisonDart,
				ItemID.CrystalDart,
				ItemID.CursedDart,
				ItemID.IchorDart,
				ItemID.ThrowingKnife,
				ItemID.PoisonedKnife,
				ItemID.BoneDagger,
				ItemID.Javelin,
				ItemID.BoneJavelin,
				ItemID.SpikyBall,
				ItemID.Seed,
				ItemID.Shuriken,
				ItemID.StarAnise,
				ItemID.Grenade,
				ItemID.StickyGrenade,
				ItemID.BouncyGrenade,
				ItemID.Beenade,
				ItemID.PartyGirlGrenade,
				ItemID.RottenEgg,
				ItemID.Snowball,
				ItemID.FrostDaggerfish,
				ItemID.MolotovCocktail,
				ItemID.Stake,
				ItemID.Nail,
				ItemID.StyngerBolt,
				ItemID.CandyCorn,
				ItemID.ExplosiveJackOLantern,
				ItemID.GreenSolution,
				ItemID.BlueSolution,
				ItemID.PurpleSolution,
				ItemID.DarkBlueSolution,
				ItemID.RedSolution,
				ItemID.SandSolution,
				ItemID.SnowSolution,
				ItemID.DirtSolution
			};

			SortedSet<string> endWords = new() {
				
			};

			SortedSet<string> searchWords = new() {
				
			};

			for (int i = 0; i < ItemLoader.ItemCount; i++) {
				Item item = ContentSamples.ItemsByType[i];
				if (item.NullOrAir())
					continue;

				if (VacuumBags.clientConfig.AllAmmoItemsGoIntoAmmoBag && item.ammo != AmmoID.None) {
					allowedItems.Add(item.type);
					continue;
				}

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
				if (group.Group == ItemGroup.Solutions || VacuumBags.clientConfig.AllAmmoItemsGoIntoAmmoBag && group.Group == ItemGroup.Ammo) {
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
				ModContent.ItemType<AmmoBag>(),
				ModContent.ItemType<PaintBucket>(),
				ItemID.CopperCoin,
				ItemID.SilverCoin,
				ItemID.GoldCoin,
				ItemID.PlatinumCoin,
			};
		}
		public static Item OnChooseAmmo(On_Player.orig_ChooseAmmo orig, Player self, Item weapon) {
			Item item = orig(self, weapon);
			int AmmoBagID = ModContent.ItemType<AmmoBag>();
			if (!self.HasItem(AmmoBagID))
				return item;

			Item fromBag = ChooseAmmoFromBag(self, weapon);
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

					if (inventory[j].stack > 0 && ItemLoader.CanChooseAmmo(weapon, inventory[j], self)) {
						return item;
					}
				}

				for (int k = 0; k < 54; k++) {
					if (inventory[k].type == AmmoBagID)
						return fromBag;

					if (inventory[k].stack > 0 && ItemLoader.CanChooseAmmo(weapon, inventory[k], self)) {
						return item;
					}
				}
			}

			return item;
		}

		private static Item ChooseAmmoFromBag(Player player, Item weapon) {
			if (player.whoAmI != Main.myPlayer)
				return null;

			foreach (Item item in StorageManager.GetItems(BagStorageID)) {
				if (!item.NullOrAir() && item.stack > 0 && ItemLoader.CanChooseAmmo(weapon, item, player))
					return item;
			}
			
			return null;
		}

		internal static void OnDrawItemSlot(ILContext il) {
			var c = new ILCursor(il);
			/*
	// if (item.useAmmo > 0)
	IL_0cc8: ldloc.1
	IL_0cc9: ldfld int32 Terraria.Item::useAmmo
	IL_0cce: ldc.i4.0
	IL_0ccf: ble.s IL_0d13

	// _ = item.useAmmo;
	IL_0cd1: ldloc.1
	IL_0cd2: ldfld int32 Terraria.Item::useAmmo
	IL_0cd7: pop
	// num11 = 0;
	IL_0cd8: ldc.i4.0
	IL_0cd9: stloc.s 29
			 */
			//Note to self.  All instructions have to be perfectly in order if using TryGotoNext.  Use multiple calls of TryGotoNext if you need to skip instructions.
			if (!c.TryGotoNext(MoveType.After,
				i => i.MatchLdloc(1),
				i => i.MatchLdfld<Item>("useAmmo"),
				i => i.MatchPop(),
				i => i.MatchLdcI4(0)
			)) { throw new Exception("Failed to find instructions OnDrawItemSlot 1/2"); }

			//Also works for jumping over instructions.
			/*if (!c.TryGotoNext(MoveType.After,
				//i => i.MatchLdloc(1),
				i => i.MatchLdfld<Item>("useAmmo"),
				i => i.MatchLdcI4(0)
			)) { throw new Exception("Failed to find instructions OnDrawItemSlot 1/2"); }

			if (!c.TryGotoNext(MoveType.After,
				i => i.MatchLdcI4(0)
			)) { throw new Exception("Failed to find instructions OnDrawItemSlot 2/2"); }*/
			//c.LogRest(10);
			c.Emit(OpCodes.Ldloc, 1);

			c.EmitDelegate((int ammoCount, Item weapon) => {
				int AmmoBagID = ModContent.ItemType<AmmoBag>();
				if (!Main.LocalPlayer.HasItem(AmmoBagID))
					return ammoCount;

				foreach (Item item in StorageManager.GetItems(BagStorageID)) {
					if (!item.NullOrAir() && item.stack > 0 && ItemLoader.CanChooseAmmo(weapon, item, Main.LocalPlayer))
						ammoCount += item.stack;
				}

				return ammoCount;
			});
		}

		#region AndroModItem attributes that you don't need.

		public virtual SellCondition SellCondition => SellCondition.Never;
		public virtual float SellPriceModifier => 1f;
		public override List<WikiTypeID> WikiItemTypes => new() { WikiTypeID.Storage };
		public override string LocalizationTooltip =>
			$"Automatically stores throwables, arrows, bullets, flares, solutions.\n" +
			$"When in your inventory, the contents of the bag are available for crafting.\n" +
			$"Right click to open the bag.";
		public override string Artist => "andro951";
		public override string Designer => "@kingjoshington";

		#endregion
	}
}
