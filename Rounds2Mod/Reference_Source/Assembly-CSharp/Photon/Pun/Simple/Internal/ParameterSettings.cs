using System;
using System.Collections.Generic;
using Photon.Compression;
using Photon.Utilities;
using UnityEngine;

namespace Photon.Pun.Simple.Internal
{
	// Token: 0x02000303 RID: 771
	[Serializable]
	public class ParameterSettings
	{
		// Token: 0x0600107A RID: 4218 RVA: 0x0004F51C File Offset: 0x0004D71C
		public ParameterSettings(int hash, ParameterDefaults defs, ref int paramCount, AnimatorControllerParameterType paramType)
		{
			this.hash = hash;
			this.paramType = paramType;
			switch (paramType)
			{
			case 1:
				this.include = defs.includeFloats;
				this.interpolate = defs.interpolateFloats;
				this.extrapolate = defs.extrapolateFloats;
				this.defaultValue = defs.defaultFloat;
				this.fcrusher = new LiteFloatCrusher(LiteFloatCompressType.Half16, 0f, 1f, true, LiteOutOfBoundsHandling.Clamp);
				return;
			case 2:
				break;
			case 3:
				this.include = defs.includeInts;
				this.interpolate = defs.interpolateInts;
				this.extrapolate = defs.extrapolateInts;
				this.defaultValue = defs.defaultInt;
				this.icrusher = new LiteIntCrusher();
				return;
			case 4:
				this.include = defs.includeBools;
				this.interpolate = ParameterInterpolation.Hold;
				this.extrapolate = ParameterExtrapolation.Hold;
				this.defaultValue = defs.defaultBool;
				return;
			default:
				if (paramType != 9)
				{
					return;
				}
				this.include = defs.includeTriggers;
				this.interpolate = ParameterInterpolation.Default;
				this.extrapolate = ParameterExtrapolation.Default;
				this.defaultValue = defs.defaultTrigger;
				break;
			}
		}

		// Token: 0x0600107B RID: 4219 RVA: 0x0004F660 File Offset: 0x0004D860
		public static List<string> RebuildParamSettings(Animator a, ref ParameterSettings[] paraSettings, ref int paramCount, ParameterDefaults defs)
		{
			AnimatorControllerParameter[] parameters = a.parameters;
			ParameterSettings.rebuiltHashes.Clear();
			ParameterSettings.rebuiltSettings.Clear();
			bool flag = false;
			paramCount = parameters.Length;
			for (int i = 0; i < paramCount; i++)
			{
				AnimatorControllerParameter animatorControllerParameter = parameters[i];
				int nameHash = animatorControllerParameter.nameHash;
				int hashIndex = ParameterSettings.GetHashIndex(paraSettings, nameHash);
				if (hashIndex != i)
				{
					flag = true;
				}
				ParameterSettings.rebuiltHashes.Add(nameHash);
				ParameterSettings.rebuiltSettings.Add((hashIndex == -1) ? new ParameterSettings(nameHash, defs, ref paramCount, animatorControllerParameter.type) : paraSettings[hashIndex]);
			}
			if (flag)
			{
				paraSettings = ParameterSettings.rebuiltSettings.ToArray();
			}
			return null;
		}

		// Token: 0x0600107C RID: 4220 RVA: 0x0004F6FC File Offset: 0x0004D8FC
		private static int GetHashIndex(ParameterSettings[] ps, int lookfor)
		{
			int i = 0;
			int num = ps.Length;
			while (i < num)
			{
				if (ps[i].hash == lookfor)
				{
					return i;
				}
				i++;
			}
			return -1;
		}

		// Token: 0x04000F78 RID: 3960
		public int hash;

		// Token: 0x04000F79 RID: 3961
		public AnimatorControllerParameterType paramType;

		// Token: 0x04000F7A RID: 3962
		public bool include;

		// Token: 0x04000F7B RID: 3963
		public ParameterInterpolation interpolate;

		// Token: 0x04000F7C RID: 3964
		public ParameterExtrapolation extrapolate;

		// Token: 0x04000F7D RID: 3965
		public SmartVar defaultValue;

		// Token: 0x04000F7E RID: 3966
		public LiteFloatCrusher fcrusher;

		// Token: 0x04000F7F RID: 3967
		public LiteIntCrusher icrusher;

		// Token: 0x04000F80 RID: 3968
		private static readonly List<int> rebuiltHashes = new List<int>();

		// Token: 0x04000F81 RID: 3969
		private static readonly List<ParameterSettings> rebuiltSettings = new List<ParameterSettings>();
	}
}
