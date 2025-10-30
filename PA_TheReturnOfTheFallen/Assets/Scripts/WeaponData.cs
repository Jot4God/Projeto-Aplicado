using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/WeaponData")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public WeaponType type;
    public int damage;
    public float attackSpeed;
    public GameObject weaponPrefab;
    public Sprite icon; // para UI
}

public enum WeaponType { Melee, Ranged, Magic }
