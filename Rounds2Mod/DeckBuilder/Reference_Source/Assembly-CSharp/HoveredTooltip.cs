using System;
using UnityEngine;

// Token: 0x02000155 RID: 341
public class HoveredTooltip : MonoBehaviour
{
	// Token: 0x060006E5 RID: 1765 RVA: 0x00026366 File Offset: 0x00024566
	private void Start()
	{
		this.anim = base.GetComponent<CurveAnimation>();
		this.hoverEvent = base.GetComponentInParent<HoverEvent>();
	}

	// Token: 0x060006E6 RID: 1766 RVA: 0x00026380 File Offset: 0x00024580
	private void Update()
	{
		if (!this.anim.IsPlaying())
		{
			if ((this.hoverEvent.isHovered || this.hoverEvent.isSelected) && this.anim.currentState != CurveAnimationUse.In)
			{
				this.anim.PlayIn();
			}
			if (!this.hoverEvent.isHovered && !this.hoverEvent.isSelected && this.anim.currentState != CurveAnimationUse.Out)
			{
				this.anim.PlayOut();
			}
		}
	}

	// Token: 0x060006E7 RID: 1767 RVA: 0x000263FF File Offset: 0x000245FF
	private void OnEnable()
	{
		if (!this.anim)
		{
			return;
		}
		this.anim.currentState = CurveAnimationUse.Out;
		this.anim.transform.localScale = Vector3.zero;
	}

	// Token: 0x060006E8 RID: 1768 RVA: 0x000263FF File Offset: 0x000245FF
	private void OnDisable()
	{
		if (!this.anim)
		{
			return;
		}
		this.anim.currentState = CurveAnimationUse.Out;
		this.anim.transform.localScale = Vector3.zero;
	}

	// Token: 0x04000854 RID: 2132
	private CurveAnimation anim;

	// Token: 0x04000855 RID: 2133
	private HoverEvent hoverEvent;
}
