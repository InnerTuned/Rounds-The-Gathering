using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000185 RID: 389
public class PlayerAIZorro : MonoBehaviour
{
	// Token: 0x060007E3 RID: 2019 RVA: 0x0002AD48 File Offset: 0x00028F48
	private void Start()
	{
		this.api = base.GetComponentInParent<PlayerAPI>();
		this.m_camera = Camera.main;
		HealthHandler healthHandler = this.api.player.data.healthHandler;
		healthHandler.delayedReviveAction = (Action)Delegate.Combine(healthHandler.delayedReviveAction, new Action(this.Init));
	}

	// Token: 0x060007E4 RID: 2020 RVA: 0x0002ADA2 File Offset: 0x00028FA2
	public void Init()
	{
		base.StopAllCoroutines();
		this.BakeMapSurfaces();
		base.StartCoroutine(this.GetNewPos());
	}

	// Token: 0x060007E5 RID: 2021 RVA: 0x0002ADC0 File Offset: 0x00028FC0
	private void Update()
	{
		this.framesSinceShot++;
		if (this.api.CanShoot())
		{
			this.timeSinceCouldShoot += TimeHandler.deltaTime;
		}
		else
		{
			this.timeSinceCouldShoot = 0f;
		}
		Vector3 vector = this.api.OtherPlayerPosition() + this.api.GetOtherPlayer().data.playerVel.velocity * Vector3.Distance(base.transform.position, this.api.OtherPlayerPosition()) * 0.01f + Vector3.down * this.api.GetOtherPlayer().data.playerVel.velocity.y * Vector3.Distance(base.transform.position, this.api.OtherPlayerPosition()) * 0.005f;
		bool flag;
		if (Physics2D.Raycast(base.transform.position, Vector3.down, 18f, this.m_MapMask).transform == null)
		{
			flag = false;
			this.tiemSpentOverDeath += TimeHandler.deltaTime;
		}
		else
		{
			flag = true;
			this.tiemSpentOverDeath = 0f;
		}
		if (this.api.CanBlock() && Vector3.Distance(base.transform.position, this.api.OtherPlayerPosition()) > 5f && (flag || this.tiemSpentOverDeath < 0.25f))
		{
			this.api.Move(vector - base.transform.position + new Vector3(Mathf.PerlinNoise(Time.time, 0f) * 5f * Mathf.Sin(Time.time * 2f), 0f, 0f));
		}
		else
		{
			this.api.Move(this.m_CurrentHidePos - base.transform.position + new Vector3(Mathf.PerlinNoise(Time.time, 0f) * 8f * Mathf.Sin(Time.time * 3f), 0f, 0f));
		}
		this.api.Jump();
		this.ShootAt(vector);
		this.MakeSureToBlock();
	}

