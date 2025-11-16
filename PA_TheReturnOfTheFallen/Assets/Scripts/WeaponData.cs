using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("Stats")]
    public string weaponName = "New Weapon";
    public int attackDamage = 20;
    public float attackCooldown = 0.3f;
    public float attackRange = 1.5f;

    // Offset relativo ao player para o ponto de ataque
    public Vector3 attackPointLocalOffset = new Vector3(1f, 0f, 0f);

    [Header("Visual")]
    public Sprite weaponSprite;                          // sprite da arma
    public RuntimeAnimatorController weaponAnimator;     // animator controller da arma

    [Header("Animação da Arma")]
    public string weaponAttackTrigger = "WeaponAttack";  // trigger no Animator da arma
}
