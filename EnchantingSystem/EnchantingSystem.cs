using RaftModLoader;
﻿using UnityEngine;
using HMLLibrary;
using System.Collections;
using UnityEngine.SceneManagement;
using Steamworks;
using System;
using System.IO;
using HarmonyLib;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

public class EnchantingSystem : Mod
{
    public static readonly string MOD_NAME = "[Enchanting System]";

    public static AssetBundle asset;
    public static Harmony harmony;
    public static Experience_Bar experienceBar;
    public static Item_Base enchant_table_item;

    private ModConfig modConfig;
    private RGD_Game_AdditionalData savedData;
    private bool delayWorldLoading;

    public static GameObject placeable_enchantment_table_gameobject;
    public static Item_Base placeable_enchantment_table;

    public async void Start()
    {
        HNotification notification = FindObjectOfType<HNotify>().AddNotification(HNotify.NotificationType.spinning, "Loading Enchanting System...");
        delayWorldLoading = true;

        AssetBundleCreateRequest request = AssetBundle.LoadFromMemoryAsync(GetEmbeddedFileBytes("enchantingsystem.assets"));
        
        while (!request.isDone)
            await Task.Delay(1);

        asset = request.assetBundle;
        await InitializeItems();

        ModDataPath();
        modConfig = new ModConfig();
        
        harmony = new Harmony("com.voidane.enchantingsystem");
        harmony.PatchAll();

        delayWorldLoading = false;

        notification.Close();
        Debug.Log($"{MOD_NAME} Has been loaded.");
    }

    public void OnModUnload()
    {
        asset.Unload(true);
        harmony.UnpatchAll();
        Debug.Log($"{MOD_NAME} Has Been Unloaded!");
    }

    [ConsoleCommand(name: "es", docs: "Gives the player an enchanting table")]
    public static void MyCommand(string[] args)
    {
        Debug.Log($"Item_Base [ uniqueName: {placeable_enchantment_table.UniqueName}, uniqueIndex: {placeable_enchantment_table.UniqueIndex} ]");
        Debug.Log($"Item_Base -> usable [ anim_on_select: {placeable_enchantment_table.settings_usable.AnimationOnSelect} ]");
        RAPI.GiveItem(ItemManager.GetItemByName("placeable_enchantment_bench"), 1);
    }

    private async Task InitializeItems()
    {
        placeable_enchantment_table = await asset.TaskLoadAssetAsync<Item_Base>("placeable_enchantment_bench");
        placeable_enchantment_table_gameobject = placeable_enchantment_table.settings_buildable.GetBlockPrefab(0).gameObject;

        if (placeable_enchantment_table != null && placeable_enchantment_table_gameobject != null)
        {
            RegisterPlaceableItem(placeable_enchantment_table, new RBlockQuadType[] { RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table});
        }
    }

    private void RegisterPlaceableItem(Item_Base item, RBlockQuadType[] blockType)
    {
        item.settings_recipe.NewCost = EnchantmentBenchObject.CostToCraft();
        RAPI.RegisterItem(item);
        
        foreach (RBlockQuadType type in blockType)
        {
            RAPI.AddItemToBlockQuadType(item, type);
        }
    }

    public override void WorldEvent_WorldSaved()
    {
        Debug.Log("Attempting to save");
        if (experienceBar != null && savedData != null)
        {
            savedData.SaveFromExpBar(experienceBar);
            SaveData(savedData);
        }
    }

    public override void WorldEvent_WorldLoaded()
    {
        Debug.Log("New world loaded in");

        experienceBar = new Experience_Bar(asset);
        Experience_Bar.expBar = Instantiate(Experience_Bar.expBar, RAPI.GetLocalPlayer().canvas.transform);

        savedData = LoadData();

        if (savedData != null)
        {
            Debug.Log("An existing files data was loaded into the world");
            savedData.LoadIntoExpBar(experienceBar);
        }
        else
        {
            Debug.Log("A fresh new start on save data");
            savedData = new RGD_Game_AdditionalData();
        }

        experienceBar.Resetup();
    }


    public override void WorldEvent_WorldUnloaded()
    {
        if (experienceBar != null && Experience_Bar.expBar != null)
        {
            if (savedData != null)
            {
                savedData.SaveFromExpBar(experienceBar);
                SaveData(savedData);
            }
            Destroy(Experience_Bar.expBar);
        }

        Experience_Bar.ResetValues();
        savedData = null;
    }

