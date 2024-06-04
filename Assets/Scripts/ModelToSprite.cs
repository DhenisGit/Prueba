using UnityEngine;
using UnityEngine.UI;

public class ModelToSprite : MonoBehaviour
{
    public Camera renderCamera;
    public RenderTexture renderTexture;
    public Image uiImage;

    void Start()
    {
        // Configura la cámara para renderizar a la RenderTexture
        renderCamera.targetTexture = renderTexture;
    }

    public void RenderModelToSprite(GameObject model)
    {
        // Asegúrate de que el modelo esté en una posición visible para la cámara
        model.transform.position = renderCamera.transform.position + renderCamera.transform.forward * 2.0f;

        // Renderiza el modelo a la RenderTexture
        renderCamera.Render();

        // Crear una nueva textura2D a partir de la RenderTexture
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();
        RenderTexture.active = null;

        // Convertir la textura2D a un sprite
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        // Asignar el sprite a la imagen de la UI
        uiImage.sprite = sprite;
    }
}
