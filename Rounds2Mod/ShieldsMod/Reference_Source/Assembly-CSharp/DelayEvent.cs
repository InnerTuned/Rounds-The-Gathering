using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000045 RID: 69
public class DelayEvent : MonoBehaviour
{
	// Token: 0x06000153 RID: 339 RVA: 0x00008CCC File Offset: 0x00006ECC
	private void Start()
	{
		CodeAnimation componentInParent = base.GetComponentInParent<CodeAnimation>();
		if (componentInParent)
		{
			this.time /= componentInParent.animations[0].animationSpeed;
		}
		if (this.auto)
		{
			this.Go();
		}
	}

	// Token: 0x06000154 RID: 340 RVA: 0x00008D10 File Offset: 0x00006F10
	public void Go()
	{
		base.StartCoroutine(this.DelayEventCall());
	}

	// Token: 0x06000155 RID: 341 RVA: 0x00008D1F File Offset: 0x00006F1F
	private IEnumerator DelayEventCall()
	{
		yield return 1;
		if (this.usedTimeScale)
		{
			yield return new WaitForSeconds(this.time);
		}
		else
		{
			yield return new WaitForSecondsRealtime(this.time);
		}
		if (base.enabled)
		{
			this.delayedEvent.Invoke();
			if (this.repeating)
			{
				this.Go();
			}
		}
		yield break;
	}

	// Token: 0x06000156 RID: 342 RVA: 0x00008D2E File Offset: 0x00006F2E
	public void DoEvent()
	{
		base.StopAllCoroutines();
		this.delayedEvent.Invoke();
	}

	// Token: 0x040001C4 RID: 452
	public UnityEvent delayedEvent;

	// Token: 0x040001C5 RID: 453
	public float time = 1f;

	// Token: 0x040001C6 RID: 454
	public bool auto;

	// Token: 0x040001C7 RID: 455
	public bool repeating;

	// Token: 0x040001C8 RID: 456
	public bool usedTimeScale = true;
}
