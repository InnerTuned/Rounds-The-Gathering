using System;
using Sonigon;
using Sonigon.Internal;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000152 RID: 338
public class GunAmmo : MonoBehaviour
{
	// Token: 0x060006D3 RID: 1747 RVA: 0x00025C6A File Offset: 0x00023E6A
	private void Start()
	{
		this.reloadRing = this.reloadAnim.GetComponent<Image>();
		this.lastMaxAmmo = this.maxAmmo;
		this.currentAmmo = this.maxAmmo;
		this.gun = base.GetComponentInParent<Gun>();
		this.ReDrawTotalBullets();
	}

	// Token: 0x060006D4 RID: 1748 RVA: 0x00025CA7 File Offset: 0x00023EA7
	private void OnDisable()
	{
		this.SoundStopReloadInProgress();
	}

	// Token: 0x060006D5 RID: 1749 RVA: 0x00025CA7 File Offset: 0x00023EA7
	private void OnDestroy()
	{
		this.SoundStopReloadInProgress();
	}

	// Token: 0x060006D6 RID: 1750 RVA: 0x00025CAF File Offset: 0x00023EAF
	private void SoundStopReloadInProgress()
	{
		if (this.soundReloadInProgressPlaying)
		{
			this.soundReloadInProgressPlaying = false;
			SoundManager.Instance.Stop(this.soundReloadInProgressLoop, base.transform, true);
		}
	}

	// Token: 0x060006D7 RID: 1751 RVA: 0x00025CD7 File Offset: 0x00023ED7
	private float ReloadTime()
	{
		return (this.reloadTime + this.reloadTimeAdd) * this.reloadTimeMultiplier;
	}

	// Token: 0x060006D8 RID: 1752 RVA: 0x00025CF0 File Offset: 0x00023EF0
	private void Update()
	{
		if (!this.gun.isReloading)
		{
			this.SoundStopReloadInProgress();
		}
		if (this.gun.isReloading)
		{
			this.reloadCounter -= TimeHandler.deltaTime;
			if (!this.soundReloadInProgressPlaying)
			{
				this.soundReloadInProgressPlaying = true;
				this.soundReloadTime = this.ReloadTime();
				this.soundReloadInProgressIntensity.intensity = 0f;
				SoundManager.Instance.Play(this.soundReloadInProgressLoop, base.transform, new SoundParameterBase[]
				{
					this.soundReloadInProgressIntensity
				});
			}
			if (this.soundReloadInProgressPlaying && this.soundReloadTime > 0f)
			{
				this.soundReloadInProgressIntensity.intensity = 1f - this.reloadCounter / this.soundReloadTime;
			}
			if (this.reloadCounter < 0f)
			{
				this.ReloadAmmo(true);
			}
		}
		else if (this.currentAmmo != this.maxAmmo)
		{
			this.freeReloadCounter += TimeHandler.deltaTime;
			if (this.freeReloadCounter > this.ReloadTime() && this.gun.player.data.stats.automaticReload)
			{
				this.currentAmmo = this.maxAmmo;
				this.SetActiveBullets(false);
			}
			this.currentRegCounter += this.ammoReg * TimeHandler.deltaTime * (float)this.maxAmmo;
			if (this.currentRegCounter > 1f)
			{
				this.currentAmmo++;
				this.currentRegCounter = 0f;
				this.SetActiveBullets(false);
			}
		}
		if (this.currentAmmo <= 0)
		{
			if (this.reloadAnim.currentState != CurveAnimationUse.In)
			{
				this.reloadAnim.PlayIn();
			}
		}
		else if (this.reloadAnim.currentState != CurveAnimationUse.Out)
		{
			this.reloadAnim.PlayOut();
		}
		this.reloadRing.fillAmount = (this.ReloadTime() - this.reloadCounter) / this.ReloadTime();
		if (this.gun.attackSpeed > 0.4f)
		{
			this.cooldownRing.fillAmount = this.gun.ReadyAmount();
			if (this.gun.ReadyAmount() >= 1f)
			{
				this.cooldownRing.fillAmount = 0f;
			}
		}
		else
		{
			this.cooldownRing.fillAmount = 0f;
		}
		if (this.maxAmmo != this.lastMaxAmmo)
		{
			this.ReDrawTotalBullets();
		}
		this.lastMaxAmmo = this.maxAmmo;
	}

