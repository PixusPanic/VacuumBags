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
	public class PortableStation : VacuumBagTile
	{
		public override Color MapColor => Items.PortableStation.PanelColor;
		public override void KillMultiTile(int i, int j, int frameX, int frameY) {
			if (Main.netMode == NetmodeID.Server)
				return;

			if (!StorageManager.HasRequiredItemToUseStorageFromBagType(Main.LocalPlayer, ModContent.ItemType<Items.PortableStation>(), out _))
				Items.PortableStation.CloseBag();
		}
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();
			Main.tileTable[Type] = true;
			Main.tileSolidTop[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
			TileObjectData.newTile.CoordinateHeights = new[] { 34 };
			TileObjectData.newTile.DrawYOffset = -16;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.AnchorInvalidTiles = new int[] {
				TileID.MagicalIceBlock,
				TileID.Boulder,
				TileID.BouncyBoulder,
				TileID.LifeCrystalBoulder,
				TileID.RollingCactus
			};
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide | AnchorType.Table, TileObjectData.newTile.Width, 0);
			AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
			AdjTiles = AdjTiles.Append(TileID.WorkBenches).ToArray();
			TileObjectData.addTile(Type);
		}
	}

	public class PortableStationGlobal : GlobalTile
	{
		private int PortableStationType {
			get {
				if (portableStationType == -1)
					portableStationType = ModContent.TileType<PortableStation>();

				return portableStationType;
			}
		}
		int portableStationType = -1;
		public override int[] AdjTiles(int type) {
			if (Main.netMode != NetmodeID.Server) {
				if (type == PortableStationType)
					Items.PortableStation.ApplyFirstXStationTiles(Main.LocalPlayer, VacuumBags.serverConfig.PortableStationNumberOfStationsWhenPlaced, true);
			}

			return new int[0];
		}
	}
}