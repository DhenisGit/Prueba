using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemButtonManager : MonoBehaviour
{
    private string itemName;
    private Sprite itemImage;
    private GameObject item3DModel;
    private ARInteractionsManager interactionsManager;

    public string ItemName
    {
        set
        {
            itemName = value;
        }
    }

    public Sprite ItemImage { set => itemImage = value; }

    public GameObject Item3DModel { set => item3DModel = value; }

    // Start is called before the first frame update
    /*void Start()
    {
        transform.GetChild(0).GetComponent<Text>().text = itemName;
        transform.GetChild(1).GetComponent<RawImage>().texture = itemImage.texture;

        var button = GetComponent<Button>();
        button.onClick.AddListener(GameManager.instance.ARPosition);
        button.onClick.AddListener(Create3DModel);
    }*/
    void Start()
    {
        var textComponent = transform.GetChild(0).GetComponent<Text>();
        if (textComponent != null)
        {
            textComponent.text = itemName;
        }
        else
        {
            Debug.LogError("Text component not found in GetChild(0).");
        }

        var rawImageComponent = transform.GetChild(1).GetComponent<RawImage>();
        if (rawImageComponent != null && itemImage != null)
        {
            rawImageComponent.texture = itemImage.texture;
        }
        else
        {
            Debug.LogError("RawImage or itemImage not assigned.");
        }

        var button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(GameManager.instance.ARPosition);
            button.onClick.AddListener(Create3DModel);
        }
        else
        {
            Debug.LogError("Button component not found.");
        }
        interactionsManager = FindObjectOfType<ARInteractionsManager>();
    }

    private void Create3DModel()
    {
        interactionsManager.Item3DModel = Instantiate(item3DModel);
    }

}
