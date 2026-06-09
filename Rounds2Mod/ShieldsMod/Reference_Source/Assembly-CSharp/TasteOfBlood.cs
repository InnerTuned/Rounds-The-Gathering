using System;
using UnityEngine;

// Token: 0x020000E5 RID: 229
public class TasteOfBlood : MonoBehaviour
{
	// Token: 0x06000485 RID: 1157 RVA: 0x0001AF2C File Offset: 0x0001912C
	private void Start()
	{
		this.part = base.GetComponentInChildren<ParticleSystem>();
		this.stats = base.GetComponentInParent<CharacterStatModifiers>();
		CharacterStatModifiers characterStatModifiers = this.stats;
		characterStatModifiers.DealtDamageAction = (Action<Vector2, bool>)Delegate.Combine(characterStatModifiers.DealtDamageAction, new Action<Vector2, bool>(this.DealtDamage));
		this.data = base.GetComponentInParent<CharacterData>();
	}

	// Token: 0x06000486 RID: 1158 RVA: 0x0001AF84 File Offset: 0x00019184
	public void DealtDamage(Vector2 damage, bool selfDamage)
	{
		if (!selfDamage)
		{
			this.damageValue += damage.magnitude;
		}
		this.damageValue = Mathf.Clamp(this.damageValue, 0f, 50f);
	}

	// Token: 0x06000487 RID: 1159 RVA: 0x0001AFB8 File Offset: 0x000191B8
	private void Update()
	{
		if (!this.data.isPlaying)
		{
			this.damageValue = 0f;
		}
		if (this.damageValue > 0f)
		{
			this.damageValue -= TimeHandler.deltaTime * this.decaySpeed;
			this.isOn = true;
		}
		else
		{
			this.isOn = false;
		}
		if (this.damageValue > 10f)
		{
			if (!this.part.isPlaying)
			{
				this.part.Play();
			}
		}
		else if (this.part.isPlaying)
		{
			this.part.Stop();
		}
		this.stats.tasteOfBloodSpeed = this.attackSpeedCurve.Evaluate(this.damageValue);
	}

	// Token: 0x04000619 RID: 1561
	public AnimationCurve attackSpeedCurve;

	// Token: 0x0400061A RID: 1562
	public float decaySpeed = 1f;

	// Token: 0x0400061B RID: 1563
	private float damageValue;

	// Token: 0x0400061C RID: 1564
	private CharacterStatModifiers stats;

	// Token: 0x0400061D RID: 1565
	private ParticleSystem part;

	// Token: 0x0400061E RID: 1566
	private CharacterData data;

	// Token: 0x0400061F RID: 1567
	private bool isOn;
}
