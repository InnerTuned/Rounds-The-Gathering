using System;
using System.Collections.Generic;
using AmplifyColor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

// Token: 0x0200013C RID: 316
[AddComponentMenu("")]
public class AmplifyColorBase : MonoBehaviour
{
	// Token: 0x1700000F RID: 15
	// (get) Token: 0x060005FA RID: 1530 RVA: 0x00021387 File Offset: 0x0001F587
	public Texture2D DefaultLut
	{
		get
		{
			if (!(this.defaultLut == null))
			{
				return this.defaultLut;
			}
			return this.CreateDefaultLut();
		}
	}

	// Token: 0x17000010 RID: 16
	// (get) Token: 0x060005FB RID: 1531 RVA: 0x000213A4 File Offset: 0x0001F5A4
	public bool IsBlending
	{
		get
		{
			return this.blending;
		}
	}

	// Token: 0x17000011 RID: 17
	// (get) Token: 0x060005FC RID: 1532 RVA: 0x000213AC File Offset: 0x0001F5AC
	private float effectVolumesBlendAdjusted
	{
		get
		{
			return Mathf.Clamp01((this.effectVolumesBlendAdjust < 0.99f) ? ((this.volumesBlendAmount - this.effectVolumesBlendAdjust) / (1f - this.effectVolumesBlendAdjust)) : 1f);
		}
	}

	// Token: 0x17000012 RID: 18
	// (get) Token: 0x060005FD RID: 1533 RVA: 0x000213E1 File Offset: 0x0001F5E1
	public string SharedInstanceID
	{
		get
		{
			return this.sharedInstanceID;
		}
	}

	// Token: 0x17000013 RID: 19
	// (get) Token: 0x060005FE RID: 1534 RVA: 0x000213E9 File Offset: 0x0001F5E9
	public bool WillItBlend
	{
		get
		{
			return this.LutTexture != null && this.LutBlendTexture != null && !this.blending;
		}
	}

	// Token: 0x060005FF RID: 1535 RVA: 0x00021414 File Offset: 0x0001F614
	public void NewSharedInstanceID()
	{
		this.sharedInstanceID = Guid.NewGuid().ToString();
	}

	// Token: 0x06000600 RID: 1536 RVA: 0x0002143A File Offset: 0x0001F63A
	private void ReportMissingShaders()
	{
		global::Debug.LogError("[AmplifyColor] Failed to initialize shaders. Please attempt to re-enable the Amplify Color Effect component. If that fails, please reinstall Amplify Color.");
		base.enabled = false;
	}

	// Token: 0x06000601 RID: 1537 RVA: 0x0002144D File Offset: 0x0001F64D
	private void ReportNotSupported()
	{
		global::Debug.LogError("[AmplifyColor] This image effect is not supported on this platform.");
		base.enabled = false;
	}

	// Token: 0x06000602 RID: 1538 RVA: 0x00021460 File Offset: 0x0001F660
	private bool CheckShader(Shader s)
	{
		if (s == null)
		{
			this.ReportMissingShaders();
			return false;
		}
		if (!s.isSupported)
		{
			this.ReportNotSupported();
			return false;
		}
		return true;
	}

	// Token: 0x06000603 RID: 1539 RVA: 0x00021484 File Offset: 0x0001F684
	private bool CheckShaders()
	{
		return this.CheckShader(this.shaderBase) && this.CheckShader(this.shaderBlend) && this.CheckShader(this.shaderBlendCache) && this.CheckShader(this.shaderMask) && this.CheckShader(this.shaderMaskBlend) && this.CheckShader(this.shaderProcessOnly);
	}

	// Token: 0x06000604 RID: 1540 RVA: 0x000214E5 File Offset: 0x0001F6E5
	private bool CheckSupport()
	{
		if (!SystemInfo.supportsImageEffects)
		{
			this.ReportNotSupported();
			return false;
		}
		return true;
	}

