using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderRoll : MonoBehaviour
{

    void Update()
    {
        transform.Rotate(transform.up, 0.1f);
        ThirdPersonController.Instance.transform.Rotate(transform.up, 0.1f);
    }
}