	// Token: 0x060006D9 RID: 1753 RVA: 0x00025F58 File Offset: 0x00024158
	public void ReloadAmmo(bool playSound = true)
	{
		this.gun.player.data.stats.OnReload(this.maxAmmo - this.currentAmmo);
		this.gun.isReloading = false;
		this.currentAmmo = this.maxAmmo;
		this.SoundStopReloadInProgress();
		if (playSound)
		{
			SoundManager.Instance.Play(this.soundReloadComplete, base.transform);
		}
		this.SetActiveBullets(false);
	}

	// Token: 0x060006DA RID: 1754 RVA: 0x00025FCC File Offset: 0x000241CC
	public void Shoot(GameObject projectile)
	{
		this.currentAmmo--;
		this.freeReloadCounter = 0f;
		this.SetActiveBullets(false);
		if (this.currentAmmo <= 0)
		{
			this.reloadCounter = this.ReloadTime();
			this.gun.isReloading = true;
			this.gun.player.data.stats.OnOutOfAmmp(this.maxAmmo);
		}
	}

	// Token: 0x060006DB RID: 1755 RVA: 0x0002603C File Offset: 0x0002423C
	public void ReDrawTotalBullets()
	{
		this.currentAmmo = this.maxAmmo;
		for (int i = this.populate.transform.childCount - 1; i >= 0; i--)
		{
			if (this.populate.transform.GetChild(i).gameObject.activeSelf)
			{
				Object.Destroy(this.populate.transform.GetChild(i).gameObject);
			}
		}
		this.populate.times = this.maxAmmo;
		this.populate.DoPopulate();
		this.SetActiveBullets(true);
	}

	// Token: 0x060006DC RID: 1756 RVA: 0x000260D0 File Offset: 0x000242D0
	private void SetActiveBullets(bool forceTurnOn = false)
	{
		for (int i = 1; i < this.populate.transform.childCount; i++)
		{
			if (i <= this.currentAmmo || forceTurnOn)
			{
				if (this.populate.transform.GetChild(i).GetComponent<CurveAnimation>().currentState > CurveAnimationUse.In || forceTurnOn)
				{
					this.populate.transform.GetChild(i).GetComponent<CurveAnimation>().PlayIn();
				}
			}
			else if (this.populate.transform.GetChild(i).GetComponent<CurveAnimation>().currentState != CurveAnimationUse.Out)
			{
				this.populate.transform.GetChild(i).GetComponent<CurveAnimation>().PlayOut();
			}
		}
	}

	// Token: 0x04000832 RID: 2098
	[Header("Sounds")]
	public SoundEvent soundReloadInProgressLoop;

	// Token: 0x04000833 RID: 2099
	public SoundEvent soundReloadComplete;

	// Token: 0x04000834 RID: 2100
	private SoundParameterIntensity soundReloadInProgressIntensity = new SoundParameterIntensity(0f, 0);

	// Token: 0x04000835 RID: 2101
	private bool soundReloadInProgressPlaying;

	// Token: 0x04000836 RID: 2102
	private float soundReloadTime;

	// Token: 0x04000837 RID: 2103
	[Header("Settings")]
	public int maxAmmo = 3;

	// Token: 0x04000838 RID: 2104
	private int lastMaxAmmo;

	// Token: 0x04000839 RID: 2105
	private int currentAmmo;

	// Token: 0x0400083A RID: 2106
	public float reloadTime = 1.5f;

	// Token: 0x0400083B RID: 2107
	public float reloadTimeMultiplier = 1f;

	// Token: 0x0400083C RID: 2108
	public float reloadTimeAdd;

	// Token: 0x0400083D RID: 2109
	private Gun gun;

	// Token: 0x0400083E RID: 2110
	public CurveAnimation reloadAnim;

	// Token: 0x0400083F RID: 2111
	private Image reloadRing;

	// Token: 0x04000840 RID: 2112
	public Image cooldownRing;

	// Token: 0x04000841 RID: 2113
	private float reloadCounter;

	// Token: 0x04000842 RID: 2114
	private float freeReloadCounter;

	// Token: 0x04000843 RID: 2115
	public Populate populate;

	// Token: 0x04000844 RID: 2116
	public float ammoReg;

	// Token: 0x04000845 RID: 2117
	private float currentRegCounter;
}
