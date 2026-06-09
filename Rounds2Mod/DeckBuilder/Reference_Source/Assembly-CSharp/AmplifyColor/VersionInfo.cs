using System;
using UnityEngine;

namespace AmplifyColor
{
	// Token: 0x02000313 RID: 787
	[Serializable]
	public class VersionInfo
	{
		// Token: 0x060010B8 RID: 4280 RVA: 0x00050499 File Offset: 0x0004E699
		public static string StaticToString()
		{
			return string.Format("{0}.{1}.{2}", 1, 8, 2) + VersionInfo.StageSuffix + VersionInfo.TrialSuffix;
		}

		// Token: 0x060010B9 RID: 4281 RVA: 0x000504C6 File Offset: 0x0004E6C6
		public override string ToString()
		{
			return string.Format("{0}.{1}.{2}", this.m_major, this.m_minor, this.m_release) + VersionInfo.StageSuffix + VersionInfo.TrialSuffix;
		}

		// Token: 0x17000136 RID: 310
		// (get) Token: 0x060010BA RID: 4282 RVA: 0x00050502 File Offset: 0x0004E702
		public int Number
		{
			get
			{
				return this.m_major * 100 + this.m_minor * 10 + this.m_release;
			}
		}

		// Token: 0x060010BB RID: 4283 RVA: 0x0005051E File Offset: 0x0004E71E
		private VersionInfo()
		{
			this.m_major = 1;
			this.m_minor = 8;
			this.m_release = 2;
		}

		// Token: 0x060010BC RID: 4284 RVA: 0x0005053B File Offset: 0x0004E73B
		private VersionInfo(byte major, byte minor, byte release)
		{
			this.m_major = (int)major;
			this.m_minor = (int)minor;
			this.m_release = (int)release;
		}

		// Token: 0x060010BD RID: 4285 RVA: 0x00050558 File Offset: 0x0004E758
		public static VersionInfo Current()
		{
			return new VersionInfo(1, 8, 2);
		}

		// Token: 0x060010BE RID: 4286 RVA: 0x00050562 File Offset: 0x0004E762
		public static bool Matches(VersionInfo version)
		{
			return 1 == version.m_major && 8 == version.m_minor && 2 == version.m_release;
		}

		// Token: 0x04000FB7 RID: 4023
		public const byte Major = 1;

		// Token: 0x04000FB8 RID: 4024
		public const byte Minor = 8;

		// Token: 0x04000FB9 RID: 4025
		public const byte Release = 2;

		// Token: 0x04000FBA RID: 4026
		private static string StageSuffix = "";

		// Token: 0x04000FBB RID: 4027
		private static string TrialSuffix = "";

		// Token: 0x04000FBC RID: 4028
		[SerializeField]
		private int m_major;

		// Token: 0x04000FBD RID: 4029
		[SerializeField]
		private int m_minor;

		// Token: 0x04000FBE RID: 4030
		[SerializeField]
		private int m_release;
	}
}
