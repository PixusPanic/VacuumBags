using androLib;
using androLib.Common.Utility;
using androLib.Items;
using androLib.Localization;
using androLib.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Intrinsics.X86;
using log4net;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using VacuumBags.Common.Configs;
using VacuumBags.Common.Globals;
using VacuumBags.Items;
using VacuumBags.Localization;
using VacuumBags.ModIntegration.RegisteringStorages;

namespace VacuumBags
{
	public class VacuumBags : Mod
	{
		public const string ModName = "VacuumBags";
		public static BagsServerConfig serverConfig = ModContent.GetInstance<BagsServerConfig>();
		public static BagsClientConfig clientConfig = ModContent.GetInstance<BagsClientConfig>();
		public static bool registeredWithAndroLib = false;

		public static VacuumBags Instance;

		List<Hook> hooks = new();

		public override void Load()
		{
			AddAllContent(this);

			if (AndroMod.ammoToolModEnabled)
			{
				hooks.Add(new(ModLoaderGlobalItemPostDrawInInventory, GlobalItemPostDrawInInventoryDetour));
				hooks.Add(new(ModLoaderGlobalItemCanBeChosenAsAmmo, GlobalItemCanBeChosenAsAmmoDetour));
				hooks.Add(new(ModLoaderGlobalItemPreDrawTooltipLine, GlobalItemPreDrawTooltipLineDetour));
			}

			foreach (Hook hook in hooks)
			{
				hook.Apply();
			}

			if (ModContent.GetInstance<BagToggle>().AmmoBag)
			{
				On_Player.ChooseAmmo += AmmoBag.OnChooseAmmo;
				IL_Player.ItemCheck_ApplyHoldStyle_Inner += AmmoBag.OnItemCheck_ApplyHoldStyle_Inner;
				IL_Player.SmartSelect_PickToolForStrategy += AmmoBag.OnSmartSelect_PickToolForStrategy;

				if (AndroMod.ammoToolModEnabled)
				{
					On_ChatManager
							.DrawColorCodedStringWithShadow_SpriteBatch_DynamicSpriteFont_string_Vector2_Color_float_Vector2_Vector2_float_float +=
						On_ChatManager_DrawColorCodedStringWithShadow_SpriteBatch_DynamicSpriteFont_string_Vector2_Color_float_Vector2_Vector2_float_float;
					On_Player.HasItem_int += On_Player_HasItem_int;
				}
			}

			if (ModContent.GetInstance<BagToggle>().FishingBelt)
			{
				On_Player.Fishing_GetBait += FishingBelt.OnFishing_GetBait;
				IL_Player.ItemCheck_CheckFishingBobber_PickAndConsumeBait +=
					FishingBelt.OnItemCheck_CheckFishingBobber_PickAndConsumeBait;
				IL_Projectile.AI_061_FishingBobber_GiveItemToPlayer +=
					FishingBelt.On_AI_061_FishingBobber_GiveItemToPlayer;
				IL_Player.GetAnglerReward += FishingBelt.OnGetAnglerReward;
				IL_Main.GUIChatDrawInner += FishingBelt.OnGUIChatDrawInner;
			}

			if (ModContent.GetInstance<BagToggle>().PaintBucket)
				On_Player.FindPaintOrCoating += PaintBucket.OnFindPaintOrCoating;
			if (ModContent.GetInstance<BagToggle>().PotionFlask)
			{
				On_Player.QuickBuff_PickBestFoodItem += PotionFlask.OnQuickBuff_PickBestFoodItem;
				On_Player.QuickBuff += PotionFlask.OnQuickBuff;
				On_Player.QuickHeal_GetItemToUse += PotionFlask.OnQuickHeal_GetItemToUse;
				On_Player.QuickMana_GetItemToUse += PotionFlask.OnQuickMana_GetItemToUse;
				On_Player.DelBuff += PotionFlask.OnDelBuff;
				On_Main.TryRemovingBuff += PotionFlask.OnTryRemovingBuff;
				On_Player.AddBuff += PotionFlask.OnAddBuff;
			}

			if (ModContent.GetInstance<BagToggle>().MechanicsToolbelt)
			{
				On_Player.PutItemInInventoryFromItemUsage +=
					MechanicsToolbelt.On_Player_PutItemInInventoryFromItemUsage;
				IL_Player.ItemCheck_UseWiringTools += MechanicsToolbelt.OnItemCheck_UseWiringTools;
				IL_Wiring.MassWireOperation += MechanicsToolbelt.OnMassWireOperation;
			}

			if (ModContent.GetInstance<BagToggle>().PortableStation)
			{
				IL_Player.AdjTiles += PortableStation.OnAdjTiles;
				AndroMod.ScenemetrictBeforeAnyCheck +=
					(sceneMetrics, settings) => PortableStation.PreScanAndExportToMain();
				AndroMod.ScenemetrictAfterTileCheck += (sceneMetrics, settings) =>
					PortableStation.PostScanAndExportToMain(ref sceneMetrics);
			}

			if (ModContent.GetInstance<BagToggle>().SlayersSack)
				IL_Player.TileInteractionsUse += SlayersSack.OnTileInteractionsUse;
			if (ModContent.GetInstance<BagToggle>().BannerBag)
			{
				AndroMod.ScenemetrictBeforeAnyCheck += (sceneMetrics, settings) => BannerBag.PreScanAndExportToMain();
				AndroMod.ScenemetrictAfterTileCheck += (sceneMetrics, settings) =>
					BannerBag.PostScanAndExportToMain(ref sceneMetrics);

			}

			On_Player.ItemCheck_Inner += BagPlayer.OnItemCheck_Inner;
			On_Main.DrawInterface_40_InteractItemIcon += BagPlayer.On_Main_DrawInterface_40_InteractItemIcon;
			On_PlayerDrawLayers.DrawPlayer_27_HeldItem += BagPlayer.On_PlayerDrawLayers_DrawPlayer_27_HeldItem;
			On_SmartCursorHelper.SmartCursorLookup += BagPlayer.On_SmartCursorHelper_SmartCursorLookup;

			On_Player.PlaceThing_Tiles_CheckWandUsability += On_Player_PlaceThing_Tiles_CheckWandUsability;
			On_Player.ConsumeItem += On_Player_ConsumeItem;
			IL_ItemSlot.Draw_SpriteBatch_ItemArray_int_int_Vector2_Color += OnDrawItemSlot;
			IL_Player.ItemCheck_CheckCanUse += OnItemCheck_CheckCanUse;

			IL_Player.ItemCheck_Inner += IL_Player_ItemCheck_Inner;

			AndroMod.ScenemetricsOnNearbyEffects.Add((num, sceneMetrics, settings) =>
				GlobalBagTile.NearbyEffects(num, ref sceneMetrics));

			if (!AndroMod.weaponEnchantmentsLoaded)
				RegisterAllBagsWithAndroLib();

			VacuumBagsLocalizationData.RegisterSDataPackage();
			StoragePlayer.OnAndroLibClientConfigChangedInGame += SimpleBag.ClearAllowedLists;
			TerrariaAutomationsIntegration.Load();
		}


