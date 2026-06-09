using System;
using UnityEngine;

// Token: 0x020000BA RID: 186
public class RemoveAfterSeconds : MonoBehaviour
{
	// Token: 0x060003F5 RID: 1013 RVA: 0x00018532 File Offset: 0x00016732
	private void Start()
	{
		this.startSeconds = this.seconds;
		this.startScale = base.transform.localScale;
		this.shrinkSpeed = Random.Range(1.5f, 2.5f);
	}

	// Token: 0x060003F6 RID: 1014 RVA: 0x00018568 File Offset: 0x00016768
	private void Update()
	{
		this.seconds -= TimeHandler.deltaTime * (this.scaleWithScale ? (1f / base.transform.localScale.x) : 1f);
		if (this.seconds < 0f)
		{
			if (this.shrink)
			{
				this.counter += Time.deltaTime * this.shrinkSpeed;
				base.transform.localScale = Vector3.Lerp(this.startScale, Vector3.zero, this.curve.Evaluate(this.counter));
				if (this.counter > 1f)
				{
					Object.Destroy(base.gameObject);
					return;
				}
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
		}
	}

	// Token: 0x060003F7 RID: 1015 RVA: 0x0001862B File Offset: 0x0001682B
	public void ResetCounter()
	{
		this.seconds = this.startSeconds;
	}

	// Token: 0x04000570 RID: 1392
	public float seconds = 3f;

	// Token: 0x04000571 RID: 1393
	private float startSeconds;

	// Token: 0x04000572 RID: 1394
	public bool shrink;

	// Token: 0x04000573 RID: 1395
	public bool scaleWithScale;

	// Token: 0x04000574 RID: 1396
	private float shrinkSpeed = 1f;

	// Token: 0x04000575 RID: 1397
	private Vector3 startScale;

	// Token: 0x04000576 RID: 1398
	private AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	// Token: 0x04000577 RID: 1399
	private float counter;
}
