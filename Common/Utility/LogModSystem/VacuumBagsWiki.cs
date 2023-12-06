using androLib.Common.Utility;
using androLib.Common.Utility.LogSystem;
using androLib.Common.Utility.LogSystem.WebpageComponenets;
using androLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using VacuumBags.Items;
using VacuumOreBag.Items;

namespace VacuumBags.Common.Utility.LogModSystem
{
    public class VacuumBagsWiki : Wiki {
        public override bool ShouldPrintWiki => LogModSystem_VB.printWiki;
		public override Func<Mod> GetMod => ModContent.GetInstance<VacuumBags>;
		public override string ModName => VacuumBags.ModName;
        protected override void AddWikiPages(List<WebPage> webPages, IEnumerable<ModItem> modItems) {
            Type simpleBagType = typeof(SimpleBag);
            List<SimpleBag> simpleBags = new();
			List<SimpleBag> simplePacks = new();

            List<ModBag> modBags = new();
            List<AndroModItem> functionalBags = new();
			foreach (ModItem modItem in modItems) {
                if (modItem is SimpleBag simpleBag) {
                    if (simpleBag.GetType().BaseType == simpleBagType) {
						simpleBags.Add(simpleBag);
					}
                    else {
                        simplePacks.Add(simpleBag);
                    }
					
					continue;
                }
                
                if (modItem is ModBag modBag) {
                    modBags.Add(modBag);
                    continue;
                }

                if (modItem is BagModItem bagModItem) {
					functionalBags.Add(bagModItem);
                    continue;
				}
            }

            functionalBags.Add(ModContent.GetContent<OreBag>().First());
			functionalBags = functionalBags.OrderBy(b => b.Item.Name).ToList();

            AddMainPage(webPages, functionalBags, simpleBags, simplePacks, modBags);
            AddAllowedListsPage(webPages, functionalBags);
			AddAllBagPages(webPages, functionalBags, simpleBags, simplePacks, modBags);
		}

        private static string FunctionalBagsHeader => "Functional Bags";
		private static string SimpleBagsHeader => "Simple Bags";
        private static string SimplePacksHeader => "Simple Packs";
        private static string ModBagsHeader => "Mod Bags";
		private static void AddMainPage(List<WebPage> webPages, List<AndroModItem> functionalBags, List<SimpleBag> simpleBags, List<SimpleBag> simplePacks, List<ModBag> modBags) {
			WebPage mainPage = new(WebPage.MainPageName, mainWikiPage: "Vacuum_Bags_Wiki");
            mainPage.AddParagraph(
                "<mainpage-leftcolumn-start />\r\n" +
                "<div style=\"text-align: center;>[[File:Icon.png]]</div>\r\n" +
                "<div style=\"text-align: center; font-size: x-large; padding: 1em;\">'''Welcome to the {{SITENAME}}!'''</div>\r\n" +
                "\r\n" +
                "If you are interested in contributing to this wiki, please let me know: https://discord.gg/hEKKVsFBMd - andro951\r\n");

            mainPage.NewLine();
            mainPage.AddSubHeading("Description");
            mainPage.AddParagraph($"Vacuum bags adds three groups of bags to Terraria.");
            mainPage.AddBulletedList(elements: new string[] {
                $"{FunctionalBagsHeader.ToSectionLink()} - They store a specific set of items by default, and many of them have special functions such as allowing you to place items by using the bag, providing contents of the bag for ammo, and other ways of using items in the bags.",
                $"{SimpleBagsHeader.ToSectionLink()} - They are able to store any items by default, but do not automatically pick up items unless they already contain an item of that type.  (Simple Packs are alternate sprites for the bags.  They provide no functional difference.)",
                $"{ModBagsHeader.ToSectionLink()} - They are for specific mods such as Calamity and Thorium.  By default, they only store items from their associated mod."
            });
            
            mainPage.NewLine();
            mainPage.AddParagraph($"Bags have default items that are and aren't allowed but each bag can store any item by utilizing the {AllowedListsPageName.ToLink("white and black lists")} which can easily be interacted with in game.");
			mainPage.NewLine();
			mainPage.AddParagraph($"The size of the bags can be adjusted in the androLib Client Config.");
			mainPage.NewLine();

			AddItemTable(functionalBags, FunctionalBagsHeader, mainPage);
            AddItemTable(simpleBags, SimpleBagsHeader, mainPage);
			AddItemTable(simplePacks, SimplePacksHeader, mainPage);
			AddItemTable(modBags, ModBagsHeader, mainPage);

            webPages.Add(mainPage);
		}

