using System;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x020002D5 RID: 725
	public static class DoubleTime
	{
		// Token: 0x06000F82 RID: 3970 RVA: 0x0004B5D8 File Offset: 0x000497D8
		public static void SnapFixed()
		{
			DoubleTime.prevFixedTime = DoubleTime.fixedTime;
			DoubleTime.fixedDeltaTime = (double)Time.fixedDeltaTime;
			if (DoubleTime.fixedTime == 0.0)
			{
				DoubleTime.fixedTime = (double)Time.fixedTime;
				DoubleTime.time = (double)Time.time;
			}
			else
			{
				DoubleTime.fixedTime += DoubleTime.fixedDeltaTime;
				DoubleTime.mixedDeltaTime = (float)(DoubleTime.fixedTime - DoubleTime.time);
				DoubleTime.time = DoubleTime.fixedTime;
			}
			DoubleTime.isInFixed = true;
			DoubleTime.isFirstUpdatePostFixed = true;
		}

		// Token: 0x06000F83 RID: 3971 RVA: 0x0004B65C File Offset: 0x0004985C
		public static void SnapUpdate()
		{
			DoubleTime.prevUpdateTime = DoubleTime.updateTime;
			if (DoubleTime.updateTime == 0.0)
			{
				DoubleTime.updateTime = (double)Time.time;
				DoubleTime.fixedTime = (double)Time.fixedTime;
				DoubleTime.time = DoubleTime.fixedTime;
			}
			else
			{
				DoubleTime.updateTime += (double)Time.deltaTime;
			}
			DoubleTime.timeSinceFixed = DoubleTime.updateTime - DoubleTime.fixedTime;
			DoubleTime.deltaTime = DoubleTime.updateTime - DoubleTime.prevUpdateTime;
			DoubleTime.normTimeSinceFixed = (float)(DoubleTime.timeSinceFixed / (double)Time.fixedDeltaTime);
			DoubleTime.mixedDeltaTime = (DoubleTime.isFirstUpdatePostFixed ? ((float)(DoubleTime.updateTime - DoubleTime.time)) : Time.deltaTime);
			DoubleTime.time = DoubleTime.updateTime;
			DoubleTime.isFirstUpdatePostFixed = false;
			DoubleTime.isInFixed = false;
		}

		// Token: 0x04000E91 RID: 3729
		public static double time;

		// Token: 0x04000E92 RID: 3730
		public static double prevUpdateTime;

		// Token: 0x04000E93 RID: 3731
		public static double prevFixedTime;

		// Token: 0x04000E94 RID: 3732
		public static double deltaTime;

		// Token: 0x04000E95 RID: 3733
		public static double timeSinceFixed;

		// Token: 0x04000E96 RID: 3734
		public static double fixedTime;

		// Token: 0x04000E97 RID: 3735
		public static double fixedDeltaTime;

		// Token: 0x04000E98 RID: 3736
		public static float normTimeSinceFixed;

		// Token: 0x04000E99 RID: 3737
		public static double updateTime;

		// Token: 0x04000E9A RID: 3738
		public static float mixedDeltaTime;

		// Token: 0x04000E9B RID: 3739
		public static bool isInFixed;

		// Token: 0x04000E9C RID: 3740
		private static bool isFirstUpdatePostFixed;
	}
}
