using System;
using System.Collections;
using SoundImplementation;
using TMPro;
using UnityEngine;

// Token: 0x02000163 RID: 355
public class LoadingScreen : MonoBehaviour
{
	// Token: 0x06000726 RID: 1830 RVA: 0x00026F99 File Offset: 0x00025199
	private void Awake()
	{
		LoadingScreen.instance = this;
	}

	// Token: 0x06000727 RID: 1831 RVA: 0x00026FA4 File Offset: 0x000251A4
	public void StartLoading(bool privateGame = false)
	{
		base.StopAllCoroutines();
		this.matchFoundSystem.Stop();
		for (int i = 0; i < this.playerNamesSystem.Length; i++)
		{
			this.playerNamesSystem[i].Stop();
		}
		this.searchingSystem.Play();
		this.m_SearchingText.text = this.GetSearchingString(privateGame);
	}

	// Token: 0x06000728 RID: 1832 RVA: 0x00026FFF File Offset: 0x000251FF
	private string GetSearchingString(bool privGame)
	{
		if (privGame)
		{
			return "WAITING FOR FRIEND";
		}
		return "SEARCHING";
	}

	// Token: 0x06000729 RID: 1833 RVA: 0x0002700F File Offset: 0x0002520F
	private IEnumerator IDoLoading()
	{
		SoundPlayerStatic.Instance.PlayMatchFound();
		this.matchFoundSystem.Play();
		yield return new WaitForSeconds(this.matchFoundTime);
		this.matchFoundSystem.Stop();
		base.GetComponentInChildren<DisplayMatchPlayerNames>().ShowNames();
		for (int i = 0; i < this.playerNamesSystem.Length; i++)
		{
			this.playerNamesSystem[i].Play();
		}
		yield return new WaitForSeconds(this.playerNameTime);
		for (int j = 0; j < this.playerNamesSystem.Length; j++)
		{
			this.playerNamesSystem[j].Stop();
			this.playerNamesSystem[j].GetComponentInParent<TextMeshProUGUI>().text = "";
		}
		this.gameMode.SetActive(true);
		yield break;
	}

	// Token: 0x0600072A RID: 1834 RVA: 0x0002701E File Offset: 0x0002521E
	public void StopLoading()
	{
		base.StopAllCoroutines();
		this.searchingSystem.Stop();
		base.StartCoroutine(this.IDoLoading());
	}

	// Token: 0x04000894 RID: 2196
	private const string SEARCHING_TEXT = "SEARCHING";

	// Token: 0x04000895 RID: 2197
	private const string SEARCHING_PRIVATE_TEXT = "WAITING FOR FRIEND";

	// Token: 0x04000896 RID: 2198
	[SerializeField]
	private TextMeshProUGUI m_SearchingText;

	// Token: 0x04000897 RID: 2199
	public GameObject gameMode;

	// Token: 0x04000898 RID: 2200
	public GeneralParticleSystem searchingSystem;

	// Token: 0x04000899 RID: 2201
	public GeneralParticleSystem matchFoundSystem;

	// Token: 0x0400089A RID: 2202
	public GeneralParticleSystem[] playerNamesSystem;

	// Token: 0x0400089B RID: 2203
	public float matchFoundTime = 0.5f;

	// Token: 0x0400089C RID: 2204
	public float playerNameTime = 2f;

	// Token: 0x0400089D RID: 2205
	public static LoadingScreen instance;
}
