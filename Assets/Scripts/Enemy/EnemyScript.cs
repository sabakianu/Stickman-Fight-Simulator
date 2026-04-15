using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public int MaxHP = 200;
    public int HP = 200;

    //public HealthBarScript HealthBar;
    // Start is called before the first frame update
    void Start()
    {
        //HealthBar.setMaxHealth(MaxHP);
        //HealthBar.setHealth(HP);
    }

    // Update is called once per frame
    void Update()
    {
        //HealthBar.setHealth(HP);
        if (HP <= 0)
        {
            Debug.Log("mort");
            Destroy(gameObject);
        }
    }
}
