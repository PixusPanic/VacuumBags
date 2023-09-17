using androLib;
using androLib.Common.Globals;
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
using static Terraria.ID.ContentSamples.CreativeHelper;

namespace VacuumBags
{
	public class VacuumBagSystem : ModSystem
	{
		public const string AnyBossTrophy = "AnyBossTrophy";
		public const string AnyBossBag = "AnyBossBag";
		public const string AnyBanner = "AnyBanner";
		public override void AddRecipeGroups() {
			RecipeGroup emblems = new(() => AnyBossTrophy.AddSpaces(), ContentSamples.ItemsByType.Select(p => p.Value).Where(i => i.IsBossTrophy()).Select(i => i.type).ToArray());
			RecipeGroup.RegisterGroup($"{typeof(VacuumBags).Name}:{AnyBossTrophy}", emblems);

			RecipeGroup bossBags = new(() => AnyBossBag.AddSpaces(), BossBagsData.BossBags.ToArray());
			RecipeGroup.RegisterGroup($"{typeof(VacuumBags).Name}:{AnyBossBag}", bossBags);

			RecipeGroup banners = new(() => AnyBanner.AddSpaces(), ItemSets.AllBanners.ToArray());
			RecipeGroup.RegisterGroup($"{typeof(VacuumBags).Name}:{AnyBanner}", banners);
		}
		public override void PostSetupRecipes() {
			BagModItem.PostSetupRecipes();
		}
	}
}
