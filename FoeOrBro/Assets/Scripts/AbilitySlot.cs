using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AbilitySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public void OnMouseOver()
    {
        Debug.Log("over");
    }

    public void OnMouseExit()
    {
        Debug.Log("out");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipUI.ShowTooltipStatic("ability");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.HideTooltipStatic();
    }
}
