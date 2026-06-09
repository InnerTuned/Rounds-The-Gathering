using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200008A RID: 138
public class ParticleExplosionModifier : MonoBehaviour
{
	// Token: 0x060002EB RID: 747 RVA: 0x00012D08 File Offset: 0x00010F08
	private void Start()
	{
		this.effect = base.GetComponent<ParticleSystem>();
		this.main = this.effect.main;
		Explosion componentInParent = base.GetComponentInParent<Explosion>();
		componentInParent.DealDamageAction = (Action<Damagable>)Delegate.Combine(componentInParent.DealDamageAction, new Action<Damagable>(this.DealDamage));
		Explosion componentInParent2 = base.GetComponentInParent<Explosion>();
		componentInParent2.DealHealAction = (Action<Damagable>)Delegate.Combine(componentInParent2.DealHealAction, new Action<Damagable>(this.DealDamage));
	}

	// Token: 0x060002EC RID: 748 RVA: 0x00012D80 File Offset: 0x00010F80
	public void DealDamage(Damagable damagable)
	{
		if (this.corutine != null)
		{
			base.StopCoroutine(this.corutine);
		}
		this.corutine = base.StartCoroutine(this.DoCurve());
	}

	// Token: 0x060002ED RID: 749 RVA: 0x00012DA8 File Offset: 0x00010FA8
	private IEnumerator DoCurve()
	{
		float c = 0f;
		float t = this.curve.keys[this.curve.keys.Length - 1].time;
		while (c < t)
		{
			ParticleSystem.MinMaxCurve startSize = this.main.startSize;
			startSize.constantMin = this.curve.Evaluate(c) * 0.5f;
			startSize.constantMax = this.curve.Evaluate(c);
			this.main.startSize = startSize;
			c += TimeHandler.deltaTime * this.speed;
			yield return null;
		}
		yield break;
	}

	// Token: 0x04000423 RID: 1059
	public AnimationCurve curve;

	// Token: 0x04000424 RID: 1060
	public float speed = 1f;

	// Token: 0x04000425 RID: 1061
	private ParticleSystem effect;

	// Token: 0x04000426 RID: 1062
	private ParticleSystem.MainModule main;

	// Token: 0x04000427 RID: 1063
	private Coroutine corutine;
}
