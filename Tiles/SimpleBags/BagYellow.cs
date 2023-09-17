using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;
using androLib;

namespace VacuumBags.Tiles
{
    public class BagYellow : SimpleBagTile
    {
		public override Color MapColor => Items.BagYellow.PanelColor;

		public override void KillMultiTile(int i, int j, int frameX, int frameY) {
			if (Main.netMode == NetmodeID.Server)
				return;

			if (!StorageManager.HasRequiredItemToUseStorageFromBagType(Main.LocalPlayer, ModContent.ItemType<Items.BagYellow>(), out _))
				Items.BagYellow.CloseBag();
        }
    }
}
