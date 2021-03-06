﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileSelectButton : Button
{
    bool isSelected = false;
    MapTile tilePrefab;
    Image tileSprite;
    public TileBrowser editor;
    protected override void Awake()
    {
        base.Awake();
        tileSprite = GetComponentsInChildren<Image>()[1];

    }

    public bool IsSelected
    {
        get
        {
            return isSelected;
        }

        set {
            isSelected = value;
            image.color = value ? Color.red : Color.white;
        }
    }

    public MapTile Tile
    {
        get
        {
            return tilePrefab;
        }

        set
        {
            tilePrefab = value;
            tileSprite.sprite = tilePrefab.ButtonSprite;
        }
    }

    public void SelectButton()
    {
        editor.SelectButton(this);
    }
}
