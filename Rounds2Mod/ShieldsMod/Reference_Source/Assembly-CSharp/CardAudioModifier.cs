using System;
using UnityEngine;

// Token: 0x0200001F RID: 31
public class CardAudioModifier : MonoBehaviour
{
	// Token: 0x04000088 RID: 136
	public string stackName;

	// Token: 0x04000089 RID: 137
	public CardAudioModifier.StackType stackType;

	// Token: 0x0200032F RID: 815
	public enum StackType
	{
		// Token: 0x04001029 RID: 4137
		RTPCValue,
		// Token: 0x0400102A RID: 4138
		PostEvent
	}
}
