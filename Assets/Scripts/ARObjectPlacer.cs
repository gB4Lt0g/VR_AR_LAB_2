using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class ARObjectPlacer : MonoBehaviour
{
    [Header("AR Components")]
    public ARRaycastManager raycastManager;
    public ARPlaneManager planeManager;

    [Header("Prefab to Place")]
    public GameObject objectPrefab;

    [Header("Settings")]
    public bool placeModeEnabled = true;

    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private GameObject lastPlacedObject;

    void Update()
    {
        if (!placeModeEnabled) return;

#if ENABLE_INPUT_SYSTEM
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();

            if (Touchscreen.current.primaryTouch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
            {
                PlaceObject(touchPosition);
            }
        }

#if UNITY_EDITOR
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            PlaceObject(Mouse.current.position.ReadValue());
        }
#endif

#else
        // Старий Input Manager
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            if (touch.phase == TouchPhase.Began)
            {
                PlaceObject(touch.position);
            }
        }
        
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            PlaceObject(Input.mousePosition);
        }
#endif
#endif
    }

    void PlaceObject(Vector2 screenPosition)
    {
        if (raycastManager.Raycast(screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;
            lastPlacedObject = Instantiate(objectPrefab, hitPose.position, hitPose.rotation);
            Debug.Log($"Object placed at {hitPose.position}");
        }
    }

    public void DeleteLastObject()
    {
        if (lastPlacedObject != null)
        {
            Destroy(lastPlacedObject);
            lastPlacedObject = null;
        }
    }

    public void ClearAllObjects()
    {
        GameObject[] placedObjects = GameObject.FindGameObjectsWithTag("PlacedObject");
        foreach (GameObject obj in placedObjects)
        {
            Destroy(obj);
        }
        lastPlacedObject = null;
    }
}