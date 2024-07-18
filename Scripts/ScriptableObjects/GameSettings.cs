
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObject/GameSettings")]
public class GameSettings : ScriptableObject
{
    [Header("Game Settings")]

    public float movesOnModelRatio;
    public float timeBetweenCoins = 1;
    public float timeBetweenBombs = 2;

    [Header("Model Settings")]
     
    public float collectingMoveSpeed = 5f;
    public float spawnCollectingRadius = 7.0f;
}
