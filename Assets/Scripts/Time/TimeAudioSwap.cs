using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeAudioSwap : TimeSwap
{
    [SerializeField] private AudioSource magic = null;
    [SerializeField] private AudioSource tech = null;

    private void Start()
    {
        this.magic.volume = 1f;
        this.tech.volume = 0f;
    }

    public override void Swap()
    {
        this.magic.volume = this.magic.volume == 1f ? 0f : 1f;
        this.tech.volume = this.tech.volume == 1f ? 0f : 1f;
    }
}
