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
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using VacuumBags.Items;

namespace VacuumBags.Tiles
{
	public class BannerBag : VacuumBagTile {
		protected override BagModItem ModBag => Items.BannerBag.Instance;
		private static int platformYOffset = -8;
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.CoordinateHeights = new int[3] { 16, 16, 16 };
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.EmptyTile | AnchorType.SolidBottom | AnchorType.PlanterBox, TileObjectData.newTile.Width, 0);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.Platform, TileObjectData.newTile.Width, 0);
			TileObjectData.newAlternate.DrawYOffset = platformYOffset;
			TileObjectData.addAlternate(0);
			TileObjectData.addTile(Type);
		}
		public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) {
			Tile tile = Main.tile[i, j];
			TileObjectData data = TileObjectData.GetTileData(tile);
			int topLeftX = i - tile.TileFrameX / 18 % data.Width;
			int topLeftY = j - tile.TileFrameY / 18 % data.Height;
			if (WorldGen.IsBelowANonHammeredPlatform(topLeftX, topLeftY))
				offsetY += platformYOffset;
		}
	}
}