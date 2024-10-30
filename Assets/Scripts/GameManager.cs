using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public event Action OnMainMenu;
    public event Action OnItemsMenu;
    public event Action OnARPosition;

    public static GameManager instance;

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

    public void MainMenu()
    {
        OnMainMenu?.Invoke();
        Debug.Log("Main Menu Activated");
    }

    public void ItemsMenu()
    {
        OnItemsMenu?.Invoke();
        Debug.Log("Items Menu Activated");

        // Llamar a la API para cargar items
        //FindObjectOfType<DataManager>().LoadItemsFromAPI();
    }

    public void ARPosition()
    {
        OnARPosition?.Invoke();
        Debug.Log("AR Position Activated");
    }

    public void CloseApp()
    {
        // Eliminar la bandera de sesion iniciada y el token de autenticacion
        PlayerPrefs.DeleteKey("isLoggedIn");
        PlayerPrefs.DeleteKey("authToken");
        PlayerPrefs.Save();

        // Encontrar el LoginController y abrir el panel de login
        FindObjectOfType<LoginController>().OpenLoginPanel();

        Debug.Log("Sesion cerrada, token eliminado y redirigido al panel de login.");
    }
}
