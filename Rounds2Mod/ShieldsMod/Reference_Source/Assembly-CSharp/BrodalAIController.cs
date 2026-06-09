using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000003 RID: 3
public class BrodalAIController : MonoBehaviour
{
	// Token: 0x06000014 RID: 20 RVA: 0x0000259C File Offset: 0x0000079C
	private void Start()
	{
		this.m_playerAPI = base.GetComponentInParent<PlayerAPI>();
		HealthHandler healthHandler = this.m_playerAPI.player.data.healthHandler;
		healthHandler.delayedReviveAction = (Action)Delegate.Combine(healthHandler.delayedReviveAction, new Action(this.Init));
	}

	// Token: 0x06000015 RID: 21 RVA: 0x000025EB File Offset: 0x000007EB
	public void Init()
	{
		this.m_platforms.Clear();
		this.m_currentPlatform = null;
		this.PlotPlatforms();
		this.RaycastMap();
		this.PostProcessPlatforms();
		this.inited = true;
		global::Debug.Log("Revived");
	}

	// Token: 0x06000016 RID: 22 RVA: 0x00002624 File Offset: 0x00000824
	private void PostProcessPlatforms()
	{
		foreach (KeyValuePair<Guid, Platform> keyValuePair in this.m_platforms)
		{
			keyValuePair.Value.PostProcessPlatformPoints();
		}
	}

	// Token: 0x06000017 RID: 23 RVA: 0x0000267C File Offset: 0x0000087C
	private void PlotPlatforms()
	{
		foreach (BoxCollider2D boxCollider2D in Object.FindObjectsOfType<BoxCollider2D>())
		{
			if (boxCollider2D.gameObject.layer != LayerMask.GetMask(new string[]
			{
				"Player"
			}))
			{
				bool flag = false;
				foreach (KeyValuePair<Guid, Platform> keyValuePair in this.m_platforms)
				{
					foreach (BoxCollider2D boxCollider2D2 in keyValuePair.Value.BoxColliders)
					{
						bool flag2 = false;
						if (boxCollider2D.bounds.Intersects(boxCollider2D2.bounds))
						{
							flag2 = true;
						}
						if (flag2)
						{
							this.m_platforms[keyValuePair.Key].AddCollider(boxCollider2D);
							flag = true;
							break;
						}
					}
					if (flag)
					{
						break;
					}
				}
				if (!flag)
				{
					Guid guid = Guid.NewGuid();
					this.m_platforms.Add(guid, new Platform());
					this.m_platforms[guid].AddCollider(boxCollider2D);
				}
			}
		}
	}

	// Token: 0x06000018 RID: 24 RVA: 0x000027C8 File Offset: 0x000009C8
	private void MergePlatforms()
	{
	}

	// Token: 0x06000019 RID: 25 RVA: 0x000027CC File Offset: 0x000009CC
	private void RaycastMap()
	{
		this.m_layerMask = LayerMask.GetMask(new string[]
		{
			"Default"
		});
		Camera component = MainCam.instance.transform.GetComponent<Camera>();
		Vector3 vector = component.ViewportToWorldPoint(new Vector2(0f, 1f));
		Vector3 vector2 = component.ViewportToWorldPoint(new Vector2(1f, 1f));
		this.m_sampleSize = (vector2.x - vector.x) / (float)this.m_sampleCount;
		for (float num = vector.x; num < vector2.x; num += this.m_sampleSize)
		{
			foreach (RaycastHit2D raycastHit2D in Physics2D.RaycastAll(new Vector3(num, vector.y, 0f), Vector2.down, 9999f, this.m_layerMask))
			{
				foreach (KeyValuePair<Guid, Platform> keyValuePair in this.m_platforms)
				{
					if (keyValuePair.Value.ContainsCollider((BoxCollider2D)raycastHit2D.collider))
					{
						this.m_platforms[keyValuePair.Key].AddPlatformPoint(raycastHit2D.point);
						break;
					}
				}
			}
		}
	}

	// Token: 0x0600001A RID: 26 RVA: 0x00002944 File Offset: 0x00000B44
	private void Update()
	{
		if (!this.inited)
		{
			return;
		}
		this.FindClosestPlatform(base.transform.position);
		Vector2 vector = this.m_playerAPI.OtherPlayerPosition() - base.transform.position;
		float magnitude = vector.magnitude;
		vector.Normalize();
		Vector2 normalized = (vector + new Vector2(0f, 0.15f)).normalized;
		this.m_playerAPI.SetAimDirection(normalized);
		Vector2 position = new Vector2(base.transform.position.x, base.transform.position.y) + vector * 1.5f;
		bool flag = this.m_currentPlatform.IsPositionOutsidePlatform(position);
		this.m_playerAPI.Move(this.m_playerAPI.TowardsOtherPlayer());
		if (flag)
		{
			this.m_playerAPI.Jump();
		}
		vector.y = 0f;
		vector.Normalize();
		RaycastHit2D raycastHit2D = Physics2D.Raycast(base.transform.position, vector, 0.85f, this.m_layerMask);
		RaycastHit2D raycastHit2D2 = Physics2D.Raycast(base.transform.position, vector, magnitude, this.m_layerMask);
		if (raycastHit2D.collider)
		{
			this.m_playerAPI.Jump();
		}
		if (raycastHit2D2.collider == null)
		{
			this.m_playerAPI.Attack();
		}
		this.m_playerAPI.Block();
	}

	// Token: 0x0600001B RID: 27 RVA: 0x00002AD0 File Offset: 0x00000CD0
	private void FindClosestPlatform(Vector2 position)
	{
		float num = float.MaxValue;
		foreach (KeyValuePair<Guid, Platform> keyValuePair in this.m_platforms)
		{
			float closestDistance = keyValuePair.Value.GetClosestDistance(position);
			if (closestDistance < num)
			{
				this.m_currentPlatform = keyValuePair.Value;
				num = closestDistance;
			}
		}
	}

	// Token: 0x04000007 RID: 7
	private PlayerAPI m_playerAPI;

	// Token: 0x04000008 RID: 8
	private int m_sampleCount = 250;

	// Token: 0x04000009 RID: 9
	private float m_sampleSize;

	// Token: 0x0400000A RID: 10
	private LayerMask m_layerMask;

	// Token: 0x0400000B RID: 11
	private Dictionary<Guid, Platform> m_platforms = new Dictionary<Guid, Platform>();

	// Token: 0x0400000C RID: 12
	private Platform m_currentPlatform;

	// Token: 0x0400000D RID: 13
	private Vector2 m_pointOnLine;

	// Token: 0x0400000E RID: 14
	private bool inited;
}
