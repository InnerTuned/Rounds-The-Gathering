using System;
using TMPro;
using UnityEngine;

// Token: 0x02000138 RID: 312
public class ErrorHandler : MonoBehaviour
{
	// Token: 0x060005ED RID: 1517 RVA: 0x00021168 File Offset: 0x0001F368
	private void Awake()
	{
		ErrorHandler.instance = this;
	}

	// Token: 0x060005EE RID: 1518 RVA: 0x00021170 File Offset: 0x0001F370
	public void ShowError(string context, string reason)
	{
		this.contextText.text = context;
		this.reasonText.text = reason;
		this.UI.SetActive(true);
	}

	// Token: 0x060005EF RID: 1519 RVA: 0x00021196 File Offset: 0x0001F396
	public void HideError()
	{
		this.UI.SetActive(false);
	}

	// Token: 0x0400077A RID: 1914
	public static ErrorHandler instance;

	// Token: 0x0400077B RID: 1915
	public TextMeshProUGUI contextText;

	// Token: 0x0400077C RID: 1916
	public TextMeshProUGUI reasonText;

	// Token: 0x0400077D RID: 1917
	public GameObject UI;
}
