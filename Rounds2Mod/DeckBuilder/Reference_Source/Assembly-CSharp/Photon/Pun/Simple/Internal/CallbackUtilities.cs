using System;
using System.Collections.Generic;

namespace Photon.Pun.Simple.Internal
{
	// Token: 0x02000309 RID: 777
	public static class CallbackUtilities
	{
		// Token: 0x06001090 RID: 4240 RVA: 0x0004FCFC File Offset: 0x0004DEFC
		public static int RegisterInterface<T>(List<T> callbackList, object c, bool register) where T : class
		{
			if (callbackList == null)
			{
				callbackList = new List<T>();
			}
			T t = c as T;
			if (t == null)
			{
				return callbackList.Count;
			}
			if (register)
			{
				if (!callbackList.Contains(t))
				{
					callbackList.Add(t);
				}
			}
			else if (callbackList.Contains(t))
			{
				callbackList.Remove(t);
			}
			return callbackList.Count;
		}
	}
}
