﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : ActivatedObject
{
    Animator myAnimator;
    public override void Activate()
    {
        isActivated = true;
        transform.GetChild(0).gameObject.SetActive(true);
        myAnimator.SetBool("isActivated", true);
    }

    public override void Deactivate()
    {
        isActivated = false;
        transform.GetChild(0).gameObject.SetActive(false);
        myAnimator.SetBool("isActivated", false);
    }

    // Use this for initialization
    void Start()
    {
        myAnimator = GetComponent<Animator>();
        transform.GetChild(0).gameObject.SetActive(false);
    }

#if UNITY_EDITOR
    public override void ScaleY(bool up)
    {
        transform.GetChild(0).GetComponent<AirColumn>().ScaleY(up);
    }

    protected override string GenerateSaveData()
    {
        string data = base.GenerateSaveData();
        data += PCLParser.CreateAttribute("Air Scale", transform.GetChild(0).localScale.y);
        return data;
    }

    public override void FromData(TileStruct tile)
    {
        base.FromData(tile);
        GameObject air = transform.GetChild(0).gameObject;
        float airScale = float.Parse(PCLParser.ParseLine(tile.info[3]));
        air.transform.localScale = air.transform.localScale.SetY(airScale);
    }
#endif
}