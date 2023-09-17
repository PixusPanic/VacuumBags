using System;
using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ModLoader.Config;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using Terraria.ID;
using androLib.Common.Globals;
using androLib.Common.Utility;
using VacuumBags.Items;

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

		[JsonIgnore]
		public const string BagEffectOptionsKey = "BagEffectOptions";
		[JsonIgnore]
		public const int BannerBagNumberOfBannersInInventoryDefault = 3;
		[Header($"$Mods.VacuumBags.Config.{BagEffectOptionsKey}")]
		[DefaultValue(BannerBagNumberOfBannersInInventoryDefault)]
		[Range(BagModItem.FirstXItemsChooseAllItems, BagsClientConfig.BagsMaxStorageSize)]
		public int BannerBagNumberOfBannersInInventory;

		[DefaultValue(BagModItem.FirstXItemsChooseAllItems)]
		[Range(BagModItem.FirstXItemsChooseAllItems, BagsClientConfig.BagsMaxStorageSize)]
		public int BannerBagNumberOfBannersWhenPlaced;

		[JsonIgnore]
		public const int PortableStationNumberOfStationsInInventoryDefault = 1;
		[DefaultValue(PortableStationNumberOfStationsInInventoryDefault)]
		[Range(BagModItem.FirstXItemsChooseAllItems, BagsClientConfig.BagsMaxStorageSize)]
		public int PortableStationNumberOfStationsInInventory;

		[DefaultValue(BagModItem.FirstXItemsChooseAllItems)]
		[Range(BagModItem.FirstXItemsChooseAllItems, BagsClientConfig.BagsMaxStorageSize)]
		public int PortableStationNumberOfStationsWhenPlaced;
	}

	public class BagsClientConfig : ModConfig {
		public override ConfigScope Mode => ConfigScope.ClientSide;

		//Storage Options
		[JsonIgnore]
		public const string BagStorageOptionsKey = "BagStorageOptions";
		[Header($"$Mods.VacuumBags.Config.{BagStorageOptionsKey}")]

		[JsonIgnore]
		public const int BagsMaxStorageSize = 10000;

		[ReloadRequired]
		[Range(1, BagsMaxStorageSize)]
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
