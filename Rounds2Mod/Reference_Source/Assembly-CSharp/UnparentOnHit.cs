using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000ED RID: 237
public class UnparentOnHit : MonoBehaviour
{
	// Token: 0x060004C9 RID: 1225 RVA: 0x0001BCF4 File Offset: 0x00019EF4
	private void Start()
	{
		ProjectileHit componentInParent = base.GetComponentInParent<ProjectileHit>();
		if (componentInParent)
		{
			componentInParent.AddHitAction(new Action(this.Unparent));
		}
	}

	// Token: 0x060004CA RID: 1226 RVA: 0x0001BD22 File Offset: 0x00019F22
	public void Unparent()
	{
		if (this.done)
		{
			return;
		}
		this.done = true;
		base.transform.SetParent(null, true);
		base.StartCoroutine(this.DelayDestroy());
		this.unparentEvent.Invoke();
	}

	// Token: 0x060004CB RID: 1227 RVA: 0x0001BD59 File Offset: 0x00019F59
	private IEnumerator DelayDestroy()
	{
		yield return new WaitForSeconds(this.destroyAfterSeconds);
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x0400065B RID: 1627
	public float destroyAfterSeconds = 2f;

	// Token: 0x0400065C RID: 1628
	public UnityEvent unparentEvent;

	// Token: 0x0400065D RID: 1629
	private bool done;
}
