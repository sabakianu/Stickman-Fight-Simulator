using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DamagePopUp : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMesh;
    public float destroyTime = 0.40f;

    void Start()
    {
        Destroy(gameObject, destroyTime);
    }

    public void putNumber(string text)
    {
        if (textMesh != null)
            textMesh.text = text;
    }
}
