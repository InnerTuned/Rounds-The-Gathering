using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000C6 RID: 198
public class ScreenEdgeBounce : MonoBehaviour
{
	// Token: 0x06000426 RID: 1062 RVA: 0x00019408 File Offset: 0x00017608
	private void Start()
	{
		base.GetComponentInParent<ChildRPC>().childRPCsVector2Vector2IntInt.Add("ScreenBounce", new Action<Vector2, Vector2, int, int>(this.DoHit));
		this.view = base.GetComponentInParent<PhotonView>();
		this.bulletSound = base.GetComponentInParent<RayHitBulletSound>();
		this.projHit = base.GetComponentInParent<ProjectileHit>();
		ScreenEdgeBounce[] componentsInChildren = base.transform.root.GetComponentsInChildren<ScreenEdgeBounce>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (i > 0)
			{
				Object.Destroy(componentsInChildren[i]);
			}
		}
		this.mainCam = MainCam.instance.transform.GetComponent<Camera>();
		this.reflect = base.GetComponentInParent<RayHitReflect>();
	}

	// Token: 0x06000427 RID: 1063 RVA: 0x000194A8 File Offset: 0x000176A8
	private void Update()
	{
		if (!this.view.IsMine)
		{
			return;
		}
		if (this.done)
		{
			return;
		}
		Vector3 vector = this.mainCam.WorldToScreenPoint(base.transform.position);
		vector.x /= (float)Screen.width;
		vector.y /= (float)Screen.height;
		vector = new Vector3(Mathf.Clamp(vector.x, 0f, 1f), Mathf.Clamp(vector.y, 0f, 1f), vector.z);
		if (vector.x == 0f || vector.x == 1f || vector.y == 1f || vector.y == 0f)
		{
			Vector2 vector2 = Vector2.zero;
			if (vector.x == 0f)
			{
				vector2 = Vector2.right;
			}
			else if (vector.x == 1f)
			{
				vector2 = -Vector2.right;
			}
			if (vector.y == 0f)
			{
				vector2 = Vector2.up;
			}
			else if (vector.y == 1f)
			{
				vector2 = -Vector2.up;
			}
			if (this.lastNormal == vector2 && Vector2.Angle(vector2, base.transform.forward) < 90f)
			{
				this.lastNormal = vector2;
				return;
			}
			this.lastNormal = vector2;
			vector.x *= (float)Screen.width;
			vector.y *= (float)Screen.height;
			RaycastHit2D raycastHit2D = default(RaycastHit2D);
			raycastHit2D.normal = vector2;
			raycastHit2D.point = this.mainCam.ScreenToWorldPoint(vector);
			int num = -1;
			if (raycastHit2D.transform)
			{
				PhotonView component = raycastHit2D.transform.root.GetComponent<PhotonView>();
				if (component)
				{
					num = component.ViewID;
				}
			}
			int intData = -1;
			if (num == -1)
			{
				Collider2D[] componentsInChildren = MapManager.instance.currentMap.Map.GetComponentsInChildren<Collider2D>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					if (componentsInChildren[i] == raycastHit2D.collider)
					{
						intData = i;
					}
				}
			}
			base.GetComponentInParent<ChildRPC>().CallFunction("ScreenBounce", raycastHit2D.point, raycastHit2D.normal, num, intData);
			if (this.reflect.reflects <= 0)
			{
				this.done = true;
			}
			this.sinceBounce = 0f;
		}
	}

	// Token: 0x06000428 RID: 1064 RVA: 0x00019714 File Offset: 0x00017914
	private void DoHit(Vector2 hitPos, Vector2 hitNormal, int viewID = -1, int colliderID = -1)
	{
		HitInfo hitInfo = new HitInfo();
		hitInfo.point = hitPos;
		hitInfo.normal = hitNormal;
		hitInfo.collider = null;
		if (viewID != -1)
		{
			PhotonView photonView = PhotonNetwork.GetPhotonView(viewID);
			hitInfo.collider = photonView.GetComponentInChildren<Collider2D>();
			hitInfo.transform = photonView.transform;
		}
		else if (colliderID != -1)
		{
			hitInfo.collider = MapManager.instance.currentMap.Map.GetComponentsInChildren<Collider2D>()[colliderID];
			hitInfo.transform = hitInfo.collider.transform;
		}
		DynamicParticles.instance.PlayBulletHit(this.projHit.damage, base.transform, hitInfo, this.projHit.projectileColor);
		this.bulletSound.DoHitEffect(hitInfo);
		this.reflect.DoHitEffect(hitInfo);
	}

	// Token: 0x040005AB RID: 1451
	private float sinceBounce = 1f;

	// Token: 0x040005AC RID: 1452
	private Camera mainCam;

	// Token: 0x040005AD RID: 1453
	private RayHitReflect reflect;

	// Token: 0x040005AE RID: 1454
	private Vector2 lastNormal;

	// Token: 0x040005AF RID: 1455
	private ProjectileHit projHit;

	// Token: 0x040005B0 RID: 1456
	private bool done;

	// Token: 0x040005B1 RID: 1457
	private RayHitBulletSound bulletSound;

	// Token: 0x040005B2 RID: 1458
	private PhotonView view;
}
