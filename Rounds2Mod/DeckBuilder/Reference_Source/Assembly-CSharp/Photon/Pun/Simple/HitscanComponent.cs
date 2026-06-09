using System;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x020002BC RID: 700
	public abstract class HitscanComponent : NetComponent, IOnPreSimulate
	{
		// Token: 0x06000F50 RID: 3920 RVA: 0x0004B267 File Offset: 0x00049467
		public override void OnAwake()
		{
			base.OnAwake();
			if (!this.origin)
			{
				this.origin = base.gameObject;
			}
		}

		// Token: 0x06000F51 RID: 3921 RVA: 0x0004B288 File Offset: 0x00049488
		public virtual void OnPreSimulate(int frameId, int subFrameId)
		{
			if (!this.triggerQueued)
			{
				return;
			}
			int num = -1;
			RaycastHit[] array;
			Collider[] hits;
			int hitcount = this.hitscanDefinition.GenericHitscanNonAlloc(base.transform, out array, out hits, ref num, false, false);
			if (this.visualize)
			{
				this.hitscanDefinition.VisualizeHitscan(base.transform, 0.5f);
			}
			this.triggerQueued = false;
			this.ProcessHits(hits, hitcount);
		}

		// Token: 0x06000F52 RID: 3922 RVA: 0x0004B2E8 File Offset: 0x000494E8
		public virtual void ProcessHits(Collider[] hits, int hitcount)
		{
			int i = 0;
			while (i < hitcount)
			{
				Collider collider = hits[i];
				if (!this.ignoreSelf)
				{
					goto IL_90;
				}
				NetObject parentComponent = NestedComponentUtilities.GetParentComponent<NetObject>(collider.transform);
				if (!(parentComponent == this.netObj))
				{
					goto IL_90;
				}
				global::Debug.Log(string.Concat(new string[]
				{
					"Ignoring self ",
					base.name,
					" hit: ",
					collider ? collider.name : "null",
					" hitnetobj: ",
					parentComponent ? parentComponent.name : "null"
				}));
				IL_9A:
				i++;
				continue;
				IL_90:
				if (this.ProcessHit(collider))
				{
					return;
				}
				goto IL_9A;
			}
		}

		// Token: 0x06000F53 RID: 3923
		public abstract bool ProcessHit(Collider hit);

		// Token: 0x04000E76 RID: 3702
		public GameObject origin;

		// Token: 0x04000E77 RID: 3703
		public HitscanDefinition hitscanDefinition = new HitscanDefinition();

		// Token: 0x04000E78 RID: 3704
		[Tooltip("Ignore any collider hits that are nested children of the same NetObject this Hitscan is on.")]
		public bool ignoreSelf = true;

		// Token: 0x04000E79 RID: 3705
		public bool visualize;

		// Token: 0x04000E7A RID: 3706
		protected bool triggerQueued;
	}
}
