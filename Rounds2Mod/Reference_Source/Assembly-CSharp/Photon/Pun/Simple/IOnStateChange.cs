using System;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x020002A1 RID: 673
	public interface IOnStateChange
	{
		// Token: 0x06000ED0 RID: 3792
		void OnStateChange(ObjState newState, ObjState previousState, Transform attachmentTransform, Mount attachTo = null, bool isReady = true);
	}
}
