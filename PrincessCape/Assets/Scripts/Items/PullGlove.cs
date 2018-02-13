﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullGlove : MagneticGlove
{
    public PullGlove() {
        itemName = "Pull Gauntlet";
    }

    private void OnEnable()
    {
        itemSprite = Resources.Load<Sprite>("Sprites/PullGauntlet");
    }
    /// <summary>
    /// Pulls the Target towards the player or vice versa
    /// </summary>
    public override void Use()
    {
        if (state == MagicItemState.Activated)
        {
            if (!target)
            {
                FindTarget();
            }

            if (target)
            {
                if (IsTargetInRange)
                {
                    Game.Instance.Player.IsPulling = true;
                    if (target.IsStatic)
                    {
                        Game.Instance.Player.Rigidbody.AddForce(-Direction * force);
                    }
                    else
                    {

                        target.Rigidbody.AddForce(Direction * force);
                    }
                } else {
                    ClearTarget();
                }
            }
        }
    }

	/// <summary>
	/// Ends the use of the glove
	/// </summary>
	public override void Deactivate()
    {
        if (state == MagicItemState.Activated)
        {
			if (slot == MagicItemSlot.First)
			{
				EventManager.TriggerEvent("ItemOneDeactivatedSuccessfully");
			}
			else if (slot == MagicItemSlot.Second)
			{
                Debug.Log("Deactivated");
				EventManager.TriggerEvent("ItemTwoDeactivatedSuccessfully");
			}
            ClearTarget();
            Game.Instance.Player.IsPulling = false;
            state = MagicItemState.OnCooldown;
            cooldownTimer.Start();
        }
    }
}
