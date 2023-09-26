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
using androLib.UI;
using Terraria.Audio;

namespace VacuumBags.Items
{
	[Autoload(false)]
	public class TrashCan : BagModItem, ISoldByWitch {
		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
		public override void SetDefaults() {
			Item.maxStack = 1;
			Item.value = 100000;
			Item.rare = ItemRarityID.Blue;
			Item.width = 24;
			Item.height = 32;
		}
		public override void AddRecipes() {
			CreateRecipe()
			.AddTile(TileID.Anvils)
			.AddIngredient(ItemID.TrashCan, 1)
			.Register();

			Recipe.Create(ItemID.TrashCan)
			.AddTile(TileID.Anvils)
			.AddIngredient(Type, 1)
			.Register();
		}
		new public static SortedSet<int> Blacklist {
			get {
				if (blacklist == null) {
					blacklist = new() {
						ModContent.ItemType<BagBlack>(),
						ModContent.ItemType<PackBlack>(),
						ItemID.CopperCoin,
						ItemID.SilverCoin,
						ItemID.GoldCoin,
						ItemID.PlatinumCoin,
					};

					blacklist.UnionWith(StorageManager.GetPlayerBlackListSortedSet(BagStorageID));
				}

				return blacklist;
			}
		}
		private static SortedSet<int> blacklist = null;

		public static int BagStorageID;//Set this when registering with androLib.
		private const int TrashCanSize = 200;
		public static void RegisterWithAndroLib(Mod mod) {
			BagStorageID = StorageManager.RegisterVacuumStorageClass(
				mod,//Mod
				typeof(TrashCan),//type 
				ItemAllowedToBeStored,//Is allowed function, Func<Item, bool>
				null,//Localization Key name.  Attempts to determine automatically by treating the type as a ModItem, or you can specify.
				TrashCanSize,//StorageSize
				null,//Can vacuum
				() => new Color(100, 100, 116, androLib.Common.Configs.ConfigValues.UIAlpha),   // Get color function. Func<using Microsoft.Xna.Framework.Color>
				() => new Color(40, 40, 50, androLib.Common.Configs.ConfigValues.UIAlpha),   // Get Scroll bar color function. Func<using Microsoft.Xna.Framework.Color>
				() => new Color(140, 140, 162, androLib.Common.Configs.ConfigValues.UIAlpha), // Get Button hover color function. Func<using Microsoft.Xna.Framework.Color>
				() => ModContent.ItemType<TrashCan>(),//Get ModItem type
				80,//UI Left
				675,//UI Top
				() => Blacklist,
				true
			);

			StorageManager.AddBagUIEdit(BagStorageID, (BagUI bagUI) => {
				bagUI.AllButtonProperites.RemoveAt(bagUI.depositAllUIIndex);
				bagUI.AddButton(ClearTrash, () => StorageTextID.ClearTrash.ToString().Lang(AndroMod.ModName, L_ID1.StorageText));
			});
		}
		public static bool ItemAllowedToBeStored(Item item) => !Blacklist.Contains(item.type) && CanTrash(item);
		public static void ClearTrash(BagUI bagUI) {
			Item[] inv = bagUI.Storage.Items;
			bool trashedAny = false;
			for (int i = 0; i < inv.Length; i++) {
				ref Item item = ref inv[i];
				if (item.NullOrAir())
					continue;

				if (TryTrashItem(ref item))
					trashedAny = true;
			}

			if (trashedAny) {
				SoundEngine.PlaySound(SoundID.Coins);
				Recipe.FindRecipes(true);
			}
		}
		public static readonly Item[] trashInventory = new Item[0];
		private static bool CanTrash(Item item) => !item.favorited && PlayerLoader.CanSellItem(Main.LocalPlayer, null, trashInventory, item);
		private static bool TryTrashItem(ref Item item) {
			if (!CanTrash(item))
				return false;

			TrashItem(ref item, Main.LocalPlayer);

			return true;
		}
		private static void TrashItem(ref Item item, Player player) {
			player.SellItem(item);

			PlayerLoader.PostSellItem(player, null, trashInventory, item);
			item.TurnToAir();
			SoundEngine.PlaySound(SoundID.CoinPickup);
			Recipe.FindRecipes(true);
		}
		private static bool[] itemTracker = new bool[TrashCanSize];
		private static SortedSet<int> itemsAlreadyFound = new();
		public static void TrashCheck() {
			if (Main.netMode == NetmodeID.Server)
				return;

			if (!StorageManager.HasRequiredItemToUseStorageFromBagTypeSlow(Main.LocalPlayer, ModContent.ItemType<TrashCan>()))
				return;

			Item[] inv = StorageManager.GetItems(BagStorageID);
			bool doTrash = false;
			for (int i = 0; i < inv.Length; i++) {
				Item item = inv[i];
				if (item.NullOrAir()) {
					if (itemTracker[i]) {
						itemTracker[i] = false;
						doTrash = true;
					}
				}
				else {
					if (!itemTracker[i]) {
						itemTracker[i] = true;
						if (!itemsAlreadyFound.Add(item.type))
							doTrash = true;
					}
				}
			}

			if (doTrash) {
				itemsAlreadyFound.Clear();
				for (int i = 0; i < inv.Length; i++) {
					ref Item item = ref inv[i];
					if (item.NullOrAir())
						continue;

					if (!itemsAlreadyFound.Add(item.type)) {
						if (TryTrashItem(ref item))
							itemTracker[i] = false;
					}
				}
			}
		}

		#region AndroModItem attributes that you don't need.

		public virtual SellCondition SellCondition => SellCondition.Never;
		public virtual float SellPriceModifier => 1f;
		public override List<WikiTypeID> WikiItemTypes => new() { WikiTypeID.Storage };
		public override string LocalizationTooltip =>
			$"Used to automatically sell items.\n" +
			$"When in your inventory, the contents of the bag are available for crafting.\n" +
			$"Right click to open the bag.\n" +
			$"The trash can acts like other bags but with a few exceptions:\n" +
			$"It will only hold 1 item of each type.\n" +
			$"If an item is added to the trash can when there is already one of the same type in the can, the new one will be sold.\n" +
			$"The Clear Trash button will sell all items in the trash can that are not favorited.";
		public override string Artist => "@kingjoshington";
		public override string Designer => "andro951";

		#endregion
	}
}
