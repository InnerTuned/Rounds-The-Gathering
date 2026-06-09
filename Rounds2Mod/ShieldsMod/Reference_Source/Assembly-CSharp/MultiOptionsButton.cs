using System;
using UnityEngine;

// Token: 0x0200016E RID: 366
public class MultiOptionsButton : MonoBehaviour
{
	// Token: 0x0600075E RID: 1886 RVA: 0x00028037 File Offset: 0x00026237
	public void Click()
	{
		base.GetComponentInParent<MultiOptions>().ClickRessButton(this);
	}

	// Token: 0x040008C3 RID: 2243
	public Resolution currentRess;

	// Token: 0x040008C4 RID: 2244
	public Optionshandler.FullScreenOption currentFull;
}
