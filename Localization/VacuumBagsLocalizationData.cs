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
						{ L_ID1.Config.ToString(), new(children: new() {
							{ CraftingHeaderKey, new(children: new() {
								{ nameof(serverConfig.HarderBagRecipes), new(dict: new() {
									{ L_ID3.Label.ToString(), "Harder Bag Recipes" },
									{ L_ID3.Tooltip.ToString(), "Makes the bag recipes require more items and some are more difficult to get." }
								}) },
							}) },
							{ BagStorageOptionsKey, new(children: new() {
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
									{ L_ID3.Label.ToString(), "All Ammo Items Go Into Ammo Bag" },
									{ L_ID3.Tooltip.ToString(), 
										"If true, all items that are used as ammo for anything will go into the bag.\n" +
										"If false, only the selected ammo items whitelist will be allowed which excludes a few items like stars." }
								}) },
							}) },
							}, dict: new() {
								{ CraftingHeaderKey, CraftingHeaderKey.AddSpaces() },
								{ BagStorageOptionsKey, BagStorageOptionsKey.AddSpaces() }
								/*,
								{ "", "" },
								{ "", "" },
								*/
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
