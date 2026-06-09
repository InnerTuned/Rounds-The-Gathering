using System;
using UnityEngine;

// Token: 0x0200010A RID: 266
public class CharacterCreator : MonoBehaviour
{
	// Token: 0x06000538 RID: 1336 RVA: 0x0001DC90 File Offset: 0x0001BE90
	private void Start()
	{
		this.nav = base.GetComponentInChildren<CharacterCreatorNavigation>();
	}

	// Token: 0x06000539 RID: 1337 RVA: 0x0001DC9E File Offset: 0x0001BE9E
	private void Update()
	{
		if (this.currentControl != this.lastControl)
		{
			Action<MenuControllerHandler.MenuControl> switchAction = this.SwitchAction;
			if (switchAction != null)
			{
				switchAction.Invoke(this.currentControl);
			}
		}
		this.lastControl = this.currentControl;
	}

	// Token: 0x0600053A RID: 1338 RVA: 0x0001DCD4 File Offset: 0x0001BED4
	public void Close()
	{
		CharacterCreatorHandler.instance.ReleasePortrait(this.portraitID);
		if (this.playerActions == null)
		{
			base.gameObject.SetActive(false);
			MainMenuHandler.instance.Open();
			return;
		}
		this.objectToEnable.SetActive(true);
		base.gameObject.SetActive(false);
	}

	// Token: 0x0600053B RID: 1339 RVA: 0x0001DD28 File Offset: 0x0001BF28
	public void Finish()
	{
		CharacterCreatorHandler.instance.ReleasePortrait(this.portraitID);
		CharacterCreatorHandler.instance.SetFacePreset(this.portraitID, this.currentPlayerFace);
		CharacterCreatorHandler.instance.SelectFace(0, this.currentPlayerFace, this.portraitID);
		if (this.playerActions == null)
		{
			base.gameObject.SetActive(false);
			MainMenuHandler.instance.Open();
			return;
		}
		this.objectToEnable.SetActive(true);
		base.gameObject.SetActive(false);
	}

	// Token: 0x0600053C RID: 1340 RVA: 0x0001DDAC File Offset: 0x0001BFAC
	internal void SetOffset(Vector2 offset, CharacterItemType itemType, int slotID)
	{
		if (itemType == CharacterItemType.Eyes)
		{
			this.currentPlayerFace.eyeOffset = offset;
		}
		if (itemType == CharacterItemType.Mouth)
		{
			this.currentPlayerFace.mouthOffset = offset;
		}
		if (itemType == CharacterItemType.Detail)
		{
			if (slotID == 0)
			{
				this.currentPlayerFace.detailOffset = offset;
			}
			if (slotID == 1)
			{
				this.currentPlayerFace.detail2Offset = offset;
			}
		}
	}

	// Token: 0x0600053D RID: 1341 RVA: 0x0001DDFB File Offset: 0x0001BFFB
	internal void SpawnFace(PlayerFace currentFace)
	{
		base.GetComponentInChildren<CharacterCreatorItemEquipper>().SpawnPlayerFace(currentFace);
	}

	// Token: 0x040006D5 RID: 1749
	public PlayerFace currentPlayerFace;

	// Token: 0x040006D6 RID: 1750
	public GameObject objectToEnable;

	// Token: 0x040006D7 RID: 1751
	public int playerID;

	// Token: 0x040006D8 RID: 1752
	public int portraitID;

	// Token: 0x040006D9 RID: 1753
	public bool ready;

	// Token: 0x040006DA RID: 1754
	public PlayerActions playerActions;

	// Token: 0x040006DB RID: 1755
	public GeneralInput.InputType inputType;

	// Token: 0x040006DC RID: 1756
	public MenuControllerHandler.MenuControl currentControl = MenuControllerHandler.MenuControl.Unassigned;

	// Token: 0x040006DD RID: 1757
	public MenuControllerHandler.MenuControl lastControl = MenuControllerHandler.MenuControl.Unassigned;

	// Token: 0x040006DE RID: 1758
	public Action<MenuControllerHandler.MenuControl> SwitchAction;

	// Token: 0x040006DF RID: 1759
	public CharacterCreatorNavigation nav;
}
