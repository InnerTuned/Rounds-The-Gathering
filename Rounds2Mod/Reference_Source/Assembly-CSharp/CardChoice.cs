using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using SoundImplementation;
using UnityEngine;

// Token: 0x02000022 RID: 34
public class CardChoice : MonoBehaviour
{
	// Token: 0x0600009B RID: 155 RVA: 0x000052E9 File Offset: 0x000034E9
	private void Awake()
	{
		CardChoice.instance = this;
	}

	// Token: 0x0600009C RID: 156 RVA: 0x000052F4 File Offset: 0x000034F4
	private void Start()
	{
		for (int i = 0; i < this.cards.Length; i++)
		{
			PhotonNetwork.PrefabPool.RegisterPrefab(this.cards[i].gameObject.name, this.cards[i].gameObject);
		}
		this.children = new Transform[base.transform.childCount];
		for (int j = 0; j < this.children.Length; j++)
		{
			this.children[j] = base.transform.GetChild(j);
		}
	}

	// Token: 0x0600009D RID: 157 RVA: 0x0000537C File Offset: 0x0000357C
	public CardInfo GetSourceCard(CardInfo info)
	{
		for (int i = 0; i < this.cards.Length; i++)
		{
			if (this.cards[i].cardName == info.cardName)
			{
				return this.cards[i];
			}
		}
		return null;
	}

	// Token: 0x0600009E RID: 158 RVA: 0x000053C0 File Offset: 0x000035C0
	public void Pick(GameObject pickedCard = null, bool clear = false)
	{
		if (pickedCard)
		{
			pickedCard.GetComponentInChildren<ApplyCardStats>().Pick(this.pickrID, false, this.pickerType);
			base.GetComponent<PhotonView>().RPC("RPCA_DoEndPick", 0, new object[]
			{
				this.CardIDs(),
				pickedCard.GetComponent<PhotonView>().ViewID,
				pickedCard.GetComponent<PublicInt>().theInt,
				this.pickrID
			});
			return;
		}
		if (PlayerManager.instance.GetPlayerWithID(this.pickrID).data.view.IsMine)
		{
			base.StartCoroutine(this.ReplaceCards(pickedCard, clear));
		}
	}

	// Token: 0x0600009F RID: 159 RVA: 0x00005474 File Offset: 0x00003674
	private int[] CardIDs()
	{
		int[] array = new int[this.spawnedCards.Count];
		for (int i = 0; i < this.spawnedCards.Count; i++)
		{
			array[i] = this.spawnedCards[i].GetComponent<PhotonView>().ViewID;
		}
		return array;
	}

	// Token: 0x060000A0 RID: 160 RVA: 0x000054C4 File Offset: 0x000036C4
	private List<GameObject> CardFromIDs(int[] cardIDs)
	{
		List<GameObject> list = new List<GameObject>();
		for (int i = 0; i < cardIDs.Length; i++)
		{
			list.Add(PhotonNetwork.GetPhotonView(cardIDs[i]).gameObject);
		}
		return list;
	}

	// Token: 0x060000A1 RID: 161 RVA: 0x000054FC File Offset: 0x000036FC
	[PunRPC]
	private void RPCA_DoEndPick(int[] cardIDs, int targetCardID, int theInt = 0, int pickId = -1)
	{
		GameObject gameObject = PhotonNetwork.GetPhotonView(targetCardID).gameObject;
		this.spawnedCards = this.CardFromIDs(cardIDs);
		base.StartCoroutine(this.IDoEndPick(gameObject, theInt, pickId));
	}

