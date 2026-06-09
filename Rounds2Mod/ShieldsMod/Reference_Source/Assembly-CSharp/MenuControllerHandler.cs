using System;
using InControl;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000169 RID: 361
public class MenuControllerHandler : MonoBehaviour
{
	// Token: 0x06000741 RID: 1857 RVA: 0x00027845 File Offset: 0x00025A45
	private void Awake()
	{
		MenuControllerHandler.instance = this;
	}

	// Token: 0x06000742 RID: 1858 RVA: 0x0002784D File Offset: 0x00025A4D
	private void Start()
	{
		this.Switch();
	}

	// Token: 0x06000743 RID: 1859 RVA: 0x00027858 File Offset: 0x00025A58
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			MenuControllerHandler.menuControl = MenuControllerHandler.MenuControl.Mouse;
		}
		if (Input.GetKeyDown(KeyCode.Mouse1))
		{
			MenuControllerHandler.menuControl = MenuControllerHandler.MenuControl.Mouse;
		}
		for (int i = 0; i < InputManager.ActiveDevices.Count; i++)
		{
			InputDevice inputDevice = InputManager.ActiveDevices[i];
			if (inputDevice.AnyButtonWasPressed || Mathf.Abs(inputDevice.LeftStick.Value.y) > 0.1f)
			{
				MenuControllerHandler.menuControl = MenuControllerHandler.MenuControl.Controller;
				if (!EventSystem.current.currentSelectedGameObject.activeInHierarchy)
				{
					ListMenuButton[] array = Object.FindObjectsOfType<ListMenuButton>();
					for (int j = 0; j < array.Length; j++)
					{
						if (array[j].enabled)
						{
							ListMenu.instance.SelectButton(array[j]);
						}
					}
				}
			}
		}
		if (MenuControllerHandler.menuControl != this.lastMenuControl)
		{
			this.Switch();
		}
		this.lastMenuControl = MenuControllerHandler.menuControl;
	}

	// Token: 0x06000744 RID: 1860 RVA: 0x00027931 File Offset: 0x00025B31
	private void Switch()
	{
		Action<MenuControllerHandler.MenuControl> action = this.switchControlAction;
		if (action == null)
		{
			return;
		}
		action.Invoke(MenuControllerHandler.menuControl);
	}

	// Token: 0x040008B3 RID: 2227
	public static MenuControllerHandler.MenuControl menuControl;

	// Token: 0x040008B4 RID: 2228
	public MenuControllerHandler.MenuControl lastMenuControl;

	// Token: 0x040008B5 RID: 2229
	public Action<MenuControllerHandler.MenuControl> switchControlAction;

	// Token: 0x040008B6 RID: 2230
	public static MenuControllerHandler instance;

	// Token: 0x02000392 RID: 914
	public enum MenuControl
	{
		// Token: 0x04001214 RID: 4628
		Controller,
		// Token: 0x04001215 RID: 4629
		Mouse,
		// Token: 0x04001216 RID: 4630
		Unassigned
	}
}
