using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public int HopeDecayValue;
    #region resources;
    private int _hope;
    [HideInInspector]
    public int Hope {
        get => _hope;
        set {
            _hope = value;
            InterfaceManager.Instance.LabelHope.text = _hope.ToString();
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
            InterfaceManager.Instance.LabelFollowersRadical.text = _fRadical.ToString();
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
            InterfaceManager.Instance.LabelFollowersModerate.text = _fModerate.ToString();
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
            InterfaceManager.Instance.LabelVisibility.text = _visibility.ToString();
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
        InitiateWarmup();
    }

    public void InitiateWarmup()
    {
        CurrentGameState = GameState.Warmup;
        CleanPredictions();
        StartCoroutine(WarmupRoutine());

    }
    
    private IEnumerator WarmupRoutine()
    {
        PredictHope -= HopeDecayValue;
        UpdatePredictions();
        for (int i = HandCount; i < 5; i++)
        {
            Deck.Instance.GenerateCard();
            HandCount++;

            yield return new WaitForSecondsRealtime(0.2f);
        }
        //display diary

        CurrentGameState = GameState.PlayerTurn;
    }


    public void OnTurnAcceptClicked()
    {
        if (CurrentGameState != GameState.PlayerTurn) return;
        ProcessTurn();
        ProcessGovAI();

        StartCoroutine(InterfaceManager.Instance.TurnEffectCoroutine());

    }

    public void GoNewspaper()
    {
        CurrentGameState = GameState.Newspaper;
        InterfaceManager.Instance.ShowNewspaper();
    }

    public void OnNewspaperOKClicked()
    {
        CurrentGameState = GameState.Cooldown;
        AddGovAIEffects();
        InterfaceManager.Instance.HideNewspaper();
        InterfaceManager.Instance.CleanTable();
        StartCoroutine(InterfaceManager.Instance.ShowAfterTurnEffects());
        
    }


    private void ProcessTurn()
    {
       
        foreach (Card item in TableCards)
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
        //TRIGGERS

        if (Power < 0) Power = 0;
        if (Funds < 0) Funds = 0;
        if (FollowersRadical < 0) FollowersRadical = 0;
        if (FolowersModerate < 0) FollowersRadical = 0;
        if (Visibility < 0) Power = 0;
        
        ProcessGovAI();
    }

    private void ProcessGovAI()
    {

    }

    private void AddGovAIEffects()
    {

    }

    public void WinGame(bool isRadical)
    {
        //TODO WIN PARAMETERS
        CurrentGameState = GameState.EndScreen;
        InterfaceManager.Instance.ShowEndScreen();
    }

    public void LooseGame()
    {
        //TODO LOSE PARAMETERS
        CurrentGameState = GameState.EndScreen;
        InterfaceManager.Instance.ShowEndScreen();
    }

    private void ResetGame()
    {
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
            PredictHope < 0 ? prefixRed + PredictHope.ToString() + suffix : prefixGreen + PredictHope.ToString() + suffix;
    } 


}
