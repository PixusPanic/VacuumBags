using androLib.Common.Utility;
using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VacuumBags.Items;

namespace VacuumBags
{
	public class VacuumBagSystem : ModSystem
	{
		public override void AddRecipeGroups() {
			RecipeGroup emblems = new(() => "Any Boss Trophy", BossBag.BossTrophies.ToArray());
			RecipeGroup.RegisterGroup("VacuumBags:AnyBossTrophy", emblems);

			RecipeGroup bossBags = new(() => "Any Boss Bag", BossBagsData.BossBags.ToArray());
			RecipeGroup.RegisterGroup("VacuumBags:AnyBossBag", bossBags);
		}
	}
}
