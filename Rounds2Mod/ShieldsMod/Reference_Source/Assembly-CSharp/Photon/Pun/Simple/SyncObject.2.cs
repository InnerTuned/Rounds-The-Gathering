using System;
using Photon.Pun.Simple.Internal;
using Photon.Utilities;

namespace Photon.Pun.Simple
{
	// Token: 0x020002EA RID: 746
	public abstract class SyncObject<TFrame> : SyncObject where TFrame : FrameBase
	{
		// Token: 0x06001001 RID: 4097 RVA: 0x0004DA50 File Offset: 0x0004BC50
		public override void OnAwake()
		{
			if (this.keyframeRate > TickEngineSettings.MaxKeyframes)
			{
				this.keyframeRate = TickEngineSettings.MaxKeyframes;
				Debug.LogWarning(string.Concat(new object[]
				{
					base.name,
					"/",
					base.GetType().Name,
					" keyframe setting exceeds max allowed for the current ",
					SettingsScriptableObject<TickEngineSettings>.single.name,
					".frameCount setting. Reducing to ",
					this.keyframeRate
				}));
			}
			base.OnAwake();
			this.PopulateFrames();
			this.offtickFrame = this.frames[TickEngineSettings.frameCount];
		}

		// Token: 0x06001002 RID: 4098 RVA: 0x0004DAF1 File Offset: 0x0004BCF1
		public override void OnPostDisable()
		{
			base.OnPostDisable();
			this.hadInitialSnapshot = false;
		}

		// Token: 0x06001003 RID: 4099 RVA: 0x0004DB00 File Offset: 0x0004BD00
		public override void OnAuthorityChanged(bool isMine, bool controllerChanged)
		{
			base.OnAuthorityChanged(isMine, controllerChanged);
			if (controllerChanged)
			{
				int controllerActorNr = this.photonView.ControllerActorNr;
				int[] originHistory = this.netObj.originHistory;
				int i = 0;
				int frameCount = TickEngineSettings.frameCount;
				while (i < frameCount)
				{
					if (controllerActorNr != originHistory[i])
					{
						this.frames[i].Clear();
					}
					i++;
				}
			}
			this.hadInitialSnapshot = false;
		}

		// Token: 0x06001004 RID: 4100 RVA: 0x0004DB65 File Offset: 0x0004BD65
		protected virtual void PopulateFrames()
		{
			FrameBase.PopulateFrames<TFrame>(ref this.frames);
		}

		// Token: 0x06001005 RID: 4101 RVA: 0x000027C8 File Offset: 0x000009C8
		protected virtual void InitialCompleteSnapshot(TFrame frame)
		{
		}

		// Token: 0x06001006 RID: 4102 RVA: 0x0004DB74 File Offset: 0x0004BD74
		public virtual bool OnSnapshot(int prevFrameId, int snapFrameId, int targFrameId, bool prevIsValid, bool snapIsValid, bool targIsValid)
		{
			if (!base.enabled)
			{
				return false;
			}
			int frameCount = TickEngineSettings.frameCount;
			int halfFrameCount = TickEngineSettings.halfFrameCount;
			this.prevFrame = this.frames[prevFrameId];
			this.snapFrame = this.frames[snapFrameId];
			if (targFrameId < 0 || targFrameId >= frameCount)
			{
				Debug.Log("BAD FRAME ID " + targFrameId);
			}
			this.targFrame = this.frames[targFrameId];
			int num = snapFrameId + halfFrameCount;
			if (num >= frameCount)
			{
				num -= frameCount;
			}
			this.frames[num].Clear();
			if (targIsValid)
			{
				switch (this.targFrame.content)
				{
				case FrameContents.Empty:
					if (this.AllowReconstructionOfEmpty)
					{
						this.ReconstructEmptyFrame();
					}
					break;
				case FrameContents.Partial:
					if (this.AllowReconstructionOfPartial)
					{
						this.ReconstructIncompleteFrame();
					}
					break;
				case FrameContents.NoChange:
					this.targFrame.CopyFrom(this.snapFrame);
					break;
				}
			}
			else if (this.AllowReconstructionOfEmpty)
			{
				if (snapIsValid || this.snapFrame.content >= FrameContents.Extrapolated)
				{
					this.ConstructMissingFrame(this.prevFrame, this.snapFrame, this.targFrame);
				}
				else
				{
					this.targFrame.content = FrameContents.Empty;
				}
			}
			else
			{
				this.targFrame.content = FrameContents.Empty;
			}
			if (this._readyState != ReadyStateEnum.Ready)
			{
				TFrame tframe = this.snapFrame;
				if (tframe.content == FrameContents.Complete)
				{
					this.InitialCompleteSnapshot(tframe);
					base.ReadyState = ReadyStateEnum.Ready;
				}
			}
			this.ApplySnapshot(this.snapFrame, this.targFrame, snapIsValid, targIsValid);
			return true;
		}