		#region AmmoTool Integration

		private Vector2
			On_ChatManager_DrawColorCodedStringWithShadow_SpriteBatch_DynamicSpriteFont_string_Vector2_Color_float_Vector2_Vector2_float_float(
				On_ChatManager.
					orig_DrawColorCodedStringWithShadow_SpriteBatch_DynamicSpriteFont_string_Vector2_Color_float_Vector2_Vector2_float_float
					orig, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch,
				ReLogic.Graphics.DynamicSpriteFont font, string text, Vector2 position, Color baseColor, float rotation,
				Vector2 origin, Vector2 baseScale, float maxWidth, float spread)
		{
			if (baseColor == Color.SkyBlue)
			{
				Item weapon = Main.LocalPlayer.HeldItem;
				if (!weapon.NullOrAir() && weapon.useAmmo > AmmoID.None && int.TryParse(text, out int stack))
				{
					AmmoBag.AddAmmoCountFromAmmoBag(weapon, ref stack);
					text = $"{stack}";
				}
			}

			return orig(spriteBatch, font, text, position, baseColor, rotation, origin, baseScale, maxWidth, spread);
		}


		private static bool ammoToolCountingAmmo = false;

		private delegate void orig_ModLoaderGlobalItemPostDrawInInventory(Item item, SpriteBatch spriteBatch,
			Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale);

		private delegate void hook_ModLoaderGlobalItemPostDrawInInventory(
			orig_ModLoaderGlobalItemPostDrawInInventory orig, Item item, SpriteBatch spriteBatch, Vector2 position,
			Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale);

		private static readonly MethodInfo ModLoaderGlobalItemPostDrawInInventory =
			typeof(ItemLoader).GetMethod("PostDrawInInventory");

		private void GlobalItemPostDrawInInventoryDetour(orig_ModLoaderGlobalItemPostDrawInInventory orig, Item item,
			SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor,
			Vector2 origin, float scale)
		{
			ammoToolCountingAmmo = true;

			orig(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);

			ammoToolCountingAmmo = false;
		}


		private delegate bool orig_GlobalItemCanBeChosenAsAmmo(Item ammo, Item weapon, Player player);

