using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDetector : MonoBehaviour
{
    private List<GameObject> cols;

    public List<GameObject> insideCols => cols;

    private void Awake()
    {
        cols = new List<GameObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (cols.Contains(other.gameObject) == false)
            cols.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (cols.Contains(other.gameObject) == true)
            cols.Remove(other.gameObject);
    }
}
