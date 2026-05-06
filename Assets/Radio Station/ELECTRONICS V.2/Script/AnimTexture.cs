using UnityEngine;
using System.Collections;

public class AnimTexture : MonoBehaviour {

	public int uvTieX = 1;
	public int uvTieY = 1;
	public int fps = 10;

	private Vector2 size;
	private Renderer myRenderer;
	private int lastIndex = -1;

	void Start () 
	{
		size = new Vector2 (1.0f / uvTieX , 1.0f / uvTieY);
		myRenderer = GetComponent<Renderer> ();
		if(myRenderer == null)
			enabled = false;
	}

	void Update()
	{
		int index = (int)(Time.timeSinceLevelLoad * fps) % (uvTieX * uvTieY);
		if(index != lastIndex)
		{
			int uIndex = index % uvTieX;
			int vIndex = index / uvTieY;
			Vector2 offset = new Vector2 (uIndex * size.x, 1.0f - size.y - vIndex * size.y);

			myRenderer.material.SetTextureOffset ("_MainTex", offset);
			myRenderer.material.SetTextureScale ("_MainTex", size);
			lastIndex = index;
		}
	}
}