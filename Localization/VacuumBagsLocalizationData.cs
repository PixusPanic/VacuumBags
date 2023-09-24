using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using static Terraria.Localization.GameCulture;
using androLib.Common.Utility;
using androLib.Common.Globals;
using androLib.Common.Configs;
using androLib.Localization;
using static VacuumBags.Common.Configs.BagsServerConfig;
using static VacuumBags.Common.Configs.BagsClientConfig;
using static VacuumBags.VacuumBags;
using VacuumBags.Items;
using VacuumBags.Common.Configs;

namespace VacuumBags.Localization
{
	public class VacuumBagsLocalizationData
	{
		public static void RegisterSDataPackage() {
			AndroLogModSystem.RegisterModLocalizationSDataPackage(new(ModContent.GetInstance<VacuumBags>, () => AllData, () => ChangedData, () => RenamedKeys, () => RenamedFullKeys, () => SameAsEnglish));
		}

		private static SortedDictionary<string, SData> allData;
		public static SortedDictionary<string, SData> AllData {
			get {
				if (allData == null) {
					allData = new() {
						{ L_ID1.Items.ToString(), new(children: new() {
							//Intentionally empty.  Filled automatically
						}) },
						{ L_ID1.Configs.ToString(), new(children: new() {
							{ nameof(BagsServerConfig), new(children: new() {
								{ nameof(serverConfig.HarderBagRecipes), new(dict: new() {
									{ L_ID3.Label.ToString(), "Harder Bag Recipes" },
									{ L_ID3.Tooltip.ToString(), "Makes the bag recipes require more items and some are more difficult to get." }
								}) },
								{ nameof(serverConfig.BannerBagNumberOfBannersInInventory), new(dict: new() {
									{ L_ID3.Label.ToString(), nameof(serverConfig.BannerBagNumberOfBannersInInventory).AddSpaces() },
									{ L_ID3.Tooltip.ToString(), "When the Banner bag is in your inventory, the first X number of banners will apply their buffs.\n" +
										"Favoriting a banner or banners gives them priority over non-favorited ones.\n" +
										$"Set to {BannerBag.FirstXItemsChooseAllItems} for all banners in the bag to give their buffs." }
								}) },
								{ nameof(serverConfig.BannerBagNumberOfBannersWhenPlaced), new(dict: new() {
									{ L_ID3.Label.ToString(), nameof(serverConfig.BannerBagNumberOfBannersWhenPlaced).AddSpaces() },
									{ L_ID3.Tooltip.ToString(), "When the Banner bag is placed in the world, the first X number of banners will apply their buffs.\n" +
										"Favoriting a banner or banners gives them priority over non-favorited ones." +
										$"Set to {BannerBag.FirstXItemsChooseAllItems} for all banners in the bag to give their buffs." }
								}) },
								{ nameof(serverConfig.PortableStationNumberOfStationsInInventory), new(dict: new() {
									{ L_ID3.Label.ToString(), nameof(serverConfig.PortableStationNumberOfStationsInInventory).AddSpaces() },
									{ L_ID3.Tooltip.ToString(), "When the Portable Station is in your inventory, the first X number of stations will be available for crafting.\n" +
										"Favoriting a station or stations gives them priority over non-favorited ones.\n" +
										$"Set to {BannerBag.FirstXItemsChooseAllItems} for all stations in the Portable Station to be available for crafting." }
								}) },
								{ nameof(serverConfig.PortableStationNumberOfStationsWhenPlaced), new(dict: new() {
									{ L_ID3.Label.ToString(), nameof(serverConfig.PortableStationNumberOfStationsWhenPlaced).AddSpaces() },
									{ L_ID3.Tooltip.ToString(), "When the Portable Station is placed in the world, the first X number of stations will be available for crafting.\n" +
										"Favoriting a station or stations gives them priority over non-favorited ones.\n" +
										$"Set to {BannerBag.FirstXItemsChooseAllItems} for all stations in the Portable Station to be available for crafting." }
								}) },
								{ nameof(serverConfig.PortableStationNumberOfPassiveBuffStationsInInventory), new(dict: new() {
									{ L_ID3.Label.ToString(), nameof(serverConfig.PortableStationNumberOfPassiveBuffStationsInInventory).AddSpaces() },
									{ L_ID3.Tooltip.ToString(), "When the Portable Station is in your inventory, the passive buffs from the first X number of stations will be active.\n" +
										"Favoriting a station or stations gives them priority over non-favorited ones.\n" +
										$"Set to {BannerBag.FirstXItemsChooseAllItems} for all stations in the Portable Station to give their passive buffs.\n" +
										$"Peace Candles, Water candles and Shadow candles will not ever be active unless favorited and do not count towards the buff limit." }
								}) },
								{ nameof(serverConfig.PortableStationNumberOfPassiveBuffStationsWhenPlaced), new(dict: new() {
									{ L_ID3.Label.ToString(), nameof(serverConfig.PortableStationNumberOfPassiveBuffStationsWhenPlaced).AddSpaces() },
									{ L_ID3.Tooltip.ToString(), "When the Portable Station is placed in the world, the passive buffs from the first X number of stations will be active.\n" +
										"Favoriting a station or stations gives them priority over non-favorited ones.\n" +
										$"Set to {BannerBag.FirstXItemsChooseAllItems} for all stations in the Portable Station to give their passive buffs.\n" +
										$"Peace Candles, Water candles and Shadow candles will not ever be active unless favorited and do not count towards the buff limit." }
								}) },
								{ nameof(serverConfig.PortableStationsActivateActiveBuffsWhenOpened), new(dict: new() {
									{ L_ID3.Label.ToString(), nameof(serverConfig.PortableStationsActivateActiveBuffsWhenOpened).AddSpaces() },
									{ L_ID3.Tooltip.ToString(), "If selected, the buffs from stations that provide buffs when interacted with will give their buffs." }
								}) },
								{ nameof(serverConfig.PortableStationCanGiveHoneyBuff), new(dict: new() {
									{ L_ID3.Label.ToString(), nameof(serverConfig.PortableStationCanGiveHoneyBuff).AddSpaces() },
									{ L_ID3.Tooltip.ToString(), "If selected, the Honey buff is allowed to be given by the Portable Station." }
								}) },
								{ nameof(serverConfig.PortableStationMustBeTouchedToGetHoneyBuff), new(dict: new() {
									{ L_ID3.Label.ToString(), nameof(serverConfig.PortableStationMustBeTouchedToGetHoneyBuff).AddSpaces() },
									{ L_ID3.Tooltip.ToString(), "If selected, the Honey buff will be given if the Portable Station is touched if there is a bucket of honey in the station.\n" +
										"If not selected, the Honey buff will be given in an area of effect around the Portable Station.\n" +
										"If the Unlimited honey bucket is in the station, the area of effect will be active regardless of this option." }
								}) },
							},
							dict: new() {
								{ L_ID2.DisplayName.ToString(), "Server Config" },
								{ CraftingHeaderKey, CraftingHeaderKey.AddSpaces() },
								{ BagEffectOptionsKey, BagEffectOptionsKey.AddSpaces() },
							}) },
							{ nameof(BagsClientConfig), new(children: new() {
								{ nameof(clientConfig.SimpleBagStorageSize), new(dict: new() {
									{ L_ID3.Label.ToString(), "Simple Bag Storage Size" },
									{ L_ID3.Tooltip.ToString(),
										"The number of slots in the simple bag storage.\n" +
										"If this option is used while the player has more items in their bag, the bag size will be reduced to the number of items still in the bag.\n" +
										"Setting this number very high could cause lag or possibly a crash from using too much memory if you don't have much RAM.  Use at your own risk." }
								}) },
								{ nameof(clientConfig.SimpleBagsVacuumAllItems), new(dict: new() {
									{ L_ID3.Label.ToString(), "Simple Bags Vacuum All Items" },
									{ L_ID3.Tooltip.ToString(),
										"If true, all items will be vacuumed into the simple bags.\n" +
										"If false, only items that are the same type as an item in the bag will be vacuumed."  }
								}) },
								{ nameof(clientConfig.AllAmmoItemsGoIntoAmmoBag), new(dict: new() {
									{ L_ID3.Label.ToString(), nameof(clientConfig.AllAmmoItemsGoIntoAmmoBag).AddSpaces() },
									{ L_ID3.Tooltip.ToString(),
										"If true, all items that are used as ammo for anything will go into the bag.\n" +
										"If false, only the selected ammo items whitelist will be allowed which excludes a few items like stars." }
								}) },
								{ nameof(clientConfig.AllOtherCreateTileItemsIntoBuildersBox), new(dict: new() {
									{ L_ID3.Label.ToString(), nameof(clientConfig.AllOtherCreateTileItemsIntoBuildersBox).AddSpaces() },
									{ L_ID3.Tooltip.ToString(), "When on, every item that creates a tile when used will be added to the Builder's Box whitelist if it does not already go into another bag's whitelist.\n" +
										"This is supposed to catch most furniture, but may end up pulling in other items that aren't intended." }
								}) },
								{ nameof(clientConfig.PortableStationPassiveBuffsOnlyActiveIfFavorited), new(dict: new() {
									{ L_ID3.Label.ToString(), nameof(clientConfig.PortableStationPassiveBuffsOnlyActiveIfFavorited).AddSpaces() },
									{ L_ID3.Tooltip.ToString(), "If selected, only favorited buff stations will provide their buffs.\n" }
								}) },
								{ nameof(clientConfig.SilencePortableStationActiveBuffs), new(dict: new() {
									{ L_ID3.Label.ToString(), nameof(clientConfig.SilencePortableStationActiveBuffs).AddSpaces() },
									{ L_ID3.Tooltip.ToString(), "If selected, the Portable Station will not play any sounds when it provides active buffs on opening the station." }
								}) },
							},
							dict: new() {
								{ L_ID2.DisplayName.ToString(), "Client Config" },
								{ BagStorageOptionsKey, BagStorageOptionsKey.AddSpaces() },
							}) },
						}) }
					};


				}

				return allData;
			}
		}

