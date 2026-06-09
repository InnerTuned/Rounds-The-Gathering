using System;
using System.Collections.Generic;
using emotitron.Compression;
using Photon.Compression;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x020002B2 RID: 690
	[DisallowMultipleComponent]
	public class SyncTransform : SyncObject<SyncTransform.Frame>, ISyncTransform, IOnSnapshot, IOnNetSerialize, IOnAuthorityChanged, IReadyable, IUseKeyframes, IDeltaFrameChangeDetect, IOnInterpolate, IOnCaptureState, IFlagTeleport
	{
		// Token: 0x06000F2F RID: 3887 RVA: 0x00049A86 File Offset: 0x00047C86
		public void FlagTeleport()
		{
			if (base.IsMine)
			{
				if (!this.hasTeleported)
				{
					this.CaptureCurrent(this.preTeleportM, this.preTeleportCM, false);
				}
				this.hasTeleported = true;
			}
		}

		// Token: 0x06000F30 RID: 3888 RVA: 0x00049AB2 File Offset: 0x00047CB2
		public void UpdateParent(ObjState state, Transform newParent)
		{
			this.teleNewParentId = (newParent ? newParent.GetInstanceID() : (((state & ObjState.Mounted) != ObjState.Despawned) ? -2 : -1));
		}

		// Token: 0x170000FC RID: 252
		// (get) Token: 0x06000F31 RID: 3889 RVA: 0x00049AD4 File Offset: 0x00047CD4
		public override bool AllowInterpolation
		{
			get
			{
				return this.allowInterpolation;
			}
		}

		// Token: 0x170000FD RID: 253
		// (get) Token: 0x06000F32 RID: 3890 RVA: 0x00049ADC File Offset: 0x00047CDC
		public override bool AllowReconstructionOfEmpty
		{
			get
			{
				return this.allowReconstructionOfEmpty;
			}
		}

		// Token: 0x170000FE RID: 254
		// (get) Token: 0x06000F33 RID: 3891 RVA: 0x00049AE4 File Offset: 0x00047CE4
		public override int ApplyOrder
		{
			get
			{
				return 9;
			}
		}

		// Token: 0x06000F34 RID: 3892 RVA: 0x00049AE8 File Offset: 0x00047CE8
		public override void OnAwake()
		{
			base.OnAwake();
			this.rb = base.GetComponent<Rigidbody>();
			this.rb2d = base.GetComponent<Rigidbody2D>();
			base.GetComponents<ITransformController>(this.iTransformControllers);
			this.teleportThresholdSqrMag = ((this.teleportThreshold <= 0f) ? 0f : (this.teleportThreshold * this.teleportThreshold));
			this.ConnectSharedCaches();
			this.allowInterpolation = true;
			this.allowReconstructionOfEmpty = true;
			for (int i = 0; i < this.iTransformControllers.Count; i++)
			{
				ITransformController transformController = this.iTransformControllers[i];
				this.allowInterpolation &= !transformController.HandlesInterpolation;
				this.allowReconstructionOfEmpty &= !transformController.HandlesExtrapolation;
			}
		}

		// Token: 0x06000F35 RID: 3893 RVA: 0x00049BAC File Offset: 0x00047DAC
		private void ConnectSharedCaches()
		{
			if (this.masterSharedCrushers.ContainsKey(this.prefabInstanceId))
			{
				this.transformCrusher = this.masterSharedCrushers[this.prefabInstanceId];
				return;
			}
			this.masterSharedCrushers.Add(this.prefabInstanceId, this.transformCrusher);
		}

		// Token: 0x06000F36 RID: 3894 RVA: 0x00049BFB File Offset: 0x00047DFB
		private void OnDestroy()
		{
			SyncTransform.framePool.Push(this.frames);
		}

		// Token: 0x06000F37 RID: 3895 RVA: 0x00049C10 File Offset: 0x00047E10
		protected override void PopulateFrames()
		{
			int frameCount = TickEngineSettings.frameCount;
			if (SyncTransform.framePool.Count == 0)
			{
				this.frames = new SyncTransform.Frame[frameCount + 1];
				this.frames[frameCount] = new SyncTransform.Frame(this, frameCount);
				for (int i = 0; i <= frameCount; i++)
				{
					this.frames[i] = new SyncTransform.Frame(this.frames[frameCount], i);
				}
				return;
			}
			this.frames = SyncTransform.framePool.Pop();
			this.frames[frameCount].Set(this, frameCount);
			for (int j = 0; j < frameCount; j++)
			{
				this.frames[j].CopyFrom(this.frames[frameCount]);
			}
		}

		// Token: 0x06000F38 RID: 3896 RVA: 0x00049CB0 File Offset: 0x00047EB0
		protected void CaptureCurrent(Matrix m, CompressedMatrix cm, bool forceUseTransform = false)
		{
			if (forceUseTransform)
			{
				this.transformCrusher.Capture(base.transform, cm, m);
				return;
			}
			if (this.rb)
			{
				this.transformCrusher.Capture(this.rb, cm, m);
				return;
			}
			if (this.rb2d)
			{
				this.transformCrusher.Capture(this.rb2d, cm, m);
				return;
			}
			this.transformCrusher.Capture(base.transform, cm, m);
		}

		// Token: 0x06000F39 RID: 3897 RVA: 0x00049D2C File Offset: 0x00047F2C
		public virtual void OnCaptureCurrentState(int frameId)
		{
			SyncTransform.Frame frame = this.frames[frameId];
			frame.hasTeleported = this.hasTeleported;
			if (this.hasTeleported)
			{
				frame.cm.CopyFrom(this.preTeleportCM);
				frame.m.CopyFrom(this.preTeleportM);
				this.CaptureCurrent(frame.telem, frame.telecm, true);
				this.transformCrusher.Apply(base.transform, frame.telem);
				this.hasTeleported = false;
				return;
			}
			this.CaptureCurrent(frame.m, frame.cm, false);
		}

		// Token: 0x06000F3A RID: 3898 RVA: 0x00049DC0 File Offset: 0x00047FC0
		public SerializationFlags OnNetSerialize(int frameId, byte[] buffer, ref int bitposition, SerializationFlags writeFlags)
		{
			SyncTransform.Frame frame = this.frames[frameId];
			bool flag = frame.hasTeleported;
			bool flag2 = flag || (writeFlags & SerializationFlags.NewConnection) > SerializationFlags.None;
			if (!flag2 && !base.isActiveAndEnabled)
			{
				ArraySerializeExt.WriteBool(buffer, false, ref bitposition);
				return SerializationFlags.None;
			}
			bool flag3 = base.IsKeyframe(frameId);
			if (!flag2 && !flag3 && (!this.useDeltas || (this.prevSentFrame != null && frame.cm.Equals(this.prevSentFrame.cm))))
			{
				ArraySerializeExt.WriteBool(buffer, false, ref bitposition);
				this.prevSentFrame = frame;
				return SerializationFlags.None;
			}
			SerializationFlags serializationFlags = SerializationFlags.HasContent;
			ArraySerializeExt.WriteBool(buffer, true, ref bitposition);
			ArraySerializeExt.WriteBool(buffer, flag, ref bitposition);
			if (flag)
			{
				this.transformCrusher.Write(frame.telecm, buffer, ref bitposition, 0);
				if (this.teleportReliable)
				{
					serializationFlags |= SerializationFlags.ForceReliable;
				}
			}
			this.transformCrusher.Write(frame.cm, buffer, ref bitposition, 0);
			this.transformCrusher.Decompress(frame.m, frame.cm);
			this.prevSentFrame = frame;
			return serializationFlags;
		}

		// Token: 0x06000F3B RID: 3899 RVA: 0x00049EC0 File Offset: 0x000480C0
		public SerializationFlags OnNetDeserialize(int originFrameId, byte[] buffer, ref int bitposition, FrameArrival arrival)
		{
			SyncTransform.Frame frame = this.photonView.IsMine ? this.offtickFrame : this.frames[originFrameId];
			if (!ArraySerializeExt.ReadBool(buffer, ref bitposition))
			{
				frame.content = FrameContents.Empty;
				return SerializationFlags.None;
			}
			frame.content = FrameContents.Complete;
			bool flag = ArraySerializeExt.ReadBool(buffer, ref bitposition);
			frame.hasTeleported = flag;
			if (flag)
			{
				this.transformCrusher.Read(frame.telecm, buffer, ref bitposition, 0);
				this.transformCrusher.Decompress(frame.telem, frame.telecm);
			}
			this.transformCrusher.Read(frame.cm, buffer, ref bitposition, 0);
			this.transformCrusher.Decompress(frame.m, frame.cm);
			return SerializationFlags.HasContent;
		}

		// Token: 0x06000F3C RID: 3900 RVA: 0x00049F70 File Offset: 0x00048170
		public override bool OnSnapshot(int prevFrameId, int snapFrameId, int targFrameId, bool prevIsValid, bool snapIsValid, bool targIsValid)
		{
			if (!base.OnSnapshot(prevFrameId, snapFrameId, targFrameId, prevIsValid, snapIsValid, targIsValid))
			{
				return false;
			}
			if (this.snapFrame.content == FrameContents.Empty)
			{
				return false;
			}
			if (this.targFrame.content == FrameContents.Empty)
			{
				return false;
			}
			bool flag = this.snapFrame.hasTeleported;
			this.targFrame.parentHash = this.teleNewParentId;
			this.skipInterpolation = false;
			if (!flag && this.teleportThresholdSqrMag > 0f)
			{
				Vector3 position = this.targFrame.m.position;
				Vector3 b = flag ? this.snapFrame.telem.position : this.snapFrame.m.position;
				if (Vector3.SqrMagnitude(position - b) > this.teleportThresholdSqrMag)
				{
					this.skipInterpolation = true;
				}
			}
			this.ApplyFrame(this.snapFrame);
			return true;
		}

		// Token: 0x06000F3D RID: 3901 RVA: 0x0004A03F File Offset: 0x0004823F
		protected void ApplyFrame(SyncTransform.Frame frame)
		{
			this.transformCrusher.Apply(base.transform, frame.hasTeleported ? frame.telem : frame.m);
		}

		// Token: 0x06000F3E RID: 3902 RVA: 0x0004A068 File Offset: 0x00048268
		public override bool OnInterpolate(int snapFrameId, int targFrameId, float t)
		{
			if (this.skipInterpolation)
			{
				return false;
			}
			if (!base.OnInterpolate(snapFrameId, targFrameId, t))
			{
				return false;
			}
			if (this.interpolation == Interpolation.None)
			{
				return false;
			}
			if (this.targFrame == null)
			{
				return false;
			}
			if (this.snapFrame.content == FrameContents.Empty)
			{
				return false;
			}
			if (this.targFrame.content == FrameContents.Empty)
			{
				return false;
			}
			if (this.snapFrame.parentHash != this.targFrame.parentHash)
			{
				return false;
			}
			Matrix start = this.snapFrame.hasTeleported ? this.snapFrame.telem : this.snapFrame.m;
			if (this.interpolation == Interpolation.Linear)
			{
				Matrix.Lerp(Matrix.reusable, start, this.targFrame.m, t);
			}
			else
			{
				Matrix.CatmullRomLerpUnclamped(Matrix.reusable, this.prevFrame.m, this.snapFrame.m, this.targFrame.m, t);
			}
			this.transformCrusher.Apply(base.transform, Matrix.reusable);
			return true;
		}

		// Token: 0x06000F3F RID: 3903 RVA: 0x0004A168 File Offset: 0x00048368
		protected override void InterpolateFrame(SyncTransform.Frame targframe, SyncTransform.Frame startframe, SyncTransform.Frame endframe, float t)
		{
			if (startframe.parentHash == -2 || startframe.parentHash != endframe.parentHash)
			{
				this.targFrame.content = FrameContents.Empty;
				return;
			}
			targframe.CopyFrom(endframe);
			Matrix.Lerp(targframe.m, startframe.hasTeleported ? startframe.telem : startframe.m, endframe.m, t);
			this.transformCrusher.Compress(targframe.cm, targframe.m);
		}

		// Token: 0x06000F40 RID: 3904 RVA: 0x0004A1E4 File Offset: 0x000483E4
		protected override void ExtrapolateFrame(SyncTransform.Frame prevframe, SyncTransform.Frame snapframe, SyncTransform.Frame targframe)
		{
			if (snapframe.content == FrameContents.Empty)
			{
				global::Debug.LogError(targframe.frameId + " Failed to extrapolate due to empty snapshot. Failsafing to current transform value.");
				targframe.content = FrameContents.Empty;
				return;
			}
			targframe.CopyFrom(snapframe);
			targframe.parentHash = this.teleNewParentId;
			if (snapframe.hasTeleported)
			{
				return;
			}
			FrameContents content = prevframe.content;
			if (content == FrameContents.Empty)
			{
				return;
			}
			if (content == FrameContents.Complete)
			{
				int parentHash = prevframe.parentHash;
				if (parentHash == -2 || prevframe.hasTeleported || parentHash != snapframe.parentHash)
				{
					return;
				}
			}
			Matrix.LerpUnclamped(targframe.m, prevframe.hasTeleported ? prevframe.telem : prevframe.m, snapframe.m, 1f + this.extrapolateRatio);
			this.transformCrusher.Compress(targframe.cm, targframe.m);
		}

		// Token: 0x04000E2F RID: 3631
		[Tooltip("How lerping between tick states is achieved. 'Standard' is Linear. 'None' holds the previous state until t = 1. 'Catmull Rom' is experimental.")]
		public Interpolation interpolation = Interpolation.Linear;

		// Token: 0x04000E30 RID: 3632
		[Tooltip("Percentage of extrapolation from previous values. [0 = No Extrapolation] [.5 = 50% extrapolation] [1 = Undampened]. This allows for gradual slowing down of motion when the buffer runs dry.")]
		[Range(0f, 1f)]
		public float extrapolateRatio = 0.5f;

		// Token: 0x04000E31 RID: 3633
		protected int extrapolationCount;

		// Token: 0x04000E32 RID: 3634
		[Tooltip("If the distance delta between snapshots exceeds this amount, object will move to new location without lerping. Set this to zero or less to disable (for some tiny CPU savings). You can manually flag a teleport by setting the HasTeleported property to True.")]
		public float teleportThreshold = 5f;

		// Token: 0x04000E33 RID: 3635
		private float teleportThresholdSqrMag;

		// Token: 0x04000E34 RID: 3636
		[Tooltip("Entire tick update from this client (all objects being serialized) will be sent as Reliable when FlagTeleport() has been called.")]
		public bool teleportReliable;

		// Token: 0x04000E35 RID: 3637
		public Dictionary<int, TransformCrusher> masterSharedCrushers = new Dictionary<int, TransformCrusher>();

		// Token: 0x04000E36 RID: 3638
		public TransformCrusher transformCrusher = new TransformCrusher
		{
			PosCrusher = new ElementCrusher(0, false)
			{
				hideFieldName = true,
				XCrusher = new FloatCrusher(0, 0, true)
				{
					BitsDeterminedBy = -4,
					AccurateCenter = true
				},
				YCrusher = new FloatCrusher(1, 0, true)
				{
					BitsDeterminedBy = -4,
					AccurateCenter = true
				},
				ZCrusher = new FloatCrusher(2, 0, true)
				{
					BitsDeterminedBy = -4,
					AccurateCenter = true
				}
			},
			RotCrusher = new ElementCrusher(2, false)
			{
				hideFieldName = true,
				XCrusher = new FloatCrusher(0, 1, true)
				{
					Bits = 12,
					AccurateCenter = true
				},
				YCrusher = new FloatCrusher(1, 1, true)
				{
					Bits = 12,
					AccurateCenter = true
				},
				ZCrusher = new FloatCrusher(2, 1, true)
				{
					Bits = 12,
					AccurateCenter = true
				},
				QCrusher = new QuatCrusher(44, true, false)
			},
			SclCrusher = new ElementCrusher(3, false)
			{
				hideFieldName = true,
				uniformAxes = ElementCrusher.UniformAxes.NonUniform,
				XCrusher = new FloatCrusher(8, -1f, 1f, 0, 3, true)
				{
					TRSType = 3,
					AccurateCenter = true,
					BitsDeterminedBy = -1
				},
				YCrusher = new FloatCrusher(8, -1f, 1f, 1, 3, true)
				{
					TRSType = 3,
					AccurateCenter = true,
					BitsDeterminedBy = -1,
					Enabled = false
				},
				ZCrusher = new FloatCrusher(8, -1f, 1f, 2, 3, true)
				{
					TRSType = 3,
					AccurateCenter = true,
					BitsDeterminedBy = -1,
					Enabled = false
				}
			}
		};

		// Token: 0x04000E37 RID: 3639
		protected bool hasTeleported;

		// Token: 0x04000E38 RID: 3640
		protected int teleNewParentId;

		// Token: 0x04000E39 RID: 3641
		protected Matrix preTeleportM = new Matrix();

		// Token: 0x04000E3A RID: 3642
		protected CompressedMatrix preTeleportCM = new CompressedMatrix();

		// Token: 0x04000E3B RID: 3643
		private Rigidbody rb;

		// Token: 0x04000E3C RID: 3644
		private Rigidbody2D rb2d;

		// Token: 0x04000E3D RID: 3645
		private List<ITransformController> iTransformControllers = new List<ITransformController>(1);

		// Token: 0x04000E3E RID: 3646
		protected bool allowInterpolation;

		// Token: 0x04000E3F RID: 3647
		protected bool allowReconstructionOfEmpty;

		// Token: 0x04000E40 RID: 3648
		public static Stack<SyncTransform.Frame[]> framePool = new Stack<SyncTransform.Frame[]>();

		// Token: 0x04000E41 RID: 3649
		public SyncTransform.Frame prevSentFrame;

		// Token: 0x04000E42 RID: 3650
		protected bool skipInterpolation;

		// Token: 0x020003D7 RID: 983
		public class Frame : FrameBase
		{
			// Token: 0x060013F6 RID: 5110 RVA: 0x0005A978 File Offset: 0x00058B78
			public Frame()
			{
				this.m = new Matrix();
				this.cm = new CompressedMatrix();
				this.telem = new Matrix();
				this.telecm = new CompressedMatrix();
				this.parentHash = -2;
			}

			// Token: 0x060013F7 RID: 5111 RVA: 0x0005A9B4 File Offset: 0x00058BB4
			public Frame(SyncTransform sst, int frameId) : base(frameId)
			{
				this.m = new Matrix();
				this.cm = new CompressedMatrix();
				this.telem = new Matrix();
				this.telecm = new CompressedMatrix();
				sst.transformCrusher.Capture(sst.transform, this.cm, this.m);
				Transform parent = sst.transform.parent;
				this.parentHash = (parent ? parent.GetInstanceID() : -1);
			}

			// Token: 0x060013F8 RID: 5112 RVA: 0x0005AA34 File Offset: 0x00058C34
			public Frame(SyncTransform.Frame srcFrame, int frameId) : base(frameId)
			{
				this.m = new Matrix();
				this.cm = new CompressedMatrix();
				this.telem = new Matrix();
				this.telecm = new CompressedMatrix();
				this.CopyFrom(srcFrame);
			}

			// Token: 0x060013F9 RID: 5113 RVA: 0x0005AA70 File Offset: 0x00058C70
			public void Set(SyncTransform sst, int frameId)
			{
				sst.transformCrusher.Capture(sst.transform, this.cm, this.m);
			}

			// Token: 0x060013FA RID: 5114 RVA: 0x0005AA90 File Offset: 0x00058C90
			public override void CopyFrom(FrameBase sourceFrame)
			{
				base.CopyFrom(sourceFrame);
				SyncTransform.Frame frame = sourceFrame as SyncTransform.Frame;
				if (frame.hasTeleported)
				{
					this.m.CopyFrom(frame.telem);
					this.cm.CopyFrom(frame.telecm);
				}
				else
				{
					this.m.CopyFrom(frame.m);
					this.cm.CopyFrom(frame.cm);
				}
				this.hasTeleported = false;
				this.parentHash = frame.parentHash;
			}

			// Token: 0x060013FB RID: 5115 RVA: 0x0005AB0C File Offset: 0x00058D0C
			public bool FastCompareCompressed(SyncTransform.Frame other)
			{
				return this.cm.Equals(other.cm);
			}

			// Token: 0x060013FC RID: 5116 RVA: 0x0005AB24 File Offset: 0x00058D24
			public bool FastCompareUncompressed(SyncTransform.Frame other)
			{
				return this.m.position == other.m.position && this.m.rotation == other.m.rotation && this.m.scale == other.m.scale;
			}

			// Token: 0x060013FD RID: 5117 RVA: 0x0005AB88 File Offset: 0x00058D88
			public override void Clear()
			{
				base.Clear();
				this.hasTeleported = false;
				this.parentHash = -2;
			}

			// Token: 0x060013FE RID: 5118 RVA: 0x0005ABA0 File Offset: 0x00058DA0
			public override string ToString()
			{
				return string.Concat(new object[]
				{
					"[",
					this.frameId,
					" ",
					this.m.position,
					" / ",
					this.m.rotation,
					"]"
				});
			}

			// Token: 0x0400130E RID: 4878
			public bool hasTeleported;

			// Token: 0x0400130F RID: 4879
			public Matrix m;

			// Token: 0x04001310 RID: 4880
			public CompressedMatrix cm;

			// Token: 0x04001311 RID: 4881
			public SyncTransform owner;

			// Token: 0x04001312 RID: 4882
			public Matrix telem;

			// Token: 0x04001313 RID: 4883
			public CompressedMatrix telecm;

			// Token: 0x04001314 RID: 4884
			public int parentHash;

			// Token: 0x04001315 RID: 4885
			public int telePparentHash;
		}
	}
}
