using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200010D RID: 269
public class CharacterCreatorHandler : MonoBehaviour
{
	// Token: 0x06000549 RID: 1353 RVA: 0x0001E676 File Offset: 0x0001C876
	public void LockPortrait(int portrait)
	{
		this.lockedPortraits.Add(portrait);
		Action action = this.lockedPortraitAction;
		if (action == null)
		{
			return;
		}
		action.Invoke();
	}

	// Token: 0x0600054A RID: 1354 RVA: 0x0001E694 File Offset: 0x0001C894
	public void ReleasePortrait(int porttrait)
	{
		for (int i = 0; i < this.lockedPortraits.Count; i++)
		{
			if (porttrait == this.lockedPortraits[i])
			{
				this.lockedPortraits.RemoveAt(i);
				break;
			}
		}
		Action action = this.lockedPortraitAction;
		if (action == null)
		{
			return;
		}
		action.Invoke();
	}

	// Token: 0x0600054B RID: 1355 RVA: 0x0001E6E4 File Offset: 0x0001C8E4
	private void ReleaseAllPortraits()
	{
		this.lockedPortraits.Clear();
		Action action = this.lockedPortraitAction;
		if (action == null)
		{
			return;
		}
		action.Invoke();
	}

	// Token: 0x0600054C RID: 1356 RVA: 0x0001E704 File Offset: 0x0001C904
	private void Awake()
	{
		CharacterCreatorHandler.instance = this;
		for (int i = 0; i < this.playerFaces.Length; i++)
		{
			this.playerFaces[i].LoadFace(i.ToString());
		}
		for (int j = 0; j < this.selectedFaceID.Length; j++)
		{
			this.selectedFaceID[j] = PlayerPrefs.GetInt("SelectedFace" + j);
			this.SelectFace(j, this.playerFaces[this.selectedFaceID[j]], this.selectedFaceID[j]);
		}
	}

	// Token: 0x0600054D RID: 1357 RVA: 0x0001E78C File Offset: 0x0001C98C
	internal void SelectFace(int faceID, PlayerFace selectedFace, int faceSlot)
	{
		this.selectedFaceID[faceID] = faceSlot;
		PlayerPrefs.SetInt("SelectedFace" + faceID, this.selectedFaceID[faceID]);
		this.selectedPlayerFaces[faceID] = selectedFace;
	}

	// Token: 0x0600054E RID: 1358 RVA: 0x0001E7BD File Offset: 0x0001C9BD
	public PlayerFace GetFacePreset(int faceID)
	{
		return this.playerFaces[faceID];
	}

	// Token: 0x0600054F RID: 1359 RVA: 0x0001E7C8 File Offset: 0x0001C9C8
	public bool SomeoneIsEditing()
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			if (base.transform.GetChild(i).gameObject.activeSelf)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000550 RID: 1360 RVA: 0x0001E806 File Offset: 0x0001CA06
	internal void SetFacePreset(int faceID, PlayerFace currentPlayerFace)
	{
		this.playerFaces[faceID] = PlayerFace.CopyFace(currentPlayerFace);
		this.playerFaces[faceID].SaveFace(faceID.ToString());
		Action<int> action = this.faceWasUpdatedAction;
		if (action == null)
		{
			return;
		}
		action.Invoke(faceID);
	}

	// Token: 0x06000551 RID: 1361 RVA: 0x0001E83C File Offset: 0x0001CA3C
	public void EditCharacterLocalMultiplayer(int playerId, int portraitID, GameObject objectToEnable, PlayerFace currentFace)
	{
		this.LockPortrait(portraitID);
		CharacterCreator component = base.transform.GetChild(playerId + 1).GetComponent<CharacterCreator>();
		component.playerActions = PlayerManager.instance.players[playerId].data.playerActions;
		component.inputType = PlayerManager.instance.players[playerId].data.input.inputType;
		component.gameObject.SetActive(true);
		component.playerID = playerId;
		component.objectToEnable = objectToEnable;
		component.currentPlayerFace = currentFace;
		component.portraitID = portraitID;
		component.SpawnFace(currentFace);
	}

	// Token: 0x06000552 RID: 1362 RVA: 0x0001E8D8 File Offset: 0x0001CAD8
	public void EditCharacterPortrait(int portraitID, PlayerFace currentFace)
	{
		this.LockPortrait(portraitID);
		MainMenuHandler.instance.Close();
		CharacterCreator component = base.transform.GetChild(0).GetComponent<CharacterCreator>();
		component.inputType = GeneralInput.InputType.Either;
		component.currentPlayerFace = currentFace;
		component.gameObject.SetActive(true);
		component.playerID = 0;
		component.portraitID = portraitID;
		component.SpawnFace(currentFace);
	}

	// Token: 0x06000553 RID: 1363 RVA: 0x0001E938 File Offset: 0x0001CB38
	public void CloseMenus()
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			base.transform.GetChild(i).GetComponent<CharacterCreator>().Close();
		}
		this.ReleaseAllPortraits();
	}

	// Token: 0x06000554 RID: 1364 RVA: 0x0001E977 File Offset: 0x0001CB77
	public void EndCustomization(int playerId = -1)
	{
		if (playerId == -1)
		{
			base.GetComponentInChildren<CharacterCreator>(true).Finish();
			base.transform.GetChild(0).gameObject.SetActive(false);
		}
	}

	// Token: 0x040006E7 RID: 1767
	public static CharacterCreatorHandler instance;

	// Token: 0x040006E8 RID: 1768
	public PlayerFace[] playerFaces = new PlayerFace[10];

	// Token: 0x040006E9 RID: 1769
	public PlayerFace[] selectedPlayerFaces = new PlayerFace[4];

	// Token: 0x040006EA RID: 1770
	public int[] selectedFaceID = new int[4];

	// Token: 0x040006EB RID: 1771
	public List<int> lockedPortraits;

	// Token: 0x040006EC RID: 1772
	public Action lockedPortraitAction;

	// Token: 0x040006ED RID: 1773
	public Action<int> faceWasUpdatedAction;
}