	// Token: 0x060000A2 RID: 162 RVA: 0x00005533 File Offset: 0x00003733
	public IEnumerator IDoEndPick(GameObject pickedCard = null, int theInt = 0, int pickId = -1)
	{
		Vector3 startPos = pickedCard.transform.position;
		Vector3 endPos = CardChoiceVisuals.instance.transform.position;
		float c = 0f;
		while (c < 1f)
		{
			CardChoiceVisuals.instance.framesToSnap = 1;
			Vector3 position = Vector3.LerpUnclamped(startPos, endPos, this.curve.Evaluate(c));
			pickedCard.transform.position = position;
			base.transform.GetChild(theInt).position = position;
			c += Time.deltaTime * this.speed;
			yield return null;
		}
		GamefeelManager.GameFeel((startPos - endPos).normalized * 2f);
		for (int i = 0; i < this.spawnedCards.Count; i++)
		{
			if (this.spawnedCards[i])
			{
				if (this.spawnedCards[i].gameObject != pickedCard)
				{
					this.spawnedCards[i].AddComponent<Rigidbody>().AddForce((this.spawnedCards[i].transform.position - endPos) * Random.Range(0f, 50f));
					this.spawnedCards[i].GetComponent<Rigidbody>().AddTorque(Random.onUnitSphere * Random.Range(0f, 200f));
					this.spawnedCards[i].AddComponent<RemoveAfterSeconds>().seconds = Random.Range(0.5f, 1f);
					this.spawnedCards[i].GetComponent<RemoveAfterSeconds>().shrink = true;
				}
				else
				{
					this.spawnedCards[i].GetComponentInChildren<CardVisuals>().Leave();
				}
			}
		}
		yield return new WaitForSeconds(0.25f);
		AnimationCurve softCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
		Vector3 startPos2 = base.transform.GetChild(theInt).transform.position;
		Vector3 endPos2 = startPos;
		c = 0f;
		while (c < 1f)
		{
			Vector3 position2 = Vector3.LerpUnclamped(startPos2, endPos2, softCurve.Evaluate(c));
			base.transform.GetChild(theInt).position = position2;
			c += Time.deltaTime * this.speed * 1.5f;
			yield return null;
		}
		SoundPlayerStatic.Instance.PlayPlayerBallDisappear();
		base.transform.GetChild(theInt).position = startPos;
		this.spawnedCards.Clear();
		if (PlayerManager.instance.GetPlayerWithID(pickId).data.view.IsMine)
		{
			base.StartCoroutine(this.ReplaceCards(pickedCard, false));
		}
		yield break;
	}

	// Token: 0x060000A3 RID: 163 RVA: 0x00005557 File Offset: 0x00003757
	private GameObject Spawn(GameObject objToSpawn, Vector3 pos, Quaternion rot)
	{
		return PhotonNetwork.Instantiate(this.GetCardPath(objToSpawn), pos, rot, 0, null);
	}

	// Token: 0x060000A4 RID: 164 RVA: 0x00005569 File Offset: 0x00003769
	private string GetCardPath(GameObject targetObj)
	{
		return targetObj.name;
	}

	// Token: 0x060000A5 RID: 165 RVA: 0x00005574 File Offset: 0x00003774
	public GameObject AddCard(CardInfo cardToSpawn)
	{
		GameObject gameObject = this.Spawn(cardToSpawn.gameObject, new Vector3(30f, -10f, 0f), Quaternion.identity);
		this.spawnedCards.Add(gameObject);
		return gameObject;
	}

	// Token: 0x060000A6 RID: 166 RVA: 0x000055B4 File Offset: 0x000037B4
	public GameObject AddCardVisual(CardInfo cardToSpawn, Vector3 pos)
	{
		GameObject gameObject = Object.Instantiate<GameObject>(cardToSpawn.gameObject, pos, Quaternion.identity);
		gameObject.GetComponentInChildren<CardVisuals>().firstValueToSet = true;
		return gameObject;
	}

	// Token: 0x060000A7 RID: 167 RVA: 0x000055D3 File Offset: 0x000037D3
	private IEnumerator ReplaceCards(GameObject pickedCard = null, bool clear = false)
	{
		if (this.picks > 0)
		{
			SoundPlayerStatic.Instance.PlayPlayerBallAppear();
		}
		this.isPlaying = true;
		if (clear && this.spawnedCards != null)
		{
			int num;
			for (int i = 0; i < this.spawnedCards.Count; i = num + 1)
			{
				if (pickedCard != this.spawnedCards[i])
				{
					this.spawnedCards[i].GetComponentInChildren<CardVisuals>().Leave();
					yield return new WaitForSecondsRealtime(0.1f);
				}
				num = i;
			}
			yield return new WaitForSecondsRealtime(0.2f);
			if (pickedCard)
			{
				pickedCard.GetComponentInChildren<CardVisuals>().Pick();
			}
			this.spawnedCards.Clear();
		}
		yield return new WaitForSecondsRealtime(0.2f);
		if (this.picks > 0)
		{
			int num;
			for (int i = 0; i < this.children.Length; i = num + 1)
			{
				this.spawnedCards.Add(this.SpawnUniqueCard(this.children[i].transform.position, this.children[i].transform.rotation));
				this.spawnedCards[i].AddComponent<PublicInt>().theInt = i;
				yield return new WaitForSecondsRealtime(0.1f);
				num = i;
			}
		}
		else
		{
			base.GetComponent<PhotonView>().RPC("RPCA_DonePicking", 0, Array.Empty<object>());
		}
		this.picks--;
		this.isPlaying = false;
		yield break;
	}

	// Token: 0x060000A8 RID: 168 RVA: 0x000055F0 File Offset: 0x000037F0
	[PunRPC]
	private void RPCA_DonePicking()
	{
		this.IsPicking = false;
	}