        private static string AllowedListsPageName = "Allowed Lists";
		private static void AddAllowedListsPage(List<WebPage> webPages, List<AndroModItem> functionalBags) {
            WebPage allowedListsPage = new(AllowedListsPageName, webPages.Where(wp => wp.HeaderName == WebPage.MainPageName).First());

            allowedListsPage.AddParagraph(
                $"Each bag has a default set of items that it can store.  These sets can be modified using the white lists and black lists.  " +
                $"Each bag has it's own whitelist and blacklist.  " +
                $"The normal way of changing the lists is by interacting with the bags in game.  They can also be modified in the configs, but I suggest against it.  " +
                $"Modifying them in the configs is very slow, difficult to navigate and has the potential to accidentally delete an entire list or all lists if you aren't careful.");

			allowedListsPage.NewLine();
			allowedListsPage.AddParagraph(
                $"WARNING: Functional bags are designed to have the default items in them.  " +
                $"The functions of the bags only work when the items are in the correct bags.");
            allowedListsPage.AddSubHeading("Functional bags summary");
            allowedListsPage.AddBulletedList(elements: functionalBags.Select(b => $"{b.Item.ToItemPNG(link: true)} - " +
                $"{(b is BagModItem bagModItem ? bagModItem.SummaryOfFunction : BagModItem.SummaryOfFunctionDefault)}").ToArray());
            allowedListsPage.NewLine();

            allowedListsPage.AddSubHeading("Modifying lists in Game");
            allowedListsPage.AddSubHeading("Whitelists", 2);
            allowedListsPage.AddParagraph(
                $"Items can be whitelisted for a bag by using your mouse to put an item into a bag.  " +
                $"When an item is not whitelisted, this is the only way to put an item into a bag.  " +
                $"When items are whitelisted, they can be put into a bag in various ways including on pickup, shift left clicking from your inventory and " +
                $"using the deposit all button.");

            allowedListsPage.AddSubHeading("Blacklists", 2);
            allowedListsPage.AddParagraph(
                $"Items can be blacklisted for a bag by shift RIGHT clicking on them while they are in the bag.  " +
                $"This will attempt to remove all items of that type from the bag. (Removal can be toggled in the config.).");

            allowedListsPage.NewLine();
            allowedListsPage.AddParagraph($"Player allowed lists take priority over the default ones that I created to allow customizing bags as you like.  " +
                $"If you feel that certain items were missed as the default for a bag such as an ore or bar from a mod in the Ore Bag and you want to help out, " +
                $"there is a config option to print all of your allowed lists to your client.log.  " +
                $"If you provide me your client.log, I will add them to the default lists. (client.log found here by default: C:\\Steam\\SteamApps\\common\\tModLoader\\tModLoader-Logs)");

            webPages.Add(allowedListsPage);
        }

        private static void AddAllBagPages(List<WebPage> webPages, List<AndroModItem> functionalBags, List<SimpleBag> simpleBags, List<SimpleBag> simplePacks, List<ModBag> modBags) {
            AddBagPages(webPages, functionalBags);
			AddBagPages(webPages, simpleBags);
			AddBagPages(webPages, simplePacks);
			AddBagPages(webPages, modBags);
        }

		private static void AddBagPages(List<WebPage> webPages, IEnumerable<AndroModItem> androModItems) {
            WebPage mainPage = webPages.Where(wp => wp.HeaderName == WebPage.MainPageName).First();
			foreach (AndroModItem androModItem in androModItems) {
				WebPage bagPage = new(androModItem.Item.Name, mainPage);
				ItemInfoBox itemInfoBox = new(androModItem, FloatID.right);
                itemInfoBox.AddStatistics(bagPage);
                itemInfoBox.AddDrops(bagPage);
                itemInfoBox.TryAddWikiDescription(bagPage);
                itemInfoBox.AddInfo(bagPage);
                itemInfoBox.AddRecipes(bagPage);
				webPages.Add(bagPage);
			}
        }
	}
}
