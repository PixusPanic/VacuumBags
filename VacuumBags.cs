using androLib;
using androLib.Common.Utility;
using androLib.Localization;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using VacuumBags.Common.Configs;
using VacuumBags.Items;
using VacuumBags.Localization;

namespace VacuumBags
{
	public class VacuumBags : Mod
	{
		public static string ModName = ModContent.GetInstance<VacuumBags>().Name;
		public static BagsServerConfig serverConfig = ModContent.GetInstance<BagsServerConfig>();
		public static BagsClientConfig clientConfig = ModContent.GetInstance<BagsClientConfig>();
		public static Mod GadgetGalore;
		public static bool gadgetGaloreEnabled = ModLoader.TryGetMod("GadgetGalore", out GadgetGalore);
		public static bool registeredWithAndroLib = false;

		//List<Hook> hooks = new();
		public override void Load() {
			AddAllContent(this);

			//foreach (Hook hook in hooks) {
			//	hook.Apply();
			//}

			if (!AndroMod.weaponEnchantmentsLoaded)
				RegisterAllBagsWithAndroLib();

			On_Player.ChooseAmmo += AmmoBag.OnChooseAmmo;
			On_Player.Fishing_GetBait += FishingBelt.OnFishing_GetBait;
			On_Player.FindPaintOrCoating += PaintBucket.OnFindPaintOrCoating;
			On_Player.QuickBuff_PickBestFoodItem += PotionFlask.OnQuickBuff_PickBestFoodItem;
			On_Player.QuickBuff += PotionFlask.OnQuickBuff;
			On_Player.QuickHeal_GetItemToUse += PotionFlask.OnQuickHeal_GetItemToUse;
			On_Player.QuickMana_GetItemToUse += PotionFlask.OnQuickMana_GetItemToUse;
			On_SceneMetrics.ScanAndExportToMain += Items.BannerBag.OnScanAndExportToMain;
			On_ItemSlot.RightClick_ItemArray_int_int += OnRightClick_ItemArray_int_int;
			On_Player.AdjTiles += PortableStation.OnAdjTiles;

			On_Player.ItemCheck_Inner += BagPlayer.OnItemCheck_Inner;
			On_Main.DrawInterface_40_InteractItemIcon += BagPlayer.On_Main_DrawInterface_40_InteractItemIcon;
			On_PlayerDrawLayers.DrawPlayer_27_HeldItem += BagPlayer.On_PlayerDrawLayers_DrawPlayer_27_HeldItem;
			On_SmartCursorHelper.SmartCursorLookup += BagPlayer.On_SmartCursorHelper_SmartCursorLookup;

			IL_ItemSlot.Draw_SpriteBatch_ItemArray_int_int_Vector2_Color += OnDrawItemSlot;
			IL_Player.ItemCheck_CheckCanUse += PaintBucket.OnItemCheck_CheckCanUse;
			IL_Player.ItemCheck_UseWiringTools += MechanicsToolbelt.OnItemCheck_UseWiringTools;
			IL_Player.ItemCheck_ApplyHoldStyle_Inner += AmmoBag.OnItemCheck_ApplyHoldStyle_Inner;
			IL_Player.SmartSelect_PickToolForStrategy += AmmoBag.OnSmartSelect_PickToolForStrategy;

			VacuumBagsLocalizationData.RegisterSDataPackage();

			BuildersBox.RegisterWithGadgetGalore();
			WallEr.RegisterWithGadgetGalore();
			JarOfDirt.RegisterWithGadgetGalore();
			PaintBucket.RegisterWithGadgetGalore();
		}
		private void AddAllContent(VacuumBags mod) {
			IEnumerable<Type> types = null;
			try {
				types = Assembly.GetExecutingAssembly().GetTypes();
			}
			catch (ReflectionTypeLoadException e) {
				types = e.Types.Where(t => t != null);
			}

			types = types.Where(t => !t.IsAbstract && (t.IsSubclassOf(typeof(BagModItem))));

			IEnumerable<ModItem> allItems = types.Select(t => Activator.CreateInstance(t)).Where(i => i != null).OfType<ModItem>().Append(new VacuumOreBag.Items.OreBag())
				.GroupBy(i => i.GetType().BaseType == typeof(BagModItem) || i is VacuumOreBag.Items.OreBag ? 0 : i.GetType().BaseType == typeof(ModBag) ? 1 : i.GetType().BaseType == typeof(SimpleBag) ? 2 : 3)
				.Select(g => g.ToList().OrderBy(i => i.Name))
				.SelectMany(i => i);

			foreach (ModItem modItem in allItems) {
				mod.AddContent(modItem);
			}
		}
		public void RegisterAllBagsWithAndroLib() {
			if (registeredWithAndroLib)
				return;

			registeredWithAndroLib = true;

			BagBlack.RegisterWithAndroLib(this);
			BagBlue.RegisterWithAndroLib(this);
			BagBrown.RegisterWithAndroLib(this);
			BagGray.RegisterWithAndroLib(this);
			BagGreen.RegisterWithAndroLib(this);
			BagOrange.RegisterWithAndroLib(this);
			BagPink.RegisterWithAndroLib(this);
			BagPurple.RegisterWithAndroLib(this);
			BagRed.RegisterWithAndroLib(this);
			BagWhite.RegisterWithAndroLib(this);
			BagYellow.RegisterWithAndroLib(this);

			CalamitousCauldron.RegisterWithAndroLib(this);
			LokisTesseract.RegisterWithAndroLib(this);
			EssenceOfGathering.RegisterWithAndroLib(this);
			FargosMementos.RegisterWithAndroLib(this);

			BannerBag.RegisterWithAndroLib(this);
			FishingBelt.RegisterWithAndroLib(this);
			PortableStation.RegisterWithAndroLib(this);
			BossBag.RegisterWithAndroLib(this);
			PaintBucket.RegisterWithAndroLib(this);
			PotionFlask.RegisterWithAndroLib(this);
			HerbSatchel.RegisterWithAndroLib(this);
			MechanicsToolbelt.RegisterWithAndroLib(this);
			AmmoBag.RegisterWithAndroLib(this);
			JarOfDirt.RegisterWithAndroLib(this);
			BuildersBox.RegisterWithAndroLib(this);
			WallEr.RegisterWithAndroLib(this);
			SlayersSack.RegisterWithAndroLib(this);

			PackBlack.RegisterWithAndroLibItemTypeOnly();
			PackBlue.RegisterWithAndroLibItemTypeOnly();
			PackBrown.RegisterWithAndroLibItemTypeOnly();
			PackGray.RegisterWithAndroLibItemTypeOnly();
			PackGreen.RegisterWithAndroLibItemTypeOnly();
			PackOrange.RegisterWithAndroLibItemTypeOnly();
			PackPink.RegisterWithAndroLibItemTypeOnly();
			PackPurple.RegisterWithAndroLibItemTypeOnly();
			PackRed.RegisterWithAndroLibItemTypeOnly();
			PackWhite.RegisterWithAndroLibItemTypeOnly();
			PackYellow.RegisterWithAndroLibItemTypeOnly();
		}
		public override object Call(params object[] args) {
			if (args.Length != 1)
				return null;

			if (args[0] is not string s)
				return null;

			if (s != "RegisterAllBagsWithAndroLib")
				return null;

			RegisterAllBagsWithAndroLib();
			return null;
		}
		private void OnRightClick_ItemArray_int_int(On_ItemSlot.orig_RightClick_ItemArray_int_int orig, Item[] inv, int context, int slot) {
			int clickedItemType = inv[slot].type;
			if (clickedItemType == ModContent.ItemType<AmmoBag>()
				|| clickedItemType == ModContent.ItemType<PaintBucket>()
				|| clickedItemType == ModContent.ItemType<FishingBelt>()
				|| clickedItemType == ModContent.ItemType<MechanicsToolbelt>()
			) {
				orig(inv, 0, slot);
			}
			else {
				orig(inv, context, slot);
			}
		}
		internal static void OnDrawItemSlot(ILContext il) {
			var c = new ILCursor(il);
			/*
	// if (item.useAmmo > 0)
	IL_0cc8: ldloc.1
	IL_0cc9: ldfld int32 Terraria.Item::useAmmo
	IL_0cce: ldc.i4.0
	IL_0ccf: ble.s IL_0d13

	// _ = item.useAmmo;
	IL_0cd1: ldloc.1
	IL_0cd2: ldfld int32 Terraria.Item::useAmmo
	IL_0cd7: pop
	// num11 = 0;
	IL_0cd8: ldc.i4.0
	IL_0cd9: stloc.s 29
			 */
			//Note to self.  All instructions have to be perfectly in order if using TryGotoNext.  Use multiple calls of TryGotoNext if you need to skip instructions.
			if (!c.TryGotoNext(MoveType.After,
				i => i.MatchLdloc(1),
				i => i.MatchLdfld<Item>("useAmmo"),
				i => i.MatchPop(),
				i => i.MatchLdcI4(0)
			)) { throw new Exception("Failed to find instructions OnDrawItemSlot 1/4"); }

			//Also works for jumping over instructions.
			/*if (!c.TryGotoNext(MoveType.After,
				//i => i.MatchLdloc(1),
				i => i.MatchLdfld<Item>("useAmmo"),
				i => i.MatchLdcI4(0)
			)) { throw new Exception("Failed to find instructions OnDrawItemSlot 1/2"); }

			if (!c.TryGotoNext(MoveType.After,
				i => i.MatchLdcI4(0)
			)) { throw new Exception("Failed to find instructions OnDrawItemSlot 2/2"); }*/
			//c.LogRest(10);
			c.Emit(OpCodes.Ldloc, 1);

			c.EmitDelegate((int ammoCount, Item weapon) => {
				int ammoBagItemType = ModContent.ItemType<AmmoBag>();
				if (!StorageManager.HasRequiredItemToUseStorageFromBagType(Main.LocalPlayer, ammoBagItemType, out _))
					return ammoCount;

				foreach (Item item in StorageManager.GetItems(AmmoBag.BagStorageID)) {
					if (!item.NullOrAir() && item.stack > 0 && ItemLoader.CanChooseAmmo(weapon, item, Main.LocalPlayer))
						ammoCount += item.stack;
				}

				return ammoCount;
			});

			// if (item.fishingPole > 0)
			//IL_0d13: ldloc.1
			//IL_0d14: ldfld int32 Terraria.Item::fishingPole
			//IL_0d19: ldc.i4.0
			//IL_0d1a: ble.s IL_0d4a

			// num11 = 0;
			//IL_0d1c: ldc.i4.0
			//IL_0d1d: stloc.s 29

			if (!c.TryGotoNext(MoveType.After,
				i => i.MatchLdloc(1),
				i => i.MatchLdfld<Item>("fishingPole"),
				i => i.MatchLdcI4(0),
				i => i.MatchBle(out _),
				i => i.MatchLdcI4(0)
			)) { throw new Exception("Failed to find instructions OnDrawItemSlot 2/4"); }

			c.EmitDelegate((int baitCount) => {
				int fishingBeltItemType = ModContent.ItemType<FishingBelt>();
				if (!StorageManager.HasRequiredItemToUseStorageFromBagType(Main.LocalPlayer, fishingBeltItemType, out _))
					return baitCount;

				foreach (Item item in StorageManager.GetItems(FishingBelt.BagStorageID)) {
					if (!item.NullOrAir() && item.bait > 0)
						baitCount += item.stack;
				}

				return baitCount;
			});

			// if (item.tileWand > 0)
			//	IL_0d4a: ldloc.1
			//IL_0d4b: ldfld int32 Terraria.Item::tileWand
			//IL_0d50: ldc.i4.0
			//IL_0d51: ble.s IL_0d8a

			// int tileWand = item.tileWand;
			//IL_0d53: ldloc.1
			//IL_0d54: ldfld int32 Terraria.Item::tileWand
			//IL_0d59: stloc.s 44
			// num11 = 0;
			//IL_0d5b: ldc.i4.0
			//IL_0d5c: stloc.s 29

			if (!c.TryGotoNext(MoveType.After,
				i => i.MatchLdloc(1),
				i => i.MatchLdfld<Item>("tileWand"),
				i => i.MatchStloc(44),
				i => i.MatchLdcI4(0)
			)) { throw new Exception("Failed to find instructions OnDrawItemSlot 3/4"); }

			c.Emit(OpCodes.Ldloc, 44);

			c.EmitDelegate((int wandAmmo, int tileWand) => {
				//int mechanicsToolbeltItemType = ModContent.ItemType<MechanicsToolbelt>();
				//if (!StorageManager.HasRequiredItemToUseStorageFromBagType(Main.LocalPlayer, mechanicsToolbeltItemType, out _))
				//	return wandAmmo;

				//foreach (Item item in StorageManager.GetItems(MechanicsToolbelt.BagStorageID)) {
				//	if (!item.NullOrAir() && item.ammo == tileWand)
				//		wandAmmo += item.stack;
				//}

				return wandAmmo;
			});

			// num11 = 0;
			//	IL_0dd8: ldc.i4.0
			//IL_0dd9: stloc.s 29
			//// for (int m = 0; m < 58; m++)
			//IL_0ddb: ldc.i4.0
			//IL_0ddc: stloc.s 46
			//// if (inv[m].type == 530)
			//IL_0dde: br.s IL_0e04

			if (!c.TryGotoNext(MoveType.Before,
				i => i.MatchStloc(29),
				i => i.MatchLdcI4(0),
				i => i.MatchStloc(46)
			)) { throw new Exception("Failed to find instructions OnDrawItemSlot 4/4"); }

			c.EmitDelegate((int wireCount) => {
				int mechanicsToolbeltItemType = ModContent.ItemType<MechanicsToolbelt>();
				if (!StorageManager.HasRequiredItemToUseStorageFromBagType(Main.LocalPlayer, mechanicsToolbeltItemType, out _))
					return wireCount;

				foreach (Item item in StorageManager.GetItems(MechanicsToolbelt.BagStorageID)) {
					if (!item.NullOrAir() && item.type == ItemID.Wire)
						wireCount += item.stack;
				}

				return wireCount;
			});
		}
	}
}