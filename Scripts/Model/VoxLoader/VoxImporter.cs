using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public struct FaceCollection
{
	public byte[,,] colorIndices;
}

[System.Serializable]
public struct Voxel {
	public byte x, y, z, colorIndex;
}

[System.Serializable]
public class VoxelChunk
{
	// all voxels
	public byte[,,] voxels;

	// 6 dir, x+. x-, y+, y-, z+, z-
	public FaceCollection[] faces;

	public int x = 0, y = 0, z = 0;

	public int sizeX { get { return voxels.GetLength (0); } }
	public int sizeY { get { return voxels.GetLength (1); } }
	public int sizeZ { get { return voxels.GetLength (2); } }
}

public enum FaceDir
{
	XPos = 0,
	XNeg = 1,
	YPos = 2,
	YNeg = 3,
	ZPos = 4,
	ZNeg = 5
}
	
[System.Serializable]
public class MainChunk
{
	public VoxelChunk voxelChunk;

	public Color[] palatte;

	public int sizeX, sizeY, sizeZ;

	public byte[] version;
}

public static class VoxImporter
{
	public static MainChunk LoadVOXFromData(byte[] data, bool generateFaces = true)
	{
		using (MemoryStream ms = new MemoryStream(data))
		{
			using (BinaryReader br = new BinaryReader(ms))
			{
				MainChunk mainChunk = new MainChunk();

				// "VOX "
				br.ReadInt32();
				// "VERSION "
				mainChunk.version = br.ReadBytes(4);

				byte[] chunkId = br.ReadBytes(4);
				if (chunkId[0] != 'M' ||
					chunkId[1] != 'A' ||
					chunkId[2] != 'I' ||
					chunkId[3] != 'N')
				{
					Debug.LogError("[MVImport] Invalid MainChunk ID, main chunk expected");
					return null;
				}

				int chunkSize = br.ReadInt32();
				int childrenSize = br.ReadInt32();

				// main chunk should have nothing... skip
				br.ReadBytes(chunkSize);

				int readSize = 0;
				while (readSize < childrenSize)
				{

					chunkId = br.ReadBytes(4);

					if (chunkId[0] == 'P' &&
						chunkId[1] == 'A' &&
						chunkId[2] == 'C' &&
						chunkId[3] == 'K')
					{

						int chunkContentBytes = br.ReadInt32();
						int childrenBytes = br.ReadInt32();

						br.ReadInt32();

						readSize += chunkContentBytes + childrenBytes + 4 * 3;
					}
					else if (chunkId[0] == 'S' &&
						chunkId[1] == 'I' &&
						chunkId[2] == 'Z' &&
						chunkId[3] == 'E')
					{

						readSize += ReadSizeChunk(br, mainChunk);

					}
					else if (chunkId[0] == 'X' &&
					  chunkId[1] == 'Y' &&
					  chunkId[2] == 'Z' &&
					  chunkId[3] == 'I')
					{

						readSize += ReadVoxelChunk(br, mainChunk.voxelChunk);

					}
					else if (chunkId[0] == 'R' &&
					  chunkId[1] == 'G' &&
					  chunkId[2] == 'B' &&
					  chunkId[3] == 'A')
					{

						mainChunk.palatte = new Color[256];
						readSize += ReadPalattee(br, mainChunk.palatte);

					}
					else
					{
						//Debug.LogWarning("[MVImport] Chunk ID not recognized, got " + System.Text.Encoding.ASCII.GetString(chunkId));
						int chunkContentBytes = br.ReadInt32();
						int childrenBytes = br.ReadInt32();
						br.ReadBytes(chunkContentBytes + childrenBytes);
						readSize += chunkContentBytes + childrenBytes + 12;
					}
				}

				if (generateFaces)
					GenerateFaces(mainChunk.voxelChunk);

				return mainChunk;
			}
		}
	}

	public static MainChunk LoadVOX(byte[] _bytes, bool generateFaces = true) //string path, byte[] _bytes, bool generateFaces = true)
	{
		//byte[] bytes = File.ReadAllBytes (path);
		byte[] bytes = _bytes;

		if (bytes[0] != 'V' ||
			bytes[1] != 'O' ||
			bytes[2] != 'X' ||
			bytes[3] != ' ')
		{
			Debug.LogError("Invalid VOX file, magic number mismatch");
			return null;
		}

		return LoadVOXFromData(bytes, generateFaces);
	}

