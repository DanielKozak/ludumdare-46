using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System;

public class CardView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEndDragHandler, IDragHandler, IBeginDragHandler
{
    public Card CardData;

    public Animator CardAnimator;
    Transform Container;

    bool isOnTable = false;
    bool isBuilding = false;

    private int tableSlot;
    //--------------CARD UI-----------

    public TMP_Text LabelPoweCost;
    public TMP_Text LabelFundsCost;
    public TMP_Text LabelName;
    public TMP_Text LabelEffects;
    public TMP_Text LabelDescription;


    private Color32 r0color = new Color32(27, 138, 31, 255);
    private Color32 r1color = new Color32(127, 0, 0, 255);
    private Color32 r2color = new Color32(64, 64, 64, 255);
    private Color32 r3color = new Color32(128, 0, 128, 255);
    private Color32 rWcolor = new Color32(255, 215, 0, 255);


    void Start()
    {
        Container = transform.parent;
    }
    public void OnDrag(PointerEventData eventData)
    {

        gameObject.transform.position = (Vector3) eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        if (CardData.Rarity != 3)
        {
            foreach (var slot in InterfaceManager.Instance.TableSlots)
            {
                for (int j = 0; j < slot.childCount; j++)
                {
                    if (slot.GetChild(j).name == "highlight")
                    {
                        slot.GetChild(j).gameObject.SetActive(false);
                    }
                }
            }
        }
        else
        {
            foreach (var slot in InterfaceManager.Instance.StaticSlots)
            {
                for (int j = 0; j < slot.childCount; j++)
                {
                    if(slot.GetChild(j).name == "highlight"){
                        slot.GetChild(j).gameObject.SetActive(false);
                    } 
                }
            }
        }
        //GetComponent<LayoutElement>().ignoreLayout = false;
        int i = CheckPositionOnSlot(eventData.position);

        if (!isOnTable) //FROM HAND TO TABLE
        {
            if (i == -1)
            {
                transform.SetParent(Container);
                transform.position = startingPosition;
            }
            else if (i == 0 || i == 1 || i == 2)//ACCEPTED TABLE
            {
                if(CardData.Rarity == 3)
                {
                    transform.SetParent(Container);
                    transform.position = startingPosition;
                    return;
                }
                transform.SetParent(InterfaceManager.Instance.TableSlots[i]);
                transform.position = InterfaceManager.Instance.TableSlots[i].transform.position;
                InterfaceManager.Instance.OccupiedTableSlots.Add(InterfaceManager.Instance.TableSlots[i]);
                tableSlot = i;
                GameManager.Instance.TableCards.Add(CardData);
                GameManager.Instance.HandCount--;

                isOnTable = true;

                if (CardData.Rarity == -1)
                {
                    GameManager.Instance.WinGame(false);
                }
                if (CardData.Rarity == -2)
                {
                    GameManager.Instance.WinGame(true);
                }
                ApplyPredictions();
            }
            else if (i == 3 || i == 4)//ACCEPTED STATIC
            {
                if(CardData.Rarity != 3)
                {
                    transform.SetParent(Container);
                    transform.position = startingPosition;
                    return;
                }
                if(InterfaceManager.Instance.OccupiedStaticSlots.Contains(InterfaceManager.Instance.StaticSlots[i - 3]))
                {
                    for (int v = 0; v < InterfaceManager.Instance.StaticSlots[i - 3].childCount; v++)
                    {
                        if(InterfaceManager.Instance.StaticSlots[i - 3].GetChild(v).name == "Card(Clone)")
                        {
                            var child = InterfaceManager.Instance.StaticSlots[i - 3].GetChild(0);
                            child.SetParent(null);
                            GameManager.Instance.UpdateStaticPredictions();
                            Destroy(child.gameObject);
                        }
                    }
                }
                transform.SetParent(InterfaceManager.Instance.StaticSlots[i-3]);
                transform.SetAsLastSibling();
                transform.position = InterfaceManager.Instance.StaticSlots[i - 3].transform.position;
                InterfaceManager.Instance.OccupiedStaticSlots.Add(InterfaceManager.Instance.StaticSlots[i - 3]);
                CardData.CostFund = 0;
                CardData.CostPower = 0;
                LabelFundsCost.text = "0";
                LabelPoweCost.text = "0";
                GameManager.Instance.StaticCards.Add(CardData);
                GameManager.Instance.HandCount--;
                isOnTable = true;
                isBuilding = true;
                ApplyPredictions();
            }
            else if(i == 999) // Garbage
            {
                //RemovePredictions();
                GameManager.Instance.HandCount--;
                Destroy(gameObject);
            }
        }
        else //FROM TABLE TO HAND
        {
            if (isBuilding && i == 999)
            {
                RemovePredictions();
                Destroy(gameObject);

            }
            if (CheckPositionOnHand(eventData.position))//ACCEPTED
            {
                transform.SetParent(Container);
                InterfaceManager.Instance.OccupiedTableSlots.Remove(InterfaceManager.Instance.TableSlots[tableSlot]);
                GameManager.Instance.TableCards.Remove(CardData);
                GameManager.Instance.HandCount++;
                isOnTable = false;
                RemovePredictions();
            }
            else
            { 
                transform.position = startingPosition;
            }
        }
    }

