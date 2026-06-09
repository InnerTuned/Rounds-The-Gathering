using System;
using System.Collections;
using Sirenix.OdinInspector;
using Sonigon;
using SoundImplementation;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200003A RID: 58
public class CurveAnimation : MonoBehaviour
{
	// Token: 0x0600010F RID: 271 RVA: 0x00007CE0 File Offset: 0x00005EE0
	private void Init()
	{
		if (this.inited)
		{
			return;
		}
		this.startScale = base.transform.localScale;
		this.startLocalPos = base.transform.localScale;
		this.rectTransform = base.GetComponent<RectTransform>();
		if (this.rectTransform)
		{
			this.startAnchoredPos = this.rectTransform.anchoredPosition;
		}
		this.startRotation = base.transform.localEulerAngles;
	}

	// Token: 0x06000110 RID: 272 RVA: 0x00007D58 File Offset: 0x00005F58
	private void Awake()
	{
		this.Init();
	}

	// Token: 0x06000111 RID: 273 RVA: 0x00007D60 File Offset: 0x00005F60
	private void OnDisable()
	{
		base.StopAllCoroutines();
		this.ResetAnimationState();
		for (int i = 0; i < this.animations.Length; i++)
		{
			this.animations[i].isPlaying = false;
		}
	}

	// Token: 0x06000112 RID: 274 RVA: 0x00007D9C File Offset: 0x00005F9C
	private void OnEnable()
	{
		this.ResetAnimationState();
		for (int i = 0; i < this.animations.Length; i++)
		{
			if (this.animations[i].playOnAwake)
			{
				this.PlayAnimation(this.animations[i]);
			}
		}
	}

	// Token: 0x06000113 RID: 275 RVA: 0x00007DDF File Offset: 0x00005FDF
	[Button]
	public void PlayIn()
	{
		this.PlayAnimationWithUse(CurveAnimationUse.In);
	}

	// Token: 0x06000114 RID: 276 RVA: 0x00007DE8 File Offset: 0x00005FE8
	[Button]
	public void PlayOut()
	{
		this.PlayAnimationWithUse(CurveAnimationUse.Out);
	}

	// Token: 0x06000115 RID: 277 RVA: 0x00007DF1 File Offset: 0x00005FF1
	[Button]
	public void PlayBoop()
	{
		this.PlayAnimationWithUse(CurveAnimationUse.Boop);
	}

	// Token: 0x06000116 RID: 278 RVA: 0x00007DFA File Offset: 0x00005FFA
	[Button]
	public void Stop()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x06000117 RID: 279 RVA: 0x00007E02 File Offset: 0x00006002
	private void ResetAnimationState()
	{
		this.ApplyAnimationFrame(this.GetAnimationWithUse(CurveAnimationUse.In), 0f);
	}

	// Token: 0x06000118 RID: 280 RVA: 0x00007E18 File Offset: 0x00006018
	private CurveAnimationInstance GetAnimationWithUse(CurveAnimationUse use)
	{
		for (int i = 0; i < this.animations.Length; i++)
		{
			if (this.animations[i].animationUse == use)
			{
				return this.animations[i];
			}
		}
		return this.animations[0];
	}

	// Token: 0x06000119 RID: 281 RVA: 0x00007E5C File Offset: 0x0000605C
	public void PlayAnimation(CurveAnimationInstance animation)
	{
		for (int i = 0; i < this.soundPlay.Length; i++)
		{
			if (this.soundPlay[i] != null)
			{
				this.soundPlay[i].soundHasPlayed = false;
			}
		}
		if (this.stopAllAnimations)
		{
			base.StopAllCoroutines();
		}
		if (animation.isPlaying && animation.animation != null)
		{
			base.StopCoroutine(animation.animation);
		}
		animation.animation = base.StartCoroutine(this.DoAnimation(animation));
	}

	// Token: 0x0600011A RID: 282 RVA: 0x00007ED4 File Offset: 0x000060D4
	public void PlayAnimationWithUse(CurveAnimationUse animationUse)
	{
		for (int i = 0; i < this.soundPlay.Length; i++)
		{
			if (this.soundPlay[i] != null)
			{
				this.soundPlay[i].soundHasPlayed = false;
			}
		}
		if (this.stopAllAnimations)
		{
			base.StopAllCoroutines();
		}
		this.currentState = animationUse;
		for (int j = 0; j < this.animations.Length; j++)
		{
			if (this.animations[j].animationUse == animationUse)
			{
				if (this.animations[j].isPlaying && this.animations[j].animation != null)
				{
					base.StopCoroutine(this.animations[j].animation);
				}
				this.animations[j].animation = base.StartCoroutine(this.DoAnimation(this.animations[j]));
			}
		}
	}

