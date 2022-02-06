using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject target;
        void Start()
    {
        GetComponent<LineRenderer>().SetPosition(0, this.transform.position);
        GetComponent<LineRenderer>().SetPosition(1, target.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<LineRenderer>().SetPosition(0, this.transform.position);
        GetComponent<LineRenderer>().SetPosition(1, target.transform.position);
    }
}
