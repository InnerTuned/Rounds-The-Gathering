using System;
using InControl;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000150 RID: 336
public class GoBack : MonoBehaviour
{
	// Token: 0x060006CC RID: 1740 RVA: 0x000027C8 File Offset: 0x000009C8
	private void Start()
	{
	}

	// Token: 0x060006CD RID: 1741 RVA: 0x000259D0 File Offset: 0x00023BD0
	private void Update()
	{
		for (int i = 0; i < InputManager.ActiveDevices.Count; i++)
		{
			if (InputManager.ActiveDevices[i].Action2.WasPressed)
			{
				this.goBackEvent.Invoke();
				this.target.Open();
			}
		}
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			this.goBackEvent.Invoke();
			this.target.Open();
		}
	}

	// Token: 0x04000823 RID: 2083
	public UnityEvent goBackEvent;

	// Token: 0x04000824 RID: 2084
	public ListMenuPage target;
}
