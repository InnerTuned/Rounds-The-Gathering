using System;
using Photon.Utilities;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x020002E3 RID: 739
	[HelpURL("https://doc.photonengine.com/en-us/pun/current/gameplay/simple/simpleoverview")]
	public class NetMasterLate : MonoBehaviour
	{
		// Token: 0x06000FB1 RID: 4017 RVA: 0x0004C27B File Offset: 0x0004A47B
		private void Awake()
		{
			if (NetMasterLate.single && NetMasterLate.single != this)
			{
				Object.Destroy(NetMasterLate.single);
			}
			NetMasterLate.single = this;
			Object.DontDestroyOnLoad(this);
		}

		// Token: 0x06000FB2 RID: 4018 RVA: 0x0004C2AC File Offset: 0x0004A4AC
		private void FixedUpdate()
		{
			if (!SettingsScriptableObject<TickEngineSettings>.single.enableTickEngine)
			{
				return;
			}
			if (NetObject.activeControlledNetObjs.Count == 0 && NetObject.activeUncontrolledNetObjs.Count == 0)
			{
				return;
			}
			NetMasterCallbacks.OnPreSimulateCallbacks(NetMaster.CurrentFrameId, NetMaster.CurrentSubFrameId);
		}

		// Token: 0x06000FB3 RID: 4019 RVA: 0x0004C2E3 File Offset: 0x0004A4E3
		private void Update()
		{
			if (!SettingsScriptableObject<TickEngineSettings>.single.enableTickEngine)
			{
				return;
			}
			if (NetObject.activeControlledNetObjs.Count == 0 && NetObject.activeUncontrolledNetObjs.Count == 0)
			{
				return;
			}
			NetMasterCallbacks.OnPostUpdateCallbacks();
		}

		// Token: 0x06000FB4 RID: 4020 RVA: 0x0004C310 File Offset: 0x0004A510
		private void LateUpdate()
		{
			if (!SettingsScriptableObject<TickEngineSettings>.single.enableTickEngine)
			{
				return;
			}
			if (NetObject.activeControlledNetObjs.Count == 0 && NetObject.activeUncontrolledNetObjs.Count == 0)
			{
				return;
			}
			NetMasterCallbacks.OnPostLateUpdateCallbacks();
		}

		// Token: 0x04000EB8 RID: 3768
		public static NetMasterLate single;
	}
}
