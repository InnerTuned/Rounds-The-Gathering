using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI.ProceduralImage;

// Token: 0x02000126 RID: 294
public class CounterUI : MonoBehaviour
{
	// Token: 0x060005BF RID: 1471 RVA: 0x00020A47 File Offset: 0x0001EC47
	private void ResetStuff()
	{
		this.counter = 0f;
	}

	// Token: 0x060005C0 RID: 1472 RVA: 0x00020A54 File Offset: 0x0001EC54
	private void Update()
	{
		if (this.done)
		{
			return;
		}
		this.outerRing.fillAmount = this.counter;
		this.fill.fillAmount = this.counter;
		this.rotator.transform.localEulerAngles = new Vector3(0f, 0f, -Mathf.Lerp(0f, 360f, this.counter));
		this.counter += TimeHandler.deltaTime / this.timeToFill;
		this.counter = Mathf.Clamp(this.counter, -0.1f / this.timeToFill, 1f);
		if (this.counter >= 1f)
		{
			this.done = true;
			this.doneEvent.Invoke();
		}
		if (this.counter <= 0f)
		{
			this.rotator.gameObject.SetActive(false);
			this.still.gameObject.SetActive(false);
			return;
		}
		this.rotator.gameObject.SetActive(true);
		this.still.gameObject.SetActive(true);
	}

	// Token: 0x0400075E RID: 1886
	[Range(0f, 1f)]
	public float counter;

	// Token: 0x0400075F RID: 1887
	public float timeToFill = 10f;

	// Token: 0x04000760 RID: 1888
	public ProceduralImage outerRing;

	// Token: 0x04000761 RID: 1889
	public ProceduralImage fill;

	// Token: 0x04000762 RID: 1890
	public Transform rotator;

	// Token: 0x04000763 RID: 1891
	public Transform still;

	// Token: 0x04000764 RID: 1892
	private float remainingDuration;

	// Token: 0x04000765 RID: 1893
	private bool isAbyssalForm;

	// Token: 0x04000766 RID: 1894
	private bool done;

	// Token: 0x04000767 RID: 1895
	public UnityEvent doneEvent;
}
