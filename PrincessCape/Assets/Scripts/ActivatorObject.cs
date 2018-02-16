﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatorObject : ActivatedObject {
	List<int> connectionIDs;
    /// <summary>
    /// Activate this instance.
    /// </summary>
    public override void Activate()
    {
        foreach(ActivatedObject ao in connections) {
            if (ao)
            {
                ao.Activate();
            }
        }
    }

    /// <summary>
    /// Deactivate this instance.
    /// </summary>
    public override void Deactivate()
    {
        foreach (ActivatedObject ao in connections)
		{
            if (ao)
            {
                ao.Deactivate();
			}
		}
    }

	protected override string GenerateSaveData()
	{
		string info = base.GenerateSaveData();

		info += PCLParser.CreateArray("Connections");
		foreach (ActivatedObject ao in connections)
		{
			if (ao)
			{
				info += PCLParser.CreateAttribute("CI", ao.ID);
			}

		}
		info += PCLParser.ArrayEnding;
		return info;
	}

	public override void FromData(TileStruct tile)
	{
		base.FromData(tile);
		connectionIDs = new List<int>();

		if (tile.info[3].Contains("Connections"))
		{
			for (int i = 4; i < tile.info.Count && tile.info[i] != "],"; i++)
			{
				connectionIDs.Add(PCLParser.ParseInt(tile.info[i]));
			}
		}
	}

	public void Reconnect()
	{
		Map map = GetComponentInParent<Map>();
		foreach (int i in connectionIDs)
		{
			AddConnection(map.GetTileByID(i) as ActivatedObject);
		}
		connectionIDs.Clear();
	}

	protected int NumConnections
	{
		get
		{
			return Mathf.Max(connectionIDs.Count, connections.Count);
		}
	}
}
