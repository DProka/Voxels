using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.EventSystems;

public class UIRateUsScreen : MonoBehaviour
{
    [Header("General")]

    private Canvas screenCanvas;
    private int reward;
    private bool isClicked;
    private bool isMoved;
    private bool isDragging;

    [Header("Windows")]

    [SerializeField] RectTransform generalWindow;
    [SerializeField] Transform starsPart;
    [SerializeField] Transform commentPart;
    [SerializeField] float finishSize = 1279f;
    [SerializeField] TMP_InputField commentInput;

    [Header("Stars")]

    [SerializeField] Image[] starImagesArray;
    [SerializeField] Sprite starFilledSprite;
    [SerializeField] Sprite starEmptySprite;

    public void Init()
    {
        screenCanvas = GetComponent<Canvas>();
        screenCanvas.enabled = false;
        commentPart.gameObject.SetActive(false);
        isClicked = false;
        isMoved = false;
    }

    public void UpdateScreen()
    {
        UpdateSwipe();
    }

    public void ActivateContinueButton()
    {
        EventBus.activateInterstitialAD?.Invoke(reward);
        CloseScreen();
    }

    //public void ActivateStar(int num)
    //{
    //    if (!isClicked)
    //    {
    //        if (starImagesArray[num].sprite == starEmptySprite)
    //        {
    //            for (int i = 0; i < starImagesArray.Length; i++)
    //            {
    //                if (i <= num)
    //                    starImagesArray[i].sprite = starFilledSprite;
    //                else
    //                    starImagesArray[i].sprite = starEmptySprite;
    //            }
    //        }
    //        else
    //            starImagesArray[num].sprite = starEmptySprite;

    //        if (CheckActiveStars())
    //            StartCoroutine(ActivateCommentPart());
    //    }
    //}

    public void SubmitComment()
    {
        Debug.Log("Comment: " + commentInput.text);
        CloseScreen();
    }

    public void OpenPlayMarket()
    {
        Application.OpenURL("market://details?id=bingo.games.lucky.journey");
        CloseScreen();
    }

    public void OpenEmailMessage()
    {
        Application.OpenURL("mailto:e.karavashkin@gmail.com?subject=TapOut_Feedback");
        CloseScreen();
    }

    private IEnumerator ActivateCommentPart()
    {
        Vector2 sizeDelta = new Vector2(generalWindow.sizeDelta.x, finishSize);
        generalWindow.DOSizeDelta(sizeDelta, 0.3f);

        if (!isMoved)
        {
            starsPart.DOMoveY(starsPart.position.y + 300, 0.3f);
            isMoved = true;
        }

        yield return new WaitForSeconds(0.3f);

        commentPart.gameObject.SetActive(true);
    }

    private void ResetStarImages()
    {
        foreach (Image star in starImagesArray)
        {
            star.sprite = starEmptySprite;
        }
    }

    private bool CheckActiveStars()
    {
        bool active = false;

        for (int i = 0; i < starImagesArray.Length; i++)
        {
            if (starImagesArray[i].sprite == starFilledSprite)
                active = true;
            break;
        }

        return active;
    }

    #region Swipe

    private void UpdateSwipe()
    {
        if (screenCanvas.enabled)
        {
            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                UpdateStars(Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                
                if(CheckActiveStars())
                    StartCoroutine(ActivateCommentPart());
            }

            if (isDragging)
            {
                UpdateStars(Input.mousePosition);
            }
        }
    }

    private void UpdateStars(Vector2 mousePosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), mousePosition, null, out Vector2 localPoint);

        if(mousePosition.y > starImagesArray[0].transform.position.y - 100 && mousePosition.y < starImagesArray[0].transform.position.y + 100)
        {
            for (int i = 0; i < starImagesArray.Length; i++)
            {
                if (localPoint.x >= starImagesArray[i].rectTransform.localPosition.x)
                {
                    starImagesArray[i].sprite = starFilledSprite;
                }
                else
                {
                    starImagesArray[i].sprite = starEmptySprite;
                }
            }
        }
    }
    #endregion

    #region Main Screen

    public void OpenScreen()
    {
        StartCoroutine(CallScreen());
    }

    public void CloseScreen()
    {
        screenCanvas.enabled = false;
        EventBus.switchUIActive?.Invoke(false);
    }

    private IEnumerator CallScreen()
    {
        ResetStarImages();

        yield return new WaitForSeconds(2f);

        screenCanvas.enabled = true;
        EventBus.switchUIActive?.Invoke(true);
    }
    #endregion
}
