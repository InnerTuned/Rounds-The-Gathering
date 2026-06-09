using System;
using System.Collections.Generic;
using UnityEngine;

namespace Photon.Pun.Simple.Pooling
{
	// Token: 0x020002F9 RID: 761
	public class Pool : MonoBehaviour
	{
		// Token: 0x06001048 RID: 4168 RVA: 0x0004ED28 File Offset: 0x0004CF28
		public static void AddPrefabToPool(GameObject _prefab, int startingSize = 8, int _growBy = 8, Type _scriptToAdd = null, bool tidyUp = false)
		{
			if (Pool.poolItemDefs.ContainsKey(_prefab))
			{
				return;
			}
			Pool.pools.Add(_prefab, new Stack<Pool>());
			Pool.PoolItemDef poolItemDef = new Pool.PoolItemDef(_prefab, _growBy, _scriptToAdd);
			Pool.poolItemDefs.Add(_prefab, poolItemDef);
			Pool.GrowPool(_prefab, startingSize);
			if (tidyUp)
			{
				_prefab.gameObject.SetActive(false);
			}
		}

		// Token: 0x06001049 RID: 4169 RVA: 0x0004ED80 File Offset: 0x0004CF80
		public static Pool Spawn(GameObject origPrefab, Transform t, float duration = 5f)
		{
			return Pool.Spawn(origPrefab, t.position, t.rotation, duration);
		}

		// Token: 0x0600104A RID: 4170 RVA: 0x0004ED95 File Offset: 0x0004CF95
		public static Pool Spawn(GameObject origPrefab, Vector3 pos, Quaternion rot, Vector3 scl, float duration = 5f)
		{
			Pool pool = Pool.Spawn(origPrefab, pos, rot, duration);
			pool.gameObject.transform.localScale = scl;
			return pool;
		}

		// Token: 0x0600104B RID: 4171 RVA: 0x0004EDB4 File Offset: 0x0004CFB4
		public static Pool Spawn(GameObject origPrefab, Vector3 pos, Quaternion rot, float duration = 5f)
		{
			if (Pool.pools[origPrefab].Count == 0)
			{
				Pool.GrowPool(origPrefab, -1);
			}
			if (!Pool.pools[origPrefab].Peek())
			{
				Pool.pools[origPrefab].Clear();
				Pool.GrowPool(origPrefab, -1);
			}
			Pool pool = Pool.pools[origPrefab].Pop();
			pool.transform.position = pos;
			pool.transform.rotation = rot;
			pool.deathClock = duration;
			pool.enabled = (duration > 0f);
			pool.gameObject.SetActive(true);
			return pool;
		}

		// Token: 0x0600104C RID: 4172 RVA: 0x0004EE54 File Offset: 0x0004D054
		private static void GrowPool(GameObject _prefab, int growAmt = -1)
		{
			Pool.PoolItemDef poolItemDef = Pool.poolItemDefs[_prefab];
			int num = (growAmt < 1) ? poolItemDef.growBy : growAmt;
			for (int i = 0; i < num; i++)
			{
				Pool.AddItemToPool(Object.Instantiate<GameObject>(poolItemDef.prefab), _prefab, poolItemDef);
			}
		}

		// Token: 0x0600104D RID: 4173 RVA: 0x0004EE9C File Offset: 0x0004D09C
		private static void AddItemToPool(GameObject go, GameObject _prefab, Pool.PoolItemDef def)
		{
			go.SetActive(false);
			Pool pool = go.AddComponent<Pool>();
			pool.CacheComponents();
			if (def.scriptToAdd != null && go.GetComponent(def.scriptToAdd) == null)
			{
				pool.extraScript = go.AddComponent(def.scriptToAdd);
			}
			pool.origPrefab = _prefab;
			Pool.pools[_prefab].Push(pool);
		}

		// Token: 0x0600104E RID: 4174 RVA: 0x0004EF09 File Offset: 0x0004D109
		private static void ReturnToPool(Pool p, GameObject _prefab)
		{
			Pool.pools[_prefab].Push(p);
		}

		// Token: 0x0600104F RID: 4175 RVA: 0x0004EF1C File Offset: 0x0004D11C
		public void CacheComponents()
		{
			this.rb = base.GetComponent<Rigidbody>();
		}

		// Token: 0x06001050 RID: 4176 RVA: 0x0004EF2A File Offset: 0x0004D12A
		private void Update()
		{
			this.deathClock -= TimeHandler.deltaTime;
			if (this.deathClock < 0f)
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x06001051 RID: 4177 RVA: 0x0004EF57 File Offset: 0x0004D157
		private void OnDisable()
		{
			Pool.ReturnToPool(this, this.origPrefab);
		}

		// Token: 0x04000F55 RID: 3925
		private static Dictionary<GameObject, Stack<Pool>> pools = new Dictionary<GameObject, Stack<Pool>>();

		// Token: 0x04000F56 RID: 3926
		private static Dictionary<GameObject, Pool.PoolItemDef> poolItemDefs = new Dictionary<GameObject, Pool.PoolItemDef>();

		// Token: 0x04000F57 RID: 3927
		[HideInInspector]
		public GameObject origPrefab;

		// Token: 0x04000F58 RID: 3928
		[HideInInspector]
		public Rigidbody rb;

		// Token: 0x04000F59 RID: 3929
		[HideInInspector]
		public Component extraScript;

		// Token: 0x04000F5A RID: 3930
		public float deathClock;

		// Token: 0x020003DE RID: 990
		private struct PoolItemDef
		{
			// Token: 0x06001401 RID: 5121 RVA: 0x0005AC1C File Offset: 0x00058E1C
			public PoolItemDef(GameObject prefab, int growBy, Type scriptToAdd)
			{
				this.prefab = prefab;
				this.growBy = growBy;
				this.scriptToAdd = scriptToAdd;
			}

			// Token: 0x0400132F RID: 4911
			public GameObject prefab;

			// Token: 0x04001330 RID: 4912
			public int growBy;

			// Token: 0x04001331 RID: 4913
			public Type scriptToAdd;
		}
	}
}
