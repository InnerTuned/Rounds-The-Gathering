using System;
using System.Collections.Generic;
using emotitron.Compression;
using Photon.Compression;
using Photon.Pun.Simple.Internal;
using Photon.Utilities;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x02000298 RID: 664
	public class SyncAnimator : SyncObject<SyncAnimator.Frame>, IOnSnapshot, IOnNetSerialize, IOnAuthorityChanged, ISyncAnimator, IReadyable, IUseKeyframes, IOnInterpolate, IOnCaptureState
	{
		// Token: 0x170000DC RID: 220
		// (get) Token: 0x06000E63 RID: 3683 RVA: 0x00044FCC File Offset: 0x000431CC
		public override int ApplyOrder
		{
			get
			{
				return 11;
			}
		}

		// Token: 0x170000DD RID: 221
		// (get) Token: 0x06000E64 RID: 3684 RVA: 0x000429F6 File Offset: 0x00040BF6
		public override bool AllowReconstructionOfEmpty
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06000E65 RID: 3685 RVA: 0x00044FD0 File Offset: 0x000431D0
		protected override void PopulateFrames()
		{
			this.Initialize();
			int frameCount = TickEngineSettings.frameCount;
			Stack<SyncAnimator.Frame[]> stack;
			if (!SyncAnimator.masterSharedFramePool.TryGetValue(this.prefabInstanceId, ref stack))
			{
				stack = new Stack<SyncAnimator.Frame[]>();
				SyncAnimator.masterSharedFramePool.Add(this.prefabInstanceId, stack);
			}
			if (stack.Count == 0)
			{
				this.frames = new SyncAnimator.Frame[frameCount + 1];
				for (int i = 0; i <= frameCount; i++)
				{
					this.frames[i] = new SyncAnimator.Frame(this, i);
				}
				return;
			}
			this.frames = stack.Pop();
			for (int j = 0; j <= frameCount; j++)
			{
				this.frames[j].Clear();
			}
		}

		// Token: 0x06000E66 RID: 3686 RVA: 0x0004506B File Offset: 0x0004326B
		public override void OnAwake()
		{
			if (this.animator == null)
			{
				this.FindUnsyncedAnimator();
			}
			base.OnAwake();
			this.ConnectSharedCaches();
			if (this.animator)
			{
				this.defaultRootMotion = this.animator.applyRootMotion;
			}
		}

		// Token: 0x06000E67 RID: 3687 RVA: 0x000450AB File Offset: 0x000432AB
		public override void OnJoinedRoom()
		{
			base.OnJoinedRoom();
			this.AutoRootMotion(base.IsMine);
		}

		// Token: 0x06000E68 RID: 3688 RVA: 0x000450BF File Offset: 0x000432BF
		public override void OnStart()
		{
			base.OnStart();
			this.AutoRootMotion(base.IsMine);
		}

		// Token: 0x06000E69 RID: 3689 RVA: 0x000450D3 File Offset: 0x000432D3
		public override void OnAuthorityChanged(bool isMine, bool controllerChanged)
		{
			base.OnAuthorityChanged(isMine, controllerChanged);
			this.AutoRootMotion(isMine);
		}

		// Token: 0x06000E6A RID: 3690 RVA: 0x000450E4 File Offset: 0x000432E4
		private void AutoRootMotion(bool isMine)
		{
			if (this.autoRootMotion && this.animator)
			{
				this.animator.applyRootMotion = (isMine && this.defaultRootMotion);
			}
		}

		// Token: 0x06000E6B RID: 3691 RVA: 0x00045114 File Offset: 0x00043314
		private void FindUnsyncedAnimator()
		{
			NestedComponentUtilities.GetNestedComponentsInChildren<Animator, NetObject>(base.transform, SyncAnimator.foundAnimators, true);
			NestedComponentUtilities.GetNestedComponentsInChildren<SyncAnimator, NetObject>(base.transform, SyncAnimator.foundSyncs, true);
			foreach (Animator y in SyncAnimator.foundAnimators)
			{
				bool flag = false;
				foreach (SyncAnimator syncAnimator in SyncAnimator.foundSyncs)
				{
					flag = false;
					if (syncAnimator.animator == y)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					this.animator = y;
					break;
				}
			}
		}

		// Token: 0x06000E6C RID: 3692 RVA: 0x000451E0 File Offset: 0x000433E0
		private void Initialize()
		{
			this.bitsForTriggerIndex = (this.sharedTriggIndexes.Count - 1).GetBitsForMaxValue();
			this.bitsForStateIndex = (this.sharedStateIndexes.Count - 1).GetBitsForMaxValue();
			this.paramCount = this.animator.parameters.Length;
			this.layerCount = this.animator.layerCount;
			this.bitsForLayerIndex = this.layerCount.GetBitsForMaxValue();
			this.lastSentParams = new SmartVar[this.paramCount];
			ParameterSettings.RebuildParamSettings(this.animator, ref this.sharedParamSettings, ref this.paramCount, this.sharedParamDefaults);
			this.lastAnimationHash = new int[this.layerCount];
			this.lastLayerWeight = new uint[this.layerCount];
			for (int i = 0; i < this.paramCount; i++)
			{
				this.lastSentParams[i] = this.sharedParamSettings[i].defaultValue;
			}
		}

		// Token: 0x06000E6D RID: 3693 RVA: 0x000452D0 File Offset: 0x000434D0
		private void ConnectSharedCaches()
		{
			if (!SyncAnimator.masterSharedTriggHashes.ContainsKey(this.prefabInstanceId))
			{
				this.sharedTriggHashes = new Dictionary<int, int>();
				for (int i = 0; i < this.sharedTriggIndexes.Count; i++)
				{
					if (this.sharedTriggHashes.ContainsKey(this.sharedTriggIndexes[i]))
					{
						global::Debug.LogError(string.Concat(new string[]
						{
							"There appear to be duplicate Trigger names in the animator controller on '",
							base.name,
							"'. This will break ",
							base.GetType().Name,
							"'s ability to sync triggers."
						}));
					}
					else
					{
						this.sharedTriggHashes.Add(this.sharedTriggIndexes[i], i);
					}
				}
				SyncAnimator.masterSharedTriggHashes.Add(this.prefabInstanceId, this.sharedTriggHashes);
				SyncAnimator.masterSharedTriggIndexes.Add(this.prefabInstanceId, this.sharedTriggIndexes);
			}
			else
			{
				this.sharedTriggHashes = SyncAnimator.masterSharedTriggHashes[this.prefabInstanceId];
				this.sharedTriggIndexes = SyncAnimator.masterSharedTriggIndexes[this.prefabInstanceId];
			}
			if (!SyncAnimator.masterSharedStateHashes.ContainsKey(this.prefabInstanceId))
			{
				this.sharedStateHashes = new Dictionary<int, int>();
				for (int j = 0; j < this.sharedStateIndexes.Count; j++)
				{
					if (this.sharedStateHashes.ContainsKey(this.sharedStateIndexes[j]))
					{
						global::Debug.LogError(string.Concat(new string[]
						{
							"There appear to be duplicate State names in the animator controller on '",
							base.name,
							"'. This will break ",
							base.GetType().Name,
							"'s ability to sync states."
						}));
					}
					else
					{
						this.sharedStateHashes.Add(this.sharedStateIndexes[j], j);
					}
				}
				SyncAnimator.masterSharedStateHashes.Add(this.prefabInstanceId, this.sharedStateHashes);
				SyncAnimator.masterSharedStateIndexes.Add(this.prefabInstanceId, this.sharedStateIndexes);
			}
			else
			{
				this.sharedStateHashes = SyncAnimator.masterSharedStateHashes[this.prefabInstanceId];
				this.sharedStateIndexes = SyncAnimator.masterSharedStateIndexes[this.prefabInstanceId];
			}
			ParameterDefaults parameterDefaults;
			if (SyncAnimator.masterSharedParamDefaults.TryGetValue(this.prefabInstanceId, ref parameterDefaults))
			{
				this.sharedParamDefaults = parameterDefaults;
			}
			else
			{
				SyncAnimator.masterSharedParamDefaults.Add(this.prefabInstanceId, this.sharedParamDefaults);
			}
			ParameterSettings[] array;
			if (SyncAnimator.masterSharedParamSettings.TryGetValue(this.prefabInstanceId, ref array))
			{
				this.sharedParamSettings = array;
				return;
			}
			SyncAnimator.masterSharedParamSettings.Add(this.prefabInstanceId, this.sharedParamSettings);
		}

		// Token: 0x06000E6E RID: 3694 RVA: 0x0004554C File Offset: 0x0004374C
		private void OnDestroy()
		{
			SyncAnimator.masterSharedFramePool[this.prefabInstanceId].Push(this.frames);
		}

		// Token: 0x06000E6F RID: 3695 RVA: 0x0004556C File Offset: 0x0004376C
		public SerializationFlags OnNetSerialize(int frameId, byte[] buffer, ref int bitposition, SerializationFlags writeFlags)
		{
			SyncAnimator.Frame frame = this.frames[frameId];
			if (frame.content == FrameContents.Empty)
			{
				ArraySerializeExt.WriteBool(buffer, false, ref bitposition);
				return SerializationFlags.None;
			}
			ArraySerializeExt.WriteBool(buffer, true, ref bitposition);
			bool isKeyframe = base.IsKeyframe(frameId);
			return this.WriteAllToBuffer(frame, buffer, ref bitposition, isKeyframe);
		}

		// Token: 0x06000E70 RID: 3696 RVA: 0x000455B0 File Offset: 0x000437B0
		public SerializationFlags OnNetDeserialize(int originFrameId, byte[] buffer, ref int bitposition, FrameArrival arrival)
		{
			bool isKeyframe = base.IsKeyframe(originFrameId);
			SyncAnimator.Frame frame = base.IsMine ? this.offtickFrame : this.frames[originFrameId];
			if (!ArraySerializeExt.ReadBool(buffer, ref bitposition))
			{
				return SerializationFlags.None;
			}
			frame.content = FrameContents.Complete;
			this.ReadAllFromBuffer(frame, buffer, ref bitposition, isKeyframe);
			return SerializationFlags.HasContent;
		}

		// Token: 0x06000E71 RID: 3697 RVA: 0x000455FC File Offset: 0x000437FC
		public virtual void OnCaptureCurrentState(int frameId)
		{
			SyncAnimator.Frame frame = this.frames[frameId];
			if (!base.isActiveAndEnabled || !this.animator.isActiveAndEnabled)
			{
				frame.content = FrameContents.Empty;
				return;
			}
			if (this.syncParams)
			{
				this.CaptureParameters(frame);
			}
			if (this.syncPassThrus)
			{
				this.CapturePassThrus(frame);
			}
			if (this.syncStates)
			{
				this.CaptureStates(frame);
			}
			if (this.syncStates)
			{
				this.CaptureLayerWeights(frame);
			}
			frame.content = FrameContents.Complete;
		}

		// Token: 0x06000E72 RID: 3698 RVA: 0x00045674 File Offset: 0x00043874
		private SerializationFlags WriteAllToBuffer(SyncAnimator.Frame frame, byte[] buffer, ref int bitposition, bool isKeyframe)
		{
			SerializationFlags serializationFlags = SerializationFlags.None;
			if (this.syncPassThrus)
			{
				serializationFlags |= this.WritePassThrus(frame, buffer, ref bitposition, isKeyframe);
			}
			if (this.syncParams)
			{
				serializationFlags |= this.WriteParameters(frame, buffer, ref bitposition, isKeyframe);
			}
			if (this.syncStates)
			{
				serializationFlags |= this.WriteStates(frame, buffer, ref bitposition, isKeyframe);
			}
			if (this.syncLayerWeights)
			{
				serializationFlags |= this.WriteLayerWeights(frame, buffer, ref bitposition, isKeyframe);
			}
			return serializationFlags;
		}

		// Token: 0x06000E73 RID: 3699 RVA: 0x000456DC File Offset: 0x000438DC
		private void ReadAllFromBuffer(SyncAnimator.Frame frame, byte[] buffer, ref int bitposition, bool isKeyframe)
		{
			if (this.syncPassThrus)
			{
				this.ReadPassThrus(frame, buffer, ref bitposition, isKeyframe);
			}
			if (this.syncParams)
			{
				this.ReadParameters(frame, buffer, ref bitposition, isKeyframe);
			}
			if (this.syncStates)
			{
				this.ReadStates(frame, buffer, ref bitposition, isKeyframe);
			}
			if (this.syncLayerWeights)
			{
				this.ReadLayerWeights(frame, buffer, ref bitposition, isKeyframe);
			}
		}

		// Token: 0x06000E74 RID: 3700 RVA: 0x00045738 File Offset: 0x00043938
		private SerializationFlags WriteParameters(SyncAnimator.Frame frame, byte[] buffer, ref int bitposition, bool isKeyframe)
		{
			SmartVar[] parameters = frame.parameters;
			SerializationFlags serializationFlags = SerializationFlags.None;
			for (int i = 0; i < this.paramCount; i++)
			{
				ParameterSettings parameterSettings = this.sharedParamSettings[i];
				if (this.useGlobalParamSettings || parameterSettings.include)
				{
					AnimatorControllerParameterType paramType = parameterSettings.paramType;
					if (paramType == 3)
					{
						int num = parameters[i];
						if (isKeyframe || num != this.lastSentParams[i])
						{
							if (!isKeyframe)
							{
								ArraySerializeExt.WriteBool(buffer, true, ref bitposition);
							}
							if (this.useGlobalParamSettings)
							{
								ArrayPackBytesExt.WriteSignedPackedBytes(buffer, num, ref bitposition, 32);
							}
							else
							{
								parameterSettings.icrusher.WriteValue(num, buffer, ref bitposition);
							}
							this.lastSentParams[i] = num;
							serializationFlags |= SerializationFlags.HasContent;
						}
						else if (!isKeyframe)
						{
							ArraySerializeExt.WriteBool(buffer, false, ref bitposition);
						}
					}
					else if (paramType == 1)
					{
						float num2 = parameters[i];
						LiteFloatCrusher fcrusher = parameterSettings.fcrusher;
						uint num3 = this.useGlobalParamSettings ? ((uint)HalfUtilities.Pack(num2)) : ((uint)fcrusher.Encode(num2));
						if (isKeyframe || num3 != this.lastSentParams[i].UInt)
						{
							if (!isKeyframe)
							{
								ArraySerializeExt.WriteBool(buffer, true, ref bitposition);
							}
							if (this.useGlobalParamSettings)
							{
								ArraySerializeExt.Write(buffer, (ulong)num3, ref bitposition, 16);
							}
							else
							{
								fcrusher.WriteCValue(num3, buffer, ref bitposition);
							}
							this.lastSentParams[i] = num3;
							serializationFlags |= SerializationFlags.HasContent;
						}
						else if (!isKeyframe)
						{
							ArraySerializeExt.WriteBool(buffer, false, ref bitposition);
						}
					}
					else if (paramType == 4)
					{
						bool flag = parameters[i];
						ArraySerializeExt.WriteBool(buffer, flag, ref bitposition);
						if (isKeyframe || flag != this.lastSentParams[i])
						{
							serializationFlags |= SerializationFlags.HasContent;
						}
					}
					else if (paramType == 9)
					{
						bool flag2 = parameters[i];
						ArraySerializeExt.WriteBool(buffer, flag2, ref bitposition);
						if (isKeyframe || flag2 != this.lastSentParams[i])
						{
							serializationFlags |= SerializationFlags.HasContent;
						}
					}
				}
			}
			return serializationFlags;
		}

		// Token: 0x06000E75 RID: 3701 RVA: 0x0004593C File Offset: 0x00043B3C
		private void CaptureParameters(SyncAnimator.Frame frame)
		{
			SmartVar[] parameters = frame.parameters;
			for (int i = 0; i < this.paramCount; i++)
			{
				ParameterSettings parameterSettings = this.sharedParamSettings[i];
				if (this.useGlobalParamSettings || parameterSettings.include)
				{
					AnimatorControllerParameterType paramType = parameterSettings.paramType;
					int hash = parameterSettings.hash;
					switch (paramType)
					{
					case 1:
						parameters[i] = this.animator.GetFloat(hash);
						break;
					case 2:
						break;
					case 3:
						parameters[i] = this.animator.GetInteger(hash);
						break;
					case 4:
						parameters[i] = this.animator.GetBool(hash);
						break;
					default:
						if (paramType == 9)
						{
							parameters[i] = this.animator.GetBool(hash);
						}
						break;
					}
				}
			}
		}

		// Token: 0x06000E76 RID: 3702 RVA: 0x00045A1C File Offset: 0x00043C1C
		private void ReadParameters(SyncAnimator.Frame frame, byte[] buffer, ref int bitposition, bool isKeyframe)
		{
			SmartVar[] parameters = frame.parameters;
			bool flag = frame == this.targFrame || frame == this.snapFrame;
			for (int i = 0; i < this.paramCount; i++)
			{
				ParameterSettings parameterSettings = this.sharedParamSettings[i];
				if (this.useGlobalParamSettings || parameterSettings.include)
				{
					AnimatorControllerParameterType paramType = parameterSettings.paramType;
					switch (paramType)
					{
					case 1:
						if (isKeyframe || ArraySerializeExt.ReadBool(buffer, ref bitposition))
						{
							parameters[i] = (this.useGlobalParamSettings ? ArraySerializeHalfExt.ReadHalf(buffer, ref bitposition) : parameterSettings.fcrusher.ReadValue(buffer, ref bitposition));
						}
						else if (!flag)
						{
							parameters[i] = SmartVar.None;
						}
						break;
					case 2:
						break;
					case 3:
						if (isKeyframe || ArraySerializeExt.ReadBool(buffer, ref bitposition))
						{
							int v = this.useGlobalParamSettings ? ArrayPackBytesExt.ReadSignedPackedBytes(buffer, ref bitposition, 32) : parameterSettings.icrusher.ReadValue(buffer, ref bitposition);
							parameters[i] = v;
						}
						else if (!flag)
						{
							parameters[i] = SmartVar.None;
						}
						break;
					case 4:
					{
						bool v2 = ArraySerializeExt.ReadBool(buffer, ref bitposition);
						parameters[i] = v2;
						break;
					}
					default:
						if (paramType == 9)
						{
							bool v3 = ArraySerializeExt.ReadBool(buffer, ref bitposition);
							parameters[i] = v3;
						}
						break;
					}
				}
			}
		}

		// Token: 0x06000E77 RID: 3703 RVA: 0x00045B80 File Offset: 0x00043D80
		private void CompleteTargetParameters()
		{
			SmartVar[] array = (this.snapFrame != null) ? this.snapFrame.parameters : this.targFrame.parameters;
			SmartVar[] parameters = this.targFrame.parameters;
			for (int i = 0; i < this.paramCount; i++)
			{
				SmartVar smartVar = array[i];
				ref SmartVar ptr = parameters[i];
				ParameterSettings parameterSettings = this.sharedParamSettings[i];
				if (smartVar.TypeCode == SmartVarTypeCode.None)
				{
					smartVar = parameterSettings.defaultValue;
					array[i] = smartVar;
				}
				if (ptr.TypeCode == SmartVarTypeCode.None)
				{
					parameters[i] = smartVar;
				}
			}
		}

		// Token: 0x06000E78 RID: 3704 RVA: 0x00045C0C File Offset: 0x00043E0C
		private void InterpolateParams(float t)
		{
			SmartVar[] parameters = this.snapFrame.parameters;
			SmartVar[] parameters2 = this.targFrame.parameters;
			for (int i = 0; i < this.paramCount; i++)
			{
				ParameterSettings parameterSettings = this.sharedParamSettings[i];
				int hash = parameterSettings.hash;
				if (this.useGlobalParamSettings || parameterSettings.include)
				{
					AnimatorControllerParameterType paramType = parameterSettings.paramType;
					SmartVar smartVar = parameters[i];
					SmartVar smartVar2 = parameters2[i];
					if (smartVar.TypeCode != SmartVarTypeCode.None && smartVar2.TypeCode != SmartVarTypeCode.None)
					{
						switch (paramType)
						{
						case 1:
							if (this.sharedParamDefaults.includeFloats)
							{
								if (t == 0f)
								{
									this.animator.SetFloat(hash, smartVar);
								}
								else
								{
									ParameterInterpolation parameterInterpolation;
									if (this.useGlobalParamSettings)
									{
										parameterInterpolation = this.sharedParamDefaults.interpolateFloats;
									}
									else
									{
										parameterInterpolation = parameterSettings.interpolate;
									}
									if (parameterInterpolation != ParameterInterpolation.Hold)
									{
										SmartVar v = (parameterInterpolation == ParameterInterpolation.Lerp) ? Mathf.Lerp(smartVar, smartVar2, t) : ((parameterInterpolation == ParameterInterpolation.Advance) ? smartVar2 : parameterSettings.defaultValue);
										this.animator.SetFloat(hash, v);
									}
								}
							}
							break;
						case 2:
							break;
						case 3:
							if (this.sharedParamDefaults.includeInts)
							{
								if (t == 0f)
								{
									this.animator.SetInteger(hash, smartVar);
								}
								else
								{
									ParameterInterpolation parameterInterpolation2;
									if (this.useGlobalParamSettings)
									{
										parameterInterpolation2 = this.sharedParamDefaults.interpolateInts;
									}
									else
									{
										parameterInterpolation2 = parameterSettings.interpolate;
									}
									if (parameterInterpolation2 != ParameterInterpolation.Hold)
									{
										int num = (parameterInterpolation2 == ParameterInterpolation.Advance) ? smartVar2 : ((parameterInterpolation2 == ParameterInterpolation.Lerp) ? ((int)Mathf.Lerp(smartVar, smartVar2, t)) : parameterSettings.defaultValue);
										this.animator.SetInteger(hash, num);
									}
								}
							}
							break;
						case 4:
							if (t == 0f && this.sharedParamDefaults.includeBools)
							{
								this.animator.SetBool(hash, smartVar);
							}
							break;
						default:
							if (paramType == 9)
							{
								if (t == 0f && this.sharedParamDefaults.includeTriggers && smartVar)
								{
									this.animator.SetTrigger(hash);
								}
							}
							break;
						}
					}
				}
			}
		}

		// Token: 0x06000E79 RID: 3705 RVA: 0x00045E60 File Offset: 0x00044060
		private void ExtrapolateParams(SyncAnimator.Frame prev, SyncAnimator.Frame targ, SyncAnimator.Frame newtarg)
		{
			if (prev == null)
			{
				return;
			}
			SmartVar[] parameters = prev.parameters;
			SmartVar[] parameters2 = targ.parameters;
			for (int i = 0; i < this.paramCount; i++)
			{
				ParameterSettings parameterSettings = this.sharedParamSettings[i];
				AnimatorControllerParameterType paramType = parameterSettings.paramType;
				if (this.useGlobalParamSettings || parameterSettings.include)
				{
					if (paramType == 1)
					{
						ParameterExtrapolation parameterExtrapolation = this.useGlobalParamSettings ? this.sharedParamDefaults.extrapolateFloats : parameterSettings.extrapolate;
						newtarg.parameters[i] = ((parameterExtrapolation == ParameterExtrapolation.Hold) ? parameters2[i] : ((parameterExtrapolation == ParameterExtrapolation.Lerp) ? (parameters2[i] + (parameters2[i] - parameters[i])) : parameterSettings.defaultValue));
					}
					else if (paramType == 3)
					{
						ParameterExtrapolation parameterExtrapolation2 = this.useGlobalParamSettings ? this.sharedParamDefaults.extrapolateInts : parameterSettings.extrapolate;
						newtarg.parameters[i] = ((parameterExtrapolation2 == ParameterExtrapolation.Hold) ? parameters2[i] : ((parameterExtrapolation2 == ParameterExtrapolation.Lerp) ? (parameters2[i] + (parameters2[i] - parameters[i])) : parameterSettings.defaultValue));
					}
					else if (paramType == 4)
					{
						ParameterExtrapolation parameterExtrapolation3 = this.useGlobalParamSettings ? this.sharedParamDefaults.extrapolateBools : parameterSettings.extrapolate;
						newtarg.parameters[i] = ((parameterExtrapolation3 == ParameterExtrapolation.Hold) ? parameters2[i] : parameterSettings.defaultValue);
					}
					else
					{
						ParameterExtrapolation parameterExtrapolation4 = this.useGlobalParamSettings ? this.sharedParamDefaults.extrapolateTriggers : parameterSettings.extrapolate;
						newtarg.parameters[i] = ((parameterExtrapolation4 == ParameterExtrapolation.Hold) ? parameters2[i] : parameterSettings.defaultValue);
					}
				}
			}
		}

		// Token: 0x06000E7A RID: 3706 RVA: 0x00046030 File Offset: 0x00044230
		private void EnqueuePassthrough(PassThruType type, int hash, int layer, float ntime, float otime, float duration, LocalApplyTiming localApplyTiming)
		{
			AnimPassThru animPassThru = new AnimPassThru(type, hash, layer, ntime, otime, duration, localApplyTiming);
			this.passThruQueue.Enqueue(animPassThru);
			if (localApplyTiming == LocalApplyTiming.Immediately || !this.syncPassThrus)
			{
				this.ExecutePassThru(animPassThru);
			}
		}

		// Token: 0x06000E7B RID: 3707 RVA: 0x00046070 File Offset: 0x00044270
		public void SetTrigger(string triggerName, LocalApplyTiming localApplyTiming = LocalApplyTiming.OnSend)
		{
			int hash = Animator.StringToHash(triggerName);
			this.EnqueuePassthrough(PassThruType.SetTrigger, hash, -1, -1f, -1f, -1f, localApplyTiming);
		}

		// Token: 0x06000E7C RID: 3708 RVA: 0x0004609D File Offset: 0x0004429D
		public void SetTrigger(int hash, LocalApplyTiming localApplyTiming = LocalApplyTiming.OnSend)
		{
			this.EnqueuePassthrough(PassThruType.SetTrigger, hash, -1, -1f, -1f, -1f, localApplyTiming);
		}

		// Token: 0x06000E7D RID: 3709 RVA: 0x000460B8 File Offset: 0x000442B8
		public void ResetTrigger(string triggerName, LocalApplyTiming localApplyTiming = LocalApplyTiming.OnSend)
		{
			int hash = Animator.StringToHash(triggerName);
			this.EnqueuePassthrough(PassThruType.ResetTrigger, hash, -1, -1f, -1f, -1f, localApplyTiming);
		}

		// Token: 0x06000E7E RID: 3710 RVA: 0x000460E5 File Offset: 0x000442E5
		public void ResetTrigger(int hash, LocalApplyTiming localApplyTiming = LocalApplyTiming.OnSend)
		{
			this.EnqueuePassthrough(PassThruType.ResetTrigger, hash, -1, -1f, -1f, -1f, localApplyTiming);
		}

		// Token: 0x06000E7F RID: 3711 RVA: 0x00046100 File Offset: 0x00044300
		public void Play(string stateName, int layer = -1, float normalizedTime = 0f, LocalApplyTiming localApplyTiming = LocalApplyTiming.OnSend)
		{
			int hash = Animator.StringToHash(stateName);
			this.EnqueuePassthrough(PassThruType.Play, hash, layer, normalizedTime, -1f, -1f, localApplyTiming);
		}

		// Token: 0x06000E80 RID: 3712 RVA: 0x0004612A File Offset: 0x0004432A
		public void Play(int hash, int layer = -1, float normalizedTime = 0f, LocalApplyTiming localApplyTiming = LocalApplyTiming.OnSend)
		{
			this.EnqueuePassthrough(PassThruType.Play, hash, layer, normalizedTime, -1f, -1f, localApplyTiming);
		}

		// Token: 0x06000E81 RID: 3713 RVA: 0x00046144 File Offset: 0x00044344
		public void PlayInFixedTime(string stateName, int layer = -1, float fixedTime = 0f, LocalApplyTiming localApplyTiming = LocalApplyTiming.OnSend)
		{
			int hash = Animator.StringToHash(stateName);
			this.EnqueuePassthrough(PassThruType.PlayFixed, hash, layer, -1f, fixedTime, -1f, localApplyTiming);
		}

		// Token: 0x06000E82 RID: 3714 RVA: 0x0004616E File Offset: 0x0004436E
		public void PlayInFixedTime(int hash, int layer = -1, float fixedTime = 0f, LocalApplyTiming localApplyTiming = LocalApplyTiming.OnSend)
		{
			this.EnqueuePassthrough(PassThruType.PlayFixed, hash, layer, -1f, fixedTime, -1f, localApplyTiming);
		}

		// Token: 0x06000E83 RID: 3715 RVA: 0x00046188 File Offset: 0x00044388
		public void CrossFade(string stateName, float duration, int layer = -1, float normalizedTime = 0f, LocalApplyTiming localApplyTiming = LocalApplyTiming.OnSend)
		{
			int hash = Animator.StringToHash(stateName);
			this.EnqueuePassthrough(PassThruType.CrossFade, hash, layer, normalizedTime, -1f, duration, localApplyTiming);
		}

		// Token: 0x06000E84 RID: 3716 RVA: 0x000461AF File Offset: 0x000443AF
		public void CrossFade(int hash, float duration, int layer = -1, float normalizedTime = 0f, LocalApplyTiming localApplyTiming = LocalApplyTiming.OnSend)
		{
			this.EnqueuePassthrough(PassThruType.CrossFade, hash, layer, normalizedTime, -1f, duration, localApplyTiming);
		}

		// Token: 0x06000E85 RID: 3717 RVA: 0x000461C4 File Offset: 0x000443C4
		public void CrossFadeInFixedTime(string stateName, float duration, int layer = -1, float fixedTime = 0f, LocalApplyTiming localApplyTiming = LocalApplyTiming.OnSend)
		{
			int hash = Animator.StringToHash(stateName);
			this.EnqueuePassthrough(PassThruType.CrossFadeFixed, hash, layer, -1f, fixedTime, duration, localApplyTiming);
		}

		// Token: 0x06000E86 RID: 3718 RVA: 0x000461EB File Offset: 0x000443EB
		public void CrossFadeInFixedTime(int hash, float duration, int layer = -1, float fixedTime = 0f, LocalApplyTiming localApplyTiming = LocalApplyTiming.OnSend)
		{
			this.EnqueuePassthrough(PassThruType.CrossFadeFixed, hash, layer, -1f, fixedTime, duration, localApplyTiming);
		}

		// Token: 0x06000E87 RID: 3719 RVA: 0x00046200 File Offset: 0x00044400
		private void ExecutePassThruQueue(SyncAnimator.Frame frame)
		{
			Queue<AnimPassThru> passThrus = frame.passThrus;
			while (passThrus.Count > 0)
			{
				AnimPassThru pt = passThrus.Dequeue();
				this.ExecutePassThru(pt);
			}
		}

		// Token: 0x06000E88 RID: 3720 RVA: 0x00046230 File Offset: 0x00044430
		private void ExecutePassThru(AnimPassThru pt)
		{
			int hash = pt.hash;
			switch (pt.passThruType)
			{
			case PassThruType.SetTrigger:
				this.animator.SetTrigger(pt.hash);
				return;
			case PassThruType.ResetTrigger:
				this.animator.ResetTrigger(pt.hash);
				return;
			case PassThruType.Play:
				this.animator.Play(hash, pt.layer, pt.normlTime);
				return;
			case PassThruType.PlayFixed:
				this.animator.PlayInFixedTime(hash, pt.layer, pt.fixedTime);
				return;
			case PassThruType.CrossFade:
				this.animator.CrossFade(hash, pt.duration, pt.layer, pt.normlTime);
				return;
			case PassThruType.CrossFadeFixed:
				this.animator.CrossFadeInFixedTime(hash, pt.duration, pt.layer, pt.fixedTime);
				return;
			default:
				return;
			}
		}

		// Token: 0x06000E89 RID: 3721 RVA: 0x00046300 File Offset: 0x00044500
		private SerializationFlags WritePassThrus(SyncAnimator.Frame frame, byte[] buffer, ref int bitposition, bool isKeyframe)
		{
			Queue<AnimPassThru> passThrus = frame.passThrus;
			SerializationFlags result = (passThrus.Count == 0) ? SerializationFlags.None : SerializationFlags.HasContent;
			while (passThrus.Count > 0)
			{
				AnimPassThru animPassThru = passThrus.Dequeue();
				PassThruType passThruType = animPassThru.passThruType;
				int hash = animPassThru.hash;
				bool flag = passThruType == PassThruType.SetTrigger || passThruType == PassThruType.ResetTrigger;
				if (animPassThru.localApplyTiming == LocalApplyTiming.OnSend)
				{
					this.ExecutePassThru(animPassThru);
				}
				ArraySerializeExt.WriteBool(buffer, true, ref bitposition);
				ArraySerializeExt.Write(buffer, (ulong)passThruType, ref bitposition, 3);
				int num;
				bool flag2 = flag ? this.sharedTriggHashes.TryGetValue(hash, ref num) : this.sharedStateHashes.TryGetValue(animPassThru.hash, ref num);
				ArraySerializeExt.WriteBool(buffer, flag2, ref bitposition);
				if (flag2)
				{
					ArraySerializeExt.Write(buffer, (ulong)num, ref bitposition, flag ? this.bitsForTriggerIndex : this.bitsForStateIndex);
				}
				else
				{
					ArraySerializeExt.WriteSigned(buffer, animPassThru.hash, ref bitposition, 32);
				}
				if (!flag)
				{
					if (this.layerCount > 1)
					{
						ArraySerializeExt.Write(buffer, (ulong)(animPassThru.layer + 1), ref bitposition, this.bitsForLayerIndex);
					}
					if (passThruType == PassThruType.Play || passThruType == PassThruType.CrossFade)
					{
						float normlTime = animPassThru.normlTime;
						if (normlTime == 0f)
						{
							ArraySerializeExt.WriteBool(buffer, false, ref bitposition);
						}
						else
						{
							ArraySerializeExt.WriteBool(buffer, true, ref bitposition);
							buffer.WriteNorm(normlTime, ref bitposition, (int)this.passthruNormTimeCompress);
						}
					}
					else
					{
						float fixedTime = animPassThru.fixedTime;
						if (fixedTime == 0f)
						{
							ArraySerializeExt.WriteBool(buffer, false, ref bitposition);
						}
						else
						{
							ArraySerializeExt.WriteBool(buffer, true, ref bitposition);
							ArraySerializeHalfExt.WriteHalf(buffer, fixedTime, ref bitposition);
						}
					}
					if (passThruType == PassThruType.CrossFade || passThruType == PassThruType.CrossFadeFixed)
					{
						ArraySerializeHalfExt.WriteHalf(buffer, animPassThru.duration, ref bitposition);
					}
				}
			}
			ArraySerializeExt.WriteBool(buffer, false, ref bitposition);
			return result;
		}

		// Token: 0x06000E8A RID: 3722 RVA: 0x00046493 File Offset: 0x00044693
		private void CapturePassThrus(SyncAnimator.Frame frame)
		{
			if (this.syncPassThrus)
			{
				while (this.passThruQueue.Count > 0)
				{
					frame.passThrus.Enqueue(this.passThruQueue.Dequeue());
				}
			}
		}

		// Token: 0x06000E8B RID: 3723 RVA: 0x000464C4 File Offset: 0x000446C4
		private void ReadPassThrus(SyncAnimator.Frame frame, byte[] buffer, ref int bitposition, bool isKeyframe)
		{
			while (ArraySerializeExt.ReadBool(buffer, ref bitposition))
			{
				PassThruType passThruType = (PassThruType)ArraySerializeExt.Read(buffer, ref bitposition, 3);
				bool flag = passThruType == PassThruType.SetTrigger || passThruType == PassThruType.ResetTrigger;
				int num;
				if (ArraySerializeExt.ReadBool(buffer, ref bitposition))
				{
					if (flag)
					{
						num = (int)ArraySerializeExt.Read(buffer, ref bitposition, this.bitsForTriggerIndex);
						num = this.sharedTriggIndexes[num];
					}
					else
					{
						num = (int)ArraySerializeExt.Read(buffer, ref bitposition, this.bitsForStateIndex);
						num = this.sharedStateIndexes[num];
					}
				}
				else
				{
					num = ArraySerializeExt.ReadSigned(buffer, ref bitposition, 32);
				}
				if (flag)
				{
					frame.passThrus.Enqueue(new AnimPassThru(passThruType, num, -1, -1f, -1f, -1f, LocalApplyTiming.OnSend));
				}
				else
				{
					int layer = (this.layerCount > 1) ? ((int)ArraySerializeExt.Read(buffer, ref bitposition, this.bitsForLayerIndex) - 1) : -1;
					float normTime;
					float otherTime;
					if (passThruType == PassThruType.Play || passThruType == PassThruType.CrossFade)
					{
						normTime = (ArraySerializeExt.ReadBool(buffer, ref bitposition) ? buffer.ReadNorm(ref bitposition, (int)this.passthruNormTimeCompress) : 0f);
						otherTime = -1f;
					}
					else
					{
						otherTime = (ArraySerializeExt.ReadBool(buffer, ref bitposition) ? ArraySerializeHalfExt.ReadHalf(buffer, ref bitposition) : 0f);
						normTime = -1f;
					}
					float duration = (passThruType == PassThruType.CrossFade || passThruType == PassThruType.CrossFadeFixed) ? ArraySerializeHalfExt.ReadHalf(buffer, ref bitposition) : -1f;
					frame.passThrus.Enqueue(new AnimPassThru(passThruType, num, layer, normTime, otherTime, duration, LocalApplyTiming.OnSend));
				}
			}
		}

		// Token: 0x06000E8C RID: 3724 RVA: 0x00046620 File Offset: 0x00044820
		private void CaptureStates(SyncAnimator.Frame frame)
		{
			int num = this.syncLayers ? this.layerCount : 1;
			for (int i = 0; i < num; i++)
			{
				frame.layerWeights[i] = new float?(this.animator.GetLayerWeight(i));
				if (this.animator.IsInTransition(i))
				{
					frame.layerIsInTransition[i] = true;
				}
				else
				{
					AnimatorStateInfo currentAnimatorStateInfo = this.animator.GetCurrentAnimatorStateInfo(i);
					frame.layerIsInTransition[i] = false;
					frame.stateHashes[i] = new int?(currentAnimatorStateInfo.fullPathHash);
					float normalizedTime = currentAnimatorStateInfo.normalizedTime;
					if (this.normalizedTimeCompress == NormalizedFloatCompression.Full32 || this.normalizedTimeCompress == NormalizedFloatCompression.Half16)
					{
						frame.normalizedTime[i] = normalizedTime;
					}
					else if (currentAnimatorStateInfo.loop)
					{
						frame.normalizedTime[i] = ((normalizedTime > 1f) ? (normalizedTime % 1f) : normalizedTime);
					}
					else
					{
						frame.normalizedTime[i] = ((normalizedTime > 1f) ? 1f : ((normalizedTime < 0f) ? 0f : normalizedTime));
					}
				}
			}
		}

		// Token: 0x06000E8D RID: 3725 RVA: 0x0004672C File Offset: 0x0004492C
		private SerializationFlags WriteStates(SyncAnimator.Frame frame, byte[] buffer, ref int bitposition, bool isKeyframe)
		{
			int?[] stateHashes = frame.stateHashes;
			float[] normalizedTime = frame.normalizedTime;
			float?[] layerWeights = frame.layerWeights;
			bool[] layerIsInTransition = frame.layerIsInTransition;
			SerializationFlags serializationFlags = SerializationFlags.None;
			int num = this.syncLayers ? this.layerCount : 1;
			for (int i = 0; i < num; i++)
			{
				int? num2 = stateHashes[i];
				bool flag = num2 == null || this.lastAnimationHash[i] != num2.Value;
				if (isKeyframe || flag)
				{
					ArraySerializeExt.WriteBool(buffer, true, ref bitposition);
					bool flag2 = layerIsInTransition[i];
					ArraySerializeExt.WriteBool(buffer, flag2, ref bitposition);
					if (!flag2)
					{
						bool flag3 = this.sharedStateIndexes.IndexOf(num2.Value) != -1;
						ArraySerializeExt.WriteBool(buffer, flag3, ref bitposition);
						if (flag3)
						{
							ArraySerializeExt.Write(buffer, (ulong)this.sharedStateIndexes.IndexOf(num2.Value), ref bitposition, this.bitsForStateIndex);
						}
						else
						{
							ArraySerializeExt.WriteSigned(buffer, num2.Value, ref bitposition, 32);
						}
						float value = (normalizedTime[i] + 1f) * 0.5f;
						buffer.WriteNorm(value, ref bitposition, (int)this.normalizedTimeCompress);
					}
					this.lastAnimationHash[i] = ((num2 != null) ? num2.Value : 0);
					serializationFlags |= SerializationFlags.HasContent;
				}
				else
				{
					ArraySerializeExt.WriteBool(buffer, false, ref bitposition);
				}
				if (this.syncLayerWeights && i != 0)
				{
					float? num3 = layerWeights[i];
					float? num4 = num3;
					float num5 = (float)1;
					if (num4.GetValueOrDefault() == num5 & num4 != null)
					{
						ArraySerializeExt.WriteBool(buffer, true, ref bitposition);
						serializationFlags |= SerializationFlags.HasContent;
					}
					else
					{
						ArraySerializeExt.WriteBool(buffer, false, ref bitposition);
						num4 = num3;
						num5 = 0f;
						if (num4.GetValueOrDefault() == num5 & num4 != null)
						{
							ArraySerializeExt.WriteBool(buffer, true, ref bitposition);
							serializationFlags |= SerializationFlags.HasContent;
						}
						else
						{
							ArraySerializeExt.WriteBool(buffer, false, ref bitposition);
							int num6 = (int)this.layerWeightCompress;
							uint num7 = num3.Value.CompressNorm(num6);
							if (isKeyframe || this.lastLayerWeight[i] != num7)
							{
								ArraySerializeExt.WriteBool(buffer, true, ref bitposition);
								ArraySerializeExt.Write(buffer, (ulong)num7, ref bitposition, num6);
								this.lastLayerWeight[i] = num7;
								serializationFlags |= SerializationFlags.HasContent;
							}
							else
							{
								ArraySerializeExt.WriteBool(buffer, false, ref bitposition);
							}
						}
					}
				}
			}
			return serializationFlags;
		}

		// Token: 0x06000E8E RID: 3726 RVA: 0x00046964 File Offset: 0x00044B64
		private void ReadStates(SyncAnimator.Frame frame, byte[] buffer, ref int bitposition, bool isKeyframe)
		{
			int?[] stateHashes = frame.stateHashes;
			float[] normalizedTime = frame.normalizedTime;
			float?[] layerWeights = frame.layerWeights;
			bool[] layerIsInTransition = frame.layerIsInTransition;
			int num = this.syncLayers ? this.layerCount : 1;
			for (int i = 0; i < num; i++)
			{
				if (ArraySerializeExt.ReadBool(buffer, ref bitposition))
				{
					bool flag = ArraySerializeExt.ReadBool(buffer, ref bitposition);
					layerIsInTransition[i] = flag;
					if (!flag)
					{
						bool flag2 = ArraySerializeExt.ReadBool(buffer, ref bitposition);
						int num2 = flag2 ? ((int)ArraySerializeExt.Read(buffer, ref bitposition, this.bitsForStateIndex)) : ArraySerializeExt.ReadSigned(buffer, ref bitposition, 32);
						if (flag2)
						{
							num2 = this.sharedStateIndexes[num2];
						}
						stateHashes[i] = new int?(num2);
						float num3 = buffer.ReadNorm(ref bitposition, (int)this.normalizedTimeCompress) * 2f - 1f;
						normalizedTime[i] = num3;
					}
				}
				if (this.syncLayerWeights && i != 0)
				{
					if (ArraySerializeExt.ReadBool(buffer, ref bitposition))
					{
						layerWeights[i] = new float?((float)1);
					}
					else if (ArraySerializeExt.ReadBool(buffer, ref bitposition))
					{
						layerWeights[i] = new float?(0f);
					}
					else if (ArraySerializeExt.ReadBool(buffer, ref bitposition))
					{
						layerWeights[i] = new float?(buffer.ReadNorm(ref bitposition, (int)this.layerWeightCompress));
					}
					else
					{
						layerWeights[i] = default(float?);
					}
				}
			}
		}

		// Token: 0x06000E8F RID: 3727 RVA: 0x00046AB0 File Offset: 0x00044CB0
		private void ApplyState(SyncAnimator.Frame applyFrame)
		{
			int num = this.syncLayers ? this.layerCount : 1;
			for (int i = 0; i < num; i++)
			{
				int? num2 = applyFrame.stateHashes[i];
				bool flag = applyFrame.layerIsInTransition[i];
				if (num2 != null)
				{
					if (!flag && num2.Value != 0)
					{
						this.animator.Play(num2.Value, i, applyFrame.normalizedTime[i]);
					}
					if (this.syncLayerWeights)
					{
						float? num3 = applyFrame.layerWeights[i];
						if (num3 != null)
						{
							this.animator.SetLayerWeight(i, num3.Value);
						}
					}
				}
			}
		}

		// Token: 0x06000E90 RID: 3728 RVA: 0x00046B54 File Offset: 0x00044D54
		private void CaptureLayerWeights(SyncAnimator.Frame frame)
		{
			int num = this.syncLayers ? this.layerCount : 1;
			for (int i = 0; i < num; i++)
			{
				frame.layerWeights[i] = new float?(this.animator.GetLayerWeight(i));
			}
		}

		// Token: 0x06000E91 RID: 3729 RVA: 0x00046B9C File Offset: 0x00044D9C
		private SerializationFlags WriteLayerWeights(SyncAnimator.Frame frame, byte[] buffer, ref int bitposition, bool isKeyframe)
		{
			float?[] layerWeights = frame.layerWeights;
			SerializationFlags serializationFlags = SerializationFlags.None;
			int num = this.syncLayers ? this.layerCount : 1;
			for (int i = 0; i < num; i++)
			{
				if (this.syncLayerWeights && i != 0)
				{
					float? num2 = layerWeights[i];
					float? num3 = num2;
					float num4 = (float)1;
					if (num3.GetValueOrDefault() == num4 & num3 != null)
					{
						ArraySerializeExt.WriteBool(buffer, true, ref bitposition);
						serializationFlags |= SerializationFlags.HasContent;
					}
					else
					{
						ArraySerializeExt.WriteBool(buffer, false, ref bitposition);
						num3 = num2;
						num4 = 0f;
						if (num3.GetValueOrDefault() == num4 & num3 != null)
						{
							ArraySerializeExt.WriteBool(buffer, true, ref bitposition);
							serializationFlags |= SerializationFlags.HasContent;
						}
						else
						{
							ArraySerializeExt.WriteBool(buffer, false, ref bitposition);
							int num5 = (int)this.layerWeightCompress;
							uint num6 = num2.Value.CompressNorm(num5);
							if (isKeyframe || this.lastLayerWeight[i] != num6)
							{
								ArraySerializeExt.WriteBool(buffer, true, ref bitposition);
								ArraySerializeExt.Write(buffer, (ulong)num6, ref bitposition, num5);
								this.lastLayerWeight[i] = num6;
								serializationFlags |= SerializationFlags.HasContent;
							}
							else
							{
								ArraySerializeExt.WriteBool(buffer, false, ref bitposition);
							}
						}
					}
				}
			}
			return serializationFlags;
		}

		// Token: 0x06000E92 RID: 3730 RVA: 0x00046CB4 File Offset: 0x00044EB4
		private SerializationFlags ReadLayerWeights(SyncAnimator.Frame frame, byte[] buffer, ref int bitposition, bool isKeyframe)
		{
			float?[] layerWeights = frame.layerWeights;
			SerializationFlags serializationFlags = SerializationFlags.None;
			int num = this.syncLayers ? this.layerCount : 1;
			for (int i = 0; i < num; i++)
			{
				if (this.syncLayerWeights && i != 0)
				{
					if (ArraySerializeExt.ReadBool(buffer, ref bitposition))
					{
						layerWeights[i] = new float?((float)1);
						serializationFlags |= SerializationFlags.HasContent;
					}
					else if (ArraySerializeExt.ReadBool(buffer, ref bitposition))
					{
						layerWeights[i] = new float?(0f);
						serializationFlags |= SerializationFlags.HasContent;
					}
					else if (ArraySerializeExt.ReadBool(buffer, ref bitposition))
					{
						layerWeights[i] = new float?(buffer.ReadNorm(ref bitposition, (int)this.layerWeightCompress));
						serializationFlags |= SerializationFlags.HasContent;
					}
					else
					{
						layerWeights[i] = default(float?);
					}
				}
			}
			return SerializationFlags.HasContent;
		}

		// Token: 0x06000E93 RID: 3731 RVA: 0x00046D6C File Offset: 0x00044F6C
		private void ApplyLayerWeights(SyncAnimator.Frame applyFrame)
		{
			int num = this.syncLayers ? this.layerCount : 1;
			for (int i = 0; i < num; i++)
			{
				if (this.syncLayerWeights)
				{
					float? num2 = applyFrame.layerWeights[i];
					if (num2 != null)
					{
						this.animator.SetLayerWeight(i, num2.Value);
					}
				}
			}
		}

		// Token: 0x06000E94 RID: 3732 RVA: 0x00046DC8 File Offset: 0x00044FC8
		public override bool OnSnapshot(int prevFrameId, int snapFrameId, int targFrameId, bool prevIsValid, bool snapIsValid, bool targIsValid)
		{
			if (this.snapFrame != null && this.snapFrame.content > FrameContents.Empty)
			{
				this.ApplyFrame(this.snapFrame);
			}
			if (!base.OnSnapshot(prevFrameId, snapFrameId, targFrameId, prevIsValid, snapIsValid, targIsValid))
			{
				return false;
			}
			this.CompleteTargetParameters();
			return true;
		}

		// Token: 0x06000E95 RID: 3733 RVA: 0x00046E16 File Offset: 0x00045016
		private void ApplyFrame(SyncAnimator.Frame frame)
		{
			if (this.syncStates)
			{
				this.ApplyState(frame);
			}
			if (this.syncLayerWeights)
			{
				this.ApplyLayerWeights(frame);
			}
			this.ExecutePassThruQueue(frame);
			this.InterpolateParams(0f);
		}

		// Token: 0x06000E96 RID: 3734 RVA: 0x00046E48 File Offset: 0x00045048
		public override bool OnInterpolate(int snapFrameId, int targFrameId, float t)
		{
			if (!base.OnInterpolate(snapFrameId, targFrameId, t))
			{
				return false;
			}
			if (this.targFrame == null)
			{
				return false;
			}
			if (this.targFrame.content == FrameContents.Empty)
			{
				return false;
			}
			if (this.syncParams)
			{
				this.InterpolateParams(t);
			}
			return true;
		}

		// Token: 0x06000E97 RID: 3735 RVA: 0x00046E80 File Offset: 0x00045080
		protected override void InterpolateFrame(SyncAnimator.Frame targframe, SyncAnimator.Frame startframe, SyncAnimator.Frame endframe, float t)
		{
			targframe.CopyFrom(endframe);
			this.InterpolateState(targframe, startframe, endframe, t);
		}

		// Token: 0x06000E98 RID: 3736 RVA: 0x00046E94 File Offset: 0x00045094
		protected override void ExtrapolateFrame(SyncAnimator.Frame prevframe, SyncAnimator.Frame snapframe, SyncAnimator.Frame targframe)
		{
			targframe.content = FrameContents.Partial;
			this.ExtrapolateParams(prevframe, snapframe, targframe);
			this.ExtrapolateState();
		}

		// Token: 0x06000E99 RID: 3737 RVA: 0x00046EAC File Offset: 0x000450AC
		private void ExtrapolateState()
		{
			int num = this.syncLayers ? this.layerCount : 1;
			for (int i = 0; i < num; i++)
			{
				this.targFrame.stateHashes[i] = default(int?);
			}
		}

		// Token: 0x06000E9A RID: 3738 RVA: 0x00046EF0 File Offset: 0x000450F0
		private void InterpolateState(SyncAnimator.Frame targFrame, SyncAnimator.Frame strFrame, SyncAnimator.Frame endFrame, float t)
		{
			int num = this.syncLayers ? this.layerCount : 1;
			for (int i = 0; i < num; i++)
			{
				int? num2 = strFrame.stateHashes[i];
				int? num3 = endFrame.stateHashes[i];
				targFrame.stateHashes[i] = num3;
				float num4 = strFrame.normalizedTime[i];
				float b = endFrame.normalizedTime[i];
				int? num5 = num2;
				int? num6 = num3;
				if (!(num5.GetValueOrDefault() == num6.GetValueOrDefault() & num5 != null == (num6 != null)) && num4 != 0f)
				{
					targFrame.normalizedTime[i] = Mathf.LerpUnclamped(num4, b, t);
				}
				else
				{
					targFrame.normalizedTime[i] = num4;
				}
			}
		}

		// Token: 0x04000D88 RID: 3464
		private static Dictionary<int, Dictionary<int, int>> masterSharedTriggHashes = new Dictionary<int, Dictionary<int, int>>();

		// Token: 0x04000D89 RID: 3465
		private static Dictionary<int, List<int>> masterSharedTriggIndexes = new Dictionary<int, List<int>>();

		// Token: 0x04000D8A RID: 3466
		[HideInInspector]
		public List<int> sharedTriggIndexes = new List<int>();

		// Token: 0x04000D8B RID: 3467
		private Dictionary<int, int> sharedTriggHashes;

		// Token: 0x04000D8C RID: 3468
		private static Dictionary<int, Dictionary<int, int>> masterSharedStateHashes = new Dictionary<int, Dictionary<int, int>>();

		// Token: 0x04000D8D RID: 3469
		private static Dictionary<int, List<int>> masterSharedStateIndexes = new Dictionary<int, List<int>>();

		// Token: 0x04000D8E RID: 3470
		[HideInInspector]
		public List<int> sharedStateIndexes = new List<int>();

		// Token: 0x04000D8F RID: 3471
		private Dictionary<int, int> sharedStateHashes;

		// Token: 0x04000D90 RID: 3472
		[Tooltip("The Animator to sync. If null the first animator on this game object will be found and used.")]
		public Animator animator;

		// Token: 0x04000D91 RID: 3473
		[Tooltip("Disables applyRootMotion on any non-authority instances, to avoid a tug of war between the transform sync and root motion.")]
		public bool autoRootMotion = true;

		// Token: 0x04000D92 RID: 3474
		[HideInInspector]
		public bool syncPassThrus = true;

		// Token: 0x04000D93 RID: 3475
		[HideInInspector]
		public NormalizedFloatCompression passthruNormTimeCompress = NormalizedFloatCompression.Bits10;

		// Token: 0x04000D94 RID: 3476
		[HideInInspector]
		public bool syncStates = true;

		// Token: 0x04000D95 RID: 3477
		[HideInInspector]
		public NormalizedFloatCompression normalizedTimeCompress = NormalizedFloatCompression.Bits10;

		// Token: 0x04000D96 RID: 3478
		[HideInInspector]
		public bool syncLayers = true;

		// Token: 0x04000D97 RID: 3479
		[HideInInspector]
		public bool syncLayerWeights = true;

		// Token: 0x04000D98 RID: 3480
		[HideInInspector]
		public NormalizedFloatCompression layerWeightCompress = NormalizedFloatCompression.Bits10;

		// Token: 0x04000D99 RID: 3481
		[NonSerialized]
		public int layerCount;

		// Token: 0x04000D9A RID: 3482
		[HideInInspector]
		public bool syncParams = true;

		// Token: 0x04000D9B RID: 3483
		[HideInInspector]
		public bool useGlobalParamSettings = true;

		// Token: 0x04000D9C RID: 3484
		private static Dictionary<int, ParameterDefaults> masterSharedParamDefaults = new Dictionary<int, ParameterDefaults>();

		// Token: 0x04000D9D RID: 3485
		[HideInInspector]
		public ParameterDefaults sharedParamDefaults = new ParameterDefaults();

		// Token: 0x04000D9E RID: 3486
		private static Dictionary<int, ParameterSettings[]> masterSharedParamSettings = new Dictionary<int, ParameterSettings[]>();

		// Token: 0x04000D9F RID: 3487
		[HideInInspector]
		public ParameterSettings[] sharedParamSettings = new ParameterSettings[0];

		// Token: 0x04000DA0 RID: 3488
		[HideInInspector]
		public int paramCount;

		// Token: 0x04000DA1 RID: 3489
		private int bitsForTriggerIndex;

		// Token: 0x04000DA2 RID: 3490
		private int bitsForStateIndex;

		// Token: 0x04000DA3 RID: 3491
		private int bitsForTransIndex;

		// Token: 0x04000DA4 RID: 3492
		private int bitsForLayerIndex;

		// Token: 0x04000DA5 RID: 3493
		private bool defaultRootMotion;

		// Token: 0x04000DA6 RID: 3494
		private int[] lastAnimationHash;

		// Token: 0x04000DA7 RID: 3495
		private uint[] lastLayerWeight;

		// Token: 0x04000DA8 RID: 3496
		private SmartVar[] lastSentParams;

		// Token: 0x04000DA9 RID: 3497
		private SyncAnimator.Frame currentFrame;

		// Token: 0x04000DAA RID: 3498
		public static Dictionary<int, Stack<SyncAnimator.Frame[]>> masterSharedFramePool = new Dictionary<int, Stack<SyncAnimator.Frame[]>>();

		// Token: 0x04000DAB RID: 3499
		private static List<Animator> foundAnimators = new List<Animator>();

		// Token: 0x04000DAC RID: 3500
		private static List<SyncAnimator> foundSyncs = new List<SyncAnimator>();

		// Token: 0x04000DAD RID: 3501
		private readonly Queue<AnimPassThru> passThruQueue = new Queue<AnimPassThru>(2);

		// Token: 0x020003CD RID: 973
		public class Frame : FrameBase
		{
			// Token: 0x060013D8 RID: 5080 RVA: 0x0005A3E9 File Offset: 0x000585E9
			public Frame()
			{
			}

			// Token: 0x060013D9 RID: 5081 RVA: 0x0005A454 File Offset: 0x00058654
			public Frame(SyncAnimator syncAnimator, int frameId) : base(frameId)
			{
				this.syncAnimator = syncAnimator;
				int layerCount = syncAnimator.layerCount;
				this.stateHashes = new int?[layerCount];
				this.layerIsInTransition = new bool[layerCount];
				this.normalizedTime = new float[layerCount];
				this.layerWeights = new float?[layerCount];
				this.passThrus = new Queue<AnimPassThru>(2);
				this.parameters = new SmartVar[syncAnimator.paramCount];
				int paramCount = syncAnimator.paramCount;
				for (int i = 0; i < paramCount; i++)
				{
					this.parameters[i] = syncAnimator.sharedParamSettings[i].defaultValue;
				}
			}

			// Token: 0x060013DA RID: 5082 RVA: 0x0005A4F0 File Offset: 0x000586F0
			public override void CopyFrom(FrameBase sourceFrame)
			{
				base.CopyFrom(sourceFrame);
				SyncAnimator.Frame frame = sourceFrame as SyncAnimator.Frame;
				if (this.syncAnimator.syncParams)
				{
					SmartVar[] array = frame.parameters;
					int num = array.Length;
					for (int i = 0; i < num; i++)
					{
						this.parameters[i] = array[i];
					}
				}
				if (this.syncAnimator.syncStates)
				{
					int num2 = frame.stateHashes.Length;
					for (int j = 0; j < num2; j++)
					{
						this.stateHashes[j] = frame.stateHashes[j];
						this.layerIsInTransition[j] = frame.layerIsInTransition[j];
						this.normalizedTime[j] = frame.normalizedTime[j];
						this.layerWeights[j] = frame.layerWeights[j];
					}
				}
			}

			// Token: 0x060013DB RID: 5083 RVA: 0x0005A5C4 File Offset: 0x000587C4
			public override void Clear()
			{
				base.Clear();
				int i = 0;
				int num = this.stateHashes.Length;
				while (i < num)
				{
					this.stateHashes[i] = default(int?);
					i++;
				}
				this.passThrus.Clear();
				int j = 0;
				int num2 = this.layerWeights.Length;
				while (j < num2)
				{
					this.layerWeights[j] = default(float?);
					this.stateHashes[j] = default(int?);
					this.normalizedTime[j] = 0f;
					this.layerWeights[j] = default(float?);
					j++;
				}
			}

			// Token: 0x040012F3 RID: 4851
			public SyncAnimator syncAnimator;

			// Token: 0x040012F4 RID: 4852
			public SmartVar[] parameters;

			// Token: 0x040012F5 RID: 4853
			public int?[] stateHashes;

			// Token: 0x040012F6 RID: 4854
			public bool[] layerIsInTransition;

			// Token: 0x040012F7 RID: 4855
			public float[] normalizedTime;

			// Token: 0x040012F8 RID: 4856
			public float?[] layerWeights;

			// Token: 0x040012F9 RID: 4857
			public Queue<AnimPassThru> passThrus;
		}
	}
}
