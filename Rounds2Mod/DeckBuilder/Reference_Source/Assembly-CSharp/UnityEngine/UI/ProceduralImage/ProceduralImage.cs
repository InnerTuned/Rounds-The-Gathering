using System;
using UnityEngine.Events;

namespace UnityEngine.UI.ProceduralImage
{
	// Token: 0x020001E4 RID: 484
	[ExecuteInEditMode]
	[AddComponentMenu("UI/Procedural Image")]
	public class ProceduralImage : Image
	{
		// Token: 0x1700004F RID: 79
		// (get) Token: 0x0600098D RID: 2445 RVA: 0x00030FDC File Offset: 0x0002F1DC
		// (set) Token: 0x0600098E RID: 2446 RVA: 0x00031004 File Offset: 0x0002F204
		private static Material DefaultProceduralImageMaterial
		{
			get
			{
				if (ProceduralImage.materialInstance == null)
				{
					ProceduralImage.materialInstance = new Material(Shader.Find("UI/Procedural UI Image"));
				}
				return ProceduralImage.materialInstance;
			}
			set
			{
				ProceduralImage.materialInstance = value;
			}
		}

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x0600098F RID: 2447 RVA: 0x0003100C File Offset: 0x0002F20C
		// (set) Token: 0x06000990 RID: 2448 RVA: 0x00031014 File Offset: 0x0002F214
		public float BorderWidth
		{
			get
			{
				return this.borderWidth;
			}
			set
			{
				this.borderWidth = value;
				this.SetVerticesDirty();
			}
		}

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x06000991 RID: 2449 RVA: 0x00031023 File Offset: 0x0002F223
		// (set) Token: 0x06000992 RID: 2450 RVA: 0x0003102B File Offset: 0x0002F22B
		public float FalloffDistance
		{
			get
			{
				return this.falloffDistance;
			}
			set
			{
				this.falloffDistance = value;
				this.SetVerticesDirty();
			}
		}

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x06000993 RID: 2451 RVA: 0x0003103A File Offset: 0x0002F23A
		// (set) Token: 0x06000994 RID: 2452 RVA: 0x0003107A File Offset: 0x0002F27A
		protected ProceduralImageModifier Modifier
		{
			get
			{
				if (this.modifier == null)
				{
					this.modifier = base.GetComponent<ProceduralImageModifier>();
					if (this.modifier == null)
					{
						this.ModifierType = typeof(FreeModifier);
					}
				}
				return this.modifier;
			}
			set
			{
				this.modifier = value;
			}
		}

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x06000995 RID: 2453 RVA: 0x00031083 File Offset: 0x0002F283
		// (set) Token: 0x06000996 RID: 2454 RVA: 0x00031090 File Offset: 0x0002F290
		public Type ModifierType
		{
			get
			{
				return this.Modifier.GetType();
			}
			set
			{
				if (this.modifier != null && this.modifier.GetType() != value)
				{
					if (base.GetComponent<ProceduralImageModifier>() != null)
					{
						Object.DestroyImmediate(base.GetComponent<ProceduralImageModifier>());
					}
					base.gameObject.AddComponent(value);
					this.Modifier = base.GetComponent<ProceduralImageModifier>();
					this.SetAllDirty();
					return;
				}
				if (this.modifier == null)
				{
					base.gameObject.AddComponent(value);
					this.Modifier = base.GetComponent<ProceduralImageModifier>();
					this.SetAllDirty();
				}
			}
		}

		// Token: 0x06000997 RID: 2455 RVA: 0x00031124 File Offset: 0x0002F324
		protected override void OnEnable()
		{
			base.OnEnable();
			this.Init();
		}

		// Token: 0x06000998 RID: 2456 RVA: 0x00031132 File Offset: 0x0002F332
		protected override void OnDisable()
		{
			base.OnDisable();
			this.m_OnDirtyVertsCallback = (UnityAction)Delegate.Remove(this.m_OnDirtyVertsCallback, new UnityAction(this.OnVerticesDirty));
		}

		// Token: 0x06000999 RID: 2457 RVA: 0x0003115C File Offset: 0x0002F35C
		private void Init()
		{
			this.FixTexCoordsInCanvas();
			this.m_OnDirtyVertsCallback = (UnityAction)Delegate.Combine(this.m_OnDirtyVertsCallback, new UnityAction(this.OnVerticesDirty));
			base.preserveAspect = false;
			this.material = null;
			if (base.sprite == null)
			{
				base.sprite = EmptySprite.Get();
			}
		}

		// Token: 0x0600099A RID: 2458 RVA: 0x000311B8 File Offset: 0x0002F3B8
		protected void OnVerticesDirty()
		{
			if (base.sprite == null)
			{
				base.sprite = EmptySprite.Get();
			}
		}

