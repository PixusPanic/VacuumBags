using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace VacuumBags.Common.Utility.LogModSystem {
	public class LogModSystem_VB : ModSystem {
		public static readonly bool printWiki = VacuumBags.clientConfig.PrintWikiInfo;
		public override void OnWorldLoad() {
			new VacuumBagsWiki().PrintWiki();
		}
	}
}
