using System;
using UnityEngine;

// Token: 0x020000CD RID: 205
public class SetPlayerSpriteLayer : MonoBehaviour
{
	// Token: 0x0600043B RID: 1083 RVA: 0x00019ABC File Offset: 0x00017CBC
	private void Start()
	{
		this.simpleSkin = base.GetComponent<PlayerSkinHandler>().simpleSkin;
		Player componentInParent = base.GetComponentInParent<Player>();
		int num = SortingLayer.NameToID("Player" + (componentInParent.playerID + 1).ToString());
		this.setSpriteLayerOfChildren(base.GetComponentInParent<Holding>().holdable.gameObject, num);
		this.setSpriteLayerOfChildren(base.gameObject, num);
		if (!this.simpleSkin)
		{
			base.GetComponent<PlayerSkinHandler>().InitSpriteMask(num);
		}
	}

	// Token: 0x0600043C RID: 1084 RVA: 0x00019B3C File Offset: 0x00017D3C
	private void setSpriteLayerOfChildren(GameObject obj, int layer)
	{
		this.sprites = obj.transform.root.GetComponentsInChildren<SpriteMask>();
		for (int i = 0; i < this.sprites.Length; i++)
		{
			if (this.simpleSkin)
			{
				this.sprites[i].enabled = false;
				this.sprites[i].GetComponent<SpriteRenderer>().enabled = true;
			}
			else
			{
				this.sprites[i].frontSortingLayerID = layer;
				this.sprites[i].backSortingLayerID = layer;
			}
		}
	}

	// Token: 0x0600043D RID: 1085 RVA: 0x00019BB9 File Offset: 0x00017DB9
	public void ToggleSimple(bool isSimple)
	{
		this.simpleSkin = isSimple;
	}

	// Token: 0x040005C3 RID: 1475
	private SpriteMask[] sprites;

	// Token: 0x040005C4 RID: 1476
	private bool simpleSkin;
}
