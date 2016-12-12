using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CachedPoint
{
	public float alpha;
	public bool isAffected = false;
}

public class GlassCondensation : MonoBehaviour {

	List<List<float>> cachedTextureAlpha = new List<List<float>>();

	static readonly Color WhiteClear = new Color(1f, 1f, 1f, 0f);

	public int sizeMaskTexture = 256;

	public float speedRecovery = 1f;
	public float radius = 1f;
	public float spriteSizeX = 4f;
	public float spriteSizeY = 4f;
	[Range(0f, 1f)]
	public float fullClearEdge = 0.5f;
	[Range(0f, 1f)]
	public float fullClearPower = 0.9f;
	public float timeBetweenRecovers = 0.1f;
	public AudioSource audioPlayer;

	[Header("Continuity")]
	public bool continuityEnabled = true;
	public float maxDistBetweenCircles;
	public int maxCirclesPerFrame = 4;

	public float averageAlpha = 1f;

	public float fullAlpha = 1f;

	Vector2 lastPosition = new Vector2(float.NaN, float.NaN);
	float lastTimeRecovered;
	float targetAudioVolume = 0f;
	bool _isWiping = false;
	public bool isWiping { get { return _isWiping; } private set { _isWiping = value; } }

	public Action OnWipe;

	Texture2D _maskTexture;
	public Texture2D maskTexture
	{
		get
		{
			
			if (_maskTexture == null)
			{
				_maskTexture = new Texture2D(sizeMaskTexture, sizeMaskTexture, TextureFormat.Alpha8, false);
				spriteRenderer.material.SetTexture("_Mask", _maskTexture);
				Color c = new Color(1f, 1f, 1f, fullAlpha);
				for (int i = 0 ; i < maskTexture.width ; i++)
				{
					cachedTextureAlpha.Add(new List<float>());
					for (int j = 0 ; j < maskTexture.height ; j++)
					{
						maskTexture.SetPixel(i, j, c);
						cachedTextureAlpha[i].Add(fullAlpha);
					}
				}
				_maskTexture = (Texture2D)spriteRenderer.material.GetTexture("_Mask");
			}
			return _maskTexture;
		}
	}

	void SetPixel(int x, int y, float a)
	{
		Color c = Color.white;
		c.a = a;
		maskTexture.SetPixel(x, y, c);
		cachedTextureAlpha[x][y] = a;
	}

	float GetPixel(int x, int y)
	{
		return cachedTextureAlpha[x][y];
	}

	SpriteRenderer _spriteRenderer;
	public SpriteRenderer spriteRenderer
	{
		get
		{
			if (_spriteRenderer == null)
				_spriteRenderer = GetComponent<SpriteRenderer>();
			return _spriteRenderer;
		}
	}

	void Awake()
	{
		lastTimeRecovered = Time.time;
	}

	void Update () 
	{
		targetAudioVolume = 0f;
		if (Input.GetMouseButton(0))
		{
			Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			if (IsInSprite(worldPos))
			{
				if (!isWiping)
				{
					if (OnWipe != null)
						OnWipe();
				}
				isWiping = true;
				ClearCircle(worldPos);
				if (continuityEnabled)
				{
					if (!lastPosition.IsNaN())
					{
						if ((worldPos - lastPosition).magnitude > 0.001f)
							targetAudioVolume = 1f;
						int nbAdditionalCircles = Mathf.RoundToInt((lastPosition - worldPos).magnitude / maxDistBetweenCircles) - 1;
						nbAdditionalCircles = Mathf.Min(maxCirclesPerFrame, nbAdditionalCircles);
						for ( int i = 0 ; i < nbAdditionalCircles ; i++)
						{
							Vector2 circlePos = (lastPosition * (i + 1) + worldPos * (nbAdditionalCircles - (i + 1))) / nbAdditionalCircles;
							ClearCircle(circlePos);
						}
					}
					lastPosition = worldPos;
				}
			}
			else
			{
				lastPosition = new Vector2(float.NaN, float.NaN);
				isWiping = false;
			}
//			ChefBubbleManager.instance.bubbleChefAnimator.UpdateMachineNeedsWipe(false);
		}
		else
		{
			lastPosition = new Vector2(float.NaN, float.NaN);
			if (Input.GetMouseButtonUp(0))
				lastTimeRecovered = Time.time;
			RecoverCondensation();
//			ChefBubbleManager.instance.bubbleChefAnimator.UpdateMachineNeedsWipe(averageAlpha > 0.5f);
			isWiping = false;
		}
		maskTexture.Apply();
		UpdateAudioVolume();
	}

