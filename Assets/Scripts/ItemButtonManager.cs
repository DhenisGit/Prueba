using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemButtonManager : MonoBehaviour
{
    [SerializeField] private Text itemNameText;
    [SerializeField] private Image itemImage;
    [SerializeField] private GameObject item3DModel;
    [SerializeField] private ModelToSprite modelToSprite;

    private ARInteractionsManager interactionsManager;

    public string ItemName
    {
        set
        {
            if (itemNameText != null)
            {
                itemNameText.text = value;
            }
            else
            {
                Debug.LogError("itemNameText is not assigned.");
            }
        }
    }

    public Sprite ItemImage
    {
        set
        {
            if (itemImage != null)
            {
                itemImage.sprite = value;
            }
            else
            {
                Debug.LogError("itemImage is not assigned.");
            }
        }
    }

    public GameObject Item3DModel
    {
        get
        {
            return item3DModel;
        }
        set
        {
            item3DModel = value;
        }
    }

    void Start()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(() => GameManager.instance.ARPosition());
        button.onClick.AddListener(Create3DModel);
        interactionsManager = FindObjectOfType<ARInteractionsManager>();
    }

    private void Create3DModel()
    {
        if (item3DModel != null)
        {
            var instantiatedModel = Instantiate(item3DModel);
            interactionsManager.Item3DModel = instantiatedModel;
            modelToSprite.RenderModelToSprite(instantiatedModel);
        }
        else
        {
            Debug.LogError("Item 3D model is null.");
        }
    }
}
