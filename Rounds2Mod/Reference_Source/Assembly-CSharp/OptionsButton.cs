using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200017A RID: 378
public class OptionsButton : MonoBehaviour
{
	// Token: 0x060007A3 RID: 1955 RVA: 0x000293B0 File Offset: 0x000275B0
	private void Awake()
	{
		this.text = base.GetComponentsInChildren<TextMeshProUGUI>(true)[1];
		if (this.settingsType == OptionsButton.SettingsType.Slider)
		{
			this.slider = base.GetComponentInChildren<Slider>(true);
			this.slider.onValueChanged.AddListener(new UnityAction<float>(this.SliderSlide));
			return;
		}
		base.GetComponent<Button>().onClick.AddListener(new UnityAction(this.ClickButton));
	}

	// Token: 0x060007A4 RID: 1956 RVA: 0x0002941C File Offset: 0x0002761C
	private void OnEnable()
	{
		switch (this.settingsTarget)
		{
		case OptionsButton.SettingsTarget.Resolution:
			this.currentResolutionValue = Optionshandler.resolution;
			break;
		case OptionsButton.SettingsTarget.Vol_Master:
			this.currentFloatValue = Optionshandler.vol_Master;
			break;
		case OptionsButton.SettingsTarget.Vol_SFX:
			this.currentFloatValue = Optionshandler.vol_Sfx;
			break;
		case OptionsButton.SettingsTarget.Vol_Music:
			this.currentFloatValue = Optionshandler.vol_Music;
			break;
		case OptionsButton.SettingsTarget.CharacterPattern:
			this.currentBoolValue = Optionshandler.characterPattrens;
			break;
		case OptionsButton.SettingsTarget.MapPattern:
			this.currentBoolValue = Optionshandler.mapPatterns;
			break;
		case OptionsButton.SettingsTarget.leftStickAim:
			this.currentBoolValue = Optionshandler.leftStickAim;
			break;
		case OptionsButton.SettingsTarget.lockAimDirections:
			this.currentBoolValue = Optionshandler.lockMouse;
			break;
		case OptionsButton.SettingsTarget.ShowCardStarts:
			this.currentBoolValue = Optionshandler.showCardStatNumbers;
			break;
		case OptionsButton.SettingsTarget.FullScreen:
			this.currentFullscreenValue = Optionshandler.fullScreen;
			break;
		case OptionsButton.SettingsTarget.FreeStickAim:
			this.currentBoolValue = Optionshandler.lockStick;
			break;
		case OptionsButton.SettingsTarget.Vsync:
			this.currentBoolValue = Optionshandler.vSync;
			break;
		}
		if (this.settingsType == OptionsButton.SettingsType.Slider)
		{
			this.slider.value = this.currentFloatValue;
		}
		this.ReDraw();
	}

	// Token: 0x060007A5 RID: 1957 RVA: 0x0002952B File Offset: 0x0002772B
	public void SetResolutionAndFullscreen(Resolution newRess, Optionshandler.FullScreenOption fullScreen)
	{
		this.currentResolutionValue = newRess;
		this.currentFullscreenValue = fullScreen;
		this.ClickButton();
		if (this.settingsTarget == OptionsButton.SettingsTarget.Resolution)
		{
			Optionshandler.instance.SetResolution(newRess);
		}
		if (this.settingsTarget == OptionsButton.SettingsTarget.FullScreen)
		{
			Optionshandler.instance.SetFullScreen(fullScreen);
		}
	}

	// Token: 0x060007A6 RID: 1958 RVA: 0x00029569 File Offset: 0x00027769
	public void SliderChanged()
	{
		this.currentFloatValue = this.slider.value;
	}

	// Token: 0x060007A7 RID: 1959 RVA: 0x0002957C File Offset: 0x0002777C
	private void SliderSlide(float newVal)
	{
		this.currentFloatValue = newVal;
		this.ClickButton();
	}

