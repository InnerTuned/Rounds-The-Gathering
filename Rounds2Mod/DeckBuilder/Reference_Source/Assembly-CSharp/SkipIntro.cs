using System;
using InControl;
using UnityEngine;

// Token: 0x020001B5 RID: 437
public class SkipIntro : MonoBehaviour
{
	// Token: 0x060008B6 RID: 2230 RVA: 0x0002DF30 File Offset: 0x0002C130
	private void Start()
	{
		if (SkipIntro.hasShown)
		{
			this.Skip();
		}
		SkipIntro.hasShown = true;
	}

	// Token: 0x060008B7 RID: 2231 RVA: 0x0002DF48 File Offset: 0x0002C148
	private void Update()
	{
		for (int i = 0; i < InputManager.ActiveDevices.Count; i++)
		{
			if (InputManager.ActiveDevices[i].AnyButton.WasPressed)
			{
				this.Skip();
			}
		}
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			this.Skip();
		}
	}

	// Token: 0x060008B8 RID: 2232 RVA: 0x0002DF96 File Offset: 0x0002C196
	private void Skip()
	{
		this.target.Open();
	}

	// Token: 0x040009F3 RID: 2547
	public static bool hasShown;

	// Token: 0x040009F4 RID: 2548
	public ListMenuPage target;
}
