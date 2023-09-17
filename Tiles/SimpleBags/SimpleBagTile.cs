using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Microsoft.Xna.Framework;

namespace VacuumBags.Tiles
{
	public abstract class SimpleBagTile : VacuumBagTile {
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();
			SetSimpleBagDefaults();
		}

		public override string HighlightTexture => (GetType().Namespace + ".Sprites." + (Name.StartsWith("Bag") ? "Bag" : Name) + "_Highlight").Replace('.', '/');
	}
}
