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
		[JsonIgnore]
		public const string CraftingHeaderKey = "Crafting";
		[Header($"$Mods.VacuumBags.Config.{CraftingHeaderKey}")]

		[ReloadRequired]
		[DefaultValue(false)]
		public bool HarderBagRecipes;
	}

	public class BagsClientConfig : ModConfig {
		public override ConfigScope Mode => ConfigScope.ClientSide;

		//Storage Options
		[JsonIgnore]
		public const string BagStorageOptionsKey = "BagStorageOptions";
		[Header($"$Mods.VacuumBags.Config.{BagStorageOptionsKey}")]

		[ReloadRequired]
		[Range(1, 10000)]
		[DefaultValue(40)]
		public int SimpleBagStorageSize;

		[ReloadRequired]
		[DefaultValue(false)]
		public bool SimpleBagsVacuumAllItems;

		[ReloadRequired]
		[DefaultValue(true)]
		public bool AllAmmoItemsGoIntoAmmoBag;
	}
}
