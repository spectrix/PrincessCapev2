﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressureSwitch : ActivatorObject {
    Animator myAnimator;
    int itemsOnTop = 0;

    /// <summary>
    /// Start this instance.
    /// </summary>
    private void Start()
    {
        myAnimator = GetComponent<Animator>();
    }

    /// <summary>
    /// When a Rigidbody with a great enough mass collides with the switch, increment ItemsOnTop.
    /// </summary>
    /// <param name="collision">Collision.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.attachedRigidbody.mass >= 0.1f) {
            ItemsOnTop++;
        }
    }

	/// <summary>
	/// When a Rigidbody with a great enough mass stops colliding with the switch, decrement ItemsOnTop.
	/// </summary>
	/// <param name="collision">Collision.</param>
	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.attachedRigidbody.mass >= 0.1f)
		{
			ItemsOnTop--;
		}
	}

    /// <summary>
    /// Gets or sets the number items on top.  If value > 0, Activate.  Else, Deactive.
    /// </summary>
    /// <value>The items on top.</value>
    int ItemsOnTop {
        get {
            return itemsOnTop;
        }

        set {
            itemsOnTop = Mathf.Max(value, 0);

            if (itemsOnTop > 0) {
                Activate();
            } else {
                Deactivate();
            }
         }
    }

    /// <summary>
    /// Activate this instance and all connected ActivatedObjects.
    /// </summary>
    public override void Activate()
    {
        if (!IsActivated)
        {
            IsActivated = true;
            base.Activate();
        }
    }

	/// <summary>
	/// Deactivate this instance and all connected ActivatedObjects.
	/// </summary>
	public override void Deactivate() {
        if (IsActivated) {
            IsActivated = false;
            base.Deactivate();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:PressureSwitch"/> is activated.
    /// </summary>
    /// <value><c>true</c> if is activated; otherwise, <c>false</c>.</value>
    public override bool IsActivated
    {
        get
        {
            return base.IsActivated;
        }
        protected set
        {
            base.IsActivated = value;
            myAnimator.SetBool("isActivated", value);
        }
    }


}