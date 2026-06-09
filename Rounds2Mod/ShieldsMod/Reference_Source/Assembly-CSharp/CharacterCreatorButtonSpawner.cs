using System;
using UnityEngine;

// Token: 0x0200010B RID: 267
public class CharacterCreatorButtonSpawner : MonoBehaviour
{
	// Token: 0x0600053F RID: 1343 RVA: 0x0001DE1F File Offset: 0x0001C01F
	private void Start()
	{
		this.creator = base.GetComponent<CharacterCreator>();
		this.loader = CharacterCreatorItemLoader.instance;
		this.OpenMenu(CharacterItemType.Eyes, 0);
	}

	// Token: 0x06000540 RID: 1344 RVA: 0x0001DE40 File Offset: 0x0001C040
	public void OpenMenu(int id)
	{
		if (id == 0)
		{
			this.OpenMenu(CharacterItemType.Eyes, 0);
		}
		if (id == 1)
		{
			this.OpenMenu(CharacterItemType.Mouth, 0);
		}
		if (id == 2)
		{
			this.OpenMenu(CharacterItemType.Detail, 0);
		}
		if (id == 3)
		{
			this.OpenMenu(CharacterItemType.Detail, 1);
		}
	}

	// Token: 0x06000541 RID: 1345 RVA: 0x0001DE74 File Offset: 0x0001C074
	public void OpenMenu(CharacterItemType target, int slotNr)
	{
		CharacterItem[] array = null;
		if (target == CharacterItemType.Eyes)
		{
			array = this.loader.eyes;
		}
		if (target == CharacterItemType.Mouth)
		{
			array = this.loader.mouths;
		}
		if (target == CharacterItemType.Detail)
		{
			array = this.loader.accessories;
		}
		for (int i = 0; i < this.sourceButton.transform.parent.childCount; i++)
		{
			if (this.sourceButton.transform.parent.GetChild(i).gameObject.activeSelf)
			{
				Object.Destroy(this.sourceButton.transform.parent.GetChild(i).gameObject);
			}
		}
		for (int j = 0; j < array.Length; j++)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.sourceButton, this.sourceButton.transform.parent);
			gameObject.SetActive(true);
			Transform parent = gameObject.transform.Find("ItemParent");
			GameObject gameObject2 = Object.Instantiate<GameObject>(array[j].gameObject, parent);
			gameObject.GetComponent<CharacterItemButton>().itemType = target;
			gameObject.GetComponent<CharacterItemButton>().slotNr = slotNr;
			gameObject.GetComponentInChildren<CharacterItem>().sprite = array[j].gameObject.GetComponent<SpriteRenderer>().sprite;
			gameObject2.GetComponentInChildren<CharacterItem>().GetComponent<SpriteRenderer>().sortingOrder = array[j].gameObject.GetComponent<SpriteRenderer>().sortingOrder;
			gameObject2.GetComponentInChildren<CharacterItem>().scale = array[j].scale;
			gameObject2.GetComponentInChildren<CharacterItem>().itemType = target;
			gameObject2.GetComponentInChildren<CharacterItem>().offset = array[j].offset;
			gameObject2.GetComponentInChildren<CharacterItem>().slotNr = slotNr;
			gameObject2.GetComponentInChildren<SpriteRenderer>().transform.localPosition = array[j].offset;
			gameObject2.GetComponentInChildren<SpriteRenderer>().transform.localScale = array[j].scale * Vector2.one;
			if (target == CharacterItemType.Eyes && j == this.creator.currentPlayerFace.eyeID)
			{
				gameObject.transform.Find("SelectedDot").gameObject.SetActive(true);
			}
			if (target == CharacterItemType.Mouth && j == this.creator.currentPlayerFace.mouthID)
			{
				gameObject.transform.Find("SelectedDot").gameObject.SetActive(true);
			}
			if (target == CharacterItemType.Detail && j == this.creator.currentPlayerFace.detailID && slotNr == 0)
			{
				gameObject.transform.Find("SelectedDot").gameObject.SetActive(true);
			}
			if (target == CharacterItemType.Detail && slotNr == 1 && j == this.creator.currentPlayerFace.detail2ID)
			{
				gameObject.transform.Find("SelectedDot").gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x06000542 RID: 1346 RVA: 0x0001E11C File Offset: 0x0001C31C
	public void SelectButton(CharacterItemType itemType, int slotNr)
	{
		for (int i = 0; i < this.sourceButton.transform.parent.childCount; i++)
		{
			GameObject gameObject = this.sourceButton.transform.parent.GetChild(i).gameObject;
			if (itemType == CharacterItemType.Eyes)
			{
				if (i - 1 == this.creator.currentPlayerFace.eyeID)
				{
					gameObject.transform.Find("SelectedDot").gameObject.SetActive(true);
				}
				else
				{
					gameObject.transform.Find("SelectedDot").gameObject.SetActive(false);
				}
			}
			if (itemType == CharacterItemType.Mouth)
			{
				if (i - 1 == this.creator.currentPlayerFace.mouthID)
				{
					gameObject.transform.Find("SelectedDot").gameObject.SetActive(true);
				}
				else
				{
					gameObject.transform.Find("SelectedDot").gameObject.SetActive(false);
				}
			}
			if (itemType == CharacterItemType.Detail)
			{
				if (i - 1 == this.creator.currentPlayerFace.detailID && slotNr == 0)
				{
					gameObject.transform.Find("SelectedDot").gameObject.SetActive(true);
				}
				else
				{
					gameObject.transform.Find("SelectedDot").gameObject.SetActive(false);
				}
			}
			if (itemType == CharacterItemType.Detail && slotNr == 1)
			{
				if (i - 1 == this.creator.currentPlayerFace.detail2ID)
				{
					gameObject.transform.Find("SelectedDot").gameObject.SetActive(true);
				}
				else
				{
					gameObject.transform.Find("SelectedDot").gameObject.SetActive(false);
				}
			}
		}
	}

	// Token: 0x040006E0 RID: 1760
	public GameObject sourceButton;

	// Token: 0x040006E1 RID: 1761
	private CharacterCreatorItemLoader loader;

	// Token: 0x040006E2 RID: 1762
	private CharacterCreator creator;
}
