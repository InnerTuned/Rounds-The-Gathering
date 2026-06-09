using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000147 RID: 327
public class SFPolygon : MonoBehaviour
{
	// Token: 0x06000656 RID: 1622 RVA: 0x00023D1D File Offset: 0x00021F1D
	public Matrix4x4 _GetMatrix()
	{
		if (!this._t)
		{
			this._t = base.transform;
		}
		return this._t.localToWorldMatrix;
	}

	// Token: 0x06000657 RID: 1623 RVA: 0x00023D44 File Offset: 0x00021F44
	private void PathBounds(Vector2[] path, int i0, ref float l, ref float b, ref float r, ref float t)
	{
		for (int j = i0; j < path.Length; j++)
		{
			Vector2 vector = path[j];
			l = Mathf.Min(vector.x, l);
			r = Mathf.Max(vector.x, r);
			b = Mathf.Min(vector.y, b);
			t = Mathf.Max(vector.y, t);
		}
	}

	// Token: 0x06000658 RID: 1624 RVA: 0x00023DAC File Offset: 0x00021FAC
	public void _UpdateBounds()
	{
		float x;
		float xmax;
		float y;
		float ymax;
		if (this._activePath > 0)
		{
			Vector2 vector = this.GetPath(this._activePath)[0];
			xmax = (x = vector.x);
			ymax = (y = vector.y);
			this.PathBounds(this._verts, 1, ref x, ref y, ref xmax, ref ymax);
		}
		else
		{
			Vector2 vector2 = this._verts[0];
			xmax = (x = vector2.x);
			ymax = (y = vector2.y);
			this.PathBounds(this._verts, 1, ref x, ref y, ref xmax, ref ymax);
			int pathCount = this.pathCount;
			for (int i = 1; i < pathCount; i++)
			{
				this.PathBounds(this.GetPath(i), 0, ref x, ref y, ref xmax, ref ymax);
			}
		}
		this._bounds = Rect.MinMaxRect(x, y, xmax, ymax);
	}

	// Token: 0x1700001C RID: 28
	// (get) Token: 0x06000659 RID: 1625 RVA: 0x00023E6D File Offset: 0x0002206D
	public Rect _WorldBounds
	{
		get
		{
			return this._worldBounds;
		}
	}

	// Token: 0x0600065A RID: 1626 RVA: 0x00023E75 File Offset: 0x00022075
	public void _CacheWorldBounds()
	{
		if (!this._t)
		{
			this._t = base.transform;
		}
		this._worldBounds = SFRenderer._TransformRect(this._t.localToWorldMatrix, this._bounds);
	}

	// Token: 0x1700001D RID: 29
	// (get) Token: 0x0600065B RID: 1627 RVA: 0x00023EAC File Offset: 0x000220AC
	// (set) Token: 0x0600065C RID: 1628 RVA: 0x00023EC4 File Offset: 0x000220C4
	public int pathCount
	{
		get
		{
			if (this._paths != null)
			{
				return this._paths.Length + 1;
			}
			return 1;
		}
		set
		{
			int num = value - 1;
			if (value == this.pathCount)
			{
				return;
			}
			if (value < 1)
			{
				global::Debug.LogError("pathCount must be positive.");
				return;
			}
			if (value == 1)
			{
				this._paths = null;
			}
			else if (this._paths == null)
			{
				this._paths = new Vector2[num][];
				for (int i = 0; i < this._paths.Length; i++)
				{
					this._paths[i] = this._verts;
				}
			}
			else
			{
				Vector2[][] paths = this._paths;
				this._paths = new Vector2[num][];
				if (num > paths.Length)
				{
					int j;
					for (j = 0; j < paths.Length; j++)
					{
						this._paths[j] = paths[j];
					}
					while (j < num)
					{
						this._paths[j] = this._verts;
						j++;
					}
				}
				else
				{
					for (int k = 0; k < num; k++)
					{
						this._paths[k] = paths[k];
					}
				}
			}
			this._UpdateBounds();
		}
	}

	// Token: 0x1700001E RID: 30
	// (get) Token: 0x0600065D RID: 1629 RVA: 0x00023FA4 File Offset: 0x000221A4
	// (set) Token: 0x0600065E RID: 1630 RVA: 0x00023FAC File Offset: 0x000221AC
	public Vector2[] verts
	{
		get
		{
			return this._verts;
		}
		set
		{
			this._verts = value;
		}
	}

	// Token: 0x0600065F RID: 1631 RVA: 0x00023FB5 File Offset: 0x000221B5
	public Vector2[] GetPath(int index)
	{
		if (index != 0)
		{
			return this._paths[index - 1];
		}
		return this._verts;
	}

	// Token: 0x06000660 RID: 1632 RVA: 0x00023FCB File Offset: 0x000221CB
	public void SetPath(int index, Vector2[] path)
	{
		this.SetPathRaw(index, path);
		this._UpdateBounds();
	}

	// Token: 0x06000661 RID: 1633 RVA: 0x00023FDB File Offset: 0x000221DB
	private void SetPathRaw(int index, Vector2[] path)
	{
		if (index == 0)
		{
			this._verts = path;
			return;
		}
		this._paths[index - 1] = path;
	}

	// Token: 0x1700001F RID: 31
	// (get) Token: 0x06000662 RID: 1634 RVA: 0x00023FF3 File Offset: 0x000221F3
	// (set) Token: 0x06000663 RID: 1635 RVA: 0x00023FFB File Offset: 0x000221FB
	public int activePath
	{
		get
		{
			return this._activePath;
		}
		set
		{
			this._activePath = value;
		}
	}

