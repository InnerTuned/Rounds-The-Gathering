using System;
using AmplifyColor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;

// Token: 0x0200013F RID: 319
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("")]
public class AmplifyColorRenderMaskBase : MonoBehaviour
{
	// Token: 0x06000622 RID: 1570 RVA: 0x00022B84 File Offset: 0x00020D84
	private void OnEnable()
	{
		if (this.maskCamera == null)
		{
			GameObject gameObject = new GameObject("Mask Camera", new Type[]
			{
				typeof(Camera)
			})
			{
				hideFlags = HideFlags.HideAndDontSave
			};
			gameObject.transform.parent = base.gameObject.transform;
			this.maskCamera = gameObject.GetComponent<Camera>();
		}
		this.referenceCamera = base.GetComponent<Camera>();
		this.colorEffect = base.GetComponent<AmplifyColorBase>();
		this.colorMaskShader = Shader.Find("Hidden/RenderMask");
	}

	// Token: 0x06000623 RID: 1571 RVA: 0x00022C0F File Offset: 0x00020E0F
	private void OnDisable()
	{
		this.DestroyCamera();
		this.DestroyRenderTextures();
	}

	// Token: 0x06000624 RID: 1572 RVA: 0x00022C1D File Offset: 0x00020E1D
	private void DestroyCamera()
	{
		if (this.maskCamera != null)
		{
			Object.DestroyImmediate(this.maskCamera.gameObject);
			this.maskCamera = null;
		}
	}

	// Token: 0x06000625 RID: 1573 RVA: 0x00022C44 File Offset: 0x00020E44
	private void DestroyRenderTextures()
	{
		if (this.maskTexture != null)
		{
			RenderTexture.active = null;
			Object.DestroyImmediate(this.maskTexture);
			this.maskTexture = null;
		}
	}

	// Token: 0x06000626 RID: 1574 RVA: 0x00022C6C File Offset: 0x00020E6C
	private void UpdateRenderTextures(bool singlePassStereo)
	{
		int num = this.referenceCamera.pixelWidth;
		int num2 = this.referenceCamera.pixelHeight;
		if (this.maskTexture == null || this.width != num || this.height != num2 || !this.maskTexture.IsCreated() || this.singlePassStereo != singlePassStereo)
		{
			this.width = num;
			this.height = num2;
			this.DestroyRenderTextures();
			if (XRSettings.enabled)
			{
				num = XRSettings.eyeTextureWidth * (singlePassStereo ? 2 : 1);
				num2 = XRSettings.eyeTextureHeight;
			}
			if (this.maskTexture == null)
			{
				this.maskTexture = new RenderTexture(num, num2, 24, RenderTextureFormat.Default, RenderTextureReadWrite.sRGB)
				{
					hideFlags = HideFlags.HideAndDontSave,
					name = "MaskTexture"
				};
				this.maskTexture.name = "AmplifyColorMaskTexture";
				bool allowMSAA = this.maskCamera.allowMSAA;
				this.maskTexture.antiAliasing = ((allowMSAA && QualitySettings.antiAliasing > 0) ? QualitySettings.antiAliasing : 1);
			}
			this.maskTexture.Create();
			this.singlePassStereo = singlePassStereo;
		}
		if (this.colorEffect != null)
		{
			this.colorEffect.MaskTexture = this.maskTexture;
		}
	}

	// Token: 0x06000627 RID: 1575 RVA: 0x00022D98 File Offset: 0x00020F98
	private void UpdateCameraProperties()
	{
		this.maskCamera.CopyFrom(this.referenceCamera);
		this.maskCamera.targetTexture = this.maskTexture;
		this.maskCamera.clearFlags = CameraClearFlags.Nothing;
		this.maskCamera.renderingPath = RenderingPath.VertexLit;
		this.maskCamera.pixelRect = new Rect(0f, 0f, (float)this.width, (float)this.height);
		this.maskCamera.depthTextureMode = DepthTextureMode.None;
		this.maskCamera.allowHDR = false;
		this.maskCamera.enabled = false;
	}

