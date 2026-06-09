using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rounds
{
	// Token: 0x0200031B RID: 795
	[ExecuteInEditMode]
	public class FakeParticle : MonoBehaviour
	{
		// Token: 0x060010EC RID: 4332 RVA: 0x00051944 File Offset: 0x0004FB44
		private void OnEnable()
		{
			this.m_renderer = base.GetComponent<SpriteRenderer>();
			this.m_propertyBlock = new MaterialPropertyBlock();
			if (this.m_material == null)
			{
				this.m_material = FakeParticleDB.GetParticleMaterial();
			}
			if (this.m_renderer == null)
			{
				global::Debug.LogError("No Sprite Renderer on " + base.gameObject.name + " and it's needed for a FakeParticle.", base.gameObject);
				return;
			}
			this.OnChangedValueInit();
		}

		// Token: 0x060010ED RID: 4333 RVA: 0x000519BB File Offset: 0x0004FBBB
		private void Update()
		{
			this.OnChangedValue();
		}

		// Token: 0x060010EE RID: 4334 RVA: 0x000519C3 File Offset: 0x0004FBC3
		private void OnDisable()
		{
			this.m_renderer.material = FakeParticleDB.GetDefaultMaterial();
		}

		// Token: 0x060010EF RID: 4335 RVA: 0x000519D8 File Offset: 0x0004FBD8
		private void OnChangedValue()
		{
			this.m_renderer.material = this.m_material;
			this.m_renderer.GetPropertyBlock(this.m_propertyBlock);
			this.m_propertyBlock.SetFloat("_ScaleFactor", this.m_ScaleFactor);
			this.m_propertyBlock.SetFloat("_Offset", (float)Random.Range(0, 1000));
			this.m_renderer.SetPropertyBlock(this.m_propertyBlock);
		}

		// Token: 0x060010F0 RID: 4336 RVA: 0x00051A4C File Offset: 0x0004FC4C
		private void OnChangedValueInit()
		{
			this.m_renderer.material = this.m_material;
			this.m_renderer.GetPropertyBlock(this.m_propertyBlock);
			this.m_propertyBlock.SetFloat("_ScaleFactor", this.m_ScaleFactor);
			this.m_propertyBlock.SetFloat("_Offset", (float)Random.Range(0, 1000));
			this.m_renderer.SetPropertyBlock(this.m_propertyBlock);
		}

		// Token: 0x04000FD3 RID: 4051
		[OnValueChanged("OnChangedValue", false)]
		public float m_ScaleFactor = 1f;

		// Token: 0x04000FD4 RID: 4052
		private Material m_material;

		// Token: 0x04000FD5 RID: 4053
		private SpriteRenderer m_renderer;

		// Token: 0x04000FD6 RID: 4054
		private MaterialPropertyBlock m_propertyBlock;
	}
}
