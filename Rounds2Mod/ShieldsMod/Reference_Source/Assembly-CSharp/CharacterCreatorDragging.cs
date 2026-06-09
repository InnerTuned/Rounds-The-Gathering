using System;
using InControl;
using UnityEngine;

// Token: 0x0200010C RID: 268
public class CharacterCreatorDragging : MonoBehaviour
{
	// Token: 0x06000544 RID: 1348 RVA: 0x0001E2B5 File Offset: 0x0001C4B5
	private void Start()
	{
		this.creator = base.GetComponentInParent<CharacterCreator>();
	}

	// Token: 0x06000545 RID: 1349 RVA: 0x0001E2C4 File Offset: 0x0001C4C4
	private void Update()
	{
		if (this.creator.inputType == GeneralInput.InputType.Keyboard || this.creator.inputType == GeneralInput.InputType.Either)
		{
			this.DoMouse();
		}
		if (this.creator.inputType == GeneralInput.InputType.Controller || this.creator.inputType == GeneralInput.InputType.Either)
		{
			this.DoController();
		}
	}

	// Token: 0x06000546 RID: 1350 RVA: 0x0001E314 File Offset: 0x0001C514
	private void DoController()
	{
		int num = -1;
		int barPos = this.creator.nav.barPos;
		for (int i = 0; i < base.transform.childCount; i++)
		{
			CharacterItem component = base.transform.GetChild(i).GetComponent<CharacterItem>();
			if (barPos == 0 && component.itemType == CharacterItemType.Eyes)
			{
				num = i;
			}
			if (barPos == 1 && component.itemType == CharacterItemType.Mouth)
			{
				num = i;
			}
			if (barPos == 2 && component.itemType == CharacterItemType.Detail && component.slotNr == 0)
			{
				num = i;
			}
			if (barPos == 3 && component.itemType == CharacterItemType.Detail && component.slotNr == 1)
			{
				num = i;
			}
		}
		if (num < base.transform.childCount)
		{
			Vector2 vector = Vector2.zero;
			if (this.creator.playerActions != null)
			{
				vector = this.creator.playerActions.Aim.Value;
			}
			else
			{
				for (int j = 0; j < InputManager.ActiveDevices.Count; j++)
				{
					vector = InputManager.ActiveDevices[j].RightStick.Value;
				}
			}
			base.transform.GetChild(num).transform.position += vector * Time.deltaTime * 3f;
			if (vector != Vector2.zero)
			{
				CharacterItem component2 = base.transform.GetChild(num).GetComponent<CharacterItem>();
				Vector2 offset = base.transform.GetChild(num).localPosition - component2.offset;
				this.creator.SetOffset(offset, component2.itemType, component2.slotNr);
			}
		}
	}

	// Token: 0x06000547 RID: 1351 RVA: 0x0001E4B4 File Offset: 0x0001C6B4
	private void DoMouse()
	{
		Vector2 vector = MainCam.instance.cam.ScreenToWorldPoint(Input.mousePosition);
		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			float num = 2f;
			int num2 = -1;
			for (int i = 0; i < base.transform.childCount; i++)
			{
				float num3 = Vector2.Distance(vector, base.transform.GetChild(i).transform.position);
				global::Debug.DrawLine(vector, base.transform.GetChild(i).GetComponent<SpriteRenderer>().bounds.center, Color.red, 2f);
				if (num3 < num)
				{
					num = num3;
					num2 = i;
				}
			}
			if (num2 != -1)
			{
				this.draggedObject = base.transform.GetChild(num2);
			}
		}
		if (Input.GetKeyUp(KeyCode.Mouse0) && this.draggedObject)
		{
			CharacterItem component = this.draggedObject.GetComponent<CharacterItem>();
			Vector2 offset = this.draggedObject.transform.localPosition - component.offset;
			this.creator.SetOffset(offset, component.itemType, component.slotNr);
			this.draggedObject = null;
		}
		if (this.draggedObject)
		{
			this.draggedObject.transform.position += vector - this.lastMouse;
			this.draggedObject.transform.localPosition = Vector2.ClampMagnitude(this.draggedObject.transform.localPosition, 4.5f);
		}
		this.lastMouse = vector;
	}

	// Token: 0x040006E3 RID: 1763
	public Vector2 rightStick;

	// Token: 0x040006E4 RID: 1764
	private CharacterCreator creator;

	// Token: 0x040006E5 RID: 1765
	private Transform draggedObject;

	// Token: 0x040006E6 RID: 1766
	private Vector2 lastMouse = Vector3.zero;
}
