using androLib.Common.Utility;
using androLib;
using androLib.Items;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Terraria.ID.ContentSamples.CreativeHelper;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using androLib.ModIntegration;
using Terraria.GameContent.LootSimulation;
using androLib.Common.Globals;
using static androLib.Items.IBagModItem;

namespace VacuumBags.Items
{
    [Autoload(false)]
	public class BossBag : AllowedListBagModItem_VB {
		public static IBagModItem Instance {
			get {
				if (instance == null)
					instance = new BossBag();

				return instance;
			}
		}
		private static IBagModItem instance;
		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
		public override void SetDefaults() {
			Item.maxStack = 1;
			Item.value = 100000;
			Item.rare = ItemRarityID.Blue;
			Item.width = 32;
			Item.height = 32;
		}
		public override int GetBagType() => ModContent.ItemType<BossBag>();
		public override void AddRecipes() {
			if (!VacuumBags.serverConfig.HarderBagRecipes) {
				CreateRecipe()
				.AddTile(TileID.WorkBenches)
				.AddRecipeGroup($"{VacuumBags.ModName}:{VacuumBagSystem.AnyBossTrophy}", 1)
				.AddRecipeGroup($"{VacuumBags.ModName}:{VacuumBagSystem.AnyBossBag}", 1)
				.AddIngredient(ItemID.GoldChest, 1)
				.Register();
			}
			else {
				CreateRecipe()
				.AddTile(TileID.HeavyWorkBench)
				.AddIngredient(ItemID.SkeletronBossBag)
				.AddIngredient(ItemID.EyeofCthulhuTrophy)
				.AddIngredient(ItemID.DeadMansChest, 1)
				.Register();
			}
		}

