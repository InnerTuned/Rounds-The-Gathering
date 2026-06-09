using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200010E RID: 270
public class CharacterCreatorItemEquipper : MonoBehaviour
{
	// Token: 0x06000556 RID: 1366 RVA: 0x0001E9D0 File Offset: 0x0001CBD0
	private void Start()
	{
		this.Init();
		if (this.creator && !this.spawnedSpecific)
		{
			this.Equip(this.defaultEyes.GetComponent<CharacterItem>(), CharacterItemType.Eyes, default(Vector2), 0);
			this.Equip(this.defaultMouth.GetComponent<CharacterItem>(), CharacterItemType.Mouth, default(Vector2), 0);
		}
	}

	// Token: 0x06000557 RID: 1367 RVA: 0x0001EA30 File Offset: 0x0001CC30
	public void SpawnPlayerFace(PlayerFace newFace)
	{
		this.spawnedSpecific = true;
		this.EquipFace(newFace);
	}

	// Token: 0x06000558 RID: 1368 RVA: 0x0001EA40 File Offset: 0x0001CC40
	private void Init()
	{
		if (this.inited)
		{
			return;
		}
		this.inited = true;
		this.creator = base.GetComponent<CharacterCreator>();
		this.itemLoader = CharacterCreatorItemLoader.instance;
	}

	// Token: 0x06000559 RID: 1369 RVA: 0x0001EA6C File Offset: 0x0001CC6C
	[PunRPC]
	public void RPCA_SetFace(int eyeID, Vector2 eyeOffset, int mouthID, Vector2 mouthOffset, int detailID, Vector2 detailOffset, int detail2ID, Vector2 detail2Offset)
	{
		PlayerFace face = PlayerFace.CreateFace(eyeID, eyeOffset, mouthID, mouthOffset, detailID, detailOffset, detail2ID, detail2Offset);
		this.EquipFace(face);
	}

	// Token: 0x0600055A RID: 1370 RVA: 0x0001EA94 File Offset: 0x0001CC94
	public void EquipFace(PlayerFace face)
	{
		this.Init();
		this.Equip(this.itemLoader.GetItem(face.eyeID, CharacterItemType.Eyes), CharacterItemType.Eyes, face.eyeOffset, 0);
		this.Equip(this.itemLoader.GetItem(face.mouthID, CharacterItemType.Mouth), CharacterItemType.Mouth, face.mouthOffset, 0);
		this.Equip(this.itemLoader.GetItem(face.detailID, CharacterItemType.Detail), CharacterItemType.Detail, face.detailOffset, 0);
		this.Equip(this.itemLoader.GetItem(face.detail2ID, CharacterItemType.Detail), CharacterItemType.Detail, face.detail2Offset, 1);
	}

	// Token: 0x0600055B RID: 1371 RVA: 0x0001EB28 File Offset: 0x0001CD28
	public void Equip(CharacterItem newSprite, CharacterItemType itemType, Vector2 offset = default(Vector2), int slotNr = 0)
	{
		if (newSprite == null)
		{
			return;
		}
		this.Init();
		if (this.creator)
		{
			if (itemType == CharacterItemType.Eyes)
			{
				this.creator.currentPlayerFace.eyeID = this.itemLoader.GetItemID(newSprite, itemType);
			}
			if (itemType == CharacterItemType.Mouth)
			{
				this.creator.currentPlayerFace.mouthID = this.itemLoader.GetItemID(newSprite, itemType);
			}
			if (itemType == CharacterItemType.Detail)
			{
				if (slotNr == 0)
				{
					this.creator.currentPlayerFace.detailID = this.itemLoader.GetItemID(newSprite, itemType);
				}
				if (slotNr == 1)
				{
					this.creator.currentPlayerFace.detail2ID = this.itemLoader.GetItemID(newSprite, itemType);
				}
			}
		}
		this.Clear(itemType, slotNr);
		this.SpawnItem(newSprite, itemType, offset, slotNr);
		CopyChildren[] componentsInChildren = base.GetComponentsInChildren<CopyChildren>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].DoUpdate();
		}
	}

	// Token: 0x0600055C RID: 1372 RVA: 0x0001EC10 File Offset: 0x0001CE10
	private void SpawnItem(CharacterItem newSprite, CharacterItemType itemType, Vector2 offset = default(Vector2), int slotNr = 0)
	{
		GameObject gameObject = Object.Instantiate<GameObject>(newSprite.gameObject);
		gameObject.gameObject.SetActive(true);
		gameObject.transform.SetParent(this.itemParent.transform);
		gameObject.GetComponent<SpriteRenderer>().sprite = newSprite.GetComponent<CharacterItem>().sprite;
		gameObject.GetComponent<SpriteRenderer>().sortingOrder = newSprite.GetComponent<CharacterItem>().GetComponent<SpriteRenderer>().sortingOrder;
		gameObject.GetComponent<CharacterItem>().itemType = itemType;
		gameObject.GetComponent<SpriteRenderer>().color = newSprite.GetComponent<SpriteRenderer>().color;
		gameObject.GetComponentInChildren<CharacterItem>().offset = newSprite.GetComponent<CharacterItem>().offset;
		gameObject.GetComponentInChildren<CharacterItem>().sprite = newSprite.GetComponent<CharacterItem>().sprite;
		gameObject.GetComponentInChildren<CharacterItem>().slotNr = slotNr;
		gameObject.transform.localScale = newSprite.GetComponent<CharacterItem>().scale * Vector3.one * this.scaleM;
		gameObject.transform.localPosition = (newSprite.GetComponent<CharacterItem>().offset + offset) * this.scaleM;
		CharacterCreator characterCreator = this.creator;
		if (characterCreator == null)
		{
			return;
		}
		characterCreator.SetOffset(offset, itemType, slotNr);
	}

	// Token: 0x0600055D RID: 1373 RVA: 0x0001ED40 File Offset: 0x0001CF40
	private void Clear(CharacterItemType itemType, int slotNr = 0)
	{
		for (int i = 0; i < this.itemParent.transform.childCount; i++)
		{
			CharacterItem component = this.itemParent.transform.GetChild(i).GetComponent<CharacterItem>();
			if (component.itemType == itemType && component.slotNr == slotNr)
			{
				Object.DestroyImmediate(this.itemParent.transform.GetChild(i).gameObject);
			}
		}
	}

	// Token: 0x040006EE RID: 1774
	public GameObject itemParent;

	// Token: 0x040006EF RID: 1775
	public GameObject defaultEyes;

	// Token: 0x040006F0 RID: 1776
	public GameObject defaultMouth;

	// Token: 0x040006F1 RID: 1777
	private CharacterCreatorItemLoader itemLoader;

	// Token: 0x040006F2 RID: 1778
	private CharacterCreator creator;

	// Token: 0x040006F3 RID: 1779
	public float scaleM = 1f;

	// Token: 0x040006F4 RID: 1780
	private bool spawnedSpecific;

	// Token: 0x040006F5 RID: 1781
	private bool inited;
}
