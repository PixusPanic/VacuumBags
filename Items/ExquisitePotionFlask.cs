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
using System;
using System.Reflection;
using Terraria.Audio;
using System.Diagnostics.CodeAnalysis;
using Terraria.ModLoader.IO;
using Humanizer;
using System.Security.Policy;
using Terraria.GameContent;
using androLib.UI;
using VacuumBags.Common.Configs;
using static androLib.Items.IBagModItem;

namespace VacuumBags.Items
{
	[Autoload(false)]
	public  class ExquisitePotionFlask : PotionFlask {
		new public static IBagModItem Instance {
			get {
				if (instance == null)
					instance = new ExquisitePotionFlask();

				return instance;
			}
		}
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<BagToggle>().ExquisitePotionFlask;
		}
		
		private static IBagModItem instance;
		public override int BagStorageID { get => PotionFlask.Instance.BagStorageID; set => PotionFlask.Instance.BagStorageID = value; }
		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
		public override void SetDefaults() {
            Item.maxStack = 1;
            Item.value = 100000;
			Item.rare = ItemRarityID.Cyan;
			Item.width = 32;
            Item.height = 32;
		}
		public override int GetBagType() => ModContent.ItemType<ExquisitePotionFlask>();
		public override void AddRecipes() {
			if (!VacuumBags.serverConfig.HarderBagRecipes) {
				CreateRecipe()
				.AddTile(TileID.AdamantiteForge)
				.AddIngredient(ModContent.ItemType<PotionFlask>())
				.AddIngredient(ItemID.BeetleHusk, 10)
				.AddIngredient(ItemID.Ectoplasm, 15)
				.AddRecipeGroup($"{AndroMod.ModName}:{AndroModSystem.CursedFlameOrIchor}", 20)
				.AddIngredient(ItemID.UnicornHorn, 5)
				.Register();
			}
			else {
				CreateRecipe()
				.AddTile(TileID.AdamantiteForge)
				.AddIngredient(ModContent.ItemType<PotionFlask>())
				.AddIngredient(ItemID.BeetleHusk, 20)
				.AddIngredient(ItemID.Ectoplasm, 30)
				.AddRecipeGroup($"{AndroMod.ModName}:{AndroModSystem.CursedFlameOrIchor}", 40)
				.AddIngredient(ItemID.SoulofFlight, 20)
				.AddIngredient(ItemID.SoulofFright, 10)
				.AddIngredient(ItemID.SoulofMight, 10)
				.AddIngredient(ItemID.SoulofSight, 10)
				.Register();
			}
		}

		#region AndroModItem attributes that you don't need.

		public override string LocalizationTooltip => base.LocalizationTooltip + $"\n\n" +
			$"Potions will last twice as long as normal by continuously adding to the time remaining so that only 1 second is used every 2 seconds.\n" +
			$"(Pairs well with the Legendary Juiced Enchantment for permanent infinite duration if that's what you want.)\n" +
			$"When potions reach 2 ticks left, they will stop counting down, giving you infinite potion duration.\n" +
			$"When you die, all potion effects that are being held at 2 ticks left will reset. (Until death do you part)";
		public override string Artist => "anodomani";
		public override string ArtModifiedBy => "andro951";
		public override string Designer => "andro951";

		#endregion
	}
}