	void UpdateAudioVolume()
	{
		if (Input.GetMouseButtonUp(0))
			audioPlayer.volume = 0f;
		else if (Input.GetMouseButtonDown(0) && isWiping)
			audioPlayer.volume = 1f;
		else
			audioPlayer.volume = Mathf.Lerp(audioPlayer.volume, targetAudioVolume, Time.deltaTime * 5f);
	}

	bool IsInSprite(Vector2 position)
	{
		Vector3 localPos = transform.InverseTransformPoint(position);
		return Mathf.Abs(localPos.x) <= spriteSizeX / 2f && Mathf.Abs(localPos.y) <= spriteSizeY / 2f;
	}

	void ClearCircle(Vector2 position)
	{
		Vector3 localPos = transform.InverseTransformPoint(position);
		float localRadiusX = transform.InverseTransformVector(Vector3.one * radius).x;
		float localRadiusY = transform.InverseTransformVector(Vector3.one * radius).y;
		float pixelRadiusX = localRadiusX * maskTexture.width / spriteSizeX;
		float pixelRadiusY = localRadiusY * maskTexture.width / spriteSizeY;
		int xCenter = (int)Mathf.Lerp(0f, (float)maskTexture.width, Mathf.InverseLerp(-spriteSizeX / 2f, spriteSizeX / 2f, localPos.x));
		int yCenter = (int)Mathf.Lerp(0f, (float)maskTexture.height, Mathf.InverseLerp(-spriteSizeY / 2f, spriteSizeY / 2f, localPos.y));
		int xMin = xCenter - (int)pixelRadiusX;
		int xMax = xCenter + (int)pixelRadiusX;
		int yMin = yCenter - (int)pixelRadiusY;
		int yMax = yCenter + (int)pixelRadiusY;
		for (int i = xMin ; i < xMax ; i++)
		{
			for (int j = yMin ; j < yMax ; j++)
			{
				int difX = i - xCenter;
				int difY = j - yCenter;
				float SqrDistToCenter = difX * difX + difY * difY;
				if (SqrDistToCenter < pixelRadiusX * pixelRadiusY)
				{
					float a = GetPixel(i, j);
					float normalizedDistToCenter = Mathf.Sqrt( SqrDistToCenter) / pixelRadiusX;
					float normalizedDistToEdge = Mathf.InverseLerp(fullClearEdge, fullAlpha, normalizedDistToCenter);
					float power = Mathf.Lerp(fullClearPower, 0f, normalizedDistToEdge);
					float min = Mathf.Lerp(fullAlpha - fullClearPower, fullAlpha, normalizedDistToEdge);
					float newAlpha = Mathf.Max(min, a - power);
					if (a > newAlpha)
					{
						a = newAlpha;
						SetPixel(i, j, a);
					}
				}
			}
		}
	}

	public void ClearAll()
	{
		float averageAlpha = 0f;
		for (int i = 0 ; i < maskTexture.width ; i++)
		{
			for (int j = 0 ; j < maskTexture.height ; j++)
			{
				float a = GetPixel(i, j);
				a = fullAlpha - fullClearPower;
				SetPixel(i, j, a);
				averageAlpha += a;
			}
		}
		averageAlpha = averageAlpha / (float) (maskTexture.width * maskTexture.height);
		this.averageAlpha = averageAlpha;
	}

	void RecoverCondensation()
	{
		if (lastTimeRecovered + timeBetweenRecovers < Time.time)
		{
			float averageAlpha = 0f;
			float recoverPower = speedRecovery * (Time.time - lastTimeRecovered);
			for (int i = 0 ; i < maskTexture.width ; i++)
			{
				for (int j = 0 ; j < maskTexture.height ; j++)
				{
					float a = GetPixel(i, j);
					averageAlpha += a;
					if (a < fullAlpha)
					{
						a += recoverPower;
						if (a > fullAlpha)
							a = fullAlpha;
						SetPixel(i, j, a);
					}
				}
			}
			averageAlpha = averageAlpha / (float) (maskTexture.width * maskTexture.height);
			this.averageAlpha = averageAlpha;
			lastTimeRecovered = Time.time;
		}

	}
}
