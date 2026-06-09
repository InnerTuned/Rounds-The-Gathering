using System;
using System.Collections;
using Photon.Pun;
using Sonigon;
using UnityEngine;

// Token: 0x02000108 RID: 264
public class CardChoiceVisuals : MonoBehaviour
{
	// Token: 0x0600052C RID: 1324 RVA: 0x0001D495 File Offset: 0x0001B695
	private void Awake()
	{
		CardChoiceVisuals.instance = this;
		this.leftHandRestPos = this.leftHandTarget.position;
		this.rightHandRestPos = this.rightHandTarget.position;
	}

	// Token: 0x0600052D RID: 1325 RVA: 0x000027C8 File Offset: 0x000009C8
	private void Start()
	{
	}

	// Token: 0x0600052E RID: 1326 RVA: 0x0001D4C0 File Offset: 0x0001B6C0
	private void Update()
	{
		if (!this.isShowinig)
		{
			return;
		}
		if (Time.unscaledDeltaTime > 0.1f)
		{
			return;
		}
		if (this.currentCardSelected >= this.cardParent.transform.childCount || this.currentCardSelected < 0)
		{
			return;
		}
		if (this.rightHandTarget.position.x == float.NaN || this.rightHandTarget.position.y == float.NaN || this.rightHandTarget.position.z == float.NaN)
		{
			this.rightHandTarget.position = Vector3.zero;
			this.rightHandVel = Vector3.zero;
		}
		if (this.leftHandTarget.position.x == float.NaN || this.leftHandTarget.position.y == float.NaN || this.leftHandTarget.position.z == float.NaN)
		{
			this.leftHandTarget.position = Vector3.zero;
			this.leftHandVel = Vector3.zero;
		}
		GameObject gameObject = this.cardParent.transform.GetChild(this.currentCardSelected).gameObject;
		Vector3 vector = Vector3.zero;
		vector = gameObject.transform.GetChild(0).position;
		if (this.currentCardSelected < 2)
		{
			this.leftHandVel += (vector - this.leftHandTarget.position) * this.spring * Time.unscaledDeltaTime;
			this.leftHandVel -= this.leftHandVel * Time.unscaledDeltaTime * this.drag;
			this.rightHandVel += (this.rightHandRestPos - this.rightHandTarget.position) * this.spring * Time.unscaledDeltaTime * 0.5f;
			this.rightHandVel -= this.rightHandVel * Time.unscaledDeltaTime * this.drag * 0.5f;
			this.rightHandVel += this.sway * new Vector3(-0.5f + Mathf.PerlinNoise(Time.unscaledTime * this.swaySpeed, 0f), -0.5f + Mathf.PerlinNoise(Time.unscaledTime * this.swaySpeed + 100f, 0f), 0f) * Time.unscaledDeltaTime;
			this.shieldGem.transform.position = this.rightHandTarget.position;
			if (this.framesToSnap > 0)
			{
				this.leftHandTarget.position = vector;
			}
		}
		else
		{
			this.rightHandVel += (vector - this.rightHandTarget.position) * this.spring * Time.unscaledDeltaTime;
			this.rightHandVel -= this.rightHandVel * Time.unscaledDeltaTime * this.drag;
			this.leftHandVel += (this.leftHandRestPos - this.leftHandTarget.position) * this.spring * Time.unscaledDeltaTime * 0.5f;
			this.leftHandVel -= this.leftHandVel * Time.unscaledDeltaTime * this.drag * 0.5f;
			this.leftHandVel += this.sway * new Vector3(-0.5f + Mathf.PerlinNoise(Time.unscaledTime * this.swaySpeed, Time.unscaledTime * this.swaySpeed), -0.5f + Mathf.PerlinNoise(Time.unscaledTime * this.swaySpeed + 100f, Time.unscaledTime * this.swaySpeed + 100f), 0f) * Time.unscaledDeltaTime;
			this.shieldGem.transform.position = this.leftHandTarget.position;
			if (this.framesToSnap > 0)
			{
				this.rightHandTarget.position = vector;
			}
		}
		this.framesToSnap--;
		this.leftHandTarget.position += this.leftHandVel * Time.unscaledDeltaTime;
		this.rightHandTarget.position += this.rightHandVel * Time.unscaledDeltaTime;
	}

