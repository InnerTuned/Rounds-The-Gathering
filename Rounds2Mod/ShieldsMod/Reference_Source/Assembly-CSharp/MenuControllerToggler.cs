using System;
using UnityEngine;

// Token: 0x0200016A RID: 362
public class MenuControllerToggler : MonoBehaviour
{
	// Token: 0x06000746 RID: 1862 RVA: 0x00027948 File Offset: 0x00025B48
	private void Awake()
	{
		CharacterCreator componentInParent = base.GetComponentInParent<CharacterCreator>();
		if (componentInParent.playerActions != null)
		{
			return;
		}
		if (this.creatorControl)
		{
			CharacterCreator characterCreator = componentInParent;
			characterCreator.SwitchAction = (Action<MenuControllerHandler.MenuControl>)Delegate.Combine(characterCreator.SwitchAction, new Action<MenuControllerHandler.MenuControl>(this.Switch));
			this.Switch(base.GetComponentInParent<CharacterCreator>().currentControl);
			return;
		}
		MenuControllerHandler instance = MenuControllerHandler.instance;
		instance.switchControlAction = (Action<MenuControllerHandler.MenuControl>)Delegate.Combine(instance.switchControlAction, new Action<MenuControllerHandler.MenuControl>(this.Switch));
		this.Switch(MenuControllerHandler.menuControl);
	}

	// Token: 0x06000747 RID: 1863 RVA: 0x000279D4 File Offset: 0x00025BD4
	private void OnEnable()
	{
		CharacterCreator componentInParent = base.GetComponentInParent<CharacterCreator>();
		if (componentInParent.playerActions != null)
		{
			CharacterCreator characterCreator = componentInParent;
			characterCreator.SwitchAction = (Action<MenuControllerHandler.MenuControl>)Delegate.Combine(characterCreator.SwitchAction, new Action<MenuControllerHandler.MenuControl>(this.Switch));
			if (componentInParent.inputType == GeneralInput.InputType.Controller)
			{
				this.Switch(MenuControllerHandler.MenuControl.Controller);
			}
			if (componentInParent.inputType == GeneralInput.InputType.Keyboard)
			{
				this.Switch(MenuControllerHandler.MenuControl.Mouse);
			}
		}
	}

	// Token: 0x06000748 RID: 1864 RVA: 0x00027A34 File Offset: 0x00025C34
	private void Switch(MenuControllerHandler.MenuControl control)
	{
		if (control == MenuControllerHandler.MenuControl.Controller)
		{
			if (this.controllerObject)
			{
				this.controllerObject.SetActive(true);
			}
			if (this.keyboardObject)
			{
				this.keyboardObject.SetActive(false);
				return;
			}
		}
		else
		{
			if (this.controllerObject)
			{
				this.controllerObject.SetActive(false);
			}
			if (this.keyboardObject)
			{
				this.keyboardObject.SetActive(true);
			}
		}
	}

	// Token: 0x040008B7 RID: 2231
	public bool creatorControl;

	// Token: 0x040008B8 RID: 2232
	public GameObject controllerObject;

	// Token: 0x040008B9 RID: 2233
	public GameObject keyboardObject;

	// Token: 0x040008BA RID: 2234
	private CharacterCreator creator;
}
