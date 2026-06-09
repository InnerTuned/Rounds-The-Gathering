using System;
using UnityEngine;

// Token: 0x020001C4 RID: 452
public class TwitchChatMessage : MonoBehaviour
{
	// Token: 0x060008EF RID: 2287 RVA: 0x0002EF94 File Offset: 0x0002D194
	private void Start()
	{
		base.transform.SetParent(TwitchAudienceVisualizer.instance.transform);
		base.transform.localScale = Vector3.one;
		base.GetComponent<RectTransform>().anchoredPosition = new Vector2((float)Random.Range(-850, 850), (float)Random.Range(-475, 475));
	}
}
