using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject highlightBar;
    [SerializeField] private Color hoverColour = Color.black;
    [SerializeField] private Color activeTurnColour = Color.yellow;

    private Image highlightImage;
    private bool isActiveTurn = false;
    private bool isHovered = false;

    private void Awake()
    {
        if (highlightBar != null)
        {
            highlightImage = highlightBar.GetComponent<Image>();
            highlightBar.SetActive(false);
        }
    }

    public void SetActiveTurn(bool active)
    {
        isActiveTurn = active;
        UpdateHighlight();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        UpdateHighlight();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        UpdateHighlight();
    }

    private void UpdateHighlight()
    {
        if (highlightBar == null) return;

        if (isActiveTurn)
        {
            highlightBar.SetActive(true);
            highlightImage.color = activeTurnColour;
        }
        else if (isHovered)
        {
            highlightBar.SetActive(true);
            highlightImage.color = hoverColour;
        }
        else
        {
            highlightBar.SetActive(false);
        }
    }
}
