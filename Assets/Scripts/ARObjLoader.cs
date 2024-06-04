using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ARObjLoader : MonoBehaviour
{
    public string apiUrl = "https://back.igvperu.com/public/api/cliente/maquinas/get";
    public Transform arAnchor;

    public IEnumerator GetObjListFromAPI()
    {
        Debug.Log("Starting coroutine to get object list from API.");
        using (UnityWebRequest www = UnityWebRequest.Get(apiUrl))
        {
            if (!string.IsNullOrEmpty(LoginController.authToken))
            {
                www.SetRequestHeader("Authorization", "Bearer " + LoginController.authToken);
            }
            else
            {
                Debug.LogError("Token not found.");
                yield break;
            }

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to fetch .obj list: " + www.error);
            }
            else
            {
                Debug.Log("API Response: " + www.downloadHandler.text);
                ApiResponse response = JsonUtility.FromJson<ApiResponse>(www.downloadHandler.text);
                FindObjectOfType<DataManager>().CreateButtons(response.data);
                foreach (var dataItem in response.data)
                {
                    string correctedUrl = dataItem.maquina.url_objeto.Replace("\\", "/");
                    Debug.Log("Downloading .obj file from: " + correctedUrl);
                    StartCoroutine(DownloadAndLoadObj(correctedUrl, dataItem.maquina.nombre));
                }
            }
        }
    }

    public IEnumerator DownloadAndLoadObj(string objUrl, string itemName)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(objUrl))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to download .obj file: " + www.error);
            }
            else
            {
                Debug.Log("Downloaded .obj file: " + objUrl);
                byte[] objData = www.downloadHandler.data;
                string objText = System.Text.Encoding.UTF8.GetString(objData);
                LoadObj(objText, itemName);
            }
        }
    }

    void LoadObj(string objText, string itemName)
    {
        Debug.Log("Loading .obj file content.");
        ObjFileImporter objImporter = new ObjFileImporter();
        Mesh mesh = objImporter.ImportFile(objText);

        if (mesh == null)
        {
            Debug.LogError("Failed to load mesh from obj file.");
            return;
        }

        GameObject objModel = new GameObject(itemName);
        MeshFilter meshFilter = objModel.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = objModel.AddComponent<MeshRenderer>();

        meshFilter.mesh = mesh;
        meshRenderer.material = new Material(Shader.Find("Standard"));

        objModel.transform.position = arAnchor.position + arAnchor.forward * 0.5f;
        objModel.transform.SetParent(arAnchor, false);
        objModel.SetActive(false); // Se oculta el modelo hasta que el usuario lo seleccione

        Debug.Log("Model loaded and placed in scene.");
    }
}
