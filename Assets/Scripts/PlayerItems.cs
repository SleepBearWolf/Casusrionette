using UnityEngine;
using UnityEngine.UI;

public class PlayerItems : MonoBehaviour
{
    public Image[] itemSlots; 
    public Sprite[] startingItems; 

    private void Start()
    {
        
        for (int i = 0; i < itemSlots.Length && i < startingItems.Length; i++)
        {
            itemSlots[i].sprite = startingItems[i];
            itemSlots[i].gameObject.SetActive(true); 
        }
    }
}
