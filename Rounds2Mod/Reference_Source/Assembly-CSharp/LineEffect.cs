using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

// Token: 0x0200007A RID: 122
public class LineEffect : MonoBehaviour
{
	// Token: 0x0600027B RID: 635 RVA: 0x000102A4 File Offset: 0x0000E4A4
	private void Start()
	{
		this.globalTimeSpeed *= Random.Range(0.95f, 1.05f);
	}

	// Token: 0x0600027C RID: 636 RVA: 0x000102C2 File Offset: 0x0000E4C2
	private void OnEnable()
	{
		if (!this.inited)
		{
			this.Init();
		}
		if (this.playOnAwake && this.lineType == LineEffect.LineType.Ring)
		{
			this.Play(base.transform);
		}
	}

	// Token: 0x0600027D RID: 637 RVA: 0x000102F0 File Offset: 0x0000E4F0
	private void Init()
	{
		if (this.inited)
		{
			return;
		}
		this.line = base.GetComponent<LineRenderer>();
		this.line.positionCount = this.segments;
		this.line.useWorldSpace = true;
		this.startWidth = this.line.widthMultiplier;
		this.inited = true;
		this.scaleMultiplier = 1f - this.inheritScaleFactor + base.transform.lossyScale.x * this.inheritScaleFactor;
	}

	// Token: 0x0600027E RID: 638 RVA: 0x00010374 File Offset: 0x0000E574
	private void Update()
	{
		if (this.debug)
		{
			if (this.lineType == LineEffect.LineType.Line && this.fromPos && this.toPos)
			{
				if (this.bezierPos)
				{
					this.DrawLine(this.fromPos.position, this.toPos.position, this.bezierPos.position);
				}
				else
				{
					this.DrawLine(this.fromPos.position, this.toPos.position);
				}
			}
			if (this.lineType == LineEffect.LineType.Ring && this.fromPos)
			{
				this.DrawLine(this.fromPos.position, Vector3.zero);
			}
		}
	}

