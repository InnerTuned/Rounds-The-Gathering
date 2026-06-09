using System;
using System.Collections.Generic;
using Photon.Compression;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x0200029C RID: 668
	public class SyncNodeMover : SyncMoverBase<SyncNodeMover.TRSDefinition, SyncNodeMover.Frame>, IOnPreUpdate, IOnPreSimulate, IOnCaptureState, IOnNetSerialize, IOnSnapshot, IOnInterpolate, IReadyable
	{
		// Token: 0x170000E2 RID: 226
		// (get) Token: 0x06000EAE RID: 3758 RVA: 0x0004731E File Offset: 0x0004551E
		public SyncNodeMover.Node StartNode
		{
			get
			{
				return this.nodes[0];
			}
		}

		// Token: 0x170000E3 RID: 227
		// (get) Token: 0x06000EAF RID: 3759 RVA: 0x0004732C File Offset: 0x0004552C
		public SyncNodeMover.Node EndNode
		{
			get
			{
				return this.nodes[this.nodes.Count - 1];
			}
		}

		// Token: 0x06000EB0 RID: 3760 RVA: 0x00047348 File Offset: 0x00045548
		protected override void Reset()
		{
			base.Reset();
			this.oscillateCurve = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f),
				new Keyframe(0.5f, 1f),
				new Keyframe(1f, 0f)
			});
		}

		// Token: 0x06000EB1 RID: 3761 RVA: 0x000473B0 File Offset: 0x000455B0
		protected override void InitializeTRS(SyncNodeMover.TRSDefinition def, TRS type)
		{
			if (this.movement != SyncNodeMover.Movement.Oscillate)
			{
				def.relation = SyncMoverBase<SyncNodeMover.TRSDefinition, SyncNodeMover.Frame>.MovementRelation.Relative;
			}
			if ((def.relation == SyncMoverBase<SyncNodeMover.TRSDefinition, SyncNodeMover.Frame>.MovementRelation.Relative && this.movement == SyncNodeMover.Movement.Oscillate) || this.movement == SyncNodeMover.Movement.Trigger)
			{
				Vector3 b;
				if (type != TRS.Position)
				{
					if (type != TRS.Rotation)
					{
						b = (def.local ? base.transform.localScale : base.transform.lossyScale);
					}
					else
					{
						b = (def.local ? base.transform.localEulerAngles : base.transform.eulerAngles);
					}
				}
				else
				{
					b = (def.local ? base.transform.localPosition : base.transform.position);
				}
				this.nodes[0].trs[(int)type] += b;
				this.nodes[1].trs[(int)type] += b;
			}
		}

		// Token: 0x06000EB2 RID: 3762 RVA: 0x000474A9 File Offset: 0x000456A9
		public override void OnPreSimulate(int frameId, int subFrameId)
		{
			if (!base.isActiveAndEnabled || (this.photonView && !this.photonView.IsMine))
			{
				return;
			}
			this.OwnerInterpolate();
		}

		// Token: 0x06000EB3 RID: 3763 RVA: 0x000474A9 File Offset: 0x000456A9
		public override void OnPreUpdate()
		{
			if (!base.isActiveAndEnabled || (this.photonView && !this.photonView.IsMine))
			{
				return;
			}
			this.OwnerInterpolate();
		}

		// Token: 0x06000EB4 RID: 3764 RVA: 0x000474D4 File Offset: 0x000456D4
		private void OwnerInterpolate()
		{
			if (this.timeoffset == 0.0)
			{
				this.timeoffset = DoubleTime.fixedTime;
			}
			if (this.movement == SyncNodeMover.Movement.Oscillate)
			{
				this.currentPhase = this.TimeToPhase(DoubleTime.time - this.timeoffset);
				float lerpT = this.OscillatePhaseToLerpT(this.currentPhase);
				this.Oscillate(lerpT);
			}
		}

		// Token: 0x06000EB5 RID: 3765 RVA: 0x00047531 File Offset: 0x00045731
		public void Trigger(int targetNode)
		{
			this.queuedTargetNode = targetNode;
		}

		// Token: 0x06000EB6 RID: 3766 RVA: 0x000027C8 File Offset: 0x000009C8
		public void TriggerMin()
		{
		}

		// Token: 0x06000EB7 RID: 3767 RVA: 0x000027C8 File Offset: 0x000009C8
		public void TriggerMax()
		{
		}

		// Token: 0x06000EB8 RID: 3768 RVA: 0x000027C8 File Offset: 0x000009C8
		private void TriggerLerp()
		{
		}

		// Token: 0x06000EB9 RID: 3769 RVA: 0x0004753C File Offset: 0x0004573C
		private void Oscillate(float lerpT)
		{
			SyncNodeMover.Node node = this.nodes[0];
			SyncNodeMover.Node node2 = this.nodes[1];
			Vector3 pos = (this.posDef.includeAxes == AxisMask.None) ? new Vector3(0f, 0f, 0f) : Vector3.Lerp(node.trs[0], node2.trs[0], lerpT);
			Vector3 rot = (this.rotDef.includeAxes == AxisMask.None) ? new Vector3(0f, 0f, 0f) : Vector3.Lerp(node.trs[1], node2.trs[1], lerpT);
			Vector3 scl = (this.sclDef.includeAxes == AxisMask.None) ? new Vector3(1f, 1f, 1f) : Vector3.Lerp(node.trs[2], node2.trs[2], lerpT);
			this.ApplyOscillate(pos, rot, scl);
		}

		// Token: 0x06000EBA RID: 3770 RVA: 0x00047634 File Offset: 0x00045834
		public void OnCaptureCurrentState(int frameId)
		{
			SyncNodeMover.Frame frame = this.frames[frameId];
			frame.targetNode = this.targetNode;
			frame.phase = this.currentPhase;
			frame.cphase = (uint)this.floatCrusher.Encode(this.currentPhase);
		}

		// Token: 0x06000EBB RID: 3771 RVA: 0x00047670 File Offset: 0x00045870
		public SerializationFlags OnNetSerialize(int frameId, byte[] buffer, ref int bitposition, SerializationFlags writeFlags)
		{
			SyncNodeMover.Frame frame = this.frames[frameId];
			if (this.movement == SyncNodeMover.Movement.Oscillate)
			{
				this.floatCrusher.WriteValue(frame.phase, buffer, ref bitposition);
			}
			return SerializationFlags.HasContent;
		}

		// Token: 0x06000EBC RID: 3772 RVA: 0x000476A4 File Offset: 0x000458A4
		public SerializationFlags OnNetDeserialize(int originFrameId, byte[] buffer, ref int bitposition, FrameArrival arrival)
		{
			SyncNodeMover.Frame frame = this.frames[originFrameId];
			frame.content = FrameContents.Complete;
			if (this.movement == SyncNodeMover.Movement.Oscillate)
			{
				frame.cphase = (uint)this.floatCrusher.ReadCValue(buffer, ref bitposition);
				frame.phase = this.floatCrusher.Decode(frame.cphase);
			}
			return SerializationFlags.HasContent;
		}

		// Token: 0x06000EBD RID: 3773 RVA: 0x000476F8 File Offset: 0x000458F8
		protected override void ApplySnapshot(SyncNodeMover.Frame snapframe, SyncNodeMover.Frame targframe, bool snapIsVaid, bool targIsValid)
		{
			if (snapIsVaid && snapframe.content == FrameContents.Complete && this.movement == SyncNodeMover.Movement.Oscillate)
			{
				if (this.predictWithRTT != 0f)
				{
					float num = (NetMaster.RTT + TickEngineSettings.targetBufferInterval) * this.predictWithRTT;
					float num2 = snapframe.phase * this.oscillatePeriod + num;
					float num3 = targframe.phase * this.oscillatePeriod + num;
					this.snapPhase = this.TimeToPhase((double)num2);
					this.targPhase = this.TimeToPhase((double)num3);
				}
				else
				{
					this.snapPhase = snapframe.phase;
					this.targPhase = targframe.phase;
				}
				float lerpT = this.OscillatePhaseToLerpT(this.snapPhase);
				this.Oscillate(lerpT);
			}
		}

		// Token: 0x06000EBE RID: 3774 RVA: 0x000477B0 File Offset: 0x000459B0
		public override bool OnInterpolate(int snapFrameId, int targFrameId, float t)
		{
			if (!base.OnInterpolate(snapFrameId, targFrameId, t))
			{
				return false;
			}
			SyncNodeMover.Movement movement = this.movement;
			if (movement != SyncNodeMover.Movement.Oscillate)
			{
				if (movement != SyncNodeMover.Movement.Trigger)
				{
				}
				return false;
			}
			if (this.targPhase < this.snapPhase)
			{
				this.targPhase += 1f;
			}
			float num = Mathf.Lerp(this.snapPhase, this.targPhase, t);
			if (num >= 1f)
			{
				num -= 1f;
			}
			float lerpT = this.OscillatePhaseToLerpT(num);
			this.Oscillate(lerpT);
			return true;
		}

		// Token: 0x06000EBF RID: 3775 RVA: 0x00047830 File Offset: 0x00045A30
		protected override void ConstructMissingFrame(SyncNodeMover.Frame prevFrame, SyncNodeMover.Frame snapframe, SyncNodeMover.Frame targframe)
		{
			if (this.movement == SyncNodeMover.Movement.Oscillate && snapframe.content == FrameContents.Complete)
			{
				float num = snapframe.phase * this.oscillatePeriod + TickEngineSettings.netTickInterval;
				targframe.phase = this.TimeToPhase((double)num);
				targframe.content = FrameContents.Complete;
				return;
			}
		}

		// Token: 0x06000EC0 RID: 3776 RVA: 0x0004787A File Offset: 0x00045A7A
		protected float OscillatePhaseToLerpT(float phase)
		{
			return this.oscillateCurve.Evaluate(phase);
		}

		// Token: 0x06000EC1 RID: 3777 RVA: 0x00047888 File Offset: 0x00045A88
		protected float TimeToPhase(double time)
		{
			return (float)(time % (double)this.oscillatePeriod) / this.oscillatePeriod;
		}

		// Token: 0x06000EC2 RID: 3778 RVA: 0x0004789C File Offset: 0x00045A9C
		private void ApplyOscillate(Vector3 pos, Vector3 rot, Vector3 scl)
		{
			AxisMask includeAxes = this.posDef.includeAxes;
			AxisMask includeAxes2 = this.rotDef.includeAxes;
			AxisMask includeAxes3 = this.sclDef.includeAxes;
			if (includeAxes3 != AxisMask.None)
			{
				base.transform.localScale = new Vector3(((includeAxes3 & AxisMask.X) != AxisMask.None) ? scl.x : base.transform.localScale.x, ((includeAxes3 & AxisMask.Y) != AxisMask.None) ? scl.y : base.transform.localScale.y, ((includeAxes3 & AxisMask.Z) != AxisMask.None) ? scl.z : base.transform.localScale.z);
			}
			if (includeAxes2 != AxisMask.None)
			{
				if (this.rotDef.local)
				{
					base.transform.localEulerAngles = new Vector3(((includeAxes2 & AxisMask.X) != AxisMask.None) ? rot.x : base.transform.localEulerAngles.x, ((includeAxes2 & AxisMask.Y) != AxisMask.None) ? rot.y : base.transform.localEulerAngles.y, ((includeAxes2 & AxisMask.Z) != AxisMask.None) ? rot.z : base.transform.localEulerAngles.z);
				}
				else
				{
					base.transform.eulerAngles = new Vector3(((includeAxes2 & AxisMask.X) != AxisMask.None) ? rot.x : base.transform.eulerAngles.x, ((includeAxes2 & AxisMask.Y) != AxisMask.None) ? rot.y : base.transform.eulerAngles.y, ((includeAxes2 & AxisMask.Z) != AxisMask.None) ? rot.z : base.transform.eulerAngles.z);
				}
			}
			if (includeAxes != AxisMask.None)
			{
				if (this.posDef.local)
				{
					base.transform.localPosition = new Vector3(((includeAxes & AxisMask.X) != AxisMask.None) ? pos.x : base.transform.localPosition.x, ((includeAxes & AxisMask.Y) != AxisMask.None) ? pos.y : base.transform.localPosition.y, ((includeAxes & AxisMask.Z) != AxisMask.None) ? pos.z : base.transform.localPosition.z);
					return;
				}
				base.transform.position = new Vector3(((includeAxes & AxisMask.X) != AxisMask.None) ? pos.x : base.transform.position.x, ((includeAxes & AxisMask.Y) != AxisMask.None) ? pos.y : base.transform.position.y, ((includeAxes & AxisMask.Z) != AxisMask.None) ? pos.z : base.transform.position.z);
			}
		}

		// Token: 0x06000EC3 RID: 3779 RVA: 0x00047AFC File Offset: 0x00045CFC
		public SyncNodeMover()
		{
			List<SyncNodeMover.Node> list = new List<SyncNodeMover.Node>();
			list.Add(new SyncNodeMover.Node());
			list.Add(new SyncNodeMover.Node());
			this.nodes = list;
			this.oscillatePeriod = 1f;
			this.oscillateCurve = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f),
				new Keyframe(0.5f, 1f),
				new Keyframe(1f, 0f)
			});
			this.floatCrusher = new LiteFloatCrusher(LiteFloatCompressType.Bits10, LiteFloatCrusher.Normalization.Positive, LiteOutOfBoundsHandling.Clamp);
			base..ctor();
		}

		// Token: 0x04000DB8 RID: 3512
		[Range(0f, 2f)]
		public float predictWithRTT = 1f;

		// Token: 0x04000DB9 RID: 3513
		[HideInInspector]
		public List<SyncNodeMover.Node> nodes;

		// Token: 0x04000DBA RID: 3514
		[HideInInspector]
		public SyncNodeMover.Movement movement;

		// Token: 0x04000DBB RID: 3515
		[HideInInspector]
		public float oscillatePeriod;

		// Token: 0x04000DBC RID: 3516
		[HideInInspector]
		public AnimationCurve oscillateCurve;

		// Token: 0x04000DBD RID: 3517
		[HideInInspector]
		public LiteFloatCrusher floatCrusher;

		// Token: 0x04000DBE RID: 3518
		protected float currentPhase;

		// Token: 0x04000DBF RID: 3519
		protected int queuedTargetNode;

		// Token: 0x04000DC0 RID: 3520
		protected int targetNode;

		// Token: 0x04000DC1 RID: 3521
		protected double timeoffset;

		// Token: 0x04000DC2 RID: 3522
		protected float snapPhase;

		// Token: 0x04000DC3 RID: 3523
		protected float targPhase;

		// Token: 0x04000DC4 RID: 3524
		protected float accumulatedTime;

		// Token: 0x020003D0 RID: 976
		public enum Movement
		{
			// Token: 0x040012FF RID: 4863
			Oscillate,
			// Token: 0x04001300 RID: 4864
			Trigger
		}

		// Token: 0x020003D1 RID: 977
		[Serializable]
		public class Node
		{
			// Token: 0x170001FB RID: 507
			// (get) Token: 0x060013DD RID: 5085 RVA: 0x0005A683 File Offset: 0x00058883
			// (set) Token: 0x060013DE RID: 5086 RVA: 0x0005A691 File Offset: 0x00058891
			public Vector3 Pos
			{
				get
				{
					return this.trs[0];
				}
				set
				{
					this.trs[0] = value;
				}
			}

			// Token: 0x170001FC RID: 508
			// (get) Token: 0x060013DF RID: 5087 RVA: 0x0005A6A0 File Offset: 0x000588A0
			// (set) Token: 0x060013E0 RID: 5088 RVA: 0x0005A6AE File Offset: 0x000588AE
			public Vector3 Rot
			{
				get
				{
					return this.trs[1];
				}
				set
				{
					this.trs[1] = value;
				}
			}

			// Token: 0x170001FD RID: 509
			// (get) Token: 0x060013E1 RID: 5089 RVA: 0x0005A6BD File Offset: 0x000588BD
			// (set) Token: 0x060013E2 RID: 5090 RVA: 0x0005A6CB File Offset: 0x000588CB
			public Vector3 Scl
			{
				get
				{
					return this.trs[2];
				}
				set
				{
					this.trs[2] = value;
				}
			}

			// Token: 0x04001301 RID: 4865
			public Vector3[] trs = new Vector3[]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(1f, 1f, 1f)
			};
		}

		// Token: 0x020003D2 RID: 978
		[Serializable]
		public class TRSDefinition : TRSDefinitionBase
		{
			// Token: 0x04001302 RID: 4866
			public AxisMask includeAxes = AxisMask.XYZ;

			// Token: 0x04001303 RID: 4867
			public SyncMoverBase<SyncNodeMover.TRSDefinition, SyncNodeMover.Frame>.MovementRelation relation = SyncMoverBase<SyncNodeMover.TRSDefinition, SyncNodeMover.Frame>.MovementRelation.Relative;
		}

		// Token: 0x020003D3 RID: 979
		public class Frame : FrameBase
		{
			// Token: 0x060013E5 RID: 5093 RVA: 0x0005A3E9 File Offset: 0x000585E9
			public Frame()
			{
			}

			// Token: 0x060013E6 RID: 5094 RVA: 0x0005A3F1 File Offset: 0x000585F1
			public Frame(int frameId) : base(frameId)
			{
			}

			// Token: 0x060013E7 RID: 5095 RVA: 0x0005A764 File Offset: 0x00058964
			public override void CopyFrom(FrameBase sourceFrame)
			{
				base.CopyFrom(sourceFrame);
				SyncNodeMover.Frame frame = sourceFrame as SyncNodeMover.Frame;
				this.targetNode = frame.targetNode;
				this.phase = frame.phase;
				this.cphase = frame.cphase;
			}

			// Token: 0x060013E8 RID: 5096 RVA: 0x0005A7A3 File Offset: 0x000589A3
			public override void Clear()
			{
				base.Clear();
				this.targetNode = -1;
				this.phase = -1f;
				this.cphase = 0U;
			}

			// Token: 0x060013E9 RID: 5097 RVA: 0x0005A7C4 File Offset: 0x000589C4
			public bool Compare(SyncNodeMover.Frame otherFrame)
			{
				return this.targetNode == otherFrame.targetNode && this.phase == otherFrame.phase && this.cphase == otherFrame.cphase;
			}

			// Token: 0x04001304 RID: 4868
			public int targetNode;

			// Token: 0x04001305 RID: 4869
			public float phase;

			// Token: 0x04001306 RID: 4870
			public uint cphase;
		}
	}
}
