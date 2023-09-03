using androLib;
using androLib.Common.Utility;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using VacuumBags.Items;

namespace VacuumBags
{
	public class BagPlayer : ModPlayer {
		private static bool baglaceItemChecked = false;
		public static Item BagPlaceItem {
			get {
				if (!baglaceItemChecked)
					bagPlaceItem = GetBagPlaceItem();

				return bagPlaceItem;
			}
		}
		private static Item bagPlaceItem = null;

		private static List<KeyValuePair<Func<int>, Func<Item>>> choseFromBagFunctions = new() {
			new(ModContent.ItemType<BuildersBox>, BuildersBox.ChooseItemFromBox),
			new(ModContent.ItemType<WallEr>, WallEr.ChooseItemFromWallEr)
		};

		private static Item GetBagPlaceItem() {
			baglaceItemChecked = true;
			int bagType = Main.LocalPlayer.HeldItem.type;
			foreach (KeyValuePair<Func<int>, Func<Item>> choseFromBagPair in choseFromBagFunctions) {
				if (choseFromBagPair.Key() == bagType)
					return choseFromBagPair.Value();
			}

			return null;
		}

		public override void ResetEffects() {
			bagPlaceItem = null;
			baglaceItemChecked = false;
		}

		internal static void OnItemCheck_Inner(On_Player.orig_ItemCheck_Inner orig, Player self) {
			bool swap = BagPlaceItem != null;
			if (swap)
				Utils.Swap(ref self.inventory[self.selectedItem], ref bagPlaceItem);

			orig(self);

			if (swap)
				Utils.Swap(ref self.inventory[self.selectedItem], ref bagPlaceItem);
		}

		internal static void On_Main_DrawInterface_40_InteractItemIcon(On_Main.orig_DrawInterface_40_InteractItemIcon orig, Main self) {
			bool swap = BagPlaceItem != null;
			if (swap)
				Utils.Swap(ref Main.LocalPlayer.inventory[Main.LocalPlayer.selectedItem], ref bagPlaceItem);

			orig(self);

			if (swap)
				Utils.Swap(ref Main.LocalPlayer.inventory[Main.LocalPlayer.selectedItem], ref bagPlaceItem);
		}


		//internal static void OnSetItemAnimation(On_Player.orig_SetItemAnimation orig, Player self, int frames) {
		//	orig(self, frames);
		//	Item item = BagPlaceItem;
		//	if (item != null)
		//		self.itemAnimation = item.useAnimation;
		//}

