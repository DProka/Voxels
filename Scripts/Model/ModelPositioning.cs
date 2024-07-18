
using UnityEngine;

public class ModelPositioning
{
    private Transform modelTransform;
    private float sensitivity = 0.5f;
    private Vector2 lastInputPosition;

    public ModelPositioning(Transform transform, float sens)
    {
        modelTransform = transform;
        sensitivity = sens;
    }

    public void UpdateScript()
    {
        if (SceneController.lvlNum >= 3 && SceneController.modelIsActive)
        {

#if UNITY_EDITOR

            MouseInput();

#endif

#if UNITY_ANDROID

            TouchInput();

#endif
        }
    }

    void MouseInput()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 mousePosition = Input.mousePosition;

            if (Input.GetMouseButtonDown(0))
            {
                lastInputPosition = mousePosition;
            }
            else if (Input.GetMouseButton(0))
            {
                Vector2 deltaMouse = mousePosition - lastInputPosition;

                float rotationY = -deltaMouse.x * sensitivity;
                float rotationX = deltaMouse.y * sensitivity;

                modelTransform.Rotate(Vector3.up, rotationY, Space.World);
                modelTransform.Rotate(Vector3.right, rotationX, Space.World);

                lastInputPosition = mousePosition;
            }

            if(SceneController.lvlNum == 3)
                EventBus.closeTutorialScreen?.Invoke();
        }
    }

    void TouchInput()
    {
        if (Input.touchCount == 1 && SceneController.rotationStatus == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                lastInputPosition = touch.position;
            }

            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 deltaTouch = touch.position - lastInputPosition;

                float rotationY = -deltaTouch.x * sensitivity;
                float rotationX = deltaTouch.y * sensitivity;

                modelTransform.Rotate(Vector3.up, rotationY, Space.World);
                modelTransform.Rotate(Vector3.right, rotationX, Space.World);

                lastInputPosition = touch.position;
            }

            if (SceneController.lvlNum == 3)
                EventBus.closeTutorialScreen?.Invoke();
        }
    }
}
