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

    public static string authToken; // Variable estática para almacenar el token

    void Start()
    {
        OpenLoginPanel();
    }

    public void OpenLoginPanel()
    {
        loginPanel.SetActive(true);
        homePanel.SetActive(false);
        errorPanel.SetActive(false);
    }

    public void OpenHomePanel()
    {
        loginPanel.SetActive(false);
        homePanel.SetActive(true);
        errorPanel.SetActive(false);
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
        form.AddField("codigo", loginEmail.text);
        form.AddField("password", loginPassword.text);

        using (UnityWebRequest www = UnityWebRequest.Post("https://back.igvperu.com/public/api/login/1", form))
        {
            yield return www.SendWebRequest();

            Debug.Log("Code Response: " + www.responseCode);
            if (www.responseCode == 200)
            {
                AuthResponse response = JsonUtility.FromJson<AuthResponse>(www.downloadHandler.text);
                if (!string.IsNullOrEmpty(response.error))
                {
                    Debug.LogError("Server Error: " + response.error);
                    ShowErrorMessage(response.error);
                }
                else
                {
                    authToken = response.token; // Almacenar el token
                    Debug.Log("Token: " + response.token);
                    OpenHomePanel();
                    FindObjectOfType<DataManager>().LoadItemsFromAPI();
                }
            }
            else if (www.responseCode == 400)
            {
                Debug.LogError("Bad Request: " + www.downloadHandler.text);
                ShowErrorMessage("Solicitud incorrecta. Por favor, revisa tus datos.");
            }
            else if (www.responseCode == 401)
            {
                ShowErrorMessage("Credenciales incorrectas");
            }
            else if (www.responseCode == 404)
            {
                ShowErrorMessage("No se encontró un usuario con el código ingresado");
            }
            else
            {
                Debug.LogError("Unexpected Error (" + www.responseCode + "): " + www.downloadHandler.text);
                ShowErrorMessage("Error inesperado. Código: " + www.responseCode);
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

    public void SetLoginCredentials()
    {
        loginEmail.text = "DaKMfVSYhG";
        loginPassword.text = "35855067";
    }
}