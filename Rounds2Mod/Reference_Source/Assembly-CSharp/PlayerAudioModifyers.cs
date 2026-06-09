using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000096 RID: 150
public class PlayerAudioModifyers : MonoBehaviour
{
	// Token: 0x0600034D RID: 845 RVA: 0x00014830 File Offset: 0x00012A30
	public void AddToStack(CardAudioModifier mod)
	{
		int num = -1;
		for (int i = 0; i < this.modifyers.Count; i++)
		{
			if (this.modifyers[i].modifier.stackName == mod.stackName)
			{
				num = i;
				break;
			}
		}
		if (num != -1)
		{
			this.modifyers[num].stacks++;
			return;
		}
		AudioModifyer audioModifyer = new AudioModifyer();
		audioModifyer.modifier = new CardAudioModifier();
		audioModifyer.modifier.stackName = mod.stackName;
		audioModifyer.modifier.stackType = mod.stackType;
		audioModifyer.stacks = 1;
		PlayerAudioModifyers.activeModifyer.Add(audioModifyer.modifier);
		this.modifyers.Add(audioModifyer);
	}

	// Token: 0x0600034E RID: 846 RVA: 0x000148F0 File Offset: 0x00012AF0
	public void SetStacks()
	{
		for (int i = 0; i < PlayerAudioModifyers.activeModifyer.Count; i++)
		{
			CardAudioModifier.StackType stackType = PlayerAudioModifyers.activeModifyer[i].stackType;
			CardAudioModifier.StackType stackType2 = PlayerAudioModifyers.activeModifyer[i].stackType;
		}
		for (int j = 0; j < this.modifyers.Count; j++)
		{
			CardAudioModifier.StackType stackType3 = this.modifyers[j].modifier.stackType;
			CardAudioModifier.StackType stackType4 = this.modifyers[j].modifier.stackType;
		}
	}

	// Token: 0x0400046F RID: 1135
	public List<AudioModifyer> modifyers = new List<AudioModifyer>();

	// Token: 0x04000470 RID: 1136
	public static List<CardAudioModifier> activeModifyer = new List<CardAudioModifier>();
}
