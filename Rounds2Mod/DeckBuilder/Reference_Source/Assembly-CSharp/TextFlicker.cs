using System;
using TMPro;
using UnityEngine;

// Token: 0x020001BC RID: 444
public class TextFlicker : MonoBehaviour
{
	// Token: 0x060008CC RID: 2252 RVA: 0x0002E566 File Offset: 0x0002C766
	private void Start()
	{
		this.text = base.GetComponent<TextMeshProUGUI>();
	}

	// Token: 0x060008CD RID: 2253 RVA: 0x0002E574 File Offset: 0x0002C774
	private void Update()
	{
		this.counter += Time.deltaTime;
		if (this.counter > this.rate)
		{
			this.counter = 0f;
			this.currentID++;
			if (this.currentID >= this.strings.Length)
			{
				this.currentID = 0;
			}
			this.text.text = this.strings[this.currentID];
		}
	}

	// Token: 0x04000A10 RID: 2576
	public string[] strings;

	// Token: 0x04000A11 RID: 2577
	public float rate = 0.2f;

	// Token: 0x04000A12 RID: 2578
	private float counter;

	// Token: 0x04000A13 RID: 2579
	private int currentID;

	// Token: 0x04000A14 RID: 2580
	private TextMeshProUGUI text;
}
