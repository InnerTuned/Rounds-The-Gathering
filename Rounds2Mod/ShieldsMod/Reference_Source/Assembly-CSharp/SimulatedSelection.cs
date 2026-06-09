using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001B4 RID: 436
public class SimulatedSelection : MonoBehaviour
{
	// Token: 0x060008B1 RID: 2225 RVA: 0x0002DE89 File Offset: 0x0002C089
	private void Start()
	{
		this.hoverEvent = base.GetComponent<HoverEvent>();
		this.button = base.GetComponent<Button>();
	}

	// Token: 0x060008B2 RID: 2226 RVA: 0x0002DEA3 File Offset: 0x0002C0A3
	private void OnDisable()
	{
		this.Deselect();
	}

	// Token: 0x060008B3 RID: 2227 RVA: 0x0002DEAC File Offset: 0x0002C0AC
	public void Select()
	{
		this.hoverEvent.OnPointerEnter(null);
		this.button.targetGraphic.color = this.button.colors.highlightedColor;
	}

	// Token: 0x060008B4 RID: 2228 RVA: 0x0002DEE8 File Offset: 0x0002C0E8
	public void Deselect()
	{
		this.hoverEvent.OnPointerExit(null);
		this.button.OnDeselect(null);
		this.button.targetGraphic.color = this.button.colors.normalColor;
	}

	// Token: 0x040009F1 RID: 2545
	private HoverEvent hoverEvent;

	// Token: 0x040009F2 RID: 2546
	private Button button;
}
