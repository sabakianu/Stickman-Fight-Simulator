using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSpawner : MonoBehaviour
{
    public GameObject PopUp;
    private string text;
    void Start()
    {
        text = "";
    }
    public void AddValues(string group, float number)
    {
        text += $"{group}: {Mathf.RoundToInt(number)}\n";
    }
    public void SpawnPopUp()
    {
        if (PopUp == null)
            return;

        // copilul spawnerului
        GameObject instance = Instantiate(PopUp, transform);
        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.identity;
        instance.transform.localScale = Vector3.one;

        DamagePopUp script = instance.GetComponent<DamagePopUp>();
        if (script != null)
        {
            script.putNumber(text);
            text = "";
        }
    }
}
