using System;
using emotitron.Utilities.GUIUtilities;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x020002E9 RID: 745
	[HelpURL("https://doc.photonengine.com/en-us/pun/current/gameplay/simple/simplecoresynccomponents")]
	public abstract class SyncObject : NetComponent, IOnEnable, IOnDisable, IApplyOrder
	{
		// Token: 0x17000119 RID: 281
		// (get) Token: 0x06000FEE RID: 4078 RVA: 0x0004D901 File Offset: 0x0004BB01
		public virtual int ApplyOrder
		{
			get
			{
				return this._applyOrder;
			}
		}

		// Token: 0x1700011A RID: 282
		// (get) Token: 0x06000FEF RID: 4079 RVA: 0x0004D909 File Offset: 0x0004BB09
		// (set) Token: 0x06000FF0 RID: 4080 RVA: 0x0004D911 File Offset: 0x0004BB11
		public bool UseDeltas
		{
			get
			{
				return this.useDeltas;
			}
			set
			{
				this.useDeltas = value;
			}
		}

		// Token: 0x06000FF1 RID: 4081 RVA: 0x0004D91A File Offset: 0x0004BB1A
		public bool IsKeyframe(int frameId)
		{
			return this.keyframeRate != 0 && (frameId % this.keyframeRate == 0 || frameId == TickEngineSettings.frameCount);
		}

		// Token: 0x06000FF2 RID: 4082 RVA: 0x000027C8 File Offset: 0x000009C8
		public virtual void ResetBuffers()
		{
		}

		// Token: 0x1700011B RID: 283
		// (get) Token: 0x06000FF3 RID: 4083 RVA: 0x0004D93A File Offset: 0x0004BB3A
		public virtual bool AlwaysReady
		{
			get
			{
				return this._alwaysReady;
			}
		}

		// Token: 0x1700011C RID: 284
		// (get) Token: 0x06000FF4 RID: 4084 RVA: 0x0004D942 File Offset: 0x0004BB42
		public virtual bool IncludeInSerialization
		{
			get
			{
				return this.serializeThis;
			}
		}

		// Token: 0x1700011D RID: 285
		// (get) Token: 0x06000FF5 RID: 4085 RVA: 0x000429F6 File Offset: 0x00040BF6
		public bool SkipWhenEmpty
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700011E RID: 286
		// (get) Token: 0x06000FF6 RID: 4086 RVA: 0x0004D94A File Offset: 0x0004BB4A
		// (set) Token: 0x06000FF7 RID: 4087 RVA: 0x0004D952 File Offset: 0x0004BB52
		public int SyncObjIndex
		{
			get
			{
				return this.syncObjIndex;
			}
			set
			{
				this.syncObjIndex = value;
			}
		}

		// Token: 0x1700011F RID: 287
		// (get) Token: 0x06000FF8 RID: 4088 RVA: 0x0004D95B File Offset: 0x0004BB5B
		// (set) Token: 0x06000FF9 RID: 4089 RVA: 0x0004D963 File Offset: 0x0004BB63
		public ReadyStateEnum ReadyState
		{
			get
			{
				return this._readyState;
			}
			set
			{
				if (this._readyState == value)
				{
					return;
				}
				this._readyState = value;
				this.netObj.OnSyncObjReadyChange(this, this._readyState);
				if (this.onReadyCallbacks != null)
				{
					this.onReadyCallbacks.Invoke(this, value);
				}
			}
		}

		// Token: 0x06000FFA RID: 4090 RVA: 0x0004D99D File Offset: 0x0004BB9D
		public override void OnPostEnable()
		{
			base.OnPostEnable();
		}

		// Token: 0x06000FFB RID: 4091 RVA: 0x0004D9A8 File Offset: 0x0004BBA8
		public override void OnAuthorityChanged(bool isMine, bool controllerChanged)
		{
			base.OnAuthorityChanged(isMine, controllerChanged);
			if (!controllerChanged)
			{
				return;
			}
			IReadyable readyable = this as IReadyable;
			if (readyable == null || readyable.AlwaysReady || this._readyState != ReadyStateEnum.Unready)
			{
				this.ReadyState = ReadyStateEnum.Ready;
				return;
			}
			if (!base.isActiveAndEnabled)
			{
				this.ReadyState = ReadyStateEnum.Disabled;
				return;
			}
			this.ReadyState = ReadyStateEnum.Unready;
		}

		// Token: 0x06000FFC RID: 4092 RVA: 0x0004D9FA File Offset: 0x0004BBFA
		public override void OnPostDisable()
		{
			base.OnPostDisable();
			this.ReadyState = ReadyStateEnum.Disabled;
		}

		// Token: 0x06000FFD RID: 4093 RVA: 0x0004DA09 File Offset: 0x0004BC09
		public override void OnAwake()
		{
			base.OnAwake();
		}

		// Token: 0x06000FFE RID: 4094 RVA: 0x000027C8 File Offset: 0x000009C8
		protected virtual void OnEnable()
		{
		}

		// Token: 0x06000FFF RID: 4095 RVA: 0x0004DA11 File Offset: 0x0004BC11
		protected int ConvertSecsToTicks(float seconds)
		{
			return (int)(seconds / (Time.fixedDeltaTime * (float)TickEngineSettings.sendEveryXTick));
		}

		// Token: 0x04000EF5 RID: 3829
		public int _applyOrder = 13;

		// Token: 0x04000EF6 RID: 3830
		[Tooltip("Every X Net Tick this object will serialize a full update, regardless of having changed or not.")]
		[Range(1f, 12f)]
		[HideInInspector]
		[SerializeField]
		protected int keyframeRate = 1;

		// Token: 0x04000EF7 RID: 3831
		[Tooltip("When enabled, components will be instructed to check for changes and serialize them. When disabled, components will be instructed to ONLY send keyframes.")]
		[HideInInspector]
		[SerializeField]
		protected bool useDeltas = true;

		// Token: 0x04000EF8 RID: 3832
		private const string ALWAYS_READY_TOOLTIP = "When true, the NetObject will not factor this SyncObj's ready state into firing IOnNetObjReady callbacks.";

		// Token: 0x04000EF9 RID: 3833
		[ShowIfInterface(typeof(IReadyable), "When true, the NetObject will not factor this SyncObj's ready state into firing IOnNetObjReady callbacks.")]
		public bool _alwaysReady = true;

		// Token: 0x04000EFA RID: 3834
		private const string INCLUDE_SERIALIZATION_TOOLTIP = "When false, the NetObject will not serialize this SyncObj's state. This cannot be changed at runtime. Disable this component on the owner to disable sync at runtime instead.";

		// Token: 0x04000EFB RID: 3835
		[Tooltip("When false, the NetObject will not serialize this SyncObj's state. This cannot be changed at runtime. Disable this component on the owner to disable sync at runtime instead.")]
		[HideInInspector]
		public bool serializeThis = true;

		// Token: 0x04000EFC RID: 3836
		[NonSerialized]
		protected int syncObjIndex;

		// Token: 0x04000EFD RID: 3837
		protected ReadyStateEnum _readyState;

		// Token: 0x04000EFE RID: 3838
		public Action<SyncObject, ReadyStateEnum> onReadyCallbacks;
	}
}
