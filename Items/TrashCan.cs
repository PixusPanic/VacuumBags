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
using VacuumBags.Common.Configs;
using static androLib.Items.IBagModItem;

namespace VacuumBags.Items
{
	[Autoload(false)]
	public class TrashCan : BagModItem_VB {
		public static IBagModItem Instance {
			get {
				if (instance == null)
					instance = new TrashCan();

				return instance;
			}
		}
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<BagToggle>().TrashCan;
		}
		
		private static IBagModItem instance;
		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
		public override void Load() {
			StoragePlayer.OnAndroLibClientConfigChangedInGame += () => {
				blacklist = null;
			};
		}
		public override void SetDefaults() {
			Item.maxStack = 1;
			Item.value = 100000;
			Item.rare = ItemRarityID.Blue;
			Item.width = 24;
			Item.height = 32;
		}
		public override int GetBagType() => ModContent.ItemType<TrashCan>();
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
		public SortedSet<int> Blacklist {
			get {
				if (blacklist == null) {
					blacklist = new() {
						ModContent.ItemType<TrashCan>(),
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

		public override void UpdateAllowedList(int item, bool add) {
			if (add) {
				Blacklist.Remove(item);
			}
			else {
				Blacklist.Add(item);
			}
		}

		public override int DefaultBagSize => 200;
		public override Color PanelColor => new(100, 100, 116, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ScrollBarColor => new(40, 40, 50, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ButtonHoverColor => new(140, 140, 162, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override bool? CanVacuum => null;
		public override bool BlackListOnly => true;
		public override void RegisterWithAndroLib(Mod mod) {
			base.RegisterWithAndroLib(mod);

			StorageManager.AddBagUIEdit(BagStorageID, (BagUI bagUI) => {
				bagUI.MyButtonProperties.RemoveAt(bagUI.depositAllUIIndex);
				bagUI.AddButton(ClearTrash, () => StorageTextID.ClearTrash.ToString().Lang(AndroMod.ModName, L_ID1.StorageText));
			});
		}
		public override bool ItemAllowedToBeStored(Item item) => !Blacklist.Contains(item.type) && CanTrash(item);
		public override Func<Item, bool> CanVacuumItemFunc => (item) => {
			return ((IBagModItem)this).BagContainsItem(item);
		};
		public static void ClearTrash(BagUI bagUI) {
			Item[] inv = bagUI.MyStorage.Items;
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
		private static bool[] itemTracker = null;
		private static SortedSet<int> itemsAlreadyFound = new();
		public static void TrashCheck() {
			if (Main.netMode == NetmodeID.Server)
				return;

			if (!StorageManager.HasRequiredItemToUseStorageFromBagTypeSlow(Main.LocalPlayer, ModContent.ItemType<TrashCan>()))
				return;

			Item[] inv = StorageManager.GetItems(Instance.BagStorageID);
			if (itemTracker == null)
				itemTracker = new bool[inv.Length];

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