		public override Color PanelColor => new Color(121, 92, 18, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ScrollBarColor => new Color(155, 130, 20, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ButtonHoverColor => new Color(147, 125, 30, androLib.Common.Configs.ConfigValues.UIAlpha);

		public override bool? DevCheck(ItemSetInfo info, SortedSet<ItemGroup> itemGroups, SortedSet<string> endWords, SortedSet<string> searchWords) {
			if (info.BossTrophyOrRelic || info.BossSpawner)
				return true;

			if (bossDropItems.Contains(info.Type))
				return true;

			return null;
		}
		private static SortedSet<int> bossDropItems = null;
		public override void PostSetup() {
			bossDropItems = null;
		}
		public override SortedSet<int> DevWhiteList() {
			SortedSet<int> devWhiteList = new() {
				ItemID.GuideVoodooDoll,
				ItemID.RedsWings,
				ItemID.RedsHelmet,
				ItemID.RedsBreastplate,
				ItemID.RedsLeggings,
				ItemID.LihzahrdPowerCell,
				ItemID.ClothierVoodooDoll,
				ItemID.CenxsTiara,
				ItemID.CenxsBreastplate,
				ItemID.CenxsLeggings,
				ItemID.CrownosMask,
				ItemID.CrownosBreastplate,
				ItemID.CrownosLeggings,
				ItemID.WillsHelmet,
				ItemID.WillsBreastplate,
				ItemID.WillsLeggings,
				ItemID.JimsHelmet,
				ItemID.JimsBreastplate,
				ItemID.JimsLeggings,
				ItemID.AaronsHelmet,
				ItemID.AaronsBreastplate,
				ItemID.AaronsLeggings,
				ItemID.DTownsHelmet,
				ItemID.DTownsBreastplate,
				ItemID.DTownsLeggings,
				ItemID.DTownsWings,
				ItemID.WillsWings,
				ItemID.CrownosWings,
				ItemID.CenxsWings,
				ItemID.CenxsDress,
				ItemID.CenxsDressPants,
				ItemID.BejeweledValkyrieHead,
				ItemID.BejeweledValkyrieBody,
				ItemID.BejeweledValkyrieWing,
				ItemID.Yoraiz0rShirt,
				ItemID.Yoraiz0rPants,
				ItemID.Yoraiz0rWings,
				ItemID.Yoraiz0rDarkness,
				ItemID.JimsWings,
				ItemID.Yoraiz0rHead,
				ItemID.SkiphsHelm,
				ItemID.SkiphsShirt,
				ItemID.SkiphsPants,
				ItemID.SkiphsWings,
				ItemID.LokisHelm,
				ItemID.LokisShirt,
				ItemID.LokisShirt,
				ItemID.LokisShirt,
				ItemID.DD2ElderCrystalStand,
				ItemID.DD2ElderCrystal,
				ItemID.ArkhalisHat,
				ItemID.ArkhalisShirt,
				ItemID.ArkhalisPants,
				ItemID.ArkhalisWings,
				ItemID.LeinforsHat,
				ItemID.LeinforsShirt,
				ItemID.LeinforsPants,
				ItemID.LeinforsWings,
				ItemID.LeinforsAccessory,
				ItemID.GhostarSkullPin,
				ItemID.GhostarShirt,
				ItemID.GhostarPants,
				ItemID.GhostarsWings,
				ItemID.SafemanWings,
				ItemID.SafemanSunHair,
				ItemID.SafemanSunDress,
				ItemID.SafemanSunDress,
				ItemID.FoodBarbarianWings,
				ItemID.FoodBarbarianWings,
				ItemID.FoodBarbarianArmor,
				ItemID.FoodBarbarianGreaves,
				ItemID.GroxTheGreatWings,
				ItemID.GroxTheGreatHelm,
				ItemID.GroxTheGreatArmor,
				ItemID.GroxTheGreatGreaves,
			};

			devWhiteList.UnionWith(BossBagsData.BossBags);

			bossDropItems = new();
			if (BossChecklistIntegration.BossInfos != null) {
				foreach (BossChecklistBossInfo bossChecklistInfo in BossChecklistIntegration.BossInfos.Select(p => p.Value)) {
					foreach (int bossSummonType in bossChecklistInfo.spawnItem) {
						devWhiteList.Add(bossSummonType);
					}
				}

				foreach (BossChecklistBossInfo bossChecklistInfo in BossChecklistIntegration.BossInfos.Select(p => p.Value)) {
					foreach (int itemType in bossChecklistInfo.loot) {
						if (itemType <= ItemID.None || itemType >= ItemLoader.ItemCount)
							continue;

						if (devWhiteList.Contains(itemType))
							continue;

						//Vanilla item from modded enemy
						if (itemType < ItemID.Count && bossChecklistInfo.modSource != "Terraria")
							continue;

						ItemSetInfo info = new(itemType);

						if (info.Equipment || info.Torch || info.Glowstick || info.Rope || info.Coin)
							continue;

						if (info.Vanity) {
							devWhiteList.Add(itemType);
							continue;
						}

						if (info.Consumable && !info.CreateTile && !info.CreateWall)
							continue;

						if (info.Material && !info.CreateTile && !info.CreateWall && (info.Consumable || !info.CanShoot && !info.HasBuff))
							continue;

						bossDropItems.Add(itemType);
					}
				}
			}

			return devWhiteList;
		}
		public override SortedSet<string> DevModBlackList() {
			SortedSet<string> devModBlackList = new() {
				"WeaponEnchantments/PowerBooster",
				"WeaponEnchantments/UltraPowerBooster",
				"StarsAbove/Starlight"
			};

			return devModBlackList;
		}
		public override SortedSet<ItemGroup> ItemGroups() {
			SortedSet<ItemGroup> itemGroups = new() {
				ItemGroup.BossBags,
				ItemGroup.BossSpawners,
				ItemGroup.EventItem,
				ItemGroup.BossItem
			};
			
			return itemGroups;
		}

		#region AndroModItem attributes that you don't need.

		public override string LocalizationTooltip =>
			$"Automatically stores boss bags, trophies, boss statues, and cosmetic boss drops.\n" +
			$"When in your inventory, the contents of the bag are available for crafting.\n" +
			$"Right click to open the bag.\n" +
			$"Uses Boss Checklist to add modded items to the allowed items if it is enabled.";
		public override string Artist => "@kingjoshington";
		public override string Designer => "andro951";

		#endregion
	}
}
