using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "State")]
public class State : ScriptableObject
{
    public string storyText;
    [SerializeField] public State[] nextStates;
    public bool requiresDiceRoll;
    public string requiredAction;
    public int rollGoal;
    [SerializeField] public State stateOnSuccess;
    [SerializeField] public State stateOnFailure;

    public bool isAttack;
    public bool isFreeAttack;
    public bool hasAdvantage;
    public bool hasDisadvantage;
    public int diceBonus;
    public int attackDamage;
    public int attackCharges;
    public bool hasCharges;
    public bool statBonusDex;
    public bool statBonusStr;
    public bool statBonusInt;
    public bool statBonusLuck;
    [SerializeField] public State loadStateOnKill;
    [SerializeField] public State loadStateAfterFreeStrike;


    public string GetText()
    {
        return storyText;
    }

    public bool GetRequiresDiceRoll()
    {
        return requiresDiceRoll;
    }

    public string GetRequiredAction()
    {
        return requiredAction;
    }

    public State GetNextState (int answer)
    {
        if ( nextStates.Length >= answer)
        {
            answer--;
            return nextStates[answer];
        }
        return this;
    }

}

[CustomEditor(typeof(State))]
public class MyScriptEditor : Editor
{
    override public void OnInspectorGUI()
    {
        var myScript = target as State;

        myScript.isAttack = GUILayout.Toggle(myScript.isAttack, "isAttack");

        SerializedObject serializedObject = new UnityEditor.SerializedObject(myScript);
        SerializedProperty serializedPropertyNextStates = serializedObject.FindProperty("nextStates");

        // Anstatt horizontales Scrolling, geht der Text in der Zeil drunter weiter. (Bei Textfields und Textareas)
        EditorStyles.textArea.wordWrap = true;
        EditorStyles.textField.wordWrap = true;

        // Es wird ein Feld erstellt, der Wert der dort eingetragen wird, wird dann für die eigentliche State übernommen
        myScript.diceBonus = EditorGUILayout.IntField("Dice bonus (Not Stats):", myScript.diceBonus);
        myScript.statBonusDex = EditorGUILayout.Toggle("Dex is a bonus:", myScript.statBonusDex);
        myScript.statBonusStr = EditorGUILayout.Toggle("Str is a bonus:", myScript.statBonusStr);
        myScript.statBonusInt = EditorGUILayout.Toggle("Int is a bonus:", myScript.statBonusInt);
        myScript.statBonusLuck = EditorGUILayout.Toggle("Luck is a bonus:", myScript.statBonusLuck);
        myScript.hasAdvantage = EditorGUILayout.Toggle("Advantage on Roll:", myScript.hasAdvantage);
        myScript.hasDisadvantage = EditorGUILayout.Toggle("Disadvantage on Roll:", myScript.hasDisadvantage);

        // Unterscheiden zwischen normalen States und Angriffsoptionen
        if (myScript.isAttack)
        {
            myScript.attackDamage = EditorGUILayout.IntField("Attack Damage:", myScript.attackDamage);

            myScript.hasCharges = EditorGUILayout.Toggle("Limited Attack Charges:", myScript.hasCharges);
            if (myScript.hasCharges == true)
            {
                myScript.attackCharges = EditorGUILayout.IntField("Attack Charges:", myScript.attackCharges);
            }
            // Properties liefern nur Bools zurück, also wird dieser mit dem "ApplyModifiedProperties" übernommen
            EditorGUILayout.PropertyField(serializedObject.FindProperty("loadStateOnKill"));

            myScript.isFreeAttack = EditorGUILayout.Toggle("Attack for Free:", myScript.isFreeAttack);

            if(myScript.isFreeAttack)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("loadStateAfterFreeStrike"));
            }

        } else
        {
            EditorGUILayout.LabelField("Storytext");
            myScript.storyText = EditorGUILayout.TextArea(myScript.storyText, GUILayout.Height(220));
            EditorGUILayout.PropertyField(serializedPropertyNextStates);
            myScript.requiresDiceRoll = EditorGUILayout.Toggle("Requires diceroll:", myScript.requiresDiceRoll);
            EditorGUILayout.LabelField("Required Action");
            myScript.requiredAction = EditorGUILayout.TextField(myScript.requiredAction);

            if (myScript.requiresDiceRoll)
            {
                myScript.rollGoal = EditorGUILayout.IntField("Diceroll Goal:", myScript.rollGoal);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("stateOnSuccess"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("stateOnFailure"));
            }
        }
        // Übernahme der Property Änderungen
        serializedObject.ApplyModifiedProperties();

    }
}
