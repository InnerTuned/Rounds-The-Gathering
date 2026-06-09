using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000146 RID: 326
[RequireComponent(typeof(RectTransform))]
public class SFLight : MonoBehaviour
{
	// Token: 0x17000014 RID: 20
	// (get) Token: 0x0600063A RID: 1594 RVA: 0x00023448 File Offset: 0x00021648
	// (set) Token: 0x0600063B RID: 1595 RVA: 0x00023450 File Offset: 0x00021650
	public float radius
	{
		get
		{
			return this._radius;
		}
		set
		{
			this._radius = value;
		}
	}

	// Token: 0x17000015 RID: 21
	// (get) Token: 0x0600063C RID: 1596 RVA: 0x00023459 File Offset: 0x00021659
	// (set) Token: 0x0600063D RID: 1597 RVA: 0x00023461 File Offset: 0x00021661
	public float intensity
	{
		get
		{
			return this._intensity;
		}
		set
		{
			this._intensity = value;
		}
	}

	// Token: 0x17000016 RID: 22
	// (get) Token: 0x0600063E RID: 1598 RVA: 0x0002346A File Offset: 0x0002166A
	// (set) Token: 0x0600063F RID: 1599 RVA: 0x00023472 File Offset: 0x00021672
	public Color color
	{
		get
		{
			return this._color;
		}
		set
		{
			this._color = value;
		}
	}

	// Token: 0x17000017 RID: 23
	// (get) Token: 0x06000640 RID: 1600 RVA: 0x0002347B File Offset: 0x0002167B
	// (set) Token: 0x06000641 RID: 1601 RVA: 0x00023483 File Offset: 0x00021683
	public Texture2D cookieTexture
	{
		get
		{
			return this._cookieTexture;
		}
		set
		{
			this._cookieTexture = value;
		}
	}

	// Token: 0x17000018 RID: 24
	// (get) Token: 0x06000642 RID: 1602 RVA: 0x0002348C File Offset: 0x0002168C
	// (set) Token: 0x06000643 RID: 1603 RVA: 0x00023494 File Offset: 0x00021694
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

	// Token: 0x17000019 RID: 25
	// (get) Token: 0x06000644 RID: 1604 RVA: 0x0002349D File Offset: 0x0002169D
	// (set) Token: 0x06000645 RID: 1605 RVA: 0x000234A5 File Offset: 0x000216A5
	public bool parallaxLight
	{
		get
		{
			return this._parallaxLight;
		}
		set
		{
			this._parallaxLight = value;
		}
	}

	// Token: 0x06000646 RID: 1606 RVA: 0x000234AE File Offset: 0x000216AE
	private void OnEnable()
	{
		SFLight._lights.Add(this);
	}

	// Token: 0x06000647 RID: 1607 RVA: 0x000234BB File Offset: 0x000216BB
	private void OnDisable()
	{
		SFLight._lights.Remove(this);
	}

	// Token: 0x1700001A RID: 26
	// (get) Token: 0x06000648 RID: 1608 RVA: 0x000234C9 File Offset: 0x000216C9
	public Rect _bounds
	{
		get
		{
			return this._rt.rect;
		}
	}

	// Token: 0x06000649 RID: 1609 RVA: 0x000234D8 File Offset: 0x000216D8
	public Matrix4x4 _ModelMatrix(bool forceProjection)
	{
		if (!this._rt)
		{
			this._rt = base.GetComponent<RectTransform>();
		}
		Matrix4x4 localToWorldMatrix = this._rt.localToWorldMatrix;
		if (!this._parallaxLight || forceProjection)
		{
			localToWorldMatrix.SetRow(2, new Vector4(0f, 0f, 1f, 0f));
		}
		return localToWorldMatrix;
	}

