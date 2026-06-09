using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000075 RID: 117
public class LegRenderer : MonoBehaviour
{
	// Token: 0x06000268 RID: 616 RVA: 0x0000FAC0 File Offset: 0x0000DCC0
	private void Awake()
	{
		for (int i = 0; i < this.segmentCount; i++)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.segment, base.transform);
			gameObject.SetActive(true);
			this.segments.Add(gameObject.transform);
			if (i == this.segmentCount - 1)
			{
				gameObject.transform.localScale *= 0f;
			}
		}
	}

	// Token: 0x06000269 RID: 617 RVA: 0x0000FB30 File Offset: 0x0000DD30
	private void LateUpdate()
	{
		for (int i = 0; i < this.segments.Count; i++)
		{
			float t = (float)i / ((float)this.segments.Count - 1f);
			Vector3 zero = Vector3.zero;
			if (i == 0)
			{
				this.segments[i + 1].position - this.segments[i].position;
			}
			else if (i == this.segments.Count - 1)
			{
				this.segments[i].position - this.segments[i - 1].position;
			}
			else
			{
				Vector3 a = this.segments[i].position - this.segments[i - 1].position;
				Vector3 b = this.segments[i + 1].position - this.segments[i].position;
				(a + b) * 0.5f;
			}
			this.segments[i].position = BezierCurve.QuadraticBezier(this.start.position, this.mid.position, this.end.position, t);
		}
		for (int j = 0; j < this.segments.Count; j++)
		{
			float num = (float)j / ((float)this.segments.Count - 1f);
			Vector3 upwards = Vector3.zero;
			if (j == 0)
			{
				upwards = this.segments[j + 1].position - this.segments[j].position;
			}
			else if (j == this.segments.Count - 1)
			{
				upwards = this.segments[j].position - this.segments[j - 1].position;
			}
			else
			{
				Vector3 a2 = this.segments[j].position - this.segments[j - 1].position;
				Vector3 b2 = this.segments[j + 1].position - this.segments[j].position;
				upwards = (a2 + b2) * 0.5f;
			}
			this.segments[j].rotation = Quaternion.LookRotation(Vector3.forward, upwards);
		}
	}

	// Token: 0x04000378 RID: 888
	public Transform start;

	// Token: 0x04000379 RID: 889
	public Transform mid;

	// Token: 0x0400037A RID: 890
	public Transform end;

	// Token: 0x0400037B RID: 891
	public int segmentCount = 10;

	// Token: 0x0400037C RID: 892
	public GameObject segment;

	// Token: 0x0400037D RID: 893
	public float segmentLength = 1f;

	// Token: 0x0400037E RID: 894
	private List<Transform> segments = new List<Transform>();
}
