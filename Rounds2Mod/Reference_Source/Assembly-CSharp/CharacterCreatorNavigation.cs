using System;
using InControl;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000112 RID: 274
public class CharacterCreatorNavigation : MonoBehaviour
{
	// Token: 0x0600056A RID: 1386 RVA: 0x0001F200 File Offset: 0x0001D400
	private void Awake()
	{
		this.creator = base.GetComponent<CharacterCreator>();
		this.dragging = base.GetComponentInChildren<CharacterCreatorDragging>();
		this.grid = base.transform.GetChild(0).GetChild(0);
		this.bar = base.transform.GetChild(0).GetChild(1);
	}

	// Token: 0x0600056B RID: 1387 RVA: 0x0001F258 File Offset: 0x0001D458
	private void Update()
	{
		if (this.creator.playerActions != null)
		{
			this.PlayerActionUpdate(this.creator.playerActions.Device);
			return;
		}
		if (this.creator.playerActions == null)
		{
			this.creator.currentControl = MenuControllerHandler.menuControl;
			for (int i = 0; i < InputManager.ActiveDevices.Count; i++)
			{
				InputDevice device = InputManager.ActiveDevices[i];
				this.PlayerActionUpdate(device);
			}
		}
	}

	// Token: 0x0600056C RID: 1388 RVA: 0x0001F2D0 File Offset: 0x0001D4D0
	private void PlayerActionUpdate(InputDevice device)
	{
		this.cd += Time.unscaledDeltaTime;
		if (device == null)
		{
			return;
		}
		if (device.CommandWasPressed)
		{
			this.creator.Finish();
		}
		this.dragging.rightStick = device.RightStick.Value;
		this.GridMovement(device);
		this.BarMovement(device);
	}

	// Token: 0x0600056D RID: 1389 RVA: 0x0001F32A File Offset: 0x0001D52A
	private void BarMovement(InputDevice device)
	{
		if (!this.currentBarObject)
		{
			this.MoveNav(0);
		}
		if (device.LeftTrigger.WasPressed)
		{
			this.MoveNav(-1);
		}
		if (device.RightTrigger.WasPressed)
		{
			this.MoveNav(1);
		}
	}

	// Token: 0x0600056E RID: 1390 RVA: 0x0001F368 File Offset: 0x0001D568
	private void MoveNav(int delta)
	{
		this.cd = 0f;
		if (delta > 0)
		{
			this.barPos++;
		}
		if (delta < 0)
		{
			this.barPos--;
		}
		this.VerifyBarPos();
	}

	// Token: 0x0600056F RID: 1391 RVA: 0x0001F3A0 File Offset: 0x0001D5A0
	private void VerifyBarPos()
	{
		if (this.currentBarObject)
		{
			this.currentBarObject.GetComponent<HoverEvent>().exitEvent.Invoke();
		}
		this.barPos = Mathf.Clamp(this.barPos, 0, 3);
		this.currentBarObject = this.bar.GetChild(this.barPos).gameObject;
		this.currentBarObject.GetComponent<HoverEvent>().enterEvent.Invoke();
		this.currentBarObject.GetComponent<Button>().onClick.Invoke();
	}

	// Token: 0x06000570 RID: 1392 RVA: 0x0001F428 File Offset: 0x0001D628
	private void GridMovement(InputDevice device)
	{
		if (!this.currentObject)
		{
			this.MoveNav(Vector2.zero);
		}
		if (device.LeftStick.Value.magnitude > 0.75f && this.cd > 0.1f)
		{
			this.MoveNav(device.LeftStick);
		}
		if (device.Action1.WasPressed)
		{
			this.currentObject.GetComponent<Button>().onClick.Invoke();
		}
	}

	// Token: 0x06000571 RID: 1393 RVA: 0x0001F4A8 File Offset: 0x0001D6A8
	private void MoveNav(Vector2 delta)
	{
		this.cd = 0f;
		if (delta.x > 0.5f)
		{
			this.x++;
		}
		if (delta.x < -0.5f)
		{
			this.x--;
		}
		if (delta.y > 0.5f)
		{
			this.y--;
		}
		if (delta.y < -0.5f)
		{
			this.y++;
		}
		this.VerifyPos();
	}

	// Token: 0x06000572 RID: 1394 RVA: 0x0001F534 File Offset: 0x0001D734
	private void VerifyPos()
	{
		if (this.currentObject)
		{
			this.currentObject.GetComponent<HoverEvent>().exitEvent.Invoke();
		}
		this.x = Mathf.Clamp(this.x, 1, this.itemsInRow);
		this.y = Mathf.Clamp(this.y, 0, Mathf.CeilToInt((float)((this.grid.childCount - 2) / this.itemsInRow)));
		this.x = Mathf.Clamp(this.x, 1, this.grid.childCount - this.y * this.itemsInRow - 1);
		int index = this.x + this.y * this.itemsInRow;
		this.currentObject = this.grid.GetChild(index).gameObject;
		this.currentObject.GetComponent<HoverEvent>().enterEvent.Invoke();
	}

	// Token: 0x04000706 RID: 1798
	private int itemsInRow = 7;

	// Token: 0x04000707 RID: 1799
	private CharacterCreator creator;

	// Token: 0x04000708 RID: 1800
	private Transform grid;

	// Token: 0x04000709 RID: 1801
	private Transform bar;

	// Token: 0x0400070A RID: 1802
	private GameObject currentObject;

	// Token: 0x0400070B RID: 1803
	private GameObject currentBarObject;

	// Token: 0x0400070C RID: 1804
	private CharacterCreatorDragging dragging;

	// Token: 0x0400070D RID: 1805
	public int x;

	// Token: 0x0400070E RID: 1806
	public int y;

	// Token: 0x0400070F RID: 1807
	public int barPos;

	// Token: 0x04000710 RID: 1808
	private float cd;
}
