using RaftModLoader;
﻿using UnityEngine;
using HMLLibrary;
using System.Collections;

public class EnchantingSystem : Mod
{

    private CanvasHelper userGameInterface;
    public static AssetBundle asset;

    private Experience_Bar experienceBar;

    private bool hasWorldBeenLoaded;
    
    public IEnumerator Start()
    {
        Debug.Log("Mod EnchantingSystem has been loaded!");

        AssetBundleCreateRequest request = AssetBundle.LoadFromMemoryAsync(GetEmbeddedFileBytes("enchantingsystem.assets"));
        yield return request;

        asset = request.assetBundle;

        experienceBar = new Experience_Bar(asset);

        Debug.Log("Enchanting system hasnt crashed yet...");
    }

    public void OnModUnload()
    {
        asset.Unload(true);
        Debug.Log("Mod EnchantingSystem has been unloaded!");
    }

    public override void WorldEvent_WorldLoaded()
    {
        Debug.Log("Loaded in asset");
        if (!hasWorldBeenLoaded)
        {
            hasWorldBeenLoaded = true;
            experienceBar.expBar = Instantiate(experienceBar.expBar, RAPI.GetLocalPlayer().canvas.transform);
            experienceBar.Resetup();
        }

        experienceBar.Enable();
    }

    public override void WorldEvent_WorldUnloaded()
    {
        experienceBar.Disable();
    }


}