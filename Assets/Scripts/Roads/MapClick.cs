using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapClick : MonoBehaviour, IPointerClickHandler
{
    public GameObject map;
    
    public void OnPointerClick(PointerEventData data) {
        Debug.Log(data.position);

        Vector2 pos = ((data.position - new Vector2(10.0f, 10.0f)) / 200.0f) * 250.0f;
        Vector2Int i_pos = new Vector2Int(
            (int)Mathf.Clamp(pos.x, 0.0f, 249.0f),
            (int)Mathf.Clamp(pos.y, 0.0f, 249.0f)
        );

        if (data.button == PointerEventData.InputButton.Left)
            map.GetComponent<DisplayRoad>().setStart(i_pos);
        else if (data.button == PointerEventData.InputButton.Right)
            map.GetComponent<DisplayRoad>().setEnd(i_pos);
    }
}
