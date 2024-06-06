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
using VacuumBags.Items;
using androLib;
using androLib.Items;

namespace VacuumBags.Tiles
{
	public abstract class VacuumBagTile : ModTile
	{
		protected abstract IBagModItem ModBag { get; }
		public virtual Color MapColor => ModBag.PanelColor;
		protected void SetSimpleBagDefaults() {
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
			TileObjectData.newTile.CoordinateHeights = new[] { 16 };
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
		public override void SetStaticDefaults() {
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = false;
			Main.tileFrameImportant[Type] = true;
			TileID.Sets.HasOutlines[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;
			TileID.Sets.IgnoredByNpcStepUp[Type] = true;
			TileID.Sets.BasicChest[Type] = true; 

			AdjTiles = new int[] { Type };
			Color mapColor = MapColor;
			mapColor.A = byte.MaxValue;
			AddMapEntry(mapColor, CreateMapEntryName());
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
		public override void KillMultiTile(int i, int j, int frameX, int frameY) {
			if (Main.netMode == NetmodeID.Server)
				return;

			if (!StorageManager.HasRequiredItemToUseStorageFromBagType(Main.LocalPlayer, ModBag.GetBagType(), out _, out _, out _))
				ModBag.CloseBag();
		}

		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
		public override string HighlightTexture => (GetType().Namespace + $".Sprites.{Name}_Highlight").Replace('.', '/');
	}
}
