using androLib;
using androLib.Localization;
using System;
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
			On_ItemSlot.RightClick_ItemArray_int_int += OnRightClick_ItemArray_int_int;
			
			IL_ItemSlot.Draw_SpriteBatch_ItemArray_int_int_Vector2_Color += AmmoBag.OnDrawItemSlot;
			IL_Player.ItemCheck_CheckCanUse += PaintBucket.OnItemCheck_CheckCanUse;

			VacuumBagsLocalizationData.RegisterSDataPackage();

			BuildersBox.RegisterWithGadgetGalore();
			WallEr.RegisterWithGadgetGalore();
			PaintBucket.RegisterWithGadgetGalore();
		}

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