    private RGD_Game_AdditionalData LoadData()
    {
        try 
        {
            string key = $"EnchantingSystem_{SaveAndLoad.CurrentGameFileName}_{RAPI.GetLocalPlayer().steamID}";
            Debug.Log(key);
            string json = PlayerPrefs.GetString(key, "");

            if (!string.IsNullOrEmpty(json))
            {
                return JsonUtility.FromJson<RGD_Game_AdditionalData>(json);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading experience data: {e}");
        }
        return null;
    }

    private void SaveData(RGD_Game_AdditionalData data)
    {
        try
        {
            string key = $"EnchantingSystem_{SaveAndLoad.CurrentGameFileName}_{RAPI.GetLocalPlayer().steamID}";
            Debug.Log(key);
            string json = JsonUtility.ToJson(data);

            PlayerPrefs.SetString(key, json);
            PlayerPrefs.Save();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving experience data: {e}");
        }
    }

    public static string ModDataPath()
    {
        string path = Path.Combine(Application.dataPath, "../mods", "ModData", "EnchantingSystem");

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        return path;
    }
}


[HarmonyPatch(typeof(Hotbar), "SelectItem")]
class Patch_Hotbar_SelectItem
{
    [HarmonyPrefix]
    static void Prefix(Hotbar __instance, Item_Base item)
    {
        if (item == null)
            return;

        RAPI.GetLocalPlayer().SendChatMessage("Selcted " + item.UniqueName);
    }


    /**
     * FOR DEBUGGING
     */
    [HarmonyPostfix]
    static void Postfix(Hotbar __instance, Item_Base item)
    {
        var playerNetwork = AccessTools.Field(typeof(Hotbar), "playerNetwork").GetValue(__instance) as Network_Player;

        if (playerNetwork.PlayerItemManager == null)
            Debug.Log("Player item manager was null");
        else
            Debug.Log("Player item manager was not null");

        if (item == null)
        {
            Debug.Log("The item was null");
        }
        else if (!item.settings_usable.IsUsable())
            Debug.Log("The item was not usable");

        if (item == null || !item.settings_usable.IsUsable())
        {
            Debug.Log("this.playerNetwork.PlayerItemManager.SelectUsable(null);");
            return;
        }
        Debug.Log("this.playerNetwork.PlayerItemManager.SelectUsable(item)");
    }
}

/**
 * DEBUGGING
**/
[HarmonyPatch(typeof(PlayerItemManager), "SelectUsable")]
class Patch_PlayerItemManager_SelectUsable
{
    [HarmonyPostfix]
    static void Postfix(PlayerItemManager __instance, Item_Base item)
    {
        var playerNetwork = AccessTools.Field(typeof(PlayerItemManager), "playerNetwork").GetValue(__instance) as Network_Player;

        if (playerNetwork.IsLocalPlayer)
        {
            Debug.Log("[PlayerItemManager : SelectUsable] Player is local");
            if (item != null)
            {
                Debug.Log("[PlayerItemManager : SelectUsable] Item was not null");
            }
            else
            {
                Debug.LogError("[PlayerItemManager : SelectUsable] Item was null");
            }
        }
        else
        {
            Debug.LogError("[PlayerItemManager : SelectUsable] Player is not local");
        }

        
    }
}

/**
[HarmonyPatch(typeof(BlockCreator), "Update")]
class Patch_BlockCreator_Update
{
    [HarmonyPrefix]
    static void Prefix(BlockCreator __instance)
    {
        Debug.Log("PREFIX DEBUGGER WORKED");

        var selectedBuildableItem = AccessTools.Field(typeof(BlockCreator), "selectedBuildableItem").GetValue(__instance) as Item_Base;
        var quadSurface = AccessTools.Field(typeof(BlockCreator), "selectedBlock").GetValue(__instance) as Block;
    }
}
**/

/**
[HarmonyPatch(typeof(BlockQuad), "AcceptsBlock")]
class Patch_BlockQuad_AcceptsBlock
{
    [HarmonyPrefix]
    static void Prefix(BlockQuad __instance, Item_Base blockItem, Vector3 surfaceNormal)
    {
        var quadType = AccessTools.Field(typeof(BlockQuad), "quadType").GetValue(__instance) as SO_BlockQuadType;

        if (quadType.AcceptsBlock(blockItem))
        {
            Debug.Log("quadType was accepted");
        }
        else
        {
            Debug.Log("quadType was not accepted");
        }
    }
}
**/

// This works but could be intensive
[HarmonyPatch(typeof(SO_BlockQuadType), "AcceptsBlock")]
class Patch_SO_BlockQuadType_AcceptsBlock
{

