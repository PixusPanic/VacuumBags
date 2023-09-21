using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using VacuumBags;
using VacuumBags.Common;
using VacuumBags.Items;

namespace VacuumBags.Common.Globals
{
	public class GlobalBagTile : GlobalTile
	{
		public static int BannerBagType {
			get {
				if (bannerBagType == -1)
					bannerBagType = ModContent.TileType<Tiles.BannerBag>();

				return bannerBagType;
			}
		}
		private static int bannerBagType = -1;
		public static int PortableStationType {
			get {
				if (portableStationType == -1)
					portableStationType = ModContent.TileType<Tiles.PortableStation>();

				return portableStationType;
			}
		}
		private static int portableStationType = -1;
		public static int NearbyEffects(int type, ref SceneMetrics sceneMetrics) {
			if (type == BannerBagType) {
				Items.BannerBag.UpdateFromPlacedTile = true;
			}
			else if (type == PortableStationType) {
				if (Main.LocalPlayer.TryGetModPlayer(out BagPlayer bagPlayer))
					bagPlayer.NearPortableStation = true;

				Items.PortableStation.UpdateFromPlacedTile = true;
			}

			return type;
		}
	}
}
