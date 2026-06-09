using System;
using UnityEngine;

// Token: 0x020001A8 RID: 424
public class RingHandler : MonoBehaviour
{
	// Token: 0x0600087D RID: 2173 RVA: 0x0002D494 File Offset: 0x0002B694
	private void Awake()
	{
		RingHandler.instance = this;
	}

	// Token: 0x0600087E RID: 2174 RVA: 0x0002D49C File Offset: 0x0002B69C
	private void Start()
	{
		this.code = base.GetComponent<CodeAnimation>();
	}

	// Token: 0x0600087F RID: 2175 RVA: 0x0002D4AC File Offset: 0x0002B6AC
	private void Update()
	{
		if (GameManager.instance.isPlaying)
		{
			if (this.code.currentState != CodeAnimationInstance.AnimationUse.In && this.counter > this.timeBeforeRing && !this.code.isPlaying)
			{
				this.code.PlayIn();
			}
			this.counter += TimeHandler.deltaTime;
			return;
		}
		if (this.code.currentState != CodeAnimationInstance.AnimationUse.Out && !this.code.isPlaying)
		{
			this.code.PlayOut();
		}
		this.counter = 0f;
	}

	// Token: 0x06000880 RID: 2176 RVA: 0x0002D53C File Offset: 0x0002B73C
	public Vector2 ClosestPoint(Vector2 pos)
	{
		return pos.normalized * this.Radius();
	}

	// Token: 0x06000881 RID: 2177 RVA: 0x0002D550 File Offset: 0x0002B750
	public float Radius()
	{
		return base.transform.localScale.x * 95f;
	}

	// Token: 0x040009BF RID: 2495
	public float timeBeforeRing = 30f;

	// Token: 0x040009C0 RID: 2496
	public static RingHandler instance;

	// Token: 0x040009C1 RID: 2497
	private CodeAnimation code;

	// Token: 0x040009C2 RID: 2498
	private float counter;
}
