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
    [Autoload(false)]
	public class AmmoBag : BagModItem, INeedsSetUpAllowedList {
		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
		public override void SetDefaults() {
            Item.maxStack = 1;
            Item.value = 100000;
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
		protected static int DefaultBagSize => 100;
		public static void RegisterWithAndroLib(Mod mod) {
			BagStorageID = StorageManager.RegisterVacuumStorageClass(
				mod,//Mod
				typeof(AmmoBag),//type 
				ItemAllowedToBeStored,//Is allowed function, Func<Item, bool>
				null,//Localization Key name.  Attempts to determine automatically by treating the type as a ModItem, or you can specify.
				-DefaultBagSize,//StorageSize
				true,//Can vacuum
				() => new Color(80, 80, 80, androLib.Common.Configs.ConfigValues.UIAlpha),   // Get color function. Func<using Microsoft.Xna.Framework.Color>
				() => new Color(90, 90, 90, androLib.Common.Configs.ConfigValues.UIAlpha),   // Get Scroll bar color function. Func<using Microsoft.Xna.Framework.Color>
				() => new Color(120, 120, 120, androLib.Common.Configs.ConfigValues.UIAlpha), // Get Button hover color function. Func<using Microsoft.Xna.Framework.Color>
				() => ModContent.ItemType<AmmoBag>(),//Get ModItem type
				80,//UI Left
				675,//UI Top
				UpdateAllowedList,
				false,
				() => Main.LocalPlayer.ChooseAmmo(Main.LocalPlayer.HeldItem)
			);
		}
		public static bool ItemAllowedToBeStored(Item item) => AllowedItems.Contains(item.type);
		public static Item OnChooseAmmo(On_Player.orig_ChooseAmmo orig, Player self, Item weapon) {
			Item item = orig(self, weapon);
			if (Main.netMode == NetmodeID.Server)
				return item;

			if (weapon.useAmmo == AmmoID.None)
				return null;

			Item bagAmmo = ChooseAmmoFromBag(item, weapon, self);

			return bagAmmo ?? item;
		}
		private static Item ChooseAmmoFromBag(Item vanillaChosenItem, Item weapon, Player player) {
			return ChooseFromBagOnlyIfFirstInInventory(
				vanillaChosenItem,
				player,
				BagStorageID,
				(Item item) => ItemLoader.CanChooseAmmo(weapon, item, player)
			);
		}
		internal static void OnItemCheck_ApplyHoldStyle_Inner(ILContext il) {
			//IL_01ee: ldloc.s 4
			//IL_01f0: ldc.i4 931
			//IL_01f5: beq.s IL_021f

			var c = new ILCursor(il);

			if (!c.TryGotoNext(MoveType.Before,
				i => i.MatchLdloc(4),
				i => i.MatchLdcI4(ItemID.Flare)
			)) { throw new Exception("Failed to find instructions OnItemCheck_ApplyHoldStyle_Inner 1/2"); }

			if (!c.TryGotoNext(MoveType.Before,
				i => i.MatchLdcI4(ItemID.Flare)
			)) { throw new Exception("Failed to find instructions OnItemCheck_ApplyHoldStyle_Inner 2/2"); }

			c.Emit(OpCodes.Ldarg_0);
			c.Emit(OpCodes.Ldarg_2);
			c.EmitDelegate((int flairType, Player player, Item heldItem) => {
				if (heldItem.useAmmo != ItemID.Flare)
					return flairType;

				Item vanillaChosenItem = flairType > ItemID.None ? new(flairType) : null;
				Item bagAmmo = ChooseAmmoFromBag(vanillaChosenItem, heldItem, player);

				if (bagAmmo != null)
					flairType = bagAmmo.type;

				return flairType;
			});
		}
		internal static void OnSmartSelect_PickToolForStrategy(ILContext il) {
			//IL_01b5: ldloc.2
			//IL_01b6: brfalse IL_0398

			//// this.SmartSelect_SelectItem(i);
			//IL_01bb: ldarg.0
			//IL_01bc: ldloc.0
			//IL_01bd: call instance void Terraria.Player::SmartSelect_SelectItem(int32)
			//// return;
			//IL_01c2: ret

			var c = new ILCursor(il);

			if (!c.TryGotoNext(MoveType.Before,
				i => i.MatchLdloc(2),
				i => i.MatchBrfalse(out _),
				i => i.MatchLdarg(0),
				i => i.MatchLdloc(0),
				i => i.MatchCall<Player>("SmartSelect_SelectItem")
			)) { throw new Exception("Failed to find instructions OnSmartSelect_PickToolForStrategy 1/2"); }

			if (!c.TryGotoNext(MoveType.Before,
				i => i.MatchBrfalse(out _),
				i => i.MatchLdarg(0),
				i => i.MatchLdloc(0),
				i => i.MatchCall<Player>("SmartSelect_SelectItem")
			)) { throw new Exception("Failed to find instructions OnSmartSelect_PickToolForStrategy 2/2"); }

			c.Emit(OpCodes.Ldarg_0);

			c.EmitDelegate(HasFlairGunAmmo);

			if (!c.TryGotoNext(MoveType.Before,
				i => i.MatchLdloc(5),
				i => i.MatchBrfalse(out _),
				i => i.MatchLdarg(0),
				i => i.MatchLdloc(0),
				i => i.MatchCall<Player>("SmartSelect_SelectItem")
			)) { throw new Exception("Failed to find instructions OnSmartSelect_PickToolForStrategy 3/4"); }

			if (!c.TryGotoNext(MoveType.Before,
				i => i.MatchBrfalse(out _),
				i => i.MatchLdarg(0),
				i => i.MatchLdloc(0),
				i => i.MatchCall<Player>("SmartSelect_SelectItem")
			)) { throw new Exception("Failed to find instructions OnSmartSelect_PickToolForStrategy 4/4"); }

			c.Emit(OpCodes.Ldarg_0);

			c.EmitDelegate(HasFlairGunAmmo);
		}
		private static bool HasFlairGunAmmo(bool hasFlare, Player player) {
			if (hasFlare)
				return hasFlare;

			Item bagAmmo = ChooseFromBag(BagStorageID, (Item item) => item.ammo == ItemID.Flare, player);

			if (bagAmmo != null)
				hasFlare = true;

			return hasFlare;
		}

		private static void UpdateAllowedList(int item, bool add) {
			if (add) {
				AllowedItems.Add(item);
			}
			else {
				AllowedItems.Remove(item);
			}
		}
		public static SortedSet<int> AllowedItems => AllowedItemsManager.AllowedItems;
		public static AllowedItemsManager AllowedItemsManager = new(ModContent.ItemType<AmmoBag>, () => BagStorageID, DevCheck, DevWhiteList, DevModWhiteList, DevBlackList, DevModBlackList, ItemGroups, EndWords, SearchWords);
		public AllowedItemsManager GetAllowedItemsManager => AllowedItemsManager;
		protected static bool? DevCheck(ItemSetInfo info, SortedSet<ItemGroup> itemGroups, SortedSet<string> endWords, SortedSet<string> searchWords) {
			if (VacuumBags.clientConfig.AllAmmoItemsGoIntoAmmoBag && (info.Ammo || info.CheckItemGroup(ItemGroup.Ammo)))
				return true;

			if (info.ConsumableWeapon)
				return true;

			if (info.Bomb)
				return true;

			return null;
		}
		protected static SortedSet<int> DevWhiteList() {
			SortedSet<int> devWhiteList = new() {
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
				ItemID.DirtSolution,
				ItemID.Bomb,
				ItemID.Dynamite,
				ItemID.HolyWater,
				ItemID.UnholyWater,
				ItemID.Cannonball,
				ItemID.Flare,
				ItemID.BlueFlare,
			};

			return devWhiteList;
		}
		protected static SortedSet<string> DevModWhiteList() {
			SortedSet<string> devModWhiteList = new() {

			};

			return devModWhiteList;
		}
		protected static SortedSet<int> DevBlackList() {
			SortedSet<int> devBlackList = new() {
				ItemID.CopperCoin,
				ItemID.SilverCoin,
				ItemID.GoldCoin,
				ItemID.PlatinumCoin,
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
				"bomb",
				"dynamite"
			};

			return endWords;
		}

		protected static SortedSet<string> SearchWords() {
			SortedSet<string> searchWords = new() {
				
			};

			return searchWords;
		}

		#region AndroModItem attributes that you don't need.

		public override string SummaryOfFunction => "Weapon Ammo";
		public override string LocalizationTooltip =>
			$"Automatically stores throwables, arrows, bullets, flares, solutions.\n" +
			$"When in your inventory, the contents of the bag are available for crafting.\n" +
			$"Right click to open the bag.\n" +
			$"Ammo in the bag is used if the ammo bag is in the first ammo item found.\n" +
			$"If any ammo in the bag that can be used by your equipped weapon is favorited, only favorited ammos will be used.";
		public override string Artist => "anodomani";
		public override string Designer => "@kingjoshington";

		#endregion
	}
}
