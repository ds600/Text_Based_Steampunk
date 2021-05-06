using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Animations : MonoBehaviour
{
    [SerializeField] GameObject playerSword, enemySword;
    [SerializeField] int torque, velocityLeft, velocityUp, gravity;
    Vector3 startRotationEnemySword, startRotationPlayerSword;
    Vector3 startPositionPlayerSword, startPositionEnemySword;
    Transform transformPlayerSword;
    Transform transformEnemySword;
    bool crossCheck;

    private void Start()
    {
        startRotationPlayerSword = new Vector3(0, 0, 40);
        startRotationEnemySword = new Vector3(0, 0, 140);
        
        startPositionPlayerSword = new Vector3(-450, 75, 0);
        startPositionEnemySword = new Vector3(300, 62, 0);

        transformPlayerSword = playerSword.transform;
        transformEnemySword = enemySword.transform;

        CrossSwords();
    }

    public void SpinPlayerSword()
    {
        Rigidbody2D rb2D = playerSword.GetComponent<Rigidbody2D>();
        Image sword = playerSword.GetComponent<Image>();

        rb2D.gravityScale = gravity;
        rb2D.AddForce((Vector2.left * velocityLeft + Vector2.up * velocityUp));
        rb2D.AddTorque(torque);

        Invoke("DisableSwordImages", 1.5f);
    }

    public void SpinEnemySword()
    {
        Rigidbody2D rb2D = enemySword.GetComponent<Rigidbody2D>();
        Image sword = enemySword.GetComponent<Image>();

        rb2D.gravityScale = gravity;
        rb2D.AddForce((Vector2.left * (velocityLeft * -1) + Vector2.up * velocityUp));
        rb2D.AddTorque(torque * -1);

        Invoke("DisableSwordImages", 1.5f);
    }

    public void CrossSwords()
    {
        Rigidbody2D playerRb2d = playerSword.GetComponent<Rigidbody2D>();
        Rigidbody2D enemyRb2d = enemySword.GetComponent<Rigidbody2D>();

        playerRb2d.gravityScale = 0;
        enemyRb2d.gravityScale = 0;
        playerRb2d.velocity = Vector3.zero;
        enemyRb2d.velocity = Vector3.zero;    
        playerRb2d.angularVelocity = 0;
        enemyRb2d.angularVelocity = 0;

        playerSword.GetComponent<Image>().enabled = true;
        enemySword.GetComponent<Image>().enabled = true;

        transformPlayerSword.localPosition = startPositionPlayerSword;
        transformEnemySword.localPosition = startPositionEnemySword;

        transformPlayerSword.eulerAngles = startRotationPlayerSword;
        transformEnemySword.eulerAngles = startRotationEnemySword;

        crossCheck = true;
        
    }

    void DisableSwordImages()
    {
        playerSword.GetComponent<Image>().enabled = false;
        enemySword.GetComponent<Image>().enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        if ((transformPlayerSword.position - transformEnemySword.position).magnitude > 60 && crossCheck == true)
        {
            transformPlayerSword.localPosition = new Vector3(transformPlayerSword.localPosition.x + 400 * Time.deltaTime, transformPlayerSword.localPosition.y, 0);
            transformEnemySword.localPosition = new Vector3(transformEnemySword.localPosition.x - 400 * Time.deltaTime, transformEnemySword.localPosition.y, 0);

            if((transformPlayerSword.position - transformEnemySword.position).magnitude < 60) {
                crossCheck = false;
            }
        }
    }
}
