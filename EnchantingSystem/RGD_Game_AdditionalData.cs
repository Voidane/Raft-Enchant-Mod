using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class RGD_Game_AdditionalData
{
    public int level;
    public float totalExp;
    public int maxExp;

    public RGD_Game_AdditionalData()
    {
        level = 0;
        totalExp = 0;
        maxExp = 100;
    }

    public void SaveFromExpBar(Experience_Bar expBar)
    {
        Debug.Log($"Saving levels: {expBar.level}, max: {expBar.maxExp}, total: {expBar.totalExp}");
        level = expBar.level;
        totalExp = expBar.totalExp;
        maxExp = expBar.maxExp;
    }

    public void LoadIntoExpBar(Experience_Bar expBar)
    {
        Debug.Log($"Loading in levels: {level}, max: {maxExp}, total: {totalExp}");
        expBar.level = level;
        expBar.maxExp = maxExp;
        expBar.totalExp = totalExp;
        expBar.UpdatePercentage(totalExp, maxExp);
        expBar.UpdateExpBarValue();
    }
}