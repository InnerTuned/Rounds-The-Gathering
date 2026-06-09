using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Pun.Simple
{
	// Token: 0x0200027E RID: 638
	public class VitalUI : VitalUIBase
	{
		// Token: 0x06000DCD RID: 3533 RVA: 0x000431F1 File Offset: 0x000413F1
		protected override void Reset()
		{
			base.Reset();
			this.FindUIElements();
		}

		// Token: 0x06000DCE RID: 3534 RVA: 0x00043200 File Offset: 0x00041400
		protected override void Awake()
		{
			base.Awake();
			this.FindUIElements();
			base.enabled = this.billboard;
		}

		// Token: 0x06000DCF RID: 3535 RVA: 0x0004321C File Offset: 0x0004141C
		public override void Recalculate()
		{
			this.AutoAlign();
			if (this.backSprite)
			{
				this.backSprite.size = new Vector2(this.widthMultiplier, this.backSprite.size.y);
			}
			this.UpdateGraphics(this.vital);
		}

		// Token: 0x06000DD0 RID: 3536 RVA: 0x0004326E File Offset: 0x0004146E
		protected virtual void AutoAlign()
		{
			if (this.autoOffset && base.transform.parent)
			{
				base.transform.localPosition = this.offset * (float)this.vitalIndex;
			}
		}

		// Token: 0x06000DD1 RID: 3537 RVA: 0x000432A8 File Offset: 0x000414A8
		public bool FindUIElements()
		{
			if (this.textMesh == null)
			{
				this.textMesh = (this.searchChildren ? base.GetComponentInChildren<TextMesh>() : base.GetComponent<TextMesh>());
			}
			if (this.searchChildren)
			{
				base.GetComponentsInChildren<SpriteRenderer>(VitalUI.resuableFindSpriteRend);
			}
			else
			{
				base.GetComponents<SpriteRenderer>(VitalUI.resuableFindSpriteRend);
			}
			if (VitalUI.resuableFindSpriteRend.Count > 0 && this.barSprite == null)
			{
				this.barSprite = VitalUI.resuableFindSpriteRend[0];
			}
			if (VitalUI.resuableFindSpriteRend.Count > 1 && this.backSprite == null)
			{
				this.backSprite = VitalUI.resuableFindSpriteRend[1];
			}
			if (this.UIText == null)
			{
				this.UIText = (this.searchChildren ? base.GetComponentInChildren<Text>() : base.GetComponent<Text>());
			}
			if (this.UIImage == null)
			{
				this.UIImage = (this.searchChildren ? base.GetComponentInChildren<Image>() : base.GetComponent<Image>());
			}
			return this.textMesh || this.barSprite || this.UIText || this.UIImage;
		}

		// Token: 0x06000DD2 RID: 3538 RVA: 0x000433E0 File Offset: 0x000415E0
		public override void UpdateGraphics(Vital vital)
		{
			if (vital == null)
			{
				return;
			}
			VitalDefinition vitalDef = vital.VitalDef;
			double num = (this.targetField == VitalUIBase.TargetField.Value) ? vital.VitalData.Value : ((this.targetField == VitalUIBase.TargetField.Max) ? vitalDef.FullValue : vitalDef.MaxValue);
			if (num == double.NegativeInfinity)
			{
				return;
			}
			if (this.textMesh)
			{
				this.textMesh.text = ((int)num).ToString();
			}
			if (this.barSprite)
			{
				this.barSprite.size = new Vector2((float)(num / vitalDef.MaxValue * (double)this.widthMultiplier), this.barSprite.size.y);
			}
			if (this.UIText != null)
			{
				this.UIText.text = ((int)num).ToString();
			}
			if (this.UIImage != null)
			{
				double maxValue = vitalDef.MaxValue;
				if (this.UIImage.type == 3 && this.UIImage.sprite != null)
				{
					this.UIImage.fillAmount = (float)(num / maxValue * (double)this.widthMultiplier);
					return;
				}
				this.UIImage.rectTransform.localScale = new Vector3((float)(num / maxValue * (double)this.widthMultiplier), this.UIImage.rectTransform.localScale.y, this.UIImage.rectTransform.localScale.z);
			}
		}

		// Token: 0x06000DD3 RID: 3539 RVA: 0x00043558 File Offset: 0x00041758
		public void LateUpdate()
		{
			Camera main = Camera.main;
			if (!main)
			{
				return;
			}
			base.transform.LookAt(main.transform, new Vector3(0f, 1f, 0f));
			Vector3 eulerAngles = base.transform.eulerAngles;
			base.transform.eulerAngles = new Vector3(0f, eulerAngles.y + 180f, 0f);
		}

		// Token: 0x04000D14 RID: 3348
		protected static GameObject vitalBarDefaultPrefab;

		// Token: 0x04000D15 RID: 3349
		public bool autoOffset = true;

		// Token: 0x04000D16 RID: 3350
		[Tooltip("Found children elements are nudged (value * vitalIndex). This is to automatically stagger multiple VitalUIs")]
		public Vector3 offset = new Vector3(0f, 0.1f, 0f);

		// Token: 0x04000D17 RID: 3351
		public float widthMultiplier = 1f;

		// Token: 0x04000D18 RID: 3352
		[Tooltip("Search for UI elements in children of this GameObject.")]
		[HideInInspector]
		public bool searchChildren = true;

		// Token: 0x04000D19 RID: 3353
		[HideInInspector]
		public Text UIText;

		// Token: 0x04000D1A RID: 3354
		[HideInInspector]
		public Image UIImage;

		// Token: 0x04000D1B RID: 3355
		[HideInInspector]
		public TextMesh textMesh;

		// Token: 0x04000D1C RID: 3356
		[HideInInspector]
		public SpriteRenderer barSprite;

		// Token: 0x04000D1D RID: 3357
		[HideInInspector]
		public SpriteRenderer backSprite;

		// Token: 0x04000D1E RID: 3358
		[HideInInspector]
		public bool billboard = true;

		// Token: 0x04000D1F RID: 3359
		private const string PLACEHOLDER_CANVAS_NAME = "PLACEHOLDER_VITALS_CANVAS";

		// Token: 0x04000D20 RID: 3360
		private static List<SpriteRenderer> resuableFindSpriteRend = new List<SpriteRenderer>();
	}
}
