using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("soundEffects")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clickIn;
    [SerializeField] private AudioClip clickoff;

    [SerializeField] private float sizeDifference = 1.05f;
    [SerializeField] private float sizeDifferenceDown = 0.95f;


    private Vector2 ogSize;
    private RectTransform recTrans;

    private bool isClicking = false;
    private bool ishovering = false;
    private bool clickInPlaying = false;

    private Coroutine clickOffCoroutine;
    private void Awake()
    {
        recTrans = GetComponent<RectTransform>();
        ogSize = recTrans.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ishovering = true;
        recTrans.localScale = ogSize *(isClicking? sizeDifferenceDown : sizeDifference);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ishovering = false;
        if (isClicking)
            return;
        recTrans.localScale = ogSize;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;


        if (ishovering && !isClicking)
        {
            isClicking = true;
            recTrans.localScale = ogSize * sizeDifferenceDown;

            if (clickOffCoroutine != null)
                StopCoroutine(clickOffCoroutine);

            if (audioSource != null && clickIn != null)
            {
                clickInPlaying = true;
                audioSource.clip = clickIn;
                audioSource.Play();

                StartCoroutine(MarkClickInFinished());
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;



        if (isClicking)
        {
            isClicking = false;
            recTrans.localScale = ogSize * (ishovering ? sizeDifference : 1f);

            clickOffCoroutine = StartCoroutine(PlayClickOffAfterClickIn());
        }
    }

    private IEnumerator MarkClickInFinished()
    {
        yield return new WaitForSecondsRealtime(clickIn.length);
        clickInPlaying = false;
    }

    private IEnumerator PlayClickOffAfterClickIn()
    {
        if (audioSource == null || clickoff == null)
            yield break;

        if (clickInPlaying && clickIn != null)
        {
            yield return new WaitForSecondsRealtime(clickIn.length);
        }

        audioSource.clip = clickoff;
        audioSource.Play();
    }
}
