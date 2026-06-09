using System;
using SoundImplementation;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Token: 0x02000156 RID: 342
public class HoverEvent : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x060006EA RID: 1770 RVA: 0x00026430 File Offset: 0x00024630
	public void OnPointerEnter(PointerEventData eventData)
	{
		SoundPlayerStatic.Instance.PlayButtonHover();
		this.enterEvent.Invoke();
		this.isHovered = true;
	}

	// Token: 0x060006EB RID: 1771 RVA: 0x0002644E File Offset: 0x0002464E
	public void OnPointerExit(PointerEventData eventData)
	{
		this.exitEvent.Invoke();
		this.isHovered = false;
	}

	// Token: 0x060006EC RID: 1772 RVA: 0x00026462 File Offset: 0x00024662
	private void Update()
	{
		this.isSelected = (EventSystem.current.currentSelectedGameObject == base.gameObject);
	}

	// Token: 0x060006ED RID: 1773 RVA: 0x0002647F File Offset: 0x0002467F
	private void OnDisable()
	{
		this.isHovered = false;
	}

	// Token: 0x04000856 RID: 2134
	public UnityEvent enterEvent;

	// Token: 0x04000857 RID: 2135
	public UnityEvent exitEvent;

	// Token: 0x04000858 RID: 2136
	public bool isHovered;

	// Token: 0x04000859 RID: 2137
	public bool isSelected;
}