	// Token: 0x060007E6 RID: 2022 RVA: 0x0002B034 File Offset: 0x00029234
	public void MakeSureToBlock()
	{
		BulletWrapper[] array = this.api.GetAllBullets().ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			float num = Vector3.Distance(array[i].projectileMovement.transform.position, base.transform.position);
			float num2 = Vector3.Angle(array[i].velocity.normalized, base.transform.position - array[i].projectileMovement.transform.position);
			if (num < 1.3f && num2 < 65f && this.framesSinceShot >= 4)
			{
				this.api.Block();
			}
		}
	}

	// Token: 0x060007E7 RID: 2023 RVA: 0x0002B0E8 File Offset: 0x000292E8
	public BulletWrapper GetMostDangerousBullet(BulletWrapper[] bullets, out bool exsists)
	{
		float num = 999999f;
		int num2 = -1;
		for (int i = 0; i < bullets.Length; i++)
		{
			float num3 = Vector3.Distance(bullets[i].projectileMovement.transform.position, base.transform.position);
			if (num3 < num && num3 < 5f)
			{
				num = num3;
				num2 = i;
			}
		}
		if (num2 != -1)
		{
			exsists = true;
			return bullets[num2];
		}
		exsists = false;
		return new BulletWrapper();
	}

	// Token: 0x060007E8 RID: 2024 RVA: 0x0002B154 File Offset: 0x00029354
	public BulletWrapper[] GetAllBulletsComingAtMe()
	{
		List<BulletWrapper> list = new List<BulletWrapper>();
		BulletWrapper[] array = this.api.GetAllBullets().ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			if (Vector3.Angle(array[i].velocity.normalized, base.transform.position - array[i].projectileMovement.transform.position) < 35f)
			{
				list.Add(array[i]);
			}
		}
		return list.ToArray();
	}

	// Token: 0x060007E9 RID: 2025 RVA: 0x0002B1D5 File Offset: 0x000293D5
	public void ShootAt(Vector3 point)
	{
		this.api.SetAimDirection(this.GetAimDirForHitting(point));
		this.api.Attack();
	}

	// Token: 0x060007EA RID: 2026 RVA: 0x0002B1F4 File Offset: 0x000293F4
	private Vector2 GetAimDirForHitting(Vector3 point)
	{
		Vector3 v = point - base.transform.position;
		this.api.SetAimDirection(v);
		this.api.GetMyBullet();
		float time = Mathf.Abs(point.x - base.transform.position.x);
		Vector3 vector = point + Vector3.up * this.m_AimCompensastionCurve.Evaluate(time);
		global::Debug.DrawLine(point, vector, Color.red, 0.2f);
		return vector - base.transform.position;
	}

	// Token: 0x060007EB RID: 2027 RVA: 0x0002B294 File Offset: 0x00029494
	public void BakeMapSurfaces()
	{
		Vector2 vector = this.m_camera.ViewportToWorldPoint(new Vector3(1f, 1f));
		Vector2 vector2 = this.m_camera.ViewportToWorldPoint(new Vector3(0f, 1f));
		Vector2 vector3 = this.m_camera.ViewportToWorldPoint(new Vector3(1f, 0f));
		Vector2 v = this.m_camera.ViewportToWorldPoint(new Vector3(0f, 0f));
		Vector3 v2 = this.m_camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f));
		global::Debug.DrawLine(vector, v2, Color.cyan);
		global::Debug.DrawLine(vector2, v2, Color.cyan);
		global::Debug.DrawLine(vector3, v2, Color.cyan);
		global::Debug.DrawLine(v, v2, Color.cyan);
		List<Vector3> list = new List<Vector3>();
		for (int i = 0; i < 360; i++)
		{
			for (int j = 0; j < 40; j++)
			{
				RaycastHit2D raycastHit2D = Physics2D.Raycast(new Vector2(Mathf.Lerp(vector2.x, vector.x, (float)i / 359f), Mathf.Lerp(vector3.y, vector.y, (float)j / 39f)), Vector3.down, 6f, this.m_MapMask);
				if (raycastHit2D.transform)
				{
					list.Add(raycastHit2D.point);
				}
			}
		}
		List<Vector3> list2 = new List<Vector3>();
		for (int k = 0; k < list.Count; k++)
		{
			if (Physics2D.OverlapCircleAll(list[k] + Vector3.up * 0.26f, 0.1f, this.m_MapMask).Length != 0)
			{
				list2.Add(list[k]);
			}
		}
		for (int l = 0; l < list2.Count; l++)
		{
			list.Remove(list2[l]);
		}
		global::Debug.Log("Points: " + list.Count);
		for (int m = 0; m < list.Count; m++)
		{
			global::Debug.DrawLine(list[m], list[m] + Vector3.up * 0.2f, Color.magenta, 1000f);
		}
		this.surfacesToHideOn = list.ToArray();
	}

	// Token: 0x060007EC RID: 2028 RVA: 0x0002B532 File Offset: 0x00029732
	private IEnumerator GetNewPos()
	{
		for (;;)
		{
			this.m_CurrentHidePos = this.GetPosAwayFrom(this.api.OtherPlayerPosition());
			yield return new WaitForSeconds(4f);
		}
		yield break;
	}

	// Token: 0x060007ED RID: 2029 RVA: 0x0002B544 File Offset: 0x00029744
	public Vector3 GetPosAwayFrom(Vector3 point)
	{
		Vector3 vector;
		do
		{
			vector = this.surfacesToHideOn[Random.Range(0, this.surfacesToHideOn.Length)];
		}
		while (Mathf.Abs(vector.x - point.x) <= 13f);
		return vector;
	}

	// Token: 0x0400093F RID: 2367
	public AnimationCurve m_AimCompensastionCurve;

	// Token: 0x04000940 RID: 2368
	public LayerMask m_MapMask;

	// Token: 0x04000941 RID: 2369
	public LayerMask m_PlayerMask;

	// Token: 0x04000942 RID: 2370
	private PlayerAPI api;

	// Token: 0x04000943 RID: 2371
	private Camera m_camera;

	// Token: 0x04000944 RID: 2372
	private int framesSinceShot;

	// Token: 0x04000945 RID: 2373
	private Vector3[] surfacesToHideOn;

	// Token: 0x04000946 RID: 2374
	private Vector3 m_CurrentHidePos;

	// Token: 0x04000947 RID: 2375
	private float tiemSpentOverDeath;

	// Token: 0x04000948 RID: 2376
	private float timeSinceCouldShoot;
}
