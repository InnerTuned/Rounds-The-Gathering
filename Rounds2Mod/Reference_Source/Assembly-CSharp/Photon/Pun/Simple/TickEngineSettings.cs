using System;
using Photon.Utilities;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x020002EB RID: 747
	public class TickEngineSettings : SettingsScriptableObject<TickEngineSettings>
	{
		// Token: 0x17000123 RID: 291
		// (get) Token: 0x06001012 RID: 4114 RVA: 0x0004DF01 File Offset: 0x0004C101
		public static TickEngineSettings.LogInfoLevel LogLevel
		{
			get
			{
				return SettingsScriptableObject<TickEngineSettings>.Single.logLevel;
			}
		}

		// Token: 0x17000124 RID: 292
		// (get) Token: 0x06001013 RID: 4115 RVA: 0x0004DF0D File Offset: 0x0004C10D
		public static int MaxKeyframes
		{
			get
			{
				return (int)(SettingsScriptableObject<TickEngineSettings>.Single._frameCount / (TickEngineSettings.FrameCountEnum)3);
			}
		}

		// Token: 0x06001014 RID: 4116 RVA: 0x0004DF1B File Offset: 0x0004C11B
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		public static void Bootstrap()
		{
			SettingsScriptableObject<TickEngineSettings>.Single.CalculateBufferValues();
		}

		// Token: 0x06001015 RID: 4117 RVA: 0x0004DF28 File Offset: 0x0004C128
		private void CalculateBufferValues()
		{
			if (this._bufferCorrection == TickEngineSettings.BufferCorrection.Manual)
			{
				TickEngineSettings.minBufferSize = this._minBufferSize;
				TickEngineSettings.maxBufferSize = this._maxBufferSize;
			}
			else
			{
				TickEngineSettings.minBufferSize = this._minBufferSize;
				TickEngineSettings.maxBufferSize = this._maxBufferSize;
			}
			TickEngineSettings.frameCount = (int)this._frameCount;
			TickEngineSettings.halfFrameCount = TickEngineSettings.frameCount / 2;
			TickEngineSettings.thirdFrameCount = TickEngineSettings.frameCount / 3;
			TickEngineSettings.quaterFrameCount = TickEngineSettings.frameCount / 4;
			TickEngineSettings.frameCountBits = TickEngineSettings.frameCount.GetBitsForMaxValue();
			TickEngineSettings.netTickInterval = Time.fixedDeltaTime * (float)this._sendEveryXTick;
			TickEngineSettings.netTickIntervalInv = 1f / (Time.fixedDeltaTime * (float)this._sendEveryXTick);
			TickEngineSettings.targetBufferInterval = Time.fixedDeltaTime * (float)this._sendEveryXTick * (float)this._targetBufferSize;
			TickEngineSettings.sendEveryXTick = this._sendEveryXTick;
			TickEngineSettings.targetBufferSize = this._targetBufferSize;
			TickEngineSettings.ticksBeforeGrow = this._ticksBeforeGrow;
			TickEngineSettings.ticksBeforeShrink = this._ticksBeforeShrink;
			TickEngineSettings.secondsOfBuffer = Time.fixedDeltaTime * (float)this._sendEveryXTick * (float)TickEngineSettings.frameCount;
			TickEngineSettings.secondsOfHalfBuffer = TickEngineSettings.secondsOfBuffer * 0.5f;
			TickEngineSettings.bufferTargSecs = TickEngineSettings.netTickInterval * (float)this._targetBufferSize;
		}

		// Token: 0x04000F05 RID: 3845
		[Space]
		[Tooltip("Enable/Disable for the NetMaster timing callbacks. This needs to be enabled for networking to work.")]
		public bool enableTickEngine = true;

		// Token: 0x04000F06 RID: 3846
		[Tooltip("Disable to pause sending Updates to Relay if no other Players are in the current Room. Default is false to save on data usage. Enable if your code requires updates to echo back to controller.")]
		public bool sendWhenSolo;

		// Token: 0x04000F07 RID: 3847
		[Header("Debugging")]
		[SerializeField]
		private TickEngineSettings.LogInfoLevel logLevel = TickEngineSettings.LogInfoLevel.WarningsAndErrors;

		// Token: 0x04000F08 RID: 3848
		[Tooltip("The size of the circular buffer.")]
		[SerializeField]
		[HideInInspector]
		private TickEngineSettings.FrameCountEnum _frameCount = TickEngineSettings.FrameCountEnum.FrameCount30;

		// Token: 0x04000F09 RID: 3849
		public static int frameCount;

		// Token: 0x04000F0A RID: 3850
		[SerializeField]
		[HideInInspector]
		private TickEngineSettings.BufferCorrection _bufferCorrection;

		// Token: 0x04000F0B RID: 3851
		[Tooltip("Target size of the frame buffer. This is the number of frames in the buffer that is considered ideal. The lower the number the less latency, but with a greater risk of buffer underruns that lead to extrapolation/hitching.")]
		[SerializeField]
		[HideInInspector]
		private int _targetBufferSize = 2;

		// Token: 0x04000F0C RID: 3852
		public static int targetBufferSize;

		// Token: 0x04000F0D RID: 3853
		[Tooltip("Buffer sizes above this value wll be considered to be excessive, and will trigger multiple frames being processed to shrink the buffer.")]
		[SerializeField]
		[HideInInspector]
		private int _maxBufferSize = 3;

		// Token: 0x04000F0E RID: 3854
		public static int maxBufferSize;

		// Token: 0x04000F0F RID: 3855
		[Tooltip("Buffer sizes below this value will trigger the frames to hold for extra ticks in order to grow the buffer.")]
		[SerializeField]
		[HideInInspector]
		private int _minBufferSize = 1;

		// Token: 0x04000F10 RID: 3856
		public static int minBufferSize;

		// Token: 0x04000F11 RID: 3857
		[Tooltip("The number of ticks a buffer will be allowed to be below the the Min Buffer Size before starting to correct. This value prevents overreaction to network hiccups and allows for a few ticks before applying harsh corrections. Ideally this value will be larger than Ticks Before Shrink.")]
		[SerializeField]
		[HideInInspector]
		private int _ticksBeforeGrow = 8;

		// Token: 0x04000F12 RID: 3858
		public static int ticksBeforeGrow;

		// Token: 0x04000F13 RID: 3859
		[Tooltip("The number of ticks a buffer will be allowed to exceed Max Buffer Size before starting to correct. This value prevents overreaction to network hiccups and allows for a few ticks before applying harsh corrections. Ideally this value will be smaller than Ticks Before Grow.")]
		[SerializeField]
		[HideInInspector]
		private int _ticksBeforeShrink = 5;

		// Token: 0x04000F14 RID: 3860
		public static int ticksBeforeShrink;

		// Token: 0x04000F15 RID: 3861
		[Tooltip("States are sent post PhysX/FixedUpdate. Setting this to a value greater than one reduces these sends by only sending every X fixed tick.\n1 = Every Tick\n2 = Every Other\n3 = Every Third, etc.")]
		[SerializeField]
		[HideInInspector]
		private int _sendEveryXTick = 3;

		// Token: 0x04000F16 RID: 3862
		public static int sendEveryXTick = 3;

		// Token: 0x04000F17 RID: 3863
		[Space(4f)]
		public static int halfFrameCount;

		// Token: 0x04000F18 RID: 3864
		public static int thirdFrameCount;

		// Token: 0x04000F19 RID: 3865
		public static int quaterFrameCount;

		// Token: 0x04000F1A RID: 3866
		public static int frameCountBits;

		// Token: 0x04000F1B RID: 3867
		public static float netTickInterval;

		// Token: 0x04000F1C RID: 3868
		public static float netTickIntervalInv;

		// Token: 0x04000F1D RID: 3869
		public static float targetBufferInterval;

		// Token: 0x04000F1E RID: 3870
		private static float secondsOfBuffer;

		// Token: 0x04000F1F RID: 3871
		private static float secondsOfHalfBuffer;

		// Token: 0x04000F20 RID: 3872
		private static float bufferTargSecs;

		// Token: 0x020003DA RID: 986
		public enum FrameCountEnum
		{
			// Token: 0x0400131F RID: 4895
			FrameCount12 = 12,
			// Token: 0x04001320 RID: 4896
			FrameCount30 = 30,
			// Token: 0x04001321 RID: 4897
			FrameCount60 = 60,
			// Token: 0x04001322 RID: 4898
			FrameCount120 = 120
		}

		// Token: 0x020003DB RID: 987
		public enum BufferCorrection
		{
			// Token: 0x04001324 RID: 4900
			Manual,
			// Token: 0x04001325 RID: 4901
			Auto
		}

		// Token: 0x020003DC RID: 988
		public enum LogInfoLevel
		{
			// Token: 0x04001327 RID: 4903
			All,
			// Token: 0x04001328 RID: 4904
			WarningsAndErrors,
			// Token: 0x04001329 RID: 4905
			ErrorsOnly,
			// Token: 0x0400132A RID: 4906
			None
		}
	}
}
