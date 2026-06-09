using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000158 RID: 344
public class HoverEventColor : MonoBehaviour
{
	// Token: 0x060006F2 RID: 1778 RVA: 0x000265FC File Offset: 0x000247FC
	private void Start()
	{
		this.img = base.GetComponent<Image>();
		this.defaultColor = this.img.color;
		this.hover = base.GetComponent<HoverEvent>();
		this.hover.enterEvent.AddListener(new UnityAction(this.Enter));
		this.hover.exitEvent.AddListener(new UnityAction(this.Exit));
	}

	// Token: 0x060006F3 RID: 1779 RVA: 0x0002666A File Offset: 0x0002486A
	private void Enter()
	{
		this.img.color = this.hoverColor;
	}

	// Token: 0x060006F4 RID: 1780 RVA: 0x0002667D File Offset: 0x0002487D
	private void Exit()
	{
		this.img.color = this.defaultColor;
	}

	// Token: 0x04000860 RID: 2144
	public Color hoverColor;

	// Token: 0x04000861 RID: 2145
	private Color defaultColor;

	// Token: 0x04000862 RID: 2146
	private HoverEvent hover;

	// Token: 0x04000863 RID: 2147
	private Image img;
}
