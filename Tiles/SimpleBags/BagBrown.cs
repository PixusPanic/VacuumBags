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

namespace VacuumBags.Tiles
{
    public class BagBrown : SimpleBagTile
    {
		public override Color MapColor => Items.BagBrown.PanelColor;

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            if (!Main.LocalPlayer.HasItem(ModContent.ItemType<Items.BagBrown>()))
                Items.BagBrown.CloseBag();
        }
    }
}