	// Token: 0x0600064A RID: 1610 RVA: 0x0002353C File Offset: 0x0002173C
	public Matrix4x4 _CookieMatrix()
	{
		if (!this._rt)
		{
			this._rt = base.GetComponent<RectTransform>();
		}
		Vector2 vector = this._rt.sizeDelta / 2f;
		Vector2 vector2 = Vector2.one - 2f * this._rt.pivot;
		Matrix4x4 identity = Matrix4x4.identity;
		identity.SetRow(0, new Vector4(vector.x, 0f, 0f, vector2.x * vector.x));
		identity.SetRow(1, new Vector4(0f, vector.y, 0f, vector2.y * vector.y));
		return identity;
	}

	// Token: 0x0600064B RID: 1611 RVA: 0x000235F4 File Offset: 0x000217F4
	private static Rect Union(Rect r1, Rect r2)
	{
		return Rect.MinMaxRect(Mathf.Min(r1.xMin, r2.xMin), Mathf.Min(r1.yMin, r2.yMin), Mathf.Max(r1.xMax, r2.xMax), Mathf.Max(r1.yMax, r2.yMax));
	}

	// Token: 0x0600064C RID: 1612 RVA: 0x00023652 File Offset: 0x00021852
	private static Rect QuadrantCull(Rect cull, Matrix4x4 mvp, Rect r)
	{
		if (!SFRenderer._FastCull(mvp, r))
		{
			return cull;
		}
		return SFLight.Union(cull, r);
	}

	// Token: 0x1700001B RID: 27
	// (get) Token: 0x0600064D RID: 1613 RVA: 0x00023666 File Offset: 0x00021866
	public Rect _CullBounds
	{
		get
		{
			return this._cullBounds;
		}
	}

	// Token: 0x0600064E RID: 1614 RVA: 0x00023670 File Offset: 0x00021870
	public Rect _CalcCullBounds(Matrix4x4 vpMatrix)
	{
		Matrix4x4 matrix4x = this._ModelMatrix(true);
		Matrix4x4 mvp = vpMatrix * matrix4x;
		Rect rect = new Rect(-this._radius, -this._radius, 2f * this._radius, 2f * this._radius);
		Rect rect2 = this._rt.rect;
		Rect rect3 = rect;
		rect3 = SFLight.QuadrantCull(rect3, mvp, Rect.MinMaxRect(rect2.xMin, rect2.yMin, rect.xMax, rect.yMax));
		rect3 = SFLight.QuadrantCull(rect3, mvp, Rect.MinMaxRect(rect.xMin, rect2.yMin, rect2.xMax, rect.yMax));
		rect3 = SFLight.QuadrantCull(rect3, mvp, Rect.MinMaxRect(rect2.xMin, rect.yMin, rect.xMax, rect2.yMax));
		rect3 = SFLight.QuadrantCull(rect3, mvp, Rect.MinMaxRect(rect.xMin, rect.yMin, rect2.xMax, rect2.yMax));
		return this._cullBounds = SFRenderer._TransformRect(matrix4x, rect3);
	}

	// Token: 0x0600064F RID: 1615 RVA: 0x00023788 File Offset: 0x00021988
	private SFLight.VertexArray GetVertexArray(int segments)
	{
		int num = 0;
		int num2 = 4;
		while (segments > num2)
		{
			num2 = num2 * SFLight.GROWTH_NUM / SFLight.GROWTH_DENOM;
			num++;
		}
		if (num >= SFLight.vertexArrays.Length)
		{
			global::Debug.LogError("SFSS: Maximum vertexes per light exceeded. (" + num2 + ")");
			return null;
		}
		if (SFLight.vertexArrays[num] == null)
		{
			SFLight.vertexArrays[num] = new SFLight.VertexArray(num2);
		}
		return SFLight.vertexArrays[num];
	}

