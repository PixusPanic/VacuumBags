using Terraria.ModLoader;
using VacuumBags.Common.Configs;
using VacuumBags.Items;

namespace VacuumBags
{
	public class VacuumBags : Mod {
		public static BagsServerConfig serverConfig = ModContent.GetInstance<BagsServerConfig>();
		public static BagsClientConfig clientConfig = ModContent.GetInstance<BagsClientConfig>();
		public override void Load() {
			BagBlue.RegisterWithAndroLib(this);
			BagBrown.RegisterWithAndroLib(this);
			BagGreen.RegisterWithAndroLib(this);
			BagPurple.RegisterWithAndroLib(this);
			BagRed.RegisterWithAndroLib(this);
			BagYellow.RegisterWithAndroLib(this);
			BuildersBox.RegisterWithAndroLib(this);
			WallEr.RegisterWithAndroLib(this);
			PaintBucket.RegisterWithAndroLib(this);
			PotionFlask.RegisterWithAndroLib(this);
			HerbSatchel.RegisterWithAndroLib(this);
			AmmoBag.RegisterWithAndroLib(this);
		}
	}
}