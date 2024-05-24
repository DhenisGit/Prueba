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

    IEnumerator LoginCoroutine()
    {
        WWWForm form = new WWWForm();
        form.AddField("email", loginEmail.text);
        form.AddField("password", loginPassword.text);

        using (UnityWebRequest www = UnityWebRequest.Post("https://back.igvperu.com/public/api/login", form))
        {
            yield return www.SendWebRequest();

            //if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            //{
            //    Debug.LogError("Network Error: " + www.error);
            //    ShowErrorMessage("Error de conexión o de protocolo: " + www.error);
            //}
            //else
            //{
                Debug.LogError("Code Response: " + www.responseCode);
                if (www.responseCode == 200 || www.responseCode == 201)
                {
                    ApiResponse response = JsonUtility.FromJson<ApiResponse>(www.downloadHandler.text);
                    if (!string.IsNullOrEmpty(response.error))
                    {
                        Debug.LogError("Server Error: " + response.error);
                        ShowErrorMessage(response.error);
                    }
                    else
                    {
                        Debug.Log("Token: " + response.token);
                        OpenHomePanel();
                    }
                }
                else if (www.responseCode == 400)
                {
                    Debug.LogError("Bad Request: " + www.downloadHandler.text);
                    ShowErrorMessage("Solicitud incorrecta. Por favor, revisa tus datos.");
                }
                else if (www.responseCode == 401)
                {
                    Debug.LogError("Unauthorized: " + www.downloadHandler.text);
                    ShowErrorMessage("Credenciales incorrectas");
                }
                else if (www.responseCode == 404)
                {
                    Debug.LogError("Not Found: " + www.downloadHandler.text);
                    ShowErrorMessage("Servicio no encontrado.");
                }
                else
                {
                    Debug.LogError("Unexpected Error (" + www.responseCode + "): " + www.downloadHandler.text);
                    ShowErrorMessage("Error inesperado. Código: " + www.responseCode);
                }
            //}
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

    IEnumerator ShowErrorMessageCoroutine(string message, float delay)
    {
        errorMessage.text = message;
        errorPanel.SetActive(true);
        yield return new WaitForSeconds(delay); // Espera 3 segundos
        errorPanel.SetActive(false); // Oculta el panel de error
    }

    [System.Serializable]
    public class ApiResponse
    {
        public string token;
        public string error;
    }
}
