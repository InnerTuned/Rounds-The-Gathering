using System;
using InControl;
using UnityEngine;

// Token: 0x0200013A RID: 314
public class EscapeMenuHandler : MonoBehaviour
{
	// Token: 0x060005F2 RID: 1522 RVA: 0x000211A4 File Offset: 0x0001F3A4
	private void Start()
	{
		EscapeMenuHandler.isEscMenu = false;
	}

	// Token: 0x060005F3 RID: 1523 RVA: 0x000211AC File Offset: 0x0001F3AC
	private void Update()
	{
		if (CharacterCreatorHandler.instance.SomeoneIsEditing())
		{
			return;
		}
		if (this.canvs.activeInHierarchy)
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			this.ToggleEsc();
		}
		for (int i = 0; i < InputManager.ActiveDevices.Count; i++)
		{
			if (InputManager.ActiveDevices[i].CommandWasPressed)
			{
				this.ToggleEsc();
			}
		}
	}

	// Token: 0x060005F4 RID: 1524 RVA: 0x00021210 File Offset: 0x0001F410
	public void ToggleEsc()
	{
		EscapeMenuHandler.isEscMenu = !EscapeMenuHandler.isEscMenu;
		if (EscapeMenuHandler.isEscMenu)
		{
			ListMenu.instance.SelectButton(base.GetComponentInChildren<ListMenuPage>().firstSelected);
		}
		else
		{
			ListMenu.instance.SelectButton(base.GetComponentsInChildren<ListMenuButton>()[1]);
		}
		for (int i = 0; i < this.togglers.Length; i++)
		{
			this.togglers[i].SetActive(EscapeMenuHandler.isEscMenu);
		}
		if (EscapeMenuHandler.isEscMenu)
		{
			ListMenu.instance.SelectButton(base.GetComponentInChildren<ListMenuPage>().firstSelected);
			return;
		}
		ListMenuButton[] componentsInChildren = base.GetComponentsInChildren<ListMenuButton>(true);
		if (componentsInChildren.Length > 2)
		{
			ListMenu.instance.SelectButton(componentsInChildren[1]);
		}
	}

	// Token: 0x0400077E RID: 1918
	public static bool isEscMenu;

	// Token: 0x0400077F RID: 1919
	public GameObject[] togglers;

	// Token: 0x04000780 RID: 1920
	public GameObject canvs;
}
