
using UnityEngine;

[CreateAssetMenu(fileName = "ModelsBase", menuName = "ScriptableObject/ModelsBase")]
public class ModelsBase : ScriptableObject
{
    [Header("Basic Models")]

    public TextAsset[] modelsArray;

    [Header("Challenge Models")]

    public TextAsset[] challengeModelsArray;
    public int[] challengeModelLvlArray;
    public int[] challengeModelRewardArray;

    public byte[] GetModelByNum(bool isBasicModel, int num)
    {
        TextAsset bindata = new TextAsset();
        
        if(isBasicModel)
            bindata = modelsArray[num];
        else
            bindata = challengeModelsArray[num];

        return bindata.bytes;
    }

    public int CheckNewModelNum()
    {
        int newNum = 0;

        foreach(int lvl in challengeModelLvlArray)
        {
            if (newNum <= lvl && lvl <= SceneController.lvlNum)
                newNum = lvl;
        }

        return newNum;
    }
}
