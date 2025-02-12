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

public class EnchantingSystem : Mod
{
    public static readonly string MOD_NAME = "[Enchanting System]";

    public static AssetBundle asset;
    public static Harmony harmony;
    public static Experience_Bar experienceBar;

    private ModConfig modConfig;
    private RGD_Game_AdditionalData savedData;

    public IEnumerator Start()
    {
        AssetBundleCreateRequest request = AssetBundle.LoadFromMemoryAsync(GetEmbeddedFileBytes("enchantingsystem.assets"));
        SceneManager.sceneLoaded += OnSceneLoaded;

        ModDataPath();
        modConfig = new ModConfig();
        
        harmony = new Harmony("com.voidane.enchantingsystem");
        harmony.PatchAll();

        yield return request;

        asset = request.assetBundle;

        Debug.Log($"{MOD_NAME} Has been loaded.");
    }

    public void OnModUnload()
    {
        asset.Unload(true);
        harmony.UnpatchAll();
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Debug.Log($"{MOD_NAME} Has Been Unloaded!");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
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
                    RAPI.GetLocalPlayer().SendChatMessage("Quest item: " + item.pickupTerm);
                    Experience_Bar.AddExperience(exp);
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
                        RAPI.GetLocalPlayer().SendChatMessage("Added: " + item.PickupName);
                        Experience_Bar.AddExperienceFromItem(instance, quantity, ModConfig.configData.Pickup_Exp);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in fishing postfix: {e.Message}");
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
        Experience_Bar.AddExperienceFromMob(networkBehaviour.behaviourType, 1, ModConfig.configData.Mob_Kill_Exp);
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
            Experience_Bar.AddExperienceFromItem(result, 1, ModConfig.configData.Cooking_Exp);
            return;
        }
        else if (current_recipe.RecipeType == CookingRecipeType.Juicer)
        {
            Experience_Bar.AddExperienceFromItem(result, 1, ModConfig.configData.Juicing_Exp);
            return;
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
            Experience_Bar.AddExperienceFromItem(item, 1, ModConfig.configData.Grilling_Exp);
        else if (ModConfig.configData.Smelting_Exp.ContainsKey(_name))
            Experience_Bar.AddExperienceFromItem(item, 1, ModConfig.configData.Smelting_Exp);

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
        RAPI.GetLocalPlayer().SendChatMessage("Crafted " + item.UniqueName);
        return true;
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
        if (ModConfig.configData.Mystery_Package_Exp.TryGetValue("all", out float exp))
        {
            Experience_Bar.AddExperience(exp);
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