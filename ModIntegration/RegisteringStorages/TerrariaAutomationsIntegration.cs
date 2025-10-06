using androLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using VacuumBags.Items;

namespace VacuumBags.ModIntegration.RegisteringStorages {
	[JITWhenModsEnabled(AndroMod.terrariaAutomationsModName)]
	public class TerrariaAutomationsIntegration {
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Load() {
			if (!AndroMod.terrariaAutomationsModEnabled)
				return;

			//RegisterPipesStorage();
		}

		/*[MethodImpl(MethodImplOptions.NoInlining)]
		private static void RegisterPipesStorage() {
			TerrariaAutomations.TileData.TA_TileData.RegisterAdditionalPipeInventory(() => StorageManager.GetItems(MechanicsToolbelt.Instance.BagStorageID));
			MechanicsToolbelt.AdditonalDevWhitelistItems.Add(() => TerrariaAutomations.Items.Pipe.PipeType);
			MechanicsToolbelt.AdditonalDevWhitelistItems.Add(() => TerrariaAutomations.Items.PipeWrench.PipeWrenchType);
		}*/
	}
}
