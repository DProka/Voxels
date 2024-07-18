
using UnityEngine;

public class CubeItem : MonoBehaviour
{
    private MeshRenderer itemMesh;
    private float rotationSpeed;
    private bool isActive;

    public void Init(float _rotationSpeed)
    {
        itemMesh = GetComponentInChildren<MeshRenderer>();
        rotationSpeed = _rotationSpeed;
        SwitchActive(false);
    }

    public void SwitchActive(bool _isActive)
    {
        isActive = _isActive;
        itemMesh.enabled = _isActive;
    }

    public void SetMeshColor(Color newColor)
    {
        Mesh mesh = GetComponentInChildren<MeshFilter>().mesh;

        Color[] colors = new Color[mesh.vertices.Length];

        for (var i = 0; i < colors.Length; i++)
        {
            colors[i] = newColor;
            Debug.Log(colors[i]);
        }

        mesh.colors = colors;
    }

    private void Update()
    {
        if(isActive)
            transform.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