		private delegate void hook_GlobalItemCanBeChosenAsAmmo(orig_GlobalItemCanBeChosenAsAmmo orig, Item ammo,
			Item weapon, Player player);

		private static readonly MethodInfo ModLoaderGlobalItemCanBeChosenAsAmmo =
			typeof(ItemLoader).GetMethod("CanChooseAmmo");

		private bool GlobalItemCanBeChosenAsAmmoDetour(orig_GlobalItemCanBeChosenAsAmmo orig, Item ammo, Item weapon,
			Player player)
		{
			ammoToolCountingAmmo = true;

			bool result = orig(ammo, weapon, player);

			ammoToolCountingAmmo = false;

			return result;
		}

		private delegate bool orig_GlobalItemPreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset);

		private delegate void hook_GlobalItemPreDrawTooltipLine(orig_GlobalItemPreDrawTooltipLine orig, Item item,
			DrawableTooltipLine line, ref int yOffset);

		private static readonly MethodInfo ModLoaderGlobalItemPreDrawTooltipLine =
			typeof(ItemLoader).GetMethod("PreDrawTooltipLine");

		private bool GlobalItemPreDrawTooltipLineDetour(orig_GlobalItemPreDrawTooltipLine orig, Item item,
			DrawableTooltipLine line, ref int yOffset)
		{
			ammoToolCountingAmmo = true;

			bool result = orig(item, line, ref yOffset);

			ammoToolCountingAmmo = false;

			return result;
		}


		private bool On_Player_HasItem_int(On_Player.orig_HasItem_int orig, Player self, int type)
		{
			bool result = orig(self, type);

			if (!result && ammoToolCountingAmmo)
			{
				Item ammoItem = type.CSI();
				if (ammoItem.ammo > AmmoID.None)
				{
					int ammoBagItemType = ModContent.ItemType<AmmoBag>();
					if (StorageManager.HasRequiredItemToUseStorageFromBagTypeSlow(Main.LocalPlayer, ammoBagItemType))
					{
						foreach (Item item in StorageManager.GetItems(AmmoBag.Instance.BagStorageID))
						{
							if (!item.NullOrAir() && item.stack > 0 && item.type == type)
							{
								result = true;

								break;
							}
						}
					}
				}
			}

			return result;
		}

		#endregion

		private void AddAllContent(VacuumBags mod)
		{
			IEnumerable<Type> types = null;
			try
			{
				types = Assembly.GetExecutingAssembly().GetTypes();
			}
			catch (ReflectionTypeLoadException e)
			{
				types = e.Types.Where(t => t != null);
			}

			types = types.Where(t => !t.IsAbstract && (t.InheritsFrom(typeof(IBagModItem))));

			IEnumerable<ModItem> allItems = types.Select(t => Activator.CreateInstance(t)).Where(i => i != null)
				.OfType<ModItem>().Append(new VacuumOreBag.Items.OreBag())
				.GroupBy(i =>
					i.GetType().BaseType == typeof(IBagModItem) || i is VacuumOreBag.Items.OreBag ? 0 :
					i.GetType().BaseType == typeof(ModBag) ? 1 :
					i.GetType().BaseType == typeof(SimpleBag) ? 2 : 3)
				.Select(g => g.ToList().OrderBy(i => i.Name))
				.SelectMany(i => i);

			foreach (ModItem modItem in allItems)
			{
				mod.AddContent(modItem);
			}
		}

