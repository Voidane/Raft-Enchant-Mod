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
    
    private ModConfig modConfig;
    private Experience_Bar experienceBar;
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
        experienceBar.expBar = Instantiate(experienceBar.expBar, RAPI.GetLocalPlayer().canvas.transform);

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
        if (experienceBar != null && experienceBar.expBar != null)
        {
            if (savedData != null)
            {
                savedData.SaveFromExpBar(experienceBar);
                SaveData(savedData);
            }
            Destroy(experienceBar.expBar);
        }
    }

    public override void LocalPlayerEvent_PickupItem(PickupItem item)
    {
        if (ModConfig.configData.pickupExp.TryGetValue(item.PickupName, out float exp))
        {
            if (!item.isDropped)
                experienceBar.AddExperience(exp);
            return;
        }

        RAPI.GetLocalPlayer().SendChatMessage("Config did not contain " + item.PickupName);
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


    public static void fishEscapeTest()
    {
        Debug.Log("FishEscapedTest");
    }
}

/// <summary>
/// Patches when the game mechanic for fishing and we recieve an even pull item from sea.
/// Depending on the catch, it will deliver an amount exp adjusted by the config.
/// </summary>
[HarmonyPatch(typeof(FishingRod), "PullItemsFromSea")]
class Patch_FishingRod_PullItemsFromSea
{
    static Dictionary<string, int> initialItems = new Dictionary<string, int>();

    [HarmonyPrefix]
    static void Prefix(FishingRod __instance)
    {
        try
        {
            // Store the state of inventory before catching
            var playerNetwork = AccessTools.Field(typeof(FishingRod), "playerNetwork").GetValue(__instance) as Network_Player;
            if (playerNetwork?.Inventory != null)
            {
                initialItems.Clear();
                foreach (var slot in playerNetwork.Inventory.allSlots)
                {
                    if (!slot.IsEmpty && slot.itemInstance != null)
                    {
                        string itemName = slot.itemInstance.UniqueName;
                        int amount = slot.GetCurrentTotalUses();

                        if (initialItems.ContainsKey(itemName))
                        {
                            initialItems[itemName] += amount;
                        }
                        else
                        {
                            initialItems[itemName] = amount;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"Error in fishing prefix: {ex.Message}");
        }
    }

    [HarmonyPostfix]
    static void Postfix(FishingRod __instance)
    {
        try
        {
            // Compare inventory after catching
            var playerNetwork = AccessTools.Field(typeof(FishingRod), "playerNetwork").GetValue(__instance) as Network_Player;
            if (playerNetwork?.Inventory != null)
            {
                var currentItems = new Dictionary<string, int>();

                // Build current inventory state
                foreach (var slot in playerNetwork.Inventory.allSlots)
                {
                    if (!slot.IsEmpty && slot.itemInstance != null)
                    {
                        string itemName = slot.itemInstance.UniqueName;
                        int amount = slot.GetCurrentTotalUses();

                        if (currentItems.ContainsKey(itemName))
                        {
                            currentItems[itemName] += amount;
                        }
                        else
                        {
                            currentItems[itemName] = amount;
                        }
                    }
                }

                // Compare quantities
                foreach (var kvp in currentItems)
                {
                    int initialQuantity = initialItems.ContainsKey(kvp.Key) ? initialItems[kvp.Key] : 0;
                    if (kvp.Value > initialQuantity)
                    {
                        Debug.Log($"Actually caught: {kvp.Key}");
                        return;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"Error in fishing postfix: {ex.Message}");
        }
    }
}