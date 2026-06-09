using System;
using UnityEngine;

// Token: 0x02000162 RID: 354
public class ListMenuPage : MonoBehaviour
{
	// Token: 0x06000721 RID: 1825 RVA: 0x00026F16 File Offset: 0x00025116
	private void Awake()
	{
		this.grid = base.transform.GetChild(0).gameObject;
	}

	// Token: 0x06000722 RID: 1826 RVA: 0x00026F2F File Offset: 0x0002512F
	public void Open()
	{
		ListMenu.instance.OpenPage(this);
		this.grid.SetActive(true);
	}

	// Token: 0x06000723 RID: 1827 RVA: 0x00026F48 File Offset: 0x00025148
	public void Close()
	{
		this.DeselectAll();
		this.grid.SetActive(false);
	}

	// Token: 0x06000724 RID: 1828 RVA: 0x00026F5C File Offset: 0x0002515C
	private void DeselectAll()
	{
		ListMenuButton[] componentsInChildren = base.GetComponentsInChildren<ListMenuButton>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Deselect();
		}
	}

	// Token: 0x04000891 RID: 2193
	public ListMenuButton firstSelected;

	// Token: 0x04000892 RID: 2194
	public float barHeight = 92f;

	// Token: 0x04000893 RID: 2195
	private GameObject grid;
}
