using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InterfaceManager : MonoBehaviour
{
    #region singleton
    public static InterfaceManager Instance;
    private void Awake()
    {
        Instance = this;
    }
    #endregion

    public Canvas currentCanvas;

    public Canvas GameCanvas;
    public Canvas MainMenuCanvas;
    public Canvas EndGameCanvas;
    public Canvas WinGameCanvas;

    public GameObject Newspaper;
    public Button TurnAcceptButtn;

    public List<RectTransform> TableSlots = new List<RectTransform>();
    public List<RectTransform> StaticSlots = new List<RectTransform>();

    public RectTransform GarbageSlot;


    public List<RectTransform> OccupiedTableSlots = new List<RectTransform>();
    public List<RectTransform> OccupiedStaticSlots = new List<RectTransform>();
 
    public RectTransform HandDropZone;
    public RectTransform HandParentPanel;

    public TMP_Text LabelPower;
    public TMP_Text LabelFunds;
    public TMP_Text LabelFollowersRadical;
    public TMP_Text LabelFollowersModerate;
    public TMP_Text LabelVisibility;
    public TMP_Text LabelHope;

    public TMP_Text LabelPredictPower;
    public TMP_Text LabelPredictFunds;
    public TMP_Text LabelPredictFollowersRadical;
    public TMP_Text LabelPredictFollowersModerate;
    public TMP_Text LabelPredictVisibility;
    public TMP_Text LabelPredictHope;

    public GameObject PyshschItemPrefab;


    public TMP_Text LoseScreenText;
    public TMP_Text LoseNewsLabel;
    public TMP_Text LoseNewsPiece;

    public List<string> monthses = new List<string>();

    public Image overlayHolder;
    bool jokeComplete = false;
    public Sprite normalOverlay;
    public Sprite jokeOverlay;
    public Sprite novemberOverlay;
    public TMP_Text monthLabel;

    public void ShowMainMenu()
    {
        currentCanvas.gameObject.SetActive(false);
        MainMenuCanvas.gameObject.SetActive(true);
    }
    public void ShowGame()
    {
        monthCounter = 0;
        currentCanvas.gameObject.SetActive(false);
        GameCanvas.gameObject.SetActive(true);
        currentCanvas = GameCanvas;


    }
    public void ShowEndScreen(int endType)
    {
        //-2 win radical
        //-1 win moderate
        //1 lose visibility
        //2 all hope is lost
        currentCanvas.gameObject.SetActive(false);
        switch (endType)
        {
            case 1:
                LoseNewsLabel.text = "GVRNMNT CRACKDOWN";
                LoseNewsPiece.text = "A series of GVRMNT actions today leave hundreds detained, some injured as the so - " +
                    "called revolutionary movement overstepped all bounds of civil protest and was classified a terrorist organisation and a danger to society.\nMore on page 6.";
                LoseScreenText.text = "Never underestimate the power of The Big Brother. We shall try again, for a different cause";

                EndGameCanvas.gameObject.SetActive(true);
                currentCanvas = EndGameCanvas;

                break;
            case 2:
                LoseNewsLabel.text = "All is well";
                LoseNewsPiece.text = "a National Zoo moose Rudolf, previously believed to be a fat male, gave birth to twin moose-lings today.More on Rudolf's predicament on page 2\n" +
                    "In other news: \"Revolutionary\" flees country: Known terrorist and malcontent seen today crossing the border as the so-called \"Cause\" fails to attract any attention or supporters.\n";

                LoseScreenText.text = "The future is grim. All hope is lost.";

                EndGameCanvas.gameObject.SetActive(true);
                currentCanvas = EndGameCanvas;

                break;

            case -1:
                WinGameCanvas.gameObject.SetActive(true);
                currentCanvas = WinGameCanvas;

                break;

            case -2:
                WinGameCanvas.gameObject.SetActive(true);
                currentCanvas = WinGameCanvas;

                break;
            default:
                break;
        }
    }


    public void ShowNewspaper()
    {
        Newspaper.SetActive(true);
    }
    public void HideNewspaper() 
    {
        Newspaper.SetActive(false);
    }
    public void CleanTable()
    {
        foreach (var item in TableSlots)
        {
            if(item.childCount > 1)
            {
                Transform card = null;
                for (int j = 0; j < item.childCount; j++)
                {
                    if (item.GetChild(j).name == "Card(Clone)")
                    {
                        card = item.GetChild(j);
                    }
                }
                card.SetParent(null);
                GameManager.Instance.TableCards.Remove(card.GetComponent<CardView>().CardData);
                OccupiedTableSlots.Remove(item);
                Destroy(card.gameObject);
            }
        }
    }

    public void Exit()
    {
        Application.Quit();
    }


    private void Start()
    {
        currentCanvas = MainMenuCanvas;
    }

    int monthCounter = 0;
    public void UpdateOverlay()
    {
        Debug.Log("MC1 " + monthCounter);
        overlayHolder.sprite = normalOverlay;
        if (!jokeComplete && monthCounter == 4)
        {
            monthLabel.text = "Diary: " + monthses[monthCounter];
            overlayHolder.sprite = jokeOverlay;
            jokeComplete = true;
        }
        else
        {
            monthLabel.text = "Diary: " + monthses[monthCounter];
            if (monthCounter == 10)
            {
                overlayHolder.sprite = novemberOverlay;
            }
            Debug.Log("MC4 " + monthCounter);
            monthCounter++;
            Debug.Log("MC5 " + monthCounter);
        }
        if (monthCounter == 14) monthCounter = 0;

    }
    
}
