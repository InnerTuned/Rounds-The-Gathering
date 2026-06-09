using System;
using UnityEngine;

// Token: 0x02000133 RID: 307
public class EmojiCombiner : MonoBehaviour
{
	// Token: 0x060005DF RID: 1503 RVA: 0x00020F9C File Offset: 0x0001F19C
	public void AddEmoji(GameObject newEmoji)
	{
		CharacterItem characterItem = this.FindEmojiPiece(this.currentEmoji, "M");
		CharacterItem characterItem2 = this.FindEmojiPiece(newEmoji, "M");
		float delta = this.GetDelta(characterItem, characterItem2);
		CharacterItem characterItem3 = this.FindEmojiPiece(this.currentEmoji, "E");
		CharacterItem characterItem4 = this.FindEmojiPiece(newEmoji, "E");
		float delta2 = this.GetDelta(characterItem3, characterItem4);
		if (delta > delta2)
		{
			Object.Destroy(characterItem.gameObject);
			GameObject gameObject = Object.Instantiate<GameObject>(characterItem2.gameObject);
			gameObject.transform.SetParent(this.currentEmoji.transform);
			gameObject.transform.localPosition = characterItem2.transform.localPosition;
			return;
		}
		Object.Destroy(characterItem3.gameObject);
		GameObject gameObject2 = Object.Instantiate<GameObject>(characterItem4.gameObject);
		gameObject2.transform.SetParent(this.currentEmoji.transform);
		gameObject2.transform.localPosition = characterItem4.transform.localPosition;
	}

	// Token: 0x060005E0 RID: 1504 RVA: 0x00021080 File Offset: 0x0001F280
	private float GetDelta(CharacterItem from, CharacterItem to)
	{
		return 1f;
	}

	// Token: 0x060005E1 RID: 1505 RVA: 0x00021094 File Offset: 0x0001F294
	private CharacterItem FindEmojiPiece(GameObject emoji, string target)
	{
		target = target.ToUpper();
		CharacterItem[] componentsInChildren = emoji.GetComponentsInChildren<CharacterItem>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].name.ToUpper().get_Chars(0) == target.ToCharArray()[0])
			{
				return componentsInChildren[i];
			}
		}
		return null;
	}

	// Token: 0x04000777 RID: 1911
	public GameObject currentEmoji;
}
