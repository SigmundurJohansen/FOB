using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySlot : MonoBehaviour
{

    public void OnMouseOver()
    {
        Debug.Log("over");
    }

    public void OnMouseExit()
    {
        Debug.Log("out");
    }

    public void MousePointerEnter()
    {
        TooltipUI.ShowTooltipStatic("ability");
    }

    public void MousePointerExit()
    {
        TooltipUI.HideTooltipStatic();
    }
}
