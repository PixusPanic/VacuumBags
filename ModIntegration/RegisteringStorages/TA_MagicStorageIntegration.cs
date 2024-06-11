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
        public static void PostSetupContent() {
			if (!AndroMod.magicStorageEnabled)
				return;

			PostSetupContentInner();
		}
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void PostSetupContentInner() {
			int biomeGlobeItem = ModContent.ItemType<MagicStorage.Items.BiomeGlobe>();
			if (biomeGlobeItem != -1) {
				ItemID.Sets.WorksInVoidBag[biomeGlobeItem] = true;
			}
		}
	}
}
