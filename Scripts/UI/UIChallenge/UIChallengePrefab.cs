using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIChallengePrefab : MonoBehaviour
{
    [SerializeField] Image backImage;

    private int price;
    private int lvlToUnlock;

    [Header("Locked Part")]

    [SerializeField] GameObject lockedPart;
    [SerializeField] TextMeshProUGUI modelNumText1;
    [SerializeField] TextMeshProUGUI modelNumSText1;
    [SerializeField] TextMeshProUGUI lockedlevelText;
    [SerializeField] TextMeshProUGUI lockedlevelSText;
    [SerializeField] Sprite lockedSprite;

    [Header("Opened Part")]

    [SerializeField] GameObject unlockedPart;
    [SerializeField] TextMeshProUGUI modelNumText2;
    [SerializeField] TextMeshProUGUI modelNumSText2;
    [SerializeField] TextMeshProUGUI unlockedPriceText;
    [SerializeField] TextMeshProUGUI unlockedPriceSText;
    [SerializeField] Sprite unlockedSprite;

    [Header("Passed Part")]

    [SerializeField] GameObject passedPart;
    [SerializeField] TextMeshProUGUI modelNumText3;
    [SerializeField] TextMeshProUGUI modelNumSText3;
    [SerializeField] Sprite passedSprite;

    public int modelNum { get; private set; }

    private UIChallengeAlbum challengeAlbum;

    public void Init(UIChallengeAlbum album, int num, int _price, int _lvlToUnlock)
    {
        challengeAlbum = album;
        
        modelNum = num;
        modelNumText1.text = "" + num;
        modelNumText2.text = "" + num;
        modelNumText3.text = "" + num;
        modelNumSText1.text = "" + num;
        modelNumSText2.text = "" + num;
        modelNumSText3.text = "" + num;

        price = _price;
        unlockedPriceText.text = "" + price;
        unlockedPriceSText.text = "" + price;

        lvlToUnlock = _lvlToUnlock;
        lockedlevelText.text = "Level " + lvlToUnlock;
        lockedlevelSText.text = "Level " + lvlToUnlock;

        SwitchStatus(0);
    }

    public void LoadModel() => challengeAlbum.OpenModelByNum(modelNum);

    public void SwitchStatus(int status)
    {
        lockedPart.SetActive(false);
        unlockedPart.SetActive(false);
        passedPart.SetActive(false);

        switch (status)
        {
            case 0:
                lockedPart.SetActive(true);
                backImage.sprite = lockedSprite;
                break;
        
            case 1:
                unlockedPart.SetActive(true);
                backImage.sprite = unlockedSprite;
                break;
        
            case 2:
                passedPart.SetActive(true);
                backImage.sprite = passedSprite;
                break;
        }
    }
}
