using System.Collections;
using UnityEngine;
using Spine.Unity;
using DG.Tweening;

public class CubePrefab : MonoBehaviour
{
    public int id { get; private set; }
    public int statusCode { get; private set; }

    private BoxCollider boxCollider;

    [Header("Prefab Settings")]
    
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float lifeTime = 2f;

    private Color baseColor = Color.red;
    private TrailRenderer trail;
    private bool canBeClicked;
    private Transform obstaclePosition;
    private bool isMoving;
    private CubeState currentState;

    [Header("Disappearing")]

    [SerializeField] MeshRenderer explosionMesh;
    [SerializeField] SkeletonAnimation explosionSpine;

    [SerializeField] float scaleSpeed = 0.1f;
    [SerializeField] float minScale = 0.1f;

    [Header("Basic")]

    [SerializeField] CubeBasic basicCube;

    [Header("Items")]

    [SerializeField] CubeGlass glassCube;
    [SerializeField] float itemRotateSpeed;

    public void Init()
    {
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.enabled = true;
        trail = GetComponent<TrailRenderer>();
        explosionMesh.enabled = false;

        canBeClicked = false;
        basicCube.Init(this, moveSpeed, scaleSpeed);
        glassCube.Init(scaleSpeed, itemRotateSpeed);

        SetCubeState(CubeState.Basic);
        SwitchTrail(false);
    }

    private void OnMouseDown()
    {

#if UNITY_EDITOR

            CheckCubeState();
#endif

#if UNITY_ANDROID

            if (Input.touchCount == 1)
                CheckCubeState();
#endif
        
    }

    #region Main

    public void SetID(int _id) => id = _id;
    public void DestroyCube() => Destroy(gameObject);

    public void SwitchCollider(bool isOn) => boxCollider.enabled = isOn;

    public void SwitchMesh(bool isOn)
    {
        if (isOn)
        {
            if (currentState == CubeState.Basic)
                basicCube.SwitchMesh(true);
            else
                glassCube.SwitchMesh(true);
        }
        else
        {
            basicCube.SwitchMesh(false);
            glassCube.SwitchMesh(false);
        }
    }

    public void SetMeshColor(Color newColor)
    {
        baseColor = newColor;
        trail.startColor = newColor;
        Color endTrailColor = new Color(newColor.r, newColor.g, newColor.b, 0);
        trail.endColor = endTrailColor;

        Mesh mesh = GetComponentInChildren<MeshFilter>().mesh;

        Color[] colors = new Color[mesh.vertices.Length];

        for (var i = 0; i < colors.Length; i++)
        {
            colors[i] = baseColor;
        }

        mesh.colors = colors;
    }

    public bool CheckCanBeDeleted()
    {
        RaycastHit hit;

        if (!Physics.Raycast(transform.position, transform.up, out hit))
            return true;
        
        else
        {
            if (hit.collider.CompareTag("Cube"))
                obstaclePosition = hit.transform;

            return false;
        }
    }

    public bool CheckIsMoving() { return isMoving; }

    public void TurnClickOn() => canBeClicked = true;

    private void SwitchTrail(bool isActive) => trail.enabled = isActive;

    #endregion

    #region Explode Animation

    public void ExplodeCube()
    {
        StartCoroutine(StartExplodeAnimation());
    }

    private IEnumerator StartExplodeAnimation()
    {
        SwitchMesh(false);
        SwitchCollider(false);

        Vector3 lookDirection = Camera.main.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(lookDirection);

        explosionMesh.enabled = true;
        explosionSpine.AnimationState.SetAnimation(0, "Explode", false);

        yield return new WaitForSeconds(1.3f);

        DestroyCube();
    }

    #endregion

    #region Cube State

    public void SetCubeState(CubeState newState)
    {
        currentState = newState;

        if(newState == CubeState.Basic)
            statusCode = 0;
        else
            StartCoroutine(ActivateItem());
    }

    public bool CheckIsBasic() { return currentState == CubeState.Basic; }
    
    public enum CubeState
    {
        Basic,
        Coin,
        Puzzle,
        Bomb
    }

    private void CheckCubeState()
    {
        if (SceneController.modelIsActive && canBeClicked && !isMoving)
        {
            if(currentState == CubeState.Basic)
                StartCoroutine(MoveUpAndDisappear());
            else
                ActivateGlassCube();
        }
    }
    #endregion

    #region Basic Cube

    public IEnumerator MoveUpAndDisappear()
    {
        if (CheckCanBeDeleted())
        {
            SwitchTrail(true);
            transform.SetParent(SceneController.controllerTransform);
            GetComponent<Collider>().enabled = false;

            EventBus.onCubeDestroyed?.Invoke(id);

            float elapsedTime = 0f;
            float moveDuration = lifeTime;

            while (elapsedTime < moveDuration)
            {
                transform.Translate(Vector3.up * Time.deltaTime * moveSpeed);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(StartBlockedCubeAnimation());
        }
    }

    public void StartCollectAnimation(Vector3 startPos) => basicCube.StartCollectAnimation(startPos, transform.position);

    private IEnumerator StartBlockedCubeAnimation()
    {
        if (obstaclePosition != null)
        {
            CubePrefab hittedCube = obstaclePosition.GetComponent<CubePrefab>();
            isMoving = true;
            basicCube.StartMovingAnimation(hittedCube.transform, hittedCube.CheckIsMoving());

            while (basicCube.CheckIsMoving())
            {
                yield return null;
            }

            isMoving = false;
        }
    }
    #endregion

    #region Glass Cube

    private void ActivateGlassCube()
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);

        switch (currentState)
        {
            case CubeState.Coin:
                EventBus.callCoinRewardScreen?.Invoke(screenPosition.x, screenPosition.y);
                statusCode = 1;
                break;

            case CubeState.Puzzle:
                EventBus.callPuzzleRewardScreen?.Invoke(screenPosition.x, screenPosition.y);
                statusCode = 2;
                break;
        
            case CubeState.Bomb:
                EventBus.callBombRewardScreen?.Invoke(screenPosition.x, screenPosition.y);
                statusCode = 3;
                break;
        }

        StartCoroutine(Disappear());
    }

    private IEnumerator ActivateItem()
    {
        basicCube.SwitchOffCube();

        yield return new WaitForSeconds(scaleSpeed);

        switch (currentState)
        {
            case CubeState.Coin:
                glassCube.SwitchActive(true, CubeGlass.State.Coin);
                break;

            case CubeState.Puzzle:
                glassCube.SwitchActive(true, CubeGlass.State.Puzzle);
                break;

            case CubeState.Bomb:
                glassCube.SwitchActive(true, CubeGlass.State.Bomb);
                break;
        }
    }

    private IEnumerator Disappear()
    {
        transform.DOScale(minScale, scaleSpeed);

        yield return new WaitForSeconds(scaleSpeed);

        EventBus.onCubeDestroyed?.Invoke(id);
        Destroy(gameObject);
    }
    #endregion
}
