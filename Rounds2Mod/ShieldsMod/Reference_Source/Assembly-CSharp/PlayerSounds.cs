using System;
using System.Collections.Generic;
using Sonigon;
using Sonigon.Internal;
using UnityEngine;

// Token: 0x020000A3 RID: 163
public class PlayerSounds : MonoBehaviour
{
	// Token: 0x0600039E RID: 926 RVA: 0x000160BF File Offset: 0x000142BF
	public void AddEnsnareEffect(EnsnareEffect ensnareEffect)
	{
		this.ensnareEffectList.Add(ensnareEffect);
	}

	// Token: 0x0600039F RID: 927 RVA: 0x000160CD File Offset: 0x000142CD
	public void RemoveEnsnareEffect(EnsnareEffect ensnareEffect)
	{
		this.ensnareEffectList.Remove(ensnareEffect);
	}

	// Token: 0x060003A0 RID: 928 RVA: 0x000160DC File Offset: 0x000142DC
	private void Start()
	{
		this.data = base.GetComponent<CharacterData>();
		CharacterData characterData = this.data;
		characterData.TouchGroundAction = (Action<float, Vector3, Vector3, Transform>)Delegate.Combine(characterData.TouchGroundAction, new Action<float, Vector3, Vector3, Transform>(this.TouchGround));
		CharacterData characterData2 = this.data;
		characterData2.TouchWallAction = (Action<float, Vector3, Vector3>)Delegate.Combine(characterData2.TouchWallAction, new Action<float, Vector3, Vector3>(this.TouchWall));
		PlayerJump jump = this.data.jump;
		jump.JumpAction = (Action)Delegate.Combine(jump.JumpAction, new Action(this.Jump));
	}

	// Token: 0x060003A1 RID: 929 RVA: 0x00016170 File Offset: 0x00014370
	public void Jump()
	{
		this.ensnareEnabled = false;
		for (int i = this.ensnareEffectList.Count - 1; i >= 0; i--)
		{
			if (this.ensnareEffectList[i] == null)
			{
				this.ensnareEffectList.RemoveAt(i);
			}
		}
		for (int j = 0; j < this.ensnareEffectList.Count; j++)
		{
			if (this.ensnareEffectList[j].soundEnsnareJumpChange)
			{
				this.ensnareEnabled = true;
			}
		}
		if (this.ensnareEnabled)
		{
			if (this.ensnareEnabled)
			{
				SoundManager.Instance.Play(this.soundCharacterJumpEnsnare, base.transform);
				return;
			}
		}
		else
		{
			if (this.data.stats.SoundTransformScaleThresholdReached())
			{
				SoundManager.Instance.Play(this.soundCharacterJumpBig, base.transform);
				return;
			}
			SoundManager.Instance.Play(this.soundCharacterJump, base.transform);
		}
	}

	// Token: 0x060003A2 RID: 930 RVA: 0x00016254 File Offset: 0x00014454
	public void TouchGround(float sinceGrounded, Vector3 pos, Vector3 normal, Transform ground)
	{
		if (sinceGrounded > 0.05f)
		{
			this.parameterIntensityLand.intensity = sinceGrounded;
			if (this.data.stats.SoundTransformScaleThresholdReached())
			{
				SoundManager.Instance.Play(this.soundCharacterLandBig, base.transform, new SoundParameterBase[]
				{
					this.parameterIntensityLand
				});
				return;
			}
			SoundManager.Instance.Play(this.soundCharacterLand, base.transform, new SoundParameterBase[]
			{
				this.parameterIntensityLand
			});
		}
	}

	// Token: 0x060003A3 RID: 931 RVA: 0x000162D4 File Offset: 0x000144D4
	public void TouchWall(float sinceWall, Vector3 pos, Vector3 normal)
	{
		float num = sinceWall;
		if (this.data.sinceGrounded < num)
		{
			num = this.data.sinceGrounded;
		}
		if (num > 0.05f)
		{
			this.parameterIntensityStickWall.intensity = num;
			if (this.data.stats.SoundTransformScaleThresholdReached())
			{
				SoundManager.Instance.Play(this.soundCharacterStickWallBig, base.transform, new SoundParameterBase[]
				{
					this.parameterIntensityStickWall
				});
				return;
			}
			SoundManager.Instance.Play(this.soundCharacterStickWall, base.transform, new SoundParameterBase[]
			{
				this.parameterIntensityStickWall
			});
		}
	}

	// Token: 0x040004B8 RID: 1208
	[Header("Sounds")]
	public SoundEvent soundCharacterJump;

	// Token: 0x040004B9 RID: 1209
	public SoundEvent soundCharacterJumpBig;

	// Token: 0x040004BA RID: 1210
	public SoundEvent soundCharacterJumpEnsnare;

	// Token: 0x040004BB RID: 1211
	public SoundEvent soundCharacterLand;

	// Token: 0x040004BC RID: 1212
	public SoundEvent soundCharacterLandBig;

	// Token: 0x040004BD RID: 1213
	private SoundParameterIntensity parameterIntensityLand = new SoundParameterIntensity(0f, 1);

	// Token: 0x040004BE RID: 1214
	public SoundEvent soundCharacterStickWall;

	// Token: 0x040004BF RID: 1215
	public SoundEvent soundCharacterStickWallBig;

	// Token: 0x040004C0 RID: 1216
	private SoundParameterIntensity parameterIntensityStickWall = new SoundParameterIntensity(0f, 1);

	// Token: 0x040004C1 RID: 1217
	public SoundEvent soundCharacterDamageScreenEdge;

	// Token: 0x040004C2 RID: 1218
	private CharacterData data;

	// Token: 0x040004C3 RID: 1219
	private List<EnsnareEffect> ensnareEffectList = new List<EnsnareEffect>();

	// Token: 0x040004C4 RID: 1220
	private bool ensnareEnabled;
}
