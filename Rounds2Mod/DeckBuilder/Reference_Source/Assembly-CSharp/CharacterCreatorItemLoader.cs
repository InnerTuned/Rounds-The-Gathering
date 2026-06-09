using System;
using UnityEngine;

// Token: 0x02000110 RID: 272
public class CharacterCreatorItemLoader : MonoBehaviour
{
	// Token: 0x06000564 RID: 1380 RVA: 0x0001F101 File Offset: 0x0001D301
	private void Awake()
	{
		CharacterCreatorItemLoader.instance = this;
	}

	// Token: 0x06000565 RID: 1381 RVA: 0x000027C8 File Offset: 0x000009C8
	private void Update()
	{
	}

	// Token: 0x06000566 RID: 1382 RVA: 0x0001F10C File Offset: 0x0001D30C
	internal CharacterItem GetItem(int itemID, CharacterItemType itemType)
	{
		CharacterItem result;
		try
		{
			if (itemType == CharacterItemType.Eyes)
			{
				result = this.eyes[itemID];
			}
			else if (itemType == CharacterItemType.Mouth)
			{
				result = this.mouths[itemID];
			}
			else
			{
				result = this.accessories[itemID];
			}
		}
		catch
		{
			result = null;
		}
		return result;
	}

	// Token: 0x06000567 RID: 1383 RVA: 0x0001F158 File Offset: 0x0001D358
	internal int GetItemID(CharacterItem newSprite, CharacterItemType itemType)
	{
		CharacterItem[] array;
		if (itemType == CharacterItemType.Eyes)
		{
			array = this.eyes;
		}
		else if (itemType == CharacterItemType.Mouth)
		{
			array = this.mouths;
		}
		else
		{
			array = this.accessories;
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].sprite == newSprite.sprite)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06000568 RID: 1384 RVA: 0x0001F1B0 File Offset: 0x0001D3B0
	public void UpdateItems(CharacterItemType target, CharacterItem[] items)
	{
		for (int i = 0; i < items.Length; i++)
		{
			items[i].sprite = items[i].GetComponent<SpriteRenderer>().sprite;
		}
		if (target == CharacterItemType.Eyes)
		{
			this.eyes = items;
		}
		if (target == CharacterItemType.Mouth)
		{
			this.mouths = items;
		}
		if (target == CharacterItemType.Detail)
		{
			this.accessories = items;
		}
	}

	// Token: 0x040006FE RID: 1790
	public CharacterItem[] eyes;

	// Token: 0x040006FF RID: 1791
	public CharacterItem[] mouths;

	// Token: 0x04000700 RID: 1792
	public CharacterItem[] accessories;

	// Token: 0x04000701 RID: 1793
	public static CharacterCreatorItemLoader instance;
}