	public static void GenerateFaces(VoxelChunk voxelChunk)
	{
		voxelChunk.faces = new FaceCollection[6];
		for (int i = 0; i < 6; ++i)
		{
			voxelChunk.faces[i].colorIndices = new byte[voxelChunk.sizeX, voxelChunk.sizeY, voxelChunk.sizeZ];
		}

		for (int x = 0; x < voxelChunk.sizeX; ++x)
		{
			for (int y = 0; y < voxelChunk.sizeY; ++y)
			{
				for (int z = 0; z < voxelChunk.sizeZ; ++z)
				{

					// left right
					if (x == 0 || voxelChunk.voxels[x - 1, y, z] == 0)
						voxelChunk.faces[(int)FaceDir.XNeg].colorIndices[x, y, z] = voxelChunk.voxels[x, y, z];

					if (x == voxelChunk.sizeX - 1 || voxelChunk.voxels[x + 1, y, z] == 0)
						voxelChunk.faces[(int)FaceDir.XPos].colorIndices[x, y, z] = voxelChunk.voxels[x, y, z];

					// up down
					if (y == 0 || voxelChunk.voxels[x, y - 1, z] == 0)
						voxelChunk.faces[(int)FaceDir.YNeg].colorIndices[x, y, z] = voxelChunk.voxels[x, y, z];

					if (y == voxelChunk.sizeY - 1 || voxelChunk.voxels[x, y + 1, z] == 0)
						voxelChunk.faces[(int)FaceDir.YPos].colorIndices[x, y, z] = voxelChunk.voxels[x, y, z];

					// forward backward
					if (z == 0 || voxelChunk.voxels[x, y, z - 1] == 0)
						voxelChunk.faces[(int)FaceDir.ZNeg].colorIndices[x, y, z] = voxelChunk.voxels[x, y, z];

					if (z == voxelChunk.sizeZ - 1 || voxelChunk.voxels[x, y, z + 1] == 0)
						voxelChunk.faces[(int)FaceDir.ZPos].colorIndices[x, y, z] = voxelChunk.voxels[x, y, z];
				}
			}
		}
	}

	static int ReadSizeChunk(BinaryReader br, MainChunk mainChunk)
	{
		int chunkSize = br.ReadInt32();
		int childrenSize = br.ReadInt32();

		mainChunk.sizeX = br.ReadInt32();
		mainChunk.sizeZ = br.ReadInt32();
		mainChunk.sizeY = br.ReadInt32();

		mainChunk.voxelChunk = new VoxelChunk();
		mainChunk.voxelChunk.voxels = new byte[mainChunk.sizeX, mainChunk.sizeY, mainChunk.sizeZ];

		Debug.Log(string.Format("[MVImporter] Voxel Size {0}x{1}x{2}", mainChunk.sizeX, mainChunk.sizeY, mainChunk.sizeZ));

		if (childrenSize > 0)
		{
			br.ReadBytes(childrenSize);
			Debug.LogWarning("[MVImporter] Nested chunk not supported");
		}

		return chunkSize + childrenSize + 4 * 3;
	}

	static int ReadVoxelChunk(BinaryReader br, VoxelChunk chunk)
	{
		int chunkSize = br.ReadInt32();
		int childrenSize = br.ReadInt32();
		int numVoxels = br.ReadInt32();

		for (int i = 0; i < numVoxels; ++i)
		{
			int x = (int)br.ReadByte();
			int z = (int)br.ReadByte();
			int y = (int)br.ReadByte();

			chunk.voxels[x, y, z] = br.ReadByte();
		}

		if (childrenSize > 0)
		{
			br.ReadBytes(childrenSize);
			Debug.LogWarning("[MVImporter] Nested chunk not supported");
		}

		return chunkSize + childrenSize + 4 * 3;
	}

