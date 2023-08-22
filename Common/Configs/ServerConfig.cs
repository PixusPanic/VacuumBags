using System;
using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ModLoader.Config;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using Terraria.ID;
using androLib.Common.Globals;
using androLib.Common.Utility;

namespace VacuumBags.Common.Configs
{
	public class BagsServerConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ServerSide;

		//Crafting
		[Header("$Mods.VacuumBags.Config.Crafting")]//TODO: Setup Localization

		//[Label("$Mods.VacuumBags.Config.HarderBagRecipes.Label")]
		[ReloadRequired]
		[DefaultValue(false)]
		public bool HarderBagRecipes;
	}

	public class BagsClientConfig : ModConfig {
		public override ConfigScope Mode => ConfigScope.ClientSide;

		//Display Settings
		[Header("$Mods.VacuumBags.Config.BagStorageOptions")]

		//[Label("$Mods.VacuumBags.Config.AllAmmoItemsGoIntoAmmoBag.Label")]
		//[Tooltip("$Mods.VacuumBags.Config.AllAmmoItemsGoIntoAmmoBag.Tooltip")]
		[ReloadRequired]
		[DefaultValue(true)]
		public bool AllAmmoItemsGoIntoAmmoBag;
	}
}
