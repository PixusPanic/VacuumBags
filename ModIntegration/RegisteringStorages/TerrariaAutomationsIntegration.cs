using androLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using TerrariaAutomations.Items;
using TerrariaAutomations.TileData;
using VacuumBags.Items;

namespace VacuumBags.ModIntegration.RegisteringStorages {
	[JITWhenModsEnabled(AndroMod.terrariaAutomationsModName)]
	public class TerrariaAutomationsIntegration : ModSystem {
		public override void Load() {
			if (AndroMod.magicStorageEnabled)
				RegisterPipesStorage();
		}
		public override void PostSetupContent() {
			if (AndroMod.terrariaAutomationsModEnabled)
				RegisterPipesStorage();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void RegisterPipesStorage() {
			TA_TileData.RegisterAdditionalPipeInventory(() => StorageManager.GetItems(MechanicsToolbelt.Instance.BagStorageID));
			MechanicsToolbelt.AdditonalDevWhitelistItems.Add(() => Pipe.PipeType);
			MechanicsToolbelt.AdditonalDevWhitelistItems.Add(() => PipeWrench.PipeWrenchType);
		}
	}
}
