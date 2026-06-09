using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000A6 RID: 166
public class Populate : MonoBehaviour
{
	// Token: 0x060003AC RID: 940 RVA: 0x00016544 File Offset: 0x00014744
	public List<GameObject> DoPopulate()
	{
		List<GameObject> list = new List<GameObject>();
		if (this.includeTargetInList)
		{
			list.Add(this.target);
		}
		for (int i = 0; i < this.times; i++)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.target, this.target.transform.position, base.transform.transform.rotation, base.transform);
			gameObject.transform.localScale = this.target.transform.localScale;
			list.Add(gameObject);
			if (this.setTargetsActive)
			{
				gameObject.SetActive(true);
			}
		}
		return list;
	}

	// Token: 0x060003AD RID: 941 RVA: 0x000165E0 File Offset: 0x000147E0
	public List<T> DoPopulate<T>(bool addComponentIfMissing = true) where T : MonoBehaviour
	{
		List<T> list = new List<T>();
		if (this.includeTargetInList && this.target != null)
		{
			T t = this.target.GetComponent<T>();
			if (t == null && addComponentIfMissing)
			{
				t = this.target.AddComponent<T>();
			}
			if (!(t != null))
			{
				global::Debug.LogError("Could not find component");
				return null;
			}
			list.Add(t);
		}
		for (int i = 0; i < this.times; i++)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.target, this.target.transform.position, base.transform.transform.rotation, this.target.transform.transform.parent);
			gameObject.transform.localScale = this.target.transform.localScale;
			T t2 = gameObject.GetComponent<T>();
			if (t2 == null && addComponentIfMissing)
			{
				t2 = gameObject.AddComponent<T>();
			}
			if (t2 != null)
			{
				list.Add(t2);
			}
			if (this.setTargetsActive)
			{
				gameObject.SetActive(true);
			}
		}
		return list;
	}

	// Token: 0x040004D1 RID: 1233
	public GameObject target;

	// Token: 0x040004D2 RID: 1234
	public bool setTargetsActive;

	// Token: 0x040004D3 RID: 1235
	public bool includeTargetInList = true;

	// Token: 0x040004D4 RID: 1236
	public int times = 5;
}
