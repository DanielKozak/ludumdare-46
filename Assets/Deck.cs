using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public static Deck Instance;

    private void Awake()
    {
        Instance = this;
    }

    public int ChanceRoll0 = 40;
    public int ChanceRoll1 = 30;
    public int ChanceRoll2 = 20;
    public int ChanceRoll3 = 10;

    public Card win_moderate;
    public Card win_radical;


    public List<Card> Rarity_0_Container = new List<Card>();
    public List<Card> Rarity_1_Container = new List<Card>();
    public List<Card> Rarity_2_Container = new List<Card>();
    public List<Card> Rarity_3_Container = new List<Card>();


    public GameObject CardViewPrefab;
    //TODO GENERATE RANDOM CARD BASED ON STATS

    public void GenerateCard()
    {
        CardView newCard = Instantiate(CardViewPrefab, InterfaceManager.Instance.HandParentPanel).GetComponent<CardView>();

        int chooseCounter = 1000;

        while (chooseCounter != 0)
        {
            Card newData = ChooseCard();
            if(newData != null)
            {
               newCard.SetValues(newData);
               break;
            }
            chooseCounter--;
        }
        if(chooseCounter == 0)
        {
            Debug.Log("<color=red> CANT FIND CARD</color>");
        }

        Debug.Log($"<color=green> chose {newCard.CardData.name}</color>");

    }
    private bool win_r = false;
    private bool win_m = false;
    private Card ChooseCard()
    {
        Card result;
        int CHANCE = UnityEngine.Random.Range(0, 100);
        //CHECK WIN CONDS AND SPAWN CARDS
        if (!win_r )//&& CheckCardAgainstReq(win_radical))
        {
            win_r = true;
            return win_radical;
        }
        if (!win_m && CheckCardAgainstReq(win_moderate))
        {
            win_m = true;
            return win_moderate;
        }

        //SPAWN NORMAL CARDS
        if (CHANCE > ChanceRoll0 + ChanceRoll1 + ChanceRoll2)
        {
            result = Rarity_3_Container[UnityEngine.Random.Range(0, Rarity_3_Container.Count - 1)];
        }
        else if (CHANCE > ChanceRoll0 + ChanceRoll1)
        {
            result = Rarity_2_Container[UnityEngine.Random.Range(0, Rarity_2_Container.Count - 1)];
        }
        else if (CHANCE > ChanceRoll0)
        {
            result = Rarity_1_Container[UnityEngine.Random.Range(0, Rarity_1_Container.Count - 1)];
        }
        else
        {
            result = Rarity_0_Container[UnityEngine.Random.Range(0, Rarity_0_Container.Count - 1)];
        }
            
        bool accepted = CheckCardAgainstReq(result);
        if (accepted) return result;
        else
        {
            Debug.Log("No card found");
            return null;
        }
        

    }

    private bool CheckCardAgainstReq(Card card)
    {
        if(GameManager.Instance.FolowersModerate < card.ReqFModerate)
        {
            return false;
        }
        if (GameManager.Instance.FollowersRadical < card.ReqFRadical)
        {
            return false;
        }
        if (GameManager.Instance.Funds < card.ReqFunds)
        {
            return false;
        }
        if (card.ReqHopeMin != -1 && GameManager.Instance.Hope < card.ReqHopeMin)
        {
            return false;
        }
        if (card.ReqHopeMax != -1 && GameManager.Instance.Hope > card.ReqHopeMax)
        {
            return false;
        }
        if (card.ReqVisibilityMin != -1 && GameManager.Instance.Visibility < card.ReqVisibilityMin)
        {
            return false;
        }
        if (card.ReqVisibilityMax != -1 && GameManager.Instance.Visibility > card.ReqVisibilityMax)
        {
            return false;
        }

        return true;


    }

    public void ResetDeck()
    {
        win_m = false;
        win_r = true;
    }
}
