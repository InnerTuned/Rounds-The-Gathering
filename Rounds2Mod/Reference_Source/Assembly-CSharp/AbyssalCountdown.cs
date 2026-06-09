using System;
using Sonigon;
using Sonigon.Internal;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;

// Token: 0x020000FB RID: 251
public class AbyssalCountdown : MonoBehaviour
{
	// Token: 0x06000500 RID: 1280 RVA: 0x0001C87C File Offset: 0x0001AA7C
	private void Start()
	{
		this.soundCounterLast = this.counter;
		this.data = base.GetComponentInParent<CharacterData>();
		HealthHandler healthHandler = this.data.healthHandler;
		healthHandler.reviveAction = (Action)Delegate.Combine(healthHandler.reviveAction, new Action(this.ResetStuff));
		base.GetComponentInParent<ChildRPC>().childRPCs.Add("Abyssal", new Action(this.RPCA_Activate));
	}

	// Token: 0x06000501 RID: 1281 RVA: 0x0001C8F0 File Offset: 0x0001AAF0
	private void OnDestroy()
	{
		HealthHandler healthHandler = this.data.healthHandler;
		healthHandler.reviveAction = (Action)Delegate.Combine(healthHandler.reviveAction, new Action(this.ResetStuff));
		base.GetComponentInParent<ChildRPC>().childRPCs.Remove("Abyssal");
		this.SoundStop();
	}

	// Token: 0x06000502 RID: 1282 RVA: 0x0001C945 File Offset: 0x0001AB45
	private void OnDisable()
	{
		this.SoundStop();
	}

	// Token: 0x06000503 RID: 1283 RVA: 0x0001C94D File Offset: 0x0001AB4D
	private void SoundPlay()
	{
		if (!this.soundChargeIsPlaying)
		{
			this.soundChargeIsPlaying = true;
			SoundManager.Instance.Play(this.soundAbyssalChargeLoop, base.transform, new SoundParameterBase[]
			{
				this.soundParameterIntensity
			});
		}
	}

	// Token: 0x06000504 RID: 1284 RVA: 0x0001C983 File Offset: 0x0001AB83
	private void SoundStop()
	{
		if (this.soundChargeIsPlaying)
		{
			this.soundChargeIsPlaying = false;
			SoundManager.Instance.Stop(this.soundAbyssalChargeLoop, base.transform, true);
		}
	}

	// Token: 0x06000505 RID: 1285 RVA: 0x0001C9AC File Offset: 0x0001ABAC
	private void ResetStuff()
	{
		this.SoundStop();
		this.remainingDuration = 0f;
		this.counter = 0f;
		if (this.isAbyssalForm)
		{
			for (int i = 0; i < this.abyssalObjects.Length; i++)
			{
				this.abyssalObjects[i].gameObject.SetActive(false);
			}
			this.data.maxHealth /= this.hpMultiplier;
			this.data.health /= this.hpMultiplier;
			this.data.stats.ConfigureMassAndSize();
			this.isAbyssalForm = false;
			this.rotator.gameObject.SetActive(false);
			this.still.gameObject.SetActive(false);
		}
		this.SoundStop();
	}

	// Token: 0x06000506 RID: 1286 RVA: 0x0001CA75 File Offset: 0x0001AC75
	private void RPCA_Activate()
	{
		this.remainingDuration = this.duration;
	}

	// Token: 0x06000507 RID: 1287 RVA: 0x0001CA84 File Offset: 0x0001AC84
	private void Update()
	{
		if (this.soundCounterLast < this.counter)
		{
			this.SoundPlay();
		}
		else
		{
			this.SoundStop();
		}
		this.soundCounterLast = this.counter;
		this.soundParameterIntensity.intensity = this.counter;
		this.outerRing.fillAmount = this.counter;
		this.fill.fillAmount = this.counter;
		this.rotator.transform.localEulerAngles = new Vector3(0f, 0f, -Mathf.Lerp(0f, 360f, this.counter));
		if (!this.data.playerVel.simulated)
		{
			this.startCounter = 1f;
			return;
		}
		this.startCounter -= TimeHandler.deltaTime;
		if (this.startCounter > 0f)
		{
			return;
		}
		if (this.remainingDuration > 0f)
		{
			if (!this.isAbyssalForm)
			{
				for (int i = 0; i < this.abyssalObjects.Length; i++)
				{
					this.abyssalObjects[i].gameObject.SetActive(true);
				}
				this.data.maxHealth *= this.hpMultiplier;
				this.data.health *= this.hpMultiplier;
				this.data.stats.ConfigureMassAndSize();
				this.isAbyssalForm = true;
			}
			this.remainingDuration -= TimeHandler.deltaTime;
			this.counter = this.remainingDuration / this.duration;
			return;
		}
		if (this.isAbyssalForm)
		{
			for (int j = 0; j < this.abyssalObjects.Length; j++)
			{
				this.abyssalObjects[j].gameObject.SetActive(false);
			}
			this.data.maxHealth /= this.hpMultiplier;
			this.data.health /= this.hpMultiplier;
			this.data.stats.ConfigureMassAndSize();
			this.isAbyssalForm = false;
		}
		if (this.data.input.direction == Vector3.zero || this.data.input.direction == Vector3.down)
		{
			this.counter += TimeHandler.deltaTime / this.timeToFill;
		}
		else
		{
			this.counter -= TimeHandler.deltaTime / this.timeToEmpty;
		}
		this.counter = Mathf.Clamp(this.counter, -0.1f / this.timeToFill, 1f);
		if (this.counter >= 1f && this.data.view.IsMine)
		{
			this.remainingDuration = this.duration;
			base.GetComponentInParent<ChildRPC>().CallFunction("Abyssal");
		}
		if (this.counter <= 0f)
		{
			this.rotator.gameObject.SetActive(false);
			this.still.gameObject.SetActive(false);
			return;
		}
		this.rotator.gameObject.SetActive(true);
		this.still.gameObject.SetActive(true);
	}

	// Token: 0x04000694 RID: 1684
	public SoundEvent soundAbyssalChargeLoop;

	// Token: 0x04000695 RID: 1685
	private bool soundChargeIsPlaying;

	// Token: 0x04000696 RID: 1686
	private float soundCounterLast;

	// Token: 0x04000697 RID: 1687
	private SoundParameterIntensity soundParameterIntensity = new SoundParameterIntensity(0f, 0);

	// Token: 0x04000698 RID: 1688
	[Range(0f, 1f)]
	public float counter;

	// Token: 0x04000699 RID: 1689
	public float timeToFill = 10f;

	// Token: 0x0400069A RID: 1690
	public float timeToEmpty = 3f;

	// Token: 0x0400069B RID: 1691
	public float duration;

	// Token: 0x0400069C RID: 1692
	public float hpMultiplier = 2f;

	// Token: 0x0400069D RID: 1693
	public ProceduralImage outerRing;

	// Token: 0x0400069E RID: 1694
	public ProceduralImage fill;

	// Token: 0x0400069F RID: 1695
	public Transform rotator;

	// Token: 0x040006A0 RID: 1696
	public Transform still;

	// Token: 0x040006A1 RID: 1697
	private CharacterData data;

	// Token: 0x040006A2 RID: 1698
	public GameObject[] abyssalObjects;

	// Token: 0x040006A3 RID: 1699
	private float remainingDuration;

	// Token: 0x040006A4 RID: 1700
	private bool isAbyssalForm;

	// Token: 0x040006A5 RID: 1701
	private float startCounter;
}
