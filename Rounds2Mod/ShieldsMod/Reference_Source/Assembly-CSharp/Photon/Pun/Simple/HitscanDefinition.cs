using System;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x020002BE RID: 702
	[Serializable]
	public class HitscanDefinition
	{
		// Token: 0x04000E80 RID: 3712
		public Replicate ownerToAuthority = Replicate.Hits;

		// Token: 0x04000E81 RID: 3713
		public Replicate authorityToAll = Replicate.Hits;

		// Token: 0x04000E82 RID: 3714
		[Tooltip("This cast/overlap test will be done on the initiating client against colliders on these layers. Exclude layers that won't include any objects that you don't want to test (such as walls).")]
		public LayerMask layerMask = -1;

		// Token: 0x04000E83 RID: 3715
		public bool useOffset;

		// Token: 0x04000E84 RID: 3716
		public Vector3 offset1 = new Vector3(0f, 0f, 0f);

		// Token: 0x04000E85 RID: 3717
		public Vector3 offset2 = new Vector3(0f, 1f, 0f);

		// Token: 0x04000E86 RID: 3718
		public Vector3 halfExtents = new Vector3(1f, 1f, 1f);

		// Token: 0x04000E87 RID: 3719
		public Vector3 orientation = new Vector3(0f, 0f, 0f);

		// Token: 0x04000E88 RID: 3720
		public HitscanType hitscanType;

		// Token: 0x04000E89 RID: 3721
		public float distance = 100f;

		// Token: 0x04000E8A RID: 3722
		public float radius = 1f;

		// Token: 0x04000E8B RID: 3723
		public bool nearestOnly = true;
	}
}
