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
        experienceBar = new Experience_Bar(asset);
        Experience_Bar.expBar = Instantiate(Experience_Bar.expBar, RAPI.GetLocalPlayer().canvas.transform);

        savedData = LoadData();

        if (savedData != null)
        {
            savedData.LoadIntoExpBar(experienceBar);
        }
        else
        {
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
    }

    public override void LocalPlayerEvent_PickupItem(PickupItem item)
    {
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

[HarmonyPatch(typeof(FishingRod), "PullItemsFromSea")]
class Patch_FishingRod_PullItemsFromSea
{
    private static Dictionary<ItemInstance, int> prefixInventory = new Dictionary<ItemInstance, int>();
    private static Dictionary<ItemInstance, int> postfixInventory = new Dictionary<ItemInstance, int>();

    [HarmonyPrefix]
    static void Prefix(FishingRod __instance)
    {
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
                    Experience_Bar.AddExperienceFromItem(instance, quantity, ConfigEventType.FISHING);
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
                    Experience_Bar.AddExperienceFromItem(instance, quantity, ConfigEventType.TREE_HARVEST);
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
        try
        {
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

            if (playerInventory != null)
            {
                postfixInventory.Clear();
                ES_Tools.CreateInventorySnapshotDictionary(playerInventory.allSlots, postfixInventory);
                Dictionary<ItemInstance, int> items = ES_Tools.ItemDifferenceFromInventory(prefixInventory, postfixInventory);

                foreach (var (instance, quantity) in items)
                {
                    // NOTE: barrels and crates wont be flagged here but could use localplayerpickup
                    if (!item.isDropped)
                        Experience_Bar.AddExperienceFromItem(instance, quantity, ConfigEventType.PICKUP);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in fishing postfix: {e.Message}");
        }
    }

}

// Patch for when making deals
// Patch for when harvesting animal bodies
// Digging ( this happens on pickup tho )