using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x0200016D RID: 365
public class MultiOptions : MonoBehaviour
{
	// Token: 0x06000753 RID: 1875 RVA: 0x000027C8 File Offset: 0x000009C8
	private void Start()
	{
	}

	// Token: 0x06000754 RID: 1876 RVA: 0x00027BA9 File Offset: 0x00025DA9
	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Escape))
		{
			this.Close();
		}
	}

	// Token: 0x06000755 RID: 1877 RVA: 0x00027BC6 File Offset: 0x00025DC6
	internal void ClickRessButton(MultiOptionsButton buttonPressed)
	{
		this.targetButton.SetResolutionAndFullscreen(buttonPressed.currentRess, buttonPressed.currentFull);
		this.Close();
	}

	// Token: 0x06000756 RID: 1878 RVA: 0x00027BE8 File Offset: 0x00025DE8
	public void Open(OptionsButton.SettingsTarget settingsTarget, Vector3 pos, OptionsButton askingButton)
	{
		this.targetButton = askingButton;
		base.transform.position = pos;
		this.source = base.transform.GetChild(0).gameObject;
		if (settingsTarget == OptionsButton.SettingsTarget.Resolution)
		{
			this.PopulateRess(Screen.resolutions);
		}
		else
		{
			this.PopulateFullScreens(new Optionshandler.FullScreenOption[]
			{
				Optionshandler.FullScreenOption.FullScreen,
				Optionshandler.FullScreenOption.WindowedFullScreen,
				Optionshandler.FullScreenOption.Windowed
			});
		}
		base.gameObject.SetActive(true);
		ListMenuButton[] componentsInChildren = base.transform.parent.GetComponentsInChildren<ListMenuButton>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].transform.parent != base.transform)
			{
				componentsInChildren[i].enabled = false;
			}
		}
		CanvasGroup[] componentsInChildren2 = base.transform.parent.GetComponentsInChildren<CanvasGroup>();
		for (int j = 0; j < componentsInChildren2.Length; j++)
		{
			componentsInChildren2[j].alpha = 0.05f;
			componentsInChildren2[j].interactable = false;
		}
		if (MenuControllerHandler.menuControl == MenuControllerHandler.MenuControl.Controller)
		{
			base.StartCoroutine(this.WaitFrame());
			return;
		}
		ListMenu.instance.ClearBar();
	}

	// Token: 0x06000757 RID: 1879 RVA: 0x00027CE5 File Offset: 0x00025EE5
	private IEnumerator WaitFrame()
	{
		yield return new WaitForSecondsRealtime(0f);
		ListMenu.instance.SelectButton(base.GetComponentInChildren<ListMenuButton>());
		yield break;
	}

	// Token: 0x06000758 RID: 1880 RVA: 0x00027CF4 File Offset: 0x00025EF4
	public void Close()
	{
		base.gameObject.SetActive(false);
		ListMenuButton[] componentsInChildren = base.transform.parent.GetComponentsInChildren<ListMenuButton>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].transform.parent != base.transform)
			{
				componentsInChildren[i].enabled = true;
			}
		}
		CanvasGroup[] componentsInChildren2 = base.transform.parent.GetComponentsInChildren<CanvasGroup>();
		for (int j = 0; j < componentsInChildren2.Length; j++)
		{
			componentsInChildren2[j].alpha = 1f;
			componentsInChildren2[j].interactable = true;
		}
		if (MenuControllerHandler.menuControl == MenuControllerHandler.MenuControl.Controller)
		{
			ListMenu.instance.SelectButton(this.targetButton.transform.GetComponent<ListMenuButton>());
			return;
		}
		ListMenu.instance.ClearBar();
	}

	// Token: 0x06000759 RID: 1881 RVA: 0x00027DB0 File Offset: 0x00025FB0
	private void PopulateRess(Resolution[] allRess)
	{
		for (int i = base.transform.childCount - 1; i > 0; i--)
		{
			Object.Destroy(base.transform.GetChild(i).gameObject);
		}
		allRess = this.GetBestResolutions();
		for (int j = 0; j < allRess.Length; j++)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.source, this.source.transform.position, this.source.transform.rotation, this.source.transform.parent);
			gameObject.GetComponentInChildren<TextMeshProUGUI>().text = allRess[j].width + " x " + allRess[j].height;
			gameObject.GetComponent<MultiOptionsButton>().currentRess = allRess[j];
			gameObject.gameObject.SetActive(true);
		}
	}

	// Token: 0x0600075A RID: 1882 RVA: 0x00027E98 File Offset: 0x00026098
	private Resolution[] GetBestResolutions()
	{
		List<Resolution> list = new List<Resolution>();
		for (int i = 0; i < Screen.resolutions.Length; i++)
		{
			if (this.IsBestRess(Screen.resolutions[i]))
			{
				list.Add(Screen.resolutions[i]);
			}
		}
		return list.ToArray();
	}

	// Token: 0x0600075B RID: 1883 RVA: 0x00027EE8 File Offset: 0x000260E8
	private bool IsBestRess(Resolution newRess)
	{
		for (int i = 0; i < Screen.resolutions.Length; i++)
		{
			if (Screen.resolutions[i].width == newRess.width && Screen.resolutions[i].height == newRess.height && newRess.refreshRate < Screen.resolutions[i].refreshRate)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600075C RID: 1884 RVA: 0x00027F58 File Offset: 0x00026158
	private void PopulateFullScreens(Optionshandler.FullScreenOption[] allScreens)
	{
		for (int i = base.transform.childCount - 1; i > 0; i--)
		{
			Object.Destroy(base.transform.GetChild(i).gameObject);
		}
		for (int j = 0; j < allScreens.Length; j++)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.source, this.source.transform.position, this.source.transform.rotation, this.source.transform.parent);
			if (allScreens[j] == Optionshandler.FullScreenOption.WindowedFullScreen)
			{
				gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "WINDOWED FULLSCREEN";
			}
			else
			{
				gameObject.GetComponentInChildren<TextMeshProUGUI>().text = allScreens[j].ToString().ToUpper();
			}
			gameObject.GetComponent<MultiOptionsButton>().currentFull = allScreens[j];
			gameObject.gameObject.SetActive(true);
		}
	}

	// Token: 0x040008C1 RID: 2241
	private GameObject source;

	// Token: 0x040008C2 RID: 2242
	private OptionsButton targetButton;
}
