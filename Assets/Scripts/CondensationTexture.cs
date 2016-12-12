using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CondensationTexture : MonoSingleton <CondensationTexture> 
{

	public List<Sprite> condensationSprites = new List<Sprite>();

	int currentSpriteIndex = 0;

	SpriteRenderer _spriteRenderer;
	SpriteRenderer spriteRenderer
	{
		get
		{
			if (_spriteRenderer == null)
				_spriteRenderer = GetComponent<SpriteRenderer>();
			return _spriteRenderer;
		}
	}

	void Start()
	{
		UpdateTexture();
	}

	public void SwitchTexture()
	{
		currentSpriteIndex = (currentSpriteIndex + 1) % condensationSprites.Count;
		UpdateTexture();
	}

	void UpdateTexture()
	{
		spriteRenderer.sprite = condensationSprites[currentSpriteIndex];
	}

}
