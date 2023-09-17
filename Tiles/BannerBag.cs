using androLib;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace VacuumBags.Tiles
{
	public class BannerBag : VacuumBagTile
	{
		public override Color MapColor => Items.BannerBag.PanelColor;
		public override void KillMultiTile(int i, int j, int frameX, int frameY) {
			if (Main.netMode == NetmodeID.Server)
				return;

			if (!StorageManager.HasRequiredItemToUseStorageFromBagType(Main.LocalPlayer, ModContent.ItemType<Items.BannerBag>(), out _))
				Items.BannerBag.CloseBag();
		}
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.CoordinateHeights = new int[3] { 16, 16, 16 };
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.SolidBottom, TileObjectData.newTile.Width, 0);
			TileObjectData.addTile(Type);
		}
	}

	public class BannerBagGlobal : GlobalTile {
		private int BannerBagType {
			get {
				if (bannerBagType == -1)
					bannerBagType = ModContent.TileType<BannerBag>();

				return bannerBagType;
			}
		}
		int bannerBagType = -1;
		public override void NearbyEffects(int i, int j, int type, bool closer) {
			if (type == BannerBagType)
				Items.BannerBag.ApplyFirstXBanners(Main.LocalPlayer, VacuumBags.serverConfig.BannerBagNumberOfBannersWhenPlaced, true);
		}
	}
}