using System;
using UnityEngine;

// Token: 0x0200008B RID: 139
public class ParticlePlayer : MonoBehaviour
{
	// Token: 0x060002EF RID: 751 RVA: 0x00012DCA File Offset: 0x00010FCA
	private void Awake()
	{
		ParticlePlayer.instance = this;
	}

	// Token: 0x060002F0 RID: 752 RVA: 0x00012DD2 File Offset: 0x00010FD2
	private void Update()
	{
		this.spawnsThisFrame = 0;
	}

	// Token: 0x060002F1 RID: 753 RVA: 0x00012DDC File Offset: 0x00010FDC
	public void PlayEffect(string effectName, Vector3 position, Quaternion rotation, float scale = 1f, Transform followTransform = null)
	{
		if ((float)this.spawnsThisFrame > 5f)
		{
			return;
		}
		this.spawnsThisFrame++;
		Transform transform = base.transform.Find(effectName);
		if (transform)
		{
			if (followTransform)
			{
				transform = Object.Instantiate<GameObject>(transform.gameObject, null).transform;
			}
			transform.transform.position = position;
			transform.transform.localScale = scale * Vector3.one;
			transform.transform.rotation = rotation;
			if (followTransform)
			{
				transform.gameObject.AddComponent<FollowLocalPos>().Follow(followTransform);
			}
			ParticleSystem[] componentsInChildren = transform.GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (followTransform)
				{
					componentsInChildren[i].main.simulationSpace = 0;
				}
				componentsInChildren[i].Play();
			}
		}
	}

	// Token: 0x04000428 RID: 1064
	public static ParticlePlayer instance;

	// Token: 0x04000429 RID: 1065
	private int spawnsThisFrame;
}
