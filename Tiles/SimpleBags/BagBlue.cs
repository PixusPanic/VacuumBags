using Microsoft.Xna.Framework;
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

namespace VacuumBags.Tiles
{
    public class BagBlue : SimpleBagTile
    {
		public override Color MapColor => Items.BagBlue.PanelColor;

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            if (!Main.LocalPlayer.HasItem(ModContent.ItemType<Items.BagBlue>()))
                Items.BagBlue.CloseBag();
        }
    }
}
