using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200014E RID: 334
public class GameCrownHandler : MonoBehaviour
{
	// Token: 0x060006C4 RID: 1732 RVA: 0x0002576D File Offset: 0x0002396D
	private void Start()
	{
		this.gm = base.GetComponentInParent<GM_ArmsRace>();
		GM_ArmsRace gm_ArmsRace = this.gm;
		gm_ArmsRace.pointOverAction = (Action)Delegate.Combine(gm_ArmsRace.pointOverAction, new Action(this.PointOver));
	}

	// Token: 0x060006C5 RID: 1733 RVA: 0x000257A4 File Offset: 0x000239A4
	public void PointOver()
	{
		int num = -1;
		int num2 = -1;
		if (this.gm.p1Rounds > this.gm.p2Rounds)
		{
			num2 = 0;
		}
		if (this.gm.p1Rounds < this.gm.p2Rounds)
		{
			num2 = 1;
		}
		if (num2 == -1)
		{
			int num3 = -1;
			if (this.gm.p1Points > this.gm.p2Points)
			{
				num3 = 0;
			}
			if (this.gm.p1Points < this.gm.p2Points)
			{
				num3 = 1;
			}
			if (num3 != -1)
			{
				num = num3;
			}
		}
		else
		{
			num = num2;
		}
		if (num != -1 && num != this.currentCrownHolder)
		{
			if (this.currentCrownHolder == -1)
			{
				this.currentCrownHolder = num;
				this.crownPos = (float)this.currentCrownHolder;
				base.GetComponent<CurveAnimation>().PlayIn();
				return;
			}
			this.GiveCrownToPlayer(num);
		}
	}

	// Token: 0x060006C6 RID: 1734 RVA: 0x0002586C File Offset: 0x00023A6C
	private void LateUpdate()
	{
		if (this.currentCrownHolder == -1)
		{
			return;
		}
		if (PlayerManager.instance.players[this.currentCrownHolder].gameObject.activeInHierarchy)
		{
			if (!base.transform.GetChild(0).gameObject.activeSelf)
			{
				base.transform.GetChild(0).gameObject.SetActive(true);
			}
		}
		else if (base.transform.GetChild(0).gameObject.activeSelf)
		{
			base.transform.GetChild(0).gameObject.SetActive(false);
		}
		Vector3 position = Vector3.LerpUnclamped(PlayerManager.instance.players[0].data.GetCrownPos(), PlayerManager.instance.players[1].data.GetCrownPos(), this.crownPos);
		base.transform.position = position;
	}

	// Token: 0x060006C7 RID: 1735 RVA: 0x00025950 File Offset: 0x00023B50
	private void GiveCrownToPlayer(int playerID)
	{
		this.currentCrownHolder = playerID;
		base.StartCoroutine(this.IGiveCrownToPlayer(playerID));
	}

	// Token: 0x060006C8 RID: 1736 RVA: 0x00025967 File Offset: 0x00023B67
	private IEnumerator IGiveCrownToPlayer(int playerID)
	{
		int fromInt = 0;
		int toInt = 0;
		if (playerID == 0)
		{
			fromInt = 1;
			toInt = 0;
		}
		else
		{
			fromInt = 0;
			toInt = 1;
		}
		for (float i = 0f; i < this.transitionCurve.keys[this.transitionCurve.keys.Length - 1].time; i += Time.unscaledDeltaTime)
		{
			this.crownPos = Mathf.LerpUnclamped((float)fromInt, (float)toInt, this.transitionCurve.Evaluate(i));
			yield return null;
		}
		yield break;
	}

	// Token: 0x0400081D RID: 2077
	private float crownPos;

	// Token: 0x0400081E RID: 2078
	public AnimationCurve transitionCurve;

	// Token: 0x0400081F RID: 2079
	private GM_ArmsRace gm;

	// Token: 0x04000820 RID: 2080
	private int currentCrownHolder = -1;
}
