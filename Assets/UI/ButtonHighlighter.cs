using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonHighlighter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Button button;
    [SerializeField] TMP_Text label;
    [SerializeField] Image icon;

    private Color normal = new(0.7843137f, 0.7843137f, 0.7843137f, 0.7843137f);
    private Color disabled = new(0.5f, 0.5f, 0.5f, 0.5f);

    private void Awake()
    {
        if (button.IsInteractable())
        {
            if (icon != null) icon.color = normal;
            label.color = new Color(label.color.r, label.color.g, label.color.b, normal.a);

        }
        else
        {
            if (icon != null) icon.color = disabled;
            label.color = new Color(label.color.r, label.color.g, label.color.b, disabled.a);
        }
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (button.IsInteractable())
        {
            if(icon != null) icon.color = Color.white;
            label.color = new Color(label.color.r, label.color.g, label.color.b, 1);
        }
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if(button.IsInteractable())
        {
            if (icon != null) icon.color = normal;
            label.color = new Color(label.color.r, label.color.g, label.color.b, normal.a);
        }
    }
}