    public void RemovePredictions()
    {
        GameManager.Instance.PredictFmod -= CardData.EffectFollowersModerate;
        GameManager.Instance.PredictFRad -= CardData.EffectFollowersRadical;
        GameManager.Instance.PredictFunds -= CardData.CostFund;
        GameManager.Instance.PredictPower -= CardData.CostPower;
        GameManager.Instance.PredictFunds -= CardData.EffectFunds;
        GameManager.Instance.PredictPower -= CardData.EffectPower;
        GameManager.Instance.PredictVisibility -= CardData.EffectVisibility;
        GameManager.Instance.PredictHope -= CardData.EffectHope;

        GameManager.Instance.UpdatePredictions();

    }

    public void ApplyPredictions()
    {
        GameManager.Instance.PredictFmod += CardData.EffectFollowersModerate;
        GameManager.Instance.PredictFRad += CardData.EffectFollowersRadical;
        GameManager.Instance.PredictFunds += CardData.CostFund;
        GameManager.Instance.PredictPower += CardData.CostPower;
        GameManager.Instance.PredictFunds += CardData.EffectFunds;
        GameManager.Instance.PredictPower += CardData.EffectPower;
        GameManager.Instance.PredictVisibility += CardData.EffectVisibility;
        GameManager.Instance.PredictHope += CardData.EffectHope;

        GameManager.Instance.UpdatePredictions();
    }

