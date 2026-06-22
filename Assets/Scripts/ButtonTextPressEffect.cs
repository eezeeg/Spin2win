using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonTextPressEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [Header("Text")]
    [SerializeField] private TMP_Text textToMove;

    [Header("Movement")]
    [SerializeField] private float moveDownAmount = 5f;

    private Vector3 originalLocalPosition;
    private bool isPressed;

    private void Awake()
    {
        if (textToMove == null)
        {
            textToMove = GetComponentInChildren<TMP_Text>();
        }

        if (textToMove != null)
        {
            originalLocalPosition = textToMove.rectTransform.localPosition;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        MoveTextDown();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        MoveTextBack();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MoveTextBack();
    }

    private void MoveTextDown()
    {
        if (textToMove == null)
            return;

        isPressed = true;

        Vector3 newPosition = originalLocalPosition;
        newPosition.y -= moveDownAmount;

        textToMove.rectTransform.localPosition = newPosition;
    }

    private void MoveTextBack()
    {
        if (textToMove == null)
            return;

        if (!isPressed)
            return;

        isPressed = false;
        textToMove.rectTransform.localPosition = originalLocalPosition;
    }

    private void OnDisable()
    {
        MoveTextBack();
    }
}