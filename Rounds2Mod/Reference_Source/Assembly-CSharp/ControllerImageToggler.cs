using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000122 RID: 290
public class ControllerImageToggler : MonoBehaviour
{
	// Token: 0x060005B0 RID: 1456 RVA: 0x00020714 File Offset: 0x0001E914
	private void Awake()
	{
		this.selector = base.GetComponentInParent<CharacterSelectionInstance>();
		this.portrait = base.GetComponentInParent<CharacterCreatorPortrait>();
		if (this.portrait && this.portrait.controlType != MenuControllerHandler.MenuControl.Unassigned)
		{
			this.Switch(this.portrait.controlType);
		}
	}

	// Token: 0x060005B1 RID: 1457 RVA: 0x00020768 File Offset: 0x0001E968
	private void Start()
	{
		this.img = base.GetComponent<Image>();
		MenuControllerHandler instance = MenuControllerHandler.instance;
		instance.switchControlAction = (Action<MenuControllerHandler.MenuControl>)Delegate.Combine(instance.switchControlAction, new Action<MenuControllerHandler.MenuControl>(this.Switch));
		if (this.selector)
		{
			return;
		}
		if (this.portrait.controlType == MenuControllerHandler.MenuControl.Unassigned)
		{
			this.Switch(MenuControllerHandler.menuControl);
		}
	}

	// Token: 0x060005B2 RID: 1458 RVA: 0x000207D0 File Offset: 0x0001E9D0
	private void Update()
	{
		if (this.selector && this.selector.currentPlayer)
		{
			if (this.selector.currentPlayer.data.input.inputType == GeneralInput.InputType.Controller)
			{
				this.img.sprite = this.controllerSprite;
				return;
			}
			this.img.sprite = this.MKSprite;
		}
	}

	// Token: 0x060005B3 RID: 1459 RVA: 0x0002083C File Offset: 0x0001EA3C
	private void Switch(MenuControllerHandler.MenuControl control)
	{
		if (!this.img)
		{
			this.img = base.GetComponent<Image>();
		}
		if (this.selector)
		{
			return;
		}
		if (control == MenuControllerHandler.MenuControl.Controller)
		{
			this.img.sprite = this.controllerSprite;
			return;
		}
		this.img.sprite = this.MKSprite;
	}

	// Token: 0x0400074F RID: 1871
	public Sprite MKSprite;

	// Token: 0x04000750 RID: 1872
	public Sprite controllerSprite;

	// Token: 0x04000751 RID: 1873
	private Image img;

	// Token: 0x04000752 RID: 1874
	private CharacterCreatorPortrait portrait;

	// Token: 0x04000753 RID: 1875
	private CharacterSelectionInstance selector;
}
