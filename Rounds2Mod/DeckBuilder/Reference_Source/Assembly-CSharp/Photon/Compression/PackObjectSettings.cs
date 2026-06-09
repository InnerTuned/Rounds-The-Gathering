using System;
using Photon.Utilities;
using UnityEngine;

namespace Photon.Compression
{
	// Token: 0x02000207 RID: 519
	public class PackObjectSettings : SettingsScriptableObject<PackObjectSettings>
	{
		// Token: 0x06000A35 RID: 2613 RVA: 0x00033CFF File Offset: 0x00031EFF
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		public static void Bootstrap()
		{
			PackObjectSettings single = SettingsScriptableObject<PackObjectSettings>.Single;
		}

		// Token: 0x04000BB2 RID: 2994
		[Header("Code Generation")]
		[Tooltip("Enables the auto generation of codegen for PackObjects / PackAttributes. Disable this if you would like to suspend codegen. Existing codegen will remain, unless it produces errors.")]
		public bool autoGenerate = true;

		// Token: 0x04000BB3 RID: 2995
		[Tooltip("Automatically deletes codegen if it produces any compile errors. Typically you will want to leave this enabled. Disable to see the actual errors being generated.")]
		public bool deleteBadCode = true;
	}
}
