using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class c : MonoBehaviour
{
    public string apiUrl = "URL_DE_TU_API";
    public Transform arAnchor;

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
                LoadObj(objText);
            }
        }
    }

    void LoadObj(string objText)
    {
        ObjImporter objImporter = new ObjImporter();
        Mesh mesh = objImporter.ImportFile(objText);

        GameObject objModel = new GameObject("LoadedObjModel");
        MeshFilter meshFilter = objModel.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = objModel.AddComponent<MeshRenderer>();

        meshFilter.mesh = mesh;
        meshRenderer.material = new Material(Shader.Find("Standard"));

        objModel.transform.SetParent(arAnchor, false);
    }
}

public class ObjImporter
{
    public Mesh ImportFile(string objText)
    {
        // Aquí implementas la lógica para cargar el archivo .obj y convertirlo en un Mesh
        // Puedes usar la implementación de SimpleObjImporter o una similar
        // Este es un ejemplo simplificado
        Mesh mesh = new Mesh();
        // Implementa la lógica para convertir objText en un Mesh
        return mesh;
    }
}