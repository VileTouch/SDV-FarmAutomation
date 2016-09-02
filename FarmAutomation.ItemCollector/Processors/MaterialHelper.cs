using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using StardewValley;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace FarmAutomation.ItemCollector.Processors

//        ========================================================================================
//        TODO:
//          + Insert all [New Machines Mod] and [More Crops Mod] materials.
//          - Establish the special machines logic (Tank, Separator, Mixer)
//          - Move item values to it's own config file for easy insertion of new materials
//        =======================================================================================

{
    public class MaterialHelper
    {
        private readonly int[] _ores = {
            Object.copper,
            Object.iron,
            Object.gold,
            Object.iridium
        };

        private int[] _seedmakerDropIns;

        public Object FindMaterialForMachine(string machineName, Chest chest)
        {
            if (chest == null)
            {
                return null;
            }

            switch (machineName)
            {
                case "Keg":
                    return (Object)chest.items.FirstOrDefault(i => i is Object && IsKegMaterial(i));
                case "Preserves Jar":
                    return (Object)chest.items.FirstOrDefault(i => i is Object && IsPreservesJarMaterial(i));
                case "Cheese Press":
                    return (Object)chest.items.FirstOrDefault(i => i is Object && IsCheesePressMaterial(i));
                case "Mayonnaise Machine":
                    return (Object)chest.items.FirstOrDefault(i => i is Object && IsMayonnaiseMachineMaterial(i));
                case "Loom":
                    return (Object)chest.items.FirstOrDefault(i => i is Object && IsLoomMaterial(i));
                case "Oil Maker":
                    return (Object)chest.items.FirstOrDefault(i => i is Object && IsOilMakerMaterial(i));
                case "Recycling Machine":
                    return (Object)chest.items.FirstOrDefault(i => i is Object && IsRecyclingMachineMaterial(i));
                case "Furnace":
                    return (Object)chest.items.FirstOrDefault(i => i is Object && IsFurnaceMaterial(i));
                case "Coal":
                    return (Object)chest.items.FirstOrDefault(i => i is Object && i.parentSheetIndex == Object.coal);
                case "Seed Maker":
                    return (Object)chest.items.FirstOrDefault(i => i is Object && IsSeedMakerMaterial(i));
                case "Crab Pot":
                    return (Object)chest.items.FirstOrDefault(i => i is Object && IsCrabPotMaterial(i));
                case "Charcoal Kiln":
                    return (Object)chest.items.FirstOrDefault(i => i is Object && i.parentSheetIndex == Object.wood);
                case "Slime Egg-Press":
                    return (Object)chest.items.FirstOrDefault(i => i is Object && i.parentSheetIndex == 766); // Slime

           try {        //start NewMachinesMod Support
                case "Drying Rack"
                    return (Object)chest.items.FirstOrDefault(i => i is Object && IsDryingRackMaterial(i));
                case "Vinegar Jug"
                    return (Object)chest.items.FirstOrDefault(i => i is Object && IsVinegarJugMaterial(i));
                case "Mill"
                    return (Object)chest.items.FirstOrDefault(i => i is Object && IsMillMaterial(i));
                case "Tank" // Needs to be filled with water before accepting materials
                    return (Object)chest.items.FirstOrDefault(i => i is Object && IsTankMaterial(i));
                case "Separator" // Input only. Doesn't accept materials if there are no modules installed.
                    return (Object)chest.items.FirstOrDefault(i => i is Object && IsSeparatorMaterial(i));
                case "Fermenter" // Separator module. Output only
                    return (Object)chest.items.FirstOrDefault(i => i is Object && IsSeparatorMaterial(i));
                case "Churn" // Separator module. Output only
                    return (Object)chest.items.FirstOrDefault(i => i is Object && IsSeparatorMaterial(i));
                case "Mixer" // Needs to be fed two different materials
                    return (Object)chest.items.FirstOrDefault(i => i is Object && IsMixerMaterial(i));
            }

            catch {}    // End NewMachinesMod Support
                default:
                    return null;
            }
            }
        }

        private bool IsCrabPotMaterial(Item item)
        {
            return item.category == Object.baitCategory;
        }

        private bool IsSeedMakerMaterial(Item item)
        {
            if (_seedmakerDropIns == null)
            {
                if (Game1.temporaryContent == null)
                {
                    Game1.temporaryContent = new ContentManager(Game1.content.ServiceProvider, Game1.content.RootDirectory);
                }
                Dictionary<int, string> dictionary = Game1.temporaryContent.Load<Dictionary<int, string>>("Data\\Crops");
                _seedmakerDropIns = dictionary.Values.Select(v => v.Split('/')).Select(s => Convert.ToInt32(s[3])).ToArray();
            }
            return _seedmakerDropIns.Contains(item.parentSheetIndex);
        }

        /// <summary>
        /// most machines only take 1 object, a few exceptions have to be handled though
        /// </summary>
        /// <param name="machineName"></param>
        /// <param name="material"></param>
        /// <returns></returns>
        public int GetMaterialAmountForMachine(string machineName, Object material)
        {
            if (machineName == "Furnace" && material.parentSheetIndex != Object.coal && material.parentSheetIndex != Object.quartzIndex)
                {
                    return 5;
                }
            if (machineName == "Slime Egg-Press" && material.parentSheetIndex == 766) // Slime
                {
                    return 100;
                }
            if (machineName == "Charcoal Kiln" && material.parentSheetIndex == Object.wood)
                {
                    return 10;
                }

            try {       // Start NewMachinesMod Support

                    if (machineName == "Charcoal Kiln" && material.parentSheetIndex == Object.hardwood)
                        {
                            return 1
                        }
                    if (machineName == "Charcoal Kiln" && material.parentSheetIndex == 322) // Wood Fence
                        {
                            return 5
                        }
                    if (machineName == "Charcoal Kiln" && material.parentSheetIndex == 325) // Gate
                        {
                            return 5
                        }
                    if (machineName == "Keg" && item.parentSheetIndex ==  839)  // Rice
                        {
                            return 5;
                        }
                    if (machineName == "Loom" && item.parentSheetIndex ==  836) // Cotton
                        {
                            return 4;
                        }

                }

            catch {}   // End NewMachinesMod Support

            return 1;
        }

        private bool IsFurnaceMaterial(Item item)
        {
            if (item.parentSheetIndex == 80) // Quartz
                {
                    return true;
                }
            if (_ores.Contains(item.parentSheetIndex) && item.Stack >= 5)
                {
                    return true;
                }
            return false;
        }

        public bool IsRecyclingMachineMaterial(Item item)
        {
            return item.parentSheetIndex == 168     // Trash
                || item.parentSheetIndex == 169     // Driftwood
                || item.parentSheetIndex == 170     // Broken Glasses
                || item.parentSheetIndex == 171     // Broken CD
                || item.parentSheetIndex == 172;    // Soggy Newspaper

        }

        public bool IsOilMakerMaterial(Item item)
        {
            return item.parentSheetIndex == 270     // Corn
                || item.parentSheetIndex == 421     // Sunflower
                || item.parentSheetIndex == 430     // Truffle
                || item.parentSheetIndex == 431;    // Sunflower Seeds

        }

        public bool IsMayonnaiseMachineMaterial(Item item)
        {
            return item.parentSheetIndex == 174     // Large Egg
                || item.parentSheetIndex == 107     // Dinosaur Egg
                || item.parentSheetIndex == 176     // Egg
                || item.parentSheetIndex == 180     // Egg
                || item.parentSheetIndex == 182     // Large Egg
                || item.parentSheetIndex == 442;    // Duck Egg
        }

        public bool IsCheesePressMaterial(Item item)
        {
            return item.parentSheetIndex == 184     // Milk
                || item.parentSheetIndex == 186     // Large Milk
                || item.parentSheetIndex == 436     // Goat Milk
                || item.parentSheetIndex == 438;    // L. Goat Milk
        }

        public bool IsPreservesJarMaterial(Item item)
        {
            return item.category == Object.FruitsCategory
                || item.category == Object.VegetableCategory;

        // Start NewMachinesMod Support

                || item.parentSheetIndex == 404     // Common Mushroom
                || item.parentSheetIndex == 281     // Chanterelle
                || item.parentSheetIndex == 257     // Morel
                || item.parentSheetIndex == 420     // Red Mushroom
                || item.parentSheetIndex == 422     // Purple Mushroom

        // End NewMachinesMod Support

        }

        public bool IsKegMaterial(Item i)
        {
            return i.category == Object.FruitsCategory
                || i.category == Object.VegetableCategory
                || i.parentSheetIndex == 262    // Wheat
                || i.parentSheetIndex == 304;   // Hops
        }

        public bool IsLoomMaterial(Item i)
        {
            return i.parentSheetIndex == 440   // Wool

        // Start NewMachinesMod Support

            || item.parentSheetIndex ==  836; // Cotton

        // End NewMachinesMod Support

        }
        try {       // Start NewMachinesMod Support
            public bool IsSeparatorMaterial(Item i)
                {
                return item.parentSheetIndex == 184     // Milk
                    || item.parentSheetIndex == 186     // Large Milk
                }

            public bool IsDryingRackMaterial(Item i)
                {
                return i.category == Object.FruitsCategory
                    || i.parentSheetIndex == 771        // Fiber
                    || i.parentSheetIndex == 839        // Rice
                    || i.parentSheetIndex == 398        // Grape
                //  || i.parentSheetIndex == ???        // Coffee Beans. temp placeholder
                }

            public bool IsMillMaterial(Item i)
                {
                return i.parentSheetIndex == 262        // Wheat
                    || i.parentSheetIndex == 300        // Amaranth
                    || i.parentSheetIndex == 270        // Corn
             //     || i.parentSheetIndex == ???        // Dried Coffee Beans. temp placeholder
                }

            public bool IsVinegarJugMaterial(Item i)
                {
                return i.parentSheetIndex == 613        // Apple
                    || i.parentSheetIndex == 902        // Wine
                }
            }

        catch{} // End NewMachinesMod Support
    }
}
