using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIFailScreen : MonoBehaviour
{
    [SerializeField] Canvas mainCanvas;
    [SerializeField] TextMeshProUGUI cubesCount;
    [SerializeField] GameObject restartButton;
    
    public void OpenScreen(int _cubesCount)
    {
        cubesCount.text = $"<color=green>{_cubesCount}</color> Cubes left";

        restartButton.SetActive(false);
        mainCanvas.enabled = true;

        StartCoroutine(ShowScreen());
    }

    private IEnumerator ShowScreen()
    {
        yield return new WaitForSeconds(1.5f);

        restartButton.SetActive(true);
    } 

    public void CloseScreen()
    {
        mainCanvas.enabled = false;
    }
}
