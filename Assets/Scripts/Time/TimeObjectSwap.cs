using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeObjectSwap : TimeSwap
{
    [SerializeField] private GameObject magic = null;
    [SerializeField] private GameObject tech = null;

    public void Start()
    {
        this.magic.SetActive(true);
        this.tech.SetActive(false);
    }

    public override void Swap()
    {
        this.magic.SetActive(!this.magic.activeSelf);
        this.tech.SetActive(!this.tech.activeSelf);
    }
}