    static List<Item_Base> acceptableBlockTypes = null;

    [HarmonyPrefix]
    static void Prefix(SO_BlockQuadType __instance, Item_Base block)
    {
        acceptableBlockTypes = AccessTools.Field(typeof(SO_BlockQuadType), "acceptableBlockTypes").GetValue(__instance) as List<Item_Base>;

        if (!acceptableBlockTypes.Contains(EnchantingSystem.placeable_enchantment_table))
        {
            Debug.Log("Added enchant to the itembase list now");
            acceptableBlockTypes.Add(EnchantingSystem.placeable_enchantment_table);
        }
    }
}

[HarmonyPatch(typeof(FishingRod), "PullItemsFromSea")]
class Patch_FishingRod_PullItemsFromSea
{
    private static Dictionary<ItemInstance, int> prefixInventory = new Dictionary<ItemInstance, int>();
    private static Dictionary<ItemInstance, int> postfixInventory = new Dictionary<ItemInstance, int>();

    [HarmonyPrefix]
    static void Prefix(FishingRod __instance)
    {
        RAPI.GetLocalPlayer().SendChatMessage("pull item from sea inv event");
        try
        {
            var playerNetwork = AccessTools.Field(typeof(FishingRod), "playerNetwork").GetValue(__instance) as Network_Player;

            if (playerNetwork.Inventory != null)
            {
                prefixInventory.Clear();
                ES_Tools.CreateInventorySnapshotDictionary(playerNetwork.Inventory.allSlots, prefixInventory);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in fishing prefix: {e.Message}");
        }
    }

    [HarmonyPostfix]
    static void Postfix(FishingRod __instance)
    {
        try
        {
            if (ModConfig.configData.Settings[0].TryGetValue(ConfigData.FishingExpKey, out bool isOn))
            {
                if (!isOn)
                    return;
            }


            var playerNetwork = AccessTools.Field(typeof(FishingRod), "playerNetwork").GetValue(__instance) as Network_Player;

            if (playerNetwork.Inventory != null)
            {
                postfixInventory.Clear();
                ES_Tools.CreateInventorySnapshotDictionary(playerNetwork.Inventory.allSlots, postfixInventory);
                Dictionary<ItemInstance, int> items = ES_Tools.ItemDifferenceFromInventory(prefixInventory, postfixInventory);

                foreach (var (instance, quantity) in items)
                {
                    Experience_Bar.AddExperienceFromItem(instance, quantity, ModConfig.configData.Fishing_Exp);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in fishing postfix: {e.Message}");
        }
    }
}

[HarmonyPatch(typeof(HarvestableTree), "Harvest")]
class Patch_HarvestableTree_Harvest
{

    private static Dictionary<ItemInstance, int> prefixInventory = new Dictionary<ItemInstance, int>();
    private static Dictionary<ItemInstance, int> postfixInventory = new Dictionary<ItemInstance, int>();

    [HarmonyPrefix]
    static bool Prefix(HarvestableTree __instance, PlayerInventory playerInventory, ref bool __result)
    {
        RAPI.GetLocalPlayer().SendChatMessage("harvest event");
        try
        {
            if (playerInventory != null)
            {
                prefixInventory.Clear();
                ES_Tools.CreateInventorySnapshotDictionary(playerInventory.allSlots, prefixInventory);
            }
            // Allow original method to run
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in harvestable tree prefix: {e.Message}");
            return true;
        }
    }

    [HarmonyPostfix]
    static void Postfix(HarvestableTree __instance, PlayerInventory playerInventory, bool __result)
    {
        try
        {
            if (ModConfig.configData.Settings[0].TryGetValue(ConfigData.TreeHarvestExpKey, out bool isOn))
            {
                if (!isOn)
                    return;
            }

            if (playerInventory != null && __result) // Only if harvest was successful
            {
                postfixInventory.Clear();

                ES_Tools.CreateInventorySnapshotDictionary(playerInventory.allSlots, postfixInventory);
                Dictionary<ItemInstance, int> items = ES_Tools.ItemDifferenceFromInventory(prefixInventory, postfixInventory);

                foreach (var (instance, quantity) in items)
                {
                    Experience_Bar.AddExperienceFromItem(instance, quantity, ModConfig.configData.Tree_Harvest_Exp);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in harvestable tree postfix: {e.Message}");
        }
    }
}

[HarmonyPatch(typeof(Pickup), "AddItemToInventory")]
class Patch_Pickup_AddItemToInventory
{

    private static Dictionary<ItemInstance, int> prefixInventory = new Dictionary<ItemInstance, int>();
    private static Dictionary<ItemInstance, int> postfixInventory = new Dictionary<ItemInstance, int>();

    [HarmonyPrefix]
    static void Prefix(Pickup __instance, PickupItem item)
    {
        RAPI.GetLocalPlayer().SendChatMessage("Add item to inv event " + item.pickupTerm);
        try
        {
            string _name = item.pickupTerm.ToLower().Replace(" ", "_");
            foreach (string _term in ModConfig.configData.Quest_Items_Exp.Keys)
            {
                Debug.Log("Key Search Term: " + _term);
                if (_name.Contains(_term))
                {
                    Debug.Log(_name + " contains " + _term);
                    ModConfig.configData.Quest_Items_Exp.TryGetValue(_term, out float exp);

                    if (ModConfig.configData.Settings[0].TryGetValue(ConfigData.QuestItemsExpKey, out bool isOn))
                    {
                        if (isOn)
                        {
                            Experience_Bar.AddExperience(exp);
                        }
                    }
                }
            }

            var playerInventory = AccessTools.Field(typeof(Pickup), "playerInventory").GetValue(__instance) as PlayerInventory;

            if (playerInventory != null)
            {
                prefixInventory.Clear();
                ES_Tools.CreateInventorySnapshotDictionary(playerInventory.allSlots, prefixInventory);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in pickup add item to inventory prefix: {e.Message}");
        }
    }

    [HarmonyPostfix]
    static void Postfix(Pickup __instance, PickupItem item)
    {
        try
        {
            var playerInventory = AccessTools.Field(typeof(Pickup), "playerInventory").GetValue(__instance) as PlayerInventory;
            RAPI.GetLocalPlayer().SendChatMessage("Pickedup: " + item.PickupName);

            if (playerInventory != null)
            {
                postfixInventory.Clear();
                ES_Tools.CreateInventorySnapshotDictionary(playerInventory.allSlots, postfixInventory);
                Dictionary<ItemInstance, int> items = ES_Tools.ItemDifferenceFromInventory(prefixInventory, postfixInventory);

                foreach (var (instance, quantity) in items)
                {
                    if (!item.isDropped)
                    {
                        if (ModConfig.configData.Settings[0].TryGetValue(ConfigData.PickupExpKey, out bool isOn))
                        {
                            if (isOn)
                            {
                                Experience_Bar.AddExperienceFromItem(instance, quantity, ModConfig.configData.Pickup_Exp);
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in pickup postfix: {e.Message}");
        }
    }
}

// Event: On animal kill event
[HarmonyPatch(typeof(AI_StateMachine_Animal), "OnDeath")]
class Patch_ResourceCollector_Harvest
{
    [HarmonyPostfix]
    static void Postfix(AI_StateMachine_Animal __instance)
    {
        RAPI.GetLocalPlayer().SendChatMessage("animal death event");
        var networkBehaviour = AccessTools.Field(typeof(AI_StateMachine_Animal), "networkBehaviour").GetValue(__instance) as AI_NetworkBehaviour_Animal;
        string _name = networkBehaviour.behaviourType.ToString().ToLower();

        if (ModConfig.configData.Settings[0].TryGetValue(ConfigData.MobKillExpKey, out bool isOn))
        {
            if (isOn)
            {
                Experience_Bar.AddExperienceFromMob(networkBehaviour.behaviourType, 1, ModConfig.configData.Mob_Kill_Exp);
            }
        }
    }
}

// Cooking pot event
// Juicer pot event
[HarmonyPatch(typeof(CookingTable), "PickupFood")]
class Patch_CookingTable_FoodPickup
{
    [HarmonyPrefix]
    static void Prefix(CookingTable __instance, Network_Player player)
    {
        RAPI.GetLocalPlayer().SendChatMessage("Pickup food event");
        var current_recipe = AccessTools.Field(typeof(CookingTable), "currentRecipe").GetValue(__instance) as SO_CookingTable_Recipe;
        Item_Base result = current_recipe.Result;

        if (current_recipe.RecipeType == CookingRecipeType.CookingPot)
        {
            if (ModConfig.configData.Settings[0].TryGetValue(ConfigData.CookingExpKey, out bool isOn))
            {
                if (isOn)
                {
                    Experience_Bar.AddExperienceFromItem(result, 1, ModConfig.configData.Cooking_Exp);
                    return;
                }
            }
        }
        else if (current_recipe.RecipeType == CookingRecipeType.Juicer)
        {
            if (ModConfig.configData.Settings[0].TryGetValue(ConfigData.JuicingExpKey, out bool isOn))
            {
                if (isOn)
                {
                    Experience_Bar.AddExperienceFromItem(result, 1, ModConfig.configData.Juicing_Exp);
                    return;
                }
            }
        }
    }
}

// Electruc Smelter
// Smelter
// Simple Grill
// Electric Grill
// Advanced Grill
[HarmonyPatch(typeof(Block_CookingStand), "CollectItem")]
class Patch_CookingStand_CollectItem
{
    [HarmonyPrefix]
    static bool Prefix(Block_CookingStand __instance, CookingSlot slot, Network_Player player)
    {
        RAPI.GetLocalPlayer().SendChatMessage("Cooking collecting event");
        Item_Base item = slot.CurrentItem;

        if (item == null)
            return true;

        string _name = item.UniqueName.Replace(" ", "_").ToLower();
        
        if (ModConfig.configData.Grilling_Exp.ContainsKey(_name))
        {
            if (ModConfig.configData.Settings[0].TryGetValue(ConfigData.GrillingExpKey, out bool isOn))
            {
                if (isOn)
                {
                    Experience_Bar.AddExperienceFromItem(item, 1, ModConfig.configData.Grilling_Exp);
                }
            }
        }
        else if (ModConfig.configData.Smelting_Exp.ContainsKey(_name))
        {
            if (ModConfig.configData.Settings[0].TryGetValue(ConfigData.SmeltingExpKey, out bool isOn))
            {
                if (isOn)
                {
                    Experience_Bar.AddExperienceFromItem(item, 1, ModConfig.configData.Smelting_Exp);
                }
            }
        }
        return true;
    }
}

[HarmonyPatch(typeof(CraftingMenu), "CraftItem")]
class Patch_CraftingMenu_CraftingItem
{
    [HarmonyPrefix]
    static bool Prefix(CraftingMenu __instance)
    {
        var recpie = AccessTools.Field(typeof(CraftingMenu), "selectedRecipeBox").GetValue(__instance) as SelectedRecipeBox;
        Item_Base item = recpie.ItemToCraft;

        if (ModConfig.configData.Settings[0].TryGetValue(ConfigData.CraftingExpKey, out bool isOn))
        {
            if (isOn)
            {
                Experience_Bar.AddExperienceFromItem(item, 1, ModConfig.configData.Crafting_Exp);
            }
        }

        return true;
    }

    /**
     * TODO : REMOVE (DEBUGGING)
     */
    [HarmonyPostfix]
    static void Postfix(CraftingMenu __instance)
    {
        var itemToCraft = AccessTools.Field(typeof(CraftingMenu), "selectedRecipeBox").GetValue(__instance) as SelectedRecipeBox;
        
        if (itemToCraft.ItemToCraft == null)
        {
            Debug.Log("Item was null");
        }
        else
        {
            Debug.Log($"Crafting: {itemToCraft.ItemToCraft}, Total: {itemToCraft.ItemToCraft.settings_recipe.AmountToCraft}");
        }
    }
}

[HarmonyPatch(typeof(Cropplot), "PlantRemoved")]
class Patch_Cropplot_PlantRemove
{

    private static Dictionary<ItemInstance, int> prefixInventory = new Dictionary<ItemInstance, int>();
    private static Dictionary<ItemInstance, int> postfixInventory = new Dictionary<ItemInstance, int>();

    [HarmonyPrefix]
    static bool Prefix(Cropplot __instance)
    {
        var player = AccessTools.Field(typeof(Cropplot), "localPlayer").GetValue(__instance) as Network_Player;
        
        if (player.Inventory != null)
        {
            prefixInventory.Clear();
            ES_Tools.CreateInventorySnapshotDictionary(player.Inventory.allSlots, prefixInventory);
            return true;
        }
        return true;
    }

    [HarmonyPostfix]
    static void Postfix(Cropplot __instance)
    {
        try
        {
            if (ModConfig.configData.Settings[0].TryGetValue(ConfigData.FarmingExpKey, out bool isOn))
            {
                if (!isOn)
                    return;
            }

            var player = AccessTools.Field(typeof(Cropplot), "localPlayer").GetValue(__instance) as Network_Player;

            if (player.Inventory != null)
            {
                postfixInventory.Clear();
                ES_Tools.CreateInventorySnapshotDictionary(player.Inventory.allSlots, postfixInventory);
                Dictionary<ItemInstance, int> items = ES_Tools.ItemDifferenceFromInventory(prefixInventory, postfixInventory);

                foreach (var (instance, quantity) in items)
                {
                    Experience_Bar.AddExperienceFromItem(instance, quantity, ModConfig.configData.Farming_Exp);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in farming postfix: {e.Message}");
        }
    }
}

[HarmonyPatch(typeof(Item_MysteryPackage), "ExtractLoot")]
class Patch_ItemMysteryPackage_ExtractLoot
{
    [HarmonyPrefix]
    static void Prefix(Cropplot __instance)
    {
        if (ModConfig.configData.Settings[0].TryGetValue(ConfigData.MysteryPackageExpKey, out bool isOn))
        {
            if (isOn)
            {
                if (ModConfig.configData.Mystery_Package_Exp.TryGetValue("all", out float exp))
                {
                    Experience_Bar.AddExperience(exp);
                }
            }
        }
    }
}

[HarmonyPatch(typeof(Placeable_Extractor), "HarvestOutput")]
class Patch_PlaceableExtractor_HarvestOutput
{

    private static Dictionary<ItemInstance, int> prefixInventory = new Dictionary<ItemInstance, int>();
    private static Dictionary<ItemInstance, int> postfixInventory = new Dictionary<ItemInstance, int>();

    [HarmonyPrefix]
    static void Prefix(Placeable_Extractor __instance, Network_Player player)
    {
        if (player.Inventory != null)
        {
            prefixInventory.Clear();
            ES_Tools.CreateInventorySnapshotDictionary(player.Inventory.allSlots, prefixInventory);
        }
    }

    [HarmonyPostfix]
    static void Postfix(Placeable_Extractor __instance, Network_Player player)
    {
        try
        {
            if (ModConfig.configData.Settings[0].TryGetValue(ConfigData.ExtractorExpKey, out bool isOn))
            {
                if (!isOn)
                    return;
            }

            if (player.Inventory != null)
            {
                postfixInventory.Clear();
                ES_Tools.CreateInventorySnapshotDictionary(player.Inventory.allSlots, postfixInventory);
                Dictionary<ItemInstance, int> items = ES_Tools.ItemDifferenceFromInventory(prefixInventory, postfixInventory);

                foreach (var (instance, quantity) in items)
                {
                    Experience_Bar.AddExperienceFromItem(instance, quantity, ModConfig.configData.Extractor_Exp);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in farming postfix: {e.Message}");
        }
    }
}

[HarmonyPatch(typeof(BeeHive), "HarvestYield")]
class Patch_BeeHive_HarvestYield
{
    private static Dictionary<ItemInstance, int> prefixInventory = new Dictionary<ItemInstance, int>();
    private static Dictionary<ItemInstance, int> postfixInventory = new Dictionary<ItemInstance, int>();

    [HarmonyPrefix]
    static void Prefix(BeeHive __instance, Network_Player player)
    {
        if (player.Inventory != null)
        {
            prefixInventory.Clear();
            ES_Tools.CreateInventorySnapshotDictionary(player.Inventory.allSlots, prefixInventory);
        }
    }

    [HarmonyPostfix]
    static void Postfix(Placeable_Extractor __instance, Network_Player player)
    {
        try
        {
            if (ModConfig.configData.Settings[0].TryGetValue(ConfigData.BeehivesExpKey, out bool isOn))
            {
                if (!isOn)
                    return;
            }

            if (player.Inventory != null)
            {
                postfixInventory.Clear();
                ES_Tools.CreateInventorySnapshotDictionary(player.Inventory.allSlots, postfixInventory);
                Dictionary<ItemInstance, int> items = ES_Tools.ItemDifferenceFromInventory(prefixInventory, postfixInventory);

                foreach (var (instance, quantity) in items)
                {
                    Experience_Bar.AddExperienceFromItem(instance, quantity, ModConfig.configData.Beehives_Exp);
                }
            }
        }

        catch (Exception e)
        {
            Debug.LogError($"Error in farming postfix: {e.Message}");
        }
    }
}