		public void RegisterAllBagsWithAndroLib()
		{
			if (Main.netMode == NetmodeID.Server || registeredWithAndroLib) return;

			registeredWithAndroLib = true;

			// Simple Bags and Packs
			if (!clientConfig.SimpleBagsVacuumAllItems)
				RegisterSimpleBagsWithAndroLib();
			// Banner Bag
			if (ModContent.GetInstance<BagToggle>().BannerBag)
				BannerBag.Instance.RegisterWithAndroLib(this);
			// Fishing Belt
			if (ModContent.GetInstance<BagToggle>().FishingBelt)
				FishingBelt.Instance.RegisterWithAndroLib(this);
			// Portable Station
			if (ModContent.GetInstance<BagToggle>().PortableStation)
				PortableStation.Instance.RegisterWithAndroLib(this);
			// Paint Bucket
			if (ModContent.GetInstance<BagToggle>().PaintBucket)
			{
				PaintBucket.Instance.RegisterWithAndroLib(this);
				PaintBucket.Instance.RegisterWithGadgetGalore();
			}

			// Potion Flask
			if (ModContent.GetInstance<BagToggle>().PotionFlask)
			{
				PotionFlask.Instance.RegisterWithAndroLib(this);
				if (ModContent.GetInstance<BagToggle>().ExquisitePotionFlask)
					ExquisitePotionFlask.Instance.RegisterWithAndroLibItemTypeOnly();
			}

			// Herb Satchel
			if (ModContent.GetInstance<BagToggle>().HerbSatchel)
				HerbSatchel.Instance.RegisterWithAndroLib(this);
			// Mechanic's Toolbelt
			if (ModContent.GetInstance<BagToggle>().MechanicsToolbelt)
				MechanicsToolbelt.Instance.RegisterWithAndroLib(this);
			// Jar of Dirt
			if (ModContent.GetInstance<BagToggle>().JarOfDirt)
			{
				JarOfDirt.Instance.RegisterWithAndroLib(this);
				JarOfDirt.Instance.RegisterWithGadgetGalore();
			}

			// Ammo Bag
			if (ModContent.GetInstance<BagToggle>().AmmoBag)
				AmmoBag.Instance.RegisterWithAndroLib(this);
			// Boss Bag
			if (ModContent.GetInstance<BagToggle>().BossBag)
				BossBag.Instance.RegisterWithAndroLib(this);
			// Builder's Box
			if (ModContent.GetInstance<BagToggle>().BuildersBox)
			{
				BuildersBox.Instance.RegisterWithAndroLib(this);
				BuildersBox.Instance.RegisterWithGadgetGalore();
			}

			// Wall-Er
			if (ModContent.GetInstance<BagToggle>().WallEr)
			{
				WallEr.Instance.RegisterWithAndroLib(this);
				WallEr.Instance.RegisterWithGadgetGalore();
			}

			// Slayer's Sack
			if (ModContent.GetInstance<BagToggle>().SlayersSack)
				SlayersSack.Instance.RegisterWithAndroLib(this);
			// Trash Can
			if (ModContent.GetInstance<BagToggle>().TrashCan)
				TrashCan.Instance.RegisterWithAndroLib(this);
			// Mod bags
			if (ModContent.GetInstance<BagToggle>().ModBags)
			{
				// Calamity Mod - Calamitous Cauldron
				if (AndroMod.calamityEnabled || !ModContent.GetInstance<BagToggle>().RequireModsForModBags)
					CalamitousCauldron.Instance.RegisterWithAndroLib(this);
				// Thorium Mod - Loki's Tesseract
				if (AndroMod.thoriumEnabled || !ModContent.GetInstance<BagToggle>().RequireModsForModBags)
					LokisTesseract.Instance.RegisterWithAndroLib(this);
				// Stars Above - Essence of Gathering
				if (AndroMod.starsAboveEnabled || !ModContent.GetInstance<BagToggle>().RequireModsForModBags)
					EssenceOfGathering.Instance.RegisterWithAndroLib(this);
				// Fargo's Mutant/Souls Mod - Fargo's Mementos
				if (AndroMod.fargosEnabled || !ModContent.GetInstance<BagToggle>().RequireModsForModBags)
					FargosMementos.Instance.RegisterWithAndroLib(this);
				// Spooky Mod - Spooky Gourd
				if (AndroMod.spookyModEnabled || !ModContent.GetInstance<BagToggle>().RequireModsForModBags)
					SpookyGourd.Instance.RegisterWithAndroLib(this);
				// Secret of the Shadows - Earthen Pyramid
				if (AndroMod.secretsOfTheShadowsEnabled || !ModContent.GetInstance<BagToggle>().RequireModsForModBags)
					EarthenPyramid.Instance.RegisterWithAndroLib(this);
				// DBZ Terraria - Hoi Poi Capsule
				if (AndroMod.dbzTerrariaEnabled || !ModContent.GetInstance<BagToggle>().RequireModsForModBags)
					HoiPoiCapsule.Instance.RegisterWithAndroLib(this);
			}
		}

		private void RegisterSimpleBagsWithAndroLib()
		{
			if (!ModContent.GetInstance<BagToggle>().BagsAndPacks) return;
			BagBlack.Instance.RegisterWithAndroLib(this);
			BagBlue.Instance.RegisterWithAndroLib(this);
			BagBrown.Instance.RegisterWithAndroLib(this);
			BagGray.Instance.RegisterWithAndroLib(this);
			BagGreen.Instance.RegisterWithAndroLib(this);
			BagOrange.Instance.RegisterWithAndroLib(this);
			BagPink.Instance.RegisterWithAndroLib(this);
			BagPurple.Instance.RegisterWithAndroLib(this);
			BagRed.Instance.RegisterWithAndroLib(this);
			BagWhite.Instance.RegisterWithAndroLib(this);
			BagYellow.Instance.RegisterWithAndroLib(this);

			PackBlack.Instance.RegisterWithAndroLibItemTypeOnly();
			PackBlue.Instance.RegisterWithAndroLibItemTypeOnly();
			PackBrown.Instance.RegisterWithAndroLibItemTypeOnly();
			PackGray.Instance.RegisterWithAndroLibItemTypeOnly();
			PackGreen.Instance.RegisterWithAndroLibItemTypeOnly();
			PackOrange.Instance.RegisterWithAndroLibItemTypeOnly();
			PackPink.Instance.RegisterWithAndroLibItemTypeOnly();
			PackPurple.Instance.RegisterWithAndroLibItemTypeOnly();
			PackRed.Instance.RegisterWithAndroLibItemTypeOnly();
			PackWhite.Instance.RegisterWithAndroLibItemTypeOnly();
			PackYellow.Instance.RegisterWithAndroLibItemTypeOnly();
		}

