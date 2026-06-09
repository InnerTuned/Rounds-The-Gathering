using System;
using Sirenix.OdinInspector;
using UnityEngine;

// Token: 0x02000114 RID: 276
public class CharacterItem : MonoBehaviour
{
	// Token: 0x0600057C RID: 1404 RVA: 0x0001F97D File Offset: 0x0001DB7D
	[Button]
	public void SaveTransform()
	{
		this.offset = base.transform.localPosition;
		this.scale = base.transform.localScale.x;
	}

	// Token: 0x0600057D RID: 1405 RVA: 0x0001F9AC File Offset: 0x0001DBAC
	private void Start()
	{
		if (base.transform.root.GetComponent<Player>())
		{
			base.gameObject.AddComponent<CharacterItemMirror>();
			if (this.moveHealthBarUp != 0f)
			{
				HealthBar componentInChildren = base.transform.root.GetComponentInChildren<HealthBar>();
				if (componentInChildren)
				{
					componentInChildren.transform.localPosition += Vector3.up * this.moveHealthBarUp;
				}
			}
		}
	}

	// Token: 0x04000719 RID: 1817
	public Sprite sprite;

	// Token: 0x0400071A RID: 1818
	public float scale = 1f;

	// Token: 0x0400071B RID: 1819
	public Vector2 offset = Vector2.zero;

	// Token: 0x0400071C RID: 1820
	public CharacterItemType itemType;

	// Token: 0x0400071D RID: 1821
	internal int slotNr;

	// Token: 0x0400071E RID: 1822
	[ShowIf("itemType", CharacterItemType.Detail, true)]
	public float moveHealthBarUp;
}
