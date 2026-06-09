using System;
using SoundImplementation;
using UnityEngine;

// Token: 0x02000115 RID: 277
public class CharacterItemButton : MonoBehaviour
{
	// Token: 0x0600057F RID: 1407 RVA: 0x0001FA48 File Offset: 0x0001DC48
	public void Click()
	{
		SoundPlayerStatic.Instance.PlayButtonClick();
		base.GetComponentInParent<CharacterCreatorItemEquipper>().Equip(base.gameObject.GetComponentInChildren<CharacterItem>(), this.itemType, Vector2.zero, this.slotNr);
		base.GetComponentInParent<CharacterCreatorButtonSpawner>().SelectButton(this.itemType, this.slotNr);
	}

	// Token: 0x0400071F RID: 1823
	public CharacterItemType itemType;

	// Token: 0x04000720 RID: 1824
	public int slotNr;
}
