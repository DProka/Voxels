
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UILvlBar : MonoBehaviour
{
    [SerializeField] Image leftCubeImg;
    [SerializeField] Image rightCubeImg;
    [SerializeField] Image frontImg;
    [SerializeField] TextMeshProUGUI currentLvlNum;
    [SerializeField] TextMeshProUGUI nextLvlNum;

    [SerializeField] Sprite[] cubeSprites;
    [SerializeField] Sprite[] barSprites;

    private int leftSpriteNum;
    private int rightSpriteNum;
    private int barSpriteNum;

    public void Init()
    {
        leftSpriteNum = 0;
        rightSpriteNum = 1;
        barSpriteNum = 0;
    }

    public void SetImageScale(float scale) => frontImg.DOFillAmount(scale, 0.3f);
    
    public void SetNewLvl(int newLvlNum)
    {
        currentLvlNum.text = "" + newLvlNum;
        nextLvlNum.text = "" + (newLvlNum + 1);

        CheckSpritesBylvl(newLvlNum);

        SetImageScale(0);
    }

    public void SetActive(bool isActive) => gameObject.SetActive(isActive);

    private void CheckSpritesBylvl(int lvl)
    {
        if (lvl > 0)
        {
            leftSpriteNum = GetSpriteNum(leftSpriteNum);
            rightSpriteNum = GetSpriteNum(rightSpriteNum);
            barSpriteNum = GetSpriteNum(barSpriteNum);
        }
        else
        {
            leftSpriteNum = 0;
            rightSpriteNum = 1;
            barSpriteNum = 0;
        }

        leftCubeImg.sprite = cubeSprites[leftSpriteNum];
        rightCubeImg.sprite = cubeSprites[rightSpriteNum];
        frontImg.sprite = barSprites[barSpriteNum];
    }

    private int GetSpriteNum(int currentNum)
    {
        int newNum = currentNum;

        if (newNum < barSprites.Length - 1)
            newNum += 1;
        else
            newNum = 0;

        return newNum;
    }
}