		// Token: 0x06001007 RID: 4103 RVA: 0x000027C8 File Offset: 0x000009C8
		protected virtual void ApplySnapshot(TFrame snapframe, TFrame targframe, bool snapIsValid, bool targIsValid)
		{
		}

		// Token: 0x17000120 RID: 288
		// (get) Token: 0x06001008 RID: 4104 RVA: 0x000422B1 File Offset: 0x000404B1
		public virtual bool AllowInterpolation
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000121 RID: 289
		// (get) Token: 0x06001009 RID: 4105 RVA: 0x000422B1 File Offset: 0x000404B1
		public virtual bool AllowReconstructionOfEmpty
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000122 RID: 290
		// (get) Token: 0x0600100A RID: 4106 RVA: 0x000422B1 File Offset: 0x000404B1
		public virtual bool AllowReconstructionOfPartial
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600100B RID: 4107 RVA: 0x0004DD2C File Offset: 0x0004BF2C
		protected virtual void ReconstructEmptyFrame()
		{
			if (this.snapFrame.content == FrameContents.Extrapolated || this.snapFrame.content == FrameContents.Complete)
			{
				this.targFrame.content = FrameContents.Extrapolated;
				this.targFrame.CopyFrom(this.snapFrame);
				return;
			}
			this.targFrame.content = FrameContents.Empty;
		}

		// Token: 0x0600100C RID: 4108 RVA: 0x0004DD9D File Offset: 0x0004BF9D
		protected virtual void ReconstructIncompleteFrame()
		{
			this.targFrame.CopyFrom(this.snapFrame);
			this.targFrame.content = FrameContents.Partial;
		}

		// Token: 0x0600100D RID: 4109 RVA: 0x0004DDCC File Offset: 0x0004BFCC
		protected virtual void ConstructMissingFrame(TFrame prevFrame, TFrame snapframe, TFrame targframe)
		{
			if (snapframe.content == FrameContents.Empty)
			{
				targframe.content = FrameContents.Empty;
				return;
			}
			ConnectionTickOffsets connectionTickOffsets;
			if (!TickManager.perConnOffsets.TryGetValue(base.ControllerActorNr, ref connectionTickOffsets))
			{
				Debug.LogError("CONN " + base.ControllerActorNr + " NOT ESTABLISHED IN TICK MANAGER YET.");
			}
			int frameId = targframe.frameId;
			for (int i = 2; i <= 3; i++)
			{
				int num = frameId + i;
				if (num >= TickEngineSettings.frameCount)
				{
					num -= TickEngineSettings.frameCount;
				}
				TFrame tframe = this.frames[num];
				if (this.netObj.frameValidMask[num] && tframe.content == FrameContents.Complete)
				{
					float t = 1f / (float)i;
					this.InterpolateFrame(targframe, snapframe, tframe, t);
					if (targframe.content != FrameContents.Empty)
					{
						return;
					}
				}
			}
			targframe.content = FrameContents.Empty;
			this.ExtrapolateFrame(prevFrame, snapframe, targframe);
		}

		// Token: 0x0600100E RID: 4110 RVA: 0x0004DEBE File Offset: 0x0004C0BE
		public virtual bool OnInterpolate(int snapFrameId, int targFrameId, float t)
		{
			return this.AllowInterpolation && base.isActiveAndEnabled && !base.IsMine;
		}

		// Token: 0x0600100F RID: 4111 RVA: 0x0004DEDF File Offset: 0x0004C0DF
		protected virtual void InterpolateFrame(TFrame targframe, TFrame startframe, TFrame endframe, float t)
		{
			targframe.Clear();
		}

		// Token: 0x06001010 RID: 4112 RVA: 0x0004DEEC File Offset: 0x0004C0EC
		protected virtual void ExtrapolateFrame(TFrame prevframe, TFrame snapframe, TFrame targframe)
		{
			targframe.Clear();
		}

		// Token: 0x04000EFF RID: 3839
		[NonSerialized]
		public TFrame[] frames;

		// Token: 0x04000F00 RID: 3840
		protected TFrame prevFrame;

		// Token: 0x04000F01 RID: 3841
		protected TFrame snapFrame;

		// Token: 0x04000F02 RID: 3842
		protected TFrame targFrame;

		// Token: 0x04000F03 RID: 3843
		protected TFrame offtickFrame;

		// Token: 0x04000F04 RID: 3844
		protected bool hadInitialSnapshot;
	}
}
