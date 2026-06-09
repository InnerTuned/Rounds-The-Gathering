using System;
using TMPro;
using UnityEngine;

// Token: 0x020001A1 RID: 417
public class RegionSelector : MonoBehaviour
{
	// Token: 0x06000864 RID: 2148 RVA: 0x0002CFD4 File Offset: 0x0002B1D4
	private void Start()
	{
		this.dropDown = base.GetComponent<TMP_Dropdown>();
		RegionSelector.region = this.dropDown.options[PlayerPrefs.GetInt("Region", 0)].text;
		this.dropDown.value = PlayerPrefs.GetInt("Region", 0);
		NetworkConnectionHandler.instance.hasRegionSelect = true;
	}

	// Token: 0x06000865 RID: 2149 RVA: 0x0002D033 File Offset: 0x0002B233
	public void OnValueChanged()
	{
		PlayerPrefs.SetInt("Region", this.dropDown.value);
		RegionSelector.region = this.dropDown.options[this.dropDown.value].text;
	}

	// Token: 0x040009A9 RID: 2473
	private TMP_Dropdown dropDown;

	// Token: 0x040009AA RID: 2474
	public static string region = "";
}
