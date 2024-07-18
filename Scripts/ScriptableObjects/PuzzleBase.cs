
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PuzzleBase", menuName = "ScriptableObject/PuzzleBase")]

public class PuzzleBase : ScriptableObject
{
    [SerializeField] Sprite[] packFood;
    [SerializeField] Sprite[] packPets;
    [SerializeField] Sprite[] packNature;
    [SerializeField] Sprite[] packAnimals;

    private Dictionary<string, Sprite[]> spritesDictionary;

    public void Init()
    {
        UpdateDictionary();
    }

    public Sprite[] GetSpritesByKey(string key) 
    {
        if (spritesDictionary.TryGetValue(key, out Sprite[] array))
            return array;
        else
            return null;
    }

    private void UpdateDictionary()
    {
        spritesDictionary = new Dictionary<string, Sprite[]>();

        spritesDictionary.Add("Food", packFood);
        spritesDictionary.Add("Pets", packPets);
        spritesDictionary.Add("Nature", packNature);
        spritesDictionary.Add("Animals", packAnimals);
    }
}

