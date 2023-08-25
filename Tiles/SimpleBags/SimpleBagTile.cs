using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Microsoft.Xna.Framework;

namespace VacuumBags.Tiles
{
	public abstract class SimpleBagTile : ModTile {
		public abstract Color MapColor { get; }
		public override void SetStaticDefaults() {
			Main.tileContainer[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			TileID.Sets.HasOutlines[Type] = true;
			TileID.Sets.BasicChest[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;
			TileID.Sets.IsAContainer[Type] = true;

			AdjTiles = new int[] { TileID.Containers };

			Color mapColor = MapColor;
			mapColor.A = byte.MaxValue;
			AddMapEntry(mapColor, CreateMapEntryName());

			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
			TileObjectData.newTile.CoordinateHeights = new[] { 18 };
			TileObjectData.newTile.AnchorInvalidTiles = new int[] {
				TileID.MagicalIceBlock,
				TileID.Boulder,
				TileID.BouncyBoulder,
				TileID.LifeCrystalBoulder,
				TileID.RollingCactus
			};
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide | AnchorType.Table, TileObjectData.newTile.Width, 0);
			TileObjectData.addTile(Type);
		}

		public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) {
			return true;
		}
		public override void MouseOver(int x, int y) {
			Main.LocalPlayer.cursorItemIconText = "";
			Main.LocalPlayer.cursorItemIconID = GetItemDrops(x, y).First().type;
			Main.LocalPlayer.noThrow = 2;
			Main.LocalPlayer.cursorItemIconEnabled = true;
		}
		public override void MouseOverFar(int x, int y) {
			MouseOver(x, y);
			if (Main.LocalPlayer.cursorItemIconText == "") {
				Main.LocalPlayer.cursorItemIconEnabled = false;
				Main.LocalPlayer.cursorItemIconID = 0;
			}
		}

		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
		public override string HighlightTexture => (GetType().Namespace + ".Sprites." + (Name.StartsWith("Bag") ? "Bag" : Name) + "_Highlight").Replace('.', '/');
	}
}