	// Token: 0x17000020 RID: 32
	// (get) Token: 0x06000664 RID: 1636 RVA: 0x00024004 File Offset: 0x00022204
	// (set) Token: 0x06000665 RID: 1637 RVA: 0x0002400C File Offset: 0x0002220C
	public bool looped
	{
		get
		{
			return this._looped;
		}
		set
		{
			this._looped = value;
		}
	}

	// Token: 0x17000021 RID: 33
	// (get) Token: 0x06000666 RID: 1638 RVA: 0x00024015 File Offset: 0x00022215
	// (set) Token: 0x06000667 RID: 1639 RVA: 0x0002401D File Offset: 0x0002221D
	public LayerMask shadowLayers
	{
		get
		{
			return this._shadowLayers;
		}
		set
		{
			this._shadowLayers = value;
		}
	}

	// Token: 0x17000022 RID: 34
	// (get) Token: 0x06000668 RID: 1640 RVA: 0x00024026 File Offset: 0x00022226
	// (set) Token: 0x06000669 RID: 1641 RVA: 0x0002402E File Offset: 0x0002222E
	public float lightPenetration
	{
		get
		{
			return this._lightPenetration;
		}
		set
		{
			this._lightPenetration = value;
		}
	}

	// Token: 0x17000023 RID: 35
	// (get) Token: 0x0600066A RID: 1642 RVA: 0x00024037 File Offset: 0x00022237
	// (set) Token: 0x0600066B RID: 1643 RVA: 0x0002403F File Offset: 0x0002223F
	public float opacity
	{
		get
		{
			return this._opacity;
		}
		set
		{
			this._opacity = value;
		}
	}

	// Token: 0x0600066C RID: 1644 RVA: 0x00024048 File Offset: 0x00022248
	private void OnEnable()
	{
		SFPolygon._polygons.Add(this);
	}

	// Token: 0x0600066D RID: 1645 RVA: 0x00024055 File Offset: 0x00022255
	private void OnDisable()
	{
		SFPolygon._polygons.Remove(this);
	}

	// Token: 0x0600066E RID: 1646 RVA: 0x00024063 File Offset: 0x00022263
	private void Start()
	{
		this._UpdateBounds();
	}

	// Token: 0x0600066F RID: 1647 RVA: 0x0002406C File Offset: 0x0002226C
	public void CopyFromCollider(Collider2D collider)
	{
		PolygonCollider2D polygonCollider2D = collider as PolygonCollider2D;
		BoxCollider2D boxCollider2D = collider as BoxCollider2D;
		if (polygonCollider2D)
		{
			this.looped = true;
			int num = this.pathCount = polygonCollider2D.pathCount;
			for (int i = 0; i < num; i++)
			{
				Vector2[] path = polygonCollider2D.GetPath(i);
				for (int j = 0; j < path.Length; j++)
				{
					path[j] += polygonCollider2D.offset;
				}
				Array.Reverse(path);
				this.SetPathRaw(i, path);
			}
			this._UpdateBounds();
			return;
		}
		if (boxCollider2D)
		{
			this.SetBoxVerts(boxCollider2D.offset - 0.5f * boxCollider2D.size, boxCollider2D.offset + 0.5f * boxCollider2D.size);
			return;
		}
		global::Debug.LogWarning("CopyFromCollider() only works with polygon and box colliders.");
	}

	// Token: 0x06000670 RID: 1648 RVA: 0x00024158 File Offset: 0x00022358
	public void _CopyFromCollider()
	{
		Collider2D component = base.GetComponent<Collider2D>();
		if (component)
		{
			this.CopyFromCollider(component);
			return;
		}
		global::Debug.LogWarning("GameObject has no polygon or box collider. Adding default SFPolygon shape instead.");
		this.SetBoxVerts(-Vector2.one, Vector2.one);
	}

	// Token: 0x06000671 RID: 1649 RVA: 0x0002419C File Offset: 0x0002239C
	private void SetBoxVerts(Vector2 min, Vector2 max)
	{
		this.looped = true;
		this.pathCount = 1;
		this.verts = new Vector2[]
		{
			new Vector2(max.x, max.y),
			new Vector2(max.x, min.y),
			new Vector2(min.x, min.y),
			new Vector2(min.x, max.y)
		};
	}

	// Token: 0x06000672 RID: 1650 RVA: 0x00024224 File Offset: 0x00022424
	public void _FlipInsideOut(int index)
	{
		if (index == -1)
		{
			int pathCount = this.pathCount;
			for (int i = 0; i < pathCount; i++)
			{
				Array.Reverse(this.GetPath(i));
			}
			return;
		}
		Array.Reverse(this.GetPath(index));
	}

	// Token: 0x040007E9 RID: 2025
	private Transform _t;

	// Token: 0x040007EA RID: 2026
	private Rect _bounds;

	// Token: 0x040007EB RID: 2027
	private Rect _worldBounds;

	// Token: 0x040007EC RID: 2028
	[SerializeField]
	private Vector2[] _verts = new Vector2[3];

	// Token: 0x040007ED RID: 2029
	[SerializeField]
	private Vector2[][] _paths;

	// Token: 0x040007EE RID: 2030
	public int _activePath;

	// Token: 0x040007EF RID: 2031
	public bool _looped;

	// Token: 0x040007F0 RID: 2032
	public LayerMask _shadowLayers = -1;

	// Token: 0x040007F1 RID: 2033
	public float _lightPenetration;

	// Token: 0x040007F2 RID: 2034
	public float _opacity = 1f;

	// Token: 0x040007F3 RID: 2035
	public static List<SFPolygon> _polygons = new List<SFPolygon>();
}
