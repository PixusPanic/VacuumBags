using androLib;
using MagicStorage;
using MagicStorage.Components;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using androLib.Common.Globals;
using androLib.Common.Utility;
using androLib.UI;
using Terraria.Audio;

namespace androLib.ModIntegration
{
    [JITWhenModsEnabled(AndroMod.magicStorageName)]
    public class TA_MagicStorageIntegration {
        public static int SearchForBiomeGlobe(Player player, IList<Item> items) {
            if (!AndroMod.magicStorageEnabled)
                return - 1;

            return SearchForBiomeGlobeInner(player, items);
        }

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static int SearchForBiomeGlobeInner(Player player,IList<Item> items) {
            int biomeGlobeItem = ModContent.ItemType<MagicStorage.Items.BiomeGlobe>();
            if (player.TryGetModPlayer(out MagicStorage.Items.BiomePlayer biomePlayer)) {
                for (int i = 0; i < items.Count; i++) {
                    Item item = items[i];
                    if (item.NullOrAir() || item.stack < 1)
                        continue;

					if (item.type == biomeGlobeItem) {
						biomePlayer.biomeGlobe = true;
						return i;
					}
				}
			}

            return -1;
		}
	}
}
