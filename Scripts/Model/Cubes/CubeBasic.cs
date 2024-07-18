using System.Collections;
using UnityEngine;
using DG.Tweening;

public class CubeBasic : MonoBehaviour
{
    //private CubePrefab cubePrefab;
    private MeshRenderer meshRenderer;
    private float moveSpeed;
    //private Vector3 startPosition;
    private bool isMoving;
    private float scaleSpeed;

    public void Init(CubePrefab _cubePrefab, float _moveSpeed, float _scaleSpeed)
    {
        meshRenderer = GetComponent<MeshRenderer>();
        //cubePrefab = _cubePrefab;
        moveSpeed = _moveSpeed;
        isMoving = false;
        scaleSpeed = _scaleSpeed;
    }

    public void SwitchMesh(bool isActive) => meshRenderer.enabled = isActive;

    #region CollectAnim

    public void StartCollectAnimation(Vector3 startPos, Vector3 finishPos)
    {
        transform.position = startPos;

        StartCoroutine(CollectAnimation(finishPos));
    }

    private IEnumerator CollectAnimation(Vector3 finishPosition)
    {
        while (Vector3.Distance(finishPosition, transform.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, finishPosition, moveSpeed * 0.8f * Time.deltaTime);

            yield return null;
        }

        transform.localPosition = new Vector3(0, 0, 0);
        SwitchMesh(true);
        EventBus.updatePraparedCubes?.Invoke();
        //cubePrefab.UpdatePraparedCubes();
    }
    #endregion

    #region BlockAnim

    public bool CheckIsMoving() { return isMoving; }
    public void StartMovingAnimation(Transform obstacle, bool obstacleIsMoving) => StartCoroutine(StartBlockedCubeAnimation(obstacle, obstacleIsMoving));
    
    private IEnumerator StartBlockedCubeAnimation(Transform obstaclePosition, bool _isMoving)
    {
        float moveDistance = Vector3.Distance(obstaclePosition.position, transform.position);

        if(moveDistance < 1.5)
        {
            transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.2f, 1, 1);
            obstaclePosition.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.2f, 1, 1);
        }
        else
        {
            //startPosition = transform.position;
            isMoving = true;

            while (Vector3.Distance(obstaclePosition.position, transform.position) > 0.9f)
            {
                transform.Translate(Vector3.up * Time.deltaTime * moveSpeed);
                yield return null;
            }

            transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.2f, 1, 1);

            if (!_isMoving)
                obstaclePosition.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.2f, 1, 1);

            while (moveDistance > 0.9f)
            {
                moveDistance -= Time.deltaTime * moveSpeed;
                transform.Translate(Vector3.down * Time.deltaTime * moveSpeed);
                yield return null;
            }

            transform.localPosition = new Vector3(0, 0, 0);
            isMoving = false;
        }
    }
    #endregion

    public void SwitchOffCube() => StartCoroutine(Disappear());

    private IEnumerator Disappear()
    {
        transform.DOScale(0.1f, scaleSpeed);

        yield return new WaitForSeconds(0.1f);

        //while (transform.localScale.x > 0.2f)
        //{
        //    yield return null;
        //}

        SwitchMesh(false);
        //Destroy(gameObject);
    }
}
