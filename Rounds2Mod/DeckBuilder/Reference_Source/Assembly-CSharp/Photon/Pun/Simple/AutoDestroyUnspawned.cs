using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Photon.Pun.Simple
{
	// Token: 0x020002F1 RID: 753
	[DisallowMultipleComponent]
	public class AutoDestroyUnspawned : MonoBehaviour
	{
		// Token: 0x0600102C RID: 4140 RVA: 0x0004E73C File Offset: 0x0004C93C
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		public static void DestroyUnspawned()
		{
			AutoDestroyUnspawned[] array = Object.FindObjectsOfType<AutoDestroyUnspawned>();
			for (int i = array.Length - 1; i >= 0; i--)
			{
				AutoDestroyUnspawned autoDestroyUnspawned = array[i];
				if ((!autoDestroyUnspawned.onlyIfPrefab || autoDestroyUnspawned.hasPrefabParent) && autoDestroyUnspawned.gameObject.scene == SceneManager.GetActiveScene())
				{
					Object.Destroy(autoDestroyUnspawned.gameObject);
				}
			}
		}

		// Token: 0x04000F43 RID: 3907
		public bool onlyIfPrefab = true;

		// Token: 0x04000F44 RID: 3908
		public bool hasPrefabParent;
	}
}
