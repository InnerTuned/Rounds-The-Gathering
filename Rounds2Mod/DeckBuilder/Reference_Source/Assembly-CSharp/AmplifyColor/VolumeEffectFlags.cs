using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AmplifyColor
{
	// Token: 0x0200031A RID: 794
	[Serializable]
	public class VolumeEffectFlags
	{
		// Token: 0x060010E5 RID: 4325 RVA: 0x000516B4 File Offset: 0x0004F8B4
		public VolumeEffectFlags()
		{
			this.components = new List<VolumeEffectComponentFlags>();
		}

		// Token: 0x060010E6 RID: 4326 RVA: 0x000516C8 File Offset: 0x0004F8C8
		public void AddComponent(Component c)
		{
			VolumeEffectComponentFlags volumeEffectComponentFlags;
			if ((volumeEffectComponentFlags = this.components.Find((VolumeEffectComponentFlags s) => s.componentName == string.Concat(c.GetType()))) != null)
			{
				volumeEffectComponentFlags.UpdateComponentFlags(c);
				return;
			}
			this.components.Add(new VolumeEffectComponentFlags(c));
		}

		// Token: 0x060010E7 RID: 4327 RVA: 0x00051720 File Offset: 0x0004F920
		public void UpdateFlags(VolumeEffect effectVol)
		{
			using (List<VolumeEffectComponent>.Enumerator enumerator = effectVol.components.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					VolumeEffectComponent comp = enumerator.Current;
					VolumeEffectComponentFlags volumeEffectComponentFlags;
					if ((volumeEffectComponentFlags = this.components.Find((VolumeEffectComponentFlags s) => s.componentName == comp.componentName)) == null)
					{
						this.components.Add(new VolumeEffectComponentFlags(comp));
					}
					else
					{
						volumeEffectComponentFlags.UpdateComponentFlags(comp);
					}
				}
			}
		}

		// Token: 0x060010E8 RID: 4328 RVA: 0x000517B8 File Offset: 0x0004F9B8
		public static void UpdateCamFlags(AmplifyColorBase[] effects, AmplifyColorVolumeBase[] volumes)
		{
			foreach (AmplifyColorBase amplifyColorBase in effects)
			{
				amplifyColorBase.EffectFlags = new VolumeEffectFlags();
				for (int j = 0; j < volumes.Length; j++)
				{
					VolumeEffect volumeEffect = volumes[j].EffectContainer.FindVolumeEffect(amplifyColorBase);
					if (volumeEffect != null)
					{
						amplifyColorBase.EffectFlags.UpdateFlags(volumeEffect);
					}
				}
			}
		}

		// Token: 0x060010E9 RID: 4329 RVA: 0x0005181C File Offset: 0x0004FA1C
		public VolumeEffect GenerateEffectData(AmplifyColorBase go)
		{
			VolumeEffect volumeEffect = new VolumeEffect(go);
			foreach (VolumeEffectComponentFlags volumeEffectComponentFlags in this.components)
			{
				if (volumeEffectComponentFlags.blendFlag)
				{
					Component component = go.GetComponent(volumeEffectComponentFlags.componentName);
					if (component != null)
					{
						volumeEffect.AddComponent(component, volumeEffectComponentFlags);
					}
				}
			}
			return volumeEffect;
		}

		// Token: 0x060010EA RID: 4330 RVA: 0x00051898 File Offset: 0x0004FA98
		public VolumeEffectComponentFlags FindComponentFlags(string compName)
		{
			for (int i = 0; i < this.components.Count; i++)
			{
				if (this.components[i].componentName == compName)
				{
					return this.components[i];
				}
			}
			return null;
		}

		// Token: 0x060010EB RID: 4331 RVA: 0x000518E4 File Offset: 0x0004FAE4
		public string[] GetComponentNames()
		{
			return Enumerable.ToArray<string>(Enumerable.Select<VolumeEffectComponentFlags, string>(Enumerable.Where<VolumeEffectComponentFlags>(this.components, (VolumeEffectComponentFlags r) => r.blendFlag), (VolumeEffectComponentFlags r) => r.componentName));
		}

		// Token: 0x04000FD2 RID: 4050
		public List<VolumeEffectComponentFlags> components;
	}
}
