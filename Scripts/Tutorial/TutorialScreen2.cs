using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Spine;
using Spine.Unity;

public class TutorialScreen2 : MonoBehaviour
{
    [SerializeField] RectTransform messageTransform;
    [SerializeField] RectTransform buttonTransform;
    [SerializeField] SkeletonGraphic handAnim;
    [SerializeField] Vector2[] vectorsArray;

    private float timeBetweenHandAnimations = 2f;
    private bool animIsActive;
    private float animTimer;
    private int clickCount;
    private bool animIsPlayind;

    public void Init(float handAnimTime)
    {
        timeBetweenHandAnimations = handAnimTime;

        handAnim.color = new Color(1, 1, 1, 0);
        messageTransform.localScale = new Vector3(0, 0, 0);
        animIsActive = false;
    }

    public void UpdateScreen()
    {
        if (animIsActive)
        {
            if (animTimer > 0)
                animTimer -= Time.deltaTime;
            else
            {
                animTimer = timeBetweenHandAnimations;
                animIsPlayind = true;
                StartCoroutine(StartHandAnimation());
            }
        }
    }

    public void CallScreen()
    {
        handAnim.AnimationState.Complete += StopHandAnimation;

        clickCount = 0;
        SetHandPosition(vectorsArray[0]);

        animIsActive = true;
        messageTransform.DOScale(1, 0.5f);

        gameObject.SetActive(true);
    }

    public void ButtonClick()
    {
        EventBus.removeCubeByID?.Invoke(clickCount);

        clickCount += 1;

        if (clickCount <= 3)
        {
            SetHandPosition(vectorsArray[clickCount]);
            StopCoroutine(StartHandAnimation());
            animIsPlayind = false;
            handAnim.DOFade(0, 0.3f);
            animTimer = 1f;
        }
        else
            StartCoroutine(CloseScreen());
    }

    private IEnumerator StartHandAnimation()
    {
        handAnim.DOFade(1, 0.3f);

        yield return new WaitForSeconds(0.3f);

        handAnim.AnimationState.SetAnimation(0, "Pressing", false);
        handAnim.timeScale = 1f;

        if (animIsPlayind)
        {
            yield return new WaitForSeconds(1f);

            handAnim.DOFade(0, 0.3f);
        }
    }

    private void StopHandAnimation(TrackEntry trackEntry)
    {
        handAnim.DOFade(0, 0.3f);
    }

    private void SetHandPosition(Vector3 newPos) => buttonTransform.DOAnchorPos(newPos, 0.5f);
    
    private IEnumerator CloseScreen()
    {
        handAnim.AnimationState.Complete -= StopHandAnimation;

        StopCoroutine(StartHandAnimation());
        animIsActive = false;
        handAnim.DOFade(0, 0.5f);
        messageTransform.DOScale(0, 0.5f);

        yield return new WaitForSeconds(1f);

        gameObject.SetActive(false);
    }
}
