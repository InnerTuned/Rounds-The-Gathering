using System;
using Sonigon;
using UnityEngine;

// Token: 0x0200012B RID: 299
public class DestructibleBoxDestruction : MonoBehaviour
{
	// Token: 0x060005CB RID: 1483 RVA: 0x00020BE4 File Offset: 0x0001EDE4
	private void Start()
	{
		DamagableEvent componentInParent = base.GetComponentInParent<DamagableEvent>();
		componentInParent.DieAction = (Action<Vector2>)Delegate.Combine(componentInParent.DieAction, new Action<Vector2>(this.Die));
	}

	// Token: 0x060005CC RID: 1484 RVA: 0x00020C10 File Offset: 0x0001EE10
	private void Die(Vector2 dmg)
	{
		if (this.soundPlayDestruction)
		{
			SoundManager.Instance.PlayAtPosition(this.soundBoxDestruction, SoundManager.Instance.GetTransform(), base.transform);
		}
		Rigidbody2D[] componentsInChildren = base.GetComponentsInChildren<Rigidbody2D>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (base.transform != componentsInChildren[i].transform)
			{
				componentsInChildren[i].transform.SetParent(base.transform.root);
				componentsInChildren[i].transform.gameObject.SetActive(true);
				componentsInChildren[i].AddForce(dmg * Random.Range(0f, 1f) * 500f, 1);
				componentsInChildren[i].AddTorque(Random.Range(-1f, 1f) * 1000f, 1);
				componentsInChildren[i].GetComponent<RemoveAfterSeconds>().seconds = Random.Range(0f, 0.5f);
				componentsInChildren[i].GetComponentInChildren<GetColor>().Start();
				componentsInChildren[i].GetComponentInChildren<ColorBlink>().timeAmount *= Random.Range(0.5f, 2f);
				componentsInChildren[i].GetComponentInChildren<ColorBlink>().DoBlink();
				componentsInChildren[i].gameObject.layer = 18;
			}
		}
	}

	// Token: 0x0400076A RID: 1898
	public bool soundPlayDestruction;

	// Token: 0x0400076B RID: 1899
	public SoundEvent soundBoxDestruction;
}
