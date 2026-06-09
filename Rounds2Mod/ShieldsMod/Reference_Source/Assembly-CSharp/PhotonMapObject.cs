using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000182 RID: 386
public class PhotonMapObject : MonoBehaviour
{
	// Token: 0x060007D3 RID: 2003 RVA: 0x0002A5E0 File Offset: 0x000287E0
	private void Awake()
	{
		if (base.transform.parent != null)
		{
			Object.DestroyImmediate(base.GetComponent<PhotonView>());
		}
	}

	// Token: 0x060007D4 RID: 2004 RVA: 0x0002A600 File Offset: 0x00028800
	private void Start()
	{
		Rigidbody2D component = base.GetComponent<Rigidbody2D>();
		component.isKinematic = true;
		component.simulated = false;
		if (base.transform.parent == null)
		{
			this.photonSpawned = true;
			base.transform.SetParent(MapManager.instance.currentMap.Map.transform, true);
			this.map = base.GetComponentInParent<Map>();
			this.map.missingObjects--;
			Map map = this.map;
			map.mapIsReadyAction = (Action)Delegate.Combine(map.mapIsReadyAction, new Action(this.Go));
			if (this.map.hasRope && !base.GetComponent<PhotonView>().IsMine)
			{
				component.gravityScale = 0f;
				return;
			}
		}
		else
		{
			this.map = base.GetComponentInParent<Map>();
			Map map2 = this.map;
			map2.mapIsReadyEarlyAction = (Action)Delegate.Combine(map2.mapIsReadyEarlyAction, new Action(this.GoEarly));
		}
	}

	// Token: 0x060007D5 RID: 2005 RVA: 0x0002A6FE File Offset: 0x000288FE
	private void GoEarly()
	{
		if (this.waitingToBeRemoved)
		{
			Object.DestroyImmediate(base.gameObject);
		}
	}

	// Token: 0x060007D6 RID: 2006 RVA: 0x0002A713 File Offset: 0x00028913
	private void Go()
	{
		base.StartCoroutine(this.IGo());
	}

	// Token: 0x060007D7 RID: 2007 RVA: 0x0002A722 File Offset: 0x00028922
	private IEnumerator IGo()
	{
		Rigidbody2D rig = base.GetComponent<Rigidbody2D>();
		yield return new WaitForSeconds(0f);
		yield return new WaitForSeconds(0f);
		yield return new WaitForSeconds(0f);
		rig.isKinematic = false;
		rig.simulated = true;
		if (rig)
		{
			for (float i = 0f; i < 1f; i += Time.deltaTime * 1f)
			{
				float d = i;
				rig.velocity -= rig.velocity * d * 0.05f;
				yield return null;
			}
		}
		yield break;
	}

	// Token: 0x060007D8 RID: 2008 RVA: 0x0002A734 File Offset: 0x00028934
	private void Update()
	{
		if (this.waitingToBeRemoved)
		{
			return;
		}
		if (this.photonSpawned)
		{
			return;
		}
		this.counter += Mathf.Clamp(Time.deltaTime, 0f, 0.1f);
		if ((PhotonNetwork.OfflineMode && this.counter > 1f && this.map.hasEntered) || (this.map && this.map.hasEntered && this.map.LoadedForAll()))
		{
			if (PhotonNetwork.IsMasterClient)
			{
				PhotonNetwork.Instantiate("4 Map Objects/" + base.gameObject.name.Split(new char[]
				{
					char.Parse(" ")
				})[0], base.transform.position, base.transform.rotation, 0, null);
			}
			this.map.missingObjects++;
			this.waitingToBeRemoved = true;
		}
	}

	// Token: 0x04000930 RID: 2352
	private Map map;

	// Token: 0x04000931 RID: 2353
	private bool photonSpawned;

	// Token: 0x04000932 RID: 2354
	private float counter;

	// Token: 0x04000933 RID: 2355
	private bool waitingToBeRemoved;
}
