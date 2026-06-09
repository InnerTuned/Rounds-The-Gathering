using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

// Token: 0x02000148 RID: 328
[ExecuteInEditMode]
public class SFRenderer : MonoBehaviour
{
	// Token: 0x06000675 RID: 1653 RVA: 0x00024298 File Offset: 0x00022498
	private void ScenePreRender(Camera camera)
	{
		if (this._renderInSceneView && camera.cameraType == CameraType.SceneView)
		{
			this.OnPreRender();
		}
	}

	// Token: 0x06000676 RID: 1654 RVA: 0x000242B1 File Offset: 0x000224B1
	private void ScenePostRender(Camera camera)
	{
		if (this._renderInSceneView && camera.cameraType == CameraType.SceneView)
		{
			this.OnPostRender();
		}
	}

	// Token: 0x06000677 RID: 1655 RVA: 0x000242CC File Offset: 0x000224CC
	private void OnEnable()
	{
		if (Application.isEditor)
		{
			Camera.onPreRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPreRender, new Camera.CameraCallback(this.ScenePreRender));
			Camera.onPostRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPostRender, new Camera.CameraCallback(this.ScenePostRender));
		}
	}

	// Token: 0x06000678 RID: 1656 RVA: 0x00024320 File Offset: 0x00022520
	private void OnDisable()
	{
		if (Application.isEditor)
		{
			Camera.onPreRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPreRender, new Camera.CameraCallback(this.ScenePreRender));
			Camera.onPostRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPostRender, new Camera.CameraCallback(this.ScenePostRender));
		}
	}

	// Token: 0x17000024 RID: 36
	// (get) Token: 0x06000679 RID: 1657 RVA: 0x00024374 File Offset: 0x00022574
	// (set) Token: 0x0600067A RID: 1658 RVA: 0x0002437C File Offset: 0x0002257C
	public bool linearLightBlending
	{
		get
		{
			return this._linearLightBlending;
		}
		set
		{
			this._linearLightBlending = value;
		}
	}

	// Token: 0x17000025 RID: 37
	// (get) Token: 0x0600067B RID: 1659 RVA: 0x00024385 File Offset: 0x00022585
	// (set) Token: 0x0600067C RID: 1660 RVA: 0x0002438D File Offset: 0x0002258D
	public bool shadows
	{
		get
		{
			return this._shadows;
		}
		set
		{
			this._shadows = value;
		}
	}

	// Token: 0x17000026 RID: 38
	// (get) Token: 0x0600067D RID: 1661 RVA: 0x00024396 File Offset: 0x00022596
	// (set) Token: 0x0600067E RID: 1662 RVA: 0x0002439E File Offset: 0x0002259E
	public Color ambientLight
	{
		get
		{
			return this._ambientLight;
		}
		set
		{
			this._ambientLight = value;
		}
	}

	// Token: 0x17000027 RID: 39
	// (get) Token: 0x0600067F RID: 1663 RVA: 0x000243A7 File Offset: 0x000225A7
	// (set) Token: 0x06000680 RID: 1664 RVA: 0x000243AF File Offset: 0x000225AF
	public float exposure
	{
		get
		{
			return this._exposure;
		}
		set
		{
			this._exposure = value;
		}
	}

	// Token: 0x17000028 RID: 40
	// (get) Token: 0x06000681 RID: 1665 RVA: 0x000243B8 File Offset: 0x000225B8
	// (set) Token: 0x06000682 RID: 1666 RVA: 0x000243C0 File Offset: 0x000225C0
	public float lightMapScale
	{
		get
		{
			return this._lightMapScale;
		}
		set
		{
			this._lightMapScale = value;
		}
	}

	// Token: 0x17000029 RID: 41
	// (get) Token: 0x06000683 RID: 1667 RVA: 0x000243C9 File Offset: 0x000225C9
	// (set) Token: 0x06000684 RID: 1668 RVA: 0x000243D1 File Offset: 0x000225D1
	public float shadowMapScale
	{
		get
		{
			return this._shadowMapScale;
		}
		set
		{
			this._shadowMapScale = value;
		}
	}

	// Token: 0x1700002A RID: 42
	// (get) Token: 0x06000685 RID: 1669 RVA: 0x000243DA File Offset: 0x000225DA
	// (set) Token: 0x06000686 RID: 1670 RVA: 0x000243E2 File Offset: 0x000225E2
	public float minLightPenetration
	{
		get
		{
			return this._minLightPenetration;
		}
		set
		{
			this._minLightPenetration = value;
		}
	}

	// Token: 0x1700002B RID: 43
	// (get) Token: 0x06000687 RID: 1671 RVA: 0x000243EB File Offset: 0x000225EB
	// (set) Token: 0x06000688 RID: 1672 RVA: 0x000243F3 File Offset: 0x000225F3
	public float shadowCompensation
	{
		get
		{
			return this._shadowCompensation;
		}
		set
		{
			this._shadowCompensation = Mathf.Clamp(value, 1f, 2f);
		}
	}

	// Token: 0x1700002C RID: 44
	// (get) Token: 0x06000689 RID: 1673 RVA: 0x0002440B File Offset: 0x0002260B
	// (set) Token: 0x0600068A RID: 1674 RVA: 0x00024413 File Offset: 0x00022613
	public Color fogColor
	{
		get
		{
			return this._fogColor;
		}
		set
		{
			this._fogColor = value;
		}
	}

	// Token: 0x1700002D RID: 45
	// (get) Token: 0x0600068B RID: 1675 RVA: 0x0002441C File Offset: 0x0002261C
	// (set) Token: 0x0600068C RID: 1676 RVA: 0x00024424 File Offset: 0x00022624
	public Color scatterColor
	{
		get
		{
			return this._scatterColor;
		}
		set
		{
			this._scatterColor = value;
		}
	}

	// Token: 0x1700002E RID: 46
	// (get) Token: 0x0600068D RID: 1677 RVA: 0x0002442D File Offset: 0x0002262D
	// (set) Token: 0x0600068E RID: 1678 RVA: 0x00024435 File Offset: 0x00022635
	public float softHardMix
	{
		get
		{
			return this._softHardMix;
		}
		set
		{
			this._softHardMix = value;
		}
	}

	// Token: 0x1700002F RID: 47
	// (get) Token: 0x0600068F RID: 1679 RVA: 0x000243A7 File Offset: 0x000225A7
	// (set) Token: 0x06000690 RID: 1680 RVA: 0x000243AF File Offset: 0x000225AF
	[Obsolete("Please use SFRenderer.exposure instead.")]
	public float globalIlluminationScale
	{
		get
		{
			return this._exposure;
		}
		set
		{
			this._exposure = value;
		}
	}

	// Token: 0x17000030 RID: 48
	// (get) Token: 0x06000691 RID: 1681 RVA: 0x000243A7 File Offset: 0x000225A7
	// (set) Token: 0x06000692 RID: 1682 RVA: 0x000243AF File Offset: 0x000225AF
	[Obsolete("Please use SFRenderer.exposure instead.")]
	public float globalDynamicRange
	{
		get
		{
			return this._exposure;
		}
		set
		{
			this._exposure = value;
		}
	}

	// Token: 0x17000031 RID: 49
	// (get) Token: 0x06000693 RID: 1683 RVA: 0x0002443E File Offset: 0x0002263E
	// (set) Token: 0x06000694 RID: 1684 RVA: 0x00024446 File Offset: 0x00022646
	public Rect extents
	{
		get
		{
			return this._extents;
		}
		set
		{
			this._extents = value;
		}
	}

	// Token: 0x17000032 RID: 50
	// (get) Token: 0x06000695 RID: 1685 RVA: 0x0002444F File Offset: 0x0002264F
	private Material shadowMaskMaterial
	{
		get
		{
			if (this._shadowMaskMaterial == null)
			{
				this._shadowMaskMaterial = new Material(Shader.Find("Hidden/SFSoftShadows/ShadowMask"));
				this._shadowMaskMaterial.hideFlags = HideFlags.HideAndDontSave;
			}
			return this._shadowMaskMaterial;
		}
	}

	// Token: 0x17000033 RID: 51
	// (get) Token: 0x06000696 RID: 1686 RVA: 0x00024488 File Offset: 0x00022688
	private Material lightMaterial
	{
		get
		{
			if (this._linearLightMaterial == null)
			{
				this._linearLightMaterial = new Material(Shader.Find("Hidden/SFSoftShadows/LightBlendLinear"));
				this._linearLightMaterial.hideFlags = HideFlags.HideAndDontSave;
				this._softLightMaterial = new Material(Shader.Find("Hidden/SFSoftShadows/LightBlendSoft"));
				this._softLightMaterial.hideFlags = HideFlags.HideAndDontSave;
			}
			if (!this._linearLightBlending)
			{
				return this._softLightMaterial;
			}
			return this._linearLightMaterial;
		}
	}

	// Token: 0x17000034 RID: 52
	// (get) Token: 0x06000697 RID: 1687 RVA: 0x000244FC File Offset: 0x000226FC
	private Material HDRClampMaterial
	{
		get
		{
			if (this._HDRClampMaterial == null)
			{
				this._HDRClampMaterial = new Material(Shader.Find("Hidden/SFSoftShadows/HDRClamp"));
				this._HDRClampMaterial.hideFlags = HideFlags.HideAndDontSave;
			}
			return this._HDRClampMaterial;
		}
	}

	// Token: 0x17000035 RID: 53
	// (get) Token: 0x06000698 RID: 1688 RVA: 0x00024534 File Offset: 0x00022734
	private Material fogMaterial
	{
		get
		{
			if (this._fogMaterial == null)
			{
				this._fogMaterial = new Material(Shader.Find("Hidden/SFSoftShadows/FogLayer"));
				this._fogMaterial.hideFlags = HideFlags.HideAndDontSave;
			}
			return this._fogMaterial;
		}
	}

	// Token: 0x17000036 RID: 54
	// (get) Token: 0x06000699 RID: 1689 RVA: 0x0002456C File Offset: 0x0002276C
	private Mesh sharedMesh
	{
		get
		{
			if (this._mesh == null)
			{
				this._mesh = new Mesh();
				this._mesh.MarkDynamic();
				this._mesh.hideFlags = HideFlags.HideAndDontSave;
			}
			return this._mesh;
		}
	}

	// Token: 0x0600069A RID: 1690 RVA: 0x000245A8 File Offset: 0x000227A8
	private void Start()
	{
		GraphicsDeviceType graphicsDeviceType = SystemInfo.graphicsDeviceType;
		this.UV_STARTS_AT_TOP = (graphicsDeviceType == GraphicsDeviceType.Direct3D9 || graphicsDeviceType == GraphicsDeviceType.Direct3D11 || graphicsDeviceType == GraphicsDeviceType.Direct3D12 || graphicsDeviceType == GraphicsDeviceType.Metal);
		if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGB32))
		{
			this.lightmapFormat = RenderTextureFormat.ARGB32;
		}
		else if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.BGRA32))
		{
			this.lightmapFormat = RenderTextureFormat.BGRA32;
		}
		if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf))
		{
			this.lightmapFormatHDR = RenderTextureFormat.ARGBHalf;
		}
		else if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBFloat))
		{
			this.lightmapFormatHDR = RenderTextureFormat.ARGBFloat;
		}
		global::Debug.Log(string.Concat(new object[]
		{
			"SFSS init: ",
			graphicsDeviceType,
			", ",
			this.UV_STARTS_AT_TOP ? "UV_STARTS_AT_TOP, " : "",
			this.lightmapFormat,
			", ",
			this.lightmapFormatHDR
		}));
	}

	// Token: 0x0600069B RID: 1691 RVA: 0x0002467E File Offset: 0x0002287E
	private void OnDestroy()
	{
		if (this._mesh)
		{
			Object.DestroyImmediate(this._mesh);
		}
	}

	// Token: 0x0600069C RID: 1692 RVA: 0x00024698 File Offset: 0x00022898
	public static Rect _TransformRect(Matrix4x4 m, Rect r)
	{
		Vector4 vector = m.MultiplyPoint3x4(new Vector4(r.x + 0.5f * r.width, r.y + 0.5f * r.height, 0f, 1f));
		float num = 0.5f * Mathf.Max(Mathf.Abs(r.width * m[0] + r.height * m[4]), Mathf.Abs(r.width * m[0] - r.height * m[4]));
		float num2 = 0.5f * Mathf.Max(Mathf.Abs(r.width * m[1] + r.height * m[5]), Mathf.Abs(r.width * m[1] - r.height * m[5]));
		return new Rect(vector.x - num, vector.y - num2, 2f * num, 2f * num2);
	}

	// Token: 0x0600069D RID: 1693 RVA: 0x000247C4 File Offset: 0x000229C4
	private static Vector2 ClampedProjection(Matrix4x4 m, float x, float y)
	{
		float num = Math.Max(0f, x * m[3] + y * m[7] + m[15]);
		return new Vector2((x * m[0] + y * m[4] + m[12]) / num, (x * m[1] + y * m[5] + m[13]) / num);
	}

	// Token: 0x0600069E RID: 1694 RVA: 0x00024844 File Offset: 0x00022A44
	private static Rect ScissorRect(Matrix4x4 mvp, float w, float h)
	{
		Vector2 vector = SFRenderer.ClampedProjection(mvp, -1f, -1f);
		Vector2 vector2 = SFRenderer.ClampedProjection(mvp, 1f, -1f);
		Vector2 vector3 = SFRenderer.ClampedProjection(mvp, 1f, 1f);
		Vector2 vector4 = SFRenderer.ClampedProjection(mvp, -1f, 1f);
		float num = Mathf.Min(Mathf.Min(vector.x, vector2.x), Mathf.Min(vector3.x, vector4.x));
		float num2 = Mathf.Min(Mathf.Min(vector.y, vector2.y), Mathf.Min(vector3.y, vector4.y));
		float num3 = Mathf.Max(Mathf.Max(vector.x, vector2.x), Mathf.Max(vector3.x, vector4.x));
		float num4 = Mathf.Max(Mathf.Max(vector.y, vector2.y), Mathf.Max(vector3.y, vector4.y));
		return Rect.MinMaxRect(Mathf.Max(0f, Mathf.Floor((0.5f * num + 0.5f) * w)), Mathf.Max(0f, Mathf.Floor((0.5f * num2 + 0.5f) * h)), Mathf.Min(w, Mathf.Ceil((0.5f * num3 + 0.5f) * w)), Mathf.Min(h, Mathf.Ceil((0.5f * num4 + 0.5f) * h)));
	}

	// Token: 0x0600069F RID: 1695 RVA: 0x000249B0 File Offset: 0x00022BB0
	private static Matrix4x4 ClipMatrix(Rect r, float dw, float dh)
	{
		float num = r.x * dw - 1f;
		float num2 = r.y * dh - 1f;
		return Matrix4x4.Ortho(num, num + r.width * dw, num2, num2 + r.height * dh, -1f, 1f);
	}

	// Token: 0x060006A0 RID: 1696 RVA: 0x00024A04 File Offset: 0x00022C04
	private static void CullPolys(List<SFPolygon> polys, Rect bounds, List<SFPolygon> culledPolygons)
	{
		for (int i = 0; i < polys.Count; i++)
		{
			SFPolygon sfpolygon = polys[i];
			if (bounds.Overlaps(sfpolygon._WorldBounds))
			{
				culledPolygons.Add(sfpolygon);
			}
		}
	}

	// Token: 0x060006A1 RID: 1697 RVA: 0x00024A40 File Offset: 0x00022C40
	private Matrix4x4 TextureProjectionMatrix(Matrix4x4 m)
	{
		m.SetRow(2, new Vector4(0f, 0f, 1f, 0f));
		Matrix4x4 matrix4x = m.inverse;
		if (this.UV_STARTS_AT_TOP)
		{
			matrix4x = SFRenderer.TEXTURE_FLIP_MATRIX * matrix4x * SFRenderer.TEXTURE_FLIP_MATRIX;
		}
		return matrix4x;
	}

	// Token: 0x060006A2 RID: 1698 RVA: 0x00024A98 File Offset: 0x00022C98
	private void RenderLightMap(Matrix4x4 viewMatrix, Matrix4x4 projection, Matrix4x4 vpMatrix, RenderTexture target, List<SFLight> lights, List<SFPolygon> polys, Color ambient, bool hdr)
	{
		int width = target.width;
		int height = target.height;
		Rect screenRect = new Rect(-1f, -1f, 2f, 2f);
		Rect sourceRect = new Rect(0f, 0f, 1f, 1f);
		Graphics.SetRenderTarget(target);
		GL.Clear(false, true, ambient);
		for (int i = 0; i < lights.Count; i++)
		{
			SFLight sflight = lights[i];
			if (sflight.enabled)
			{
				Matrix4x4 rhs = sflight._ModelMatrix(false) * sflight._CookieMatrix();
				Rect rect = SFRenderer.ScissorRect(vpMatrix * rhs, (float)width, (float)height);
				Matrix4x4 matrix4x = SFRenderer.ClipMatrix(rect, 2f / (float)width, 2f / (float)height) * projection;
				GL.Viewport(rect);
				GL.LoadProjectionMatrix(matrix4x);
				if (polys != null && sflight._shadowLayers != 0)
				{
					SFRenderer.CullPolys(polys, sflight._CalcCullBounds(vpMatrix), this._perLightCulledPolygons);
					Mesh mesh = sflight._BuildShadowMesh(this.sharedMesh, this._perLightCulledPolygons, this._minLightPenetration);
					this._perLightCulledPolygons.Clear();
					if (mesh != null)
					{
						this.shadowMaskMaterial.SetPass(0);
						Graphics.DrawMeshNow(mesh, sflight._ModelMatrix(true));
						mesh.Clear();
					}
					if (hdr)
					{
						Graphics.DrawTexture(screenRect, Texture2D.blackTexture, this.HDRClampMaterial);
					}
				}
				Texture2D texture2D = sflight._cookieTexture;
				if (!texture2D)
				{
					texture2D = Texture2D.whiteTexture;
				}
				Material lightMaterial = this.lightMaterial;
				if (this._linearLightBlending)
				{
					lightMaterial.SetFloat("_intensity", sflight._intensity);
				}
				GL.LoadProjectionMatrix(this.TextureProjectionMatrix(matrix4x * viewMatrix * rhs));
				Graphics.DrawTexture(screenRect, texture2D, sourceRect, 0, 0, 0, 0, sflight._color, lightMaterial);
			}
		}
	}

	// Token: 0x060006A3 RID: 1699 RVA: 0x00024C80 File Offset: 0x00022E80
	private RenderTexture GetTexture(Camera cam, Matrix4x4 extensionInv, float downscale)
	{
		Vector4 vector = extensionInv * cam.pixelRect.size / downscale;
		RenderTextureFormat format = cam.allowHDR ? this.lightmapFormatHDR : this.lightmapFormat;
		return RenderTexture.GetTemporary((int)vector.x, (int)vector.y, 0, format);
	}

	// Token: 0x060006A4 RID: 1700 RVA: 0x00024CDC File Offset: 0x00022EDC
	public static bool _FastCull(Matrix4x4 mvp, Rect bounds)
	{
		Vector2 center = bounds.center;
		Vector2 vector = 0.5f * bounds.size;
		Vector4 vector2 = mvp * new Vector4(center.x, center.y, 0f, 1f);
		float num = vector.x * Mathf.Abs(mvp[0]) + vector.y * Mathf.Abs(mvp[4]);
		float num2 = vector.x * Mathf.Abs(mvp[1]) + vector.y * Mathf.Abs(mvp[5]);
		float num3 = vector.x * Mathf.Abs(mvp[2]) + vector.y * Mathf.Abs(mvp[6]);
		float num4 = Mathf.Max(0f, vector2.w + vector.x * Mathf.Abs(mvp[3]) + vector.y * Mathf.Abs(mvp[7]));
		return Mathf.Abs(vector2.x) - num < num4 && Mathf.Abs(vector2.y) - num2 < num4 && Mathf.Abs(vector2.z) - num3 < num4;
	}

	// Token: 0x060006A5 RID: 1701 RVA: 0x00024E18 File Offset: 0x00023018
	private static Rect CullLights(Matrix4x4 vpMatrix, List<SFLight> lights, List<SFLight> culledLights)
	{
		float num = float.PositiveInfinity;
		float num2 = float.PositiveInfinity;
		float num3 = float.NegativeInfinity;
		float num4 = float.NegativeInfinity;
		for (int i = 0; i < lights.Count; i++)
		{
			SFLight sflight = lights[i];
			if (SFRenderer._FastCull(vpMatrix * sflight._ModelMatrix(false), sflight._bounds))
			{
				culledLights.Add(sflight);
				if (sflight._shadowLayers != 0)
				{
					Rect rect = sflight._CalcCullBounds(vpMatrix);
					num = Mathf.Min(num, rect.xMin);
					num2 = Mathf.Min(num2, rect.yMin);
					num3 = Mathf.Max(num3, rect.xMax);
					num4 = Mathf.Max(num4, rect.yMax);
				}
			}
		}
		return Rect.MinMaxRect(num, num2, num3, num4);
	}

	// Token: 0x060006A6 RID: 1702 RVA: 0x00024EE0 File Offset: 0x000230E0
	private void OnPreRender()
	{
		Color ambientLight = this._ambientLight;
		ambientLight.a = 1f;
		Matrix4x4 lhs = Matrix4x4.Ortho(this._extents.xMin, this._extents.xMax, this._extents.yMin, this._extents.yMax, 1f, -1f);
		Matrix4x4 inverse = lhs.inverse;
		RenderBuffer activeColorBuffer = Graphics.activeColorBuffer;
		RenderBuffer activeDepthBuffer = Graphics.activeDepthBuffer;
		Camera current = Camera.current;
		bool allowHDR = current.allowHDR;
		Matrix4x4 worldToCameraMatrix = current.worldToCameraMatrix;
		Matrix4x4 matrix4x = lhs * current.projectionMatrix;
		Matrix4x4 vpMatrix = matrix4x * worldToCameraMatrix;
		List<SFLight> lights = SFLight._lights;
		List<SFPolygon> list = SFPolygon._polygons;
		if (!Application.isPlaying)
		{
			lights = new List<SFLight>(Enumerable.Where<SFLight>(Object.FindObjectsOfType<SFLight>(), (SFLight o) => o.isActiveAndEnabled));
			list = new List<SFPolygon>(Enumerable.Where<SFPolygon>(Object.FindObjectsOfType<SFPolygon>(), (SFPolygon o) => o.isActiveAndEnabled));
			foreach (SFPolygon sfpolygon in list)
			{
				sfpolygon._UpdateBounds();
			}
		}
		Rect bounds = SFRenderer.CullLights(vpMatrix, lights, this._culledLights);
		GL.PushMatrix();
		this._lightMap = this.GetTexture(current, inverse, this._lightMapScale);
		this.RenderLightMap(worldToCameraMatrix, matrix4x, vpMatrix, this._lightMap, this._culledLights, null, ambientLight, allowHDR);
		if (this._shadows)
		{
			for (int i = 0; i < list.Count; i++)
			{
				list[i]._CacheWorldBounds();
			}
			SFRenderer.CullPolys(list, bounds, this._culledPolygons);
			Shader.SetGlobalFloat("_SFShadowCompensation", this._shadowCompensation);
			this._ShadowMap = this.GetTexture(current, inverse, this._shadowMapScale);
			this.RenderLightMap(worldToCameraMatrix, matrix4x, vpMatrix, this._ShadowMap, this._culledLights, this._culledPolygons, ambientLight, allowHDR);
			this._culledPolygons.Clear();
		}
		GL.PopMatrix();
		this._culledLights.Clear();
		Graphics.SetRenderTarget(null);
		GL.Viewport(current.pixelRect);
		Shader.SetGlobalMatrix("_SFProjection", lhs * Camera.current.projectionMatrix);
		Shader.SetGlobalColor("_SFAmbientLight", ambientLight);
		Shader.SetGlobalFloat("_SFExposure", this._exposure);
		Shader.SetGlobalTexture("_SFLightMap", this._lightMap);
		Shader.SetGlobalTexture("_SFLightMapWithShadows", this._shadows ? this._ShadowMap : this._lightMap);
		Graphics.SetRenderTarget(activeColorBuffer, activeDepthBuffer);
	}

	// Token: 0x060006A7 RID: 1703 RVA: 0x000251A4 File Offset: 0x000233A4
	private void OnPostRender()
	{
		if (this._fogColor.a + this._scatterColor.r + this._scatterColor.g + this._scatterColor.b > 0f)
		{
			GL.PushMatrix();
			GL.LoadProjectionMatrix(Matrix4x4.identity);
			Color scatterColor = this._scatterColor;
			scatterColor.a = this._softHardMix;
			Material fogMaterial = this.fogMaterial;
			fogMaterial.SetColor("_FogColor", this._fogColor);
			fogMaterial.SetColor("_Scatter", scatterColor);
			fogMaterial.SetPass(0);
			Graphics.DrawTexture(new Rect(-1f, -1f, 2f, 2f), Texture2D.blackTexture, fogMaterial);
			GL.PopMatrix();
		}
		Shader.SetGlobalColor("_SFAmbientLight", Color.white);
		Shader.SetGlobalFloat("_SFExposure", 1f);
		Shader.SetGlobalTexture("_SFLightMap", Texture2D.whiteTexture);
		Shader.SetGlobalTexture("_SFLightMapWithShadows", Texture2D.whiteTexture);
		RenderTexture.ReleaseTemporary(this._lightMap);
		this._lightMap = null;
		RenderTexture.ReleaseTemporary(this._ShadowMap);
		this._ShadowMap = null;
	}

	// Token: 0x040007F4 RID: 2036
	public bool _renderInSceneView = true;

	// Token: 0x040007F5 RID: 2037
	private RenderTexture _lightMap;

	// Token: 0x040007F6 RID: 2038
	private RenderTexture _ShadowMap;

	// Token: 0x040007F7 RID: 2039
	[Tooltip("Blend the lights in linear space rather than gamma space. Nonlinear blending prevents oversaturation, but can cause draw order artifacts.")]
	public bool _linearLightBlending = true;

	// Token: 0x040007F8 RID: 2040
	public bool _shadows = true;

	// Token: 0x040007F9 RID: 2041
	[Tooltip("The global ambient light color- the ambient light is used to light your scene when no lights are affecting part of it. A darker grey, blue, or yellow is often a good place to start. Alpha unused. ")]
	public Color _ambientLight = Color.black;

	// Token: 0x040007FA RID: 2042
	[Tooltip("Exposure is a multiplier applied to all lights in this renderer. Use to adjust all your lights at once. Particularly useful if you're using HDR lighting, otherwise it can be used to cause oversaturation.")]
	[FormerlySerializedAs("_globalDynamicRange")]
	public float _exposure = 1f;

	// Token: 0x040007FB RID: 2043
	[Tooltip("Scale of the render texture for the colored lights. Larger numbers will give you blockier lights, but will run faster. Since lighting tends to be pretty diffuse, high numbers like 8 usually look good here. Recommended values are between 8 - 32.")]
	public float _lightMapScale = 8f;

	// Token: 0x040007FC RID: 2044
	[Tooltip("Scale of the render texture for the colored lights. Larger numbers will give you blockier shadows, but will run faster. Blocky shadows tend to look worse than blocky lights, so this should usually be lower than the light map scale. Recommended values are between 2 - 8. Less if you have a lot of sharp shadows.")]
	public float _shadowMapScale = 4f;

	// Token: 0x040007FD RID: 2045
	[Tooltip("How far will light penetrate into each shadow casting object. Makes it look like objects that are casting shadows are illuminated by the lights.")]
	public float _minLightPenetration = 0.2f;

	// Token: 0x040007FE RID: 2046
	[Tooltip("Extra darkening to apply to shadows to hide precision artifacts in the seams.")]
	public float _shadowCompensation = 1.01f;

	// Token: 0x040007FF RID: 2047
	[Tooltip("The color of the fog color. The alpha controls the fog's strength.")]
	public Color _fogColor = new Color(1f, 1f, 1f, 0f);

	// Token: 0x04000800 RID: 2048
	[Tooltip("The scatter color is the color that the fog will glow when it is lit. Alpha is unused. Black disables illumination effects on the fog.")]
	public Color _scatterColor = new Color(0f, 0f, 0f, 0f);

	// Token: 0x04000801 RID: 2049
	[Tooltip("What percentage of unshadowed/shadowed light should apply to the fog. At 1.0, your shadows will be fully applied to the scattered light in your fog.")]
	public float _softHardMix;

	// Token: 0x04000802 RID: 2050
	private Rect _extents = Rect.MinMaxRect(-1f, -1f, 1f, 1f);

	// Token: 0x04000803 RID: 2051
	private Material _shadowMaskMaterial;

	// Token: 0x04000804 RID: 2052
	private Material _linearLightMaterial;

	// Token: 0x04000805 RID: 2053
	private Material _softLightMaterial;

	// Token: 0x04000806 RID: 2054
	private Material _HDRClampMaterial;

	// Token: 0x04000807 RID: 2055
	private Material _fogMaterial;

	// Token: 0x04000808 RID: 2056
	private Mesh _mesh;

	// Token: 0x04000809 RID: 2057
	private bool UV_STARTS_AT_TOP;

	// Token: 0x0400080A RID: 2058
	private static Matrix4x4 TEXTURE_FLIP_MATRIX = Matrix4x4.Scale(new Vector3(1f, -1f, 1f));

	// Token: 0x0400080B RID: 2059
	private RenderTextureFormat lightmapFormat = RenderTextureFormat.ARGB1555;

	// Token: 0x0400080C RID: 2060
	private RenderTextureFormat lightmapFormatHDR = RenderTextureFormat.ARGB1555;

	// Token: 0x0400080D RID: 2061
	private List<SFPolygon> _perLightCulledPolygons = new List<SFPolygon>();

	// Token: 0x0400080E RID: 2062
	private List<SFLight> _culledLights = new List<SFLight>();

	// Token: 0x0400080F RID: 2063
	private List<SFPolygon> _culledPolygons = new List<SFPolygon>();
}
