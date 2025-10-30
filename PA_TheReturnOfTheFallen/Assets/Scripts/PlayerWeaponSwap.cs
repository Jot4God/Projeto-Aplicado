using UnityEngine;

public class PlayerWeaponSwap : MonoBehaviour
{
    public GameObject[] weapons; // array com os prefabs ou objetos das armas
    private int currentWeaponIndex = 0;

    void Start()
    {
        ActivateWeapon(currentWeaponIndex);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SwapWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            SwapWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            SwapWeapon(2);
    }

    void SwapWeapon(int index)
    {
        if(index < 0 || index >= weapons.Length) return;

        weapons[currentWeaponIndex].SetActive(false); // desativa arma atual
        currentWeaponIndex = index;
        weapons[currentWeaponIndex].SetActive(true); // ativa nova arma
    }

    void ActivateWeapon(int index)
    {
        for(int i = 0; i < weapons.Length; i++)
            weapons[i].SetActive(i == index);
    }
}
