using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] string tooltipText;
    [SerializeField] ToolTipType type;
    public void OnPointerEnter(PointerEventData eventData){
        TooltipManager.show(true, tooltipText, type);
    }

    public void OnPointerExit(PointerEventData eventData){
        TooltipManager.show(false, null, type);
    }
    public void OnDestroy(){
        TooltipManager.show(false, null, type);
    }
}
