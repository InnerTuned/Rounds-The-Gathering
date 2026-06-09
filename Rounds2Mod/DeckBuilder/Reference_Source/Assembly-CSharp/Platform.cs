using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000002 RID: 2
public class Platform
{
	// Token: 0x17000001 RID: 1
	// (get) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
	public List<BoxCollider2D> BoxColliders
	{
		get
		{
			return this.m_boxColliders;
		}
	}

	// Token: 0x17000002 RID: 2
	// (get) Token: 0x06000002 RID: 2 RVA: 0x00002058 File Offset: 0x00000258
	// (set) Token: 0x06000003 RID: 3 RVA: 0x00002060 File Offset: 0x00000260
	public List<Vector2> PlatformPoints
	{
		get
		{
			return this.m_platformPoints;
		}
		set
		{
			this.m_platformPoints = value;
		}
	}

	// Token: 0x17000003 RID: 3
	// (get) Token: 0x06000004 RID: 4 RVA: 0x00002069 File Offset: 0x00000269
	public List<Vector2> Edges
	{
		get
		{
			return this.m_edges;
		}
	}

	// Token: 0x17000004 RID: 4
	// (get) Token: 0x06000005 RID: 5 RVA: 0x00002071 File Offset: 0x00000271
	// (set) Token: 0x06000006 RID: 6 RVA: 0x00002079 File Offset: 0x00000279
	public Color Color { get; set; }

	// Token: 0x06000007 RID: 7 RVA: 0x00002084 File Offset: 0x00000284
	public Platform()
	{
		this.Color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
	}

