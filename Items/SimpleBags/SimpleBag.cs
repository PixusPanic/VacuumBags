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
using System;

namespace VacuumBags.Items
{
	public abstract class SimpleBag : AndroModItem, ISoldByWitch {

		#region Children Properties
		
		protected abstract IDictionary<int, int> recipeIngredients { get; }
		public static void RegisterWithAndroLib(Mod mod) {
			BagStorageID = StorageManager.RegisterVacuumStorageClass(
				mod,//Mod
				typeof(SimpleBag),//type 
				ItemAllowedToBeStored,//Is allowed function, Func<Item, bool>
				null,//Localization Key name.  Attempts to determine automatically by treating the type as a ModItem, or you can specify.
				100,//StorageSize
				null,//Can vacuum
				() => new Color(80, 10, 10, androLib.Common.Configs.ConfigValues.UIAlpha),//Get color function. Func<using Microsoft.Xna.Framework.Color>
				() => new Color(90, 10, 10, androLib.Common.Configs.ConfigValues.UIAlpha),//Get Scroll bar color function. Func<using Microsoft.Xna.Framework.Color>
				() => new Color(120, 0, 0, androLib.Common.Configs.ConfigValues.UIAlpha),//Get Button hover color function. Func<using Microsoft.Xna.Framework.Color>
				() => ModContent.ItemType<SimpleBag>(),//Get ModItem type
				80,//UI Left
				675//UI Top
			);
		}

		protected virtual void EditRecipe(ref Recipe recipe) { }

		#endregion

		#region Parent Properties

		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
		public override void SetDefaults() {
            Item.maxStack = 99;
            Item.value = 100000;
			Item.rare = ItemRarityID.Blue;
			Item.width = 24;
            Item.height = 24;
        }
        public override void AddRecipes() {
            Recipe recipe = CreateRecipe();
			recipe.AddTile(TileID.WorkBenches);
			foreach (KeyValuePair<int, int> ingredient in recipeIngredients) {
				recipe.AddIngredient(ingredient.Key, ingredient.Value);
			}

			EditRecipe(ref recipe);

			recipe.Register();
        }

		public static int BagStorageID;//Set this when registering with androLib.

		public static bool ItemAllowedToBeStored(Item item) => true;

		#endregion

		#region AndroModItem attributes that you don't need.

		public virtual SellCondition SellCondition => SellCondition.Never;
		public virtual float SellPriceModifier => 1f;
		public override List<WikiTypeID> WikiItemTypes => new() { WikiTypeID.Storage };
		public override string LocalizationTooltip =>
			$"Automatically stores items already contained in the bag.\n" +
			$"When in your inventory, the contents of the bag are available for crafting.\n" +
			$"Right click to open the bag.";
		public override string Artist => "andro951";
		public override string ArtModifiedBy => "@kingjoshington";
		public override string Designer => "@kingjoshington";

		#endregion
	}
}