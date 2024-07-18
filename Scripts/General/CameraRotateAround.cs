using UnityEngine;
using System.Collections;

public class CameraRotateAround : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    [Header("Position")]

    public float moveRadius = 50f;
    public float moveSpeed = 5f;

    private Transform centerPoint;
    private float radius;

    private Vector3 startPosition;
    private Vector3 lastMousePosition;
    private Vector2 lastTouchPosition1;
    private Vector2 lastTouchPosition2;

    [Header("Zoom")]

    public float zoomSpeedEditor = 0.5f;
    public float zoomSpeedTouch = 0.5f;

    private float zoomMax = 10;
    private float zoomMin = 3;

    private float initialFingerDistance;

    public void Init()
    {
        centerPoint = transform;
        startPosition = transform.position;
    }

    public void UpdateScript()
    {

#if UNITY_EDITOR

        UpdateInEditor();
#endif

#if UNITY_ANDROID

        UpdateOnAndroid();
#endif

    }

    public void ResetPosition()
    {
        transform.position = startPosition;
    }

    public void SetCameraSettings(float newMaxZoom)
    {
        zoomMin = 3f;
        zoomMax = newMaxZoom + 5;
        offset.z = -newMaxZoom;
        transform.position = transform.localRotation * offset + target.position;
        startPosition = transform.position;
    }

    private void UpdateInEditor()
    {
        if (SceneController.modelIsActive && SceneController.lvlNum > 3)
        {
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                float scrollWheel = Input.GetAxis("Mouse ScrollWheel") * zoomSpeedEditor;

                if (SceneController.lvlNum < 5)
                    EventBus.closeTutorialScreen?.Invoke();

                ZoomCamera(scrollWheel);
            }

            if (Input.GetMouseButtonDown(1))
            {
                lastMousePosition = Input.mousePosition;
            }

            if (Input.GetMouseButton(1))
            {
                Vector3 mouseDelta = Input.mousePosition - lastMousePosition;

                Vector3 moveDelta = new Vector3(-mouseDelta.x * moveSpeed, -mouseDelta.y * moveSpeed, 0);
                Vector3 newPosition = transform.position + transform.TransformDirection(moveDelta);

                float distance = Vector3.Distance(startPosition, newPosition);
                Debug.Log("Distance between positions " + distance);

                if (distance < moveRadius)
                {
                    Debug.Log("Distance between positions " + Vector3.Distance(startPosition, newPosition));
                    transform.position = newPosition;
                }

                lastMousePosition = Input.mousePosition;
            }
        }

    }

    private void UpdateOnAndroid()
    {
        if (SceneController.modelIsActive && Input.touchCount == 2 && SceneController.lvlNum > 3)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            if (touch0.phase == TouchPhase.Began || touch1.phase == TouchPhase.Began)
            {
                lastTouchPosition1 = touch0.position;
                lastTouchPosition2 = touch1.position;
            }

            else if (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved)
            {
                Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
                Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

                float prevMagnitude = (touch0PrevPos - touch1PrevPos).magnitude;
                float currentMagnitude = (touch0.position - touch1.position).magnitude;

                float difference = currentMagnitude - prevMagnitude;
                float zoom = difference * zoomSpeedTouch;

                ZoomCamera(zoom);

                if (SceneController.lvlNum < 5)
                    EventBus.closeTutorialScreen?.Invoke();

                Vector2 touch1Delta = touch0.position - lastTouchPosition1;
                Vector2 touch2Delta = touch1.position - lastTouchPosition2;

                // Среднее смещение обоих пальцев
                Vector2 averageDelta = (touch1Delta + touch2Delta) / 2;

                // Перемещение объекта по осям X и Y
                Vector3 moveDelta = new Vector3(-averageDelta.x * moveSpeed, -averageDelta.y * moveSpeed, 0);
                Vector3 newPosition = transform.position + transform.TransformDirection(moveDelta);

                // Проверяем, находится ли новый объект в пределах радиуса
                if (Vector3.Distance(startPosition, newPosition) <= moveRadius)
                {
                    transform.position = newPosition;
                }

                lastTouchPosition1 = touch0.position;
                lastTouchPosition2 = touch1.position;
            }
        }
    }

    private bool CheckMoveRadius(Vector3 newPosition) { return Vector3.Distance(startPosition, newPosition) < moveRadius; }

    private void ZoomCamera(float increment) => Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView - increment, 20, 70);
}
