
using UnityEngine;

public class UIBonusLine : MonoBehaviour
{
    [SerializeField] Transform bonusLine;
    [SerializeField] Transform bonusArrowPoint;
    [SerializeField] Transform arrowImage;
    
    private float bonusArrowSpeed;
    private bool isActive;
    private bool goingRight;

    public void Init(float arrowSpeed)
    {
        bonusArrowSpeed = arrowSpeed;
        goingRight = true;
    }

    public void UpdateLine()
    {
        if (isActive)
        {
            if (goingRight)
            {
                if (bonusLine.localScale.x <= 1)
                    bonusLine.localScale = new Vector2(bonusLine.localScale.x + (bonusArrowSpeed * Time.deltaTime), bonusLine.localScale.y);
                else
                    goingRight = false;
            }
            else
            {
                if (bonusLine.localScale.x >= 0)
                    bonusLine.localScale = new Vector2(bonusLine.localScale.x - (bonusArrowSpeed * Time.deltaTime), bonusLine.localScale.y);
                else
                    goingRight = true;
            }
            
        }

        arrowImage.position = bonusArrowPoint.position;
    }

    public void SwitchActive(bool _isActive)
    {
        isActive = _isActive;
        if (isActive)
            bonusLine.localScale = new Vector2(0, 0);
    }

    public float GetLineScale() { return bonusLine.transform.localScale.x; }
}
