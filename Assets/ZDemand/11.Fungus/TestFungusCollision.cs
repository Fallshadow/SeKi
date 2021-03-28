using System.Collections;
using System.Collections.Generic;
using Fungus;
using UnityEngine;
using Collision = Fungus.Collision;

public class TestFungusCollision : MonoBehaviour
{
    public Flowchart MyFlowchart;
    // Start is called before the first frame update
    void Start()
    {
        MyFlowchart.GetVariable("Var1") = GetComponent<Collision>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
