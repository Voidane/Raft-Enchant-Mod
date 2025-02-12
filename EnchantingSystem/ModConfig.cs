using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
        configData = ConfigData.CreateConfig();
        File.WriteAllText(path, JsonConvert.SerializeObject(configData, Formatting.Indented));
    }

    private void LoadConfigFile(string path)
    {
        configData = JsonConvert.DeserializeObject<ConfigData>(File.ReadAllText(path));

        if (configData == null)
            Debug.LogError($"{EnchantingSystem.MOD_NAME} An error occured while trying to load the config file! Is it corrupt?");
    }


    public static bool IsConfigEventType(Dictionary<string, float> _event, string _name, int quantity, out float exp)
    {
        exp = 0.0F;

        if (_event.TryGetValue(_name, out exp))
        {
            exp *= quantity;
            Debug.Log($"{EnchantingSystem.MOD_NAME} {nameof(_event)} did give exp for {_name} for amount: {quantity} , exp: {exp}");
            return true;
        }
        else
        {
            Debug.Log($"{EnchantingSystem.MOD_NAME} {nameof(_event)} did not have {_name} for amount: {quantity} , exp: {exp}");
        }
        return false;
    }
}

public enum ConfigEventType
{
    PICKUP,
    FISHING,
    MOB_KILLING,
    TREE_HARVEST
}