		public override object Call(params object[] args)
		{
			if (args.Length != 1)
				return null;

			if (args[0] is not string s)
				return null;

			if (s != "RegisterAllBagsWithAndroLib")
				return null;

			RegisterAllBagsWithAndroLib();
			return null;
		}

		#region IL edits
		private static void OnDrawItemSlot(ILContext il)
		{
			var c = new ILCursor(il);

			//// if (item.useAmmo > 0)
			//IL_0cc8: ldloc.1
			//IL_0cc9: ldfld int32 Terraria.Item::useAmmo
			//IL_0cce: ldc.i4.0
			//IL_0ccf: ble.s IL_0d13

			//// _ = item.useAmmo;
			//IL_0cd1: ldloc.1
			//IL_0cd2: ldfld int32 Terraria.Item::useAmmo
			//IL_0cd7: pop
			//// num11 = 0;
			//IL_0cd8: ldc.i4.0
			//IL_0cd9: stloc.s 29

			//Note to self.  All instructions have to be perfectly in order if using TryGotoNext.  Use multiple calls of TryGotoNext if you need to skip instructions.

			int itemLoc = -1;

			if (ModContent.GetInstance<BagToggle>().AmmoBag)
			{
				if (!c.TryGotoNext(MoveType.After,
					    i => i.MatchLdloc(out itemLoc),
					    i => i.MatchLdfld<Item>("useAmmo"),
					    i => i.MatchPop(),
					    i => i.MatchLdcI4(0)
				    ))
				{
					if (!ModContent.GetInstance<BagsClientConfig>().ThrowExceptions)
						Instance.Logger.Debug(
							$"Could not successfully implement Ammo Bag logic! Failed to find instructions OnDrawItemSlot 1/4");
					else
						throw new Exception(
							"Could not successfully implement Ammo Bag logic! Failed to find instructions OnDrawItemSlot 1/4");
				}


				//Also works for jumping over instructions.
				/*if (!c.TryGotoNext(MoveType.After,
					//i => i.MatchLdloc(1),(
					i => i.MatchLdfld<Item>("useAmmo"),
					i => i.MatchLdcI4(0)
				)) { throw new Exception("Failed to find instructions OnDrawItemSlot 1/2"); }

				if (!c.TryGotoNext(MoveType.After,
					i => i.MatchLdcI4(0)
				)) { throw new Exception("Failed to find instructions OnDrawItemSlot 2/2"); }*/
				//c.LogRest(10);
				c.Emit(OpCodes.Ldloc, itemLoc);

				c.EmitDelegate((int ammoCount, Item weapon) =>
				{
					AmmoBag.AddAmmoCountFromAmmoBag(weapon, ref ammoCount);

					return ammoCount;
				});
			}

			// if (item.fishingPole > 0)
			//IL_0d13: ldloc.1
			//IL_0d14: ldfld int32 Terraria.Item::fishingPole
			//IL_0d19: ldc.i4.0
			//IL_0d1a: ble.s IL_0d4a

			// num11 = 0;
			//IL_0d1c: ldc.i4.0
			//IL_0d1d: stloc.s 29

			if (ModContent.GetInstance<BagToggle>().FishingBelt)
			{
				if (!c.TryGotoNext(MoveType.After,
					    i => i.MatchLdloc(itemLoc),
					    i => i.MatchLdfld<Item>("fishingPole"),
					    i => i.MatchLdcI4(0),
					    i => i.MatchBle(out _),
					    i => i.MatchLdcI4(0)
				    ))
				{
					if (!ModContent.GetInstance<BagsClientConfig>().ThrowExceptions)
						Instance.Logger.Debug(
							$"Could not successfully implement Fishing Belt logic! Failed to find instructions OnDrawItemSlot 2/4");
					else
						throw new Exception(
							"Could not successfully implement Fishing Belt logic! Failed to find instructions OnDrawItemSlot 2/4");
				}

				c.EmitDelegate((int baitCount) =>
				{
					int fishingBeltItemType = ModContent.ItemType<FishingBelt>();
					if (!StorageManager.HasRequiredItemToUseStorageFromBagTypeSlow(Main.LocalPlayer,
						    fishingBeltItemType, out _, out _, out _))
						return baitCount;

					foreach (Item item in StorageManager.GetItems(FishingBelt.Instance.BagStorageID))
					{
						if (!item.NullOrAir() && item.bait > 0)
							baitCount += item.stack;
					}

					return baitCount;
				});
			}


			//// if (item.tileWand > 0)
			//IL_0d4e: ldloc.1
			//IL_0d4f: ldfld int32 Terraria.Item::tileWand
			//IL_0d54: ldc.i4.0
			//IL_0d55: ble.s IL_0d8e

			//// int tileWand = item.tileWand;
			//IL_0d57: ldloc.1
			//IL_0d58: ldfld int32 Terraria.Item::tileWand
			//IL_0d5d: stloc.s 45
			//// num11 = 0;
			//IL_0d5f: ldc.i4.0
			//IL_0d60: stloc.s 30

			if (ModContent.GetInstance<BagToggle>().AmmoBag ||
			    ModContent.GetInstance<BagToggle>().JarOfDirt ||
			    ModContent.GetInstance<BagToggle>().BuildersBox)
			{
				int tileWandLoc = -1;

				if (!c.TryGotoNext(MoveType.After,
					    i => i.MatchLdloc(itemLoc),
					    i => i.MatchLdfld<Item>("tileWand"),
					    i => i.MatchStloc(out tileWandLoc),
					    i => i.MatchLdcI4(0)
				    ))
				{
					if (!ModContent.GetInstance<BagsClientConfig>().ThrowExceptions)
						Instance.Logger.Debug(
							$"Could not successfully implement Tile Wand logic Ammo Bag/Jar of Dirt/Builder's Box!! Failed to find Failed to find instructions OnDrawItemSlot 3/4");
					else
						throw new Exception(
							"Could not successfully implement Tile Wand logic for Ammo Bag/Jar of Dirt/Builder's Box! Failed to find instructions OnDrawItemSlot 3/4");
				}

				c.Emit(OpCodes.Ldloc, tileWandLoc);

				c.EmitDelegate((int wandAmmo, int tileWand) =>
				{
					Item wandAmmoItem = tileWand.CSI();
					List<(int, int)> list = new()
					{
						(ModContent.ItemType<AmmoBag>(), AmmoBag.Instance.BagStorageID),
						(ModContent.ItemType<JarOfDirt>(), JarOfDirt.Instance.BagStorageID),
						(ModContent.ItemType<BuildersBox>(), BuildersBox.Instance.BagStorageID),
					};

					foreach ((int bagType, int storageID) pair in list)
					{
						if (StorageManager.HasRequiredItemToUseStorageFromBagTypeSlow(Main.LocalPlayer, pair.bagType))
						{
							BagUI bagUI = StorageManager.BagUIs[pair.storageID];
							if (bagUI.CanBeStored(wandAmmoItem))
							{
								foreach (Item item in bagUI.Inventory)
								{
									if (!item.NullOrAir() && item.stack > 0 && item.type == tileWand)
										wandAmmo += item.stack;
								}
							}
						}
					}

					return wandAmmo;
				});
			}

			//// num11 = 0;
			//IL_0ddc: ldc.i4.0
			//IL_0ddd: stloc.s 30
			//// for (int m = 0; m < 58; m++)
			//IL_0ddf: ldc.i4.0
			//IL_0de0: stloc.s 47
			//// if (inv[m].type == 530)
			//IL_0de2: br.s IL_0e08
			//// loop start (head: IL_0e08)

			if (ModContent.GetInstance<BagToggle>().MechanicsToolbelt)
			{
				int num11Loc = -1;
				int lLoc = -1;

				if (!c.TryGotoNext(MoveType.Before,
					    i => i.MatchStloc(out num11Loc),
					    i => i.MatchLdcI4(0),
					    i => i.MatchStloc(out lLoc)
				    ))
				{
					if (!ModContent.GetInstance<BagsClientConfig>().ThrowExceptions)
						Instance.Logger.Debug(
							$"Could not successfully implement Mechanic's Toolbelt logic! Failed to find Failed to find instructions OnDrawItemSlot 4/4");
					else
						throw new Exception(
							"Could not successfully implement Mechanic's Toolbelt logic! Failed to find instructions OnDrawItemSlot 4/4");
				}

				c.EmitDelegate((int wireCount) =>
				{
					int mechanicsToolbeltItemType = ModContent.ItemType<MechanicsToolbelt>();
					if (!StorageManager.HasRequiredItemToUseStorageFromBagTypeSlow(Main.LocalPlayer,
						    mechanicsToolbeltItemType, out _, out _, out _))
						return wireCount;

					foreach (Item item in StorageManager.GetItems(MechanicsToolbelt.Instance.BagStorageID))
					{
						if (!item.NullOrAir() && item.type == ItemID.Wire)
							wireCount += item.stack;
					}

					return wireCount;
				});
			}
		}