	// Token: 0x0600027F RID: 639 RVA: 0x0001042C File Offset: 0x0000E62C
	public void StartDraw()
	{
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(true);
		}
		this.counter = 0f;
	}

	// Token: 0x06000280 RID: 640 RVA: 0x00010452 File Offset: 0x0000E652
	public void DrawLine(Vector3 start, Vector3 end)
	{
		this.DrawLine(start, end, Vector3.one * 100f);
	}

	// Token: 0x06000281 RID: 641 RVA: 0x0001046C File Offset: 0x0000E66C
	public void DrawLine(Vector3 start, Vector3 end, Vector3 bezier)
	{
		this.Init();
		Vector3 vector = Vector3.zero;
		for (int i = 0; i < this.line.positionCount; i++)
		{
			float num = 0f;
			for (int j = 0; j < this.effects.Length; j++)
			{
				if (this.effects[j].active)
				{
					float num2 = (float)i / ((float)this.line.positionCount - 1f);
					float time = num2;
					float num3 = this.effects[j].mainCurveTiling;
					if (this.effects[j].tilingPerMeter)
					{
						if (this.lineType == LineEffect.LineType.Line)
						{
							num3 *= (start - end).magnitude;
						}
						if (this.lineType == LineEffect.LineType.Ring)
						{
							num3 *= this.radius;
						}
					}
					num2 *= num3;
					if (this.effects[j].mainCurveScrollSpeed > 0f)
					{
						num2 += this.effects[j].mainCurveScrollSpeed * Time.unscaledTime;
					}
					if (this.effects[j].mainCurveScrollSpeed < 0f)
					{
						num2 += -this.effects[j].mainCurveScrollSpeed * (100000f - Time.unscaledTime);
					}
					num2 %= 1f;
					float num4 = this.effects[j].mainCurve.Evaluate(num2);
					num4 *= this.effects[j].mainCurveMultiplier;
					num4 *= this.effects[j].effectOverLineCurve.Evaluate(time);
					num4 *= this.effects[j].effectOverTimeCurve.Evaluate(this.counter);
					if (this.effects[j].curveType == LineEffectInstance.CurveType.Add)
					{
						num += num4;
					}
					else if (this.effects[j].curveType == LineEffectInstance.CurveType.Multiply)
					{
						num *= num4;
					}
				}
			}
			float t = (float)i / ((float)this.line.positionCount - 1f);
			Vector3 vector2 = Vector3.zero;
			Vector3 a = Vector3.zero;
			if (this.lineType == LineEffect.LineType.Ring)
			{
				float f = 0.017453292f * ((float)i * 360f / (float)(this.segments - 1));
				vector2 = start + new Vector3(Mathf.Sin(f) * (this.radius * base.transform.root.localScale.x * this.radiusOverTime.Evaluate(this.counter)), Mathf.Cos(f) * (this.radius * base.transform.root.localScale.x * this.radiusOverTime.Evaluate(this.counter)), 0f);
				if (vector != Vector3.zero)
				{
					a = Vector3.Cross(Vector3.forward, vector2 - vector).normalized;
				}
				else
				{
					a = Vector3.up;
				}
			}
			if (this.lineType == LineEffect.LineType.Line)
			{
				vector2 = Vector3.Lerp(start, end, t);
				a = Vector3.Cross(start - end, Vector3.forward).normalized;
				if (bezier != Vector3.one * 100f)
				{
					vector2 = BezierCurve.QuadraticBezier(start, bezier, end, t);
				}
			}
			vector = vector2;
			this.line.SetPosition(i, vector2 + a * num * this.offsetMultiplier);
		}
		if (this.raycastCollision)
		{
			this.RaycastPositions();
		}
		if (this.lineType == LineEffect.LineType.Ring)
		{
			this.SmoothSeam();
		}
		this.line.widthMultiplier = this.lineWidthOverTimeCurve.Evaluate(this.counter) * this.startWidth * this.scaleMultiplier * this.widthMultiplier;
		this.currentWidth = this.line.widthMultiplier;
		if (this.useColorOverTime)
		{
			this.line.startColor = this.colorOverTime.Evaluate(this.counter);
			this.line.endColor = this.colorOverTime.Evaluate(this.counter);
		}
		this.counter += Time.unscaledDeltaTime * this.globalTimeSpeed;
		if (this.counter > 1f)
		{
			if (this.debug || this.loop)
			{
				this.StartDraw();
				return;
			}
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000282 RID: 642 RVA: 0x000108AC File Offset: 0x0000EAAC
	private void RaycastPositions()
	{
		for (int i = 0; i < this.line.positionCount; i++)
		{
			this.line.SetPosition(i, PhysicsFunctions.ObstructionPoint(base.transform.position, this.line.GetPosition(i)));
		}
	}

	// Token: 0x06000283 RID: 643 RVA: 0x00010908 File Offset: 0x0000EB08
	private void SmoothSeam()
	{
		float num = 0.1f;
		Vector3 vector = (this.line.GetPosition(0) + this.line.GetPosition(this.line.positionCount - 1)) * 0.5f;
		for (int i = 0; i < this.line.positionCount; i++)
		{
			float num2 = (float)i / ((float)this.line.positionCount - 1f);
			Vector3 zero = Vector3.zero;
			Vector3 b = Vector3.zero;
			Vector3 position = this.line.GetPosition(i);
			float num3;
			if (num2 > 0.5f)
			{
				num3 = Mathf.Clamp((num2 - (1f - num)) / num, 0f, 1f);
			}
			else
			{
				num3 = Mathf.Clamp(num2 * (1f / num), 0f, 1f);
				num3 = 1f - num3;
			}
			b = Vector3.Lerp(position, vector, num3) - position;
			b.x *= 0.5f;
			this.line.SetPosition(i, position + b);
		}
		this.line.SetPosition(1, new Vector3(this.line.GetPosition(1).x, vector.y));
		this.line.SetPosition(this.line.positionCount - 2, new Vector3(this.line.GetPosition(this.line.positionCount - 2).x, vector.y));
	}

	// Token: 0x06000284 RID: 644 RVA: 0x00010A86 File Offset: 0x0000EC86
	public void Stop()
	{
		base.StopAllCoroutines();
		if (!this.loop)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000285 RID: 645 RVA: 0x00010AA2 File Offset: 0x0000ECA2
	public void Play()
	{
		this.Play(base.transform);
	}

	// Token: 0x06000286 RID: 646 RVA: 0x00010AB0 File Offset: 0x0000ECB0
	private void OnDisable()
	{
		this.Stop();
	}

	// Token: 0x06000287 RID: 647 RVA: 0x00010AB8 File Offset: 0x0000ECB8
	public void ResetMultipliers()
	{
		this.offsetMultiplier = 1f;
		this.scaleMultiplier = 1f;
		this.widthMultiplier = 1f;
	}

	// Token: 0x06000288 RID: 648 RVA: 0x00010ADB File Offset: 0x0000ECDB
	public void Play(Transform fromTransform, Transform toTransform, float bezierOffset = 0f)
	{
		this.StartDraw();
		base.StopAllCoroutines();
		base.StartCoroutine(this.DoPlay(fromTransform, toTransform, bezierOffset));
	}

	// Token: 0x06000289 RID: 649 RVA: 0x00010AF9 File Offset: 0x0000ECF9
	private IEnumerator DoPlay(Transform fromTransform, Transform toTransform, float bezierOffset = 0f)
	{
		this.isPlaying = true;
		Vector3 currentBez = Vector3.zero;
		while (base.gameObject.activeSelf && fromTransform && toTransform)
		{
			if (bezierOffset != 0f)
			{
				float d = 1f;
				if (toTransform.position.x < fromTransform.position.x)
				{
					d = -1f;
				}
				Vector3 vector = Vector3.Cross(Vector3.forward, toTransform.position - fromTransform.position).normalized * bezierOffset * d;
				if (currentBez == Vector3.zero)
				{
					currentBez = vector;
				}
				else
				{
					currentBez = Vector3.Lerp(currentBez, vector, TimeHandler.deltaTime * 5f);
				}
				Vector3 bezier = currentBez + (fromTransform.position + toTransform.position) * 0.5f;
				this.DrawLine(fromTransform.position, toTransform.position, bezier);
			}
			else
			{
				this.DrawLine(fromTransform.position, toTransform.position);
			}
			yield return null;
		}
		this.isPlaying = false;
		yield break;
	}

	// Token: 0x0600028A RID: 650 RVA: 0x00010B1D File Offset: 0x0000ED1D
	public void Play(Transform fromTransform, Vector3 toPosition, float bezierOffset = 0f)
	{
		this.StartDraw();
		base.StopAllCoroutines();
		base.StartCoroutine(this.DoPlay(fromTransform, toPosition, bezierOffset));
	}

	// Token: 0x0600028B RID: 651 RVA: 0x00010B3B File Offset: 0x0000ED3B
	private IEnumerator DoPlay(Transform fromTransform, Vector3 toPosition, float bezierOffset = 0f)
	{
		Vector3 currentBezOffset = Vector3.zero;
		while (base.gameObject.activeSelf && fromTransform)
		{
			if (bezierOffset != 0f)
			{
				float d = 1f;
				if (toPosition.x < fromTransform.position.x)
				{
					d = -1f;
				}
				Vector3 vector = Vector3.Cross(Vector3.forward, toPosition - fromTransform.position).normalized * bezierOffset * d;
				if (currentBezOffset == Vector3.zero)
				{
					currentBezOffset = vector;
				}
				else
				{
					currentBezOffset = Vector3.Lerp(currentBezOffset, vector, TimeHandler.deltaTime * 2f);
				}
				Vector3 bezier = currentBezOffset + (fromTransform.position + toPosition) * 0.5f;
				bezier = (fromTransform.position + toPosition) * 0.5f;
				this.DrawLine(fromTransform.position, toPosition, bezier);
			}
			else
			{
				this.DrawLine(fromTransform.position, toPosition);
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x0600028C RID: 652 RVA: 0x00010B5F File Offset: 0x0000ED5F
	public void Play(Transform fromTransform)
	{
		this.StartDraw();
		base.StopAllCoroutines();
		base.StartCoroutine(this.DoPlay(fromTransform));
	}

	// Token: 0x0600028D RID: 653 RVA: 0x00010B7B File Offset: 0x0000ED7B
	private IEnumerator DoPlay(Transform fromTransform)
	{
		while (base.gameObject.activeSelf && fromTransform)
		{
			this.DrawLine(fromTransform.position, Vector3.zero);
			yield return null;
		}
		yield break;
	}

	// Token: 0x0600028E RID: 654 RVA: 0x00010B94 File Offset: 0x0000ED94
	public void PlayAnim(LineEffect.AnimType animType, AnimationCurve curve, float speed = 1f)
	{
		if (animType == LineEffect.AnimType.Offset && this.offsetAnim != null)
		{
			base.StopCoroutine(this.offsetAnim);
		}
		if (animType == LineEffect.AnimType.Width && this.widthAnim != null)
		{
			base.StopCoroutine(this.widthAnim);
		}
		Coroutine coroutine = base.StartCoroutine(this.DoPlayAnim(animType, curve, speed));
		if (animType == LineEffect.AnimType.Offset)
		{
			this.offsetAnim = coroutine;
		}
		if (animType == LineEffect.AnimType.Width)
		{
			this.widthAnim = coroutine;
		}
	}

	// Token: 0x0600028F RID: 655 RVA: 0x00010BF5 File Offset: 0x0000EDF5
	private IEnumerator DoPlayAnim(LineEffect.AnimType animType, AnimationCurve curve, float speed = 1f)
	{
		float c = 0f;
		float t = curve.keys[curve.keys.Length - 1].time;
		while (c < t)
		{
			if (animType == LineEffect.AnimType.Offset)
			{
				this.offsetMultiplier = curve.Evaluate(c);
			}
			if (animType == LineEffect.AnimType.Width)
			{
				this.widthMultiplier = curve.Evaluate(c);
			}
			c += TimeHandler.deltaTime * speed;
			yield return null;
		}
		yield break;
	}

	// Token: 0x06000290 RID: 656 RVA: 0x00010C19 File Offset: 0x0000EE19
	internal float GetRadius()
	{
		return this.radius * base.transform.root.localScale.x * this.radiusOverTime.Evaluate(this.counter);
	}

	// Token: 0x04000397 RID: 919
	public bool playOnAwake;

	// Token: 0x04000398 RID: 920
	public bool loop;

	// Token: 0x04000399 RID: 921
	public LineEffect.LineType lineType;

	// Token: 0x0400039A RID: 922
	public int segments = 20;

	// Token: 0x0400039B RID: 923
	public float globalTimeSpeed = 1f;

	// Token: 0x0400039C RID: 924
	public bool raycastCollision;

	// Token: 0x0400039D RID: 925
	[FoldoutGroup("Animation", 0)]
	public AnimationCurve lineWidthOverTimeCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

	// Token: 0x0400039E RID: 926
	[Space(10f)]
	[FoldoutGroup("Animation", 0)]
	public bool useColorOverTime;

	// Token: 0x0400039F RID: 927
	[FoldoutGroup("Animation", 0)]
	[ShowIf("useColorOverTime", true)]
	public Gradient colorOverTime;

	// Token: 0x040003A0 RID: 928
	[Space(10f)]
	[FoldoutGroup("Animation", 0)]
	[ShowIf("lineType", LineEffect.LineType.Ring, true)]
	public float radius = 5f;

	// Token: 0x040003A1 RID: 929
	[FoldoutGroup("Animation", 0)]
	[ShowIf("lineType", LineEffect.LineType.Ring, true)]
	public AnimationCurve radiusOverTime = AnimationCurve.Linear(0f, 1f, 1f, 1f);

	// Token: 0x040003A2 RID: 930
	[Space(10f)]
	[FoldoutGroup("Special", 0)]
	public float inheritScaleFactor;

	// Token: 0x040003A3 RID: 931
	[Space(10f)]
	public LineEffectInstance[] effects;

	// Token: 0x040003A4 RID: 932
	private LineRenderer line;

	// Token: 0x040003A5 RID: 933
	[HideInInspector]
	public float counter;

	// Token: 0x040003A6 RID: 934
	private float startWidth;

	// Token: 0x040003A7 RID: 935
	[HideInInspector]
	public float currentWidth;

	// Token: 0x040003A8 RID: 936
	[FoldoutGroup("Debug", 0)]
	public bool debug;

	// Token: 0x040003A9 RID: 937
	[FoldoutGroup("Debug", 0)]
	public Transform fromPos;

	// Token: 0x040003AA RID: 938
	[FoldoutGroup("Debug", 0)]
	public Transform toPos;

	// Token: 0x040003AB RID: 939
	[FoldoutGroup("Debug", 0)]
	public Transform bezierPos;

	// Token: 0x040003AC RID: 940
	[HideInInspector]
	public float offsetMultiplier = 1f;

	// Token: 0x040003AD RID: 941
	[HideInInspector]
	public float widthMultiplier = 1f;

	// Token: 0x040003AE RID: 942
	private float scaleMultiplier = 1f;

	// Token: 0x040003AF RID: 943
	private bool inited;

	// Token: 0x040003B0 RID: 944
	public bool isPlaying;

	// Token: 0x040003B1 RID: 945
	private Coroutine widthAnim;

	// Token: 0x040003B2 RID: 946
	private Coroutine offsetAnim;

	// Token: 0x0200035A RID: 858
	public enum LineType
	{
		// Token: 0x0400111C RID: 4380
		Line,
		// Token: 0x0400111D RID: 4381
		Ring
	}

	// Token: 0x0200035B RID: 859
	public enum AnimType
	{
		// Token: 0x0400111F RID: 4383
		Width,
		// Token: 0x04001120 RID: 4384
		Offset
	}
}
