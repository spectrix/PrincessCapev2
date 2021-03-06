﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullGauntlet : MagneticGlove
{
    public PullGauntlet() {
        itemName = "Pull Gauntlet";

		itemGetMessage = new List<string>() {
			"You got the Pull Gaunlet!",
			"Slipping it on, you feel the power flowing through your hands",
            "Press [[[Inventory]]] and equipt it with [[[ItemOne]]] or [[[ItemTwo]]]",
			"Press and hold the item button to pull metal blocks towards you",
            "But beware, if the block can't be moved.  You will be."
		};

		itemDescritpion = "Press and hold the Item Key to pull on metal objects.";
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
        if (Game.Instance.IsPlaying)
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
                            Vector2 inputForce = Vector2.up * Controller.Instance.Vertical + Vector2.right * Controller.Instance.Horizontal;
                            Game.Instance.Player.Rigidbody.AddForce((-Direction + inputForce.normalized) * force);
                            Game.Instance.Player.Rigidbody.ClampVelocity(maxSpeed);
                        }
                        else
                        {
                            Vector2 inputForce = Vector2.up * Controller.Instance.Vertical + Vector2.right * Controller.Instance.Horizontal;
                            target.Rigidbody.AddForce((Direction + inputForce.normalized) * force);
                            target.Rigidbody.ClampVelocity(maxSpeed);
                        }
                    }
                    else
                    {
                        ClearTarget();
                    }
                }
            }
        }
    }

	/// <summary>
	/// Ends the use of the glove
	/// </summary>
	public override void Deactivate()
    {
        if (Game.Instance.IsPlaying)
        {
            if (state == MagicItemState.Activated)
            {
                ClearTarget();
                Game.Instance.Player.IsPulling = false;
                State = MagicItemState.OnCooldown;
                cooldownTimer.Start();
            }
        }
    }
}
