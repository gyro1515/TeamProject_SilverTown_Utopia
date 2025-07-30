using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCam : MonoBehaviour
{
    public GameObject target;
    
    // Update is called once per frame
    void Update()
    {
        Vector3 tarPos = target.transform.position;
        tarPos.z = -10f;
        transform.position = tarPos;
    }
}