	// Token: 0x0600011B RID: 283 RVA: 0x00007F96 File Offset: 0x00006196
	private IEnumerator DoAnimation(CurveAnimationInstance animation)
	{
		base.StartCoroutine(this.DelayEvent(animation.delay / animation.speed, animation.delayedEvent));
		animation.statEvent.Invoke();
		animation.isPlaying = true;
		float c = 0f;
		float t = animation.Curve().keys[animation.Curve().keys.Length - 1].time;
		while (c < t)
		{
			c += (this.useTimeScale ? (TimeHandler.deltaTime * animation.speed) : (Time.unscaledDeltaTime * animation.speed));
			for (int i = 0; i < this.soundPlay.Length; i++)
			{
				if (this.soundPlay[i] != null && this.soundPlay[i].soundEvent != null && c >= this.soundPlay[i].soundDelay && animation.animationUse == this.soundPlay[i].curveAnimationUse && !this.soundPlay[i].soundHasPlayed && SoundManager.Instance)
				{
					this.soundPlay[i].soundHasPlayed = true;
					SoundManager.Instance.Play(this.soundPlay[i].soundEvent, base.transform);
				}
			}
			this.ApplyAnimationFrame(animation, c);
			yield return null;
		}
		this.ApplyAnimationFrame(animation, t);
		animation.isPlaying = false;
		animation.endEvent.Invoke();
		if (animation.loop)
		{
			this.PlayAnimationWithUse(animation.animationUse);
		}
		yield break;
	}

	// Token: 0x0600011C RID: 284 RVA: 0x00007FAC File Offset: 0x000061AC
	private IEnumerator DelayEvent(float seconds, UnityEvent eventToCall)
	{
		yield return new WaitForSeconds(seconds);
		eventToCall.Invoke();
		yield break;
	}

	// Token: 0x0600011D RID: 285 RVA: 0x00007FC4 File Offset: 0x000061C4
	private void ApplyAnimationFrame(CurveAnimationInstance anim, float time)
	{
		if (anim.animationType == CurveAnimationType.Scale)
		{
			Vector3 vector = this.startScale * anim.Curve().Evaluate(time) * anim.multiplier;
			Vector3 localScale = new Vector3(this.X ? vector.x : base.transform.localScale.x, this.Y ? vector.y : base.transform.localScale.y, this.Z ? vector.z : base.transform.localScale.z);
			base.transform.localScale = localScale;
			return;
		}
		if (anim.animationType == CurveAnimationType.Position)
		{
			base.transform.localPosition = this.startLocalPos + anim.animDirection * anim.Curve().Evaluate(time) * anim.multiplier;
			return;
		}
		if (anim.animationType == CurveAnimationType.RectPosition)
		{
			this.rectTransform.anchoredPosition = this.startAnchoredPos + anim.animDirection * anim.Curve().Evaluate(time) * anim.multiplier;
			return;
		}
		if (anim.animationType == CurveAnimationType.Rotation)
		{
			base.transform.localEulerAngles = this.startRotation + anim.animDirection * anim.Curve().Evaluate(time) * anim.multiplier;
		}
	}

	// Token: 0x0600011E RID: 286 RVA: 0x00008140 File Offset: 0x00006340
	public bool IsPlaying()
	{
		bool result = false;
		for (int i = 0; i < this.animations.Length; i++)
		{
			if (this.animations[i].isPlaying)
			{
				result = true;
			}
		}
		return result;
	}

	// Token: 0x04000177 RID: 375
	[Header("Sound")]
	public SoundAnimationPlay[] soundPlay = new SoundAnimationPlay[0];

	// Token: 0x04000178 RID: 376
	[Header("Settings")]
	public CurveAnimationInstance[] animations;

	// Token: 0x04000179 RID: 377
	[HideInInspector]
	public Vector3 startScale;

	// Token: 0x0400017A RID: 378
	[HideInInspector]
	public Vector3 startLocalPos;

	// Token: 0x0400017B RID: 379
	[HideInInspector]
	public Vector3 startAnchoredPos;

	// Token: 0x0400017C RID: 380
	[HideInInspector]
	public Vector3 startRotation;

	// Token: 0x0400017D RID: 381
	public CurveAnimationUse currentState = CurveAnimationUse.Out;

	// Token: 0x0400017E RID: 382
	private RectTransform rectTransform;

	// Token: 0x0400017F RID: 383
	[FoldoutGroup("Global settings", 0)]
	public bool useTimeScale = true;

	// Token: 0x04000180 RID: 384
	[FoldoutGroup("Global settings", 0)]
	public bool X = true;

	// Token: 0x04000181 RID: 385
	[FoldoutGroup("Global settings", 0)]
	public bool Y = true;

	// Token: 0x04000182 RID: 386
	[FoldoutGroup("Global settings", 0)]
	public bool Z = true;

	// Token: 0x04000183 RID: 387
	[FoldoutGroup("Global settings", 0)]
	public bool stopAllAnimations;

	// Token: 0x04000184 RID: 388
	private bool inited;
}
