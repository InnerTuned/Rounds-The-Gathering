using System;
using SoundImplementation;
using UnityEngine;

// Token: 0x0200017B RID: 379
public class Optionshandler : MonoBehaviour
{
	// Token: 0x060007AB RID: 1963 RVA: 0x000297DE File Offset: 0x000279DE
	private void Start()
	{
		Optionshandler.instance = this;
		this.LoadOptions();
		this.ApplyOptions();
	}

	// Token: 0x060007AC RID: 1964 RVA: 0x000297F4 File Offset: 0x000279F4
	private void LoadOptions()
	{
		Optionshandler.vol_Master = PlayerPrefs.GetFloat("Vol_Master", 1f);
		Optionshandler.vol_Sfx = PlayerPrefs.GetFloat("vol_Sfx", 1f);
		Optionshandler.vol_Music = PlayerPrefs.GetFloat("vol_Music", 1f);
		Optionshandler.resolution.width = PlayerPrefs.GetInt("res_X", 0);
		Optionshandler.resolution.height = PlayerPrefs.GetInt("res_Y", 0);
		Optionshandler.fullScreen = (Optionshandler.FullScreenOption)PlayerPrefs.GetInt("fullScreen", 1);
		Optionshandler.characterPattrens = this.GetBool(PlayerPrefs.GetInt("characterPattrens", 1));
		Optionshandler.mapPatterns = this.GetBool(PlayerPrefs.GetInt("mapPatterns", 1));
		Optionshandler.leftStickAim = this.GetBool(PlayerPrefs.GetInt("leftStickAim", 1));
		Optionshandler.vSync = this.GetBool(PlayerPrefs.GetInt("vSync", 0));
		Optionshandler.lockStick = this.GetBool(PlayerPrefs.GetInt("lockStick", 0));
		Optionshandler.lockMouse = this.GetBool(PlayerPrefs.GetInt("lockMouse", 0));
		Optionshandler.showCardStatNumbers = this.GetBool(PlayerPrefs.GetInt("showCardStatNumbers", 0));
		if (Optionshandler.resolution.height == 0 || Optionshandler.resolution.width == 0)
		{
			Optionshandler.resolution = Screen.currentResolution;
			this.SaveOptions();
			return;
		}
		this.ApplyOptions();
	}

	// Token: 0x060007AD RID: 1965 RVA: 0x00029940 File Offset: 0x00027B40
	private void SaveOptions()
	{
		PlayerPrefs.SetFloat("Vol_Master", Optionshandler.vol_Master);
		PlayerPrefs.SetFloat("vol_Sfx", Optionshandler.vol_Sfx);
		PlayerPrefs.SetFloat("vol_Music", Optionshandler.vol_Music);
		PlayerPrefs.SetInt("res_X", Optionshandler.resolution.width);
		PlayerPrefs.SetInt("res_Y", Optionshandler.resolution.height);
		PlayerPrefs.SetInt("fullScreen", (int)Optionshandler.fullScreen);
		PlayerPrefs.SetInt("characterPattrens", this.GetInt(Optionshandler.characterPattrens));
		PlayerPrefs.SetInt("mapPatterns", this.GetInt(Optionshandler.mapPatterns));
		PlayerPrefs.SetInt("leftStickAim", this.GetInt(Optionshandler.leftStickAim));
		PlayerPrefs.SetInt("vSync", this.GetInt(Optionshandler.vSync));
		PlayerPrefs.SetInt("lockMouse", this.GetInt(Optionshandler.lockMouse));
		PlayerPrefs.SetInt("lockStick", this.GetInt(Optionshandler.lockStick));
		PlayerPrefs.SetInt("showCardStatNumbers", this.GetInt(Optionshandler.showCardStatNumbers));
		this.ApplyOptions();
	}

	// Token: 0x060007AE RID: 1966 RVA: 0x00029A4C File Offset: 0x00027C4C
	private void ApplyOptions()
	{
		Screen.SetResolution(Optionshandler.resolution.width, Optionshandler.resolution.height, (FullScreenMode)Optionshandler.fullScreen);
		QualitySettings.vSyncCount = (Optionshandler.vSync ? 1 : 0);
		SoundVolumeManager.Instance.SetAudioMixerVolumes(Optionshandler.vol_Master, Optionshandler.vol_Music, Optionshandler.vol_Sfx);
	}

	// Token: 0x060007AF RID: 1967 RVA: 0x00029AA0 File Offset: 0x00027CA0
	private bool GetBool(int value)
	{
		return value == 1;
	}

	// Token: 0x060007B0 RID: 1968 RVA: 0x00029AA9 File Offset: 0x00027CA9
	private int GetInt(bool value)
	{
		if (!value)
		{
			return 0;
		}
		return 1;
	}

