using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ARObjLoader : MonoBehaviour
{
    public string apiUrl = "URL_DE_TU_API";
    public Transform buttonContainer;
    public GameObject buttonPrefab; // Prefab para los botones de ítems
    public ARInteractionsManager arInteractionsManager;

    void Start()
    {
        StartCoroutine(GetObjListFromAPI());
    }

    IEnumerator GetObjListFromAPI()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(apiUrl))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to fetch .obj list: " + www.error);
            }
            else
            {
                List<string> objUrls = ParseObjList(www.downloadHandler.text);
                foreach (string objUrl in objUrls)
                {
                    StartCoroutine(DownloadAndLoadObj(objUrl));
                }
            }
        }
    }

    List<string> ParseObjList(string jsonResponse)
    {
        // Suponiendo que la respuesta es un JSON array de URLs de archivos .obj
        return JsonUtility.FromJson<List<string>>(jsonResponse);
    }

    IEnumerator DownloadAndLoadObj(string objUrl)
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
                byte[] objData = www.downloadHandler.data;
                string objText = System.Text.Encoding.UTF8.GetString(objData);
                GameObject objModel = LoadObj(objText);
                CreateItemButton(objModel, objUrl);
            }
        }
    }

    GameObject LoadObj(string objText)
    {
        ObjImporter objImporter = new ObjImporter();
        Mesh mesh = objImporter.ImportFile(objText);

        GameObject objModel = new GameObject("LoadedObjModel");
        MeshFilter meshFilter = objModel.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = objModel.AddComponent<MeshRenderer>();

        meshFilter.mesh = mesh;
        meshRenderer.material = new Material(Shader.Find("Standard"));

        return objModel;
    }

    void CreateItemButton(GameObject objModel, string objUrl)
    {
        GameObject newItemButton = Instantiate(buttonPrefab, buttonContainer);
        ItemButtonManager buttonManager = newItemButton.GetComponent<ItemButtonManager>();
        buttonManager.Item3DModel = objModel;
        buttonManager.interactionsManager = arInteractionsManager;
        buttonManager.objUrl = objUrl; // Para referencia futura si es necesario
    }
}

public class ObjImporter
{
    public Mesh ImportFile(string objText)
    {
        // Implementación simplificada para convertir objText en un Mesh
        Mesh mesh = new Mesh();
        // Lógica para convertir objText en un Mesh
        return mesh;
    }
}
