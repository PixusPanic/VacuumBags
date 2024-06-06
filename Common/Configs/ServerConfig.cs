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
using androLib.Common.Configs;
using androLib.Items;

namespace VacuumBags.Common.Configs
{
	public class BagsServerConfig : ModConfig
	{
		public const string ServerConfigName = "BagsServerConfig";
		public override ConfigScope Mode => ConfigScope.ServerSide;

		//Crafting
		[JsonIgnore]
		public const string CraftingHeaderKey = "Crafting";
		[Header($"$Mods.{VacuumBags.ModName}.{L_ID_Tags.Configs}.{ServerConfigName}.{CraftingHeaderKey}")]

		[ReloadRequired]
		[DefaultValue(false)]
		public bool HarderBagRecipes;

		//Bag Effects
		[JsonIgnore]
		public const string BagEffectOptionsKey = "BagEffectOptions";
		[JsonIgnore]
		public const int BannerBagNumberOfBannersInInventoryDefault = 3;
		[Header($"$Mods.{VacuumBags.ModName}.{L_ID_Tags.Configs}.{ServerConfigName}.{BagEffectOptionsKey}")]

		[DefaultValue(BannerBagNumberOfBannersInInventoryDefault)]
		[Range(IBagModItem.FirstXItemsChooseAllItems, StorageSizePair.MaxStorageSize)]
		public int BannerBagNumberOfBannersInInventory;

		[DefaultValue(IBagModItem.FirstXItemsChooseAllItems)]
		[Range(IBagModItem.FirstXItemsChooseAllItems, StorageSizePair.MaxStorageSize)]
		public int BannerBagNumberOfBannersWhenPlaced;

		[JsonIgnore]
		public const int PortableStationNumberOfCraftingStationsInInventoryDefault = 1;
		[DefaultValue(PortableStationNumberOfCraftingStationsInInventoryDefault)]
		[Range(IBagModItem.FirstXItemsChooseAllItems, StorageSizePair.MaxStorageSize)]
		public int PortableStationNumberOfStationsInInventory;

		[DefaultValue(IBagModItem.FirstXItemsChooseAllItems)]
		[Range(IBagModItem.FirstXItemsChooseAllItems, StorageSizePair.MaxStorageSize)]
		public int PortableStationNumberOfStationsWhenPlaced;

		[JsonIgnore]
		public const int PortableStationNumberOfPassiveBuffStationsInInventoryDefault = 1;
		[DefaultValue(PortableStationNumberOfPassiveBuffStationsInInventoryDefault)]
		[Range(IBagModItem.FirstXItemsChooseAllItems, StorageSizePair.MaxStorageSize)]
		public int PortableStationNumberOfPassiveBuffStationsInInventory;

		[DefaultValue(IBagModItem.FirstXItemsChooseAllItems)]
		[Range(IBagModItem.FirstXItemsChooseAllItems, StorageSizePair.MaxStorageSize)]
		public int PortableStationNumberOfPassiveBuffStationsWhenPlaced;

		[DefaultValue(true)]
		public bool PortableStationsActivateActiveBuffsWhenOpened;

		[DefaultValue(true)]
		public bool PortableStationCanGiveHoneyBuff;

		[DefaultValue(true)]
		public bool PortableStationMustBeTouchedToGetHoneyBuff;

		[DefaultValue(true)]
		public bool PotionFlaskSavesBuffsOnDeath;
	}

	public class BagsClientConfig : ModConfig {
		public const string ClientConfigName = "BagsClientConfig";
		public override ConfigScope Mode => ConfigScope.ClientSide;

		//Storage Options
		[JsonIgnore]
		public const string BagStorageOptionsKey = "BagStorageOptions";
		[Header($"$Mods.{VacuumBags.ModName}.{L_ID_Tags.Configs}.{ClientConfigName}.{BagStorageOptionsKey}")]

		[ReloadRequired]
		[DefaultValue(false)]
		public bool SimpleBagsVacuumAllItems;

		[ReloadRequired]
		[DefaultValue(true)]
		public bool AllAmmoItemsGoIntoAmmoBag;

		[ReloadRequired]
		[DefaultValue(true)]
		public bool AllOtherCreateTileItemsIntoBuildersBox;

		[DefaultValue(false)]
		public bool PortableStationPassiveBuffsOnlyActiveIfFavorited;

		[DefaultValue(false)]
		public bool SilencePortableStationActiveBuffs;

		[DefaultValue(false)]
		public bool TurnOffRegularPotionFlask;

		//Logging Information
		[JsonIgnore]
		public const string LoggingInformationKey = "LoggingInformation";
		[Header($"$Mods.{VacuumBags.ModName}.{L_ID_Tags.Configs}.{ClientConfigName}.{LoggingInformationKey}")]

		[DefaultValue(false)]
		[ReloadRequired]
		public bool PrintWikiInfo;
	}
}
