using System;
using UnityEngine;

// Token: 0x020000CE RID: 206
public class SetRotation : MonoBehaviour
{
	// Token: 0x0600043F RID: 1087 RVA: 0x00019BC4 File Offset: 0x00017DC4
	private void Init()
	{
		if (this.inited)
		{
			return;
		}
		this.inited = true;
		int attackLevel = base.GetComponentInParent<AttackLevel>().attackLevel;
		this.setRot += (float)attackLevel * this.extraSetPerLevel;
	}

	// Token: 0x06000440 RID: 1088 RVA: 0x00019C03 File Offset: 0x00017E03
	public void Set()
	{
		this.Init();
		this.rot = this.setRot;
		base.transform.localRotation = Quaternion.Euler(new Vector3(this.rot, 0f, 0f));
	}

	// Token: 0x06000441 RID: 1089 RVA: 0x00019C3C File Offset: 0x00017E3C
	public void Add()
	{
		this.Init();
		this.rot += this.addRot;
		base.transform.localRotation = Quaternion.Euler(new Vector3(this.rot, 0f, 0f));
	}

	// Token: 0x040005C5 RID: 1477
	public float setRot;

	// Token: 0x040005C6 RID: 1478
	public float addRot;

	// Token: 0x040005C7 RID: 1479
	public float extraSetPerLevel;

	// Token: 0x040005C8 RID: 1480
	private float rot;

	// Token: 0x040005C9 RID: 1481
	private bool inited;
}
