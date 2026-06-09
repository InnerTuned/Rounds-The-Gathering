using System;
using UnityEngine;

// Token: 0x020000A1 RID: 161
public class PlayerSkinHandler : MonoBehaviour
{
	// Token: 0x06000392 RID: 914 RVA: 0x00015E61 File Offset: 0x00014061
	private void Start()
	{
		this.Init();
	}

	// Token: 0x06000393 RID: 915 RVA: 0x00015E6C File Offset: 0x0001406C
	private void Init()
	{
		if (this.inited)
		{
			return;
		}
		this.inited = true;
		this.ToggleSimpleSkin(this.simpleSkin);
		this.data = base.GetComponentInParent<CharacterData>();
		if (!this.simpleSkin)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(PlayerSkinBank.GetPlayerSkinColors(this.data.player.playerID).gameObject, base.transform.position, base.transform.rotation, base.transform);
			this.skins = gameObject.GetComponentsInChildren<PlayerSkinParticle>();
		}
	}

	// Token: 0x06000394 RID: 916 RVA: 0x00015EF1 File Offset: 0x000140F1
	public void TakeDamageBlink(Vector2 damage, bool selfDamage)
	{
		this.BlinkColor(Color.white * 0.95f);
	}

	// Token: 0x06000395 RID: 917 RVA: 0x00015F08 File Offset: 0x00014108
	public void BlinkColor(Color blinkColor)
	{
		if (this.skins != null)
		{
			for (int i = 0; i < this.skins.Length; i++)
			{
				this.skins[i].BlinkColor(blinkColor);
			}
		}
	}

	// Token: 0x06000396 RID: 918 RVA: 0x00015F40 File Offset: 0x00014140
	public void InitSpriteMask(int spriteLayerID)
	{
		this.Init();
		for (int i = 0; i < this.skins.Length; i++)
		{
			this.skins[i].Init(spriteLayerID);
		}
	}

	// Token: 0x06000397 RID: 919 RVA: 0x00015F74 File Offset: 0x00014174
	public void ToggleSimpleSkin(bool isSimple)
	{
		this.simpleSkin = isSimple;
		base.GetComponent<SetPlayerSpriteLayer>().ToggleSimple(isSimple);
	}

	// Token: 0x040004AE RID: 1198
	public bool simpleSkin;

	// Token: 0x040004AF RID: 1199
	private PlayerSkinParticle[] skins;

	// Token: 0x040004B0 RID: 1200
	private CharacterData data;

	// Token: 0x040004B1 RID: 1201
	private bool inited;
}
