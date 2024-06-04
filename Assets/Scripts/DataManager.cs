using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DataManager : MonoBehaviour
{
    [SerializeField] private ARObjLoader arObjLoader;
    [SerializeField] private GameObject buttonContainer; // Contenedor donde se agregarán los botones
    [SerializeField] private ItemButtonManager itemButtonPrefab; // Prefab del botón

    public void LoadItemsFromAPI()
    {
        if (!string.IsNullOrEmpty(LoginController.authToken))
        {
            Debug.Log("Loading items from API.");
            StartCoroutine(arObjLoader.GetObjListFromAPI());
        }
        else
        {
            Debug.LogError("Token not found, cannot load items.");
        }
    }

    public void CreateButtons(List<DataItem> dataItems)
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

            ItemButtonManager itemButton = Instantiate(itemButtonPrefab, buttonContainer.transform);
            itemButton.ItemName = item.maquina.nombre;
            itemButton.ItemImage = null; // Aquí puedes asignar una imagen predeterminada si es necesario

            // Verifica que el modelo exista antes de asignarlo
            GameObject model = arObjLoader.transform.Find(item.maquina.nombre)?.gameObject;
            if (model != null)
            {
                itemButton.Item3DModel = model;
            }
            else
            {
                Debug.LogError($"Model for {item.maquina.nombre} not found.");
            }

            itemButton.name = item.maquina.nombre;

            // Configura el botón para mostrar el modelo 3D
            itemButton.GetComponent<Button>().onClick.AddListener(() => {
                if (itemButton.Item3DModel != null)
                {
                    itemButton.Item3DModel.SetActive(true);
                    // Añade lógica para mostrar el modelo en la cámara AR
                }
                else
                {
                    Debug.LogError($"Model for {item.maquina.nombre} is null.");
                }
            });

            // Puedes añadir más configuraciones aquí si es necesario
        }
    }
}
