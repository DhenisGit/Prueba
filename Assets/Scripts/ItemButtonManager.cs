using UnityEngine;
using UnityEngine.UI;

public class ItemButtonManager : MonoBehaviour
{
    [SerializeField] private Text itemNameText;
    [SerializeField] private Image itemImage;

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
}