	// Token: 0x06000008 RID: 8 RVA: 0x000020F8 File Offset: 0x000002F8
	public bool ContainsCollider(BoxCollider2D collider)
	{
		foreach (BoxCollider2D y in this.m_boxColliders)
		{
			if (collider == y)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000009 RID: 9 RVA: 0x00002154 File Offset: 0x00000354
	public void AddCollider(BoxCollider2D collider)
	{
		this.m_boxColliders.Add(collider);
	}

	// Token: 0x0600000A RID: 10 RVA: 0x00002162 File Offset: 0x00000362
	public void AddPlatformPoint(Vector2 point)
	{
		this.m_platformPoints.Add(point);
	}

	// Token: 0x0600000B RID: 11 RVA: 0x00002170 File Offset: 0x00000370
	public float GetClosestDistance(Vector2 position)
	{
		float num = float.MaxValue;
		for (int i = 0; i < this.m_edges.Count; i++)
		{
			float num2 = Vector2.Distance(position, this.m_edges[i]);
			if (num2 < num)
			{
				this.m_lastCalculatedClosestPoint = i;
				num = num2;
			}
		}
		return num;
	}

	// Token: 0x0600000C RID: 12 RVA: 0x000021BC File Offset: 0x000003BC
	public bool IsPositionOutsidePlatform(Vector2 position)
	{
		Vector2 vector = this.m_edges[this.m_lastCalculatedClosestPoint];
		return (position.x > vector.x && this.m_lastCalculatedClosestPoint == this.m_edges.Count - 1) || (position.x < vector.x && this.m_lastCalculatedClosestPoint == 0);
	}

	// Token: 0x0600000D RID: 13 RVA: 0x0000221C File Offset: 0x0000041C
	public Vector2 GetPointOnPath(Vector2 position)
	{
		Vector2 vector = this.m_edges[this.m_lastCalculatedClosestPoint];
		if (position.x > vector.x && this.m_lastCalculatedClosestPoint == this.m_edges.Count - 1)
		{
			return vector;
		}
		if (position.x < vector.x && this.m_lastCalculatedClosestPoint == 0)
		{
			return vector;
		}
		int num = -1;
		int num2 = 0;
		if (position.x > vector.x)
		{
			num = this.m_lastCalculatedClosestPoint + 1;
			num2 = 1;
		}
		else if (position.x <= vector.x)
		{
			num = this.m_lastCalculatedClosestPoint - 1;
			num2 = -1;
		}
		if (num2 == 1)
		{
			return Platform.GetClosestPointOnLineSegment(vector, this.m_edges[num], position);
		}
		return Platform.GetClosestPointOnLineSegment(this.m_edges[num], vector, position);
	}

	// Token: 0x0600000E RID: 14 RVA: 0x000022DC File Offset: 0x000004DC
	private Vector2 Project(Vector2 line1, Vector2 line2, Vector2 toProject)
	{
		float num = (line2.y - line1.y) / (line2.x - line1.x);
		float num2 = line1.y - num * line1.x;
		float x = (num * toProject.y + toProject.x - num * num2) / (num * num + 1f);
		float y = (num * num * toProject.y + num * toProject.x + num2) / (num * num + 1f);
		return new Vector2(x, y);
	}

	// Token: 0x0600000F RID: 15 RVA: 0x00002358 File Offset: 0x00000558
	public static Vector2 GetClosestPointOnLineSegment(Vector2 A, Vector2 B, Vector2 P)
	{
		Vector2 lhs = P - A;
		Vector2 vector = B - A;
		float sqrMagnitude = vector.sqrMagnitude;
		float num = Vector2.Dot(lhs, vector) / sqrMagnitude;
		if (num < 0f)
		{
			return A;
		}
		if (num > 1f)
		{
			return B;
		}
		return A + vector * num;
	}

	// Token: 0x06000010 RID: 16 RVA: 0x000023A8 File Offset: 0x000005A8
	public void PostProcessPlatformPoints()
	{
		int mask = LayerMask.GetMask(new string[]
		{
			"Default"
		});
		List<Vector2> list = new List<Vector2>(Enumerable.ToArray<Vector2>(Enumerable.OrderBy<Vector2, float>(this.m_platformPoints, (Vector2 v) => v.x)));
		new HashSet<int>();
		for (int i = list.Count - 1; i > 0; i--)
		{
			if (Physics2D.OverlapCircle(list[i] + new Vector2(0f, 0.25f), 0.2f, mask) != null)
			{
				list.RemoveAt(i);
			}
		}
		this.m_platformPoints = list;
		this.DetectEdges();
	}

	// Token: 0x06000011 RID: 17 RVA: 0x0000245C File Offset: 0x0000065C
	public void DetectEdges()
	{
		if (this.m_platformPoints.Count == 0)
		{
			return;
		}
		List<Vector2> list = new List<Vector2>();
		for (int i = 0; i < this.m_platformPoints.Count - 2; i++)
		{
			Vector2 vector = this.m_platformPoints[i];
			Vector2 vector2 = this.m_platformPoints[i + 1];
			if (i == 0)
			{
				list.Add(vector);
			}
			else if (i == this.m_platformPoints.Count - 3)
			{
				list.Add(vector2);
			}
			else if (vector2.y - vector.y > Mathf.Epsilon)
			{
				list.Add(vector);
				list.Add(vector2);
			}
		}
		this.m_edges = list;
	}

	// Token: 0x06000012 RID: 18 RVA: 0x00002500 File Offset: 0x00000700
	public void DrawGizmos()
	{
		Gizmos.color = this.Color;
		for (int i = 0; i < this.m_edges.Count; i++)
		{
			Gizmos.DrawSphere(this.m_edges[i], 0.2f);
		}
		for (int j = 0; j < this.m_edges.Count - 1; j++)
		{
			Gizmos.DrawLine(this.m_edges[j], this.m_edges[j + 1]);
		}
	}

	// Token: 0x04000001 RID: 1
	private List<BoxCollider2D> m_boxColliders = new List<BoxCollider2D>();

	// Token: 0x04000002 RID: 2
	private List<Vector2> m_platformPoints = new List<Vector2>();

	// Token: 0x04000003 RID: 3
	public List<Vector2> m_edges = new List<Vector2>();

	// Token: 0x04000005 RID: 5
	private static float EPSILON = Mathf.Epsilon * 10f;

	// Token: 0x04000006 RID: 6
	private int m_lastCalculatedClosestPoint = -1;
}
