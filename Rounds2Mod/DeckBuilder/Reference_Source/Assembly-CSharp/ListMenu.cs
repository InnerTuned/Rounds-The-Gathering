using System;
using System.Collections;
using SoundImplementation;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000160 RID: 352
public class ListMenu : MonoBehaviour
{
	// Token: 0x0600070C RID: 1804 RVA: 0x00026B52 File Offset: 0x00024D52
	private void Awake()
	{
		ListMenu.instance = this;
	}

	// Token: 0x0600070D RID: 1805 RVA: 0x00026B5A File Offset: 0x00024D5A
	private void Start()
	{
		base.GetComponentInChildren<ListMenuPage>(true).Open();
	}

	// Token: 0x0600070E RID: 1806 RVA: 0x00026B68 File Offset: 0x00024D68
	private void Update()
	{
		if ((this.lastPos - this.bar.transform.position).sqrMagnitude > 0.5f && (MainMenuHandler.instance.isOpen || EscapeMenuHandler.isEscMenu))
		{
			if (!this.playButtonHoverFirst)
			{
				SoundPlayerStatic.Instance.PlayButtonHover();
			}
			else
			{
				this.playButtonHoverFirst = false;
			}
		}
		this.lastPos = this.bar.transform.position;
		if (this.selectedButton && EventSystem.current.currentSelectedGameObject == null)
		{
			EventSystem.current.SetSelectedGameObject(this.selectedButton.gameObject);
		}
		if (EscapeMenuHandler.isEscMenu || this.menuCanvas.activeInHierarchy)
		{
			if (!this.bar.activeSelf)
			{
				this.bar.SetActive(true);
				this.particle.SetActive(true);
				return;
			}
		}
		else if (this.bar.activeSelf)
		{
			this.bar.SetActive(false);
			this.particle.SetActive(false);
		}
	}

	// Token: 0x0600070F RID: 1807 RVA: 0x00026C76 File Offset: 0x00024E76
	internal void DeselectButton()
	{
		if (this.selectedButton)
		{
			this.selectedButton.Deselect();
		}
	}

	// Token: 0x06000710 RID: 1808 RVA: 0x00026C90 File Offset: 0x00024E90
	public void OpenPage(ListMenuPage pageToOpen)
	{
		if (this.selectedPage)
		{
			this.selectedPage.Close();
		}
		this.bar.transform.localScale = new Vector3(this.bar.transform.localScale.x, pageToOpen.barHeight, 1f);
		this.selectedPage = pageToOpen;
		if (MenuControllerHandler.menuControl == MenuControllerHandler.MenuControl.Controller)
		{
			this.SelectButton(this.selectedPage.firstSelected);
			return;
		}
		this.bar.transform.position = Vector3.up * 10000f;
	}

	// Token: 0x06000711 RID: 1809 RVA: 0x00026D29 File Offset: 0x00024F29
	public void SelectButton(ListMenuButton buttonToSelect)
	{
		if (!buttonToSelect || buttonToSelect.hideBar)
		{
			this.bar.transform.position = Vector3.up * 10000f;
			return;
		}
		base.StartCoroutine(this.ISelectButton(buttonToSelect));
	}

	// Token: 0x06000712 RID: 1810 RVA: 0x00026D69 File Offset: 0x00024F69
	public IEnumerator ISelectButton(ListMenuButton buttonToSelect)
	{
		if (this.selectedButton)
		{
			this.selectedButton.Deselect();
		}
		yield return new WaitForEndOfFrame();
		buttonToSelect.Select();
		this.selectedButton = buttonToSelect;
		this.bar.transform.position = buttonToSelect.transform.position;
		if (EventSystem.current.currentSelectedGameObject != this.selectedButton.gameObject)
		{
			EventSystem.current.SetSelectedGameObject(this.selectedButton.gameObject);
		}
		if (buttonToSelect.setBarHeight != 0f)
		{
			this.bar.transform.localScale = new Vector3(this.bar.transform.localScale.x, buttonToSelect.setBarHeight, 1f);
		}
		yield break;
	}

	// Token: 0x06000713 RID: 1811 RVA: 0x00026D7F File Offset: 0x00024F7F
	public void ClearBar()
	{
		if (MenuControllerHandler.menuControl != MenuControllerHandler.MenuControl.Controller)
		{
			this.bar.transform.position = Vector3.up * 10000f;
		}
	}

	// Token: 0x0400087D RID: 2173
	[Header("Settings")]
	public GameObject bar;

	// Token: 0x0400087E RID: 2174
	[Header("Settings")]
	public GameObject particle;

	// Token: 0x0400087F RID: 2175
	public ListMenuButton selectedButton;

	// Token: 0x04000880 RID: 2176
	public ListMenuPage selectedPage;

	// Token: 0x04000881 RID: 2177
	public static ListMenu instance;

	// Token: 0x04000882 RID: 2178
	public GameObject menuCanvas;

	// Token: 0x04000883 RID: 2179
	private bool isActive = true;

	// Token: 0x04000884 RID: 2180
	private Vector3 lastPos;

	// Token: 0x04000885 RID: 2181
	private bool playButtonHoverFirst = true;
}
