using System.Collections;
using UnityEngine;
using DG.Tweening;
using Spine.Unity;

public class TutorialScreen4 : MonoBehaviour
{
    [SerializeField] SkeletonGraphic handAnim;
    public RectTransform messageTransform;

    public void Init()
    {
        handAnim.transform.localScale = new Vector3(0, 0, 0);
        messageTransform.transform.localScale = new Vector3(0, 0, 0);
    }

    public void CallScreen()
    {
        gameObject.SetActive(true);
        SwitchHandAnimation(true);
        messageTransform.DOScale(1, 0.5f);
    }

    public void ButtonClick()
    {
        StartCoroutine(CloseScreen());
    }

    private void SwitchHandAnimation(bool isActive)
    {
        if (isActive)
        {
            handAnim.transform.DOScale(1, 0.5f);
            handAnim.AnimationState.SetAnimation(0, "Zoom", true);
            handAnim.timeScale = 1f;
        }
        else
        {
            handAnim.transform.DOScale(0, 0.5f);
            handAnim.timeScale = 0f;
        }
    }

    private IEnumerator CloseScreen()
    {
        SwitchHandAnimation(false);
        messageTransform.DOScale(0, 0.5f);

        yield return new WaitForSeconds(1f);

        gameObject.SetActive(false);
    }
}
