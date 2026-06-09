using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000031 RID: 49
public class CodeAnimation : MonoBehaviour
{
	// Token: 0x060000F2 RID: 242 RVA: 0x00007548 File Offset: 0x00005748
	public void Start()
	{
		this.currentState = CodeAnimationInstance.AnimationUse.None;
		this.rectTransform = base.GetComponent<RectTransform>();
		for (int i = 0; i < this.animations.Length; i++)
		{
			this.animations[i].animationSpeed += Random.Range(-this.animations[i].animationSpeed * this.animations[i].randomSpeedAmount, this.animations[i].animationSpeed * this.animations[i].randomSpeedAmount);
			if (this.animations[i].animationUse == CodeAnimationInstance.AnimationUse.In)
			{
				float d = this.animations[i].curve[this.animations[i].curve.length - 1].value - this.animations[i].curve[0].value;
				if (this.rectTransform && this.animations[i].animationType == CodeAnimationInstance.AnimationType.rectPosition)
				{
					this.rectTransform.anchoredPosition += d * this.animations[i].direction * -this.animations[i].multiplier;
				}
			}
		}
		this.SetDefaults();
		if (this.playInOnAwake)
		{
			this.PlayIn();
		}
		if (this.setFirstFrame)
		{
			this.ApplyValues(this.animations[0], 0f);
		}
	}

	// Token: 0x060000F3 RID: 243 RVA: 0x000076BB File Offset: 0x000058BB
	private void OnEnable()
	{
		if (this.playInOnEnable)
		{
			base.StartCoroutine(this.DelayEnablePlay());
		}
	}

	// Token: 0x060000F4 RID: 244 RVA: 0x000076D2 File Offset: 0x000058D2
	private IEnumerator DelayEnablePlay()
	{
		yield return new WaitForSecondsRealtime(this.enablePlayDelay);
		this.PlayIn();
		yield break;
	}

	// Token: 0x060000F5 RID: 245 RVA: 0x000076E4 File Offset: 0x000058E4
	private void SetDefaults()
	{
		this.defaultScale = base.transform.localScale;
		this.defaultLocalPosition = base.transform.localPosition;
		this.defaultLocalRotation = base.transform.localRotation.eulerAngles;
		if (this.rectTransform)
		{
			this.defaultRectPosition = this.rectTransform.anchoredPosition;
		}
	}

	// Token: 0x060000F6 RID: 246 RVA: 0x00007750 File Offset: 0x00005950
	public void Animate(CodeAnimationInstance.AnimationUse use)
	{
		this.currentState = use;
		if (this.interuptAnimations)
		{
			base.StopAllCoroutines();
		}
		for (int i = 0; i < this.animations.Length; i++)
		{
			if (this.animations[i].animationUse == use)
			{
				base.StartCoroutine(this.PlayAnimations(this.animations[i]));
			}
		}
	}

	// Token: 0x060000F7 RID: 247 RVA: 0x000077AA File Offset: 0x000059AA
	[Button]
	public void PlayIn()
	{
		this.Animate(CodeAnimationInstance.AnimationUse.In);
	}

	// Token: 0x060000F8 RID: 248 RVA: 0x000077B3 File Offset: 0x000059B3
	[Button]
	public void PlayOut()
	{
		this.Animate(CodeAnimationInstance.AnimationUse.Out);
	}

	// Token: 0x060000F9 RID: 249 RVA: 0x000077BC File Offset: 0x000059BC
	[Button]
	public void PlayBoop()
	{
		this.Animate(CodeAnimationInstance.AnimationUse.Boop);
	}

	// Token: 0x060000FA RID: 250 RVA: 0x000077C5 File Offset: 0x000059C5
	private IEnumerator PlayAnimations(CodeAnimationInstance animation)
	{
		this.isPlaying = true;
		animation.startEvent.Invoke();
		base.StartCoroutine(this.DelayTimedEvent(animation.eventTiming / animation.animationSpeed, animation.timedEvent));
		float t = animation.curve.keys[animation.curve.keys.Length - 1].time;
		float c = 0f;
		while (c < t)
		{
			this.ApplyValues(animation, c);
			c += (this.useTimeScale ? TimeHandler.deltaTime : Time.unscaledDeltaTime) * animation.animationSpeed;
			yield return null;
		}
		this.ApplyValues(animation, t);
		this.isPlaying = false;
		animation.endEvent.Invoke();
		if (this.loonIn)
		{
			this.PlayIn();
		}
		yield break;
	}

