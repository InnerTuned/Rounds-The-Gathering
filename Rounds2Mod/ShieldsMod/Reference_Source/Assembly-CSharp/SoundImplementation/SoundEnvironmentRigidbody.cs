using System;
using Sonigon;
using Sonigon.Internal;
using UnityEngine;

namespace SoundImplementation
{
	// Token: 0x020001D5 RID: 469
	public class SoundEnvironmentRigidbody : MonoBehaviour
	{
		// Token: 0x06000942 RID: 2370 RVA: 0x0002F91B File Offset: 0x0002DB1B
		private void Awake()
		{
			this.cachedRigidbody2D = base.GetComponent<Rigidbody2D>();
		}

		// Token: 0x06000943 RID: 2371 RVA: 0x0002F92C File Offset: 0x0002DB2C
		private void FixedUpdate()
		{
			if (this.cachedRigidbody2D != null && this.soundMoveLoop != null)
			{
				if (!this.soundIsPlaying)
				{
					this.soundIsPlaying = true;
					SoundManager.Instance.Play(this.soundMoveLoop, base.transform, new SoundParameterBase[]
					{
						this.parameterIntensity
					});
				}
				this.parameterIntensity.intensity = this.cachedRigidbody2D.velocity.magnitude;
			}
		}

		// Token: 0x04000A6D RID: 2669
		public SoundEvent soundMoveLoop;

		// Token: 0x04000A6E RID: 2670
		private Rigidbody2D cachedRigidbody2D;

		// Token: 0x04000A6F RID: 2671
		private bool soundIsPlaying;

		// Token: 0x04000A70 RID: 2672
		private SoundParameterIntensity parameterIntensity = new SoundParameterIntensity(0f, 0);
	}
}
