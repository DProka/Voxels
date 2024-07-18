
using UnityEngine;
using UnityEngine.UI;

public class UISettings : MonoBehaviour
{
    private Canvas mainCanvas;
    private int[] settingsArray;

    [Header("Buttons")]

    [SerializeField] Image[] musicButtonImage;
    [SerializeField] Image[] effectsButtonImage;
    [SerializeField] Image[] vibroButtonImage;

    [Header("Sprites")]

    [SerializeField] Sprite[] buttonSprites;
    [SerializeField] Sprite[] musicSprites;
    [SerializeField] Sprite[] effectsSprites;
    [SerializeField] Sprite[] vibroSprites;

    public void Init()
    {
        mainCanvas = GetComponent<Canvas>();
        mainCanvas.enabled = false;

        settingsArray = new int[] { 1, 1, 1 };
    }

    #region Buttons

    public void SwitchButton(int buttonNum)
    {
        switch (buttonNum)
        {
            case 1:
                UpdateButton(0, musicButtonImage[0], musicButtonImage[1], musicSprites);
                break;
        
            case 2:
                UpdateButton(1, effectsButtonImage[0], effectsButtonImage[1], effectsSprites);
                break;
        
            case 3:
                UpdateButton(2, vibroButtonImage[0], vibroButtonImage[1], vibroSprites);
                break;
        }
    }

    private void UpdateButton(int num, Image buttonImg, Image spriteImage, Sprite[] spritesArray)
    {
        if(settingsArray[num] == 0)
        {
            buttonImg.sprite = buttonSprites[0];
            spriteImage.sprite = spritesArray[0];
            settingsArray[num] = 1;
        }
        else
        {
            buttonImg.sprite = buttonSprites[1];
            spriteImage.sprite = spritesArray[1];
            settingsArray[num] = 0;
        }
    }
    #endregion

    #region Main Window

    public void SwitchCanvas()
    {
        if (mainCanvas.enabled)
        {
            EventBus.switchUIActive?.Invoke(false);
            mainCanvas.enabled = false;
        }
        else
        {
            EventBus.switchUIActive?.Invoke(true);
            mainCanvas.enabled = true;
        }
    }

    public void CloseScreen()
    {
        mainCanvas.enabled = false;
    }
    #endregion
}
