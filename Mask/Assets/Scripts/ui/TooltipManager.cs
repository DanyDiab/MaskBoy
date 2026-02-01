using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour
{
    static TooltipManager instance;
    static GameObject tooltip; 
    static TextMeshProUGUI ttText;
    static Image image;

    [Header("Colors")] 
    [SerializeField] Color infoColor;
    [SerializeField] Color warnColor;
    [SerializeField] Color noteColor;
    [SerializeField] Color yellowWarn;
    [Header("Offset")]
    [SerializeField] Vector2 offset;
    Vector2 finalOffset;
    int xThreshold;
    

    void Awake(){
        instance = this;
        tooltip = GameObject.FindGameObjectWithTag("Tooltip");
        ttText = tooltip.GetComponentInChildren<TextMeshProUGUI>();
        image = tooltip.GetComponent<Image>();
        show(false,null, ToolTipType.Info);
        xThreshold = 650;
    }

    void Update(){
        if(!tooltip.activeSelf) return;
        Vector2 mousePos = Input.mousePosition;
        determineSide(mousePos);
        moveToolTipToMouse(mousePos);
    }

    void moveToolTipToMouse(Vector2 mousePos){
        Vector2 finalPos = mousePos + finalOffset;
        tooltip.transform.position = finalPos;
    }

// determines which side the tooltip will show on, updates finalOffset
    void determineSide(Vector2 mousePos){
         int dir = mousePos.x > xThreshold ? -1 : 1;
         float x = offset.x * dir;
         finalOffset = new Vector2(x,offset.y);
    }
    public static void show(bool show, string text, ToolTipType type){
        ttText.text = text;
        Color currColor = instance.infoColor;

        switch(type){
            case ToolTipType.Info:
                currColor = instance.infoColor;
                break;
            case ToolTipType.Warn:
                currColor = instance.warnColor;
                break;
            case ToolTipType.Note:
                currColor = instance.noteColor;
                break;
            case ToolTipType.YellowWarn:
                currColor = instance.yellowWarn;
                break;
        }

        image.color = currColor;
        tooltip.SetActive(show);
    }

}
