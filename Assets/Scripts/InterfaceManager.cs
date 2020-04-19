using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InterfaceManager : MonoBehaviour
{
    #region singleton
    public static InterfaceManager Instance;
    private void Awake()
    {
        Instance = this;
    }
    #endregion

    private Canvas currentCanvas;

    public Canvas GameCanvas;
    public Canvas MainMenuCanvas;
    public Canvas EndGameCanvas;

    public GameObject Newspaper;

    public List<RectTransform> TableSlots = new List<RectTransform>();
    public List<RectTransform> StaticSlots = new List<RectTransform>();


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

    public List<string> monthses = new List<string>();

    public void ShowMainMenu()
    {
        currentCanvas.gameObject.SetActive(false);
        MainMenuCanvas.gameObject.SetActive(true);
    }
    public void ShowGame()
    {
        currentCanvas.gameObject.SetActive(false);
        GameCanvas.gameObject.SetActive(true);

    }
    public void ShowEndScreen()
    {

        currentCanvas.gameObject.SetActive(false);
        EndGameCanvas.gameObject.SetActive(true);
    }

    public IEnumerator TurnEffectCoroutine()
    {
        yield return new WaitForSecondsRealtime(1f);

        GameManager.Instance.CleanPredictions();
        GameManager.Instance.GoNewspaper();
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
                var card = item.GetChild(1);
                card.SetParent(null);
                GameManager.Instance.TableCards.Remove(card.GetComponent<CardView>().CardData);
                OccupiedTableSlots.Remove(item);
                Destroy(card.gameObject);
            }
        }
    }

    public IEnumerator ShowAfterTurnEffects()
    {
        yield return new WaitForSecondsRealtime(1f);
        GameManager.Instance.InitiateWarmup();
    }


    private void Start()
    {
        currentCanvas = MainMenuCanvas;
    }
}
