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
    bool isAcceptingDrag = true;

    private int tableSlot;
    //--------------CARD UI-----------

    public TMP_Text LabelPoweCost;
    public TMP_Text LabelFundsCost;
    public TMP_Text LabelName;
    public TMP_Text LabelEffects;
    public TMP_Text LabelDescription;

    
    void Start()
    {
        Container = transform.parent;
    }
    public void OnDrag(PointerEventData eventData)
    {

        if (!isAcceptingDrag) return;
        gameObject.transform.position = (Vector3) eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        if (!isAcceptingDrag) return;
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
                    var child = InterfaceManager.Instance.StaticSlots[i - 3].GetChild(1);
                    child.SetParent(null);
                    Destroy(child.gameObject);
                }
                transform.SetParent(InterfaceManager.Instance.StaticSlots[i-3]);
                transform.SetAsFirstSibling();
                transform.position = InterfaceManager.Instance.StaticSlots[i - 3].transform.position;
                InterfaceManager.Instance.OccupiedStaticSlots.Add(InterfaceManager.Instance.StaticSlots[i - 3]);
                GameManager.Instance.TableCards.Add(CardData);
                GameManager.Instance.HandCount--;

                isOnTable = true;
                isAcceptingDrag = false;

            }
        }
        else //FROM TABLE TO HAND
        {
            
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

    private void RemovePredictions()
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

    private void ApplyPredictions()
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
        if (!isAcceptingDrag) return;
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
                Debug.Log("ret " + i);
                return i;
            }
        }
        for (int i = 0; i < 2; i++)
        {
            InterfaceManager.Instance.StaticSlots[i].GetWorldCorners(tempCorners);
            if (position.x > tempCorners[1].x && position.x < tempCorners[3].x && position.y < tempCorners[1].y && position.y > tempCorners[3].y)
            {
                Debug.Log("ret " + (i + 3));
                return i+3;
            }
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
        CardData = card;
        LabelFundsCost.text = CardData.CostFund.ToString();
        LabelPoweCost.text = CardData.CostPower.ToString();
        LabelName.text = CardData.Name;
        LabelDescription.text = CardData.Description;
        LabelEffects.text = "effects placeholder";
    }

}
