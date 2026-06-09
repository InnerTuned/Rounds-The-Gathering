using System;
using Photon.Utilities;

namespace Photon.Pun.Simple
{
	// Token: 0x020002EC RID: 748
	public class ConnectionTickOffsets
	{
		// Token: 0x06001018 RID: 4120 RVA: 0x0004E0B0 File Offset: 0x0004C2B0
		public int ConvertFrameLocalToOrigin(int localFrameId)
		{
			int frameCount = TickEngineSettings.frameCount;
			int num = localFrameId + this.localToOriginFrame;
			if (num >= frameCount)
			{
				num -= frameCount;
			}
			return num;
		}

		// Token: 0x06001019 RID: 4121 RVA: 0x0004E0D8 File Offset: 0x0004C2D8
		public int ConvertFrameOriginToLocal(int originFrameId)
		{
			int frameCount = TickEngineSettings.frameCount;
			int num = originFrameId + this.originToLocalFrame;
			if (num >= frameCount)
			{
				num -= frameCount;
			}
			return num;
		}

		// Token: 0x0600101A RID: 4122 RVA: 0x0004E100 File Offset: 0x0004C300
		public ConnectionTickOffsets(int connId, int originToLocal, int localToOrigin)
		{
			this.connId = connId;
			this.originToLocalFrame = originToLocal;
			this.localToOriginFrame = localToOrigin;
			int frameCount = TickEngineSettings.frameCount;
			this.validFrameMask = new FastBitMask128(frameCount + 1);
			this.frameArriveTime = new float[frameCount];
			this.frameTimeBeforeConsumption = new float[frameCount];
			for (int i = 0; i < frameCount; i++)
			{
				this.frameTimeBeforeConsumption[i] = float.PositiveInfinity;
			}
		}

		// Token: 0x0600101B RID: 4123 RVA: 0x0004E170 File Offset: 0x0004C370
		public void SnapshotAdvance()
		{
			int frameCount = TickEngineSettings.frameCount;
			int num = NetMaster.CurrentFrameId + this.localToOriginFrame;
			if (num >= frameCount)
			{
				num -= frameCount;
			}
			int num2 = this.validFrameMask.CountValidRange(num, TickEngineSettings.quaterFrameCount);
			if (!this.hadInitialSnapshot)
			{
				if (num2 == 0)
				{
					this.advanceCount = 0;
					return;
				}
				if (num2 > TickEngineSettings.targetBufferSize)
				{
					this.advanceCount = num2 - TickEngineSettings.targetBufferSize;
					return;
				}
			}
			if (num2 == 0)
			{
				if (this.frameArrivedTooLate)
				{
					this.numOfSequentialFramesWithTooLargeBuffer = 0;
					this.numOfSequentialFramesWithTooSmallBuffer = 0;
					this.frameArrivedTooLate = false;
					this.advanceCount = 0;
					return;
				}
				this.numOfSequentialFramesWithTooLargeBuffer = 0;
				this.advanceCount = 1;
			}
			else if (num2 < TickEngineSettings.minBufferSize)
			{
				this.numOfSequentialFramesWithTooLargeBuffer = 0;
				this.numOfSequentialFramesWithTooSmallBuffer += (this.frameArrivedTooLate ? 2 : 1);
				this.frameArrivedTooLate = false;
				if (this.numOfSequentialFramesWithTooSmallBuffer >= TickEngineSettings.ticksBeforeGrow)
				{
					this.advanceCount = 0;
					return;
				}
				this.advanceCount = 1;
			}
			else if (num2 > TickEngineSettings.maxBufferSize)
			{
				this.numOfSequentialFramesWithTooSmallBuffer = 0;
				if (this.numOfSequentialFramesWithTooLargeBuffer > TickEngineSettings.ticksBeforeGrow)
				{
					this.advanceCount = num2 - TickEngineSettings.targetBufferSize + 1;
					this.numOfSequentialFramesWithTooLargeBuffer = 0;
				}
				else
				{
					this.advanceCount = 1;
					this.numOfSequentialFramesWithTooLargeBuffer++;
				}
			}
			else
			{
				this.numOfSequentialFramesWithTooLargeBuffer = 0;
				this.numOfSequentialFramesWithTooSmallBuffer = (this.frameArrivedTooLate ? 1 : 0);
				this.advanceCount = 1;
			}
			this.frameArrivedTooLate = false;
		}

		// Token: 0x0600101C RID: 4124 RVA: 0x0004E2D0 File Offset: 0x0004C4D0
		public void PostSnapshot()
		{
			int frameCount = TickEngineSettings.frameCount;
			int num = NetMaster.CurrentFrameId + this.localToOriginFrame;
			if (num >= frameCount)
			{
				num -= frameCount;
			}
			int num2 = num - TickEngineSettings.quaterFrameCount;
			if (num2 < 0)
			{
				num2 += frameCount;
			}
			this.validFrameMask.ClearBitsBefore(num2, TickEngineSettings.quaterFrameCount);
			if (this.advanceCount > 0)
			{
				this.hadInitialSnapshot = true;
			}
			if (this.advanceCount != 1)
			{
				this.localToOriginFrame += this.advanceCount - 1;
				if (this.localToOriginFrame < 0)
				{
					this.localToOriginFrame += frameCount;
				}
				else if (this.localToOriginFrame >= frameCount)
				{
					this.localToOriginFrame -= frameCount;
				}
				this.originToLocalFrame = frameCount - this.localToOriginFrame;
				if (this.originToLocalFrame < 0)
				{
					this.originToLocalFrame += frameCount;
				}
			}
		}

		// Token: 0x04000F21 RID: 3873
		public int connId;

		// Token: 0x04000F22 RID: 3874
		public int originToLocalFrame;

		// Token: 0x04000F23 RID: 3875
		public int localToOriginFrame;

		// Token: 0x04000F24 RID: 3876
		public int numOfSequentialFramesWithTooSmallBuffer;

		// Token: 0x04000F25 RID: 3877
		public int numOfSequentialFramesWithTooLargeBuffer;

		// Token: 0x04000F26 RID: 3878
		public bool frameArrivedTooLate;

		// Token: 0x04000F27 RID: 3879
		public bool hadInitialSnapshot;

		// Token: 0x04000F28 RID: 3880
		public int advanceCount;

		// Token: 0x04000F29 RID: 3881
		public float[] frameArriveTime;

		// Token: 0x04000F2A RID: 3882
		public float[] frameTimeBeforeConsumption;

		// Token: 0x04000F2B RID: 3883
		public FastBitMask128 validFrameMask;
	}
}
