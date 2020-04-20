using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    //-----------GameStates----------

    public enum GameState
    {
        MainMenu,
        Warmup,
        PlayerTurn,
        TurnEffect,
        Newspaper,
        Cooldown,
        EndScreen        
    }

    public GameState CurrentGameState;

    public bool win_radical = false;
    public bool win_moderate = false;
    //-----------RESOURCES-------------
    public int PowerIncrementValue;
    public int HopeDecayValue;
    #region resources;
    private int _hope;
    bool hopeFlashing = false;
    bool firstTime = true;

    [HideInInspector]
    public int Hope {
        get => _hope;
        set {
            
            _hope = value;
            if (_hope >= 100)
            {
                winConditionHope = true;
                InterfaceManager.Instance.LabelHope.text = "<color=#ffd700>" +_hope.ToString() + "<color=#ffd700>/100</color>";
            }
            else
            {
                InterfaceManager.Instance.LabelHope.text = _hope.ToString() + "<color=#ffd700>/100</color>";
            }
        }
    }
    public int HopeDefault;
    public int PredictHope;
         

    private int _power;
    [HideInInspector]
    public int Power
    {
        get => _power;
        set
        {
            _power = value;
            InterfaceManager.Instance.LabelPower.text = _power.ToString();
        }
    }
    public int PowerDefault;
    public int PredictPower;

    private int _funds;
    [HideInInspector]
    public int Funds
    {
        get => _funds;
        set
        {
            if (value < 0)
            {
                value = 0; //END GAME TRIGGER
            }
            _funds = value;
            InterfaceManager.Instance.LabelFunds.text = _funds.ToString();
        }
    }
    public int FundsDefault;
    public int PredictFunds;


    private int _fRadical;
    [HideInInspector]
    public int FollowersRadical
    {
        get => _fRadical;
        set
        {
            _fRadical = value;

            if (winConditionHope)
            {
                if (_fRadical >= 100)
                {
                    InterfaceManager.Instance.LabelFollowersRadical.text = "<color=#ffd700>" + _fRadical.ToString() + "<color=#ffd700>/200</color>";
                }
                else
                {
                    InterfaceManager.Instance.LabelFollowersRadical.text = _fRadical.ToString() + "<color=#ffd700>/200</color>";
                }
            }
            else InterfaceManager.Instance.LabelFollowersRadical.text = _fRadical.ToString() ;
        }
    }
    public int FollowersRadicalDefault;
    public int PredictFRad;


    private int _fModerate;
    [HideInInspector]
    public int FolowersModerate
    {
        get => _fModerate;
        set
        {
            _fModerate = value;
            if (winConditionHope)
            {
                if (_fModerate >=1000)
                {
                    InterfaceManager.Instance.LabelFollowersModerate.text = "<color=#ffd700>" + _fModerate.ToString() + "<color=#ffd700>/500</color>";
                }
                else
                {
                    InterfaceManager.Instance.LabelFollowersModerate.text = _fModerate.ToString() + "<color=#ffd700>/500</color>";
                }
            }
            else InterfaceManager.Instance.LabelFollowersModerate.text = _fModerate.ToString();
            
        }
    }
    public int FolowersModerateDefault;
    public int PredictFmod;

    private int _visibility;
    [HideInInspector]
    public int Visibility
    {
        get => _visibility;
        set
        {
            
            _visibility = value;
            InterfaceManager.Instance.LabelVisibility.text = _visibility.ToString() + "%";
        }
    }
    public int VisibilityDefault;
    public int PredictVisibility;

    #endregion
    public List<Card> TableCards = new List<Card>();
    public List<Card> StaticCards = new List<Card>();

    public int HandCount;

    private void Start()
    {
        CurrentGameState = GameState.MainMenu;
        InterfaceManager.Instance.ShowMainMenu();
    }

    public void OnNewGameClicked()
    {
        InterfaceManager.Instance.ShowGame();
        ResetGame();
        if (firstTime)
        {
            InterfaceManager.Instance.ShowNewspaper();
            firstTime = false;
        }
        else
        {
            InitiateWarmup();
        }
    }

    public void InitiateWarmup()
    {
        CurrentGameState = GameState.Warmup;
        CleanPredictions();
        StartCoroutine(WarmupRoutine());
        InterfaceManager.Instance.UpdateOverlay();
    }
    
    private IEnumerator WarmupRoutine()
    {
        PredictHope -= HopeDecayValue;
        PredictPower += PowerIncrementValue;
        Power = PowerDefault;
        UpdatePredictions();
        UpdateStaticPredictions();
        for (int i = HandCount; i < 5; i++)
        {
            Deck.Instance.GenerateCard();
            HandCount++;

            yield return new WaitForSecondsRealtime(0.2f);
        }
        //display diary

        InterfaceManager.Instance.TurnAcceptButtn.interactable = true;
        CurrentGameState = GameState.PlayerTurn;
    }


    public void OnTurnAcceptClicked()
    {
        InterfaceManager.Instance.TurnAcceptButtn.interactable = false;
        if (CurrentGameState != GameState.PlayerTurn) return;
        ProcessTurn();
    }
    public GameObject PItemPrefab;
    
    public void GoNewspaper()
    {
        
        CurrentGameState = GameState.Newspaper;
        //InterfaceManager.Instance.ShowNewspaper();
        InterfaceManager.Instance.CleanTable();
        InitiateWarmup();
    }

    public void OnNewspaperOKClicked()
    {
        CurrentGameState = GameState.Cooldown;
        InterfaceManager.Instance.HideNewspaper();
        
    }


    private void ProcessTurn()
    {
        Hope -= HopeDecayValue;
        
        foreach (Card item in TableCards)
        {
            if (item.Rarity == -1)
            {
                WinGame(false);
                return;
            }
            if (item.Rarity == -2)
            {
                WinGame(true);
                return;
            }
            Power += item.CostPower;
            Funds += item.CostFund;

            Hope += item.EffectHope;
            Power += item.EffectPower;
            Funds += item.EffectFunds;
            FollowersRadical += item.EffectFollowersRadical; 
            FolowersModerate += item.EffectFollowersModerate;
            Visibility += item.EffectVisibility;
        }
        foreach (Card item in StaticCards)
        {
            Power += item.CostPower;
            Funds += item.CostFund;

            Hope += item.EffectHope;
            Power += item.EffectPower;
            Funds += item.EffectFunds;
            FollowersRadical += item.EffectFollowersRadical;
            FolowersModerate += item.EffectFollowersModerate;
            Visibility += item.EffectVisibility;
        }
        if (Power < 0) Power = 0;
        if (Funds < 0) Funds = 0;
        if (FollowersRadical < 0) FollowersRadical = 0;
        if (FolowersModerate < 0) FolowersModerate = 0;
        if (Visibility < 0) Visibility = 0;
        if (Hope <= 0)
        {
            LoseGame(2);
        }
        else if(Visibility >= 100)
        {
            LoseGame(1);
        }
        else
        {
            CleanPredictions();
            GoNewspaper();
        }


        //StartCoroutine(InterfaceManager.Instance.TurnEffectCoroutine());
    }

    public bool winConditionHope = false;
    
    public void WinGame(bool isRadical)
    {
        //TODO WIN PARAMETERS
        CurrentGameState = GameState.EndScreen;
        int type = isRadical ? -1 : -2;
        InterfaceManager.Instance.ShowEndScreen(type);
    }

    public void LoseGame(int type)
    {
        //TODO LOSE PARAMETERS
        CurrentGameState = GameState.EndScreen;
        ResetGame();
        InterfaceManager.Instance.ShowEndScreen(type);
    }

    private void ResetGame()
    {
        winConditionHope = false;
        HandCount = 0;
        Hope = HopeDefault;
        Power = PowerDefault;
        Funds = FundsDefault;
        FolowersModerate = FolowersModerateDefault;
        FollowersRadical = FollowersRadicalDefault;
        Hope = HopeDefault;
        Visibility = VisibilityDefault;
        TableCards.Clear();
        StaticCards.Clear();
        InterfaceManager.Instance.OccupiedTableSlots.Clear();
        InterfaceManager.Instance.OccupiedStaticSlots.Clear();
        Transform c = null;
        while(InterfaceManager.Instance.HandParentPanel.childCount > 0)
        {
            c = InterfaceManager.Instance.HandParentPanel.GetChild(0);
            Debug.Log(c.name);
            c.SetParent(null);
            DestroyImmediate(c.gameObject);
        }
        foreach (var item in InterfaceManager.Instance.TableSlots)
        {
            if (item.childCount > 0)
            {
                Transform card = null;
                for (int j = 0; j < item.childCount; j++)
                {
                    if (item.GetChild(j).name == "Card(Clone)")
                    {
                        card = item.GetChild(j);
                        card.SetParent(null);
                        Destroy(card.gameObject);
                    }
                }
            }
        }
        foreach (var item in InterfaceManager.Instance.StaticSlots)
        {
            if (item.childCount > 1)
            {
                Transform card = null;
                for (int j = 0; j < item.childCount; j++)
                {
                    if (item.GetChild(j).name == "Card(Clone)")
                    {
                        card = item.GetChild(j);
                        card.SetParent(null);
                        Destroy(card.gameObject);
                    }
                }
            }
        }
            

        win_radical = false;
        win_moderate = false;
        Deck.Instance.ResetDeck();
    }

    public void CleanPredictions()
    {
        PredictPower = 0;
        PredictFunds = 0;
        PredictFRad = 0;
        PredictFmod = 0;
        PredictVisibility = 0;
        PredictHope = 0;
        UpdatePredictions();
    }
    public void UpdatePredictions()
    {
        string prefixGreen = "<color=green>";
        string prefixRed = "<color=red>";
        string suffix = "</color>";

        // MAYBE ADD blink?

        bool isBlinking;
        InterfaceManager.Instance.LabelPredictPower.text = PredictPower == 0 ? "" : 
            PredictPower < 0 ? prefixRed + PredictPower.ToString() + suffix : prefixGreen + "+" + PredictPower.ToString() + suffix;

        InterfaceManager.Instance.LabelPredictFunds.text = PredictFunds == 0 ? "" : 
            PredictFunds < 0 ? prefixRed + PredictFunds.ToString() + suffix : prefixGreen + "+" + PredictFunds.ToString() + suffix;

        InterfaceManager.Instance.LabelPredictFollowersRadical.text = PredictFRad == 0 ? "" :
            PredictFRad < 0 ? prefixRed + PredictFRad.ToString() + suffix : prefixGreen + "+" + PredictFRad.ToString() + suffix;

        InterfaceManager.Instance.LabelPredictFollowersModerate.text = PredictFmod == 0 ? "" :
            PredictFmod < 0 ? prefixRed + PredictFmod.ToString() + suffix : prefixGreen + "+" + PredictFmod.ToString() + suffix;

        InterfaceManager.Instance.LabelPredictVisibility.text = PredictVisibility == 0 ? "" : 
            PredictVisibility < 0 ? prefixGreen + PredictVisibility.ToString() + suffix : prefixRed + "+" + PredictVisibility.ToString() + suffix;

        InterfaceManager.Instance.LabelPredictHope.text = PredictHope == 0 ? "" :
            PredictHope < 0 ? prefixRed + PredictHope.ToString() + suffix : prefixGreen + "+" + PredictHope.ToString() + suffix;
    } 

    public void UpdateStaticPredictions()
    {
        foreach (var slot in InterfaceManager.Instance.StaticSlots)
        {
            if (slot.childCount == 2) continue;

            Card CardData = slot.GetComponentsInChildren<CardView>()[0].CardData;

            PredictFmod += CardData.EffectFollowersModerate;
            PredictFRad += CardData.EffectFollowersRadical;
            PredictFunds += CardData.CostFund;
            PredictPower += CardData.CostPower;
            PredictFunds += CardData.EffectFunds;
            PredictPower += CardData.EffectPower;
            PredictVisibility += CardData.EffectVisibility;
            PredictHope += CardData.EffectHope;

        }
        UpdatePredictions();
    }
    public void UpdateStaticValues()
    {
        foreach (var slot in InterfaceManager.Instance.StaticSlots)
        {
            if (slot.childCount == 2) continue;

            Card CardData = slot.GetComponentsInChildren<CardView>()[0].CardData;

            FolowersModerate += CardData.EffectFollowersModerate;
            FollowersRadical += CardData.EffectFollowersRadical;
            Funds += CardData.EffectFunds;
            Power += CardData.EffectPower;
            Visibility += CardData.EffectVisibility;
            Hope += CardData.EffectHope;

            CardData.CostFund = 0;
            slot.GetComponentsInChildren<CardView>()[0].LabelFundsCost.text = CardData.CostFund.ToString();
            CardData.CostPower = 0;
            slot.GetComponentsInChildren<CardView>()[0].LabelPoweCost.text = CardData.CostPower.ToString();
        }
        UpdatePredictions();
    }
    Color c = new Color32(24, 24, 24, 255);




}
