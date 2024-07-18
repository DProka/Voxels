
using UnityEngine;
using DG.Tweening;

public class CubeGlass : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private float scaleSpeed;
    private State currentState;

    [Header("Coin")]

    [SerializeField] CubeItem coin;

    [Header("Puzzle")]
    
    [SerializeField] CubeItem puzzle;
    [SerializeField] Color puzzleColor;
    
    [Header("Bomb")]
    
    [SerializeField] CubeItem bomb;

    public void Init(float _scaleSpeed, float itemRotateSpeed)
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
        scaleSpeed = _scaleSpeed;

        coin.Init(itemRotateSpeed);
        puzzle.Init(itemRotateSpeed);
        bomb.Init(itemRotateSpeed);


        //puzzle.SetMeshColor(puzzleColor);
    }

    public void SwitchActive(bool isActive, State newState)
    {
        if (isActive)
            Appear();

        currentState = newState;

        switch (currentState)
        {
            case State.Coin:
                coin.SwitchActive(true);
                puzzle.SwitchActive(false);
                bomb.SwitchActive(false);
                break;
        
            case State.Puzzle:
                coin.SwitchActive(false);
                puzzle.SwitchActive(true);
                //puzzle.SetMeshColor(puzzleColor);
                bomb.SwitchActive(false);
                break;
        
            case State.Bomb:
                coin.SwitchActive(false);
                puzzle.SwitchActive(false);
                bomb.SwitchActive(true);
                break;
        }
    }

    public void SwitchMesh(bool isActive)
    {
        meshRenderer.enabled = isActive;

        switch (currentState)
        {
            case State.Coin:
                coin.SwitchActive(isActive);
                break;
        
            case State.Puzzle:
                puzzle.SwitchActive(isActive);
                break;
        
            case State.Bomb:
                bomb.SwitchActive(isActive);
                break;
        }
    }

    public enum State
    {
        Coin,
        Puzzle,
        Bomb,
    }

    private void Appear()
    {
        meshRenderer.enabled = true;
        transform.localScale = new Vector3(0, 0, 0);
        transform.DOScale(1, scaleSpeed);
    }
}
