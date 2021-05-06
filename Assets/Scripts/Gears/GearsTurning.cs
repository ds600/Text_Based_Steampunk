using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearsTurning : MonoBehaviour
{
    float zRotation = 0;
    [SerializeField] int rotationChange;
    // Update is called once per frame
    void Update()
    {
        zRotation += rotationChange * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, 0, zRotation);
    }
}
