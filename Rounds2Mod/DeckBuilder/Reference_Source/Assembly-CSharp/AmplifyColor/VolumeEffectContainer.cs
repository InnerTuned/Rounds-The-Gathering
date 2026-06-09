using System;
using System.Collections.Generic;
using System.Linq;

namespace AmplifyColor
{
	// Token: 0x02000317 RID: 791
	[Serializable]
	public class VolumeEffectContainer
	{
		// Token: 0x060010D7 RID: 4311 RVA: 0x0005129F File Offset: 0x0004F49F
		public VolumeEffectContainer()
		{
			this.volumes = new List<VolumeEffect>();
		}

		// Token: 0x060010D8 RID: 4312 RVA: 0x000512B4 File Offset: 0x0004F4B4
		public void AddColorEffect(AmplifyColorBase colorEffect)
		{
			VolumeEffect volumeEffect;
			if ((volumeEffect = this.FindVolumeEffect(colorEffect)) != null)
			{
				volumeEffect.UpdateVolume();
				return;
			}
			volumeEffect = new VolumeEffect(colorEffect);
			this.volumes.Add(volumeEffect);
			volumeEffect.UpdateVolume();
		}

		// Token: 0x060010D9 RID: 4313 RVA: 0x000512EC File Offset: 0x0004F4EC
		public VolumeEffect AddJustColorEffect(AmplifyColorBase colorEffect)
		{
			VolumeEffect volumeEffect = new VolumeEffect(colorEffect);
			this.volumes.Add(volumeEffect);
			return volumeEffect;
		}

		// Token: 0x060010DA RID: 4314 RVA: 0x00051310 File Offset: 0x0004F510
		public VolumeEffect FindVolumeEffect(AmplifyColorBase colorEffect)
		{
			for (int i = 0; i < this.volumes.Count; i++)
			{
				if (this.volumes[i].gameObject == colorEffect)
				{
					return this.volumes[i];
				}
			}
			for (int j = 0; j < this.volumes.Count; j++)
			{
				if (this.volumes[j].gameObject != null && this.volumes[j].gameObject.SharedInstanceID == colorEffect.SharedInstanceID)
				{
					return this.volumes[j];
				}
			}
			return null;
		}

		// Token: 0x060010DB RID: 4315 RVA: 0x000513B9 File Offset: 0x0004F5B9
		public void RemoveVolumeEffect(VolumeEffect volume)
		{
			this.volumes.Remove(volume);
		}

		// Token: 0x060010DC RID: 4316 RVA: 0x000513C8 File Offset: 0x0004F5C8
		public AmplifyColorBase[] GetStoredEffects()
		{
			return Enumerable.ToArray<AmplifyColorBase>(Enumerable.Select<VolumeEffect, AmplifyColorBase>(this.volumes, (VolumeEffect r) => r.gameObject));
		}

		// Token: 0x04000FCB RID: 4043
		public List<VolumeEffect> volumes;
	}
}
