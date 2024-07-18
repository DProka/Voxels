using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VoxModel : MonoBehaviour 
{
	[SerializeField] GameSettings gameSettings;
	[SerializeField] ModelsBase modelsBase;

	private SceneController sceneController;
	private ModelSave save;

	private int[] savedCubesIDArray;
	private int[] savedCubesStatusArray;

	[Header("Model Rotating")]

	[SerializeField] float sensitivity = 0.5f;
	private ModelPositioning modelPositioning;

	[SerializeField] CubePrefab voxelPrefab;
	public static CubePrefab _voxelPrefab { get; private set; }

	private MainChunk lastChunk;
	private List<CubePrefab> cubesArray;

	private int preparedCubes;

	public void Init(SceneController controller)
	{
		sceneController = controller;
		_voxelPrefab = voxelPrefab;
		modelPositioning = new ModelPositioning(transform, sensitivity);

		EventBus.onCubeDestroyed += RemoveCube;
		EventBus.removeCubeByID += RemoveCubeBuyID;
		EventBus.activateBomb += ExplodeCubes;
		EventBus.updatePraparedCubes += UpdatedPreparedCubes;

		Load();
	}

	public void UpdateModel()
    {
		if (SceneController.modelIsActive)
			modelPositioning.UpdateScript();
	}

	public int GetVoxCount() { return cubesArray.Count; }

    #region VOX Loader

    public void ClearVoxMeshes()
    {
        if (cubesArray == null)
            cubesArray = new List<CubePrefab>();
        else
        {
            foreach (CubePrefab obj in cubesArray)
            {
                Destroy(obj.gameObject);
            }

            cubesArray = new List<CubePrefab>();
        }
    }

    public IEnumerator LoadModelFromVOXFile(bool _isBasicModel, int num)
	{
		yield return new WaitForSeconds(0.1f);

		byte[] bytes = modelsBase.GetModelByNum(_isBasicModel, num);
		//ClearVoxMeshes();
		lastChunk = VoxImporter.LoadVOX(bytes);
		CreateModel(lastChunk);
		Save();
	}
	
    public IEnumerator LoadModelFromSave(bool _isBasicModel, int num)
	{
		yield return new WaitForSeconds(0.1f);

		byte[] bytes = modelsBase.GetModelByNum(_isBasicModel, num);
		ClearVoxMeshes();
		lastChunk = VoxImporter.LoadVOX(bytes);
		int maxCubes = 0;

		if (lastChunk != null)
		{
			GameObject[] array = VoxImporter.CreateIndividualVoxelGameObjects(lastChunk, gameObject.transform, 1);
			maxCubes = array.Length;

			for (int i = 0; i < array.Length; i++)
			{
				if (CheckCubeInSave(i))
				{
					CubePrefab script = array[i].GetComponent<CubePrefab>();
					script.SetID(i);
					cubesArray.Add(script);
				}
				else
					Destroy(array[i]);
			}

			if(cubesArray.Count <= 0)
				sceneController.GoToNextLvl(1);
		}

		sceneController.SetCubesCount(maxCubes, cubesArray.Count);
		StartCoroutine(CheckVoxPosition());
	}

	public void CreateFromLastChunk() { CreateModel(lastChunk); }

	private void CreateModel(MainChunk v)
    {
		ClearVoxMeshes();

		if (v != null)
		{
			GameObject[] array = VoxImporter.CreateIndividualVoxelGameObjects(v, gameObject.transform, 1);
			

			for (int i = 0; i < array.Length; i++)
			{
				CubePrefab script = array[i].GetComponent<CubePrefab>();
				script.SetID(i); 
				cubesArray.Add(script);
			}
		}

		sceneController.SetCubesCount(cubesArray.Count, cubesArray.Count);
		StartCoroutine(CheckVoxPosition());
    }
    #endregion

    #region Voxels

    public void RemoveCube(int _id) 
	{
		for (int i = 0; i < cubesArray.Count; i++)
        {
			if(cubesArray[i].id == _id)
            {
				cubesArray.Remove(cubesArray[i]);
				break;
			}
		}

		sceneController.RemoveCubeFromCount();
		Save();
	}

	public void SetRandomItemCube(int itemNum)
	{
		if (cubesArray.Count > 0)
		{
			List<CubePrefab> cubes = new List<CubePrefab>();

			foreach (CubePrefab cube in cubesArray)
			{
				if (cube.CheckIsBasic())
					cubes.Add(cube);
			}

			if (cubes.Count > 0)
			{
				int cubeID = Random.Range(0, cubesArray.Count);

				switch (itemNum)
				{
					case 1:
						cubes[cubeID].SetCubeState(CubePrefab.CubeState.Coin);
						break;

					case 2:
						cubes[cubeID].SetCubeState(CubePrefab.CubeState.Puzzle);
						break;

					case 3:
						cubes[cubeID].SetCubeState(CubePrefab.CubeState.Bomb);
						break;
				}
			}
		}
	}

	public void SwitchModelActive(bool isActive)
    {
		SwitchVoxColliders(isActive);
		SwitchMeshRenderer(isActive);
	}

	public void SwitchCubesClickOn()
    {
		for (int i = 0; i < cubesArray.Count; i++)
		{
			cubesArray[i].TurnClickOn();
		}
	}

	private void SwitchVoxColliders(bool isActive) 
	{
		foreach (CubePrefab cube in cubesArray)
		{
			cube.SwitchCollider(isActive);
		}
	}

	private void SwitchMeshRenderer(bool isOn)
	{
		foreach (CubePrefab cube in cubesArray)
		{
			cube.SwitchMesh(isOn);
		}
	}

    private void RemoveCubeBuyID(int _id)
    {
        CubePrefab currentCube = null;

        foreach (CubePrefab cube in cubesArray)
        {
            if (cube.id == _id)
                currentCube = cube;
        }

        if (SceneController.lvlNum <3)
        {
            if (currentCube != null)
                StartCoroutine(currentCube.MoveUpAndDisappear());
        }
        else
        {
            currentCube.DestroyCube();
        }
    }

	private void ExplodeCubes()
	{
		if (!SceneController.modelIsActive)
			return;

		int cubesForExplosion = 0;
		int explodedCubes = 0;

        List<int> visibleCubes = new List<int>();

        for (int i = 0; i < cubesArray.Count; i++)
        {
            Ray ray = new Ray(cubesArray[i].transform.position, (Camera.main.transform.position - cubesArray[i].transform.position).normalized);

            RaycastHit hit;
            if (!Physics.Raycast(ray, out hit) && cubesArray[i].CheckIsBasic())
                visibleCubes.Add(cubesArray[i].id);
        }

        if (cubesArray.Count >= 3)
			cubesForExplosion = 3;
        else
			cubesForExplosion = cubesArray.Count;

		if(visibleCubes.Count >= 3)
        {
			for (int i = 0; i < cubesForExplosion; i++)
            {
                int randomNum = Random.Range(0, visibleCubes.Count);
                int idToExplode = visibleCubes[randomNum];

                for (int j = 0; j < cubesArray.Count; j++)
                {
                    if (cubesArray[j].id == idToExplode && cubesArray[j].CheckIsBasic())
                    {
                        cubesArray[j].ExplodeCube();
						RemoveCube(cubesArray[j].id);
						//cubesArray.Remove(cubesArray[j]);
						//sceneController.RemoveCubeFromCount();
						explodedCubes++;
                        visibleCubes.Remove(idToExplode);
                    }

                    if (explodedCubes >= cubesForExplosion)
                        return;
                }
            }
        }
		else
        {
			for (int i = 0; i < cubesForExplosion; i++)
			{
				for (int j = 0; j < cubesArray.Count; j++)
				{
					if (cubesArray[j].CheckIsBasic())
					{
						cubesArray[j].ExplodeCube();
						RemoveCube(cubesArray[j].id);
						//cubesArray.Remove(cubesArray[j]);
						//sceneController.RemoveCubeFromCount();
						explodedCubes++;
					}

					if (explodedCubes >= cubesForExplosion)
						return;
				}
			}
		}
	}
    #endregion

    #region Prepare_Model

    public IEnumerator CheckVoxPosition()
	{
		Debug.Log("start check vox positions");

		SwitchMeshRenderer(false);

		yield return new WaitForSeconds(0.01f);

		List<CubePrefab> voxels = new List<CubePrefab>(cubesArray);
		int oldCount = voxels.Count;
		//StartCoroutine(VoxImporter.CheckRotateDirection(voxels));

		while (voxels.Count > 0)
		{
			voxels = CheckVoxelMovement(voxels);

			yield return null;
		}

		//if (SceneController.lvlNum != 2)
		//	SwitchVoxColliders(true);
		//else
		//	SwitchVoxColliders(false);

		preparedCubes = cubesArray.Count;
		
		SpawnCubes();
		EventBus.updateLoadingDebugText?.Invoke(0);

		Debug.Log("vox positions checked");
	}

	private List<CubePrefab> CheckVoxelMovement(List<CubePrefab> _voxels)
	{
		Debug.Log($"start check vox movement. array lenght = {_voxels.Count}");

		List<CubePrefab> voxels = _voxels;

        for (int i = 0; i < voxels.Count; i++)
        {
            if (voxels[i].CheckCanBeDeleted())
            {
                voxels[i].SwitchCollider(false);
                voxels.RemoveAt(i);
            }
            else
            {
                foreach (CubePrefab cube in voxels)
                {
                    cube.transform.rotation = VoxImporter.RandomRotation();
                }

                EventBus.updateLoadingDebugText?.Invoke(cubesArray.Count - voxels.Count);
                VoxImporter.CheckRotateDirection(voxels);
            }
        }

		//if(voxels.Count > 0)
  //      {
		//	EventBus.updateLoadingDebugText?.Invoke(cubesArray.Count - voxels.Count);
		//	StartCoroutine(VoxImporter.CheckRotateDirection(voxels));
		//	voxels = CheckVoxelMovement(voxels);
		//}

        return voxels;
	}
	#endregion

	#region Collecting Model

	private void UpdatedPreparedCubes() 
	{ 
		preparedCubes -= 1; 

		if(preparedCubes <= 0)
        {
			EventBus.switchModelActive?.Invoke(true);

			if(SceneController.lvlNum != 2)
				SwitchCubesClickOn();
		}
	}

	private void SpawnCubes()
	{
		foreach (CubePrefab cube in cubesArray)
		{
			cube.SwitchMesh(true);
			Vector3 randomCircle = Random.insideUnitCircle * gameSettings.spawnCollectingRadius;
			cube.StartCollectAnimation(randomCircle);
		}

		Debug.Log("model is spawned");
	}
    #endregion

	#region Play_Check

	public void PlayCheck()
	{
		List<CubePrefab> voxels = new List<CubePrefab>(cubesArray);

		StartCoroutine(CheckModel(voxels));
	}

	private IEnumerator CheckModel(List<CubePrefab> _voxels)
	{
		List<CubePrefab> voxels = _voxels;

		if (voxels != null && voxels.Count > 0)
		{
			bool canMove = false;

			foreach (CubePrefab vox in voxels)
			{
				if (vox.CheckCanBeDeleted())
					canMove = true;
			}

			if (canMove)
			{
				for (int i = 0; i < voxels.Count; i++)
				{
					if (voxels[i].CheckCanBeDeleted())
					{
						StartCoroutine(voxels[i].MoveUpAndDisappear());
						voxels.RemoveAt(i);
					}
				}
			}

			yield return new WaitForSeconds(0.3f);

			StartCoroutine(CheckModel(voxels));
		}
		else
		{
			StopCoroutine(CheckModel(null));
			yield break;
		}
	}
	#endregion

	#region Save/Load

	private bool CheckCubeInSave(int id) { return System.Array.Exists(savedCubesIDArray, element => element == id); }
    
	private void Save()
	{
		savedCubesIDArray = new int[cubesArray.Count];
		savedCubesStatusArray = new int[cubesArray.Count];
			
		for (int i = 0; i < cubesArray.Count; i++)
        {
			savedCubesIDArray[i] = cubesArray[i].id;
			savedCubesStatusArray[i] = cubesArray[i].statusCode;
		}

		save.SaveModel(savedCubesIDArray, savedCubesStatusArray);
	}

	private void Load()
    {
		save = new ModelSave();
		save.Load();

		savedCubesIDArray = save.savedCubesID;
		savedCubesStatusArray = save.savedCubesStatus;
	}
    #endregion

    private void OnDestroy()
    {
		EventBus.onCubeDestroyed -= RemoveCube;
		EventBus.removeCubeByID -= RemoveCubeBuyID;
		EventBus.activateBomb -= ExplodeCubes;
		EventBus.updatePraparedCubes -= UpdatedPreparedCubes;
	}
}
