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
            pickupExp = new Dictionary<string, float>
            {
                { "Leaf", 1 },
                { "Palm Leaf", 1 },
                { "Plastic", 1 },
                { "Plank", 1 },
                { "Barrel", 10},
                { "Crate", 20},
            },

            mobKillExp = new Dictionary<string, float>
            {
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
}