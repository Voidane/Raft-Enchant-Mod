using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ConfigData
{
    public static string PickupExpKey = "Pickup Exp";
    public static string MobKillExpKey = "Mob Kill Exp";
    public static string FishingExpKey = "Fishing Exp";
    public static string TreeHarvestExpKey = "Tree Harvest Exp";
    public static string CookingExpKey = "Cooking Exp";
    public static string GrillingExpKey = "Grilling Exp";
    public static string JuicingExpKey = "Juicing Exp";
    public static string FarmingExpKey = "Farming Exp";
    public static string CraftingExpKey = "Crafting Exp";
    public static string MysteryPackageExpKey = "Mystery Package Exp";
    public static string QuestItemsExpKey = "Quest Items Exp";
    public static string ExtractorExpKey = "Extractor Exp";
    public static string BeehivesExpKey = "Beehives Exp";
    public static string SmeltingExpKey = "Smelting Exp";

    public Dictionary<string, bool>[] Settings;

    public Dictionary<string, float> Pickup_Exp;
    public Dictionary<string, float> Mob_Kill_Exp;
    public Dictionary<string, float> Fishing_Exp;
    public Dictionary<string, float> Tree_Harvest_Exp;
    public Dictionary<string, float> Cooking_Exp;
    public Dictionary<string, float> Grilling_Exp;
    public Dictionary<string, float> Juicing_Exp;
    public Dictionary<string, float> Farming_Exp;
    public Dictionary<string, float> Crafting_Exp;
    public Dictionary<string, float> Mystery_Package_Exp;
    public Dictionary<string, float> Quest_Items_Exp;
    public Dictionary<string, float> Extractor_Exp;
    public Dictionary<string, float> Beehives_Exp;
    public Dictionary<string, float> Smelting_Exp;
    
    public static ConfigData CreateConfig()
    {
        return new ConfigData
        {
            Settings = new Dictionary<string, bool>[]
            {
                new Dictionary<string, bool>
                {
                    { PickupExpKey, true },
                    { MobKillExpKey, true },
                    { FishingExpKey, true },
                    { TreeHarvestExpKey, true },
                    { CookingExpKey, true },
                    { GrillingExpKey, true },
                    { JuicingExpKey, true },
                    { FarmingExpKey, true },
                    { CraftingExpKey, true },
                    { MysteryPackageExpKey, true },
                    { QuestItemsExpKey, true },
                    { ExtractorExpKey, true },
                    { BeehivesExpKey, true },
                    { SmeltingExpKey, true },
                }
            },

            Pickup_Exp = new Dictionary<string, float>
            {
                { "leaf", 1.0F },
                { "palm_leaf", 1.0F },
                { "plastic", 1.0F },
                { "plank", 1.0F },
                { "barrel", 10.0F },
                { "crate", 20.0F },
                { "scrap", 1.0F },
                { "stone", 1.0F },
                { "clay", 1.0F },
                { "sand", 1.0F },
                { "metal_ore", 4.0F },
                { "copper", 4.0F },
                { "red_barries", 5.0F },
                { "cave_mushroom", 5.0F },
                { "thatch", 1.0F },
                { "mystery_package", 5.0F },
                { "rope", 3.0F },
                { "silveralgae", 2.5F },
                { "titaniumore", 10.0F },

                /** Radio Tower **/
                { "blueprint_headlight", 20.0F },
                { "blueprint_recycler", 20.0F },

                /** Vasagatan **/
                { "blueprint_motorwheel", 35.0F },
                { "blueprint_steeringwheel", 30.0F },

                /** Balboa **/
                { "blueprint_fueltank", 35.0F },
                { "blueprint_biofuelextractor", 35.0F },
                { "blueprint_machete", 35.0F },
                { "blueprint_fuelpipe", 35.0F },

                /** Caravan **/
                { "blueprint_batterycharger", 40.0F },
                { "blueprint_enginecontrols", 40.0F },
                { "blueprint_firework", 40.0F },
                { "blueprint_metaldetector", 40.0F },
                { "blueprint_ziplinebase", 40.0F },
                { "blueprint_ziplinetool", 40.0F },

                /** Tangaroa **/
                { "blueprint_electricpurifier", 50.0F },
                { "blueprint_storage_large", 50.0F },
                { "blueprint_pipe_water", 50.0F },
                { "blueprint_watertank", 50.0F },

                /** Varuna **/
                { "hat_construction", 65.0F },
                { "blueprint_batteryadvanced", 65.0F },
                { "blueprint_headlight_advanced", 65.0F },
                { "blueprint_windturbine", 65.0F },
                { "blueprint_electricgrill", 65.0F },

                /** Temperance **/
                { "blueprint_biofuelextractor_advanced", 80.0F },
                { "blueprint_smelterelectric", 80.0F },
                { "blueprint_anchorstationaryadvanced", 80.0F },

                /** Utopia **/
                { "blueprint_backpackadvanced", 125.0F },
                { "blueprint_ziplinetoolelectric", 125.0F },
                { "blueprint_titaniumtools", 250.0F }
            },

            Mob_Kill_Exp = new Dictionary<string, float>
            {
                { "bear", 50.0F },
                { "lurker", 10.0F },
                { "mamabear", 250.0F },
                { "shark", 62.5F  },
                { "seagull", 4.0F },
                { "stonebird", 50.0F },
                { "pufferfish", 10.0F },
                { "llama", 25.0F },
                { "goat", 25.0F },
                { "chicken", 10.0F },
                { "boar", 40.0F },
                { "rat", 5.0F },
                { "boss_varuna", 500.0F },
                { "anglerfisher", 15.0F },
                { "polarbear", 50.0F },
                { "roach", 2.0F },
                { "puffin", 25.0F },
                { "hyena", 25.0F },
                { "hyenaboss", 50.0F },
                { "alpha", 225.0F },
                { "bee", 3.5F }
            },

            Fishing_Exp = new Dictionary<string, float>
            {
                { "raw_pomfret", 3.5F },
                { "raw_herring", 3.5F },
                { "raw_tilapia", 6.0F },
                { "raw_mackerel", 6.0F },
                { "raw_catfish", 17.0F },
                { "raw_salmon", 17.0F },
                { "candle_bottle", 22.0F },
                { "old_shoe", 22.0F },
                { "placeable_luckycat", 65.0F },
                { "scrap_mechanic_duck", 65.0F }
            },

            Tree_Harvest_Exp = new Dictionary<string, float>
            {
                { "seed_palm", 1.5F },
                { "plank", 1.5F },
                { "palm_leaf", 1.5F},
                { "mango_seed", 1.5F },
                { "mango", 3.5F },
                { "coconut", 3.5F },
                { "thatch", 2.0F },
                { "pinecone", 5.0F },
                { "banana", 2.5F },
                { "seed_banana", 2.5F },
            },
            
            Cooking_Exp = new Dictionary<string, float>
            {
                { "claybowl_leftover", 5.0F },
                { "clayplate_bbq", 25.0F },
                { "claybowl_coconutchicken", 25.0F },
                { "clayplate_sushi", 25.0F },
                { "clayplate_sharkdinner", 25.0F },
                { "clayplate_steakwithjam", 25.0F },
                { "clayplate_salmonsalad", 25.0F },
                { "claybowl_simplefishstew", 25.0F },
                { "claybowl_rootvegetablesoup", 25.0F },
                { "clayplate_catfishdeluxe", 25.0F },
                { "clayplate_mushroomomelette", 25.0F },
                { "claybowl_headbrowth", 25.0F },
                { "claybowl_heartystew", 25.0F },
            },

            Grilling_Exp = new Dictionary<string, float>
            {
                { "raw_beet", 2.5F },
                { "raw_potato", 2.5F },
                { "raw_herring", 3.0F },
                { "raw_pomfret", 3.0F },
                { "raw_mackerel", 5.5F },
                { "raw_tilapia", 5.5F },
                { "raw_shark", 7.0F },
                { "raw_genericmeat", 7.0F },
                { "raw_drumstick", 7.0F },
                { "raw_catfish", 8.5F },
                { "raw_salmon", 8.5F }
            },

            Juicing_Exp = new Dictionary<string, float>
            {
                { "drinkingglass_spicypineberry", 15.0F },
                { "drinkingglass_redmelon", 15.0F },
                { "drinkingglass_simplesmoothie", 15.0F },
                { "drinkingglass_coconutbeat", 15.0F },
                { "drinkingglass_silversmoothie", 15.0F },
                { "drinkingglass_strawberrycolada", 15.0F },
                { "drinkingglass_mangonana", 15.0F },
                { "drinkingglass_redbeetshot", 15.0F },
                { "drinkingglass_leftover", 5.0F },
            },

            Farming_Exp = new Dictionary<string, float>
            {
                { "seed_strawberry", 1.5F },
                { "seed_pineapple", 1.5F },
                { "seed_banana", 1.5F },
                { "seed_flower_black", 1.5F },
                { "seed_flower_blue", 1.5F },
                { "seed_flower_red", 1.5F },
                { "seed_flower_white", 1.5F },
                { "seed_flower_yellow", 1.5F },
                { "seed_mango", 1.5F },
                { "seed_palm", 1.5F },
                { "seed_watermelon", 1.5F },
                { "strawberry", 2.0F },
                { "pineapple", 2.0F },
                { "banana", 2.0F },
                { "flower_black", 2.0F },
                { "flower_blue", 2.0F },
                { "flower_red", 2.0F },
                { "flower_white", 2.0F },
                { "flower_yellow", 2.0F },
                { "mango", 2.0F },
                { "watermelon", 2.0F },
                { "raw_potato", 2.0F },
                { "raw_beet", 2.0F },
            },

            Crafting_Exp = new Dictionary<string, float>
            {
                { "plasticcup_empty", 0.5F },
                { "plasticbottle_empty", 3.5F },
                { "canteen_empty", 6.0F },
                { "bucket", 0.5F },
                { "claybowl_empty", 1.0F },
                { "drinkingglass", 2.0F },
                { "placeable_cookingstand_purifier_one", 1.0F }, // Simple Purifier
                { "placeable_cookingstand_purifier_two", 5.0F }, // Water Purifier
                { "placeable_cookingstand_food_one", 1.0F }, // Simple Grill
                { "placeable_cookingstand_food_two", 5.0F }, // Advanced Grill
                { "placeable_cookingpot", 5.0F },
                { "placeable_juicer", 5.0F },
                { "placeable_cropplot_small", 1.0F },
                { "placeable_cropplot_medium", 2.0F },
                { "placeable_cropplot_large", 4.0F },
                { "placeable_scarecrow", 2.0F },
                { "placeable_scarecrow_advanced", 2.0F },
                { "placeable_researchtable", 1.0F },
                { "placeable_bed_basic", 1.0F },
                { "placeable_bed_hammock", 5.0F },
                { "placeable_storage_small", 1.0F },
                { "placeable_storage_medium", 4.0F },
                { "placeable_cookingstand_smelter", 3.0F },
                { "placeable_collectionnet_basic", 7.5F },
                { "placeable_collectionnet_advanced", 20.0F },
                { "placeable_beehive", 25.0F },
                { "hook_scrap", 4.0F },
                { "axe_stone", 1.0F },
                { "axe", 5.0F },
                { "fishingrod", 1.0F },
                { "fishingrod_metal", 3.0F },
                { "shear", 2.5F },
                { "binoculars", 5.0F },
                { "sharkbait", 2.0F },
                { "sweepnet", 4.0F },
                { "spear_plank", 0.5F },
                { "spear_scrap", 3.5F },
                { "bow", 2.5F },
                { "netgun", 5.0F },
                { "netcanister", 4.0F },
                { "oxygenbottle", 6.0F },
                { "flipper", 10.0F },
                { "backpack", 10.0F },
                { "equipment_leatherhelmet", 8.0F },
                { "equipment_leatherchest", 10.0F },
                { "equipment_leatherlegs", 7.5F },
                { "jar_honey", 5.0F },
                { "circuitboard", 3.5F },
                { "battery", 3.5F },
                { "paddle", 1.5F },
                { "placeable_sail", 5.0F },
                { "placeable_reciever", 7.0F },
                { "placeable_reciever_antenna", 4.0F },
                { "placeable_streamer", 2.0F },

                /** Radio Tower **/
                { "headlight", 10.0F },
                { "placeable_recycler", 10.0F },

                /** Vasagatan **/
                { "placeable_motorwheel", 10.0F },
                { "placeable_steeringwheel", 10.0F },

                /** Balboa **/
                { "placeable_fueltank", 10.0F },
                { "placeable_biofuelextractor", 10.0F },
                { "machete", 10.0F },
                { "placeable_pipe_fuel", 2.5F },

                /** Caravan **/
                { "placeable_batterycharger", 35.0F },
                { "placeable_enginecontrols", 35.0F },
                { "placeable_firework", 5.0F },
                { "metaldetector", 12.5F },
                { "placeable_ziplinebase", 10.0F },
                { "ziplinetool", 10.0F },

                /** Tangaroa **/
                { "placeable_electric_purifier", 40.0F },
                { "placeable_storage_large", 30.0F },
                { "placeable_pipe_water", 2.0F },
                { "placeable_watertank", 35.0F },

                /** Varuna **/
                { "battery_advanced", 40.0F },
                { "headlight_advanced", 25.0F },
                { "placeable_windturbine", 40.0F },
                { "placeable_cookingstand_food_electric", 40.0F },

                /** Temperance **/
                { "placeable_biofuelextractor_advanced", 50.0F },
                { "placeable_cookingstand_smelter_electric", 60.0F },
                { "placeable_anchor_stationary_advanced", 60.0F },

                /** Utopia **/
                { "backpack_advanced", 45.0F },
                { "ziplinetool_electric", 30.0F },
                { "sword_titanium", 20.0F },
                { "axe_titanium", 20.0F },
                { "hook_titanium", 20.0F },
                { "arrow_titanium", 5.0F }
            },

            Mystery_Package_Exp = new Dictionary<string, float>
            {
                { "all", 5.00F }
            },

            Extractor_Exp = new Dictionary<string, float>
            {
                { "trashcube", 10.00F },
                { "biofuel", 15.00F },
                { "honeycomb", 5.0F }
            },

            Quest_Items_Exp = new Dictionary<string, float>
            {
                { "vasagatab", 5.0F },
                { "balboa", 7.5F },
                { "caravan", 10.0F },
                { "tangaroa", 12.5F },
                { "varuna", 15.0F },
                { "temperance", 17.5F },
                { "utopia", 30.0F }
            },

            Beehives_Exp = new Dictionary<string, float>
            {
                { "honeycomb", 5.0F }
            },

            Smelting_Exp = new Dictionary<string, float>
            {
                { "titaniumore" , 10.0F },
                { "metalore" , 5.0F },
                { "copperore" , 5.0F },
                { "seavine" , 3.0F },
                { "sand" , 2.0F },
                { "explosivegood" , 4.0F },
            },

        };

    }
}