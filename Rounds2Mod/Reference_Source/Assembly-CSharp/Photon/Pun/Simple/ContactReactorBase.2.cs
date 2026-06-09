using System;

namespace Photon.Pun.Simple
{
	// Token: 0x0200025B RID: 603
	public abstract class ContactReactorBase<T> : ContactReactorBase, IOnContactEvent where T : class, IContactSystem
	{
	}
}
