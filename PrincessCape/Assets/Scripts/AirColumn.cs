﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirColumn : MapTile {

    private void OnTriggerStay2D(Collider2D collision)
    {
        collision.attachedRigidbody.AddForce(transform.up * 50);
    }
}