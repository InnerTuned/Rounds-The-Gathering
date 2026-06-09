using System;
using AmplifyColor;
using UnityEngine;

// Token: 0x02000145 RID: 325
[ExecuteInEditMode]
[AddComponentMenu("")]
public class AmplifyColorVolumeBase : MonoBehaviour
{
	// Token: 0x06000637 RID: 1591 RVA: 0x00023300 File Offset: 0x00021500
	private void OnDrawGizmos()
	{
		if (this.ShowInSceneView)
		{
			BoxCollider component = base.GetComponent<BoxCollider>();
			BoxCollider2D component2 = base.GetComponent<BoxCollider2D>();
			if (component != null || component2 != null)
			{
				Vector3 center;
				Vector3 size;
				if (component != null)
				{
					center = component.center;
					size = component.size;
				}
				else
				{
					center = component2.offset;
					size = component2.size;
				}
				Gizmos.color = Color.green;
				Gizmos.matrix = base.transform.localToWorldMatrix;
				Gizmos.DrawWireCube(center, size);
			}
		}
	}

	// Token: 0x06000638 RID: 1592 RVA: 0x00023388 File Offset: 0x00021588
	private void OnDrawGizmosSelected()
	{
		BoxCollider component = base.GetComponent<BoxCollider>();
		BoxCollider2D component2 = base.GetComponent<BoxCollider2D>();
		if (component != null || component2 != null)
		{
			Color green = Color.green;
			green.a = 0.2f;
			Gizmos.color = green;
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Vector3 center;
			Vector3 size;
			if (component != null)
			{
				center = component.center;
				size = component.size;
			}
			else
			{
				center = component2.offset;
				size = component2.size;
			}
			Gizmos.DrawCube(center, size);
		}
	}

	// Token: 0x040007D7 RID: 2007
	public Texture2D LutTexture;

	// Token: 0x040007D8 RID: 2008
	public float Exposure = 1f;

	// Token: 0x040007D9 RID: 2009
	public float EnterBlendTime = 1f;

	// Token: 0x040007DA RID: 2010
	public int Priority;

	// Token: 0x040007DB RID: 2011
	public bool ShowInSceneView = true;

	// Token: 0x040007DC RID: 2012
	[HideInInspector]
	public VolumeEffectContainer EffectContainer = new VolumeEffectContainer();
}
