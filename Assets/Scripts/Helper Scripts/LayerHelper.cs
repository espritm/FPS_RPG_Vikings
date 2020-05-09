using System.Collections.Generic;
using UnityEngine;


public class LayerHelper : MonoBehaviour
{
    public static List<GameObject> FindGameObjectInLayer(string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);

        List<Object> lsObj = new List<Object>(FindObjectsOfType(typeof(GameObject)));

        List<GameObject> lsRes = new List<GameObject>();

        foreach(Object o in lsObj)
        {
            GameObject gameObject = o as GameObject;

            if (gameObject.layer == layer)
                lsRes.Add(gameObject);
        }   

        return lsRes;
    }
}

