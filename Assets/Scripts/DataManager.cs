using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DataManager : MonoBehaviour
{
    [SerializeField] private string apiUrl = "https://back.igvperu.com/public/api/cliente/maquinas/get";
    [SerializeField] private GameObject buttonContainer; // Contenedor donde se agregarán los botones
    [SerializeField] private ItemButtonManager itemButtonPrefab; // Prefab del botón
    [SerializeField] private Transform modelSpawnPoint; // Punto de aparición de modelos

    public void LoadItemsFromAPI()
    {
        if (!string.IsNullOrEmpty(LoginController.authToken))
        {
            Debug.Log("Loading items from API.");
            StartCoroutine(GetItemsFromAPI());
        }
        else
        {
            Debug.LogError("Token not found, cannot load items.");
        }
    }

    private IEnumerator GetItemsFromAPI()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(apiUrl))
        {
            www.SetRequestHeader("Authorization", "Bearer " + LoginController.authToken);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to fetch data: " + www.error);
            }
            else
            {
                Debug.Log("API Response: " + www.downloadHandler.text);
                ApiResponse response = JsonUtility.FromJson<ApiResponse>(www.downloadHandler.text);
                Debug.Log($"Data count: {response.data.Count}");
                CreateButtons(response.data);
            }
        }
    }

    private void CreateButtons(List<DataItem> dataItems)
    {
        if (buttonContainer == null)
        {
            Debug.LogError("Button container is not assigned.");
            return;
        }

        if (itemButtonPrefab == null)
        {
            Debug.LogError("Item button prefab is not assigned.");
            return;
        }

        Debug.Log($"Creating buttons for {dataItems.Count} items.");

        // Limpia los botones anteriores si existen
        foreach (Transform child in buttonContainer.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in dataItems)
        {
            Debug.Log($"Creating button for item: {item.maquina.nombre}");

            // Instanciar el prefab del botón
            ItemButtonManager itemButton = Instantiate(itemButtonPrefab, buttonContainer.transform);
            itemButton.ItemName = item.maquina.nombre;

            // Descargar y asignar la imagen
            StartCoroutine(DownloadImage(item.maquina.url_imagen, itemButton));

            // Descargar y mostrar el modelo 3D
            StartCoroutine(DownloadModel(item.maquina.url_objeto, itemButton));

            itemButton.name = item.maquina.nombre;

            Debug.Log($"Button created for item: {item.maquina.nombre}");
        }
    }

    private IEnumerator DownloadImage(string url, ItemButtonManager itemButton)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to download image: " + www.error);
            }
            else
            {
                Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                itemButton.ItemImage = sprite;
            }
        }
    }

    private IEnumerator DownloadModel(string url, ItemButtonManager itemButton)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to download model: " + www.error);
            }
            else
            {
                byte[] modelData = www.downloadHandler.data;
                string filePath = System.IO.Path.Combine(Application.persistentDataPath, "model.fbx");
                System.IO.File.WriteAllBytes(filePath, modelData);

                Debug.Log($"Model downloaded to: {filePath}");

                // Pasar la ruta del archivo al gestor de interacciones AR
                ARInteractionsManager.instance.SetModelPath(filePath);
            }
        }
    }

    private void ImportModel(string filePath)
    {
        GameObject model = new GameObject("ImportedModel");
        model.transform.position = modelSpawnPoint.position;

        // Aquí se requiere un proceso adicional de importación específico de Unity Editor para los archivos .fbx
        // Este proceso no se puede realizar en tiempo de ejecución en Unity sin un Asset Bundle o Addressable

        // Para una solución completa, deberías considerar usar Asset Bundles o Addressables para manejar modelos 3D en tiempo de ejecución
    }
}

[System.Serializable]
public class ApiResponse
{
    public List<DataItem> data;
    public int size;
}

[System.Serializable]
public class DataItem
{
    public int id;
    public int maquina_id;
    public int user_id;
    public string estado_registro;
    public Maquina maquina;
}

[System.Serializable]
public class Maquina
{
    public int id;
    public string nombre;
    public string url_objeto;
    public string url_imagen;
}
