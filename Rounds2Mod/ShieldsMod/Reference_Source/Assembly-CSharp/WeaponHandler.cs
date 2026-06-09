using System;
using System.Collections;
using Sonigon;
using UnityEngine;

// Token: 0x020000F7 RID: 247
public class WeaponHandler : MonoBehaviour
{
	// Token: 0x060004EE RID: 1262 RVA: 0x0001C147 File Offset: 0x0001A347
	internal void DoReload()
	{
		this.gun.GetComponentInChildren<GunAmmo>().ReloadAmmo(true);
	}

	// Token: 0x060004EF RID: 1263 RVA: 0x0001C15A File Offset: 0x0001A35A
	private void Awake()
	{
		this.holding = base.GetComponent<Holding>();
		this.input = base.GetComponent<GeneralInput>();
		this.data = base.GetComponent<CharacterData>();
	}

	// Token: 0x060004F0 RID: 1264 RVA: 0x0001C180 File Offset: 0x0001A380
	private void Start()
	{
		this.overHeatPivot = this.heatRenderer.transform.parent;
		this.baseHeatColor = this.heatRenderer.color;
	}

	// Token: 0x060004F1 RID: 1265 RVA: 0x0001C1AC File Offset: 0x0001A3AC
	private void Update()
	{
		if (!this.gun.holdable.holder && this.data)
		{
			this.gun.holdable.holder = this.data;
		}
		if (!this.data.playerVel.simulated)
		{
			return;
		}
		this.gun.attackSpeedMultiplier = this.data.stats.attackSpeedMultiplier;
		this.heatSinceAttack += TimeHandler.deltaTime;
		this.Attack();
		this.OverHeat();
	}

	// Token: 0x060004F2 RID: 1266 RVA: 0x0001C240 File Offset: 0x0001A440
	private void Attack()
	{
		if (!this.gun)
		{
			return;
		}
		if (!this.gun.IsReady(0f))
		{
			return;
		}
		if (this.input.shootIsPressed)
		{
			if (!this.soundFireHold)
			{
				this.soundFireHold = true;
				if (this.gun.isReloading || this.data.isSilenced)
				{
					SoundManager.Instance.Play(this.soundCharacterCantShoot, base.transform);
				}
			}
		}
		else
		{
			this.soundFireHold = false;
		}
		if (this.gun.bursts == 0 && (!this.soundFireHold || this.gun.isReloading || this.data.isSilenced))
		{
			this.gun.soundGun.StopAutoPlayTail();
		}
		if ((!this.input.shootWasPressed || this.gun.useCharge) && (!this.input.shootWasReleased || !this.gun.useCharge) && (this.gun.attackSpeed / this.data.stats.attackSpeedMultiplier >= 0.3f || !this.input.shootIsPressed || this.gun.useCharge || this.gun.dontAllowAutoFire))
		{
			return;
		}
		if (this.isOverHeated)
		{
			this.heatRenderer.GetComponent<CodeAnimation>().PlayBoop();
			this.gun.sinceAttack = 0f;
			return;
		}
		this.gun.Attack(0f, false, 1f, 1f, true);
		if (this.heat >= 1f)
		{
			base.StartCoroutine(this.DoOverHeat());
			this.isOverHeated = true;
		}
		this.heatSinceAttack = 0f;
	}

	// Token: 0x060004F3 RID: 1267 RVA: 0x0001C3F3 File Offset: 0x0001A5F3
	internal void NewGun()
	{
		this.gun.ResetStats();
		this.gun.soundGun.ClearSoundModifiers();
	}

	// Token: 0x060004F4 RID: 1268 RVA: 0x0001C410 File Offset: 0x0001A610
	private void OverHeat()
	{
		if (this.isOverHeated)
		{
			return;
		}
		if (this.heatSinceAttack > this.secondsBeforeStartToCool)
		{
			this.heat -= TimeHandler.deltaTime * this.coolPerSecond;
		}
		this.SetOverHeatColor();
	}

	// Token: 0x060004F5 RID: 1269 RVA: 0x0001C448 File Offset: 0x0001A648
	private IEnumerator DoOverHeat()
	{
		this.SetOverHeatColor();
		yield return new WaitForSeconds(this.overHeatTime);
		while (this.heat > 0f)
		{
			this.heat -= this.resetSpeed * TimeHandler.deltaTime;
			this.SetOverHeatColor();
			yield return null;
		}
		this.isOverHeated = false;
		yield break;
	}

	// Token: 0x060004F6 RID: 1270 RVA: 0x0001C458 File Offset: 0x0001A658
	private void SetOverHeatColor()
	{
		this.heat = Mathf.Clamp(this.heat, 0f, 1f);
		this.heatRenderer.color = Color.Lerp(this.baseHeatColor, this.overHeatColor, this.heat);
		if (this.heat > 0.25f || this.hasBeenHeated)
		{
			this.hasBeenHeated = true;
			this.overHeatPivot.transform.localScale = new Vector3(this.heat, 1f, 1f);
		}
		else
		{
			this.overHeatPivot.transform.localScale = new Vector3(0f, 1f, 1f);
		}
		if (this.heat == 0f)
		{
			this.hasBeenHeated = false;
		}
	}

	// Token: 0x04000675 RID: 1653
	[Header("Sounds")]
	public SoundEvent soundCharacterCantShoot;

	// Token: 0x04000676 RID: 1654
	private bool soundFireHold;

	// Token: 0x04000677 RID: 1655
	[Header("Settings")]
	public Gun gun;

	// Token: 0x04000678 RID: 1656
	private Holding holding;

	// Token: 0x04000679 RID: 1657
	private GeneralInput input;

	// Token: 0x0400067A RID: 1658
	private CharacterData data;

	// Token: 0x0400067B RID: 1659
	private float heatSinceAttack;

	// Token: 0x0400067C RID: 1660
	private float heat;

	// Token: 0x0400067D RID: 1661
	public float heatPerBullet = 0.1f;

	// Token: 0x0400067E RID: 1662
	public float secondsBeforeStartToCool = 0.1f;

	// Token: 0x0400067F RID: 1663
	public float coolPerSecond = 0.2f;

	// Token: 0x04000680 RID: 1664
	public float overHeatTime = 1f;

	// Token: 0x04000681 RID: 1665
	public float resetSpeed = 2f;

	// Token: 0x04000682 RID: 1666
	public bool isOverHeated;

	// Token: 0x04000683 RID: 1667
	private bool hasBeenHeated;

	// Token: 0x04000684 RID: 1668
	public SpriteRenderer heatRenderer;

	// Token: 0x04000685 RID: 1669
	private Transform overHeatPivot;

	// Token: 0x04000686 RID: 1670
	public Color overHeatColor;

	// Token: 0x04000687 RID: 1671
	private Color baseHeatColor;
}
