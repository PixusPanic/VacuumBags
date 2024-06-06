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
using androLib.UI;
using System.Reflection;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using rail;
using Terraria.Audio;
using static androLib.Items.IBagModItem;

namespace VacuumBags.Items
{
    [Autoload(false)]
	public  class FishingBelt : AllowedListBagModItem_VB {
		public static IBagModItem Instance {
			get {
				if (instance == null)
					instance = new FishingBelt();

				return instance;
			}
		}
		private static IBagModItem instance;
		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
		public override void SetDefaults() {
            Item.maxStack = 1;
            Item.value = 100000;
			Item.rare = ItemRarityID.Blue;
			Item.width = 32;
            Item.height = 26;
			Item.ammo = Type;
		}
		public override int GetBagType() => ModContent.ItemType<FishingBelt>();
		public override void AddRecipes() {
			if (!VacuumBags.serverConfig.HarderBagRecipes) {
				CreateRecipe()
				.AddTile(TileID.Loom)
				.AddIngredient(ItemID.Silk, 2)
				.AddIngredient(ItemID.Rope, 5)
				.AddIngredient(ItemID.WhiteString, 1)
				.AddIngredient(ItemID.WoodFishingPole, 1)
				.Register();
			}
			else {
				CreateRecipe()
				.AddTile(TileID.Loom)
				.AddIngredient(ItemID.Silk, 5)
				.AddIngredient(ItemID.Rope, 30)
				.AddIngredient(ItemID.WhiteString, 3)
				.AddIngredient(ItemID.WoodFishingPole, 1)
				.AddIngredient(ItemID.Hook, 1)
				.Register();
			}
		}
		public override Color PanelColor => new Color(38, 38, 67, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ScrollBarColor => new Color(46, 31, 18, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ButtonHoverColor => new Color(92, 122, 173, androLib.Common.Configs.ConfigValues.UIAlpha);
		protected override Action SelectItemForUIOnly => () => typeof(Player).GetMethod("Fishing_GetBait", BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(Main.LocalPlayer, new object[] { null });
		protected override bool ShouldUpdateInfoAccessories => true;

		internal static void OnFishing_GetBait(On_Player.orig_Fishing_GetBait orig, Player self, out Item bait) {
			orig(self, out bait);
			Item bagBait = GetBaitFromBag(bait, self);

			if (bagBait != null)
				bait = bagBait;
		}
		private static Item GetBaitFromBag(Item vanillaBait, Player player) {
			return ChooseFromBagOnlyIfFirstInInventory(
				vanillaBait,
				player,
				Instance.BagStorageID,
				(Item item) => item.bait > 0
			);
		}
		private static Item chosenBaitToConsume = null;
		internal static void OnItemCheck_CheckFishingBobber_PickAndConsumeBait(ILContext il) {
			//IL_0041: ldloc.0
			//IL_007e: ldc.i4.m1
			//IL_007f: bgt.s IL_0082

			//IL_0081: ret

			var c = new ILCursor(il);

			if (!c.TryGotoNext(MoveType.Before,
				i => i.MatchLdcI4(-1),
				i => i.MatchBgt(out _),
				i => i.MatchRet()
			)) { throw new Exception("Failed to find instructions OnItemCheck_CheckFishingBobber_PickAndConsumeBait 1/2"); }

			c.Emit(OpCodes.Ldloca, 0);
			c.Emit(OpCodes.Ldarg_0);

			c.EmitDelegate((int loadedInventoryIndex, ref int inventoryIndex, Player player) => {
				chosenBaitToConsume = null;
				Item vanillaBait = inventoryIndex >= 0 ? player.inventory[inventoryIndex] : null;
				Item bait = GetBaitFromBag(vanillaBait, player);
				if (bait != null) {
					inventoryIndex = 0;
					chosenBaitToConsume = bait;
				}

				return inventoryIndex;
			});

			//IL_008a: stloc.1
			//IL_008b: ldc.i4.0
			//IL_008c: stloc.2

			if (!c.TryGotoNext(MoveType.Before,
				i => i.MatchStloc(1),
				i => i.MatchLdcI4(0),
				i => i.MatchStloc(2)
			)) { throw new Exception("Failed to find instructions OnItemCheck_CheckFishingBobber_PickAndConsumeBait 2/2"); }

			c.EmitDelegate((Item item) => {
				return chosenBaitToConsume ?? item;
			});
		}
		internal static void On_AI_061_FishingBobber_GiveItemToPlayer(ILContext il) {
			var c = new ILCursor(il);

			//IL_0176: callvirt instance class Terraria.Item Terraria.Player::GetItem(int32, class Terraria.Item, valuetype Terraria.GetItemSettings)
			//IL_017b: stloc.1

			//note to self: If trying to replace a function call, you need to use c.Remove() which removes the next instruction,
			//	not pop as pop is an instruction and will attempt to be an argument (I think)
			if (!c.TryGotoNext(MoveType.Before,
				i => i.MatchCallvirt(typeof(Player).GetMethod("GetItem", new Type[] { typeof(int), typeof(Item), typeof(GetItemSettings) })),
				i => i.MatchStloc(1)
			)) { throw new Exception("Failed to find instructions On_AI_061_FishingBobber_GiveItemToPlayer 1/1"); }

			//c.Emit(OpCodes.Ldarg_1);
			//$"c.Index: {c.Index} Instruction: {c.Next}".LogSimple();
			//$"c.Index: {c.Index} Instruction: {c.Prev}".LogSimple();
			//c.EmitPop();
			c.Remove();
			//$"c.Index: {c.Index} Instruction: {c.Next}".LogSimple();
			//$"c.Index: {c.Index} Instruction: {c.Prev}".LogSimple();
			c.EmitDelegate((Player player, int owner, Item item, GetItemSettings settings) => {
				if (owner == Main.myPlayer) {
					Item clone = item.Clone();
					if (StorageManager.TryVacuumItem(ref clone, player))
						return clone;
				}

				return player.GetItem(owner, item, settings);
			});
		}
		internal static void OnGetAnglerReward(ILContext il) {
			var c = new ILCursor(il);

			//IL_00ac: call instance class Terraria.Item Terraria.Player::GetItem(int32, class Terraria.Item, valuetype Terraria.GetItemSettings)
			//IL_00b1: stloc.s 7

			if (!c.TryGotoNext(MoveType.Before,
				i => i.MatchCall(typeof(Player).GetMethod("GetItem", new Type[] { typeof(int), typeof(Item), typeof(GetItemSettings) })),
				i => i.MatchStloc(7)
			)) { throw new Exception("Failed to find instructions OnGetAnglerReward 1/1"); }

			c.Remove();
			c.EmitDelegate((Player player, int owner, Item item, GetItemSettings settings) => {
				if (owner == Main.myPlayer) {
					Item clone = item.Clone();
					if (StorageManager.TryVacuumItem(ref clone, player)) {
						PopupText.NewText(PopupTextContext.RegularItemPickup, item, clone.stack - item.stack);
						return clone;
					}
				}

				return player.GetItem(owner, item, settings);
			});
		}
		internal static void OnGUIChatDrawInner(ILContext il) {
			var c = new ILCursor(il);

			//IL_16d3: ldloc.s 48
			//IL_16d5: ldc.i4.m1

			if (!c.TryGotoNext(MoveType.Before,
				i => i.MatchLdloc(52),
				i => i.MatchLdcI4(-1)
			)) { throw new Exception("Failed to find instructions OnGUIChatDrawInner 1/1"); }

			c.Emit(OpCodes.Ldloc, 52);
			c.Emit(OpCodes.Ldloca, 51);

			c.EmitDelegate((int foundQuestFishIndex, ref bool questCompleted) => {
				if (foundQuestFishIndex >= 0)
					return;

				int questFish = Main.anglerQuestItemNetIDs[Main.anglerQuest];
				Item[] inv = StorageManager.GetItems(Instance.BagStorageID);
				Item foundItem = null;
				for (int i = 0; i < inv.Length; i++) {
					if (inv[i].netID == questFish) {
						foundItem = inv[i];
						break;
					}
				}

				if (foundItem == null)
					return;

				foundItem.stack--;
				if (foundItem.stack <= 0)
					foundItem.TurnToAir();

				questCompleted = true;

				SoundEngine.PlaySound(SoundID.Chat);

				Player player = Main.LocalPlayer;
				player.anglerQuestsFinished++;
				player.GetAnglerReward(Main.npc[player.talkNPC], Main.anglerQuestItemNetIDs[Main.anglerQuest]);
			});
		}

		#region AllowedItems

		public override bool? DevCheck(ItemSetInfo info, SortedSet<ItemGroup> itemGroups, SortedSet<string> endWords, SortedSet<string> searchWords) {
			if (ItemID.Sets.CanFishInLava[info.Type]
				|| ItemID.Sets.IsLavaBait[info.Type]
				|| ItemID.Sets.IsFishingCrate[info.Type]
				|| ItemID.Sets.IsFishingCrateHardmode[info.Type]) {
				return true;
			}

			return null;
		}
		public override SortedSet<int> DevWhiteList() {
			SortedSet<int> devWhiteList = new() {
				ItemID.FishBowl,
				ItemID.Fish,
				ItemID.OldShoe,
				ItemID.FishingSeaweed,
				ItemID.TinCan,
				ItemID.AnglerHat,
				ItemID.AnglerVest,
				ItemID.AnglerPants,
				ItemID.HighTestFishingLine,
				ItemID.AnglerEarring,
				ItemID.TackleBox,
				ItemID.FuzzyCarrot,
				ItemID.ScalyTruffle,
				ItemID.LifePreserver,
				ItemID.ShipsWheel,
				ItemID.CompassRose,
				ItemID.WallAnchor,
				ItemID.GoldfishTrophy,
				ItemID.BunnyfishTrophy,
				ItemID.SwordfishTrophy,
				ItemID.SharkteethTrophy,
				ItemID.ShipInABottle,
				ItemID.TreasureMap,
				ItemID.SeaweedPlanter,
				ItemID.PillaginMePixels,
				ItemID.FishCostumeMask,
				ItemID.FishCostumeShirt,
				ItemID.FishCostumeFinskirt,
				ItemID.GingerBeard,
				ItemID.FishFinder,
				ItemID.WeatherRadio,
				ItemID.LockBox,
				ItemID.ObsidianLockbox,
				ItemID.Sextant,
				ItemID.FishermansGuide,
				ItemID.CellPhone,
				ItemID.PDA,
				ItemID.FishFinder,
				ItemID.AnglerTackleBag,
				ItemID.CanOfWorms,
				ItemID.Oyster,
				ItemID.ShuckedOyster,
				ItemID.WhitePearl,
				ItemID.BlackPearl,
				ItemID.PinkPearl,
				ItemID.SharkBait,
				ItemID.ChumBucket,
				ItemID.LavaFishingHook,
				ItemID.LavaproofTackleBag,
				ItemID.ArmoredCavefish,
				ItemID.AtlanticCod,
				ItemID.Bass,
				ItemID.BlueJellyfish,
				ItemID.ChaosFish,
				ItemID.CrimsonTigerfish,
				ItemID.Damselfish,
				ItemID.DoubleCod,
				ItemID.Ebonkoi,
				ItemID.FlarefinKoi,
				ItemID.Flounder,
				ItemID.FrostMinnow,
				ItemID.GoldenCarp,
				ItemID.GreenJellyfish,
				ItemID.Hemopiranha,
				ItemID.Honeyfin,
				ItemID.NeonTetra,
				ItemID.Obsidifish,
				ItemID.PinkJellyfish,
				ItemID.PrincessFish,
				ItemID.Prismite,
				ItemID.RedSnapper,
				ItemID.RockLobster,
				ItemID.Salmon,
				ItemID.Shrimp,
				ItemID.SpecularFish,
				ItemID.Trout,
				ItemID.Tuna,
				ItemID.VariegatedLardfish,
				ItemID.ZephyrFish,
				ItemID.DemonConch,
				ItemID.MagicConch,
				ItemID.Sextant,
				ItemID.CombatBook
			};

			foreach (int bugType in RecipeGroup.recipeGroups[RecipeGroupID.Bugs].ValidItems) {
				devWhiteList.Add(bugType);
			}

			foreach (int itemType in RecipeGroup.recipeGroups[RecipeGroupID.Scorpions].ValidItems) {
				devWhiteList.Add(itemType);
			}

			foreach (int itemType in RecipeGroup.recipeGroups[RecipeGroupID.Butterflies].ValidItems) {
				devWhiteList.Add(itemType);
			}

			foreach (int itemType in RecipeGroup.recipeGroups[RecipeGroupID.Fireflies].ValidItems) {
				devWhiteList.Add(itemType);
			}

			foreach (int itemType in RecipeGroup.recipeGroups[RecipeGroupID.Snails].ValidItems) {
				devWhiteList.Add(itemType);
			}

			foreach (int itemType in RecipeGroup.recipeGroups[RecipeGroupID.Dragonflies].ValidItems) {
				devWhiteList.Add(itemType);
			}

			devWhiteList.UnionWith(Main.anglerQuestItemNetIDs);

			return devWhiteList;
		}
		public override SortedSet<int> DevBlackList() {
			SortedSet<int> devBlackList = new() {
				ItemID.FishingPotion,
				ItemID.Rockfish
			};

			return devBlackList;
		}
		public override SortedSet<ItemGroup> ItemGroups() {
			SortedSet<ItemGroup> itemGroups = new() {
				ItemGroup.Fish,
				ItemGroup.FishingBait,
				ItemGroup.FishingRods,
				ItemGroup.FishingQuestFish
			};

			return itemGroups;
		}
		public override SortedSet<string> SearchWords() {
			SortedSet<string> searchWords = new() {
				"fishingbobber"
			};

			return searchWords;
		}

		#endregion

		#region AndroModItem attributes that you don't need.

		public override string SummaryOfFunction => "Fishing Bait";
		public override string LocalizationTooltip =>
			$"Automatically stores fishing related items such as fish, bait and angler rewards.\n" +
			$"When in your inventory, the contents of the belt are available for crafting.\n" +
			$"Right click to open the belt.\n" +
			$"Bait in the belt is used if the fishing belt is in the first bait item found.\n" +
			$"If any bait in the belt that can be used is favorited, only favorited baits will be used.\n" +
			$"Information accessories that are favorited will show their info as if they are in your inventory.";
		public override string Artist => "anodomani";
		public override string Designer => "andro951";

		#endregion
	}
}
