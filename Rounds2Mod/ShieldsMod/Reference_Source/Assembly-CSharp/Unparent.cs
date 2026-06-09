using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000EC RID: 236
public class Unparent : MonoBehaviour
{
	// Token: 0x060004C5 RID: 1221 RVA: 0x0001BC48 File Offset: 0x00019E48
	private void Start()
	{
		this.parent = base.transform.root;
	}

	// Token: 0x060004C6 RID: 1222 RVA: 0x0001BC5C File Offset: 0x00019E5C
	private void LateUpdate()
	{
		if (this.done)
		{
			return;
		}
		if (base.transform.root != null)
		{
			base.transform.SetParent(null, true);
		}
		if (this.follow && this.parent)
		{
			base.transform.position = this.parent.transform.position;
		}
		if (!this.parent)
		{
			base.StartCoroutine(this.DelayRemove());
			this.done = true;
		}
	}

	// Token: 0x060004C7 RID: 1223 RVA: 0x0001BCE3 File Offset: 0x00019EE3
	private IEnumerator DelayRemove()
	{
		yield return new WaitForSeconds(this.destroyDelay);
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x04000657 RID: 1623
	public Transform parent;

	// Token: 0x04000658 RID: 1624
	public bool follow;

	// Token: 0x04000659 RID: 1625
	public float destroyDelay;

	// Token: 0x0400065A RID: 1626
	private bool done;
}
