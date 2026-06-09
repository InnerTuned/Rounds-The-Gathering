using System;
using InControl;

// Token: 0x0200008F RID: 143
public class PlayerActions : PlayerActionSet
{
	// Token: 0x0600031B RID: 795 RVA: 0x0001395C File Offset: 0x00011B5C
	public PlayerActions()
	{
		this.Fire = base.CreatePlayerAction("Fire");
		this.Start = base.CreatePlayerAction("Start");
		this.Block = base.CreatePlayerAction("Block");
		this.Jump = base.CreatePlayerAction("Jump");
		this.Left = base.CreatePlayerAction("Move Left");
		this.Right = base.CreatePlayerAction("Move Right");
		this.Up = base.CreatePlayerAction("Move Up");
		this.Down = base.CreatePlayerAction("Move Down");
		this.AimLeft = base.CreatePlayerAction("Aim Left");
		this.AimRight = base.CreatePlayerAction("Aim Right");
		this.AimUp = base.CreatePlayerAction("Aim Up");
		this.AimDown = base.CreatePlayerAction("Aim Down");
		this.Move = base.CreateTwoAxisPlayerAction(this.Left, this.Right, this.Down, this.Up);
		this.Aim = base.CreateTwoAxisPlayerAction(this.AimLeft, this.AimRight, this.AimDown, this.AimUp);
	}

	// Token: 0x0600031C RID: 796 RVA: 0x00013A84 File Offset: 0x00011C84
	public static PlayerActions CreateWithKeyboardBindings()
	{
		PlayerActions playerActions = new PlayerActions();
		playerActions.Fire.AddDefaultBinding(1);
		playerActions.Block.AddDefaultBinding(2);
		playerActions.Jump.AddDefaultBinding(new Key[]
		{
			76
		});
		playerActions.Up.AddDefaultBinding(new Key[]
		{
			58
		});
		playerActions.Down.AddDefaultBinding(new Key[]
		{
			54
		});
		playerActions.Left.AddDefaultBinding(new Key[]
		{
			36
		});
		playerActions.Right.AddDefaultBinding(new Key[]
		{
			39
		});
		playerActions.Start.AddDefaultBinding(new Key[]
		{
			72
		});
		playerActions.ListenOptions.IncludeUnknownControllers = true;
		playerActions.ListenOptions.MaxAllowedBindings = 4U;
		playerActions.ListenOptions.UnsetDuplicateBindingsOnSet = true;
		playerActions.ListenOptions.OnBindingFound = delegate(PlayerAction action, BindingSource binding)
		{
			if (binding == new KeyBindingSource(new Key[]
			{
				13
			}))
			{
				action.StopListeningForBinding();
				return false;
			}
			return true;
		};
		BindingListenOptions listenOptions = playerActions.ListenOptions;
		listenOptions.OnBindingAdded = (Action<PlayerAction, BindingSource>)Delegate.Combine(listenOptions.OnBindingAdded, delegate(PlayerAction action, BindingSource binding)
		{
			Debug.Log("Binding added... " + binding.DeviceName + ": " + binding.Name);
		});
		BindingListenOptions listenOptions2 = playerActions.ListenOptions;
		listenOptions2.OnBindingRejected = (Action<PlayerAction, BindingSource, BindingSourceRejectionType>)Delegate.Combine(listenOptions2.OnBindingRejected, delegate(PlayerAction action, BindingSource binding, BindingSourceRejectionType reason)
		{
			Debug.Log("Binding rejected... " + reason);
		});
		return playerActions;
	}

	// Token: 0x0600031D RID: 797 RVA: 0x00013BF8 File Offset: 0x00011DF8
	public static PlayerActions CreateWithControllerBindings()
	{
		PlayerActions playerActions = new PlayerActions();
		playerActions.Fire.AddDefaultBinding(21);
		playerActions.Fire.AddDefaultBinding(16);
		playerActions.Block.AddDefaultBinding(20);
		playerActions.Block.AddDefaultBinding(15);
		playerActions.Jump.AddDefaultBinding(19);
		playerActions.Jump.AddDefaultBinding(17);
		playerActions.Jump.AddDefaultBinding(18);
		playerActions.Start.AddDefaultBinding(101);
		playerActions.Left.AddDefaultBinding(3);
		playerActions.Right.AddDefaultBinding(4);
		playerActions.Up.AddDefaultBinding(1);
		playerActions.Down.AddDefaultBinding(2);
		playerActions.AimLeft.AddDefaultBinding(8);
		playerActions.AimRight.AddDefaultBinding(9);
		playerActions.AimUp.AddDefaultBinding(6);
		playerActions.AimDown.AddDefaultBinding(7);
		playerActions.ListenOptions.IncludeUnknownControllers = true;
		playerActions.ListenOptions.MaxAllowedBindings = 4U;
		playerActions.ListenOptions.UnsetDuplicateBindingsOnSet = true;
		playerActions.ListenOptions.OnBindingFound = delegate(PlayerAction action, BindingSource binding)
		{
			if (binding == new KeyBindingSource(new Key[]
			{
				13
			}))
			{
				action.StopListeningForBinding();
				return false;
			}
			return true;
		};
		BindingListenOptions listenOptions = playerActions.ListenOptions;
		listenOptions.OnBindingAdded = (Action<PlayerAction, BindingSource>)Delegate.Combine(listenOptions.OnBindingAdded, delegate(PlayerAction action, BindingSource binding)
		{
			Debug.Log("Binding added... " + binding.DeviceName + ": " + binding.Name);
		});
		BindingListenOptions listenOptions2 = playerActions.ListenOptions;
		listenOptions2.OnBindingRejected = (Action<PlayerAction, BindingSource, BindingSourceRejectionType>)Delegate.Combine(listenOptions2.OnBindingRejected, delegate(PlayerAction action, BindingSource binding, BindingSourceRejectionType reason)
		{
			Debug.Log("Binding rejected... " + reason);
		});
		return playerActions;
	}

	// Token: 0x0400043D RID: 1085
	public PlayerAction Fire;

	// Token: 0x0400043E RID: 1086
	public PlayerAction Block;

	// Token: 0x0400043F RID: 1087
	public PlayerAction Jump;

	// Token: 0x04000440 RID: 1088
	public PlayerAction Left;

	// Token: 0x04000441 RID: 1089
	public PlayerAction Right;

	// Token: 0x04000442 RID: 1090
	public PlayerAction Up;

	// Token: 0x04000443 RID: 1091
	public PlayerAction Down;

	// Token: 0x04000444 RID: 1092
	public PlayerTwoAxisAction Move;

	// Token: 0x04000445 RID: 1093
	public PlayerTwoAxisAction Aim;

	// Token: 0x04000446 RID: 1094
	public PlayerAction AimLeft;

	// Token: 0x04000447 RID: 1095
	public PlayerAction AimRight;

	// Token: 0x04000448 RID: 1096
	public PlayerAction AimUp;

	// Token: 0x04000449 RID: 1097
	public PlayerAction AimDown;

	// Token: 0x0400044A RID: 1098
	public PlayerAction Start;
}