		private static List<string> changedData;
		public static List<string> ChangedData {
			get {
				if (changedData == null)
					changedData = new();

				return changedData;
			}

			set => changedData = value;
		}

		private static Dictionary<string, string> renamedFullKeys;
		public static Dictionary<string, string> RenamedFullKeys {
			get {
				if (renamedFullKeys == null)
					renamedFullKeys = new();

				return renamedFullKeys;
			}

			set => renamedFullKeys = value;
		}

		public static Dictionary<string, string> RenamedKeys = new() {
			//{ typeof(ItemCooldown).Name, "AllForOne" },
			//{ DialogueID.HateCrowded.ToString(), "HateCrouded" }
		};

		public static Dictionary<CultureName, List<string>> SameAsEnglish = new() {
			{ CultureName.German,
				new() {
					"Wall-Er"
				}
			},
			{
				CultureName.Spanish,
				new() {
					"Wall-Er"
				}
			},
			{
				CultureName.French,
				new() {
					"Wall-Er"
				}
			},
			{
				CultureName.Italian,
				new() {
					"Wall-Er"
				}
			},
			{
				CultureName.Polish,
				new() {
					
				}
			},
			{
				CultureName.Portuguese,
				new() {
					"Wall-Er"
				}
			},
			{
				CultureName.Russian,
				new() {

				}
			},
			{
				CultureName.Chinese,
				new() {
					
				}
			},
		};
	}
	public static class VacuumBagsLocalizationDataStaticMethods
	{
		/// <summary>
		/// Should only be used for items directly in androLib, not items derived from AndroModItem, or the localization will end up in androLib localization.
		/// </summary>
		public static void AddLocalizationTooltip(this ModItem modItem, string tooltip, string name = null) {
			SortedDictionary<string, SData> all = VacuumBagsLocalizationData.AllData;
			if (AndroLogModSystem.printLocalization || AndroLogModSystem.printLocalizationKeysAndValues) {
				VacuumBagsLocalizationData.AllData[L_ID1.Items.ToString()].Children.Add(modItem.Name, new(dict: new()));
				VacuumBagsLocalizationData.AllData[L_ID1.Items.ToString()].Children[modItem.Name].Dict.Add(L_ID1.Tooltip.ToString(), tooltip);
				VacuumBagsLocalizationData.AllData[L_ID1.Items.ToString()].Children[modItem.Name].Dict.Add(L_ID2.DisplayName.ToString(), name ?? modItem.Name.AddSpaces());
			}
		}
	}
}