		private bool On_Player_PlaceThing_Tiles_CheckWandUsability(
			On_Player.orig_PlaceThing_Tiles_CheckWandUsability orig, Player self, bool canUse)
		{
			canUse = orig(self, canUse);
			int tileWand = self.HeldItem.tileWand;
			if (canUse || tileWand <= 0)
				return canUse;

			Item wandAmmoItem = tileWand.CSI();
			List<(int, int)> list = new()
			{
				(ModContent.ItemType<AmmoBag>(), AmmoBag.Instance.BagStorageID),
				(ModContent.ItemType<JarOfDirt>(), JarOfDirt.Instance.BagStorageID),
				(ModContent.ItemType<BuildersBox>(), BuildersBox.Instance.BagStorageID),
			};

			foreach ((int bagType, int storageID) pair in list)
			{
				if (StorageManager.HasRequiredItemToUseStorageFromBagTypeSlow(Main.LocalPlayer, pair.bagType))
				{
					BagUI bagUI = StorageManager.BagUIs[pair.storageID];
					if (bagUI.CanBeStored(wandAmmoItem))
					{
						foreach (Item item in bagUI.Inventory)
						{
							if (!item.NullOrAir() && item.stack > 0 && item.type == tileWand)
							{
								canUse = true;
								break;
							}
						}
					}
				}

				if (canUse)
					break;
			}

			return canUse;
		}

