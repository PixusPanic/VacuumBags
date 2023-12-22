using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using VacuumBags.Localization;
using VacuumOreBag.Items;

namespace VacuumBags.Items {
	public abstract class BagModItem_VB : BagModItem {
		protected override Action<ModItem, string, string> AddLocalizationTooltipFunc => VacuumBagsLocalizationDataStaticMethods.AddLocalizationTooltip;

	}
}
