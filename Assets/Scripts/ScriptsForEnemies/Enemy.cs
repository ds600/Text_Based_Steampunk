using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


[CreateAssetMenu(menuName = "Enemy")]
public class Enemy : ScriptableObject
{
    [SerializeField] string enemyName;
    [SerializeField] int diceBonus;
    [SerializeField] int ac;
    [SerializeField] int hp, maxHp;
    [SerializeField] int damage;
    [SerializeField] int initiativeBonus;
    [SerializeField] bool hasAdvantage, hasDisadvantage;
    int initiative;

    public int GetDamage()
    {
        return damage;
    }

    public int GetHp()
    {
        return hp;
    }

    public int GetAc()
    {
        return ac;
    }

    public int GetDiceBonus()
    {
        return diceBonus;
    }

    public string GetName()
    {
        return enemyName;
    }

    public void ResetHP()
    {
        hp = maxHp;
    }

    public int changeHp(int hpChange)
    {
        hp += hpChange;
        if (hp <= 0)
        {
            hp = 0;
            return hp;
        }

        return hp;
    }

    public void setInitiative(int enemyIntiative)
    {
        initiative = enemyIntiative;
    }

    public int GetIntiative()
    {
        return initiative;
    }

    public bool GetAdvantage()
    {
        return hasAdvantage;
    }

    public bool GetDisadvantage()
    {
        return hasDisadvantage;
    }


}
