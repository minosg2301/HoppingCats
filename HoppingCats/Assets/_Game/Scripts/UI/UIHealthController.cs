using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using moonNest;
using Doozy.Engine.Progress;

public class UIHealthController : SingletonMono<UIHealthController>
{
    [HideInInspector] public float currentCatHP;
    [SerializeField] private float decreaseHP;
    [SerializeField] private float catInitialHP = 100f;

    public Progressor  healthProgressor;

    protected override void Start()
    {
        base.Start();

        currentCatHP = catInitialHP;

    }

    private void Update()
    {
        DecreaseHP();
    }

    void DecreaseHP()
    {
        currentCatHP -= decreaseHP * Time.deltaTime;
        healthProgressor.SetValue(currentCatHP/catInitialHP);
        if (currentCatHP <= 0f)
        {
            Debug.Log("Dat - Game Lost");
        }
    }

    
}
