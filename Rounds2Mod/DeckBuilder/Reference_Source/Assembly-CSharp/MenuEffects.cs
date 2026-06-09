using System;
using System.Collections;
using TMPro;
using UnityEngine;

// Token: 0x0200016B RID: 363
public class MenuEffects : MonoBehaviour
{
	// Token: 0x0600074A RID: 1866 RVA: 0x00027AA9 File Offset: 0x00025CA9
	private void Awake()
	{
		MenuEffects.instance = this;
	}

	// Token: 0x0600074B RID: 1867 RVA: 0x00027AB1 File Offset: 0x00025CB1
	public void ShakeObject(GameObject objectToShake, Vector3 defaultPos, float amount, float time)
	{
		base.StartCoroutine(this.DoShakeObject(objectToShake, defaultPos, amount, time));
	}

	// Token: 0x0600074C RID: 1868 RVA: 0x00027AC5 File Offset: 0x00025CC5
	private IEnumerator DoShakeObject(GameObject objectToShake, Vector3 defaultPos, float amount, float time)
	{
		float c = 0f;
		while (c < time)
		{
			Vector3 localPosition = defaultPos + Random.onUnitSphere * amount * ((time - c) / time);
			localPosition.z = defaultPos.z;
			objectToShake.transform.localPosition = localPosition;
			c += Time.unscaledDeltaTime;
			yield return null;
		}
		objectToShake.transform.localPosition = defaultPos;
		yield break;
	}

	// Token: 0x0600074D RID: 1869 RVA: 0x00027AEA File Offset: 0x00025CEA
	public void BlinkInColor(TextMeshProUGUI textToBlink, Color blinkColor, Color defaultColor, float seconds)
	{
		base.StartCoroutine(this.DoTextColorBlink(textToBlink, blinkColor, defaultColor, seconds));
	}

	// Token: 0x0600074E RID: 1870 RVA: 0x00027AFE File Offset: 0x00025CFE
	private IEnumerator DoTextColorBlink(TextMeshProUGUI textToBlink, Color blinkColor, Color defaultColor, float seconds)
	{
		float c = 0f;
		while (c < seconds)
		{
			textToBlink.color = blinkColor;
			c += Time.unscaledDeltaTime;
			yield return null;
		}
		textToBlink.color = defaultColor;
		yield break;
	}

	// Token: 0x040008BB RID: 2235
	public static MenuEffects instance;

	// Token: 0x040008BC RID: 2236
	public Color nopeColor;
}
