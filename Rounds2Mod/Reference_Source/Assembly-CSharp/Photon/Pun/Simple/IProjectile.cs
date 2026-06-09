using System;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x0200026E RID: 622
	public interface IProjectile
	{
		// Token: 0x170000AD RID: 173
		// (get) Token: 0x06000D83 RID: 3459
		// (set) Token: 0x06000D84 RID: 3460
		IProjectileCannon Owner { get; set; }

		// Token: 0x06000D85 RID: 3461
		void Initialize(IProjectileCannon owner, int frameId, int subFrameId, Vector3 velocity, RespondTo terminateOn, RespondTo damageOn, float timeshift);
	}
}
