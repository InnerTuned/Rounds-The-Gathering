using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

// Token: 0x02000179 RID: 377
public class OnlineNameSelect : MonoBehaviour
{
	// Token: 0x060007A0 RID: 1952 RVA: 0x00029346 File Offset: 0x00027546
	private void Start()
	{
		this.nameField = base.GetComponentInChildren<TMP_InputField>();
		this.nameField.text = PlayerPrefs.GetString("PlayerName", "");
		PhotonNetwork.LocalPlayer.NickName = this.nameField.text;
	}

	// Token: 0x060007A1 RID: 1953 RVA: 0x00029383 File Offset: 0x00027583
	public void OnChangedVal(string newVal)
	{
		PlayerPrefs.SetString("PlayerName", this.nameField.text);
		PhotonNetwork.LocalPlayer.NickName = this.nameField.text;
	}

	// Token: 0x040008FA RID: 2298
	private TMP_InputField nameField;
}
