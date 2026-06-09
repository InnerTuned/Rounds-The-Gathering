using System;
using UnityEngine;

// Token: 0x02000166 RID: 358
public class MainMenuHandler : MonoBehaviour
{
	// Token: 0x06000730 RID: 1840 RVA: 0x0002708D File Offset: 0x0002528D
	private void Awake()
	{
		MainMenuHandler.instance = this;
	}

	// Token: 0x06000731 RID: 1841 RVA: 0x00027095 File Offset: 0x00025295
	public void Close()
	{
		this.isOpen = false;
		base.transform.GetChild(0).gameObject.SetActive(false);
	}

	// Token: 0x06000732 RID: 1842 RVA: 0x000270B5 File Offset: 0x000252B5
	public void Open()
	{
		this.isOpen = true;
		base.transform.GetChild(0).gameObject.SetActive(true);
	}

	// Token: 0x0400089F RID: 2207
	public static MainMenuHandler instance;

	// Token: 0x040008A0 RID: 2208
	public bool isOpen = true;
}
