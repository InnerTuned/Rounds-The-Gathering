using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;

// Token: 0x020001C3 RID: 451
public class TwitchAudienceVisualizer : MonoBehaviour
{
	// Token: 0x060008E7 RID: 2279 RVA: 0x0002EDFB File Offset: 0x0002CFFB
	private void Awake()
	{
		TwitchAudienceVisualizer.instance = this;
		this.m_RecieveRate = 0.1f;
	}

	// Token: 0x060008E8 RID: 2280 RVA: 0x0002EE0E File Offset: 0x0002D00E
	private void Start()
	{
		this.m_Page = base.GetComponentInParent<ListMenuPage>();
		this.shake = base.GetComponentInChildren<Screenshaker>();
		TwitchUIHandler.Instance.AddMsgAction(new Action<string, string>(this.Chat));
		this.StartAudition();
	}

	// Token: 0x060008E9 RID: 2281 RVA: 0x0002EE44 File Offset: 0x0002D044
	public void Chat(string msg, string from)
	{
		if (!this.m_IsAudition)
		{
			return;
		}
		if (!this.m_MsgsPerTwitchUser.ContainsKey(from))
		{
			this.m_MsgsPerTwitchUser.Add(from, 0);
		}
		if (this.m_MsgsPerTwitchUser[from] >= 100)
		{
			return;
		}
		Dictionary<string, int> msgsPerTwitchUser = this.m_MsgsPerTwitchUser;
		int num = msgsPerTwitchUser[from];
		msgsPerTwitchUser[from] = num + 1;
		if (Time.unscaledTime >= this.m_LastRecievedTime + this.m_RecieveRate)
		{
			this.m_LastRecievedTime = Time.unscaledTime;
			if (this.m_IsReadyToSpawnChatObjects)
			{
				this.SpawnChatObject(msg, from);
			}
		}
		this.currentViewerScore += 100;
		this.shake.OnUIGameFeel(Random.insideUnitCircle.normalized);
	}

	// Token: 0x060008EA RID: 2282 RVA: 0x0002EEF5 File Offset: 0x0002D0F5
	private void SpawnChatObject(string msg, string from)
	{
		Object.Instantiate<GameObject>(this.m_TwitchChatObject).GetComponentInChildren<TextMeshProUGUI>().text = msg;
	}

	// Token: 0x060008EB RID: 2283 RVA: 0x0002EF0D File Offset: 0x0002D10D
	private void StartAudition()
	{
		this.m_IsAudition = true;
		this.m_MsgsPerTwitchUser = new Dictionary<string, int>();
		base.StartCoroutine(this.DoAudition());
	}

	// Token: 0x060008EC RID: 2284 RVA: 0x0002EF30 File Offset: 0x0002D130
	private void Shake(float m = 1f)
	{
		this.shake.OnUIGameFeel(Random.insideUnitCircle.normalized * m);
	}

	// Token: 0x060008ED RID: 2285 RVA: 0x0002EF5B File Offset: 0x0002D15B
	private IEnumerator DoAudition()
	{
		this.border.fillAmount = 0f;
		this.audioenceText.text = "";
		yield return new WaitForSecondsRealtime(1f / this.countDownSpeed);
		this.Shake(3f);
		this.audioenceText.text = "3";
		yield return new WaitForSecondsRealtime(1f / this.countDownSpeed);
		this.Shake(3f);
		this.audioenceText.text = "2";
		yield return new WaitForSecondsRealtime(1f / this.countDownSpeed);
		this.Shake(3f);
		this.audioenceText.text = "1";
		yield return new WaitForSecondsRealtime(1f / this.countDownSpeed);
		this.Shake(10f);
		this.audioenceText.text = "CHAT";
		yield return new WaitForSecondsRealtime(2f / this.countDownSpeed);
		this.Shake(10f);
		this.audioenceText.text = "MAKE";
		yield return new WaitForSecondsRealtime(1f / this.countDownSpeed);
		this.Shake(10f);
		this.audioenceText.text = "SOME";
		yield return new WaitForSecondsRealtime(1f / this.countDownSpeed);
		this.Shake(10f);
		this.audioenceText.text = "NOISE";
		float t = 0f;
		while (t < 1f)
		{
			t += Time.unscaledDeltaTime;
			this.Shake(3f - t);
			yield return null;
		}
		yield return new WaitForSecondsRealtime(0.5f / this.countDownSpeed);
		this.m_IsReadyToSpawnChatObjects = true;
		float c = this.totalAmountOfTime;
		while (c > 0f)
		{
			this.border.fillAmount = c / this.totalAmountOfTime;
			c -= Time.unscaledDeltaTime;
			this.audioenceText.text = this.currentViewerScore + "\n<size=50>AUDIENCE RATING</size>";
			yield return null;
		}
		this.m_IsAudition = false;
		this.m_IsReadyToSpawnChatObjects = false;
		this.m_Page.Close();
		NetworkConnectionHandler.instance.TwitchJoin(this.currentViewerScore);
		yield break;
	}

	// Token: 0x04000A37 RID: 2615
	private const int MAXIMUM_MSG_PER_PLAYER_IN_AUDIENCE = 100;

	// Token: 0x04000A38 RID: 2616
	private const int MAXIMUM_MSG_PER_SECOND = 10;

	// Token: 0x04000A39 RID: 2617
	private float m_RecieveRate;

	// Token: 0x04000A3A RID: 2618
	private float m_LastRecievedTime;

	// Token: 0x04000A3B RID: 2619
	public float countDownSpeed = 2f;

	// Token: 0x04000A3C RID: 2620
	public float totalAmountOfTime = 30f;

	// Token: 0x04000A3D RID: 2621
	[SerializeField]
	private GameObject m_TwitchChatObject;

	// Token: 0x04000A3E RID: 2622
	public ProceduralImage border;

	// Token: 0x04000A3F RID: 2623
	public TextMeshProUGUI audioenceText;

	// Token: 0x04000A40 RID: 2624
	private Screenshaker shake;

	// Token: 0x04000A41 RID: 2625
	private bool m_IsAudition;

	// Token: 0x04000A42 RID: 2626
	private bool m_IsReadyToSpawnChatObjects;

	// Token: 0x04000A43 RID: 2627
	private ListMenuPage m_Page;

	// Token: 0x04000A44 RID: 2628
	public static TwitchAudienceVisualizer instance;

	// Token: 0x04000A45 RID: 2629
	private Dictionary<string, int> m_MsgsPerTwitchUser = new Dictionary<string, int>();

	// Token: 0x04000A46 RID: 2630
	private int currentViewerScore;
}
