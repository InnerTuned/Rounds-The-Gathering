using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

// Token: 0x02000049 RID: 73
public class DevConsole : MonoBehaviour
{
	// Token: 0x06000162 RID: 354 RVA: 0x00008F23 File Offset: 0x00007123
	private void Start()
	{
		DevConsole.isTyping = false;
	}

	// Token: 0x06000163 RID: 355 RVA: 0x00008F2C File Offset: 0x0000712C
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Return))
		{
			this.inputField.gameObject.SetActive(!this.inputField.gameObject.activeSelf);
			DevConsole.isTyping = this.inputField.gameObject.activeSelf;
			GameManager.lockInput = DevConsole.isTyping;
			if (this.inputField.gameObject.activeSelf)
			{
				this.inputField.ActivateInputField();
				return;
			}
			this.Send(this.inputField.text);
		}
	}

	// Token: 0x06000164 RID: 356 RVA: 0x00008FB4 File Offset: 0x000071B4
	private void Send(string message)
	{
		if (Application.isEditor || (GM_Test.instance && GM_Test.instance.gameObject.activeSelf))
		{
			this.SpawnCard(message);
		}
		if (Application.isEditor)
		{
			this.SpawnMap(message);
		}
		int viewID = PlayerManager.instance.GetPlayerWithActorID(PhotonNetwork.LocalPlayer.ActorNumber).data.view.ViewID;
		base.GetComponent<PhotonView>().RPC("RPCA_SendChat", 0, new object[]
		{
			message,
			viewID
		});
	}

	// Token: 0x06000165 RID: 357 RVA: 0x00009044 File Offset: 0x00007244
	private void SpawnMap(string message)
	{
		try
		{
			int id = int.Parse(message);
			MapManager.instance.LoadLevelFromID(id, false, true);
		}
		catch
		{
		}
	}

	// Token: 0x06000166 RID: 358 RVA: 0x0000907C File Offset: 0x0000727C
	[PunRPC]
	private void RPCA_SendChat(string message, int playerViewID)
	{
		PhotonNetwork.GetPhotonView(playerViewID).GetComponentInChildren<PlayerChat>().Send(message);
	}

	// Token: 0x06000167 RID: 359 RVA: 0x00009090 File Offset: 0x00007290
	private void SpawnCard(string message)
	{
		CardInfo[] cards = CardChoice.instance.cards;
		int num = -1;
		float num2 = 0f;
		for (int i = 0; i < cards.Length; i++)
		{
			string text = cards[i].GetComponent<CardInfo>().cardName.ToUpper();
			text = text.Replace(" ", "");
			string text2 = message.ToUpper();
			text2 = text2.Replace(" ", "");
			float num3 = 0f;
			for (int j = 0; j < text2.Length; j++)
			{
				if (text.Length > j && text2.get_Chars(j) == text.get_Chars(j))
				{
					num3 += 1f / (float)text2.Length;
				}
			}
			num3 -= (float)Mathf.Abs(text2.Length - text.Length) * 0.001f;
			if (num3 > 0.1f && num3 > num2)
			{
				num2 = num3;
				num = i;
			}
		}
		if (num != -1)
		{
			GameObject gameObject = CardChoice.instance.AddCard(cards[num]);
			gameObject.GetComponentInChildren<CardVisuals>().firstValueToSet = true;
			gameObject.transform.root.GetComponentInChildren<ApplyCardStats>().shootToPick = true;
		}
	}

	// Token: 0x06000168 RID: 360 RVA: 0x000091BC File Offset: 0x000073BC
	public static int GetClosestString(string inputText, string[] compareTo)
	{
		CardInfo[] cards = CardChoice.instance.cards;
		int result = -1;
		float num = 0f;
		for (int i = 0; i < cards.Length; i++)
		{
			string text = compareTo[i];
			text = text.Replace(" ", "");
			string text2 = inputText.ToUpper();
			text2 = text2.Replace(" ", "");
			float num2 = 0f;
			for (int j = 0; j < text2.Length; j++)
			{
				if (text.Length > j && text2.get_Chars(j) == text.get_Chars(j))
				{
					num2 += 1f / (float)text2.Length;
				}
			}
			num2 -= (float)Mathf.Abs(text2.Length - text.Length) * 0.001f;
			if (num2 > 0.1f && num2 > num)
			{
				num = num2;
				result = i;
			}
		}
		return result;
	}

	// Token: 0x040001CF RID: 463
	public TMP_InputField inputField;

	// Token: 0x040001D0 RID: 464
	public static bool isTyping;
}
