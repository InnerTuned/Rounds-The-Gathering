using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x0200000C RID: 12
public class ArtHandler : MonoBehaviour
{
	// Token: 0x06000038 RID: 56 RVA: 0x00003CD4 File Offset: 0x00001ED4
	private void Awake()
	{
		ArtHandler.instance = this;
	}

	// Token: 0x06000039 RID: 57 RVA: 0x00003CDC File Offset: 0x00001EDC
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.LeftShift))
		{
			this.NextArt();
		}
	}

	// Token: 0x0600003A RID: 58 RVA: 0x00003CF0 File Offset: 0x00001EF0
	public void NextArt()
	{
		for (int i = 0; i < this.arts.Length; i++)
		{
			this.arts[i].TogglePart(false);
		}
		int num = Random.Range(0, this.arts.Length);
		if (num >= this.arts.Length)
		{
			num = 0;
		}
		this.ApplyArt(this.arts[num]);
	}

	// Token: 0x0600003B RID: 59 RVA: 0x00003D48 File Offset: 0x00001F48
	private void ApplyArt(ArtInstance art)
	{
		art.TogglePart(true);
		this.currentArt = this.GetArtID(art);
		this.volume.profile = art.profile;
	}

	// Token: 0x0600003C RID: 60 RVA: 0x00003D6F File Offset: 0x00001F6F
	public void SetSpecificArt(ArtInstance art)
	{
		this.ApplyArt(art);
	}

	// Token: 0x0600003D RID: 61 RVA: 0x00003D78 File Offset: 0x00001F78
	public void ApplyPost(PostProcessProfile profileToSet)
	{
		this.volume.profile = profileToSet;
	}

	// Token: 0x0600003E RID: 62 RVA: 0x00003D88 File Offset: 0x00001F88
	public void SetSpecificArt(string artName)
	{
		for (int i = 0; i < this.arts.Length; i++)
		{
			if (this.arts[i].profile.name == artName)
			{
				this.ApplyArt(this.arts[i]);
				return;
			}
		}
	}

	// Token: 0x0600003F RID: 63 RVA: 0x00003DD4 File Offset: 0x00001FD4
	private int GetArtID(ArtInstance art)
	{
		int result = -1;
		for (int i = 0; i < this.arts.Length; i++)
		{
			if (art == this.arts[i])
			{
				result = i;
			}
		}
		return result;
	}

	// Token: 0x0400002C RID: 44
	public ArtInstance[] arts;

	// Token: 0x0400002D RID: 45
	public PostProcessVolume volume;

	// Token: 0x0400002E RID: 46
	public static ArtHandler instance;

	// Token: 0x0400002F RID: 47
	private ColorGrading colorGrading;

	// Token: 0x04000030 RID: 48
	private int currentArt = -1;
}
