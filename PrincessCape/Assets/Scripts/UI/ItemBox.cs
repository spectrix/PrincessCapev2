﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemBox : MonoBehaviour {
    Image background;
    Image itemImage;
    Text keyBox;
    MagicItem item;
    [SerializeField]
    MagicItemSlot slot;
	// Use this for initialization
	void Awake () {
        Init();
	}

    public void Init() {
        background = GetComponent<Image>();
        itemImage = GetComponentsInChildren<Image>()[1];

        keyBox = transform.parent.GetComponentInChildren<Text>();
    }

    public MagicItem Item {
        get {
            return item;
        }

        set {

            if (IsHidden) {
                IsHidden = false;
            }
            if (item) {
                item.OnActivationStateChange.RemoveListener(OnItemActivationStateChanged);
                item.Slot = MagicItemSlot.None;
            }

            item = value;
            item.Slot = slot;
            item.OnActivationStateChange.AddListener(OnItemActivationStateChanged);
            if (item != null)
            {
                itemImage.sprite = item.Sprite;
            }
        }
    }
    private void OnEnable()
    {
		if (Game.Instance && Game.Instance.Player && Game.Instance.Player.Inventory.Count == 0)
		{
			IsHidden = true;
		}
    }

    private void OnDisable()
    {
        if (item) {
            item.OnActivationStateChange.RemoveListener(OnItemActivationStateChanged);
        }
    }

    void OnItemActivationStateChanged(MagicItemState state) {
        if (state == MagicItemState.Activated) {
            background.color = Color.blue;
        } else if (state == MagicItemState.OnCooldown) {
            background.color = Color.gray;
        } else if (state == MagicItemState.Ready) {
            background.color = Color.white;
        }
    }

    void Clear() {
        itemImage.sprite = null;
        background.color = Color.white;
    }

	public bool IsHidden
	{
		set
		{
			foreach (Image i in transform.parent.GetComponentsInChildren<Image>())
			{
				i.enabled = !value;
			}

			foreach (Text t in transform.parent.GetComponentsInChildren<Text>())
			{
				t.enabled = !value;
			}

		}

		get
		{
			return !GetComponent<Image>().enabled;
		}
	}

    public string KeyText {
        get {
            return keyBox.text;
        }

        set {
            if (keyBox == null) {
                Init();
            }
            keyBox.text = value;
        }
    }
}
