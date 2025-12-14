using UnityEngine;

public class HotbarController : MonoBehaviour
{
    public HotbarSlot[] slots;
    public PlayerHP playerHP;
    public PlayerMana playerMana;

    [HideInInspector] public int selectedIndex = 0;

    void Start()
    {
        UpdateSelection();
    }

    void Update()
    {
        // Scroll do rato
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f) SelectNext();
        else if (scroll < 0f) SelectPrevious();

        // Teclas 1 a 5
        if (Input.GetKeyDown(KeyCode.Alpha1)) UseSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) UseSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) UseSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) UseSlot(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) UseSlot(4);
    }

    public void UseSlot(int index)
    {
        if (index < slots.Length)
        {
            selectedIndex = index;
            UpdateSelection();
            slots[selectedIndex].UseItem(playerHP, playerMana);
        }
    }

    public void SelectNext()
    {
        selectedIndex++;
        if (selectedIndex >= slots.Length) selectedIndex = 0;
        UpdateSelection();
    }

    public void SelectPrevious()
    {
        selectedIndex--;
        if (selectedIndex < 0) selectedIndex = slots.Length - 1;
        UpdateSelection();
    }

    void UpdateSelection()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].SetSelected(i == selectedIndex);
        }
    }

    public void AddItemToHotbar(ShopItem item)
    {
        // Se já existe na hotbar, só aumenta quantidade
        foreach (HotbarSlot slot in slots)
        {
            if (slot.currentItem == item)
            {
                slot.SetItem(item, 1);
                return;
            }
        }

        // Coloca no primeiro slot vazio
        foreach (HotbarSlot slot in slots)
        {
            if (slot.currentItem == null)
            {
                slot.SetItem(item, 1);
                return;
            }
        }

        Debug.Log("Hotbar cheia!");
    }
}
