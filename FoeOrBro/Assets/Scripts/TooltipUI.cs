using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance {get;private set;}
    private RectTransform backgroundRectTransform;
    private TextMeshProUGUI textMeshPro;
    private RectTransform rectTransform;
    [SerializeField] private RectTransform canvasRecTransform;
    
    private System.Func<string> getTooltipTextFunc;

    private void Awake(){
        Instance = this;
        backgroundRectTransform = transform.Find("Background").GetComponent<RectTransform>();
        textMeshPro = transform.Find("TooltipText").GetComponent<TextMeshProUGUI>();
        rectTransform = transform.GetComponent<RectTransform>();
        HideTooltip();
    }

    private void SetText(string _tooltip)
    {
        textMeshPro.SetText(_tooltip);
        textMeshPro.ForceMeshUpdate();
        Vector2 textSize = textMeshPro.GetRenderedValues(false);
        Vector2 paddingSize = new Vector2(8,8);
        backgroundRectTransform.sizeDelta = textSize + paddingSize;
    }

    private void Update(){
        SetText(getTooltipTextFunc());
        Vector2 anchor = Input.mousePosition / canvasRecTransform.localScale.x;
        if(anchor.x + backgroundRectTransform.rect.width > canvasRecTransform.rect.width){
            anchor.x = canvasRecTransform.rect.width - backgroundRectTransform.rect.width;
        }
        if(anchor.y + backgroundRectTransform.rect.height > canvasRecTransform.rect.height){
            anchor.y = canvasRecTransform.rect.height - backgroundRectTransform.rect.height;
        }
        rectTransform.anchoredPosition = anchor;
    }
    private void ShowTooltip(string _tooltip){
        ShowTooltip(() => _tooltip);
    }
    private void ShowTooltip(System.Func<string> _tooltip)
    {
        this.getTooltipTextFunc = _tooltip;
        SetText(getTooltipTextFunc());
        gameObject.SetActive(true);
    }

    private static void ShowTooltipStatic(System.Func<string> getTooltipText)
    {
        Instance.ShowTooltip(getTooltipText);
    }
    public static void ShowTooltipStatic(string _tooltip)
    {
        Instance.ShowTooltip(_tooltip);
    }

    private void HideTooltip()
    {
        gameObject.SetActive(false);
    }
    public static void HideTooltipStatic()
    {
        Instance.HideTooltip();
    }
}
