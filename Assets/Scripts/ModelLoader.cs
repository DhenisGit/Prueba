using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ModelLoader : MonoBehaviour
{
    public string bundleURL;
    public string assetName;
    public Transform spawnPoint;

    public static ModelLoader instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void LoadModel()
    {
        StartCoroutine(DownloadAndCache());
    }

    private IEnumerator DownloadAndCache()
    {
        while (!Caching.ready)
        {
            yield return null;
        }

        UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(bundleURL);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to download AssetBundle: " + www.error);
            yield break;
        }

        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);

        if (bundle != null)
        {
            GameObject model = bundle.LoadAsset<GameObject>(assetName);
            if (model != null)
            {
                Instantiate(model, spawnPoint.position, spawnPoint.rotation);
            }

            bundle.Unload(false);
        }
    }
}