	// Token: 0x060000A9 RID: 169 RVA: 0x000055FC File Offset: 0x000037FC
	private GameObject GetRanomCard()
	{
		GameObject result = null;
		float num = 0f;
		for (int i = 0; i < this.cards.Length; i++)
		{
			if (this.cards[i].rarity == CardInfo.Rarity.Common)
			{
				num += 10f;
			}
			if (this.cards[i].rarity == CardInfo.Rarity.Uncommon)
			{
				num += 4f;
			}
			if (this.cards[i].rarity == CardInfo.Rarity.Rare)
			{
				num += 1f;
			}
		}
		float num2 = Random.Range(0f, num);
		for (int j = 0; j < this.cards.Length; j++)
		{
			if (this.cards[j].rarity == CardInfo.Rarity.Common)
			{
				num2 -= 10f;
			}
			if (this.cards[j].rarity == CardInfo.Rarity.Uncommon)
			{
				num2 -= 4f;
			}
			if (this.cards[j].rarity == CardInfo.Rarity.Rare)
			{
				num2 -= 1f;
			}
			if (num2 <= 0f)
			{
				result = this.cards[j].gameObject;
				break;
			}
		}
		return result;
	}

	// Token: 0x060000AA RID: 170 RVA: 0x000056F4 File Offset: 0x000038F4
	private GameObject SpawnUniqueCard(Vector3 pos, Quaternion rot)
	{
		GameObject ranomCard = this.GetRanomCard();
		CardInfo component = ranomCard.GetComponent<CardInfo>();
		Player player;
		if (this.pickerType == PickerType.Team)
		{
			player = PlayerManager.instance.GetPlayersInTeam(this.pickrID)[0];
		}
		else
		{
			player = PlayerManager.instance.players[this.pickrID];
		}
		for (int i = 0; i < this.spawnedCards.Count; i++)
		{
			bool flag = this.spawnedCards[i].GetComponent<CardInfo>().cardName == ranomCard.GetComponent<CardInfo>().cardName;
			if (this.pickrID != -1)
			{
				Holdable holdable = player.data.GetComponent<Holding>().holdable;
				if (holdable)
				{
					Gun component2 = holdable.GetComponent<Gun>();
					Gun component3 = ranomCard.GetComponent<Gun>();
					if (component3 && component2 && component3.lockGunToDefault && component2.lockGunToDefault)
					{
						flag = true;
					}
				}
				for (int j = 0; j < player.data.currentCards.Count; j++)
				{
					CardInfo component4 = player.data.currentCards[j].GetComponent<CardInfo>();
					for (int k = 0; k < component4.blacklistedCategories.Length; k++)
					{
						for (int l = 0; l < component.categories.Length; l++)
						{
							if (component.categories[l] == component4.blacklistedCategories[k])
							{
								flag = true;
							}
						}
					}
					if (!component4.allowMultiple && component.cardName == component4.cardName)
					{
						flag = true;
					}
				}
			}
			if (flag)
			{
				return this.SpawnUniqueCard(pos, rot);
			}
		}
		GameObject gameObject = this.Spawn(ranomCard.gameObject, pos, rot);
		gameObject.GetComponent<CardInfo>().sourceCard = ranomCard.GetComponent<CardInfo>();
		gameObject.GetComponentInChildren<DamagableEvent>().GetComponent<Collider2D>().enabled = false;
		return gameObject;
	}

	// Token: 0x060000AB RID: 171 RVA: 0x000058D0 File Offset: 0x00003AD0
	private void Update()
	{
		this.counter += Time.deltaTime;
		bool flag = this.isPlaying;
		if (this.pickrID != -1 && this.IsPicking)
		{
			this.DoPlayerSelect();
		}
		if (Application.isEditor && !DevConsole.isTyping && Input.GetKeyDown(KeyCode.N))
		{
			this.picks++;
			CardChoice.instance.Pick(null, true);
		}
	}