	// Token: 0x060007A8 RID: 1960 RVA: 0x0002958C File Offset: 0x0002778C
	public void ClickButton()
	{
		if (this.settingsType == OptionsButton.SettingsType.Binary)
		{
			this.currentBoolValue = !this.currentBoolValue;
		}
		switch (this.settingsTarget)
		{
		case OptionsButton.SettingsTarget.Resolution:
			base.transform.parent.parent.GetComponentInChildren<MultiOptions>(true).Open(this.settingsTarget, base.transform.GetChild(1).position, this);
			break;
		case OptionsButton.SettingsTarget.Vol_Master:
			Optionshandler.instance.SetVolMaster(this.currentFloatValue);
			break;
		case OptionsButton.SettingsTarget.Vol_SFX:
			Optionshandler.instance.SetVolSFX(this.currentFloatValue);
			break;
		case OptionsButton.SettingsTarget.Vol_Music:
			Optionshandler.instance.SetVolMusic(this.currentFloatValue);
			break;
		case OptionsButton.SettingsTarget.CharacterPattern:
			Optionshandler.instance.SetCharacterPatterns(this.currentBoolValue);
			break;
		case OptionsButton.SettingsTarget.MapPattern:
			Optionshandler.instance.SetMapPatterns(this.currentBoolValue);
			break;
		case OptionsButton.SettingsTarget.leftStickAim:
			Optionshandler.instance.SetLeftStickAim(this.currentBoolValue);
			break;
		case OptionsButton.SettingsTarget.lockAimDirections:
			Optionshandler.instance.lockMouseAim(this.currentBoolValue);
			break;
		case OptionsButton.SettingsTarget.ShowCardStarts:
			Optionshandler.instance.SetShowCardStatNumbers(this.currentBoolValue);
			break;
		case OptionsButton.SettingsTarget.FullScreen:
			base.transform.parent.parent.GetComponentInChildren<MultiOptions>(true).Open(this.settingsTarget, base.transform.GetChild(1).position, this);
			break;
		case OptionsButton.SettingsTarget.FreeStickAim:
			Optionshandler.instance.lockStickAim(this.currentBoolValue);
			break;
		case OptionsButton.SettingsTarget.Vsync:
			Optionshandler.instance.SetVSync(this.currentBoolValue);
			break;
		}
		this.ReDraw();
	}

	// Token: 0x060007A9 RID: 1961 RVA: 0x00029728 File Offset: 0x00027928
	private void ReDraw()
	{
		if (this.settingsType == OptionsButton.SettingsType.Binary)
		{
			this.text.text = (this.currentBoolValue ? "YES" : "NO");
		}
		if (this.settingsType == OptionsButton.SettingsType.MultiOption)
		{
			if (this.settingsTarget == OptionsButton.SettingsTarget.Resolution)
			{
				this.text.text = this.currentResolutionValue.width + " X " + this.currentResolutionValue.height;
				return;
			}
			if (this.currentFullscreenValue == Optionshandler.FullScreenOption.WindowedFullScreen)
			{
				this.text.text = "WINDOWED FULLSCREEN";
				return;
			}
			this.text.text = this.currentFullscreenValue.ToString().ToUpper();
		}
	}

	// Token: 0x040008FB RID: 2299
	public OptionsButton.SettingsTarget settingsTarget;

	// Token: 0x040008FC RID: 2300
	public OptionsButton.SettingsType settingsType;

	// Token: 0x040008FD RID: 2301
	public bool currentBoolValue;

	// Token: 0x040008FE RID: 2302
	public Resolution currentResolutionValue;

	// Token: 0x040008FF RID: 2303
	public Optionshandler.FullScreenOption currentFullscreenValue;

	// Token: 0x04000900 RID: 2304
	public float currentFloatValue;

	// Token: 0x04000901 RID: 2305
	private TextMeshProUGUI text;

	// Token: 0x04000902 RID: 2306
	public Slider slider;

	// Token: 0x0200039F RID: 927
	public enum SettingsTarget
	{
		// Token: 0x04001246 RID: 4678
		Resolution,
		// Token: 0x04001247 RID: 4679
		Vol_Master,
		// Token: 0x04001248 RID: 4680
		Vol_SFX,
		// Token: 0x04001249 RID: 4681
		Vol_Music,
		// Token: 0x0400124A RID: 4682
		CharacterPattern,
		// Token: 0x0400124B RID: 4683
		MapPattern,
		// Token: 0x0400124C RID: 4684
		leftStickAim,
		// Token: 0x0400124D RID: 4685
		lockAimDirections,
		// Token: 0x0400124E RID: 4686
		ShowCardStarts,
		// Token: 0x0400124F RID: 4687
		FullScreen,
		// Token: 0x04001250 RID: 4688
		FreeStickAim,
		// Token: 0x04001251 RID: 4689
		Vsync
	}

	// Token: 0x020003A0 RID: 928
	public enum SettingsType
	{
		// Token: 0x04001253 RID: 4691
		Binary,
		// Token: 0x04001254 RID: 4692
		Slider,
		// Token: 0x04001255 RID: 4693
		MultiOption
	}
}