	// Token: 0x060000FB RID: 251 RVA: 0x000077DC File Offset: 0x000059DC
	private void ApplyValues(CodeAnimationInstance animation, float time)
	{
		if (animation.animationType == CodeAnimationInstance.AnimationType.rectPosition && this.rectTransform)
		{
			this.rectTransform.anchoredPosition = this.defaultRectPosition + animation.direction * animation.curve.Evaluate(time) * animation.multiplier;
		}
		if (animation.animationType == CodeAnimationInstance.AnimationType.position)
		{
			base.transform.localPosition = this.defaultLocalPosition + animation.direction * animation.curve.Evaluate(time) * animation.multiplier;
		}
		if (animation.animationType == CodeAnimationInstance.AnimationType.scale)
		{
			Vector3 localScale = this.defaultScale * animation.curve.Evaluate(time) * animation.multiplier;
			if (!animation.X)
			{
				localScale.x = base.transform.localScale.x;
			}
			if (!animation.Y)
			{
				localScale.y = base.transform.localScale.y;
			}
			if (!animation.Z)
			{
				localScale.z = base.transform.localScale.z;
			}
			base.transform.localScale = localScale;
		}
		if (animation.animationType == CodeAnimationInstance.AnimationType.floatNumber)
		{
			this.animationValue = animation.curve.Evaluate(time) * animation.multiplier;
		}
		if (animation.animationType == CodeAnimationInstance.AnimationType.rotation)
		{
			base.transform.localRotation = Quaternion.Euler(animation.curve.Evaluate(time) * animation.direction + this.defaultLocalRotation);
		}
		if (this.AnimationChangeAction != null)
		{
			this.AnimationChangeAction.Invoke();
		}
	}

	// Token: 0x060000FC RID: 252 RVA: 0x00007984 File Offset: 0x00005B84
	public void AddAnimationChangeAction(Action action)
	{
		this.AnimationChangeAction = (Action)Delegate.Combine(this.AnimationChangeAction, action);
	}

	// Token: 0x060000FD RID: 253 RVA: 0x0000799D File Offset: 0x00005B9D
	private IEnumerator DelayTimedEvent(float time, UnityEvent eventToCall)
	{
		yield return new WaitForSeconds(time);
		eventToCall.Invoke();
		yield break;
	}

	// Token: 0x04000147 RID: 327
	[HideInInspector]
	public bool isPlaying;

	// Token: 0x04000148 RID: 328
	public bool loonIn;

	// Token: 0x04000149 RID: 329
	public bool playInOnAwake;

	// Token: 0x0400014A RID: 330
	public bool playInOnEnable;

	// Token: 0x0400014B RID: 331
	public float enablePlayDelay;

	// Token: 0x0400014C RID: 332
	public bool interuptAnimations = true;

	// Token: 0x0400014D RID: 333
	public bool setFirstFrame;

	// Token: 0x0400014E RID: 334
	public bool useTimeScale = true;

	// Token: 0x0400014F RID: 335
	private Vector3 defaultScale;

	// Token: 0x04000150 RID: 336
	private Vector3 defaultLocalPosition;

	// Token: 0x04000151 RID: 337
	private Vector3 defaultRectPosition;

	// Token: 0x04000152 RID: 338
	private Vector3 defaultLocalRotation;

	// Token: 0x04000153 RID: 339
	public CodeAnimationInstance[] animations;

	// Token: 0x04000154 RID: 340
	private RectTransform rectTransform;

	// Token: 0x04000155 RID: 341
	[HideInInspector]
	public CodeAnimationInstance.AnimationUse currentState;

	// Token: 0x04000156 RID: 342
	[HideInInspector]
	public float animationValue;

	// Token: 0x04000157 RID: 343
	private Action AnimationChangeAction;
}