	static int ReadPalattee(BinaryReader br, Color[] colors)
	{
		int chunkSize = br.ReadInt32();
		int childrenSize = br.ReadInt32();

		for (int i = 0; i < 256; ++i)
		{
			colors[i] = new Color((float)br.ReadByte() / 255.0f, (float)br.ReadByte() / 255.0f, (float)br.ReadByte() / 255.0f, (float)br.ReadByte() / 255.0f);
		}

		if (childrenSize > 0)
		{
			br.ReadBytes(childrenSize);
			Debug.LogWarning("[MVImporter] Nested chunk not supported");
		}

		return chunkSize + childrenSize + 4 * 3;
	}

	public static GameObject[] CreateIndividualVoxelGameObjects(MainChunk chunk, Transform parent, float sizePerVox)
	{
		return CreateIndividualVoxelGameObjectsForChunk(chunk.voxelChunk, chunk.palatte, parent, sizePerVox);
	}

	public static GameObject[] CreateIndividualVoxelGameObjectsForChunk(VoxelChunk chunk, Color[] palette, Transform parent, float sizePerVox)
	{
		Vector3 origin = new Vector3(
			(float)chunk.sizeX / 2,
			(float)chunk.sizeY / 2,
			(float)chunk.sizeZ / 2);

		List<GameObject> result = new List<GameObject>();

		List<CubePrefab> cubes = new List<CubePrefab>();
		List<Color> uniqueColorList = new List<Color>();

		for (int x = 0; x < chunk.sizeX; ++x)
		{
			for (int y = 0; y < chunk.sizeY; ++y)
			{
				for (int z = 0; z < chunk.sizeZ; ++z)
				{
					if (chunk.voxels[x, y, z] != 0)
					{
						float px = (x - origin.x + 0.5f) * sizePerVox, py = (y - origin.y + 0.5f) * sizePerVox, pz = (z - origin.z + 0.5f) * sizePerVox;
						int cidx = chunk.voxels[x, y, z];

						GameObject go = Object.Instantiate(VoxModel._voxelPrefab.gameObject, parent);
						go.transform.position = new Vector3(px, py, pz);
						go.name = string.Format("Voxel ({0}, {1}, {2})", x, y, z);

						CubePrefab cube = go.GetComponentInChildren<CubePrefab>();
						cube.Init();
						cubes.Add(cube);

						if (!uniqueColorList.Contains(palette[cidx - 1]))
						{
							uniqueColorList.Add(palette[cidx - 1]);
						}

						cube.SetMeshColor(palette[cidx - 1]);

						result.Add(go);
					}
				}
			}
		}

		GameObject[] objArray = result.ToArray();

		if(SceneController.lvlNum >= 2)
			RotateCubesRandom(cubes.ToArray(), 1);

		return objArray;
	}

    public static void RotateCubesRandom(CubePrefab[] voxelObjects, int seed)
    {
        if (voxelObjects == null || voxelObjects.Length == 0)
        {
            Debug.LogError("Voxel objects array is null or empty.");
            return;
        }

        Random.InitState(seed);

        foreach (CubePrefab cube in voxelObjects)
        {
            cube.transform.rotation = RandomRotation();
		}
    }

	public static Quaternion RandomRotation()
	{
		float angleX = Random.Range(0, 4) * 90.0f;
		float angleY = Random.Range(0, 4) * 90.0f;
		float angleZ = Random.Range(0, 4) * 90.0f;

		return Quaternion.Euler(angleX, angleY, angleZ);
	}

    public static IEnumerator CheckRotateDirection(List<CubePrefab> cubes)
    {
        yield return new WaitForSeconds(0.1f);

        foreach (CubePrefab cube in cubes)
        {
            CheckCube(cube.transform);
        }
    }

    private static void CheckCube(Transform cubeToCheck)
	{
		if (cubeToCheck == null)
		{
			Debug.LogError("Cube is null.");
			return;
		}

		RaycastHit[] hits = Physics.RaycastAll(cubeToCheck.transform.position, cubeToCheck.up, float.MaxValue);

		foreach (RaycastHit hit in hits)
		{
			Transform neighborCube = hit.transform;

			if (-neighborCube.up == cubeToCheck.up)
			{
				neighborCube.transform.rotation = RandomRotation();
				CheckCube(neighborCube);
			}
		}
	}
}