		/*
		internal static void IL_ItemCheck_Inner(ILContext il) {
	//		 	IL_0029: ldarg.0
	//IL_002a: ldfld class Terraria.Item[] Terraria.Player::inventory
	//IL_002f: ldarg.0
	//IL_0030: ldfld int32 Terraria.Player::selectedItem
	//IL_0035: ldelem.ref
	//IL_0036: stloc.1
			var c = new ILCursor(il);
			if (!c.TryGotoNext(MoveType.After,
				i => i.MatchLdarg(0),
				i => i.MatchLdfld<Player>("inventory"),
				i => i.MatchLdarg(0),
				i => i.MatchLdfld<Player>("selectedItem"),
				i => i.MatchLdelemRef(),
				i => i.MatchStloc(1)
			)) { throw new Exception("Failed to find instructions IL_ItemCheck_Inner 1/1"); }

			c.Emit(OpCodes.Ldloca, 1);
			c.EmitDelegate((ref Item heldItem) => {
				if (heldItem.NullOrAir())
					return;

				Item item = BagPlaceItem;
				if (item != null)
					heldItem = item;
			});
		}
		internal static void On_Player_ApplyItemAnimation_Item(On_Player.orig_ApplyItemAnimation_Item orig, Player self, Item sItem) {
			Item item = BagPlaceItem ?? sItem;
			orig(self, item);
			bool itemAnimation = Main.LocalPlayer.itemAnimation > 0;
		}
		internal static void On_Player_ApplyItemAnimation_Item_float_Nullable1(On_Player.orig_ApplyItemAnimation_Item_float_Nullable1 orig, Player self, Item sItem, float multiplier, int? itemReuseDelay) {
			Item item = BagPlaceItem ?? sItem;
			orig(self, item, multiplier, itemReuseDelay);
			bool itemAnimation = Main.LocalPlayer.itemAnimation > 0;
		}
		public static void On_Main_DrawInterface_40_InteractItemIcon(On_Main.orig_DrawInterface_40_InteractItemIcon orig, Main self) {
			Item item = BagPlaceItem;
			if (item != null) {
				if (!Main.HoveringOverAnNPC && !Main.LocalPlayer.mouseInterface) {
					bool mouseOutOfTilePlaceRange = item.MouseOutOfTilePlaceRange();
					if (!mouseOutOfTilePlaceRange) {
						Main.LocalPlayer.cursorItemIconID = item.type;
						Main.LocalPlayer.cursorItemIconEnabled = true;
					}
				}
			}

			orig(self);
		}
		internal static void OnPlaceThing_Tiles(ILContext il) {
	//			.locals init (
	//	[0] class Terraria.Item item,
	//	[1] int32 tileToCreate,
	//	[2] bool flag,
	//	[3] bool canUse,
	//	[4] valuetype Terraria.Tile tile,
	//	[5] bool canPlace,
	//	[6] bool newObjectType,
	//	[7] valuetype [System.Runtime]System.Nullable`1<bool> overrideCanPlace,
	//	[8] valuetype [System.Runtime]System.Nullable`1<int32> forcedRandom,
	//	[9] valuetype Terraria.TileObject objectData,
	//	[10] int32 previewPlaceStyle
	//)

	//// Item item = this.inventory[this.selectedItem];
	//IL_0000: ldarg.0
	//IL_0001: ldfld class Terraria.Item[] Terraria.Player::inventory
	//IL_0006: ldarg.0
	//IL_0007: ldfld int32 Terraria.Player::selectedItem
	//IL_000c: ldelem.ref
	//IL_000d: stloc.0
	//// int tileToCreate = item.createTile;
	//IL_000e: ldloc.0

			var c = new ILCursor(il);
			if (!c.TryGotoNext(MoveType.After,
				i => i.MatchLdarg(0),
				i => i.MatchLdfld<Player>("inventory"),
				i => i.MatchLdarg(0),
				i => i.MatchLdfld<Player>("selectedItem"),
				i => i.MatchLdelemRef(),
				i => i.MatchStloc(0)
			)) { throw new Exception("Failed to find instructions OnPlaceThing_Tiles 1/1"); }

			c.Emit(OpCodes.Ldloca, 0);
			c.EmitDelegate((ref Item heldItem) => {
				if (heldItem.NullOrAir())
					return;

				Item item = BagPlaceItem;
				if (item != null)
					heldItem = item;
			});

	//			IL_018c: ldloc.3
	//IL_018d: brfalse IL_0337

	//IL_0192: ldloca.s 4
	//IL_0194: call instance bool Terraria.Tile::active()
	//IL_0199: brtrue.s IL_01a1

			if (!c.TryGotoNext(MoveType.Before,
				i => i.MatchLdloc(3),
				i => i.MatchBrfalse(out _),
				i => i.MatchLdloca(4),
				i => i.MatchCall<Tile>("active"),
				i => i.MatchBrtrue(out _)
			)) { throw new Exception("Failed to find instructions OnPlaceThing_Tiles (Temp 1)"); }
			c.Index++;
			c.Emit(OpCodes.Ldloc, 3);
			c.Emit(OpCodes.Ldloc, 2);
			c.EmitDelegate((bool canUse, bool flag) => {
				Tile tile = Main.tile[Player.tileTargetX, Player.tileTargetY];
				bool tileNotActive = !tile.HasTile && !flag;
				bool final = canUse && tileNotActive;
				bool itemTimeIsZero = Main.LocalPlayer.ItemTimeIsZero;
				bool itemAnimation = Main.LocalPlayer.itemAnimation > 0;
				bool controlUseItem = Main.LocalPlayer.controlUseItem;
				Main.NewText("Hit 1");
			});

			if (!c.TryGotoNext(MoveType.After,
				i => i.MatchStloc(5)
			)) { throw new Exception("Failed to find instructions OnPlaceThing_Tiles (Temp 2)"); }
			c.EmitDelegate(() => {
				Main.NewText("Hit 2");
			});
		}
		internal static void IL_Player_PlaceThing_Tiles_CheckRopeUsability(ILContext il) {
	//			IL_0000: ldsfld bool[] Terraria.Main::tileRope
	//IL_0005: ldarg.0
	//IL_0006: ldfld class Terraria.Item[] Terraria.Player::inventory
	//IL_000b: ldarg.0
	//IL_000c: ldfld int32 Terraria.Player::selectedItem
	//IL_0011: ldelem.ref
	//IL_0012: ldfld int32 Terraria.Item::createTile
			var c = new ILCursor(il);
			if (!c.TryGotoNext(MoveType.After,
				i => i.MatchLdarg0(),
				i => i.MatchLdfld<Player>("inventory"),
				i => i.MatchLdarg0(),
				i => i.MatchLdfld<Player>("selectedItem"),
				i => i.MatchLdelemRef(),
				i => i.MatchLdfld<Item>("createTile")
			)) { throw new Exception("Failed to find instructions IL_Player_PlaceThing_Tiles_CheckRopeUsability 1/1"); }

			//c.Emit(OpCodes.Pop);
			c.EmitDelegate((int createTileType) => {
				Item item = BagPlaceItem;
				if (item != null)
					createTileType = item.createTile;

				return createTileType;
			});
		}
		internal static void IL_Player_PlaceThing_ValidTileForReplacement(ILContext il) {
	//			.locals init (
	//	[0] int32 createTile,
	//	[1] int32 num,
	//	[2] valuetype Terraria.Tile tile,
	//	[3] bool flag,
	//	[4] valuetype Terraria.Tile,
	//	[5] int32 dropItem,
	//	[6] int32,
	//	[7] int32,
	//	[8] int32
	//)

	//// int type = this.HeldItem.createTile;
	//IL_0000: ldarg.0
	//IL_0001: call instance class Terraria.Item Terraria.Player::get_HeldItem()
	//IL_0006: ldfld int32 Terraria.Item::createTile
	//IL_000b: stloc.0
	//// int style = this.HeldItem.placeStyle;
	//IL_000c: ldarg.0
	//IL_000d: call instance class Terraria.Item Terraria.Player::get_HeldItem()
	//IL_0012: ldfld int32 Terraria.Item::placeStyle
	//IL_0017: stloc.1
	//// if (this.UsingBiomeTorches && type == 4)
	//IL_0018: ldarg.0
	//IL_0019: call instance bool Terraria.Player::get_UsingBiomeTorches()
	//IL_001e: brfalse.s IL_002f

	//IL_0020: ldloc.0
	//IL_0021: ldc.i4.4
	//IL_0022: bne.un.s IL_002f

			var c = new ILCursor(il);
			if (!c.TryGotoNext(MoveType.After,
				i => i.MatchLdarg(0),
				i => i.MatchCall<Player>("get_HeldItem"),
				i => i.MatchLdfld<Item>("createTile")
			)) { throw new Exception("Failed to find instructions IL_Player_PlaceThing_Tiles_CheckRopeUsability 1/1"); }

			//c.Emit(OpCodes.Pop);
			c.EmitDelegate((int createTileType) => {
				Item item = BagPlaceItem;
				if (item != null)
					createTileType = item.createTile;

				return createTileType;
			});
		}
		internal static void IL_Player_PlaceThing_Tiles_CheckLavaBlocking(ILContext il) {
	//			IL_0040: ldsfld bool[] Terraria.Main::tileSolid
	//IL_0045: ldarg.0
	//IL_0046: ldfld class Terraria.Item[] Terraria.Player::inventory
	//IL_004b: ldarg.0
	//IL_004c: ldfld int32 Terraria.Player::selectedItem
	//IL_0051: ldelem.ref
	//IL_0052: ldfld int32 Terraria.Item::createTile
			var c = new ILCursor(il);
			if (!c.TryGotoNext(MoveType.After,
				i => i.MatchLdarg0(),
				i => i.MatchLdfld<Player>("inventory"),
				i => i.MatchLdarg0(),
				i => i.MatchLdfld<Player>("selectedItem"),
				i => i.MatchLdelemRef(),
				i => i.MatchLdfld<Item>("createTile")
			)) { throw new Exception("Failed to find instructions IL_Player_PlaceThing_Tiles_CheckLavaBlocking 1/3"); }

			//c.Emit(OpCodes.Pop);
			c.EmitDelegate((int createTileType) => {
				Item item = BagPlaceItem;
				if (item != null)
					createTileType = item.createTile;

				return createTileType;
			});

	//IL_005e: ldarg.0
	//IL_005f: ldfld class Terraria.Item[] Terraria.Player::inventory
	//IL_0064: ldarg.0
	//IL_0065: ldfld int32 Terraria.Player::selectedItem
	//IL_006a: ldelem.ref
	//IL_006b: ldfld int32 Terraria.Item::createTile
	//IL_0070: ldarg.0
	//IL_0071: ldfld class Terraria.Item[] Terraria.Player::inventory
	//IL_0076: ldarg.0
	//IL_0077: ldfld int32 Terraria.Player::selectedItem
	//IL_007c: ldelem.ref
	//IL_007d: ldfld int32 Terraria.Item::placeStyle

			if (!c.TryGotoNext(MoveType.After,
				i => i.MatchLdarg0(),
				i => i.MatchLdfld<Player>("inventory"),
				i => i.MatchLdarg0(),
				i => i.MatchLdfld<Player>("selectedItem"),
				i => i.MatchLdelemRef(),
				i => i.MatchLdfld<Item>("createTile")
			)) { throw new Exception("Failed to find instructions IL_Player_PlaceThing_Tiles_CheckLavaBlocking 2/3"); }

			//c.Emit(OpCodes.Pop);
			c.EmitDelegate((int createTileType) => {
				Item item = BagPlaceItem;
				if (item != null)
					createTileType = item.createTile;

				return createTileType;
			});

			if (!c.TryGotoNext(MoveType.After,
				i => i.MatchLdarg0(),
				i => i.MatchLdfld<Player>("inventory"),
				i => i.MatchLdarg0(),
				i => i.MatchLdfld<Player>("selectedItem"),
				i => i.MatchLdelemRef(),
				i => i.MatchLdfld<Item>("placeStyle")
			)) { throw new Exception("Failed to find instructions IL_Player_PlaceThing_Tiles_CheckLavaBlocking 2/3"); }

			//c.Emit(OpCodes.Pop);
			c.EmitDelegate((int placeStyle) => {
				Item item = BagPlaceItem;
				if (item != null)
					placeStyle = item.placeStyle;

				return placeStyle;
			});
		}
		*/
	}
}
