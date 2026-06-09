using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000052 RID: 82
public class EventSequence : MonoBehaviour
{
	// Token: 0x06000185 RID: 389 RVA: 0x00009EA2 File Offset: 0x000080A2
	private void Start()
	{
		this.level = base.GetComponentInParent<AttackLevel>().attackLevel;
		if (this.playOnAwake)
		{
			this.Go();
		}
	}

	// Token: 0x06000186 RID: 390 RVA: 0x00009EC3 File Offset: 0x000080C3
	public void Go()
	{
		base.StartCoroutine(this.DoSequence());
	}

	// Token: 0x06000187 RID: 391 RVA: 0x00009ED2 File Offset: 0x000080D2
	private IEnumerator DoSequence()
	{
		int num2;
		for (int i = 0; i < this.events.Length; i = num2 + 1)
		{
			float num = 0f;
			for (int i2 = 0; i2 < this.events[i].cycles + this.events[i].cyclesPerLvl * this.level; i2 = num2 + 1)
			{
				num += this.events[i].delay;
				if (num > TimeHandler.deltaTime)
				{
					if (this.useTimeScale)
					{
						yield return new WaitForSeconds(num);
					}
					else
					{
						yield return new WaitForSecondsRealtime(num);
					}
					num = 0f;
				}
				this.events[i].eventTrigger.Invoke();
				num2 = i2;
			}
			num2 = i;
		}
		yield break;
	}

	// Token: 0x04000206 RID: 518
	public bool playOnAwake = true;

	// Token: 0x04000207 RID: 519
	public bool useTimeScale = true;

	// Token: 0x04000208 RID: 520
	public DelayedEvent[] events;

	// Token: 0x04000209 RID: 521
	private int level;
}
