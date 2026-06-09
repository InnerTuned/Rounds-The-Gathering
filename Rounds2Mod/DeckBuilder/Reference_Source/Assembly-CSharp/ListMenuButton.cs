using System;
using SoundImplementation;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000161 RID: 353
public class ListMenuButton : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler, IUpdateSelectedHandler, ISelectHandler
{
	// Token: 0x06000715 RID: 1813 RVA: 0x000027C8 File Offset: 0x000009C8
	private void Awake()
	{
	}

	// Token: 0x06000716 RID: 1814 RVA: 0x00026DBD File Offset: 0x00024FBD
	private void Start()
	{
		this.Init();
		this.smallFont = this.text.fontSize;
		this.defaultPos = this.text.transform.localPosition;
		this.defaultColor = this.text.color;
	}

	// Token: 0x06000717 RID: 1815 RVA: 0x00026E00 File Offset: 0x00025000
	public void Nope()
	{
		this.Init();
		MenuEffects.instance.BlinkInColor(this.text, MenuEffects.instance.nopeColor, this.defaultColor, 0.15f);
		MenuEffects.instance.ShakeObject(this.text.gameObject, this.defaultPos, 15f, 0.2f);
	}

	// Token: 0x06000718 RID: 1816 RVA: 0x00026E5D File Offset: 0x0002505D
	public void Deselect()
	{
		this.Init();
		this.text.fontStyle = 0;
		bool flag = this.changeFontSize;
		if (this.toggleTextColor)
		{
			base.GetComponentInChildren<TextMeshProUGUI>().color = this.defaultTextColor;
		}
	}

	// Token: 0x06000719 RID: 1817 RVA: 0x00026E91 File Offset: 0x00025091
	public void Select()
	{
		this.Init();
		this.text.fontStyle = 1;
		bool flag = this.changeFontSize;
		if (this.toggleTextColor)
		{
			base.GetComponentInChildren<TextMeshProUGUI>().color = this.selectedTextColor;
		}
	}

	// Token: 0x0600071A RID: 1818 RVA: 0x00026EC5 File Offset: 0x000250C5
	public void OnPointerClick(PointerEventData eventData)
	{
		SoundPlayerStatic.Instance.PlayButtonClick();
	}

	// Token: 0x0600071B RID: 1819 RVA: 0x00026ED1 File Offset: 0x000250D1
	public void OnPointerEnter(PointerEventData eventData)
	{
		ListMenu.instance.SelectButton(this);
	}

	// Token: 0x0600071C RID: 1820 RVA: 0x000027C8 File Offset: 0x000009C8
	public void OnPointerExit(PointerEventData eventData)
	{
	}

	// Token: 0x0600071D RID: 1821 RVA: 0x000027C8 File Offset: 0x000009C8
	public void OnUpdateSelected(BaseEventData eventData)
	{
	}

	// Token: 0x0600071E RID: 1822 RVA: 0x00026ED1 File Offset: 0x000250D1
	public void OnSelect(BaseEventData eventData)
	{
		ListMenu.instance.SelectButton(this);
	}

	// Token: 0x0600071F RID: 1823 RVA: 0x00026EDE File Offset: 0x000250DE
	private void Init()
	{
		if (this.inited)
		{
			return;
		}
		this.inited = true;
		this.text = base.GetComponentInChildren<TextMeshProUGUI>();
	}

	// Token: 0x04000886 RID: 2182
	private TextMeshProUGUI text;

	// Token: 0x04000887 RID: 2183
	private Color defaultColor;

	// Token: 0x04000888 RID: 2184
	private Vector3 defaultPos;

	// Token: 0x04000889 RID: 2185
	public bool changeFontSize = true;

	// Token: 0x0400088A RID: 2186
	public bool hideBar;

	// Token: 0x0400088B RID: 2187
	public float setBarHeight;

	// Token: 0x0400088C RID: 2188
	public bool toggleTextColor;

	// Token: 0x0400088D RID: 2189
	public Color selectedTextColor;

	// Token: 0x0400088E RID: 2190
	public Color defaultTextColor;

	// Token: 0x0400088F RID: 2191
	private float smallFont = 40f;

	// Token: 0x04000890 RID: 2192
	private bool inited;
}
