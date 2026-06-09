using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000118 RID: 280
public class CharacterSelectionInstance : MonoBehaviour
{
	// Token: 0x06000584 RID: 1412 RVA: 0x0001FBE3 File Offset: 0x0001DDE3
	private void Start()
	{
		this.selectors = base.transform.parent.GetComponentsInChildren<CharacterSelectionInstance>();
	}

	// Token: 0x06000585 RID: 1413 RVA: 0x0001FBFB File Offset: 0x0001DDFB
	public void ResetMenu()
	{
		base.transform.GetChild(0).gameObject.SetActive(false);
		this.currentPlayer = null;
		this.getReadyObj.gameObject.SetActive(false);
		PlayerManager.instance.RemovePlayers();
	}

	// Token: 0x06000586 RID: 1414 RVA: 0x0001FC36 File Offset: 0x0001DE36
	private void OnEnable()
	{
		if (!base.transform.GetChild(0).gameObject.activeSelf)
		{
			base.GetComponentInChildren<GeneralParticleSystem>(true).gameObject.SetActive(true);
			base.GetComponentInChildren<GeneralParticleSystem>(true).Play();
		}
	}

	// Token: 0x06000587 RID: 1415 RVA: 0x0001FC70 File Offset: 0x0001DE70
	public void StartPicking(Player pickingPlayer)
	{
		this.currentPlayer = pickingPlayer;
		this.currentlySelectedFace = 0;
		base.GetComponentInChildren<GeneralParticleSystem>(true).gameObject.SetActive(false);
		base.GetComponentInChildren<GeneralParticleSystem>(true).Stop();
		base.transform.GetChild(0).gameObject.SetActive(true);
		this.getReadyObj.gameObject.SetActive(true);
		if (this.currentPlayer.data.input.inputType == GeneralInput.InputType.Keyboard)
		{
			this.getReadyObj.GetComponent<TextMeshProUGUI>().text = "PRESS [SPACE] WHEN READY";
		}
		else
		{
			this.getReadyObj.GetComponent<TextMeshProUGUI>().text = "PRESS [START] WHEN READY";
		}
		this.buttons = base.transform.GetComponentsInChildren<HoverEvent>();
		for (int i = 0; i < this.buttons.Length; i++)
		{
			if (pickingPlayer.data.input.inputType == GeneralInput.InputType.Controller)
			{
				this.buttons[i].enabled = false;
				this.buttons[i].GetComponent<Button>().interactable = false;
				this.buttons[i].GetComponent<CharacterCreatorPortrait>().controlType = MenuControllerHandler.MenuControl.Controller;
			}
			else
			{
				this.buttons[i].enabled = true;
				this.buttons[i].GetComponent<Button>().interactable = true;
				this.buttons[i].GetComponent<CharacterCreatorPortrait>().controlType = MenuControllerHandler.MenuControl.Mouse;
				Navigation navigation = this.buttons[i].GetComponent<Button>().navigation;
				navigation.mode = 0;
				this.buttons[i].GetComponent<Button>().navigation = navigation;
			}
		}
	}

	// Token: 0x06000588 RID: 1416 RVA: 0x0001FDEC File Offset: 0x0001DFEC
	public void ReadyUp()
	{
		this.isReady = !this.isReady;
		bool flag = true;
		for (int i = 0; i < this.selectors.Length; i++)
		{
			if (!this.selectors[i].isReady)
			{
				flag = false;
			}
		}
		if (flag)
		{
			MainMenuHandler.instance.Close();
			GM_ArmsRace.instance.StartGame();
		}
		if (this.currentPlayer.data.input.inputType == GeneralInput.InputType.Keyboard)
		{
			this.getReadyObj.GetComponent<TextMeshProUGUI>().text = (this.isReady ? "READY" : "PRESS [SPACE] WHEN READY");
			return;
		}
		this.getReadyObj.GetComponent<TextMeshProUGUI>().text = (this.isReady ? "READY" : "PRESS [START] WHEN READY");
	}

	// Token: 0x06000589 RID: 1417 RVA: 0x0001FEA8 File Offset: 0x0001E0A8
	private void Update()
	{
		if (!this.currentPlayer)
		{
			return;
		}
		if (this.currentPlayer.data.input.inputType != GeneralInput.InputType.Controller)
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				this.ReadyUp();
			}
			return;
		}
		if (this.currentPlayer.data.playerActions.Device.CommandWasPressed)
		{
			this.ReadyUp();
		}
		HoverEvent component = this.buttons[this.currentlySelectedFace].GetComponent<HoverEvent>();
		if (this.currentButton != component)
		{
			if (this.currentButton)
			{
				this.currentButton.GetComponent<SimulatedSelection>().Deselect();
			}
			this.currentButton = component;
			this.currentButton.GetComponent<SimulatedSelection>().Select();
		}
		this.counter += Time.deltaTime;
		if (Mathf.Abs(this.currentPlayer.data.playerActions.Move.X) > 0.5f && this.counter > 0.2f)
		{
			if (this.currentPlayer.data.playerActions.Move.X > 0.5f)
			{
				this.currentlySelectedFace++;
			}
			else
			{
				this.currentlySelectedFace--;
			}
			this.counter = 0f;
		}
		if (this.currentPlayer.data.playerActions.Jump.WasPressed)
		{
			this.currentButton.GetComponent<Button>().onClick.Invoke();
		}
		if (this.currentPlayer.data.playerActions.Device.Action4.WasPressed)
		{
			this.currentButton.GetComponent<CharacterCreatorPortrait>().EditCharacter();
		}
		this.currentlySelectedFace = Mathf.Clamp(this.currentlySelectedFace, 0, this.buttons.Length - 1);
	}

	// Token: 0x04000727 RID: 1831
	public int currentlySelectedFace;

	// Token: 0x04000728 RID: 1832
	public Player currentPlayer;

	// Token: 0x04000729 RID: 1833
	public GameObject getReadyObj;

	// Token: 0x0400072A RID: 1834
	private HoverEvent currentButton;

	// Token: 0x0400072B RID: 1835
	private CharacterSelectionInstance[] selectors;

	// Token: 0x0400072C RID: 1836
	private HoverEvent[] buttons;

	// Token: 0x0400072D RID: 1837
	public bool isReady;

	// Token: 0x0400072E RID: 1838
	private float counter;
}
