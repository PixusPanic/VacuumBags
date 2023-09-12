using androLib;
using androLib.Localization;
using System;
using System.Collections;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using VacuumBags.Common.Configs;
using VacuumBags.Items;
using VacuumBags.Localization;

namespace VacuumBags
{

	public class VacuumBags : Mod {
		public static BagsServerConfig serverConfig = ModContent.GetInstance<BagsServerConfig>();
		public static BagsClientConfig clientConfig = ModContent.GetInstance<BagsClientConfig>();
		public static Mod GadgetGalore;
		public static bool gadgetGaloreEnabled = ModLoader.TryGetMod("GadgetGalore", out GadgetGalore);
		public override void Load() {
			BagBlack.RegisterWithAndroLib(this);
			BagBlue.RegisterWithAndroLib(this);
			BagBrown.RegisterWithAndroLib(this);
			BagGray.RegisterWithAndroLib(this);
			BagGreen.RegisterWithAndroLib(this);
			BagOrange.RegisterWithAndroLib(this);
			BagPink.RegisterWithAndroLib(this);
			BagPurple.RegisterWithAndroLib(this);
			BagRed.RegisterWithAndroLib(this);
			BagWhite.RegisterWithAndroLib(this);
			BagYellow.RegisterWithAndroLib(this);
			BuildersBox.RegisterWithAndroLib(this);
			WallEr.RegisterWithAndroLib(this);
			PaintBucket.RegisterWithAndroLib(this);
			PotionFlask.RegisterWithAndroLib(this);
			HerbSatchel.RegisterWithAndroLib(this);
			AmmoBag.RegisterWithAndroLib(this);
			BossBag.RegisterWithAndroLib(this);

			CalamitousCauldron.RegisterWithAndroLib(this);
			LokisTesseract.RegisterWithAndroLib(this);
			EssenceOfGathering.RegisterWithAndroLib(this);
			FargosMementos.RegisterWithAndroLib(this);

			PackBlack.RegisterWithAndroLibItemTypeOnly();
			PackBlue.RegisterWithAndroLibItemTypeOnly();
			PackBrown.RegisterWithAndroLibItemTypeOnly();
			PackGray.RegisterWithAndroLibItemTypeOnly();
			PackGreen.RegisterWithAndroLibItemTypeOnly();
			PackOrange.RegisterWithAndroLibItemTypeOnly();
			PackPink.RegisterWithAndroLibItemTypeOnly();
			PackPurple.RegisterWithAndroLibItemTypeOnly();
			PackRed.RegisterWithAndroLibItemTypeOnly();
			PackWhite.RegisterWithAndroLibItemTypeOnly();
			PackYellow.RegisterWithAndroLibItemTypeOnly();

			On_Player.ChooseAmmo += AmmoBag.OnChooseAmmo;
			On_Player.FindPaintOrCoating += PaintBucket.OnFindPaintOrCoating;
			On_Player.QuickBuff_PickBestFoodItem += PotionFlask.OnQuickBuff_PickBestFoodItem;
			On_Player.QuickBuff += PotionFlask.OnQuickBuff;
			On_Player.QuickHeal_GetItemToUse += PotionFlask.OnQuickHeal_GetItemToUse;
			On_Player.QuickMana_GetItemToUse += PotionFlask.OnQuickMana_GetItemToUse;
			On_ItemSlot.RightClick_ItemArray_int_int += OnRightClick_ItemArray_int_int;

			On_Player.ItemCheck_Inner += BagPlayer.OnItemCheck_Inner;
			On_Main.DrawInterface_40_InteractItemIcon += BagPlayer.On_Main_DrawInterface_40_InteractItemIcon;

			//On_Player.ItemCheck_Inner += BuildersBox.OnItemCheck_Inner;
			//On_Player.SetItemAnimation += BagPlayer.OnSetItemAnimation;
			//IL_Player.ItemCheck_Inner += BagPlayer.IL_ItemCheck_Inner;
			//On_Player.ApplyItemAnimation_Item += BagPlayer.On_Player_ApplyItemAnimation_Item;
			//On_Player.ApplyItemAnimation_Item_float_Nullable1 += BagPlayer.On_Player_ApplyItemAnimation_Item_float_Nullable1;
			//On_Main.DrawInterface_40_InteractItemIcon += BagPlayer.On_Main_DrawInterface_40_InteractItemIcon;
			//IL_Player.PlaceThing_Tiles += BagPlayer.OnPlaceThing_Tiles;
			//IL_Player.PlaceThing_Tiles_CheckLavaBlocking += BagPlayer.IL_Player_PlaceThing_Tiles_CheckLavaBlocking;
			//IL_Player.PlaceThing_Tiles_CheckRopeUsability += BagPlayer.IL_Player_PlaceThing_Tiles_CheckRopeUsability;
			//IL_Player.PlaceThing_ValidTileForReplacement += BagPlayer.IL_Player_PlaceThing_ValidTileForReplacement;

			//On_Player.PlaceThing_Tiles_CheckLavaBlocking += On_Player_PlaceThing_Tiles_CheckLavaBlocking;
			//On_Player.PlaceThing_Tiles_CheckRopeUsability += On_Player_PlaceThing_Tiles_CheckRopeUsability;
			//On_Player.PlaceThing_Tiles_CheckFlexibleWand += On_Player_PlaceThing_Tiles_CheckFlexibleWand;
			//On_Player.PlaceThing_TryReplacingTiles += On_Player_PlaceThing_TryReplacingTiles;
			//On_Player.FigureOutWhatToPlace += On_Player_FigureOutWhatToPlace;
			//On_Player.PlaceThing_Tiles += On_Player_PlaceThing_Tiles;

			IL_ItemSlot.Draw_SpriteBatch_ItemArray_int_int_Vector2_Color += AmmoBag.OnDrawItemSlot;
			IL_Player.ItemCheck_CheckCanUse += PaintBucket.OnItemCheck_CheckCanUse;

			VacuumBagsLocalizationData.RegisterSDataPackage();

			BuildersBox.RegisterWithGadgetGalore();
			WallEr.RegisterWithGadgetGalore();
			PaintBucket.RegisterWithGadgetGalore();
		}

