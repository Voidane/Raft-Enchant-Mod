using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ModConfig
{
    public static ConfigData configData;

    public ModConfig()
    {
        LoadOrCreateConfig();
    }

    private void LoadOrCreateConfig()
    {
        try
        {
            string _path = Path.Combine(EnchantingSystem.ModDataPath(), "config.json");

            if (File.Exists(_path))
            {
                LoadConfigFile(_path);
            }
            else
            {
                CreateDefaultConfig(_path);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"{EnchantingSystem.MOD_NAME} An error occured while trying to create or find the config.json file! {e.Message}");
        }
    }

    private void CreateDefaultConfig(string path)
    {
        configData = new ConfigData
        {
            Pickup_Exp = new Dictionary<string, float>
            {
                { "leaf", 1.0F },
                { "palm_leaf", 1.0F },
                { "plastic", 1.0F },
                { "plank", 1.0F },
                { "barrel", 10.0F },
                { "crate", 20.0F },
                { "pineapple", 1.5F },
                { "scrap", 1.0F },
                { "raw_potato", 1.5F },
                { "stone", 1.0F },
                { "clay", 1.0F },
                { "sand", 1.0F },
                { "metal_ore", 4.0F },
                { "copper", 4.0F },
                { "watermelon", 1.5F },
                { "red_barries", 1.0F },
                { "cave_mushroom", 1.0F },
                { "thatch", 1.0F },
                { "mystery_package", 5.0F },
                { "rope", 3.0F },

            },

            Mob_Kill_Exp = new Dictionary<string, float>
            {
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
                { "coconut", 3.5F }
            }
        };

        File.WriteAllText(path, JsonConvert.SerializeObject(configData, Formatting.Indented));
    }

    private void LoadConfigFile(string path)
    {
        configData = JsonConvert.DeserializeObject<ConfigData>(File.ReadAllText(path));

        if (configData == null)
            Debug.LogError($"{EnchantingSystem.MOD_NAME} An error occured while trying to load the config file! Is it corrupt?");
    }


    public static bool IsConfigEventType(ConfigEventType typeEvent, string _name, int quantity, out float exp)
    {
        exp = 0.0F;

        switch (typeEvent)
        {
            case ConfigEventType.PICKUP:
                if (configData.Pickup_Exp.TryGetValue(_name, out exp))
                {
                    exp *= quantity;
                    Debug.Log($"{EnchantingSystem.MOD_NAME} {nameof(ConfigEventType.PICKUP)} did give exp for {_name} for amount: {quantity} , exp: {exp}");
                    return true;
                }
                else
                {
                    Debug.Log($"{EnchantingSystem.MOD_NAME} {nameof(ConfigEventType.PICKUP)} did not have {_name} for amount: {quantity} , exp: {exp}");
                }
                return false;

            case ConfigEventType.FISHING:
                if (configData.Fishing_Exp.TryGetValue(_name, out exp))
                {
                    exp *= quantity;
                    Debug.Log($"{EnchantingSystem.MOD_NAME} {nameof(ConfigEventType.FISHING)} did give exp for {_name} for amount: {quantity} , exp: {exp}");
                    return true;
                }
                else
                {
                    Debug.Log($"{EnchantingSystem.MOD_NAME} {nameof(ConfigEventType.FISHING)} did not have {_name} for amount: {quantity} , exp: {exp}");
                }
                return false;

            case ConfigEventType.MOB_KILLING:
                if (configData.Mob_Kill_Exp.TryGetValue(_name, out exp))
                {
                    exp *= quantity;
                    Debug.Log($"{EnchantingSystem.MOD_NAME} {nameof(ConfigEventType.MOB_KILLING)} did give exp for {_name} for amount: {quantity} , exp: {exp}");
                    return true;
                }
                else
                {
                    Debug.Log($"{EnchantingSystem.MOD_NAME} {nameof(ConfigEventType.MOB_KILLING)} did not have {_name} for amount: {quantity} , exp: {exp}");
                }
                return false;

            case ConfigEventType.TREE_HARVEST:
                if (configData.Mob_Kill_Exp.TryGetValue(_name, out exp))
                {
                    exp *= quantity;
                    Debug.Log($"{EnchantingSystem.MOD_NAME} {nameof(ConfigEventType.TREE_HARVEST)} did give exp for {_name} for amount: {quantity} , exp: {exp}");
                    return true;
                }
                else
                {
                    Debug.Log($"{EnchantingSystem.MOD_NAME} {nameof(ConfigEventType.TREE_HARVEST)} did not have {_name} for amount: {quantity} , exp: {exp}");
                }
                return false;

            default:
                Debug.Log($"{EnchantingSystem.MOD_NAME} Unknown event type {typeEvent}");
                return false;
        }
    }
}

public enum ConfigEventType
{
    PICKUP,
    FISHING,
    MOB_KILLING,
    TREE_HARVEST
}