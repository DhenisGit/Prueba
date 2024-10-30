using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoginController : MonoBehaviour
{
    public GameObject loginPanel, homePanel, errorPanel;
    public InputField loginEmail, loginPassword, passwordField;
    public Text errorMessage;
    public Button buttonHidePass, buttonShowPass;
    //public GameObject aRCamera; // Referencia al GameObject de la camara AR

    public static string authToken; // Variable estatica para almacenar el token

    void Start()
    {
        Debug.Log("Inicio de la aplicacion.");

        // Revisar si ya hay una sesion iniciada
        bool isLoggedIn = PlayerPrefs.GetInt("isLoggedIn", 0) == 1;
        //authToken = PlayerPrefs.GetString("authToken", null);
        Debug.Log("Estado de sesion iniciada: " + isLoggedIn);
        //Debug.Log("Token recuperado: " + authToken);

        //if (isLoggedIn && !string.IsNullOrEmpty(authToken))
        if (isLoggedIn)
        {
            OpenHomePanel();
        }
        else
        {
            OpenLoginPanel();
        }
    }

    public void OpenLoginPanel()
    {
        Debug.Log("Abriendo panel de login.");
        loginPanel.SetActive(true);
        homePanel.SetActive(false);
        errorPanel.SetActive(false);
        //aRCamera.SetActive(false); // Desactivar la camara AR
    }

    public void OpenHomePanel()
    {
        Debug.Log("Abriendo panel de inicio.");
        loginPanel.SetActive(false);
        homePanel.SetActive(true);
        errorPanel.SetActive(false);
        //aRCamera.SetActive(true); // Activar la camara AR
    }

    public void LoginUser()
    {
        if (string.IsNullOrEmpty(loginEmail.text))
        {
            ShowErrorMessage("Por favor, completa el campo de usuario");
            return;
        }
        if (string.IsNullOrEmpty(loginPassword.text))
        {
            ShowErrorMessage("Por favor, completa el campo de contraseña");
            return;
        }
        StartCoroutine(LoginCoroutine());
    }

    private IEnumerator LoginCoroutine()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", loginEmail.text);
        form.AddField("password", loginPassword.text);

        using (UnityWebRequest www = UnityWebRequest.Post("https://back.igvperu.com/public/api/login/1", form))
        {
            yield return www.SendWebRequest();

            Debug.Log("Respuesta de login recibida con codigo: " + www.responseCode);

            if (www.responseCode == 200)
            {
                AuthResponse response = JsonUtility.FromJson<AuthResponse>(www.downloadHandler.text);
                if (!string.IsNullOrEmpty(response.error))
                {
                    Debug.LogError("Error del servidor: " + response.error);
                    ShowErrorMessage(response.error);
                }
                else
                {
                    authToken = response.token; // Almacenar el token
                    //PlayerPrefs.SetString("authToken", authToken); // Guardar el token de manera persistente
                    //PlayerPrefs.SetInt("isLoggedIn", 1); // Guardar que la sesion esta iniciada
                    PlayerPrefs.Save(); // Asegurarse de que los datos se guarden
                    Debug.Log("Token guardado: " + response.token);
                    OpenHomePanel();
                }
            }
            else if (www.responseCode == 400)
            {
                Debug.LogError("Solicitud incorrecta: " + www.downloadHandler.text);
                ShowErrorMessage("Solicitud incorrecta. Por favor, revisa tus datos.");
            }
            else if (www.responseCode == 401)
            {
                ShowErrorMessage("Credenciales incorrectas");
            }
            else if (www.responseCode == 404)
            {
                ShowErrorMessage("No se encontro un usuario con el codigo ingresado");
            }
            else
            {
                Debug.LogError("Error inesperado (" + www.responseCode + "): " + www.downloadHandler.text);
                ShowErrorMessage("Error inesperado. Codigo: " + www.responseCode);
            }
        }
    }

    public void ShowPassword()
    {
        passwordField.contentType = InputField.ContentType.Standard;
        passwordField.ForceLabelUpdate();
        buttonShowPass.gameObject.SetActive(false);
        buttonHidePass.gameObject.SetActive(true);
    }

    public void HidePassword()
    {
        passwordField.contentType = InputField.ContentType.Password;
        passwordField.ForceLabelUpdate();
        buttonShowPass.gameObject.SetActive(true);
        buttonHidePass.gameObject.SetActive(false);
    }

    public void ShowErrorMessage(string message)
    {
        StartCoroutine(ShowErrorMessageCoroutine(message, 3.0f)); // Inicia la coroutine con un retraso de 3 segundos
    }

    private IEnumerator ShowErrorMessageCoroutine(string message, float delay)
    {
        errorMessage.text = message;
        errorPanel.SetActive(true);
        yield return new WaitForSeconds(delay); // Espera 3 segundos
        errorPanel.SetActive(false); // Oculta el panel de error
    }
}

[System.Serializable]
public class AuthResponse
{
    public string token;
    public string error;
}