	// Token: 0x06000628 RID: 1576 RVA: 0x00022E2C File Offset: 0x0002102C
	private void OnPreRender()
	{
		if (this.maskCamera != null)
		{
			RenderBuffer activeColorBuffer = Graphics.activeColorBuffer;
			RenderBuffer activeDepthBuffer = Graphics.activeDepthBuffer;
			bool flag = false;
			if (this.referenceCamera.stereoEnabled)
			{
				flag = (XRSettings.eyeTextureDesc.vrUsage == VRTextureUsage.TwoEyes);
				this.maskCamera.SetStereoViewMatrix(Camera.StereoscopicEye.Left, this.referenceCamera.GetStereoViewMatrix(Camera.StereoscopicEye.Left));
				this.maskCamera.SetStereoViewMatrix(Camera.StereoscopicEye.Right, this.referenceCamera.GetStereoViewMatrix(Camera.StereoscopicEye.Right));
				this.maskCamera.SetStereoProjectionMatrix(Camera.StereoscopicEye.Left, this.referenceCamera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left));
				this.maskCamera.SetStereoProjectionMatrix(Camera.StereoscopicEye.Right, this.referenceCamera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right));
			}
			this.UpdateRenderTextures(flag);
			this.UpdateCameraProperties();
			Graphics.SetRenderTarget(this.maskTexture);
			GL.Clear(true, true, this.ClearColor);
			if (flag)
			{
				this.maskCamera.worldToCameraMatrix = this.referenceCamera.GetStereoViewMatrix(Camera.StereoscopicEye.Left);
				this.maskCamera.projectionMatrix = this.referenceCamera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
				this.maskCamera.rect = new Rect(0f, 0f, 0.5f, 1f);
			}
			foreach (RenderLayer renderLayer in this.RenderLayers)
			{
				Shader.SetGlobalColor("_COLORMASK_Color", renderLayer.color);
				this.maskCamera.cullingMask = renderLayer.mask;
				this.maskCamera.RenderWithShader(this.colorMaskShader, "RenderType");
			}
			if (flag)
			{
				this.maskCamera.worldToCameraMatrix = this.referenceCamera.GetStereoViewMatrix(Camera.StereoscopicEye.Right);
				this.maskCamera.projectionMatrix = this.referenceCamera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
				this.maskCamera.rect = new Rect(0.5f, 0f, 0.5f, 1f);
				foreach (RenderLayer renderLayer2 in this.RenderLayers)
				{
					Shader.SetGlobalColor("_COLORMASK_Color", renderLayer2.color);
					this.maskCamera.cullingMask = renderLayer2.mask;
					this.maskCamera.RenderWithShader(this.colorMaskShader, "RenderType");
				}
			}
			Graphics.SetRenderTarget(activeColorBuffer, activeDepthBuffer);
		}
	}

	// Token: 0x040007C6 RID: 1990
	[FormerlySerializedAs("clearColor")]
	public Color ClearColor = Color.black;

	// Token: 0x040007C7 RID: 1991
	[FormerlySerializedAs("renderLayers")]
	public RenderLayer[] RenderLayers = new RenderLayer[0];

	// Token: 0x040007C8 RID: 1992
	[FormerlySerializedAs("debug")]
	public bool DebugMask;

	// Token: 0x040007C9 RID: 1993
	private Camera referenceCamera;

	// Token: 0x040007CA RID: 1994
	private Camera maskCamera;

	// Token: 0x040007CB RID: 1995
	private AmplifyColorBase colorEffect;

	// Token: 0x040007CC RID: 1996
	private int width;

	// Token: 0x040007CD RID: 1997
	private int height;

	// Token: 0x040007CE RID: 1998
	private RenderTexture maskTexture;

	// Token: 0x040007CF RID: 1999
	private Shader colorMaskShader;

	// Token: 0x040007D0 RID: 2000
	private bool singlePassStereo;
}