	// Token: 0x060000AC RID: 172 RVA: 0x00005940 File Offset: 0x00003B40
	private void DoPlayerSelect()
	{
		SoundMusicManager.Instance.PlayIngame(true);
		if (this.spawnedCards.Count == 0 || this.pickrID == -1)
		{
			return;
		}
		PlayerActions[] array;
		if (this.pickerType == PickerType.Team)
		{
			array = PlayerManager.instance.GetActionsFromTeam(this.pickrID);
		}
		else
		{
			array = PlayerManager.instance.GetActionsFromPlayer(this.pickrID);
		}
		if (array == null)
		{
			this.Pick(this.spawnedCards[0], false);
			return;
		}
		CardChoice.StickDirection stickDirection = CardChoice.StickDirection.None;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null)
			{
				if (array[i].Right.Value > 0.7f)
				{
					stickDirection = CardChoice.StickDirection.Right;
				}
				if (array[i].Left.Value > 0.7f)
				{
					stickDirection = CardChoice.StickDirection.Left;
				}
				this.currentlySelectedCard = Mathf.Clamp(this.currentlySelectedCard, 0, this.spawnedCards.Count - 1);
				for (int j = 0; j < this.spawnedCards.Count; j++)
				{
					if (this.spawnedCards[j] && (this.spawnedCards[j].GetComponentInChildren<CardVisuals>().isSelected != (this.currentlySelectedCard == j) || this.counter > 0.2f))
					{
						this.counter = 0f;
						this.spawnedCards[j].GetComponent<PhotonView>().RPC("RPCA_ChangeSelected", 0, new object[]
						{
							this.currentlySelectedCard == j
						});
					}
				}
				if (array[i].Jump.WasPressed && !this.isPlaying && this.spawnedCards[this.currentlySelectedCard] != null)
				{
					this.Pick(this.spawnedCards[this.currentlySelectedCard], false);
					this.pickrID = -1;
					break;
				}
			}
		}
		if (stickDirection != this.lastStickDirection)
		{
			if (stickDirection == CardChoice.StickDirection.Left)
			{
				this.currentlySelectedCard--;
			}
			if (stickDirection == CardChoice.StickDirection.Right)
			{
				this.currentlySelectedCard++;
			}
			this.lastStickDirection = stickDirection;
		}
		if (CardChoiceVisuals.instance.currentCardSelected != this.currentlySelectedCard)
		{
			CardChoiceVisuals.instance.SetCurrentSelected(this.currentlySelectedCard);
		}
	}

	// Token: 0x060000AD RID: 173 RVA: 0x00005B5E File Offset: 0x00003D5E
	public IEnumerator DoPick(int picksToSet, int picketIDToSet, PickerType pType = PickerType.Team)
	{
		this.pickerType = pType;
		this.StartPick(picksToSet, picketIDToSet);
		while (this.IsPicking)
		{
			yield return null;
		}
		UIHandler.instance.StopShowPicker();
		CardChoiceVisuals.instance.Hide();
		yield break;
	}

	// Token: 0x060000AE RID: 174 RVA: 0x00005B82 File Offset: 0x00003D82
	public void StartPick(int picksToSet, int pickerIDToSet)
	{
		this.pickrID = pickerIDToSet;
		this.IsPicking = true;
		this.picks = picksToSet;
		ArtHandler.instance.SetSpecificArt(this.cardPickArt);
		this.Pick(null, false);
	}

	// Token: 0x060000AF RID: 175 RVA: 0x00005BB4 File Offset: 0x00003DB4
	public Color GetCardColor(CardThemeColor.CardThemeColorType colorType)
	{
		for (int i = 0; i < this.cardThemes.Length; i++)
		{
			if (this.cardThemes[i].themeType == colorType)
			{
				return this.cardThemes[i].targetColor;
			}
		}
		return Color.black;
	}

	// Token: 0x060000B0 RID: 176 RVA: 0x00005BF8 File Offset: 0x00003DF8
	public Color GetCardColor2(CardThemeColor.CardThemeColorType colorType)
	{
		for (int i = 0; i < this.cardThemes.Length; i++)
		{
			if (this.cardThemes[i].themeType == colorType)
			{
				return this.cardThemes[i].bgColor;
			}
		}
		return Color.black;
	}

	// Token: 0x0400008A RID: 138
	public int pickrID = -1;

	// Token: 0x0400008B RID: 139
	public ArtInstance cardPickArt;

	// Token: 0x0400008C RID: 140
	private Transform[] children;

	// Token: 0x0400008D RID: 141
	public CardInfo[] cards;

	// Token: 0x0400008E RID: 142
	public int picks = 6;

	// Token: 0x0400008F RID: 143
	public static CardChoice instance;

	// Token: 0x04000090 RID: 144
	public bool IsPicking;

	// Token: 0x04000091 RID: 145
	private List<GameObject> spawnedCards = new List<GameObject>();

	// Token: 0x04000092 RID: 146
	public AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	// Token: 0x04000093 RID: 147
	private float speed = 4f;

	// Token: 0x04000094 RID: 148
	private bool isPlaying;

	// Token: 0x04000095 RID: 149
	private CardChoice.StickDirection lastStickDirection;

	// Token: 0x04000096 RID: 150
	private int currentlySelectedCard;

	// Token: 0x04000097 RID: 151
	private float counter = 1f;

	// Token: 0x04000098 RID: 152
	private PickerType pickerType;

	// Token: 0x04000099 RID: 153
	public CardThemeColor[] cardThemes;

	// Token: 0x02000330 RID: 816
	private enum StickDirection
	{
		// Token: 0x0400102C RID: 4140
		Left,
		// Token: 0x0400102D RID: 4141
		Right,
		// Token: 0x0400102E RID: 4142
		None
	}
}
