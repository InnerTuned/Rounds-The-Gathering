using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200007B RID: 123
public class LineEffectExplosionModifier : MonoBehaviour
{
	// Token: 0x06000292 RID: 658 RVA: 0x00010CDC File Offset: 0x0000EEDC
	private void Start()
	{
		this.effect = base.GetComponent<LineEffect>();
		Explosion componentInParent = base.GetComponentInParent<Explosion>();
		componentInParent.DealDamageAction = (Action<Damagable>)Delegate.Combine(componentInParent.DealDamageAction, new Action<Damagable>(this.DealDamage));
		Explosion componentInParent2 = base.GetComponentInParent<Explosion>();
		componentInParent2.DealHealAction = (Action<Damagable>)Delegate.Combine(componentInParent2.DealHealAction, new Action<Damagable>(this.DealDamage));
	}

	// Token: 0x06000293 RID: 659 RVA: 0x00010D43 File Offset: 0x0000EF43
	public void DealDamage(Damagable damagable)
	{
		if (this.corutine != null)
		{
			base.StopCoroutine(this.corutine);
		}
		this.corutine = base.StartCoroutine(this.DoCurve());
	}

	// Token: 0x06000294 RID: 660 RVA: 0x00010D6B File Offset: 0x0000EF6B
	private IEnumerator DoCurve()
	{
		float c = 0f;
		float t = this.curve.keys[this.curve.keys.Length - 1].time;
		while (c < t)
		{
			this.effect.offsetMultiplier = this.curve.Evaluate(c);
			c += TimeHandler.deltaTime * this.speed;
			yield return null;
		}
		yield break;
	}

	// Token: 0x040003B3 RID: 947
	public AnimationCurve curve;

	// Token: 0x040003B4 RID: 948
	public float speed = 1f;

	// Token: 0x040003B5 RID: 949
	private LineEffect effect;

	// Token: 0x040003B6 RID: 950
	private Coroutine corutine;
}