		// Token: 0x0600099B RID: 2459 RVA: 0x000311D4 File Offset: 0x0002F3D4
		protected void FixTexCoordsInCanvas()
		{
			Canvas componentInParent = base.GetComponentInParent<Canvas>();
			if (componentInParent != null)
			{
				this.FixTexCoordsInCanvas(componentInParent);
			}
		}

		// Token: 0x0600099C RID: 2460 RVA: 0x000311F8 File Offset: 0x0002F3F8
		protected void FixTexCoordsInCanvas(Canvas c)
		{
			c.additionalShaderChannels |= 7;
		}

		// Token: 0x0600099D RID: 2461 RVA: 0x00031208 File Offset: 0x0002F408
		private Vector4 FixRadius(Vector4 vec)
		{
			Rect rect = base.rectTransform.rect;
			vec = new Vector4(Mathf.Max(vec.x, 0f), Mathf.Max(vec.y, 0f), Mathf.Max(vec.z, 0f), Mathf.Max(vec.w, 0f));
			float d = Mathf.Min(Mathf.Min(Mathf.Min(Mathf.Min(rect.width / (vec.x + vec.y), rect.width / (vec.z + vec.w)), rect.height / (vec.x + vec.w)), rect.height / (vec.z + vec.y)), 1f);
			return vec * d;
		}

		// Token: 0x0600099E RID: 2462 RVA: 0x000312DD File Offset: 0x0002F4DD
		protected override void OnPopulateMesh(VertexHelper toFill)
		{
			base.OnPopulateMesh(toFill);
			this.EncodeAllInfoIntoVertices(toFill, this.CalculateInfo());
		}

		// Token: 0x0600099F RID: 2463 RVA: 0x000312F3 File Offset: 0x0002F4F3
		protected override void OnTransformParentChanged()
		{
			base.OnTransformParentChanged();
			this.FixTexCoordsInCanvas();
		}

		// Token: 0x060009A0 RID: 2464 RVA: 0x00031304 File Offset: 0x0002F504
		private ProceduralImageInfo CalculateInfo()
		{
			Rect pixelAdjustedRect = base.GetPixelAdjustedRect();
			float pixelSize = 1f / Mathf.Max(0f, this.falloffDistance);
			Vector4 a = this.FixRadius(this.Modifier.CalculateRadius(pixelAdjustedRect));
			float num = Mathf.Min(pixelAdjustedRect.width, pixelAdjustedRect.height);
			return new ProceduralImageInfo(pixelAdjustedRect.width + this.falloffDistance, pixelAdjustedRect.height + this.falloffDistance, this.falloffDistance, pixelSize, a / num, this.borderWidth / num * 2f);
		}

		// Token: 0x060009A1 RID: 2465 RVA: 0x00031394 File Offset: 0x0002F594
		private void EncodeAllInfoIntoVertices(VertexHelper vh, ProceduralImageInfo info)
		{
			UIVertex uivertex = default(UIVertex);
			Vector2 uv = new Vector2(info.width, info.height);
			Vector2 uv2 = new Vector2(this.EncodeFloats_0_1_16_16(info.radius.x, info.radius.y), this.EncodeFloats_0_1_16_16(info.radius.z, info.radius.w));
			Vector2 uv3 = new Vector2((info.borderWidth == 0f) ? 1f : Mathf.Clamp01(info.borderWidth), info.pixelSize);
			for (int i = 0; i < vh.currentVertCount; i++)
			{
				vh.PopulateUIVertex(ref uivertex, i);
				uivertex.position += (uivertex.uv0 - new Vector3(0.5f, 0.5f)) * info.fallOffDistance;
				uivertex.uv1 = uv;
				uivertex.uv2 = uv2;
				uivertex.uv3 = uv3;
				vh.SetUIVertex(uivertex, i);
			}
		}

		// Token: 0x060009A2 RID: 2466 RVA: 0x000314A8 File Offset: 0x0002F6A8
		private float EncodeFloats_0_1_16_16(float a, float b)
		{
			Vector2 rhs = new Vector2(1f, 1.5259022E-05f);
			return Vector2.Dot(new Vector2(Mathf.Floor(a * 65534f) / 65535f, Mathf.Floor(b * 65534f) / 65535f), rhs);
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x060009A3 RID: 2467 RVA: 0x000314F5 File Offset: 0x0002F6F5
		// (set) Token: 0x060009A4 RID: 2468 RVA: 0x00031511 File Offset: 0x0002F711
		public override Material material
		{
			get
			{
				if (this.m_Material == null)
				{
					return ProceduralImage.DefaultProceduralImageMaterial;
				}
				return base.material;
			}
			set
			{
				base.material = value;
			}
		}

		// Token: 0x04000AE8 RID: 2792
		[SerializeField]
		private float borderWidth;

		// Token: 0x04000AE9 RID: 2793
		private ProceduralImageModifier modifier;

		// Token: 0x04000AEA RID: 2794
		private static Material materialInstance;

		// Token: 0x04000AEB RID: 2795
		[SerializeField]
		private float falloffDistance = 1f;
	}
}
