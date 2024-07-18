
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class ButtonAlert : MonoBehaviour
{
    [SerializeField] float animationTime = 2f;

    private Image alertImage;
    private bool isActive;

    private void Start()
    {
        alertImage = GetComponentInChildren<Image>();
        alertImage.DOFade(0, 0);
    }

    public void SwitchAlert(bool _isActive)
    {
        isActive = _isActive;

        if (isActive)
            StartCoroutine(StartAlertAnimation());
    }

    private IEnumerator StartAlertAnimation()
    {
        alertImage.DOFade(1, animationTime / 2 - 0.2f);

        yield return new WaitForSeconds(animationTime/ 2);

        alertImage.DOFade(0, animationTime / 2 - 0.2f);

        yield return new WaitForSeconds(animationTime / 2);

        if (isActive)
            StartCoroutine(StartAlertAnimation());
    }
}
