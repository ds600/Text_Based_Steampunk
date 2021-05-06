using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AdventureGame : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI storyText, lifeText, shipLifeText;
    [SerializeField] TextMeshProUGUI hpValueText, acValueText, nameValueText, bonusValueText, initiativeValueText, myInitiativeValueText;
    [SerializeField] State StartingState;
    [SerializeField] Image ownImage;
    [SerializeField] Sprite[] diceStates, enemyDiceStates;
    [SerializeField] Image enemyDice;
    [SerializeField] AudioClip diceRollSound;
    [SerializeField] int dexterity, strengh, intelligence, ac, luck;

    [SerializeField] StateMethods stateMethodContainer;
    [SerializeField] Animations fightAnimations;

    public State nextState;
    public Enemy currentOpponent;

    public bool canAnswer = true;

    int life = 100, shipLife = 100, intiative;

    public int result, enemyResult, diceBonus;

    State currentState;
    State oldState;

    // Start is called before the first frame update
    void Start()
    {
        currentState = StartingState;
        storyText.text = currentState.GetText();
    }

    // Update is called once per frame
    void Update()
    {
        if (canAnswer == true)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                StateAfterButtonPress(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                StateAfterButtonPress(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                StateAfterButtonPress(3);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                StateAfterButtonPress(4);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                StateAfterButtonPress(5);
            }
        }
    }

    /// <summary>
    /// L�dt den State, der sich auf dem mitgegebenem Index befindet
    /// </summary>
    /// <param name="answer"></param>
    private void GetNextState(int answer)
    {
        oldState = currentState;
        currentState = currentState.GetNextState(answer);
        if (currentState != oldState)
        {
            storyText.text = currentState.GetText();
        }
    }

    /// <summary>
    /// L�dt den mitgegebene State
    /// </summary>
    /// <param name="nextState"></param>
    public void LoadNextState(State nextState)
    {
        oldState = currentState;
        currentState = nextState;
        if (currentState != oldState)
        {
            storyText.text = currentState.GetText();
        }
    }

    /// <summary>
    /// Die Spieler HP werden um den in "damage" mitgegebenen Wert gesenkt
    /// </summary>
    /// <param name="damage"></param>
    public void TakeLifeDamage(int damage)
    {
        life -= damage;
        if (life < 0)
        {
            life = 0;
        }

        lifeText.text = "Leben: " + life;
    }

    /// <summary>
    /// Die Schiff HP werden um den in "damage" mitgegebenen Wert gesenkt
    /// </summary>
    /// <param name="damage"></param>
    public void TakeShipLifeDamage(int damage)
    {
        shipLife -= damage;
        if (shipLife < 0)
        {
            shipLife = 0;
        }

        shipLifeText.text = "Schiff R�stung: " + shipLife;
    }

    /// <summary>
    /// Es wird f�r den Spieler gew�rfelt. Erst wird auf Advantage und Disadvantage beim wurf gepr�ft. Dann wird so oft gew�rfelt wie n�tig und das
    /// W�rfelergebnis wird zusammengerechnet: Index + 1 + W�rfelbonus
    /// </summary>
    /// <param name="callback"> Die Methode mit diesem Namen wird ausgef�hrt, wenn sie sich in "StateMethods" befindet</param>
    /// <returns></returns>
    private IEnumerator RollDice(string callback)
    {
        canAnswer = false;
        int randomNumber = UnityEngine.Random.Range(0, 20);
        ownImage.enabled = true;
        
        int anzahlWuerfe = 0;

        if (nextState.hasAdvantage == true || nextState.hasDisadvantage == true)
        {
            anzahlWuerfe = 2;
        }
        else
        {
            anzahlWuerfe = 1;
        }

        result = -100;
        for (int wuerfe = 0; wuerfe < anzahlWuerfe; wuerfe++)
        {
            AudioSource.PlayClipAtPoint(diceRollSound, Camera.main.transform.position);
            for (int i = 0; i < 14; i++)
            {
                randomNumber = UnityEngine.Random.Range(0, 20);
                ownImage.sprite = diceStates[randomNumber];
                yield return new WaitForSeconds(0.1f);
            }

            if (result == -100)
            {
                result = randomNumber + 1 + diceBonus;
            }
            else if (nextState.hasAdvantage && enemyResult < randomNumber + 1 + diceBonus)
            {
                result = randomNumber + 1 + diceBonus;
            }
            else if (nextState.hasDisadvantage && enemyResult > randomNumber + 1 + diceBonus)
            {
                result = randomNumber + 1 + diceBonus;
            }

            if (anzahlWuerfe == 2)
            {
                yield return new WaitForSeconds(0.5f);
            }

        }

        canAnswer = true;
        if (callback != "")
        {
            stateMethodContainer.Invoke(callback, 0);
        }
    }

    /// <summary>
    /// Es wird f�r den Gegner gew�rfelt. Erst wird auf Advantage und Disadvantage beim wurf gepr�ft. Dann wird so oft gew�rfelt wie n�tig und das
    /// W�rfelergebnis wird zusammengerechnet: Index + 1 + W�rfelbonus
    /// </summary>
    /// <param name="callback"></param>
    /// <returns></returns>
    private IEnumerator RollEnemyDice(string callback)
    {

        canAnswer = false;
        int randomNumber = UnityEngine.Random.Range(0, 20);
        ownImage.enabled = true;

        int anzahlWuerfe = 0;

        if ( currentOpponent.GetAdvantage() == true || currentOpponent.GetDisadvantage() == true)
        {
            anzahlWuerfe = 2;
        } else
        {
            anzahlWuerfe = 1;
        }

        enemyResult = -100;
        for(int wuerfe = 0; wuerfe < anzahlWuerfe; wuerfe++)
        {
            AudioSource.PlayClipAtPoint(diceRollSound, Camera.main.transform.position);

            for (int i = 0; i < 14; i++)
            {
                randomNumber = UnityEngine.Random.Range(0, 20);
                enemyDice.sprite = enemyDiceStates[randomNumber];
                yield return new WaitForSeconds(0.1f);
            }

            if(enemyResult == -100)
            {
                enemyResult = randomNumber + 1 + currentOpponent.GetDiceBonus();
            }
            else if (currentOpponent.GetAdvantage() && enemyResult < randomNumber + 1 + currentOpponent.GetDiceBonus())
            {
                enemyResult = randomNumber + 1 + currentOpponent.GetDiceBonus();
            }
            else if (currentOpponent.GetDisadvantage() && enemyResult > randomNumber + 1 + currentOpponent.GetDiceBonus())
            {
                enemyResult = randomNumber + 1 + currentOpponent.GetDiceBonus();
            }

            if (anzahlWuerfe == 2)
            {
                yield return new WaitForSeconds(0.5f);
            }

        }

        canAnswer = true;
        if (callback != "")
        {
            stateMethodContainer.Invoke(callback, 0);
        }
    }

    /// <summary>
    /// Es wird f�r den Spieler gew�rfelt, danach wird die angegebene Methode in "StateMethods" ausgef�hrt
    /// </summary>
    /// <param name="executeAfterDiceRoll"></param>
    public void GetDiceRoll(string executeAfterDiceRoll)
    {
        StartCoroutine(RollDice(executeAfterDiceRoll));
    }

    /// <summary>
    /// Es wird f�r den Gegner gew�rfelt, danach wird die angegebene Methode in "StateMethods" ausgef�hrt
    /// </summary>
    /// <param name="executeAfterDiceRoll"></param>
    public void GetEnemyDiceRoll(string executeAfterDiceRoll)
    {
        StartCoroutine(RollEnemyDice(executeAfterDiceRoll));
    }

    /// <summary>
    /// Berechnet den W�rfelbonus. Wenn es eine Attacke ist, dann wird sie ausgef�hrt und beim Tot des Gegners eine State geladen.
    /// Wenn es eine "Free Attack" ist, dann wird der Spieler danach nicht angegriffen, sondern eine State geladen
    /// Wenn es eine normale State ist, dann wird direkt eine State geladen, wenn w�rfeln nicht aktiviert ist.
    /// Wenn W�rfeln aktiviert ist, dann wird eine Methode aus "StateMethods" ausgef�hrt, wenn keine angegeben ist, dann wird die Methode ausgef�hrt die je nach
    /// Wurf bei "StateOnSucess" oder eben bei "StateOnFailure" hinterlegt ist
    /// </summary>
    /// <param name="stateNumber"></param>
    void StateAfterButtonPress(int stateNumber)
    {
        nextState = currentState.GetNextState(stateNumber);
        diceBonus = 0;

        if (nextState.statBonusDex == true)
        {
            diceBonus += dexterity;
        }

        if (nextState.statBonusInt == true)
        {
            diceBonus += intelligence;
        }

        if (nextState.statBonusLuck == true)
        {
            diceBonus += luck;
        }

        if (nextState.statBonusStr == true)
        {
            diceBonus += strengh;
        }

        diceBonus += nextState.diceBonus;

        if(nextState.isAttack && nextState.hasCharges && nextState.attackCharges == 0)
        {
            return;
        }
        else if(nextState.isAttack && nextState.hasCharges)
        {
            nextState.attackCharges -= 1;
        }

        if (nextState.isAttack == true && nextState.isFreeAttack)
        {
            GetDiceRoll("FreeAttackOnEnemy");
            return;
        }
        else if(nextState.isAttack == true)
        {
            GetDiceRoll("AttackEnemy");
            return;
        }

        // Wenn f�r die N�chste Seite gerollt werden muss
        if (nextState.GetRequiresDiceRoll() == true)
        {
            // Wenn keine Action angegeben wird, dann soll einfach je nach Ergebnis ein State geladen werden
            if (nextState.GetRequiredAction() == "")
            {
                GetDiceRoll("loadSceneAfterRoll");
            }
            else
            {
                // W�rfel werfen und dann Methode ausf�hren
                GetDiceRoll(nextState.GetRequiredAction());
            }
        }
        // Wenn eine Methode f�r die n�chste Seite ausgef�hrt werden muss
        else if (nextState.GetRequiredAction() != "")
        {
            stateMethodContainer.Invoke(nextState.GetRequiredAction(), 0);
        }
        else
        {
            GetNextState(stateNumber);
        }
    }

    /// <summary>
    /// Initiative f�r Gegner und Spieler W�rfeln, danach wird "SetIntiatives" aufgerufen, welches wiederum die gleichnahmige Methode hier aufruft
    /// </summary>
    /// <param name="enemy"></param>
    public void initiateFight(Enemy enemy)
    {
        nameValueText.text = enemy.GetName();
        bonusValueText.text = enemy.GetDiceBonus().ToString();
        acValueText.text = enemy.GetAc().ToString();
        hpValueText.text = enemy.GetHp().ToString();

        currentOpponent = enemy;
        fightAnimations.CrossSwords();
        StartCoroutine(RollEnemyDice(""));
        StartCoroutine(RollDice("SetIntiatives"));
    }

    /// <summary>
    /// Setzt die Intiative f�r den Spieler und den Gegner, wenn der Gegner eine h�here Initiative hat, dann greift er auch direkt an.
    /// </summary>
    /// <param name="myIntiative"></param>
    /// <param name="enemyIntiative"></param>
    public void setIntiative(int myIntiative, int enemyIntiative )
    {
        intiative = myIntiative;
        myInitiativeValueText.text = intiative.ToString();

        currentOpponent.setInitiative(enemyIntiative);
        initiativeValueText.text = currentOpponent.GetIntiative().ToString();

        if(myIntiative < enemyIntiative)
        {
            fightAnimations.SpinPlayerSword();
            StartCoroutine(RollEnemyDice("AttackPlayer"));
        } 
        else
        {
            fightAnimations.SpinEnemySword();
        }
    }

    /// <summary>
    /// Liefert den Dexterity stat zur�ck
    /// </summary>
    /// <returns></returns>
    public int GetDex()
    {
        return dexterity;
    }

    /// <summary>
    /// Liefert den Armour Class stat zur�ck
    /// </summary>
    /// <returns></returns>
    public int GetAc()
    {
        return ac;
    }

    /// <summary>
    /// Liefert den St�rke stat zur�ck
    /// </summary>
    /// <returns></returns>
    public int GetStr()
    {
        return strengh;
    }

    /// <summary>
    /// Liefert den Intelligenz stat zur�ck
    /// </summary>
    /// <returns></returns>
    public int GetInt()
    {
        return intelligence;
    }
    
    /// <summary>
    /// Setzt den HP-Wert Text, des Gegners.
    /// </summary>
    /// <param name="hpValueText">HP des Gegners</param>
    public void SetHpValueText(string hpValueText)
    {
        this.hpValueText.text = hpValueText;
    }
}