	// Token: 0x0600052F RID: 1327 RVA: 0x0001D974 File Offset: 0x0001BB74
	public void Show(int pickerID = 0, bool animateIn = false)
	{
		this.isShowinig = true;
		base.transform.GetChild(0).gameObject.SetActive(true);
		if (animateIn)
		{
			base.GetComponent<CurveAnimation>().PlayIn();
		}
		else
		{
			base.transform.localScale = Vector3.one * 33f;
		}
		if (this.currentSkin)
		{
			Object.Destroy(this.currentSkin);
		}
		if (PlayerManager.instance.players[pickerID].data.view.IsMine)
		{
			PlayerFace playerFace;
			if (PhotonNetwork.OfflineMode)
			{
				playerFace = CharacterCreatorHandler.instance.selectedPlayerFaces[pickerID];
			}
			else
			{
				playerFace = CharacterCreatorHandler.instance.selectedPlayerFaces[0];
			}
			base.GetComponent<PhotonView>().RPC("RPCA_SetFace", 0, new object[]
			{
				playerFace.eyeID,
				playerFace.eyeOffset,
				playerFace.mouthID,
				playerFace.mouthOffset,
				playerFace.detailID,
				playerFace.detailOffset,
				playerFace.detail2ID,
				playerFace.detail2Offset
			});
		}
		this.currentSkin = Object.Instantiate<GameObject>(PlayerSkinBank.GetPlayerSkinColors(pickerID).gameObject, base.transform.GetChild(0).transform.position, Quaternion.identity, base.transform.GetChild(0).transform);
		this.currentSkin.GetComponentInChildren<ParticleSystem>().Play();
		this.leftHandTarget.position = base.transform.GetChild(0).position;
		this.rightHandTarget.position = base.transform.GetChild(0).position;
		this.leftHandVel *= 0f;
		this.rightHandVel *= 0f;
		base.StopAllCoroutines();
	}

	// Token: 0x06000530 RID: 1328 RVA: 0x0001DB6D File Offset: 0x0001BD6D
	public void Hide()
	{
		base.GetComponent<CurveAnimation>().PlayOut();
		base.StartCoroutine(this.DelayHide());
	}

	// Token: 0x06000531 RID: 1329 RVA: 0x0001DB87 File Offset: 0x0001BD87
	private IEnumerator DelayHide()
	{
		yield return new WaitForSecondsRealtime(0.3f);
		base.transform.GetChild(0).gameObject.SetActive(false);
		this.isShowinig = false;
		yield break;
	}

	// Token: 0x06000532 RID: 1330 RVA: 0x0001DB96 File Offset: 0x0001BD96
	internal void SetCurrentSelected(int toSet)
	{
		base.GetComponent<PhotonView>().RPC("RPCA_SetCurrentSelected", 0, new object[]
		{
			toSet
		});
	}

	// Token: 0x06000533 RID: 1331 RVA: 0x0001DBB8 File Offset: 0x0001BDB8
	[PunRPC]
	internal void RPCA_SetCurrentSelected(int toSet)
	{
		SoundManager.Instance.Play(this.soundCardSwitch, base.transform);
		this.currentCardSelected = toSet;
	}

	// Token: 0x040006BF RID: 1727
	[Header("Sounds")]
	public SoundEvent soundCardSwitch;

	// Token: 0x040006C0 RID: 1728
	public static CardChoiceVisuals instance;

	// Token: 0x040006C1 RID: 1729
	[Header("Settings")]
	public int currentCardSelected;

	// Token: 0x040006C2 RID: 1730
	public GameObject cardParent;

	// Token: 0x040006C3 RID: 1731
	public Transform leftHandTarget;

	// Token: 0x040006C4 RID: 1732
	public Transform rightHandTarget;

	// Token: 0x040006C5 RID: 1733
	public Transform shieldGem;

	// Token: 0x040006C6 RID: 1734
	private Vector3 leftHandRestPos;

	// Token: 0x040006C7 RID: 1735
	private Vector3 rightHandRestPos;

	// Token: 0x040006C8 RID: 1736
	private Vector3 leftHandVel;

	// Token: 0x040006C9 RID: 1737
	private Vector3 rightHandVel;

	// Token: 0x040006CA RID: 1738
	public float spring = 40f;

	// Token: 0x040006CB RID: 1739
	public float drag = 10f;

	// Token: 0x040006CC RID: 1740
	public float sway = 1f;

	// Token: 0x040006CD RID: 1741
	public float swaySpeed = 1f;

	// Token: 0x040006CE RID: 1742
	public int framesToSnap;

	// Token: 0x040006CF RID: 1743
	private bool isShowinig;

	// Token: 0x040006D0 RID: 1744
	private GameObject currentSkin;
}