	// Token: 0x06000650 RID: 1616 RVA: 0x000237F4 File Offset: 0x000219F4
	private void BatchPath(SFPolygon poly, int pathIndex, Matrix4x4 t, ref int j, bool flipped, Vector3 properties, Vector3[] verts, Vector4[] tangents, Vector2[] uvs, int[] tris)
	{
		Vector2[] path = poly.GetPath(pathIndex);
		int num;
		Vector2 vector;
		if (poly.looped)
		{
			num = 0;
			vector = SFLight.Transform(t, path[path.Length - 1]);
		}
		else
		{
			num = 1;
			vector = SFLight.Transform(t, path[0]);
		}
		for (int i = num; i < path.Length; i++)
		{
			Vector2 vector2 = SFLight.Transform(t, path[i]);
			Vector4 vector3 = flipped ? new Vector4(vector2.x, vector2.y, vector.x, vector.y) : new Vector4(vector.x, vector.y, vector2.x, vector2.y);
			verts[j * 4] = properties;
			tangents[j * 4] = vector3;
			uvs[j * 4] = new Vector2(0f, 0f);
			verts[j * 4 + 1] = properties;
			tangents[j * 4 + 1] = vector3;
			uvs[j * 4 + 1] = new Vector2(1f, 0f);
			verts[j * 4 + 2] = properties;
			tangents[j * 4 + 2] = vector3;
			uvs[j * 4 + 2] = new Vector2(0f, 1f);
			verts[j * 4 + 3] = properties;
			tangents[j * 4 + 3] = vector3;
			uvs[j * 4 + 3] = new Vector2(1f, 1f);
			tris[j * 6] = j * 4;
			tris[j * 6 + 1] = j * 4 + 1;
			tris[j * 6 + 2] = j * 4 + 2;
			tris[j * 6 + 3] = j * 4 + 1;
			tris[j * 6 + 4] = j * 4 + 3;
			tris[j * 6 + 5] = j * 4 + 2;
			j++;
			vector = vector2;
		}
	}

	// Token: 0x06000651 RID: 1617 RVA: 0x000239FC File Offset: 0x00021BFC
	public Mesh _BuildShadowMesh(Mesh mesh, List<SFPolygon> polys, float minLightPenetration)
	{
		int num = 0;
		for (int i = 0; i < polys.Count; i++)
		{
			if ((polys[i].shadowLayers & this.shadowLayers) != 0)
			{
				SFPolygon sfpolygon = polys[i];
				int activePath = sfpolygon.activePath;
				bool looped = sfpolygon.looped;
				if (activePath >= 0)
				{
					num += sfpolygon.GetPath(activePath).Length - (looped ? 0 : 1);
				}
				else
				{
					int pathCount = sfpolygon.pathCount;
					for (int j = 0; j < pathCount; j++)
					{
						num += sfpolygon.GetPath(j).Length - (looped ? 0 : 1);
					}
				}
			}
		}
		SFLight.VertexArray vertexArray = this.GetVertexArray(num);
		Vector3[] verts = vertexArray.verts;
		Vector4[] tangents = vertexArray.tangents;
		Vector2[] uvs = vertexArray.uvs;
		int[] tris = vertexArray.tris;
		if (num == 0 || vertexArray.capacity == 0)
		{
			return null;
		}
		if (!this._rt)
		{
			this._rt = base.GetComponent<RectTransform>();
		}
		Matrix4x4 worldToLocalMatrix = this._rt.worldToLocalMatrix;
		int num2 = 0;
		for (int k = 0; k < polys.Count; k++)
		{
			SFPolygon sfpolygon2 = polys[k];
			if ((sfpolygon2.shadowLayers & this.shadowLayers) != 0)
			{
				Matrix4x4 matrix4x = sfpolygon2._GetMatrix();
				Matrix4x4 t = worldToLocalMatrix * matrix4x;
				bool flipped = SFLight.Det2x3(matrix4x) < 0f;
				float y = Mathf.Max(sfpolygon2._lightPenetration, minLightPenetration);
				Vector3 properties = new Vector3(this._radius, y, sfpolygon2._opacity);
				int activePath2 = sfpolygon2.activePath;
				if (activePath2 >= 0)
				{
					this.BatchPath(sfpolygon2, activePath2, t, ref num2, flipped, properties, verts, tangents, uvs, tris);
				}
				else
				{
					int pathCount2 = sfpolygon2.pathCount;
					for (int l = 0; l < pathCount2; l++)
					{
						this.BatchPath(sfpolygon2, l, t, ref num2, flipped, properties, verts, tangents, uvs, tris);
					}
				}
			}
		}
		for (int m = 6 * num; m < 6 * vertexArray.size; m++)
		{
			vertexArray.tris[m] = 0;
		}
		mesh.vertices = verts;
		mesh.tangents = tangents;
		mesh.uv = uvs;
		mesh.triangles = tris;
		vertexArray.size = num;
		return mesh;
	}

