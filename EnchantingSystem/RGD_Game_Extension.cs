using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

public static class RGD_Game_Extension
{

    private static readonly ConditionalWeakTable<RGD_Game, RGD_Game_AdditionalData> data =
        new ConditionalWeakTable<RGD_Game, RGD_Game_AdditionalData>();

    public static RGD_Game_AdditionalData GetAdditionalData(this RGD_Game rgd_game)
    {
        return data.GetOrCreateValue(rgd_game);
    }

    public static void AddData(this RGD_Game rgd_game, RGD_Game_AdditionalData value)
    {
        try
        {
            data.Add(rgd_game, value);
        }
        catch (Exception)
        {

        }
    }

}
