using System;
using System.Collections.Generic;
using Photon.Utilities;
using UnityEngine;

namespace Photon.Pun.Simple.Internal
{
	// Token: 0x02000307 RID: 775
	public static class NetObjColliderExt
	{
		// Token: 0x0600107E RID: 4222 RVA: 0x0004F740 File Offset: 0x0004D940
		public static void IndexColliders(this NetObject netObj)
		{
			List<Component> indexedColliders = netObj.indexedColliders;
			Dictionary<Component, int> colliderLookup = netObj.colliderLookup;
			colliderLookup.Clear();
			indexedColliders.Clear();
			NestedComponentUtilities.GetNestedComponentsInChildren<Component, NetObject>(netObj.transform, NetObjColliderExt.reusableComponents, true);
			int count = NetObjColliderExt.reusableComponents.Count;
			for (int i = 0; i < count; i++)
			{
				Component component = NetObjColliderExt.reusableComponents[i];
				if (component as Collider)
				{
					indexedColliders.Add(component);
				}
				else if (component as Collider2D)
				{
					indexedColliders.Add(component);
				}
			}
			int j = 0;
			int count2 = indexedColliders.Count;
			while (j < count2)
			{
				colliderLookup.Add(indexedColliders[j], j);
				j++;
			}
			netObj.bitsForColliderIndex = (indexedColliders.Count - 1).GetBitsForMaxValue();
		}

		// Token: 0x0600107F RID: 4223 RVA: 0x0004F808 File Offset: 0x0004DA08
		public static int GetFirstChildCollider(this Transform transform, ref Component firstFoundCollider, bool countTriggers, bool countNonTriggers)
		{
			if (!countTriggers && !countNonTriggers)
			{
				global::Debug.LogError("Counting Colliders, but args indicate to ignore everything. Set one to true.");
				firstFoundCollider = null;
				return 0;
			}
			transform.GetComponentsInChildren<Collider>(true, NetObjColliderExt.reusableColliders);
			int count = NetObjColliderExt.reusableColliders.Count;
			if (count > 0)
			{
				if (countTriggers && countNonTriggers)
				{
					firstFoundCollider = NetObjColliderExt.reusableColliders[0];
					return count;
				}
				int num = 0;
				firstFoundCollider = null;
				for (int i = 0; i < count; i++)
				{
					Collider collider = NetObjColliderExt.reusableColliders[i];
					if (countTriggers ? collider.isTrigger : (!collider.isTrigger))
					{
						if (firstFoundCollider == null)
						{
							firstFoundCollider = collider;
						}
						num++;
					}
				}
				return num;
			}
			else
			{
				transform.GetComponentsInChildren<Collider2D>(true, NetObjColliderExt.reusableColliders2D);
				int count2 = NetObjColliderExt.reusableColliders2D.Count;
				if (count2 <= 0)
				{
					firstFoundCollider = null;
					return 0;
				}
				if (countTriggers && countNonTriggers)
				{
					firstFoundCollider = NetObjColliderExt.reusableColliders[0];
					return count;
				}
				int num2 = 0;
				firstFoundCollider = null;
				for (int j = 0; j < count2; j++)
				{
					Collider2D collider2D = NetObjColliderExt.reusableColliders2D[j];
					if (countTriggers ? collider2D.isTrigger : (!collider2D.isTrigger))
					{
						if (firstFoundCollider == null)
						{
							firstFoundCollider = collider2D;
						}
						num2++;
					}
				}
				return num2;
			}
		}

		// Token: 0x06001080 RID: 4224 RVA: 0x0004F92C File Offset: 0x0004DB2C
		public static int CountChildCollider(this Transform transform, bool countTriggers, bool countNonTriggers)
		{
			if (!countTriggers && !countNonTriggers)
			{
				global::Debug.LogError("Counting Colliders, but args indicate to ignore everything. Set one to true.");
				return 0;
			}
			transform.GetComponentsInChildren<Collider>(true, NetObjColliderExt.reusableColliders);
			int count = NetObjColliderExt.reusableColliders.Count;
			if (count > 0)
			{
				if (countTriggers && countNonTriggers)
				{
					return count;
				}
				int num = 0;
				for (int i = 0; i < count; i++)
				{
					Collider collider = NetObjColliderExt.reusableColliders[i];
					if (countTriggers ? collider.isTrigger : (!collider.isTrigger))
					{
						num++;
					}
				}
				return num;
			}
			else
			{
				transform.GetComponentsInChildren<Collider2D>(true, NetObjColliderExt.reusableColliders2D);
				int count2 = NetObjColliderExt.reusableColliders2D.Count;
				if (count2 <= 0)
				{
					return 0;
				}
				if (countTriggers && countNonTriggers)
				{
					return count;
				}
				int num2 = 0;
				for (int j = 0; j < count2; j++)
				{
					Collider2D collider2D = NetObjColliderExt.reusableColliders2D[j];
					if (countTriggers ? collider2D.isTrigger : (!collider2D.isTrigger))
					{
						num2++;
					}
				}
				return num2;
			}
		}

		// Token: 0x04000F8E RID: 3982
		public static readonly List<Component> reusableComponents = new List<Component>();

		// Token: 0x04000F8F RID: 3983
		public static readonly List<Collider> reusableColliders = new List<Collider>();

		// Token: 0x04000F90 RID: 3984
		public static readonly List<Collider2D> reusableColliders2D = new List<Collider2D>();
	}
}
