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
		
		// Debugging
		[JsonIgnore]
		public const string DebuggingInformationKey = "DebuggingInformation";
		[Header($"$Mods.{VacuumBags.ModName}.{L_ID_Tags.Configs}.{ClientConfigName}.{DebuggingInformationKey}")]

		[DefaultValue(false)]
		[ReloadRequired]
		public bool ThrowExceptions;
	}
	
	#region BagToggle
	//[SeparatePage]
	public class BagToggle : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ServerSide;
		public const string BagToggleConfigName = "BagToggle";
		
		[JsonIgnore]
		public const string BagToggleHeaderKey = "BagToggle";
		[Header($"$Mods.{VacuumBags.ModName}.{L_ID_Tags.Configs}.{BagToggleConfigName}.{BagToggleHeaderKey}")]
		
		/*[ReloadRequired]
		[DefaultValue(true)]
		public bool OreBag;*/
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool BannerBag;

		[ReloadRequired]
		[DefaultValue(true)]
		public bool FishingBelt;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool PortableStation;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool PaintBucket;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool PotionFlask;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool ExquisitePotionFlask;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool HerbSatchel;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool MechanicsToolbelt;

		[ReloadRequired]
		[DefaultValue(true)]
		public bool JarOfDirt;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool AmmoBag;

		[ReloadRequired]
		[DefaultValue(true)]
		public bool BossBag;

		[ReloadRequired]
		[DefaultValue(true)]
		public bool BuildersBox;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool WallEr;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool SlayersSack;

		[ReloadRequired]
		[DefaultValue(true)]
		public bool TrashCan;

		[ReloadRequired]
		[DefaultValue(true)]
		public bool BagsAndPacks;
		
		[JsonIgnore]
		public const string ModSupportHeaderKey = "ModSupport";
		[Header($"$Mods.{VacuumBags.ModName}.{L_ID_Tags.Configs}.{BagToggleConfigName}.{ModSupportHeaderKey}")]
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool ModBags;
		
		[ReloadRequired]
		[DefaultValue(false)]
		public bool RequireModsForModBags;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool CalamitousCauldron;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool LokisTesseract;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool EssenceOfGathering;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool FargosMementos;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool SpookyGourd;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool EarthenPyramid;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool HoiPoiCapsule;
	}
	#endregion
}
