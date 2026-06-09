using System;

namespace Photon.Pun.Simple
{
	// Token: 0x020002E7 RID: 743
	public abstract class FrameBase
	{
		// Token: 0x06000FE8 RID: 4072 RVA: 0x00003CCC File Offset: 0x00001ECC
		public FrameBase()
		{
		}

		// Token: 0x06000FE9 RID: 4073 RVA: 0x0004D87F File Offset: 0x0004BA7F
		public FrameBase(int frameId)
		{
			this.frameId = frameId;
		}

		// Token: 0x06000FEA RID: 4074 RVA: 0x0004D88E File Offset: 0x0004BA8E
		public static TFrame Instantiate<TFrame>(int frameId) where TFrame : FrameBase
		{
			TFrame tframe = (TFrame)((object)Activator.CreateInstance(typeof(TFrame)));
			tframe.frameId = frameId;
			return tframe;
		}

		// Token: 0x06000FEB RID: 4075 RVA: 0x0004D8B0 File Offset: 0x0004BAB0
		public virtual void CopyFrom(FrameBase sourceFrame)
		{
			this.content = sourceFrame.content;
		}

		// Token: 0x06000FEC RID: 4076 RVA: 0x0004D8BE File Offset: 0x0004BABE
		public virtual void Clear()
		{
			this.content = FrameContents.Empty;
		}

		// Token: 0x06000FED RID: 4077 RVA: 0x0004D8C8 File Offset: 0x0004BAC8
		public static void PopulateFrames<TFrame>(ref TFrame[] frames) where TFrame : FrameBase
		{
			int frameCount = TickEngineSettings.frameCount;
			frames = new TFrame[frameCount + 1];
			for (int i = 0; i <= frameCount; i++)
			{
				TFrame tframe = FrameBase.Instantiate<TFrame>(i);
				frames[i] = tframe;
			}
		}

		// Token: 0x04000EEF RID: 3823
		public int frameId;

		// Token: 0x04000EF0 RID: 3824
		public FrameContents content;
	}
}
