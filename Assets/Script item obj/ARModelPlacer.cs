using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class ARModelPlacer : MonoBehaviour
{
    private ARRaycastManager raycastManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private List<GameObject> spawnedModels = new List<GameObject>();

    void Start()
    {
        raycastManager = FindObjectOfType<ARRaycastManager>();
    }

    public void PlaceModel(GameObject modelPrefab)
    {
        if (raycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.Planes))
        {
            Pose hitPose = hits[0].pose;

            GameObject spawnedModel = Instantiate(modelPrefab, hitPose.position, hitPose.rotation);
            spawnedModels.Add(spawnedModel);
        }
    }
}
