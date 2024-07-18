using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialModelPrefab : MonoBehaviour
{
    private List<CubePrefab> cubes;

    public void Init()
    {
        cubes = new List<CubePrefab>();

        for (int i = 0; i < transform.childCount; i++)
        {
            CubePrefab newCube = transform.GetChild(i).GetComponent<CubePrefab>();
            newCube.Init();
            //newCube.SetMeshColor(newCube.baseColor);
            cubes.Add(newCube);
        }
    }
}
