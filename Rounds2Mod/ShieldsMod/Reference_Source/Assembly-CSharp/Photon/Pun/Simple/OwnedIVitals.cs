using System;
using System.Collections.Generic;

namespace Photon.Pun.Simple
{
	// Token: 0x0200027A RID: 634
	public static class OwnedIVitals
	{
		// Token: 0x170000BA RID: 186
		// (get) Token: 0x06000DA7 RID: 3495 RVA: 0x00042920 File Offset: 0x00040B20
		public static IVitalsSystem LastItem
		{
			get
			{
				int count = OwnedIVitals.ownedVitalComponents.Count;
				if (count <= 0)
				{
					return null;
				}
				return OwnedIVitals.ownedVitalComponents[count - 1];
			}
		}

		// Token: 0x06000DA8 RID: 3496 RVA: 0x0004294C File Offset: 0x00040B4C
		public static void OnChangeAuthority(IVitalsSystem ivc, bool isMine, bool asServer)
		{
			if (isMine)
			{
				if (!OwnedIVitals.ownedVitalComponents.Contains(ivc))
				{
					OwnedIVitals.ownedVitalComponents.Add(ivc);
					for (int i = 0; i < OwnedIVitals.iOnChangeOwnedVitals.Count; i++)
					{
						OwnedIVitals.iOnChangeOwnedVitals[i].OnChangeOwnedVitals(ivc, null);
					}
					return;
				}
			}
			else if (OwnedIVitals.ownedVitalComponents.Contains(ivc))
			{
				OwnedIVitals.ownedVitalComponents.Remove(ivc);
				for (int j = 0; j < OwnedIVitals.iOnChangeOwnedVitals.Count; j++)
				{
					OwnedIVitals.iOnChangeOwnedVitals[j].OnChangeOwnedVitals(null, ivc);
				}
			}
		}

		// Token: 0x04000CFD RID: 3325
		public static List<IVitalsSystem> ownedVitalComponents = new List<IVitalsSystem>();

		// Token: 0x04000CFE RID: 3326
		public static List<IOnChangeOwnedVitals> iOnChangeOwnedVitals = new List<IOnChangeOwnedVitals>();
	}
}
