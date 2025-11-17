using UnityEngine;

[CreateAssetMenu(fileName = "New Shop Item", menuName = "Shop/Item")]
public class ShopItem : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int price;
    [TextArea]
    public string description;

    // Estatísticas opcionais que o item altera
    public int addedArmor = 0;
    public int addedHealth = 0;
    public float addedSpeed = 0f;

    // Não serializado, usado apenas em runtime
    [HideInInspector]
    public bool isSold = false;
}