	// Token: 0x06000652 RID: 1618 RVA: 0x00023C3B File Offset: 0x00021E3B
	private static float Det2x3(Matrix4x4 m)
	{
		return m[0] * m[5] - m[1] * m[4];
	}

	// Token: 0x06000653 RID: 1619 RVA: 0x00023C60 File Offset: 0x00021E60
	private static Vector2 Transform(Matrix4x4 m, Vector2 p)
	{
		return new Vector2(p.x * m[0] + p.y * m[4] + m[12], p.x * m[1] + p.y * m[5] + m[13]);
	}

	// Token: 0x040007DD RID: 2013
	[Tooltip("The radius of the light source. Larger lights cast softer shadows.")]
	public float _radius = 0.5f;

	// Token: 0x040007DE RID: 2014
	[Tooltip("The brightness of the light. (Ignored when using non-linear light blending.) Allows for colors brighter than 1.0 in HDR lighting situations.")]
	public float _intensity = 1f;

	// Token: 0x040007DF RID: 2015
	[Tooltip("The color of the light.")]
	public Color _color = Color.white;

	// Token: 0x040007E0 RID: 2016
	[Tooltip("The shape of the light.")]
	public Texture2D _cookieTexture;

	// Token: 0x040007E1 RID: 2017
	[Tooltip("Which layers cast shadows.")]
	public LayerMask _shadowLayers = -1;

	// Token: 0x040007E2 RID: 2018
	[Tooltip("Allows the light cookie to move off the light plane. Use gently as it can cause shadows to look weird.")]
	public bool _parallaxLight;

	// Token: 0x040007E3 RID: 2019
	private RectTransform _rt;

	// Token: 0x040007E4 RID: 2020
	public static List<SFLight> _lights = new List<SFLight>();

	// Token: 0x040007E5 RID: 2021
	private Rect _cullBounds;

	// Token: 0x040007E6 RID: 2022
	private static int GROWTH_NUM = 4;

	// Token: 0x040007E7 RID: 2023
	private static int GROWTH_DENOM = 3;

	// Token: 0x040007E8 RID: 2024
	private static SFLight.VertexArray[] vertexArrays = new SFLight.VertexArray[40];

	// Token: 0x0200038B RID: 907
	private class VertexArray
	{
		// Token: 0x060012FE RID: 4862 RVA: 0x00057FB8 File Offset: 0x000561B8
		public VertexArray(int segments)
		{
			this.capacity = segments;
			this.size = 0;
			this.verts = new Vector3[segments * 4];
			this.tangents = new Vector4[segments * 4];
			this.uvs = new Vector2[segments * 4];
			this.tris = new int[segments * 6];
		}

		// Token: 0x040011F6 RID: 4598
		public int capacity;

		// Token: 0x040011F7 RID: 4599
		public int size;

		// Token: 0x040011F8 RID: 4600
		public Vector3[] verts;

		// Token: 0x040011F9 RID: 4601
		public Vector4[] tangents;

		// Token: 0x040011FA RID: 4602
		public Vector2[] uvs;

		// Token: 0x040011FB RID: 4603
		public int[] tris;
	}
}
