using System;
using UnityEngine;

// Token: 0x020000DE RID: 222
public class SpawnStaticRemnant : MonoBehaviour
{
	// Token: 0x0600046D RID: 1133 RVA: 0x0001A622 File Offset: 0x00018822
	private void Start()
	{
		this.remnantColor = PlayerSkinBank.GetPlayerSkinColors(base.transform.GetComponentInParent<Player>().playerID).winText;
		this.level = base.GetComponent<AttackLevel>();
	}

	// Token: 0x0600046E RID: 1134 RVA: 0x0001A650 File Offset: 0x00018850
	public void Go()
	{
		GameObject gameObject = Object.Instantiate<GameObject>(this.remnantSource, base.transform.position, base.transform.rotation);
		SpriteRenderer[] componentsInChildren = base.transform.root.GetComponentsInChildren<SpriteRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].transform.lossyScale.x != 0f && componentsInChildren[i].transform.lossyScale.y != 0f && componentsInChildren[i].transform.lossyScale.z != 0f && (!(componentsInChildren[i].transform.parent.name != "Art") || !(componentsInChildren[i].transform.parent.parent.name != "Face")))
			{
				Vector3 lossyScale = componentsInChildren[i].transform.lossyScale;
				GameObject gameObject2 = Object.Instantiate<GameObject>(componentsInChildren[i].gameObject, componentsInChildren[i].transform.position, componentsInChildren[i].transform.rotation, gameObject.transform.GetChild(0));
				gameObject2.transform.localScale = lossyScale;
				this.Strip(gameObject2);
				SpriteRenderer component = gameObject2.GetComponent<SpriteRenderer>();
				component.enabled = true;
				component.color = this.remnantColor;
				SpriteMask component2 = gameObject2.GetComponent<SpriteMask>();
				if (component2)
				{
					Object.Destroy(component2);
				}
			}
		}
		gameObject.GetComponentInChildren<ParticleSystem>().startColor = PlayerSkinBank.GetPlayerSkinColors(base.transform.GetComponentInParent<Player>().playerID).particleEffect;
		gameObject.AddComponent<SpawnedAttack>().spawner = base.transform.root.GetComponent<Player>();
		gameObject.transform.localScale *= 1f + (float)(this.level.attackLevel - 1) * 0.3f;
	}

	// Token: 0x0600046F RID: 1135 RVA: 0x0001A838 File Offset: 0x00018A38
	private void Strip(GameObject go)
	{
		MonoBehaviour[] componentsInChildren = go.GetComponentsInChildren<MonoBehaviour>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Object.Destroy(componentsInChildren[i]);
		}
	}

	// Token: 0x040005FC RID: 1532
	public GameObject remnantSource;

	// Token: 0x040005FD RID: 1533
	private AttackLevel level;

	// Token: 0x040005FE RID: 1534
	private Color remnantColor;
}