    Vector3 startingPosition;
    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.SetParent(InterfaceManager.Instance.GameCanvas.transform);
        startingPosition = transform.position;
        if (isOnTable) return;
        if (CardData.Rarity != 3)
        {
            foreach(var slot in InterfaceManager.Instance.TableSlots)
            {
                if (!InterfaceManager.Instance.OccupiedTableSlots.Contains(slot))
                {
                    for (int j = 0; j < slot.childCount; j++)
                    {
                        if (slot.GetChild(j).name == "highlight")
                        {
                            slot.GetChild(j).gameObject.SetActive(true);
                        }
                    }
                }
            }
        }
        else
        {
            foreach (var slot in InterfaceManager.Instance.StaticSlots)
            {
                for (int j = 0; j < slot.childCount; j++)
                {
                    if (slot.GetChild(j).name == "highlight")
                    {
                        slot.GetChild(j).gameObject.SetActive(true);
                    }
                }
            }
        }

    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            CardAnimator.SetBool("peek", true);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            CardAnimator.SetBool("peek", false);
        }
    }

    int CheckPositionOnSlot(Vector2 position)
    {
        Vector3[] tempCorners = new Vector3[4];

        for (int i = 0; i < 3; i++)
        {
            
            InterfaceManager.Instance.TableSlots[i].GetWorldCorners(tempCorners);
            if (position.x > tempCorners[1].x && position.x < tempCorners[3].x && position.y < tempCorners[1].y && position.y > tempCorners[3].y)
            {
                if (InterfaceManager.Instance.OccupiedTableSlots.Contains(InterfaceManager.Instance.TableSlots[i]))
                {
                    Debug.Log("ret -1 occHeck " + i);
                    return -1;
                }

                if (!CheckStats()) return -1;

                Debug.Log("ret " + i);
                return i;
            }
        }
        for (int i = 0; i < 2; i++)
        {
            InterfaceManager.Instance.StaticSlots[i].GetWorldCorners(tempCorners);
            if (position.x > tempCorners[1].x && position.x < tempCorners[3].x && position.y < tempCorners[1].y && position.y > tempCorners[3].y)
            {
                if (!CheckStats()) return -1;
                Debug.Log("ret " + (i + 3));
                return i+3;
            }

        }
        InterfaceManager.Instance.GarbageSlot.GetWorldCorners(tempCorners);
        if (position.x > tempCorners[1].x && position.x < tempCorners[3].x && position.y < tempCorners[1].y && position.y > tempCorners[3].y)
        {
            Debug.Log("ret garbage");
            return 999;
        }
        Debug.Log("ret -1");
        return -1;
    }

    bool CheckPositionOnHand(Vector2 position)
    {
        Vector3[] tempCorners = new Vector3[4];

        InterfaceManager.Instance.HandDropZone.GetWorldCorners(tempCorners);

        if (position.x > tempCorners[1].x && position.x < tempCorners[3].x && position.y < tempCorners[1].y && position.y > tempCorners[3].y)
            return true;
        else return false;
    }



    public void SetValues(Card card)
    {
        CardData = ScriptableObject.CreateInstance("Card") as Card;

        CardData.CostFund = card.CostFund;
        CardData.CostPower = card.CostPower;
        CardData.Description = card.Description;
        CardData.EffectFollowersModerate = card.EffectFollowersModerate;
        CardData.EffectFollowersRadical = card.EffectFollowersRadical;
        CardData.EffectFunds = card.EffectFunds;
        CardData.EffectHope = card.EffectHope;
        CardData.EffectPower = card.EffectPower;
        CardData.EffectVisibility = card.EffectVisibility;
        CardData.Graphic = card.Graphic;
        CardData.Name = card.Name;
        CardData.Rarity = card.Rarity;
        CardData.ReqFModerate = card.ReqFModerate;
        CardData.ReqFRadical = card.ReqFRadical;
        CardData.ReqFunds = card.ReqFunds;
        CardData.ReqHopeMax = card.ReqHopeMax;
        CardData.ReqHopeMin = card.ReqHopeMin;
        CardData.ReqVisibilityMax = card.ReqVisibilityMax;
        CardData.ReqVisibilityMin = card.ReqVisibilityMin;


        LabelFundsCost.text = CardData.CostFund.ToString();
        LabelPoweCost.text = CardData.CostPower.ToString();
        LabelName.text = CardData.Name;
        LabelDescription.text = CardData.Description;
        PopulateEffects();
        ColorFrame();
    }

    public TMP_Text TextPowerEffect;
    public TMP_Text TextFundsEffect;
    public TMP_Text TextFRadEffect;
    public TMP_Text TextFModEffect;
    public TMP_Text TextVisiblityEffect;
    public TMP_Text TextHopeEffect;

    public Image frame;

    void ColorFrame()
    {
        switch (CardData.Rarity)
        {
            case 0:
                frame.color = r0color;
                break;
            case 1:
                frame.color = r1color;
                break;
            case 2:
                frame.color = r2color;
                break;
            case 3:
                frame.color = r3color;
                break;
            case -1:
                frame.color = rWcolor;
                break;
            default:
                break;
        }
    }


    void PopulateEffects()
    {
        string prefixGreen = "<color=#1B8A1F>";
        string prefixRed = "<color=red>";
        string prefixPurple = "<color=purple>";
        string suffix = "</color>";

        if(CardData.EffectPower == 0)
        {
            TextPowerEffect.transform.parent.parent.gameObject.SetActive(false);
        }
        else
        {
            if (CardData.Rarity == 3) TextPowerEffect.text = prefixPurple + CardData.EffectPower.ToString() + suffix;
            else TextPowerEffect.text =  CardData.EffectPower < 0 ? prefixRed + CardData.EffectPower.ToString() + suffix : prefixGreen + "+" + CardData.EffectPower.ToString() + suffix;
        }
        if (CardData.EffectFunds == 0)
        {
            TextFundsEffect.transform.parent.parent.gameObject.SetActive(false);
        }
        else
        {
            if (CardData.Rarity == 3) TextFundsEffect.text = prefixPurple + CardData.EffectFunds.ToString() + suffix;
            else
                TextFundsEffect.text = CardData.EffectFunds < 0 ? prefixRed + CardData.EffectFunds.ToString() + suffix : prefixGreen + "+" + CardData.EffectFunds.ToString() + suffix;
        }

        if (CardData.EffectFollowersRadical == 0)
        {
            TextFRadEffect.transform.parent.parent.gameObject.SetActive(false);
        }
        else
        {
            if (CardData.Rarity == 3) TextFRadEffect.text = prefixPurple + CardData.EffectFollowersRadical.ToString() + suffix;
            else TextFRadEffect.text = CardData.EffectFollowersRadical < 0 ? prefixRed + CardData.EffectFollowersRadical.ToString() + suffix : 
                prefixGreen + "+" + CardData.EffectFollowersRadical.ToString() + suffix;
        }
        if (CardData.EffectFollowersModerate == 0)
        {
            TextFModEffect.transform.parent.parent.gameObject.SetActive(false);
        }
        else
        {
            if (CardData.Rarity == 3) TextFModEffect.text = prefixPurple + CardData.EffectFollowersModerate.ToString() + suffix;
            else
                TextFModEffect.text = CardData.EffectFollowersModerate < 0 ? prefixRed + CardData.EffectFollowersModerate.ToString() + suffix :
                    prefixGreen + "+" + CardData.EffectFollowersModerate.ToString() + suffix;
        }
        if (CardData.EffectVisibility == 0)
        {
            TextVisiblityEffect.transform.parent.parent.gameObject.SetActive(false);
        }
        else
        {
            if (CardData.Rarity == 3) TextVisiblityEffect.text = prefixPurple + CardData.EffectVisibility.ToString() + suffix;
            else
                TextVisiblityEffect.text = CardData.EffectVisibility < 0 ? prefixGreen + CardData.EffectVisibility.ToString() + suffix :
                prefixRed + "+" + CardData.EffectVisibility.ToString() + suffix;
        }
        if (CardData.EffectHope == 0)
        {
            TextHopeEffect.transform.parent.parent.gameObject.SetActive(false);
        }
        else
        {
            if (CardData.Rarity == 3) TextHopeEffect.text = prefixPurple + CardData.EffectHope.ToString() + suffix;
            else
                TextHopeEffect.text = CardData.EffectHope < 0 ? prefixRed + CardData.EffectHope.ToString() + suffix :
                prefixGreen + "+" + CardData.EffectHope.ToString() + suffix;
        }
    }


    public bool CheckStats()
    {
        if(CardData.CostPower + GameManager.Instance.PredictPower < -GameManager.Instance.Power)
        {
            StartCoroutine(FlashPanelRoutine(InterfaceManager.Instance.LabelPower.transform.parent)); 
            return false;
        }
        if (CardData.CostFund + GameManager.Instance.PredictFunds < -GameManager.Instance.Funds)
        {
            StartCoroutine(FlashPanelRoutine(InterfaceManager.Instance.LabelFunds.transform.parent));

            return false;
        }
        //}
        //if (CardData.EffectFollowersModerate + GameManager.Instance.PredictFmod < -GameManager.Instance.FolowersModerate)
        //{
        //    StartCoroutine(FlashPanelRoutine(InterfaceManager.Instance.LabelFollowersModerate.transform.parent));

        //    return false;
        //}
        //if (CardData.EffectFollowersRadical + GameManager.Instance.PredictFRad < -GameManager.Instance.FollowersRadical)
        //{
        //    StartCoroutine(FlashPanelRoutine(InterfaceManager.Instance.LabelFollowersRadical.transform.parent));

        //    return false;
        //}
        return true;
        
    }
    IEnumerator FlashPanelRoutine(Transform panel)
    {
        Color c = panel.GetComponent<Image>().color;
        panel.GetComponent<Image>().color = Color.red;
        yield return new WaitForSecondsRealtime(0.1f);
        panel.GetComponent<Image>().color = c;
    }
}

