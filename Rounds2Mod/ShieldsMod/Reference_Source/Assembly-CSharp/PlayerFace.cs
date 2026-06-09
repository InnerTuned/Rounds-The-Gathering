using System;
using UnityEngine;

// Token: 0x0200010F RID: 271
[Serializable]
public class PlayerFace
{
	// Token: 0x0600055F RID: 1375 RVA: 0x0001EDC0 File Offset: 0x0001CFC0
	public void LoadFace(string key)
	{
		this.eyeID = PlayerPrefs.GetInt("eyeID-" + key);
		this.eyeOffset.x = PlayerPrefs.GetFloat("eyeOffsetX-" + key);
		this.eyeOffset.y = PlayerPrefs.GetFloat("eyeOffsetY-" + key);
		this.mouthID = PlayerPrefs.GetInt("mouthID-" + key);
		this.mouthOffset.x = PlayerPrefs.GetFloat("mouthOffsetX-" + key);
		this.mouthOffset.y = PlayerPrefs.GetFloat("mouthOffsetY-" + key);
		this.detailID = PlayerPrefs.GetInt("detailID-" + key);
		this.detailOffset.x = PlayerPrefs.GetFloat("detailOffsetX-" + key);
		this.detailOffset.y = PlayerPrefs.GetFloat("detailOffsetY-" + key);
		this.detail2ID = PlayerPrefs.GetInt("detail2ID-" + key);
		this.detail2Offset.x = PlayerPrefs.GetFloat("detail2OffsetX-" + key);
		this.detail2Offset.y = PlayerPrefs.GetFloat("detail2OffsetY-" + key);
	}

	// Token: 0x06000560 RID: 1376 RVA: 0x0001EF00 File Offset: 0x0001D100
	internal static PlayerFace CopyFace(PlayerFace currentPlayerFace)
	{
		return new PlayerFace
		{
			eyeID = currentPlayerFace.eyeID,
			eyeOffset = currentPlayerFace.eyeOffset,
			mouthID = currentPlayerFace.mouthID,
			mouthOffset = currentPlayerFace.mouthOffset,
			detailID = currentPlayerFace.detailID,
			detailOffset = currentPlayerFace.detailOffset,
			detail2ID = currentPlayerFace.detail2ID,
			detail2Offset = currentPlayerFace.detail2Offset
		};
	}

	// Token: 0x06000561 RID: 1377 RVA: 0x0001EF74 File Offset: 0x0001D174
	internal static PlayerFace CreateFace(int eyeID, Vector2 eyeOffset, int mouthID, Vector2 mouthOffset, int detailID, Vector2 detailOffset, int detail2ID, Vector2 detail2Offset)
	{
		return new PlayerFace
		{
			eyeID = eyeID,
			eyeOffset = eyeOffset,
			mouthID = mouthID,
			mouthOffset = mouthOffset,
			detailID = detailID,
			detailOffset = detailOffset,
			detail2ID = detail2ID,
			detail2Offset = detail2Offset
		};
	}

	// Token: 0x06000562 RID: 1378 RVA: 0x0001EFC4 File Offset: 0x0001D1C4
	public void SaveFace(string key)
	{
		PlayerPrefs.SetInt("eyeID-" + key, this.eyeID);
		PlayerPrefs.SetFloat("eyeOffsetX-" + key, this.eyeOffset.x);
		PlayerPrefs.SetFloat("eyeOffsetY-" + key, this.eyeOffset.y);
		PlayerPrefs.SetInt("mouthID-" + key, this.mouthID);
		PlayerPrefs.SetFloat("mouthOffsetX-" + key, this.mouthOffset.x);
		PlayerPrefs.SetFloat("mouthOffsetY-" + key, this.mouthOffset.y);
		PlayerPrefs.SetInt("detailID-" + key, this.detailID);
		PlayerPrefs.SetFloat("detailOffsetX-" + key, this.detailOffset.x);
		PlayerPrefs.SetFloat("detailOffsetY-" + key, this.detailOffset.y);
		PlayerPrefs.SetInt("detail2ID-" + key, this.detail2ID);
		PlayerPrefs.SetFloat("detail2OffsetX-" + key, this.detail2Offset.x);
		PlayerPrefs.SetFloat("detail2OffsetY-" + key, this.detail2Offset.y);
	}

	// Token: 0x040006F6 RID: 1782
	public int eyeID;

	// Token: 0x040006F7 RID: 1783
	public Vector2 eyeOffset;

	// Token: 0x040006F8 RID: 1784
	public int mouthID;

	// Token: 0x040006F9 RID: 1785
	public Vector2 mouthOffset;

	// Token: 0x040006FA RID: 1786
	public int detailID;

	// Token: 0x040006FB RID: 1787
	public Vector2 detailOffset;

	// Token: 0x040006FC RID: 1788
	public int detail2ID;

	// Token: 0x040006FD RID: 1789
	public Vector2 detail2Offset;
}
