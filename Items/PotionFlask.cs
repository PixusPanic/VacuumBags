using androLib.Common.Utility;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;
using Microsoft.Xna.Framework;
using androLib.Items;
using androLib.Common.Globals;
using androLib;
using static Terraria.ID.ContentSamples.CreativeHelper;

namespace VacuumBags.Items
{
	public  class PotionFlask : VBModItem, ISoldByWitch {
		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
		public override void SetDefaults() {
            Item.maxStack = 1;
            Item.value = 100000;
			Item.rare = ItemRarityID.Blue;
			Item.width = 32;
            Item.height = 32;
        }
        public override void AddRecipes() {
			if (!VacuumBags.serverConfig.HarderBagRecipes) {
				CreateRecipe()
				.AddTile(TileID.WorkBenches)
				.AddIngredient(ItemID.Glass, 100)
				.AddIngredient(ItemID.Fireblossom, 10)
				.AddIngredient(ItemID.Moonglow, 10)
				.AddIngredient(ItemID.Shiverthorn, 10)
				.Register();
			}
			else {
				CreateRecipe()
				.AddTile(TileID.AlchemyTable)
				.AddIngredient(ItemID.Glass, 100)
				.AddIngredient(ItemID.LesserHealingPotion, 100)
				.AddIngredient(ItemID.Fireblossom, 20)
				.AddIngredient(ItemID.Moonglow, 20)
				.AddIngredient(ItemID.Shiverthorn, 20)
				.AddIngredient(ItemID.Blinkroot, 20)
				.AddIngredient(ItemID.Daybloom, 20)
				.AddIngredient(ItemID.Deathweed, 20)
				.AddIngredient(ItemID.Waterleaf, 20)
				.Register();
			}
		}

		public static int BagStorageID;//Set this when registering with androLib.


		public static void RegisterWithAndroLib(Mod mod) {
			BagStorageID = StorageManager.RegisterVacuumStorageClass(
				mod,//Mod
				typeof(PotionFlask),//type 
				ItemAllowedToBeStored,//Is allowed function, Func<Item, bool>
				null,//Localization Key name.  Attempts to determine automatically by treating the type as a ModItem, or you can specify.
				100,//StorageSize
				true,//Can vacuum
				() => new Color(80, 10, 80, androLib.Common.Configs.ConfigValues.UIAlpha),    // Get color function. Func<using Microsoft.Xna.Framework.Color>
				() => new Color(90, 10, 90, androLib.Common.Configs.ConfigValues.UIAlpha),    // Get Scroll bar color function. Func<using Microsoft.Xna.Framework.Color>
				() => new Color(120, 0, 120, androLib.Common.Configs.ConfigValues.UIAlpha),   // Get Button hover color function. Func<using Microsoft.Xna.Framework.Color>
				() => ModContent.ItemType<PotionFlask>(),//Get ModItem type
				80,//UI Left
				675//UI Top
			);
		}
		public static bool ItemAllowedToBeStored(Item item) => AllowedItems.Contains(item.type);

		public static SortedSet<int> AllowedItems {
			get {
				if (allowedItems == null)
					GetAllowedItems();

				return allowedItems;
			}
		}
		private static SortedSet<int> allowedItems = null;

		private static void GetAllowedItems() {
			allowedItems = new() {
				ItemID.RecallPotion,
				ItemID.PotionOfReturn,
				ItemID.LesserHealingPotion,
				ItemID.HealingPotion,
				ItemID.GreaterHealingPotion,
				ItemID.LesserRestorationPotion,
				ItemID.RestorationPotion,
				ItemID.LesserManaPotion,
				ItemID.ManaPotion,
				ItemID.GreaterManaPotion,
				ItemID.SuperManaPotion,
				ItemID.WormholePotion,
				ItemID.SuperHealingPotion,

				ItemID.ObsidianSkinPotion,
				ItemID.RegenerationPotion,
				ItemID.SwiftnessPotion,
				ItemID.GillsPotion,
				ItemID.IronskinPotion,
				ItemID.ManaRegenerationPotion,
				ItemID.MagicPowerPotion,
				ItemID.FeatherfallPotion,
				ItemID.SpelunkerPotion,
				ItemID.InvisibilityPotion,
				ItemID.ShinePotion,
				ItemID.NightOwlPotion,
				ItemID.BattlePotion,
				ItemID.ThornsPotion,
				ItemID.WaterWalkingPotion,
				ItemID.ArcheryPotion,
				ItemID.HunterPotion,
				ItemID.GravitationPotion,
				ItemID.RedPotion,
				ItemID.MiningPotion,
				ItemID.HeartreachPotion,
				ItemID.CalmingPotion,
				ItemID.BuilderPotion,
				ItemID.TitanPotion,
				ItemID.FlipperPotion,
				ItemID.SummoningPotion,
				ItemID.TrapsightPotion,
				ItemID.AmmoReservationPotion,
				ItemID.LifeforcePotion,
				ItemID.EndurancePotion,
				ItemID.RagePotion,
				ItemID.InfernoPotion,
				ItemID.WrathPotion,
				ItemID.TeleportationPotion,
				ItemID.LovePotion,
				ItemID.StinkPotion,
				ItemID.FishingPotion,
				ItemID.SonarPotion,
				ItemID.CratePotion,
				ItemID.WarmthPotion,
				ItemID.GenderChangePotion,
				ItemID.LuckPotionLesser,
				ItemID.LuckPotion,
				ItemID.LuckPotionGreater,
				ItemID.BiomeSightPotion,
			};

			SortedSet<string> endWords = new() {
				"potion"
			};

			SortedSet<string> searchWords = new() {
				
			};

			for (int i = 0; i < ItemLoader.ItemCount; i++) {
				Item item = ContentSamples.ItemsByType[i];
				if (item.NullOrAir())
					continue;

				string lowerName = item.Name.ToLower();
				bool added = false;
				foreach (string endWord in endWords) {
					if (lowerName.EndsWith(endWord)) {
						allowedItems.Add(item.type);
						added = true;
						break;
					}
				}

				if (added)
					continue;

				foreach (string searchWord in searchWords) {
					if (lowerName.Contains(searchWord)) {
						allowedItems.Add(item.type);
						added = true;
						break;
					}
				}

				if (ItemID.Sets.IsFood[item.type]) {
					allowedItems.Add(item.type);
					continue;
				}

				ItemGroupAndOrderInGroup group = new ItemGroupAndOrderInGroup(item);
				if (group.Group == ItemGroup.BuffPotion || group.Group == ItemGroup.LifePotions || group.Group == ItemGroup.ManaPotions || group.Group == ItemGroup.Flask || group.Group == ItemGroup.Food) {
					allowedItems.Add(item.type);
					continue;
				}
			}
		}

		#region AndroModItem attributes that you don't need.

		public virtual SellCondition SellCondition => SellCondition.Never;
		public virtual float SellPriceModifier => 1f;
		public override List<WikiTypeID> WikiItemTypes => new() { WikiTypeID.Storage };
		public override string LocalizationTooltip =>
			$"Automatically stores potions, food and drink.\n" +
			$"When in your inventory, the contents of the bag are available for crafting.\n" +
			$"Right click to open the bag.";
		public override string Artist => "andro951";
		public override string Designer => "@kingjoshington";

		#endregion
	}
}
