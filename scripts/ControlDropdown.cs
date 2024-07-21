using UnityEngine;
using UnityEngine.EventSystems;

public class ControlDropdown : MonoBehaviour, IPointerClickHandler
{
    private int previousIndex = 0;
    private const float HEIGHT_OF_ITEM = 113.37f;
    // implementing of interface for receiving of pointer event in the dropdown to control mouse lock
    public void OnPointerClick(PointerEventData eventData)
    {
        var content = GameObject.Find("Content").GetComponent<RectTransform>();
        content.anchoredPosition3D += new Vector3(0f, HEIGHT_OF_ITEM * previousIndex, 0f);
        MouseLook.isLocked = true;
    }
    /// <summary>
    /// receives ParserMse's BroadcastMessage when dropdown is opened
    /// </summary>
    /// <param name="index"></param>
    void SetPreviousIndex(int index)
    {
        previousIndex = index;
    }
}
