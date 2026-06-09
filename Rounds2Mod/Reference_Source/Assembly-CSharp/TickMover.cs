using System;
using Photon.Pun.Simple;
using UnityEngine;

// Token: 0x02000181 RID: 385
public class TickMover : MonoBehaviour, IOnPostSimulate
{
	// Token: 0x060007CF RID: 1999 RVA: 0x0002A50C File Offset: 0x0002870C
	private void Awake()
	{
		NetMasterCallbacks.RegisterCallbackInterfaces(this, true, false);
		this.rotationPerTick = new Vector3(0f, 0f, 360f * (Time.fixedDeltaTime * (float)TickEngineSettings.sendEveryXTick));
		this.tickText = base.GetComponentInChildren<TextMesh>();
		if (!this.tickText)
		{
			this.tickText = base.GetComponentInParent<TextMesh>();
		}
		if (this.tickText)
		{
			this.tickText.text = "";
		}
	}

	// Token: 0x060007D0 RID: 2000 RVA: 0x0002A58A File Offset: 0x0002878A
	private void OnDestroy()
	{
		NetMasterCallbacks.RegisterCallbackInterfaces(this, false, true);
	}

	// Token: 0x060007D1 RID: 2001 RVA: 0x0002A594 File Offset: 0x00028794
	public void OnPostSimulate(int frameId, int subFrameId, bool isNetTick)
	{
		if (!isNetTick)
		{
			return;
		}
		base.transform.eulerAngles -= this.rotationPerTick;
		if (this.tickText)
		{
			this.tickText.text = frameId.ToString();
		}
	}

	// Token: 0x0400092E RID: 2350
	private Vector3 rotationPerTick;

	// Token: 0x0400092F RID: 2351
	private TextMesh tickText;
}