		private void IL_Player_ItemCheck_Inner(ILContext il)
		{
			var c = new ILCursor(il);

			//IL_16af: ldloc.1
			//IL_16b0: ldfld int32 Terraria.Item::tileWand
			//IL_16b5: stloc.s 48

			if (!c.TryGotoNext(MoveType.After,
				    i => i.MatchLdloc(1),
				    i => i.MatchLdfld<Item>("tileWand"),
				    i => i.MatchStloc(48)
			    ))
			{
				if (!ModContent.GetInstance<BagsClientConfig>().ThrowExceptions)
					Instance.Logger.Debug(
						$"Could not successfully implement Tile Wand logic! Failed to find instructions IL_Player_ItemCheck_Inner 1/2");
				else
					throw new Exception(
						"Could not successfully implement Tile Wand logic! Failed to find instructions IL_Player_ItemCheck_Inner 1/2");
			}

			c.Emit(OpCodes.Ldarg_0);
			c.Emit(OpCodes.Ldloc, 48);

			c.EmitDelegate((Player player, int tileWand) =>
			{
				bool found = false;
				Item[] inventory = player.inventory;
				for (int num15 = 0; num15 < 58; num15++)
				{
					if (tileWand == inventory[num15].type && inventory[num15].stack > 0)
					{
						found = true;
						break;
					}
				}

				if (found)
					return;

				Item wandAmmoItem = tileWand.CSI();
				List<(int, int)> list = new()
				{
					(ModContent.ItemType<AmmoBag>(), AmmoBag.Instance.BagStorageID),
					(ModContent.ItemType<JarOfDirt>(), JarOfDirt.Instance.BagStorageID),
					(ModContent.ItemType<BuildersBox>(), BuildersBox.Instance.BagStorageID),
				};

				foreach ((int bagType, int storageID) pair in list)
				{
					if (StorageManager.HasRequiredItemToUseStorageFromBagTypeSlow(Main.LocalPlayer, pair.bagType))
					{
						BagUI bagUI = StorageManager.BagUIs[pair.storageID];
						if (bagUI.CanBeStored(wandAmmoItem))
						{
							foreach (Item item in bagUI.Inventory)
							{
								if (!item.NullOrAir() && item.stack > 0 && item.type == tileWand)
								{
									if (ItemLoader.ConsumeItem(item, player))
										item.stack--;

									if (item.stack <= 0)
										item.TurnToAir();

									break;
								}
							}
						}
					}
				}
			});
		}

