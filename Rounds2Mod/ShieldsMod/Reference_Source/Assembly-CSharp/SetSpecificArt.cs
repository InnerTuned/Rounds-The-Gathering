using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x020001AF RID: 431
public class SetSpecificArt : MonoBehaviour
{
	// Token: 0x06000898 RID: 2200 RVA: 0x0002D8DC File Offset: 0x0002BADC
	private void Start()
	{
		if (this.playOnStart)
		{
			if (this.profile)
			{
				ArtHandler.instance.ApplyPost(this.profile);
			}
			if (this.artName != "")
			{
				ArtHandler.instance.SetSpecificArt(this.artName);
			}
		}
	}

	// Token: 0x06000899 RID: 2201 RVA: 0x0002D930 File Offset: 0x0002BB30
	public void Go()
	{
		if (this.profile)
		{
			ArtHandler.instance.ApplyPost(this.profile);
		}
		if (this.artName != "")
		{
			ArtHandler.instance.SetSpecificArt(this.artName);
		}
	}

	// Token: 0x040009D4 RID: 2516
	public PostProcessProfile profile;

	// Token: 0x040009D5 RID: 2517
	public string artName;

	// Token: 0x040009D6 RID: 2518
	public bool playOnStart;
}
