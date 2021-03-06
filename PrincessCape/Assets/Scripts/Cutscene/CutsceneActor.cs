﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class CutsceneActor : MonoBehaviour
{
	SpriteRenderer mySpriteRenderer;
    Animator myAnimator;
	bool isHidden = true;
    [SerializeField]
    string characterName = "Character";

    // Use this for initialization
    private void Awake()
    {
        if (Application.isEditor) {
            Init();
        } 
    }
    public void Init() {
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        myAnimator = GetComponent<Animator>();
        IsHidden = true;
    }

    Timer CreateTimer(float time) {
		int totalTicks = Mathf.FloorToInt(time / 0.03f);
		Timer timer = new Timer(0.03f, totalTicks);
        return timer;
    }

    public void Animate(string trigger) {
        myAnimator.SetTrigger(trigger);
    }

	public string CharacterName
	{
		get
		{
			return characterName;
		}

        set {
            characterName = value;
        }
	}

	public Sprite MySprite
	{
		get
		{
			return mySpriteRenderer.sprite;
		}

		set
		{
			mySpriteRenderer.sprite = value;
		}
	}

	public Vector3 Position
	{
		get
		{
			return transform.position;
		}

		set
		{
			transform.position = value;
		}
	}

	public void FlipX()
	{
		mySpriteRenderer.flipX = !mySpriteRenderer.flipX;
	}

	public void FlipY()
	{
		mySpriteRenderer.flipY = !mySpriteRenderer.flipY;
	}
	public bool IsHidden
	{
		get
		{
			return isHidden;
		}

		set
		{
			isHidden = value;
            mySpriteRenderer.color = mySpriteRenderer.color.SetAlpha(value ? 0 : 255);
		}
	}

	public void DestroySelf()
	{
		Destroy(gameObject);
	}

    public void Fade(bool fadeIn, float time) {
        Timer fadeTimer = CreateTimer(time);
        if (fadeIn) {
            fadeTimer.OnTick.AddListener(() =>
            {
                mySpriteRenderer.color = mySpriteRenderer.color.SetAlpha(fadeTimer.RunPercent);
            });

            fadeTimer.OnComplete.AddListener(() =>
            {
                mySpriteRenderer.color = mySpriteRenderer.color.SetAlpha(1.0f);
            });
        } else {
            fadeTimer.OnTick.AddListener(() =>
            {
                mySpriteRenderer.color = mySpriteRenderer.color.SetAlpha(1.0f - fadeTimer.RunPercent);
            });

            fadeTimer.OnComplete.AddListener(() =>
            {
                mySpriteRenderer.color = mySpriteRenderer.color.SetAlpha(0.0f);
            });
        }
    } 
}

