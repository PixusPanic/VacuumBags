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
			int[] bossTropies = ContentSamples.ItemsByType.Select(p => p.Value).Where(i => i.IsBossTrophy()).Select(i => i.type).ToArray();
			int indexOfKingSlimeTrophy = Array.IndexOf(bossTropies, ItemID.KingSlimeTrophy);
			if (indexOfKingSlimeTrophy != -1) {
				bossTropies[indexOfKingSlimeTrophy] = bossTropies[0];
				bossTropies[0] = ItemID.KingSlimeTrophy;
			}

			RecipeGroup trophies = new(() => AnyBossTrophy.AddSpaces(), bossTropies);
			RecipeGroup.RegisterGroup($"{typeof(VacuumBags).Name}:{AnyBossTrophy}", trophies);

			int[] bossBagItemTypes = BossBagsData.BossBags.ToArray();
			int indexOfKingSlimeBag = Array.IndexOf(bossBagItemTypes, ItemID.KingSlimeBossBag);
			if (indexOfKingSlimeBag != -1) {
				bossBagItemTypes[indexOfKingSlimeBag] = bossBagItemTypes[0];
				bossBagItemTypes[0] = ItemID.KingSlimeBossBag;
			}

			RecipeGroup bossBags = new(() => AnyBossBag.AddSpaces(), bossBagItemTypes);
			RecipeGroup.RegisterGroup($"{typeof(VacuumBags).Name}:{AnyBossBag}", bossBags);

			int[] bannerItemTypes = ItemSets.AllBanners.ToArray();
			int indexOfSlimeBanner = Array.IndexOf(bannerItemTypes, ItemID.GreenSlimeBanner);
			if (indexOfSlimeBanner != -1) {
				bannerItemTypes[indexOfSlimeBanner] = bannerItemTypes[0];
				bannerItemTypes[0] = ItemID.GreenSlimeBanner;
			}

			RecipeGroup banners = new(() => AnyBanner.AddSpaces(), bannerItemTypes);
			RecipeGroup.RegisterGroup($"{typeof(VacuumBags).Name}:{AnyBanner}", banners);
		}
		public override void PreSaveAndQuit() {
			ExquisitePotionFlask.PreSaveAndQuit();
		}
	}
}
