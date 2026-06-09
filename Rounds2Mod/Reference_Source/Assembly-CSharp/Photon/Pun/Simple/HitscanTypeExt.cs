using System;

namespace Photon.Pun.Simple
{
	// Token: 0x020002BA RID: 698
	public static class HitscanTypeExt
	{
		// Token: 0x06000F43 RID: 3907 RVA: 0x0004A4BC File Offset: 0x000486BC
		public static bool IsCast(this HitscanType hitscanType)
		{
			return hitscanType < HitscanType.OverlapSphere;
		}

		// Token: 0x06000F44 RID: 3908 RVA: 0x0004A4C2 File Offset: 0x000486C2
		public static bool IsOverlap(this HitscanType hitscanType)
		{
			return hitscanType > HitscanType.BoxCast;
		}

		// Token: 0x06000F45 RID: 3909 RVA: 0x0004A4C8 File Offset: 0x000486C8
		public static bool UsesRadius(this HitscanType hitscanType)
		{
			return hitscanType == HitscanType.SphereCast || hitscanType == HitscanType.CapsuleCast || hitscanType == HitscanType.OverlapSphere || hitscanType == HitscanType.OverlapCapsule;
		}

		// Token: 0x06000F46 RID: 3910 RVA: 0x0004A4DC File Offset: 0x000486DC
		public static bool IsBox(this HitscanType hitscanType)
		{
			return hitscanType == HitscanType.BoxCast || hitscanType == HitscanType.OverlapBox;
		}

		// Token: 0x06000F47 RID: 3911 RVA: 0x0004A4E8 File Offset: 0x000486E8
		public static bool IsCapsule(this HitscanType hitscanType)
		{
			return hitscanType == HitscanType.CapsuleCast || hitscanType == HitscanType.OverlapCapsule;
		}
	}
}
