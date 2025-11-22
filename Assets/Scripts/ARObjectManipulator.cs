using UnityEngine;

public class ARObjectManipulator : MonoBehaviour
{
    [Header("Manipulation Settings")]
    public float rotationSpeed = 50f;
    public float scaleSpeed = 0.01f;
    public float minScale = 0.1f;
    public float maxScale = 3.0f;

    private Vector2 lastTouchPosition;
    private bool isManipulating = false;
    private Camera arCamera;

    void Start()
    {
        arCamera = Camera.main;
        gameObject.tag = "PlacedObject";
    }

    void Update()
    {
        if (Input.touchCount == 2)
        {
            HandleTwoFingerGestures();
        }
        else if (Input.touchCount == 1)
        {
            HandleOneFingerRotation();
        }
        else
        {
            isManipulating = false;
        }
    }

    void HandleOneFingerRotation()
    {
        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began)
        {
            Ray ray = arCamera.ScreenPointToRay(touch.position);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    isManipulating = true;
                    lastTouchPosition = touch.position;
                }
            }
        }
        if (touch.phase == TouchPhase.Moved && isManipulating)
        {
            Vector2 delta = touch.position - lastTouchPosition;
            transform.Rotate(Vector3.up, delta.x * rotationSpeed * Time.deltaTime, Space.World);
            lastTouchPosition = touch.position;
        }
    }

    void HandleTwoFingerGestures()
    {
        Touch touch0 = Input.GetTouch(0);
        Touch touch1 = Input.GetTouch(1);

        Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
        Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

        float prevMagnitude = (touch0PrevPos - touch1PrevPos).magnitude;
        float currentMagnitude = (touch0.position - touch1.position).magnitude;

        float difference = currentMagnitude - prevMagnitude;

        Vector3 newScale = transform.localScale + Vector3.one * difference * scaleSpeed;
        newScale = Vector3.Max(newScale, Vector3.one * minScale);
        newScale = Vector3.Min(newScale, Vector3.one * maxScale);
        transform.localScale = newScale;
    }

    void OnMouseDown()
    {
    }
}