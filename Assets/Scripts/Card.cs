using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Card : ScriptableObject
{
    public string Name;
    public string Description;
    public Texture2D Graphic;

    public int CostPower;
    public int CostFund;

    public int EffectPower;
    public int EffectFunds;
    public int EffectFollowersRadical;
    public int EffectFollowersModerate;
    public int EffectVisibility;
    public int EffectHope;

    public int Rarity;


    public int ReqFunds;
    public int ReqFRadical;
    public int ReqFModerate;
    public int ReqVisibilityMin;
    public int ReqVisibilityMax = -1;
    public int ReqHopeMin;
    public int ReqHopeMax = -1;

}
