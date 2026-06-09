using System;
using UnityEngine;

// Token: 0x02000149 RID: 329
[RequireComponent(typeof(Renderer))]
public class SFSample : MonoBehaviour
{
	// Token: 0x17000037 RID: 55
	// (get) Token: 0x060006AA RID: 1706 RVA: 0x000253D6 File Offset: 0x000235D6
	// (set) Token: 0x060006AB RID: 1707 RVA: 0x000253DE File Offset: 0x000235DE
	public Vector2 samplePosition
	{
		get
		{
			return this._samplePosition;
		}
		set
		{
			this._samplePosition = value;
			if (this._material)
			{
				this._material.SetVector("_SamplePosition", this._samplePosition);
			}
		}
	}

	// Token: 0x17000038 RID: 56
	// (get) Token: 0x060006AC RID: 1708 RVA: 0x0002540F File Offset: 0x0002360F
	// (set) Token: 0x060006AD RID: 1709 RVA: 0x00025418 File Offset: 0x00023618
	public bool lineSample
	{
		get
		{
			return this._lineSample;
		}
		set
		{
			this._lineSample = value;
			if (this._material)
			{
				if (value)
				{
					this._material.EnableKeyword("LINESAMPLE_ON");
					this._material.DisableKeyword("FIXEDSAMPLEPOINT_ON");
					return;
				}
				this._material.DisableKeyword("LINESAMPLE_ON");
				this._material.EnableKeyword("FIXEDSAMPLEPOINT_ON");
			}
		}
	}

	// Token: 0x060006AE RID: 1710 RVA: 0x00025480 File Offset: 0x00023680
	private void Start()
	{
		Renderer component = base.GetComponent<Renderer>();
		Material sharedMaterial = component.sharedMaterial;
		if (sharedMaterial == null || sharedMaterial.shader.name != "Sprites/SFSoftShadow")
		{
			global::Debug.LogError("SFSample requires the attached renderer to be using the Sprites/SFSoftShadow shader.");
			return;
		}
		this._material = new Material(sharedMaterial);
		component.material = this._material;
		this._material.SetFloat("_SoftHardMix", sharedMaterial.GetFloat("_SoftHardMix"));
		this.samplePosition = this._samplePosition;
		this.lineSample = this._lineSample;
	}

	// Token: 0x060006AF RID: 1711 RVA: 0x00025511 File Offset: 0x00023711
	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawIcon(base.transform.TransformPoint(this._samplePosition), "SFDotGizmo.psd");
	}

	// Token: 0x04000810 RID: 2064
	private Material _material;

	// Token: 0x04000811 RID: 2065
	public Vector2 _samplePosition = Vector2.zero;

	// Token: 0x04000812 RID: 2066
	public bool _lineSample;
}
