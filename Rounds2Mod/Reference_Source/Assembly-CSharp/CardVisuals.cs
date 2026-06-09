using System;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000028 RID: 40
public class CardVisuals : MonoBehaviour
{
	// Token: 0x060000BA RID: 186 RVA: 0x00005FF4 File Offset: 0x000041F4
	private void Start()
	{
		this.group = base.transform.Find("Canvas/Front/Grid").GetComponent<CanvasGroup>();
		this.selectedColor = base.GetComponentInParent<CardInfo>().cardColor;
		this.part = base.GetComponentInChildren<GeneralParticleSystem>();
		this.shake = base.GetComponent<ScaleShake>();
		Transform transform = base.transform.Find("Canvas/Front/Background/Art");
		CardInfo componentInParent = base.GetComponentInParent<CardInfo>();
		this.defaultColor = CardChoice.instance.GetCardColor(componentInParent.colorTheme);
		this.selectedColor = CardChoice.instance.GetCardColor2(componentInParent.colorTheme);
		if (componentInParent.cardArt)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(componentInParent.cardArt, transform.transform.position, transform.transform.rotation, transform);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.SetAsFirstSibling();
			gameObject.transform.localScale = Vector3.one;
		}
		this.cardAnims = base.GetComponentsInChildren<CardAnimation>();
		this.isSelected = !this.firstValueToSet;
		this.ChangeSelected(this.firstValueToSet);
	}

	// Token: 0x060000BB RID: 187 RVA: 0x00006109 File Offset: 0x00004309
	public void Leave()
	{
		Object.Destroy(base.transform.root.gameObject);
	}

	// Token: 0x060000BC RID: 188 RVA: 0x00006120 File Offset: 0x00004320
	public void Pick()
	{
		PhotonNetwork.Destroy(base.transform.root.gameObject);
	}

	// Token: 0x060000BD RID: 189 RVA: 0x00006138 File Offset: 0x00004338
	public void ChangeSelected(bool setSelected)
	{
		if (!this.part)
		{
			return;
		}
		if (this.isSelected == setSelected)
		{
			return;
		}
		this.isSelected = setSelected;
		Action<bool> action = this.toggleSelectionAction;
		if (action != null)
		{
			action.Invoke(this.isSelected);
		}
		if (this.isSelected)
		{
			this.part.simulationSpeedMultiplier = 1.25f;
			this.part.particleSettings.randomColor = this.selectedColor;
			this.shake.targetScale = 1.15f;
			this.group.alpha = 1f;
			for (int i = 0; i < this.images.Length; i++)
			{
				this.images[i].color = this.defaultColor;
			}
			for (int j = 0; j < this.objectsToToggle.Length; j++)
			{
				this.objectsToToggle[j].SetActive(false);
			}
			for (int k = 0; k < this.cardAnims.Length; k++)
			{
				this.cardAnims[k].enabled = true;
			}
			this.nameText.color = this.defaultColor;
			CurveAnimation[] componentsInChildren = base.GetComponentsInChildren<CurveAnimation>();
			for (int l = 0; l < componentsInChildren.Length; l++)
			{
				if (componentsInChildren[l].transform.parent != base.transform)
				{
					componentsInChildren[l].PlayIn();
				}
				else if (componentsInChildren[l].currentState != CurveAnimationUse.In)
				{
					componentsInChildren[l].PlayIn();
				}
			}
			return;
		}
		this.part.simulationSpeedMultiplier = 0.5f;
		this.part.particleSettings.randomColor = this.unSelectedColor;
		this.shake.targetScale = 0.9f;
		this.group.alpha = 0.15f;
		for (int m = 0; m < this.images.Length; m++)
		{
			this.images[m].color = this.chillColor;
		}
		for (int n = 0; n < this.objectsToToggle.Length; n++)
		{
			this.objectsToToggle[n].SetActive(true);
		}
		for (int num = 0; num < this.cardAnims.Length; num++)
		{
			this.cardAnims[num].enabled = false;
		}
		this.nameText.color = this.chillColor;
		CurveAnimation[] componentsInChildren2 = base.GetComponentsInChildren<CurveAnimation>();
		for (int num2 = 0; num2 < componentsInChildren2.Length; num2++)
		{
			if (componentsInChildren2[num2].transform.parent != base.transform)
			{
				componentsInChildren2[num2].PlayOut();
			}
		}
	}

	// Token: 0x040000BA RID: 186
	private ScaleShake shake;

	// Token: 0x040000BB RID: 187
	public bool isSelected;

	// Token: 0x040000BC RID: 188
	private GeneralParticleSystem part;

	// Token: 0x040000BD RID: 189
	private Color selectedColor;

	// Token: 0x040000BE RID: 190
	private Color unSelectedColor = new Color(0.1f, 0.1f, 0.1f);

	// Token: 0x040000BF RID: 191
	public Color defaultColor;

	// Token: 0x040000C0 RID: 192
	public Color chillColor;

	// Token: 0x040000C1 RID: 193
	public Image[] images;

	// Token: 0x040000C2 RID: 194
	public TextMeshProUGUI nameText;

	// Token: 0x040000C3 RID: 195
	public GameObject statsObj;

	// Token: 0x040000C4 RID: 196
	public GameObject[] objectsToToggle;

	// Token: 0x040000C5 RID: 197
	private CardAnimation[] cardAnims;

	// Token: 0x040000C6 RID: 198
	private CanvasGroup group;

	// Token: 0x040000C7 RID: 199
	public bool firstValueToSet;

	// Token: 0x040000C8 RID: 200
	public Action<bool> toggleSelectionAction;
}
