﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public abstract class ActivatedObject : MapTile
{

    [SerializeField]
    protected List<ActivatedObject> connections = new List<ActivatedObject>();
    [SerializeField]
    protected bool startActive = false;
    protected bool isActivated = false;

    private void Awake()
    {
        Init();

    }
    
	public override void Init()
	{
		base.Init();

		if (startActive && Application.isPlaying)
        {
            Activate();
            //EventManager.StartListening("LevelLoaded", Activate);
        }
	}
	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="T:ActivatedObject"/> is activated.
	/// </summary>
	/// <value><c>true</c> if is activated; otherwise, <c>false</c>.</value>
	public virtual bool IsActivated
    {
        get
        {
            return isActivated;
        }

        protected set
        {
            isActivated = value;
        }
    }

    /// <summary>
    /// Activate this instance.
    /// </summary>
    public abstract void Activate();

    /// <summary>
    /// Deactivate this instance.
    /// </summary>
    public abstract void Deactivate();

    /// <summary>
    /// Adds a connection to another <see cref="T:ActivatedObject"/> if they are not already connected
    /// </summary>
    /// <param name="ao">Ao.</param>
    public void AddConnection(ActivatedObject ao)
    {
        if (ao && !HasConnection(ao))
        {
            if (this is ActivatorObject) {
                ao.StartsActive = startActive;
            }
            connections.Add(ao);
            ao.AddConnection(this);
        }
    }

    /// <summary>
    /// Removes a connection to another <see cref="T:ActivatedObject"/> if they are connected
    /// </summary>
    /// <param name="ao">Ao.</param>
    public void RemoveConnection(ActivatedObject ao)
    {
        if (HasConnection(ao))
        {

            connections.Remove(ao);
            ao.RemoveConnection(this);
        }
    }

    /// <summary>
    /// Gets the connections.
    /// </summary>
    /// <value>The connections.</value>
    public List<ActivatedObject> Connections
    {
        get
        {
            return connections;
        }
    }

    /// <summary>
    /// Whether or not this ActivatedObject is connected to the other.
    /// </summary>
    /// <returns><c>true</c>, If this is connected to ao, <c>false</c> otherwise.</returns>
    /// <param name="ao">Ao.</param>
    public bool HasConnection(ActivatedObject ao)
    {
        return connections.Contains(ao);
    }

    protected override string GenerateSaveData()
    {
        string data = base.GenerateSaveData();
        data += PCLParser.CreateAttribute("Starts Active", startActive);
        return data;
    }

    public override void FromData(TileStruct tile)
    {
		base.FromData(tile);
		StartsActive = PCLParser.ParseBool(tile.NextLine);
    }
#if UNITY_EDITOR
    /// <summary>
    /// Draws indicators of the connections between the Activated Object and its connections as well as the activation status of the object
    /// </summary>
    public override void RenderInEditor()
    {
		foreach (ActivatedObject acto in Connections)
        {
            if (acto)
            {
                Handles.DrawDottedLine(Center, acto.Center, 8.0f);
            }
        }

        Handles.color = Color.black;
        Handles.DrawSolidArc(Center, -Vector3.forward, Vector3.up, 360, 0.4f);

        if (startActive)
        {
            Handles.color = Color.green;

        }
        else
        {
            Handles.color = Color.red;
        }
        Handles.DrawSolidArc(Center, -Vector3.forward, Vector3.up, 360, 0.33f);
        Handles.color = Color.white;
    }

    #endif
    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:ActivatedObject"/> starts active.
    /// </summary>
    /// <value><c>true</c> if starts active; otherwise, <c>false</c>.</value>
    public bool StartsActive {
        get {
            return startActive;
        }

        set {
            startActive = value;

            if (this is ActivatorObject) {
                foreach(ActivatedObject ao in connections) {
                    ao.StartsActive = value;
                }
            }
        }
    }

}
