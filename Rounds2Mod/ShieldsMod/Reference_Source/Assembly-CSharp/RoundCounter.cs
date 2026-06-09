using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000C0 RID: 192
public class RoundCounter : MonoBehaviour
{
	// Token: 0x06000407 RID: 1031 RVA: 0x00018835 File Offset: 0x00016A35
	private void Start()
	{
		this.p1Parent.gameObject.SetActive(true);
		this.p2Parent.gameObject.SetActive(true);
	}

	// Token: 0x06000408 RID: 1032 RVA: 0x00018859 File Offset: 0x00016A59
	public void UpdateRounds(int r1, int r2)
	{
		this.p1Rounds = r1;
		this.p2Rounds = r2;
		this.ReDraw();
	}

	// Token: 0x06000409 RID: 1033 RVA: 0x00018870 File Offset: 0x00016A70
	private void Clear()
	{
		for (int i = 0; i < this.p1Parent.childCount; i++)
		{
			if (this.p1Populate.transform.GetChild(i).gameObject.activeSelf)
			{
				Object.Destroy(this.p1Populate.transform.GetChild(i).gameObject);
			}
		}
		for (int j = 0; j < this.p2Parent.childCount; j++)
		{
			if (this.p2Populate.transform.GetChild(j).gameObject.activeSelf)
			{
				Object.Destroy(this.p2Populate.transform.GetChild(j).gameObject);
			}
		}
	}

	// Token: 0x0600040A RID: 1034 RVA: 0x00018919 File Offset: 0x00016B19
	private void Populate()
	{
		this.p1Populate.times = this.roundsNeeded;
		this.p1Populate.DoPopulate();
		this.p2Populate.times = this.roundsNeeded;
		this.p2Populate.DoPopulate();
	}

	// Token: 0x0600040B RID: 1035 RVA: 0x00018955 File Offset: 0x00016B55
	public void UpdatePoints(int p1, int p2)
	{
		this.p1Points = p1;
		this.p2Points = p2;
		this.ReDraw();
	}

	// Token: 0x0600040C RID: 1036 RVA: 0x0001896C File Offset: 0x00016B6C
	private void ReDraw()
	{
		for (int i = 0; i < this.p1Parent.childCount; i++)
		{
			if (this.p1Rounds + this.p1Points > i)
			{
				this.p1Parent.GetChild(i).GetComponentInChildren<Image>().color = PlayerSkinBank.GetPlayerSkinColors(0).winText;
				if (this.p1Rounds > i)
				{
					this.p1Parent.GetChild(i).localScale = Vector3.one;
				}
			}
			else
			{
				this.p1Parent.GetChild(i).GetComponentInChildren<Image>().color = this.offColor;
				this.p1Parent.GetChild(i).localScale = Vector3.one * 0.3f;
			}
		}
		for (int j = 0; j < this.p2Parent.childCount; j++)
		{
			if (this.p2Rounds + this.p2Points > j)
			{
				this.p2Parent.GetChild(j).GetComponentInChildren<Image>().color = PlayerSkinBank.GetPlayerSkinColors(1).winText;
				if (this.p2Rounds > j)
				{
					this.p2Parent.GetChild(j).localScale = Vector3.one;
				}
			}
			else
			{
				this.p2Parent.GetChild(j).GetComponentInChildren<Image>().color = this.offColor;
				this.p2Parent.GetChild(j).localScale = Vector3.one * 0.3f;
			}
		}
	}

	// Token: 0x0600040D RID: 1037 RVA: 0x00018ACD File Offset: 0x00016CCD
	internal Vector3 GetPointPos(int teamID)
	{
		if (teamID == 0)
		{
			return this.p1Parent.GetChild(this.p1Rounds).transform.position;
		}
		return this.p2Parent.GetChild(this.p2Rounds).transform.position;
	}

	// Token: 0x0600040E RID: 1038 RVA: 0x00018B09 File Offset: 0x00016D09
	internal void SetNumberOfRounds(int roundsToWinGame)
	{
		this.roundsNeeded = roundsToWinGame;
		this.Clear();
		this.Populate();
		this.ReDraw();
	}

	// Token: 0x04000585 RID: 1413
	public int p1Rounds;

	// Token: 0x04000586 RID: 1414
	public int p2Rounds;

	// Token: 0x04000587 RID: 1415
	public int p1Points;

	// Token: 0x04000588 RID: 1416
	public int p2Points;

	// Token: 0x04000589 RID: 1417
	public Transform p1Parent;

	// Token: 0x0400058A RID: 1418
	public Transform p2Parent;

	// Token: 0x0400058B RID: 1419
	public Color offColor;

	// Token: 0x0400058C RID: 1420
	public Populate p1Populate;

	// Token: 0x0400058D RID: 1421
	public Populate p2Populate;

	// Token: 0x0400058E RID: 1422
	private int roundsNeeded;
}
