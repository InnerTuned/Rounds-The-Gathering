using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000087 RID: 135
public class ObjectPool
{
	// Token: 0x060002DE RID: 734 RVA: 0x00012420 File Offset: 0x00010620
	public ObjectPool(GameObject prefab, int initSpawn = 0, Transform parent = null)
	{
		this.m_objectPrefab = prefab;
		this.m_objectRoot = new GameObject(prefab.name + "_root").transform;
		this.m_objectRoot.transform.position = parent.position;
		this.m_objectRoot.transform.rotation = parent.rotation;
		this.m_objectRoot.transform.SetParent(parent, true);
		this.m_objectRoot.transform.localScale = Vector3.one;
		for (int i = 0; i < initSpawn; i++)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.m_objectPrefab, this.m_objectRoot);
			gameObject.SetActive(false);
			this.m_availableObjects.Push(gameObject);
		}
	}

	// Token: 0x060002DF RID: 735 RVA: 0x000124FC File Offset: 0x000106FC
	public GameObject GetObject()
	{
		if (this.m_availableObjects.Count > 0)
		{
			GameObject gameObject = this.m_availableObjects.Pop();
			this.m_usedObjects.Add(gameObject);
			gameObject.SetActive(true);
			return gameObject;
		}
		GameObject gameObject2 = Object.Instantiate<GameObject>(this.m_objectPrefab, this.m_objectRoot);
		this.m_usedObjects.Add(gameObject2);
		gameObject2.SetActive(true);
		return gameObject2;
	}

	// Token: 0x060002E0 RID: 736 RVA: 0x0001255E File Offset: 0x0001075E
	public bool ReleaseObject(GameObject obj)
	{
		bool flag = this.m_usedObjects.Remove(obj);
		if (flag)
		{
			this.m_availableObjects.Push(obj);
			obj.SetActive(false);
		}
		return flag;
	}

	// Token: 0x060002E1 RID: 737 RVA: 0x00012584 File Offset: 0x00010784
	public void ClearPool()
	{
		while (this.m_availableObjects.Count > 0)
		{
			Object.Destroy(this.m_availableObjects.Pop());
		}
		for (int i = 0; i < this.m_usedObjects.Count; i++)
		{
			Object.Destroy(this.m_usedObjects[i]);
		}
		this.m_usedObjects.Clear();
	}

	// Token: 0x0400040B RID: 1035
	private Stack<GameObject> m_availableObjects = new Stack<GameObject>();

	// Token: 0x0400040C RID: 1036
	private List<GameObject> m_usedObjects = new List<GameObject>();

	// Token: 0x0400040D RID: 1037
	private GameObject m_objectPrefab;

	// Token: 0x0400040E RID: 1038
	private Transform m_objectRoot;

	// Token: 0x0400040F RID: 1039
	private int testCounter = 1;
}
