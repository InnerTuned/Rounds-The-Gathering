using System;
using InControl;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000113 RID: 275
public class CharacterCreatorPortrait : MonoBehaviour
{
	// Token: 0x06000574 RID: 1396 RVA: 0x0001F624 File Offset: 0x0001D824
	private void Start()
	{
		this.defColor = base.transform.Find("BG").GetComponent<Image>().color;
		CharacterCreatorHandler instance = CharacterCreatorHandler.instance;
		instance.faceWasUpdatedAction = (Action<int>)Delegate.Combine(instance.faceWasUpdatedAction, new Action<int>(this.FaceUpdated));
		this.myFace = CharacterCreatorHandler.instance.GetFacePreset(base.transform.GetSiblingIndex());
		this.hoverEvent = base.GetComponent<HoverEvent>();
		base.GetComponentInChildren<CharacterCreatorItemEquipper>().EquipFace(this.myFace);
		if (CharacterCreatorHandler.instance.selectedFaceID[0] == base.transform.GetSiblingIndex())
		{
			this.ClickButton();
		}
		CharacterCreatorHandler instance2 = CharacterCreatorHandler.instance;
		instance2.lockedPortraitAction = (Action)Delegate.Combine(instance2.lockedPortraitAction, new Action(this.CheckLocked));
		this.CheckLocked();
	}

	// Token: 0x06000575 RID: 1397 RVA: 0x0001F6FC File Offset: 0x0001D8FC
	private void CheckLocked()
	{
		this.isLocked = false;
		for (int i = 0; i < CharacterCreatorHandler.instance.lockedPortraits.Count; i++)
		{
			if (base.transform.GetSiblingIndex() == CharacterCreatorHandler.instance.lockedPortraits[i])
			{
				this.isLocked = true;
			}
		}
		if (this.isLocked)
		{
			this.lockedObj.SetActive(true);
			return;
		}
		this.lockedObj.SetActive(false);
	}

	// Token: 0x06000576 RID: 1398 RVA: 0x0001F76F File Offset: 0x0001D96F
	private void FaceUpdated(int faceID)
	{
		if (faceID == base.transform.GetSiblingIndex())
		{
			this.myFace = CharacterCreatorHandler.instance.GetFacePreset(faceID);
			base.GetComponent<CharacterCreatorItemEquipper>().EquipFace(this.myFace);
		}
	}

	// Token: 0x06000577 RID: 1399 RVA: 0x0001F7A1 File Offset: 0x0001D9A1
	public void ClickButton()
	{
		CharacterCreatorHandler.instance.SelectFace(Mathf.Clamp(this.playerId, 0, 10), this.myFace, base.transform.GetSiblingIndex());
		this.ShownFace();
	}

	// Token: 0x06000578 RID: 1400 RVA: 0x0001F7D4 File Offset: 0x0001D9D4
	private void ShownFace()
	{
		for (int i = 0; i < base.transform.parent.childCount; i++)
		{
			if (base.transform.parent.GetChild(i) == base.transform)
			{
				base.transform.parent.GetChild(i).Find("Frame").gameObject.SetActive(true);
			}
			else
			{
				base.transform.parent.GetChild(i).Find("Frame").gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06000579 RID: 1401 RVA: 0x0001F86C File Offset: 0x0001DA6C
	public void EditCharacter()
	{
		if (this.isLocked)
		{
			return;
		}
		this.ClickButton();
		if (this.playerId == -1)
		{
			CharacterCreatorHandler.instance.EditCharacterPortrait(base.transform.GetSiblingIndex(), this.myFace);
			return;
		}
		GameObject gameObject = base.transform.parent.parent.parent.gameObject;
		gameObject.SetActive(false);
		CharacterCreatorHandler.instance.EditCharacterLocalMultiplayer(this.playerId, base.transform.GetSiblingIndex(), gameObject, this.myFace);
	}

	// Token: 0x0600057A RID: 1402 RVA: 0x0001F8F4 File Offset: 0x0001DAF4
	private void Update()
	{
		for (int i = 0; i < InputManager.ActiveDevices.Count; i++)
		{
			if (InputManager.ActiveDevices[i].Action4.WasPressed && this.hoverEvent.isSelected)
			{
				this.EditCharacter();
			}
		}
		if (Input.GetKeyDown(KeyCode.Mouse1) && this.controlType != MenuControllerHandler.MenuControl.Controller && this.hoverEvent.isHovered)
		{
			this.EditCharacter();
		}
	}

	// Token: 0x04000711 RID: 1809
	public int playerId = -1;

	// Token: 0x04000712 RID: 1810
	public PlayerFace myFace;

	// Token: 0x04000713 RID: 1811
	public GameObject selectedObj;

	// Token: 0x04000714 RID: 1812
	public MenuControllerHandler.MenuControl controlType = MenuControllerHandler.MenuControl.Unassigned;

	// Token: 0x04000715 RID: 1813
	private Color defColor;

	// Token: 0x04000716 RID: 1814
	private HoverEvent hoverEvent;

	// Token: 0x04000717 RID: 1815
	public GameObject lockedObj;

	// Token: 0x04000718 RID: 1816
	private bool isLocked;
}