	// Token: 0x060007B1 RID: 1969 RVA: 0x00029AB1 File Offset: 0x00027CB1
	public void SetResolution(Resolution resolutionToSet)
	{
		Optionshandler.resolution = resolutionToSet;
		this.SaveOptions();
	}

	// Token: 0x060007B2 RID: 1970 RVA: 0x00029ABF File Offset: 0x00027CBF
	public void SetVSync(bool vSyncToSet)
	{
		Optionshandler.vSync = vSyncToSet;
		this.SaveOptions();
	}

	// Token: 0x060007B3 RID: 1971 RVA: 0x00029ACD File Offset: 0x00027CCD
	public void SetVolMaster(float vol_Master_ToSet)
	{
		Optionshandler.vol_Master = vol_Master_ToSet;
		this.SaveOptions();
	}

	// Token: 0x060007B4 RID: 1972 RVA: 0x00029ADB File Offset: 0x00027CDB
	public void SetVolSFX(float vol_SFX_ToSet)
	{
		Optionshandler.vol_Sfx = vol_SFX_ToSet;
		this.SaveOptions();
	}

	// Token: 0x060007B5 RID: 1973 RVA: 0x00029AE9 File Offset: 0x00027CE9
	public void SetVolMusic(float vol_Music_ToSet)
	{
		Optionshandler.vol_Music = vol_Music_ToSet;
		this.SaveOptions();
	}

	// Token: 0x060007B6 RID: 1974 RVA: 0x00029AF7 File Offset: 0x00027CF7
	public void SetCharacterPatterns(bool characterPatternsToSet)
	{
		Optionshandler.characterPattrens = characterPatternsToSet;
		this.SaveOptions();
	}

	// Token: 0x060007B7 RID: 1975 RVA: 0x00029B05 File Offset: 0x00027D05
	public void SetMapPatterns(bool mapPatternsToSet)
	{
		Optionshandler.mapPatterns = mapPatternsToSet;
		this.SaveOptions();
	}

	// Token: 0x060007B8 RID: 1976 RVA: 0x00029B13 File Offset: 0x00027D13
	public void SetLeftStickAim(bool leftStickAimToSet)
	{
		Optionshandler.leftStickAim = leftStickAimToSet;
		this.SaveOptions();
	}

	// Token: 0x060007B9 RID: 1977 RVA: 0x00029B21 File Offset: 0x00027D21
	public void lockMouseAim(bool setLockMouse)
	{
		Optionshandler.lockMouse = setLockMouse;
		this.SaveOptions();
	}

	// Token: 0x060007BA RID: 1978 RVA: 0x00029B2F File Offset: 0x00027D2F
	public void lockStickAim(bool setLockStick)
	{
		Optionshandler.lockStick = setLockStick;
		this.SaveOptions();
	}

	// Token: 0x060007BB RID: 1979 RVA: 0x00029B3D File Offset: 0x00027D3D
	public void SetShowCardStatNumbers(bool showCardStatNumbersToSet)
	{
		Optionshandler.showCardStatNumbers = showCardStatNumbersToSet;
		this.SaveOptions();
	}

	// Token: 0x060007BC RID: 1980 RVA: 0x00029B4B File Offset: 0x00027D4B
	public void SetFullScreen(Optionshandler.FullScreenOption fullscreenToSet)
	{
		Optionshandler.fullScreen = fullscreenToSet;
		this.SaveOptions();
	}

	// Token: 0x04000903 RID: 2307
	public static Resolution resolution;

	// Token: 0x04000904 RID: 2308
	public static float vol_Master;

	// Token: 0x04000905 RID: 2309
	public static float vol_Sfx;

	// Token: 0x04000906 RID: 2310
	public static float vol_Music;

	// Token: 0x04000907 RID: 2311
	public static Optionshandler.FullScreenOption fullScreen;

	// Token: 0x04000908 RID: 2312
	public static bool characterPattrens;

	// Token: 0x04000909 RID: 2313
	public static bool mapPatterns;

	// Token: 0x0400090A RID: 2314
	public static bool vSync;

	// Token: 0x0400090B RID: 2315
	public static bool leftStickAim;

	// Token: 0x0400090C RID: 2316
	public static bool lockMouse;

	// Token: 0x0400090D RID: 2317
	public static bool showCardStatNumbers;

	// Token: 0x0400090E RID: 2318
	public static bool lockStick;

	// Token: 0x0400090F RID: 2319
	public static Optionshandler instance;

	// Token: 0x020003A1 RID: 929
	public enum FullScreenOption
	{
		// Token: 0x04001257 RID: 4695
		FullScreen,
		// Token: 0x04001258 RID: 4696
		WindowedFullScreen,
		// Token: 0x04001259 RID: 4697
		MaximizedWindow,
		// Token: 0x0400125A RID: 4698
		Windowed
	}
}