		internal static void OnItemCheck_CheckCanUse(ILContext il)
		{
			var c = new ILCursor(il);

			// if (sItem.type == 1071 || sItem.type == 1072)
			//IL_02ae: ldarg.1
			//IL_02af: ldfld int32 Terraria.Item::'type'
			//IL_02b4: ldc.i4 1071
			//IL_02b9: beq.s IL_02c8

			//IL_02bb: ldarg.1
			//IL_02bc: ldfld int32 Terraria.Item::'type'
			//IL_02c1: ldc.i4 1072
			//IL_02c6: bne.un.s IL_02f7

			//// bool flag2 = false;
			//IL_02c8: ldc.i4.0
			//IL_02c9: stloc.s 9
			//// for (int i = 0; i < 58; i++)
			//IL_02cb: ldc.i4.0
			//IL_02cc: stloc.s 10
			//// if (this.inventory[i].PaintOrCoating)
			//IL_02ce: br.s IL_02eb

			if (!c.TryGotoNext(MoveType.Before,
				    i => i.MatchLdcI4(0),
				    i => i.MatchStloc(9)
			    ))
			{
				if (!ModContent.GetInstance<BagsClientConfig>().ThrowExceptions)
					Instance.Logger.Debug(
						$"Could not successfully implement Paint Bucket logic! Failed to find instructions PaintBucket.OnItemCheck_CheckCanUse() 1/2");
				else
					throw new Exception(
						"Could not successfully implement Paint Bucket logic! Failed to find instructions PaintBucket.OnItemCheck_CheckCanUse() 1/2");
			}

			c.Index++;

			c.EmitDelegate((bool hasPaint) =>
			{
				int PaintBucketID = ModContent.ItemType<PaintBucket>();
				if (!Main.LocalPlayer.HasItem(PaintBucketID))
					return hasPaint;

				foreach (Item item in StorageManager.GetItems(PaintBucket.Instance.BagStorageID))
				{
					if (!item.NullOrAir() && item.stack > 0 && item.PaintOrCoating)
						return true;
				}

				return hasPaint;
			});

			//IL_034c: ldarg.1
			//IL_034d: ldfld int32 Terraria.Item::shoot

			if (!c.TryGotoNext(MoveType.After,
				    i => i.MatchLdarg(1),
				    i => i.MatchLdfld<Item>("shoot")
			    ))
			{
				if (!ModContent.GetInstance<BagsClientConfig>().ThrowExceptions)
					Instance.Logger.Debug(
						$"Could not successfully implement Paint Bucket logic! Failed to find instructions PaintBucket.OnItemCheck_CheckCanUse() 2/2");
				else
					throw new Exception(
						"Could not successfully implement Paint Bucket logic! Failed to find instructions PaintBucket.OnItemCheck_CheckCanUse() 2/2");
			}

			c.Emit(OpCodes.Ldloca, 1);
			c.Emit(OpCodes.Ldarg_1);

			c.EmitDelegate((ref bool canUse, Item item) =>
			{
				if (item.tileWand <= 0)
					return;

				List<(int, int)> list = new()
				{
					(ModContent.ItemType<AmmoBag>(), AmmoBag.Instance.BagStorageID),
					(ModContent.ItemType<JarOfDirt>(), JarOfDirt.Instance.BagStorageID),
					(ModContent.ItemType<BuildersBox>(), BuildersBox.Instance.BagStorageID),
				};

				foreach ((int bagType, int storageID) pair in list)
				{
					if (StorageManager.HasRequiredItemToUseStorageFromBagTypeSlow(Main.LocalPlayer, pair.bagType))
					{
						BagUI bagUI = StorageManager.BagUIs[pair.storageID];
						if (bagUI.CanBeStored(item.tileWand.CSI()))
						{
							foreach (Item item2 in bagUI.Inventory)
							{
								if (!item2.NullOrAir() && item2.stack > 0 && item2.type == item.tileWand)
								{
									canUse = true;
									break;
								}
							}
						}
					}
				}
			});
		}

		private bool On_Player_ConsumeItem(On_Player.orig_ConsumeItem orig, Player self, int type, bool reverseOrder,
			bool includeVoidBag)
		{
			bool foundItemToConsume = orig(self, type, reverseOrder, includeVoidBag);
			if (foundItemToConsume)
				return foundItemToConsume;

			switch (type)
			{
				case ItemID.Wire:
				case ItemID.Actuator:
					int mechanicsToolbeltItemType = ModContent.ItemType<MechanicsToolbelt>();
					if (!StorageManager.HasRequiredItemToUseStorageFromBagTypeSlow(Main.LocalPlayer,
						    mechanicsToolbeltItemType))
						return foundItemToConsume;

					Item itemToConsume = MechanicsToolbelt.ChooseFromBelt(self, type);
					if (itemToConsume == null)
						return foundItemToConsume;

					if (ItemLoader.ConsumeItem(itemToConsume, self))
						itemToConsume.stack--;

					if (itemToConsume.stack <= 0)
						itemToConsume.SetDefaults();

					break;
			}

			return foundItemToConsume;
		} 
		#endregion
	}
}