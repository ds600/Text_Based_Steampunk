using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMethods : MonoBehaviour
{
    [SerializeField] AdventureGame adG;
    [SerializeField] Enemy normalPirate;
    
    void LoeschdeckeSchiff()
    {
        adG.canAnswer = false;
        int ergebnis = adG.result;
        if (ergebnis < 12)
        {
            adG.TakeLifeDamage(5);
            adG.LoadNextState(adG.nextState.stateOnFailure);
        }
        else
        {
            adG.LoadNextState(adG.nextState.stateOnSuccess);
        }
        adG.canAnswer = true;
    }

    void HarpuneHaendisch()
    {
        adG.canAnswer = false;
        int ergebnis = adG.result;
        if (ergebnis < adG.nextState.rollGoal)
        {
            adG.TakeLifeDamage(10);
            adG.LoadNextState(adG.nextState.stateOnFailure);
        }
        else
        {
            adG.TakeLifeDamage(5);
            adG.LoadNextState(adG.nextState.stateOnSuccess);
        }
        adG.canAnswer = true;
    }

    void InitiateTestCombat()
    {
        adG.canAnswer = false;
        adG.initiateFight(normalPirate);
    }

    void SetIntiatives()
    {
        adG.canAnswer = false;
        adG.setIntiative(adG.result, adG.enemyResult);
        adG.canAnswer = true;
    }

    void AttackPlayer()
    {
        adG.canAnswer = false;
        if (adG.enemyResult >= adG.GetAc())
        {
            // Doppelter Damage, bei Krits
            if (adG.enemyResult - adG.currentOpponent.GetDiceBonus() == 20)
            {
                adG.TakeLifeDamage(adG.currentOpponent.GetDamage());
            }

            adG.TakeLifeDamage(adG.currentOpponent.GetDamage());
        }
        adG.canAnswer = true; 
    }

    void AttackEnemy()
    {
        adG.canAnswer = false;

        if( adG.result >= adG.currentOpponent.GetAc())
        {
            // Doppelter Damage, bei Krits
            if (adG.result - adG.diceBonus == 20)
            {
                adG.currentOpponent.changeHp(-adG.nextState.attackDamage);
            }

            adG.currentOpponent.changeHp(-adG.nextState.attackDamage);
            adG.SetHpValueText(adG.currentOpponent.GetHp().ToString());
        }

        if (adG.currentOpponent.GetHp() > 0)
        {
            adG.GetEnemyDiceRoll("AttackPlayer");
        }
        else
        {
            adG.LoadNextState(adG.nextState.loadStateOnKill);
            adG.currentOpponent.ResetHP();
        }

        adG.canAnswer = true;
    }

    void FreeAttackOnEnemy()
    {
        adG.canAnswer = false;

        if (adG.result >= adG.currentOpponent.GetAc())
        {
            // Doppelter Damage, bei Krits
            if (adG.result - adG.diceBonus == 20)
            {
                adG.currentOpponent.changeHp(-adG.nextState.attackDamage);
            }

            adG.currentOpponent.changeHp(-adG.nextState.attackDamage);
            adG.SetHpValueText(adG.currentOpponent.GetHp().ToString());
        }

        if (adG.currentOpponent.GetHp() <= 0)
        {
            adG.LoadNextState(adG.nextState.loadStateOnKill);
            adG.currentOpponent.ResetHP();

            adG.canAnswer = true;

            return;
        } 

        if (adG.nextState.isFreeAttack == true)
        {
            adG.LoadNextState(adG.nextState.loadStateAfterFreeStrike);
        }

        adG.canAnswer = true;
    }

    void loadSceneAfterRoll()
    {
        adG.canAnswer = false;

        if (adG.result >= adG.nextState.rollGoal)
        {
            adG.LoadNextState(adG.nextState.stateOnSuccess);
        }
        else
        {
            adG.LoadNextState(adG.nextState.stateOnFailure);
        }

        adG.canAnswer = true;
    }

}
