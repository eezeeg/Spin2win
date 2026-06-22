using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonTextPressEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [Header("Text")]
    [SerializeField] private TMP_Text textToMove;

    [Header("Movement")]
    [SerializeField] private float moveDownAmount = 5f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clickSound;

    private Vector3 originalLocalPosition;
    private bool isPressed;

    private void Awake()
    {
        if (textToMove == null)
        {
            textToMove = GetComponentInChildren<TMP_Text>();
        }

        if (audioSource == null)
        {
            GameObject GO = GameObject.FindGameObjectWithTag("AudioSourceButtons");
            audioSource = GO.GetComponent<AudioSource>();
        }

        if (textToMove != null)
        {
            originalLocalPosition = textToMove.rectTransform.localPosition;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        MoveTextDown();

        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
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
        if (textToMove != null)
        {
            textToMove.rectTransform.localPosition = originalLocalPosition;
        }

        isPressed = false;
    }
}