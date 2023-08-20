using Terraria.ModLoader;
using VacuumBags.Items;

namespace VacuumBags
{
	public class VacuumBags : Mod {
		public override void Load() {
			BuildersBox.RegisterWithAndroLib(this);
			BagBlue.RegisterWithAndroLib(this);
			//BagBrown.RegisterWithAndroLib(this);
			//BagGreen.RegisterWithAndroLib(this);
			//BagPurple.RegisterWithAndroLib(this);
			//BagRed.RegisterWithAndroLib(this);
			//BagYellow.RegisterWithAndroLib(this);
		}
	}
}