	// Token: 0x06000605 RID: 1541 RVA: 0x000214F8 File Offset: 0x0001F6F8
	private void OnEnable()
	{
		if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null)
		{
			global::Debug.LogWarning("[AmplifyColor] Null graphics device detected. Skipping effect silently.");
			this.silentError = true;
			return;
		}
		if (!this.CheckSupport())
		{
			return;
		}
		if (!this.CreateMaterials())
		{
			return;
		}
		Texture2D texture2D = this.LutTexture as Texture2D;
		Texture2D texture2D2 = this.LutBlendTexture as Texture2D;
		if ((texture2D != null && texture2D.mipmapCount > 1) || (texture2D2 != null && texture2D2.mipmapCount > 1))
		{
			global::Debug.LogError("[AmplifyColor] Please disable \"Generate Mip Maps\" import settings on all LUT textures to avoid visual glitches. Change Texture Type to \"Advanced\" to access Mip settings.");
		}
	}

	// Token: 0x06000606 RID: 1542 RVA: 0x00021579 File Offset: 0x0001F779
	private void OnDisable()
	{
		if (this.actualTriggerProxy != null)
		{
			Object.DestroyImmediate(this.actualTriggerProxy.gameObject);
			this.actualTriggerProxy = null;
		}
		this.ReleaseMaterials();
		this.ReleaseTextures();
	}

	// Token: 0x06000607 RID: 1543 RVA: 0x000215AC File Offset: 0x0001F7AC
	private void VolumesBlendTo(Texture blendTargetLUT, float blendTimeInSec)
	{
		this.volumesLutBlendTexture = blendTargetLUT;
		this.volumesBlendAmount = 0f;
		this.volumesBlendingTime = blendTimeInSec;
		this.volumesBlendingTimeCountdown = blendTimeInSec;
		this.volumesBlending = true;
	}

	// Token: 0x06000608 RID: 1544 RVA: 0x000215D5 File Offset: 0x0001F7D5
	public void BlendTo(Texture blendTargetLUT, float blendTimeInSec, Action onFinishBlend)
	{
		this.LutBlendTexture = blendTargetLUT;
		this.BlendAmount = 0f;
		this.onFinishBlend = onFinishBlend;
		this.blendingTime = blendTimeInSec;
		this.blendingTimeCountdown = blendTimeInSec;
		this.blending = true;
	}

	// Token: 0x06000609 RID: 1545 RVA: 0x00021608 File Offset: 0x0001F808
	private void CheckCamera()
	{
		if (this.ownerCamera == null)
		{
			this.ownerCamera = base.GetComponent<Camera>();
		}
		if (this.UseDepthMask && (this.ownerCamera.depthTextureMode & DepthTextureMode.Depth) == DepthTextureMode.None)
		{
			this.ownerCamera.depthTextureMode |= DepthTextureMode.Depth;
		}
	}

	// Token: 0x0600060A RID: 1546 RVA: 0x0002165C File Offset: 0x0001F85C
	private void Start()
	{
		if (this.silentError)
		{
			return;
		}
		this.CheckCamera();
		this.worldLUT = this.LutTexture;
		this.worldVolumeEffects = this.EffectFlags.GenerateEffectData(this);
		this.blendVolumeEffects = (this.currentVolumeEffects = this.worldVolumeEffects);
		this.worldExposure = this.Exposure;
		this.blendExposure = (this.currentExposure = this.worldExposure);
	}

	// Token: 0x0600060B RID: 1547 RVA: 0x000216CC File Offset: 0x0001F8CC
	private void Update()
	{
		if (this.silentError)
		{
			return;
		}
		this.CheckCamera();
		bool flag = false;
		if (this.volumesBlending)
		{
			this.volumesBlendAmount = (this.volumesBlendingTime - this.volumesBlendingTimeCountdown) / this.volumesBlendingTime;
			this.volumesBlendingTimeCountdown -= Time.smoothDeltaTime;
			if (this.volumesBlendAmount >= 1f)
			{
				this.volumesBlendAmount = 1f;
				flag = true;
			}
		}
		else
		{
			this.volumesBlendAmount = Mathf.Clamp01(this.volumesBlendAmount);
		}
		if (this.blending)
		{
			this.BlendAmount = (this.blendingTime - this.blendingTimeCountdown) / this.blendingTime;
			this.blendingTimeCountdown -= Time.smoothDeltaTime;
			if (this.BlendAmount >= 1f)
			{
				this.LutTexture = this.LutBlendTexture;
				this.BlendAmount = 0f;
				this.blending = false;
				this.LutBlendTexture = null;
				if (this.onFinishBlend != null)
				{
					this.onFinishBlend.Invoke();
				}
			}
		}
		else
		{
			this.BlendAmount = Mathf.Clamp01(this.BlendAmount);
		}
		if (this.UseVolumes)
		{
			if (this.actualTriggerProxy == null)
			{
				GameObject gameObject = new GameObject(base.name + "+ACVolumeProxy")
				{
					hideFlags = HideFlags.HideAndDontSave
				};
				if (this.TriggerVolumeProxy != null && this.TriggerVolumeProxy.GetComponent<Collider2D>() != null)
				{
					this.actualTriggerProxy = gameObject.AddComponent<AmplifyColorTriggerProxy2D>();
				}
				else
				{
					this.actualTriggerProxy = gameObject.AddComponent<AmplifyColorTriggerProxy>();
				}
				this.actualTriggerProxy.OwnerEffect = this;
			}
			this.UpdateVolumes();
		}
		else if (this.actualTriggerProxy != null)
		{
			Object.DestroyImmediate(this.actualTriggerProxy.gameObject);
			this.actualTriggerProxy = null;
		}
		if (flag)
		{
			this.LutTexture = this.volumesLutBlendTexture;
			this.volumesBlendAmount = 0f;
			this.volumesBlending = false;
			this.volumesLutBlendTexture = null;
			this.effectVolumesBlendAdjust = 0f;
			this.currentVolumeEffects = this.blendVolumeEffects;
			this.currentVolumeEffects.SetValues(this);
			this.currentExposure = this.blendExposure;
			if (this.blendingFromMidBlend && this.midBlendLUT != null)
			{
				this.midBlendLUT.DiscardContents();
			}
			this.blendingFromMidBlend = false;
		}
	}

	// Token: 0x0600060C RID: 1548 RVA: 0x000218FE File Offset: 0x0001FAFE
	public void EnterVolume(AmplifyColorVolumeBase volume)
	{
		if (!this.enteredVolumes.Contains(volume))
		{
			this.enteredVolumes.Insert(0, volume);
		}
	}

	// Token: 0x0600060D RID: 1549 RVA: 0x0002191B File Offset: 0x0001FB1B
	public void ExitVolume(AmplifyColorVolumeBase volume)
	{
		if (this.enteredVolumes.Contains(volume))
		{
			this.enteredVolumes.Remove(volume);
		}
	}

	// Token: 0x0600060E RID: 1550 RVA: 0x00021938 File Offset: 0x0001FB38
	private void UpdateVolumes()
	{
		if (this.volumesBlending)
		{
			this.currentVolumeEffects.BlendValues(this, this.blendVolumeEffects, this.effectVolumesBlendAdjusted);
		}
		if (this.volumesBlending)
		{
			this.Exposure = Mathf.Lerp(this.currentExposure, this.blendExposure, this.effectVolumesBlendAdjusted);
		}
		Transform transform = (this.TriggerVolumeProxy == null) ? base.transform : this.TriggerVolumeProxy;
		if (this.actualTriggerProxy.transform.parent != transform)
		{
			this.actualTriggerProxy.Reference = transform;
			this.actualTriggerProxy.gameObject.layer = transform.gameObject.layer;
		}
		AmplifyColorVolumeBase amplifyColorVolumeBase = null;
		int num = int.MinValue;
		for (int i = 0; i < this.enteredVolumes.Count; i++)
		{
			AmplifyColorVolumeBase amplifyColorVolumeBase2 = this.enteredVolumes[i];
			if (amplifyColorVolumeBase2.Priority > num)
			{
				amplifyColorVolumeBase = amplifyColorVolumeBase2;
				num = amplifyColorVolumeBase2.Priority;
			}
		}
		if (amplifyColorVolumeBase != this.currentVolumeLut)
		{
			this.currentVolumeLut = amplifyColorVolumeBase;
			Texture texture = (amplifyColorVolumeBase == null) ? this.worldLUT : amplifyColorVolumeBase.LutTexture;
			float num2 = (amplifyColorVolumeBase == null) ? this.ExitVolumeBlendTime : amplifyColorVolumeBase.EnterBlendTime;
			if (this.volumesBlending && !this.blendingFromMidBlend && texture == this.LutTexture)
			{
				this.LutTexture = this.volumesLutBlendTexture;
				this.volumesLutBlendTexture = texture;
				this.volumesBlendingTimeCountdown = num2 * ((this.volumesBlendingTime - this.volumesBlendingTimeCountdown) / this.volumesBlendingTime);
				this.volumesBlendingTime = num2;
				this.currentVolumeEffects = VolumeEffect.BlendValuesToVolumeEffect(this.EffectFlags, this.currentVolumeEffects, this.blendVolumeEffects, this.effectVolumesBlendAdjusted);
				this.currentExposure = Mathf.Lerp(this.currentExposure, this.blendExposure, this.effectVolumesBlendAdjusted);
				this.effectVolumesBlendAdjust = 1f - this.volumesBlendAmount;
				this.volumesBlendAmount = 1f - this.volumesBlendAmount;
			}
			else
			{
				if (this.volumesBlending)
				{
					this.materialBlendCache.SetFloat("_LerpAmount", this.volumesBlendAmount);
					if (this.blendingFromMidBlend)
					{
						Graphics.Blit(this.midBlendLUT, this.blendCacheLut);
						this.materialBlendCache.SetTexture("_RgbTex", this.blendCacheLut);
					}
					else
					{
						this.materialBlendCache.SetTexture("_RgbTex", this.LutTexture);
					}
					this.materialBlendCache.SetTexture("_LerpRgbTex", (this.volumesLutBlendTexture != null) ? this.volumesLutBlendTexture : this.defaultLut);
					Graphics.Blit(this.midBlendLUT, this.midBlendLUT, this.materialBlendCache);
					this.blendCacheLut.DiscardContents();
					this.currentVolumeEffects = VolumeEffect.BlendValuesToVolumeEffect(this.EffectFlags, this.currentVolumeEffects, this.blendVolumeEffects, this.effectVolumesBlendAdjusted);
					this.currentExposure = Mathf.Lerp(this.currentExposure, this.blendExposure, this.effectVolumesBlendAdjusted);
					this.effectVolumesBlendAdjust = 0f;
					this.blendingFromMidBlend = true;
				}
				this.VolumesBlendTo(texture, num2);
			}
			this.blendVolumeEffects = ((amplifyColorVolumeBase == null) ? this.worldVolumeEffects : amplifyColorVolumeBase.EffectContainer.FindVolumeEffect(this));
			this.blendExposure = ((amplifyColorVolumeBase == null) ? this.worldExposure : amplifyColorVolumeBase.Exposure);
			if (this.blendVolumeEffects == null)
			{
				this.blendVolumeEffects = this.worldVolumeEffects;
			}
		}
	}

	// Token: 0x0600060F RID: 1551 RVA: 0x00021CA4 File Offset: 0x0001FEA4
	private void SetupShader()
	{
		this.colorSpace = QualitySettings.activeColorSpace;
		this.qualityLevel = this.QualityLevel;
		this.shaderBase = Shader.Find("Hidden/Amplify Color/Base");
		this.shaderBlend = Shader.Find("Hidden/Amplify Color/Blend");
		this.shaderBlendCache = Shader.Find("Hidden/Amplify Color/BlendCache");
		this.shaderMask = Shader.Find("Hidden/Amplify Color/Mask");
		this.shaderMaskBlend = Shader.Find("Hidden/Amplify Color/MaskBlend");
		this.shaderDepthMask = Shader.Find("Hidden/Amplify Color/DepthMask");
		this.shaderDepthMaskBlend = Shader.Find("Hidden/Amplify Color/DepthMaskBlend");
		this.shaderProcessOnly = Shader.Find("Hidden/Amplify Color/ProcessOnly");
	}

	// Token: 0x06000610 RID: 1552 RVA: 0x00021D48 File Offset: 0x0001FF48
	private void ReleaseMaterials()
	{
		this.SafeRelease<Material>(ref this.materialBase);
		this.SafeRelease<Material>(ref this.materialBlend);
		this.SafeRelease<Material>(ref this.materialBlendCache);
		this.SafeRelease<Material>(ref this.materialMask);
		this.SafeRelease<Material>(ref this.materialMaskBlend);
		this.SafeRelease<Material>(ref this.materialDepthMask);
		this.SafeRelease<Material>(ref this.materialDepthMaskBlend);
		this.SafeRelease<Material>(ref this.materialProcessOnly);
	}

	// Token: 0x06000611 RID: 1553 RVA: 0x00021DB8 File Offset: 0x0001FFB8
	private Texture2D CreateDefaultLut()
	{
		this.defaultLut = new Texture2D(1024, 32, TextureFormat.RGB24, false, true)
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		this.defaultLut.name = "DefaultLut";
		this.defaultLut.hideFlags = HideFlags.DontSave;
		this.defaultLut.anisoLevel = 1;
		this.defaultLut.filterMode = FilterMode.Bilinear;
		Color32[] array = new Color32[32768];
		for (int i = 0; i < 32; i++)
		{
			int num = i * 32;
			for (int j = 0; j < 32; j++)
			{
				int num2 = num + j * 1024;
				for (int k = 0; k < 32; k++)
				{
					float num3 = (float)k / 31f;
					float num4 = (float)j / 31f;
					float num5 = (float)i / 31f;
					byte r = (byte)(num3 * 255f);
					byte g = (byte)(num4 * 255f);
					byte b = (byte)(num5 * 255f);
					array[num2 + k] = new Color32(r, g, b, byte.MaxValue);
				}
			}
		}
		this.defaultLut.SetPixels32(array);
		this.defaultLut.Apply();
		return this.defaultLut;
	}

	// Token: 0x06000612 RID: 1554 RVA: 0x00021ED8 File Offset: 0x000200D8
	private Texture2D CreateDepthCurveLut()
	{
		this.SafeRelease<Texture2D>(ref this.depthCurveLut);
		this.depthCurveLut = new Texture2D(1024, 1, TextureFormat.Alpha8, false, true)
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		this.depthCurveLut.name = "DepthCurveLut";
		this.depthCurveLut.hideFlags = HideFlags.DontSave;
		this.depthCurveLut.anisoLevel = 1;
		this.depthCurveLut.wrapMode = TextureWrapMode.Clamp;
		this.depthCurveLut.filterMode = FilterMode.Bilinear;
		this.depthCurveColors = new Color32[1024];
		return this.depthCurveLut;
	}

	// Token: 0x06000613 RID: 1555 RVA: 0x00021F64 File Offset: 0x00020164
	private void UpdateDepthCurveLut()
	{
		if (this.depthCurveLut == null)
		{
			this.CreateDepthCurveLut();
		}
		float num = 0f;
		int i = 0;
		while (i < 1024)
		{
			this.depthCurveColors[i].a = (byte)Mathf.FloorToInt(Mathf.Clamp01(this.DepthMaskCurve.Evaluate(num)) * 255f);
			i++;
			num += 0.0009775171f;
		}
		this.depthCurveLut.SetPixels32(this.depthCurveColors);
		this.depthCurveLut.Apply();
	}

	// Token: 0x06000614 RID: 1556 RVA: 0x00021FF0 File Offset: 0x000201F0
	private void CheckUpdateDepthCurveLut()
	{
		bool flag = false;
		if (this.DepthMaskCurve.length != this.prevDepthMaskCurve.length)
		{
			flag = true;
		}
		else
		{
			float num = 0f;
			int i = 0;
			while (i < this.DepthMaskCurve.length)
			{
				if (Mathf.Abs(this.DepthMaskCurve.Evaluate(num) - this.prevDepthMaskCurve.Evaluate(num)) > 1E-45f)
				{
					flag = true;
					break;
				}
				i++;
				num += 0.0009775171f;
			}
		}
		if (this.depthCurveLut == null || flag)
		{
			this.UpdateDepthCurveLut();
			this.prevDepthMaskCurve = new AnimationCurve(this.DepthMaskCurve.keys);
		}
	}

	// Token: 0x06000615 RID: 1557 RVA: 0x00022094 File Offset: 0x00020294
	private void CreateHelperTextures()
	{
		this.ReleaseTextures();
		this.blendCacheLut = new RenderTexture(1024, 32, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear)
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		this.blendCacheLut.name = "BlendCacheLut";
		this.blendCacheLut.wrapMode = TextureWrapMode.Clamp;
		this.blendCacheLut.useMipMap = false;
		this.blendCacheLut.anisoLevel = 0;
		this.blendCacheLut.Create();
		this.midBlendLUT = new RenderTexture(1024, 32, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear)
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		this.midBlendLUT.name = "MidBlendLut";
		this.midBlendLUT.wrapMode = TextureWrapMode.Clamp;
		this.midBlendLUT.useMipMap = false;
		this.midBlendLUT.anisoLevel = 0;
		this.midBlendLUT.Create();
		this.CreateDefaultLut();
		if (this.UseDepthMask)
		{
			this.CreateDepthCurveLut();
		}
	}

	// Token: 0x06000616 RID: 1558 RVA: 0x00022178 File Offset: 0x00020378
	private bool CheckMaterialAndShader(Material material, string name)
	{
		if (material == null || material.shader == null)
		{
			global::Debug.LogWarning("[AmplifyColor] Error creating " + name + " material. Effect disabled.");
			base.enabled = false;
		}
		else if (!material.shader.isSupported)
		{
			global::Debug.LogWarning("[AmplifyColor] " + name + " shader not supported on this platform. Effect disabled.");
			base.enabled = false;
		}
		else
		{
			material.hideFlags = HideFlags.HideAndDontSave;
		}
		return base.enabled;
	}

	// Token: 0x06000617 RID: 1559 RVA: 0x000221F4 File Offset: 0x000203F4
	private bool CreateMaterials()
	{
		this.SetupShader();
		if (!this.CheckShaders())
		{
			return false;
		}
		this.ReleaseMaterials();
		this.materialBase = new Material(this.shaderBase);
		this.materialBlend = new Material(this.shaderBlend);
		this.materialBlendCache = new Material(this.shaderBlendCache);
		this.materialMask = new Material(this.shaderMask);
		this.materialMaskBlend = new Material(this.shaderMaskBlend);
		this.materialDepthMask = new Material(this.shaderDepthMask);
		this.materialDepthMaskBlend = new Material(this.shaderDepthMaskBlend);
		this.materialProcessOnly = new Material(this.shaderProcessOnly);
		if (!true || !this.CheckMaterialAndShader(this.materialBase, "BaseMaterial") || !this.CheckMaterialAndShader(this.materialBlend, "BlendMaterial") || !this.CheckMaterialAndShader(this.materialBlendCache, "BlendCacheMaterial") || !this.CheckMaterialAndShader(this.materialMask, "MaskMaterial") || !this.CheckMaterialAndShader(this.materialMaskBlend, "MaskBlendMaterial") || !this.CheckMaterialAndShader(this.materialDepthMask, "DepthMaskMaterial") || !this.CheckMaterialAndShader(this.materialDepthMaskBlend, "DepthMaskBlendMaterial") || !this.CheckMaterialAndShader(this.materialProcessOnly, "ProcessOnlyMaterial"))
		{
			return false;
		}
		this.CreateHelperTextures();
		return true;
	}

	// Token: 0x06000618 RID: 1560 RVA: 0x0002235C File Offset: 0x0002055C
	private void SetMaterialKeyword(string keyword, bool state)
	{
		bool flag = this.materialBase.IsKeywordEnabled(keyword);
		if (state && !flag)
		{
			this.materialBase.EnableKeyword(keyword);
			this.materialBlend.EnableKeyword(keyword);
			this.materialBlendCache.EnableKeyword(keyword);
			this.materialMask.EnableKeyword(keyword);
			this.materialMaskBlend.EnableKeyword(keyword);
			this.materialDepthMask.EnableKeyword(keyword);
			this.materialDepthMaskBlend.EnableKeyword(keyword);
			this.materialProcessOnly.EnableKeyword(keyword);
			return;
		}
		if (!state && this.materialBase.IsKeywordEnabled(keyword))
		{
			this.materialBase.DisableKeyword(keyword);
			this.materialBlend.DisableKeyword(keyword);
			this.materialBlendCache.DisableKeyword(keyword);
			this.materialMask.DisableKeyword(keyword);
			this.materialMaskBlend.DisableKeyword(keyword);
			this.materialDepthMask.DisableKeyword(keyword);
			this.materialDepthMaskBlend.DisableKeyword(keyword);
			this.materialProcessOnly.DisableKeyword(keyword);
		}
	}

	// Token: 0x06000619 RID: 1561 RVA: 0x00022450 File Offset: 0x00020650
	private void SafeRelease<T>(ref T obj) where T : Object
	{
		if (obj != null)
		{
			if (obj.GetType() == typeof(RenderTexture))
			{
				(obj as RenderTexture).Release();
			}
			Object.DestroyImmediate(obj);
			obj = default(T);
		}
	}

	// Token: 0x0600061A RID: 1562 RVA: 0x000224B9 File Offset: 0x000206B9
	private void ReleaseTextures()
	{
		RenderTexture.active = null;
		this.SafeRelease<RenderTexture>(ref this.blendCacheLut);
		this.SafeRelease<RenderTexture>(ref this.midBlendLUT);
		this.SafeRelease<Texture2D>(ref this.defaultLut);
		this.SafeRelease<Texture2D>(ref this.depthCurveLut);
	}

	// Token: 0x0600061B RID: 1563 RVA: 0x000224F4 File Offset: 0x000206F4
	public static bool ValidateLutDimensions(Texture lut)
	{
		bool result = true;
		if (lut != null)
		{
			if (lut.width / lut.height != lut.height)
			{
				global::Debug.LogWarning("[AmplifyColor] Lut " + lut.name + " has invalid dimensions.");
				result = false;
			}
			else if (lut.anisoLevel != 0)
			{
				lut.anisoLevel = 0;
			}
		}
		return result;
	}

	// Token: 0x0600061C RID: 1564 RVA: 0x0002254F File Offset: 0x0002074F
	private void UpdatePostEffectParams()
	{
		if (this.UseDepthMask)
		{
			this.CheckUpdateDepthCurveLut();
		}
		this.Exposure = Mathf.Max(this.Exposure, 0f);
	}

	// Token: 0x0600061D RID: 1565 RVA: 0x00022578 File Offset: 0x00020778
	private int ComputeShaderPass()
	{
		bool flag = this.QualityLevel == Quality.Mobile;
		bool flag2 = this.colorSpace == ColorSpace.Linear;
		bool allowHDR = this.ownerCamera.allowHDR;
		int num = flag ? 18 : 0;
		if (allowHDR)
		{
			num += 2;
			num += (flag2 ? 8 : 0);
			num += (this.ApplyDithering ? 4 : 0);
			num = (int)(num + this.Tonemapper);
		}
		else
		{
			num += (flag2 ? 1 : 0);
		}
		return num;
	}

	// Token: 0x0600061E RID: 1566 RVA: 0x000225E4 File Offset: 0x000207E4
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (this.silentError)
		{
			Graphics.Blit(source, destination);
			return;
		}
		this.BlendAmount = Mathf.Clamp01(this.BlendAmount);
		if (this.colorSpace != QualitySettings.activeColorSpace || this.qualityLevel != this.QualityLevel)
		{
			this.CreateMaterials();
		}
		this.UpdatePostEffectParams();
		bool flag = AmplifyColorBase.ValidateLutDimensions(this.LutTexture);
		bool flag2 = AmplifyColorBase.ValidateLutDimensions(this.LutBlendTexture);
		bool flag3 = this.LutTexture == null && this.LutBlendTexture == null && this.volumesLutBlendTexture == null;
		Texture texture = (this.LutTexture == null) ? this.defaultLut : this.LutTexture;
		Texture lutBlendTexture = this.LutBlendTexture;
		int pass = this.ComputeShaderPass();
		bool flag4 = this.BlendAmount != 0f || this.blending;
		bool flag5 = flag4 || (flag4 && lutBlendTexture != null);
		bool flag6 = flag5;
		bool flag7 = !flag || !flag2 || flag3;
		Material material;
		if (flag7)
		{
			material = this.materialProcessOnly;
		}
		else if (flag5 || this.volumesBlending)
		{
			if (this.UseDepthMask)
			{
				material = this.materialDepthMaskBlend;
			}
			else
			{
				material = ((this.MaskTexture != null) ? this.materialMaskBlend : this.materialBlend);
			}
		}
		else if (this.UseDepthMask)
		{
			material = this.materialDepthMask;
		}
		else
		{
			material = ((this.MaskTexture != null) ? this.materialMask : this.materialBase);
		}
		material.SetFloat("_Exposure", this.Exposure);
		material.SetFloat("_ShoulderStrength", 0.22f);
		material.SetFloat("_LinearStrength", 0.3f);
		material.SetFloat("_LinearAngle", 0.1f);
		material.SetFloat("_ToeStrength", 0.2f);
		material.SetFloat("_ToeNumerator", 0.01f);
		material.SetFloat("_ToeDenominator", 0.3f);
		material.SetFloat("_LinearWhite", this.LinearWhitePoint);
		material.SetFloat("_LerpAmount", this.BlendAmount);
		if (this.MaskTexture != null)
		{
			material.SetTexture("_MaskTex", this.MaskTexture);
		}
		if (this.UseDepthMask)
		{
			material.SetTexture("_DepthCurveLut", this.depthCurveLut);
		}
		if (this.MaskTexture != null && source.dimension == TextureDimension.Tex2DArray)
		{
			material.SetVector("_StereoScale", new Vector4(0.5f, 1f, 0.5f, 0f));
		}
		else
		{
			material.SetVector("_StereoScale", new Vector4(1f, 1f, 0f, 0f));
		}
		if (!flag7)
		{
			if (this.volumesBlending)
			{
				this.volumesBlendAmount = Mathf.Clamp01(this.volumesBlendAmount);
				this.materialBlendCache.SetFloat("_LerpAmount", this.volumesBlendAmount);
				if (this.blendingFromMidBlend)
				{
					this.materialBlendCache.SetTexture("_RgbTex", this.midBlendLUT);
				}
				else
				{
					this.materialBlendCache.SetTexture("_RgbTex", texture);
				}
				this.materialBlendCache.SetTexture("_LerpRgbTex", (this.volumesLutBlendTexture != null) ? this.volumesLutBlendTexture : this.defaultLut);
				Graphics.Blit(texture, this.blendCacheLut, this.materialBlendCache);
			}
			if (flag6)
			{
				this.materialBlendCache.SetFloat("_LerpAmount", this.BlendAmount);
				RenderTexture renderTexture = null;
				if (this.volumesBlending)
				{
					renderTexture = RenderTexture.GetTemporary(this.blendCacheLut.width, this.blendCacheLut.height, this.blendCacheLut.depth, this.blendCacheLut.format, RenderTextureReadWrite.Linear);
					Graphics.Blit(this.blendCacheLut, renderTexture);
					this.materialBlendCache.SetTexture("_RgbTex", renderTexture);
				}
				else
				{
					this.materialBlendCache.SetTexture("_RgbTex", texture);
				}
				this.materialBlendCache.SetTexture("_LerpRgbTex", (lutBlendTexture != null) ? lutBlendTexture : this.defaultLut);
				Graphics.Blit(texture, this.blendCacheLut, this.materialBlendCache);
				if (renderTexture != null)
				{
					RenderTexture.ReleaseTemporary(renderTexture);
				}
				material.SetTexture("_RgbBlendCacheTex", this.blendCacheLut);
			}
			else if (this.volumesBlending)
			{
				material.SetTexture("_RgbBlendCacheTex", this.blendCacheLut);
			}
			else
			{
				if (texture != null)
				{
					material.SetTexture("_RgbTex", texture);
				}
				if (lutBlendTexture != null)
				{
					material.SetTexture("_LerpRgbTex", lutBlendTexture);
				}
			}
		}
		Graphics.Blit(source, destination, material, pass);
		if (flag6 || this.volumesBlending)
		{
			this.blendCacheLut.DiscardContents();
		}
	}

	// Token: 0x04000782 RID: 1922
	public const int LutSize = 32;

	// Token: 0x04000783 RID: 1923
	public const int LutWidth = 1024;

	// Token: 0x04000784 RID: 1924
	public const int LutHeight = 32;

	// Token: 0x04000785 RID: 1925
	private const int DepthCurveLutRange = 1024;

	// Token: 0x04000786 RID: 1926
	public Tonemapping Tonemapper;

	// Token: 0x04000787 RID: 1927
	public float Exposure = 1f;

	// Token: 0x04000788 RID: 1928
	public float LinearWhitePoint = 11.2f;

	// Token: 0x04000789 RID: 1929
	[FormerlySerializedAs("UseDithering")]
	public bool ApplyDithering;

	// Token: 0x0400078A RID: 1930
	public Quality QualityLevel = Quality.Standard;

	// Token: 0x0400078B RID: 1931
	public float BlendAmount;

	// Token: 0x0400078C RID: 1932
	public Texture LutTexture;

	// Token: 0x0400078D RID: 1933
	public Texture LutBlendTexture;

	// Token: 0x0400078E RID: 1934
	public Texture MaskTexture;

	// Token: 0x0400078F RID: 1935
	public bool UseDepthMask;

	// Token: 0x04000790 RID: 1936
	public AnimationCurve DepthMaskCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 1f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x04000791 RID: 1937
	public bool UseVolumes;

	// Token: 0x04000792 RID: 1938
	public float ExitVolumeBlendTime = 1f;

	// Token: 0x04000793 RID: 1939
	public Transform TriggerVolumeProxy;

	// Token: 0x04000794 RID: 1940
	public LayerMask VolumeCollisionMask = -1;

	// Token: 0x04000795 RID: 1941
	private Camera ownerCamera;

	// Token: 0x04000796 RID: 1942
	private Shader shaderBase;

	// Token: 0x04000797 RID: 1943
	private Shader shaderBlend;

	// Token: 0x04000798 RID: 1944
	private Shader shaderBlendCache;

	// Token: 0x04000799 RID: 1945
	private Shader shaderMask;

	// Token: 0x0400079A RID: 1946
	private Shader shaderMaskBlend;

	// Token: 0x0400079B RID: 1947
	private Shader shaderDepthMask;

	// Token: 0x0400079C RID: 1948
	private Shader shaderDepthMaskBlend;

	// Token: 0x0400079D RID: 1949
	private Shader shaderProcessOnly;

	// Token: 0x0400079E RID: 1950
	private RenderTexture blendCacheLut;

	// Token: 0x0400079F RID: 1951
	private Texture2D defaultLut;

	// Token: 0x040007A0 RID: 1952
	private Texture2D depthCurveLut;

	// Token: 0x040007A1 RID: 1953
	private Color32[] depthCurveColors;

	// Token: 0x040007A2 RID: 1954
	private ColorSpace colorSpace = ColorSpace.Uninitialized;

	// Token: 0x040007A3 RID: 1955
	private Quality qualityLevel = Quality.Standard;

	// Token: 0x040007A4 RID: 1956
	private Material materialBase;

	// Token: 0x040007A5 RID: 1957
	private Material materialBlend;

	// Token: 0x040007A6 RID: 1958
	private Material materialBlendCache;

	// Token: 0x040007A7 RID: 1959
	private Material materialMask;

	// Token: 0x040007A8 RID: 1960
	private Material materialMaskBlend;

	// Token: 0x040007A9 RID: 1961
	private Material materialDepthMask;

	// Token: 0x040007AA RID: 1962
	private Material materialDepthMaskBlend;

	// Token: 0x040007AB RID: 1963
	private Material materialProcessOnly;

	// Token: 0x040007AC RID: 1964
	private bool blending;

	// Token: 0x040007AD RID: 1965
	private float blendingTime;

	// Token: 0x040007AE RID: 1966
	private float blendingTimeCountdown;

	// Token: 0x040007AF RID: 1967
	private Action onFinishBlend;

	// Token: 0x040007B0 RID: 1968
	private AnimationCurve prevDepthMaskCurve = new AnimationCurve();

	// Token: 0x040007B1 RID: 1969
	private bool volumesBlending;

	// Token: 0x040007B2 RID: 1970
	private float volumesBlendingTime;

	// Token: 0x040007B3 RID: 1971
	private float volumesBlendingTimeCountdown;

	// Token: 0x040007B4 RID: 1972
	private Texture volumesLutBlendTexture;

	// Token: 0x040007B5 RID: 1973
	private float volumesBlendAmount;

	// Token: 0x040007B6 RID: 1974
	private Texture worldLUT;

	// Token: 0x040007B7 RID: 1975
	private AmplifyColorVolumeBase currentVolumeLut;

	// Token: 0x040007B8 RID: 1976
	private RenderTexture midBlendLUT;

	// Token: 0x040007B9 RID: 1977
	private bool blendingFromMidBlend;

	// Token: 0x040007BA RID: 1978
	private VolumeEffect worldVolumeEffects;

	// Token: 0x040007BB RID: 1979
	private VolumeEffect currentVolumeEffects;

	// Token: 0x040007BC RID: 1980
	private VolumeEffect blendVolumeEffects;

	// Token: 0x040007BD RID: 1981
	private float worldExposure = 1f;

	// Token: 0x040007BE RID: 1982
	private float currentExposure = 1f;

	// Token: 0x040007BF RID: 1983
	private float blendExposure = 1f;

	// Token: 0x040007C0 RID: 1984
	private float effectVolumesBlendAdjust;

	// Token: 0x040007C1 RID: 1985
	private List<AmplifyColorVolumeBase> enteredVolumes = new List<AmplifyColorVolumeBase>();

	// Token: 0x040007C2 RID: 1986
	private AmplifyColorTriggerProxyBase actualTriggerProxy;

	// Token: 0x040007C3 RID: 1987
	[HideInInspector]
	public VolumeEffectFlags EffectFlags = new VolumeEffectFlags();

	// Token: 0x040007C4 RID: 1988
	[SerializeField]
	[HideInInspector]
	private string sharedInstanceID = "";

	// Token: 0x040007C5 RID: 1989
	private bool silentError;
}
