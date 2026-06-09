using System;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x02000270 RID: 624
	public static class ProjectileHelpers
	{
		// Token: 0x06000D8A RID: 3466 RVA: 0x00042448 File Offset: 0x00040648
		public static GameObject GetPlaceholderProj()
		{
			if (ProjectileHelpers.prefab != null)
			{
				return ProjectileHelpers.prefab;
			}
			GameObject gameObject = new GameObject("Projectile Placeholder Prefab");
			gameObject.gameObject.SetActive(false);
			Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
			rigidbody.useGravity = false;
			rigidbody.interpolation = 1;
			rigidbody.collisionDetectionMode = 2;
			gameObject.AddComponent<ContactProjectile>();
			gameObject.AddComponent<ContactTrigger>();
			GameObject gameObject2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.GetComponent<Collider>().isTrigger = true;
			gameObject2.GetComponent<Renderer>().material.color = Color.yellow;
			gameObject2.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
			ProjectileHelpers.prefab = gameObject;
			return gameObject;
		}

		// Token: 0x04000CE5 RID: 3301
		public static GameObject prefab;
	}
}
