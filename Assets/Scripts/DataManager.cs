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

    // Elimina la llamada en el Start
    // void Start()
    // {
    //     LoadItemsFromAPI();
    // }

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
                CreateButtons(response.data);
            }
        }
    }

    private void CreateButtons(List<DataItem> dataItems)
    {
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
}
