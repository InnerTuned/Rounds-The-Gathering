using System;
using System.Collections;
using Sonigon;
using Sonigon.Internal;
using UnityEngine;

// Token: 0x02000167 RID: 359
public class MapObjet_Rope : MonoBehaviour
{
	// Token: 0x06000734 RID: 1844 RVA: 0x000270E4 File Offset: 0x000252E4
	private void Start()
	{
		this.map = base.GetComponentInParent<Map>();
		this.map.hasRope = true;
		this.lineRenderer = base.GetComponent<LineRenderer>();
		Map map = this.map;
		map.mapIsReadyAction = (Action)Delegate.Combine(map.mapIsReadyAction, new Action(this.Go));
		Map map2 = this.map;
		map2.mapMovingOutAction = (Action)Delegate.Combine(map2.mapMovingOutAction, new Action(this.Leave));
	}

	// Token: 0x06000735 RID: 1845 RVA: 0x00027163 File Offset: 0x00025363
	private void Leave()
	{
		if (this.joint)
		{
			Object.Destroy(this.joint);
		}
	}

	// Token: 0x06000736 RID: 1846 RVA: 0x0002717D File Offset: 0x0002537D
	public void Go()
	{
		base.StartCoroutine(this.IGo());
	}

	// Token: 0x06000737 RID: 1847 RVA: 0x0002718C File Offset: 0x0002538C
	private IEnumerator IGo()
	{
		yield return new WaitForSeconds(0f);
		Rigidbody2D rigidbody2D = null;
		Rigidbody2D rigidbody2D2 = null;
		for (int i = 0; i < this.map.allRigs.Length; i++)
		{
			Collider2D component = this.map.allRigs[i].GetComponent<Collider2D>();
			if (component)
			{
				if (component.OverlapPoint(base.transform.position))
				{
					rigidbody2D = this.map.allRigs[i];
				}
				if (component.OverlapPoint(base.transform.GetChild(0).position))
				{
					rigidbody2D2 = this.map.allRigs[i];
				}
			}
		}
		if (rigidbody2D)
		{
			this.AddJoint(rigidbody2D);
			if (rigidbody2D2)
			{
				this.joint.connectedBody = rigidbody2D2;
				this.joint.anchor = rigidbody2D.transform.InverseTransformPoint(base.transform.position);
				this.joint.connectedAnchor = rigidbody2D2.transform.InverseTransformPoint(base.transform.GetChild(0).position);
			}
			else
			{
				this.joint.anchor = rigidbody2D.transform.InverseTransformPoint(base.transform.position);
				this.joint.connectedAnchor = base.transform.GetChild(0).position;
			}
			this.joint.enableCollision = true;
		}
		else if (rigidbody2D2)
		{
			this.AddJoint(rigidbody2D2);
			this.joint.anchor = rigidbody2D2.transform.InverseTransformPoint(base.transform.GetChild(0).position);
			this.joint.connectedAnchor = base.transform.position;
			this.joint.enableCollision = true;
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
		yield break;
	}

	// Token: 0x06000738 RID: 1848 RVA: 0x0002719C File Offset: 0x0002539C
	private void AddJoint(Rigidbody2D target)
	{
		MapObjet_Rope.JointType jointType = this.jointType;
		if (jointType == MapObjet_Rope.JointType.spring)
		{
			this.joint = target.gameObject.AddComponent<SpringJoint2D>();
			return;
		}
		if (jointType != MapObjet_Rope.JointType.Distance)
		{
			return;
		}
		this.joint = target.gameObject.AddComponent<DistanceJoint2D>();
	}

	// Token: 0x06000739 RID: 1849 RVA: 0x000271DC File Offset: 0x000253DC
	private void OnDrawGizmos()
	{
		if (this.joint)
		{
			return;
		}
		if (!this.lineRenderer)
		{
			this.lineRenderer = base.GetComponent<LineRenderer>();
		}
		this.lineRenderer.SetPosition(0, base.transform.position);
		this.lineRenderer.SetPosition(1, base.transform.GetChild(0).position);
		if (Event.current.shift)
		{
			base.transform.GetChild(0).position = this.lastPos;
		}
		this.lastPos = base.transform.GetChild(0).position;
	}

	// Token: 0x0600073A RID: 1850 RVA: 0x0002727E File Offset: 0x0002547E
	private void OnDestroy()
	{
		this.soundIsPlaying = false;
		this.soundInitialized = false;
		SoundManager.Instance.StopAtPosition(this.soundRopeLoop, base.transform, true);
	}

	// Token: 0x0600073B RID: 1851 RVA: 0x0002727E File Offset: 0x0002547E
	private void OnDisable()
	{
		this.soundIsPlaying = false;
		this.soundInitialized = false;
		SoundManager.Instance.StopAtPosition(this.soundRopeLoop, base.transform, true);
	}

	// Token: 0x0600073C RID: 1852 RVA: 0x000272A8 File Offset: 0x000254A8
	private void Update()
	{
		if (this.joint)
		{
			if (this.joint.attachedRigidbody && !this.joint.attachedRigidbody.gameObject.activeSelf)
			{
				this.postSnapPos1 = this.lineRenderer.GetPosition(0);
			}
			if (this.joint.connectedBody && !this.joint.connectedBody.gameObject.activeSelf)
			{
				this.postSnapPos2 = this.lineRenderer.GetPosition(1);
			}
			if ((this.joint.attachedRigidbody && !this.joint.attachedRigidbody.gameObject.activeSelf) || (this.joint.connectedBody && !this.joint.connectedBody.gameObject.activeSelf))
			{
				this.sinceBreak += Time.deltaTime;
				if (Vector2.Distance(this.lineRenderer.GetPosition(0), this.lineRenderer.GetPosition(1)) < 0.2f)
				{
					if (this.soundIsPlaying)
					{
						SoundManager.Instance.StopAtPosition(this.soundRopeLoop, base.transform, true);
					}
					this.lineRenderer.enabled = false;
					return;
				}
			}
			if (this.joint.attachedRigidbody && this.joint.attachedRigidbody.gameObject.activeSelf)
			{
				this.vel1 = this.joint.attachedRigidbody.velocity * 1.5f;
			}
			if (this.joint.attachedRigidbody && !this.joint.attachedRigidbody.gameObject.activeSelf)
			{
				this.vel1 = FRILerp.Lerp(this.vel1, (this.lineRenderer.GetPosition(1) - this.lineRenderer.GetPosition(0)) * 15f * Mathf.Clamp(this.sinceBreak * 2f, 0f, 1f), 10f);
				this.postSnapPos1 += this.vel1 * Time.deltaTime;
			}
			if (this.postSnapPos1 == Vector3.zero)
			{
				this.lineRenderer.SetPosition(0, this.joint.attachedRigidbody.transform.TransformPoint(this.joint.anchor));
			}
			else
			{
				this.lineRenderer.SetPosition(0, this.postSnapPos1);
			}
			if (this.joint.connectedBody)
			{
				if (!this.joint.connectedBody.gameObject.activeSelf)
				{
					this.vel2 = FRILerp.Lerp(this.vel2, (this.lineRenderer.GetPosition(0) - this.lineRenderer.GetPosition(1)) * 15f * Mathf.Clamp(this.sinceBreak * 2f, 0f, 1f), 10f);
					this.postSnapPos2 += this.vel2 * Time.deltaTime;
				}
				else
				{
					this.vel2 = this.joint.connectedBody.velocity * 1.5f;
				}
				if (this.postSnapPos2 == Vector3.zero)
				{
					this.lineRenderer.SetPosition(1, this.joint.connectedBody.transform.TransformPoint(this.joint.connectedAnchor));
				}
				else
				{
					this.lineRenderer.SetPosition(1, this.postSnapPos2);
				}
			}
			else if (this.postSnapPos2 == Vector3.zero)
			{
				this.lineRenderer.SetPosition(1, this.joint.connectedAnchor);
			}
			else
			{
				this.lineRenderer.SetPosition(1, this.postSnapPos2);
			}
		}
		else
		{
			this.lineRenderer.SetPosition(0, Vector3.up * 200f);
			this.lineRenderer.SetPosition(1, Vector3.up * 200f);
		}
		if (this.soundRopePlay && this.lineRenderer.positionCount >= 1)
		{
			if (!this.soundInitialized)
			{
				this.soundInitialized = true;
				this.soundRopeLengthLast = Vector3.Distance(this.lineRenderer.GetPosition(0), this.lineRenderer.GetPosition(1));
			}
			this.soundRopeLengthCurrent = Vector3.Distance(this.lineRenderer.GetPosition(0), this.lineRenderer.GetPosition(1));
			this.soundRopeLengthVelocity = Mathf.Abs(this.soundRopeLengthLast - this.soundRopeLengthCurrent);
			this.soundParameterIntensity.intensity = this.soundRopeLengthVelocity;
			if (this.soundRopeLengthVelocity > 0.03f)
			{
				if (!this.soundIsPlaying)
				{
					this.soundIsPlaying = true;
					SoundManager.Instance.PlayAtPosition(this.soundRopeLoop, SoundManager.Instance.transform, base.transform, new SoundParameterBase[]
					{
						this.soundParameterIntensity
					});
				}
			}
			else if (this.soundIsPlaying)
			{
				this.soundIsPlaying = false;
				this.soundInitialized = false;
				SoundManager.Instance.StopAtPosition(this.soundRopeLoop, base.transform, true);
			}
			this.soundRopeLengthLast = Vector3.Distance(this.lineRenderer.GetPosition(0), this.lineRenderer.GetPosition(1));
		}
	}

	// Token: 0x040008A1 RID: 2209
	[Header("Sound")]
	public bool soundRopePlay;

	// Token: 0x040008A2 RID: 2210
	public SoundEvent soundRopeLoop;

	// Token: 0x040008A3 RID: 2211
	private SoundParameterIntensity soundParameterIntensity = new SoundParameterIntensity(0f, 0);

	// Token: 0x040008A4 RID: 2212
	private bool soundIsPlaying;

	// Token: 0x040008A5 RID: 2213
	private bool soundInitialized;

	// Token: 0x040008A6 RID: 2214
	private float soundRopeLengthCurrent;

	// Token: 0x040008A7 RID: 2215
	private float soundRopeLengthLast;

	// Token: 0x040008A8 RID: 2216
	private float soundRopeLengthVelocity;

	// Token: 0x040008A9 RID: 2217
	[Header("Settings")]
	public MapObjet_Rope.JointType jointType;

	// Token: 0x040008AA RID: 2218
	private Map map;

	// Token: 0x040008AB RID: 2219
	private LineRenderer lineRenderer;

	// Token: 0x040008AC RID: 2220
	private AnchoredJoint2D joint;

	// Token: 0x040008AD RID: 2221
	private Vector3 lastPos;

	// Token: 0x040008AE RID: 2222
	private Vector3 vel1;

	// Token: 0x040008AF RID: 2223
	private Vector3 vel2;

	// Token: 0x040008B0 RID: 2224
	private Vector3 postSnapPos1;

	// Token: 0x040008B1 RID: 2225
	private Vector3 postSnapPos2;

	// Token: 0x040008B2 RID: 2226
	private float sinceBreak;

	// Token: 0x02000390 RID: 912
	public enum JointType
	{
		// Token: 0x0400120E RID: 4622
		spring,
		// Token: 0x0400120F RID: 4623
		Distance
	}
}
