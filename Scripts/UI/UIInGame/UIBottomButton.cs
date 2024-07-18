using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBottomButton : MonoBehaviour
{
    public bool isActive { get; private set; }

    [SerializeField] ButtonAlert buttonAlert;
    [SerializeField] Sprite[] sprites;

    private Image main;

    public void Init()
    {
        main = GetComponent<Image>();
        isActive = false;
    }

    public void SwitchButton(bool _isActive)
    {
        isActive = _isActive;
        main.sprite = isActive ? sprites[0] : sprites[1];
    }

    public void SwitchAlert(bool isActive) => buttonAlert.SwitchAlert(isActive);
}
