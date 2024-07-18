using System.Collections;
using UnityEngine;
using DG.Tweening;
using Spine;
using Spine.Unity;

public class TutorialScreen1 : MonoBehaviour
{
    [SerializeField] SkeletonGraphic handAnim;
    [SerializeField] RectTransform messageTransform;
    
    private float timeBetweenHandAnimations = 1f;
    private bool animIsActive;
    private float animTimer;
    
    public void Init(float handAnimTime)
    {
        timeBetweenHandAnimations = handAnimTime;

        handAnim.color = new Color(1, 1, 1, 0);
        messageTransform.localScale = new Vector3(0, 0, 0);
        animIsActive = false;
    }

    public void UpdateScreen()
    {
        if (!gameObject.activeSelf)
            animIsActive = false;

        if (animIsActive)
        {
            if (animTimer > 0)
                animTimer -= Time.deltaTime;
            else
            {
                animTimer = timeBetweenHandAnimations;
                StartCoroutine(StartHandAnimation());
            }
        }
    }

    public void CallScreen()
    {
        handAnim.AnimationState.Complete += StopHandAnimation;
        gameObject.SetActive(true);
        animIsActive = true;
        messageTransform.DOScale(1, 0.5f);
    }

    public void ButtonClick() { StartCoroutine(CloseScreen()); }

    private IEnumerator StartHandAnimation()
    {
        handAnim.DOFade(1, 0.3f);

        yield return new WaitForSeconds(0.3f);

        handAnim.AnimationState.SetAnimation(0, "Pressing", false);
        handAnim.timeScale = 1f;
    }

    private void StopHandAnimation(TrackEntry trackEntry)
    {
        handAnim.DOFade(0, 0.3f);
    }

    private IEnumerator CloseScreen()
    {
        handAnim.AnimationState.Complete -= StopHandAnimation;
        StopCoroutine(StartHandAnimation());
        animIsActive = false;
        messageTransform.DOScale(0, 0.5f);
        handAnim.DOFade(0, 0.5f);

        yield return new WaitForSeconds(1f);

        gameObject.SetActive(false);
    }
}
