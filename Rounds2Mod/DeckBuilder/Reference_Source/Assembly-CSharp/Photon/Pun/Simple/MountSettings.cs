using System;
using System.Collections.Generic;
using Photon.Utilities;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x02000293 RID: 659
	public class MountSettings : SettingsScriptableObject<MountSettings>
	{
		// Token: 0x06000E52 RID: 3666 RVA: 0x00044BD7 File Offset: 0x00042DD7
		public override void Initialize()
		{
			base.Initialize();
			MountSettings.mountTypeCount = SettingsScriptableObject<MountSettings>.Single.mountNames.Count;
			MountSettings.bitsForMountId = (MountSettings.mountTypeCount - 1).GetBitsForMaxValue();
		}

		// Token: 0x06000E53 RID: 3667 RVA: 0x00044C04 File Offset: 0x00042E04
		public static int GetIndex(string name)
		{
			return SettingsScriptableObject<MountSettings>.single.mountNames.IndexOf(name);
		}

		// Token: 0x06000E54 RID: 3668 RVA: 0x00044C16 File Offset: 0x00042E16
		public static string GetName(int index)
		{
			if (index >= MountSettings.mountTypeCount)
			{
				return null;
			}
			return SettingsScriptableObject<MountSettings>.single.mountNames[index];
		}

		// Token: 0x170000DB RID: 219
		// (get) Token: 0x06000E55 RID: 3669 RVA: 0x00044C32 File Offset: 0x00042E32
		public static int AllTrueMask
		{
			get
			{
				if (SettingsScriptableObject<MountSettings>.Single.mountNames.Count == 32)
				{
					return -1;
				}
				return (int)((1L << SettingsScriptableObject<MountSettings>.Single.mountNames.Count) - 1L);
			}
		}

		// Token: 0x06000E56 RID: 3670 RVA: 0x00044C62 File Offset: 0x00042E62
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void Bootstrap()
		{
			MountSettings single = SettingsScriptableObject<MountSettings>.Single;
		}

		// Token: 0x06000E57 RID: 3671 RVA: 0x00044C6C File Offset: 0x00042E6C
		public MountSettings()
		{
			List<string> list = new List<string>();
			list.Add("Root");
			list.Add("1");
			list.Add("2");
			list.Add("3");
			list.Add("4");
			this.mountNames = list;
			base..ctor();
		}

		// Token: 0x04000D6D RID: 3437
		[HideInInspector]
		[SerializeField]
		private List<string> mountNames;

		// Token: 0x04000D6E RID: 3438
		public static int mountTypeCount;

		// Token: 0x04000D6F RID: 3439
		public static int bitsForMountId;
	}
}
