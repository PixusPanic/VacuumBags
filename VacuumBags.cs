using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using VacuumBags.Common.Configs;
using VacuumBags.Items;

namespace VacuumBags
{
	public class VacuumBags : Mod {
		public static BagsServerConfig serverConfig = ModContent.GetInstance<BagsServerConfig>();
		public static BagsClientConfig clientConfig = ModContent.GetInstance<BagsClientConfig>();
		public override void Load() {
			BagBlue.RegisterWithAndroLib(this);
			BagBrown.RegisterWithAndroLib(this);
			BagGreen.RegisterWithAndroLib(this);
			BagOrange.RegisterWithAndroLib(this);
			BagPurple.RegisterWithAndroLib(this);
			BagRed.RegisterWithAndroLib(this);
			BagYellow.RegisterWithAndroLib(this);
			BuildersBox.RegisterWithAndroLib(this);
			WallEr.RegisterWithAndroLib(this);
			PaintBucket.RegisterWithAndroLib(this);
			PotionFlask.RegisterWithAndroLib(this);
			HerbSatchel.RegisterWithAndroLib(this);
			AmmoBag.RegisterWithAndroLib(this);

			On_Player.ChooseAmmo += AmmoBag.OnChooseAmmo;
			On_Player.FindPaintOrCoating += PaintBucket.OnFindPaintOrCoating;
			On_ItemSlot.RightClick_ItemArray_int_int += OnRightClick_ItemArray_int_int;
			
			IL_ItemSlot.Draw_SpriteBatch_ItemArray_int_int_Vector2_Color += AmmoBag.OnDrawItemSlot;
			IL_Player.ItemCheck_CheckCanUse += PaintBucket.OnItemCheck_CheckCanUse;
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