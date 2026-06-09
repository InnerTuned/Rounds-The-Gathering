using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000157 RID: 343
public class HoverEvent_WobbleButton : MonoBehaviour
{
	// Token: 0x060006EF RID: 1775 RVA: 0x00026488 File Offset: 0x00024688
	private void Awake()
	{
		HoverEvent hoverEvent = base.gameObject.AddComponent<HoverEvent>();
		this.sh = base.gameObject.AddComponent<ScaleShake>();
		Button component = base.GetComponent<Button>();
		this.sh.spring = this.spring;
		this.sh.drag = this.drag;
		this.sh.multiplier = this.force;
		this.sh.high = this.max;
		this.sh.low = this.min;
		this.sh.useTimeScale = false;
		this.sh.SetTarget(this.min);
		UnityEvent unityEvent = new UnityEvent();
		unityEvent.AddListener(new UnityAction(this.sh.SetHigh));
		hoverEvent.enterEvent = unityEvent;
		UnityEvent unityEvent2 = new UnityEvent();
		unityEvent2.AddListener(new UnityAction(this.sh.SetLow));
		hoverEvent.exitEvent = unityEvent2;
		component.onClick.AddListener(new UnityAction(this.sh.AddForce));
	}

	// Token: 0x060006F0 RID: 1776 RVA: 0x0002658D File Offset: 0x0002478D
	private void OnDisable()
	{
		base.transform.localScale = Vector3.one * this.min;
		this.sh.SetTarget(this.min);
	}

	// Token: 0x0400085A RID: 2138
	public float min = 1f;

	// Token: 0x0400085B RID: 2139
	public float max = 1.1f;

	// Token: 0x0400085C RID: 2140
	public float spring = 1f;

	// Token: 0x0400085D RID: 2141
	public float drag = 1f;

	// Token: 0x0400085E RID: 2142
	public float force = -0.1f;

	// Token: 0x0400085F RID: 2143
	private ScaleShake sh;
}
