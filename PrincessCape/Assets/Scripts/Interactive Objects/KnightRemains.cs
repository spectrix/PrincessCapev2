﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class KnightRemains : InteractiveObject
{

	[SerializeField]
	ItemLevel itemOnRemains = ItemLevel.MagicCape;
	[SerializeField]
	string knightName = "Sir Matthew";
	[SerializeField]
	TextAsset messageFile;
    bool itemGiven = false;
    public override void Interact()
    {
        if (!itemGiven)
        {
            UIManager.Instance.SetInteractionText("");
            IsHighlighted = false;
            //Add item to the player's inventory
            if (messageFile != null)
            {
                UIManager.Instance.SetMainText(messageFile.text.Split('\n').ToList());
                SpeakerBox.SetSpeaker(knightName);
                EventManager.StartListening("EndOfMessage", GiveItem);
                EventManager.TriggerEvent("ShowDialog");
            }

        }
    }

    void GiveItem()
    {
        itemGiven = true;
        IsHighlighted = false;
        EventManager.StopListening("EndOfMessage", GiveItem);
        Game.Instance.Player.AddItem(ScriptableObject.CreateInstance(itemOnRemains.ToString()) as MagicItem, true);
        Timer fadeOutTimer = new Timer(0.05f, 20);
        fadeOutTimer.OnTick.AddListener(()=>{
            myRenderer.color = myRenderer.color.SetAlpha(myRenderer.color.a - 0.05f);
        });

        fadeOutTimer.OnComplete.AddListener(()=> {
            Destroy(gameObject);
        });

        fadeOutTimer.Start();
    }

    /// <summary>
    /// Highlight the object when it touches the player
    /// </summary>
    /// <param name="collision">Collision.</param>
    public new void OnTriggerEnter2D(Collider2D collision)
    {
        if (!itemGiven && collision.CompareTag("Player") && !Game.Instance.Player.IsHoldingItem)
        {
            IsHighlighted = true;
        }
    }

    public ItemLevel Item {
        get {
            return itemOnRemains;
        }

        set {
            itemOnRemains = value;
        }
    }

    public string KnightName {
        get {
            return knightName;
        }

        set {
            knightName = value;
        }
    }

    public TextAsset Message {
        get {
            return messageFile;
        }

        set {
            messageFile = value;
        }
    }
}