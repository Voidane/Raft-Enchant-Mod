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
        Debug.Log($"Saving levels: {Experience_Bar.level}, max: {Experience_Bar.maxExp}, total: {Experience_Bar.totalExp}");
        level = Experience_Bar.level;
        totalExp = Experience_Bar.totalExp;
        maxExp = Experience_Bar.maxExp;
    }

    public void LoadIntoExpBar(Experience_Bar expBar)
    {
        Debug.Log($"Loading in levels: {level}, max: {maxExp}, total: {totalExp}");
        Experience_Bar.level = level;
        Experience_Bar.maxExp = maxExp;
        Experience_Bar.totalExp = totalExp;
        Experience_Bar.UpdatePercentage(totalExp, maxExp);
        Experience_Bar.UpdateExpBarValue();
    }
}