		//private void On_Player_PlaceThing_Tiles(On_Player.orig_PlaceThing_Tiles orig, Player self) {
		//	orig(self);
		//}

		//private void On_Player_FigureOutWhatToPlace(On_Player.orig_FigureOutWhatToPlace orig, Player self, Tile targetTile, Item sItem, out int tileToCreate, out int previewPlaceStyle, out bool? overrideCanPlace, out int? forcedRandom) {
		//	orig(self, targetTile, sItem, out tileToCreate, out previewPlaceStyle, out overrideCanPlace, out forcedRandom);
		//}

		//private bool On_Player_PlaceThing_Tiles_CheckLavaBlocking(On_Player.orig_PlaceThing_Tiles_CheckLavaBlocking orig, Player self) {
		//	bool result = orig(self);
		//	return result;
		//}

		//private bool On_Player_PlaceThing_TryReplacingTiles(On_Player.orig_PlaceThing_TryReplacingTiles orig, Player self, bool canUse) {
		//	bool result = orig(self, canUse);
		//	return result;
		//}

		//private bool On_Player_PlaceThing_Tiles_CheckFlexibleWand(On_Player.orig_PlaceThing_Tiles_CheckFlexibleWand orig, Player self, bool canUse) {
		//	bool result = orig(self, canUse);
		//	return result;
		//}

		//private bool On_Player_PlaceThing_Tiles_CheckRopeUsability(On_Player.orig_PlaceThing_Tiles_CheckRopeUsability orig, Player self, bool canUse) {
		//	bool result = orig(self, canUse);
		//	return result;
		//}

		private void OnRightClick_ItemArray_int_int(On_ItemSlot.orig_RightClick_ItemArray_int_int orig, Item[] inv, int context, int slot) {
			int clickedItemType = inv[slot].type;
			if (clickedItemType == ModContent.ItemType<AmmoBag>() || 
				clickedItemType == ModContent.ItemType<PaintBucket>()
			) {
				orig(inv, 0, slot);
			}
			else {
				orig(inv, context, slot);
			}
		}
	}
}