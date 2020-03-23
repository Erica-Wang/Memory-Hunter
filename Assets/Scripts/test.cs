using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class test : MonoBehaviour
{
    public GameObject orb;

    // Start is called before the first frame update
    void Start()
    {           
        Instantiate(orb,new Vector3(0,0,0),Quaternion.identity);
    }

}
