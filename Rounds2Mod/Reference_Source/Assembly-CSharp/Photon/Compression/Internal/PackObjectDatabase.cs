using System;
using System.Collections.Generic;
using Photon.Utilities;

namespace Photon.Compression.Internal
{
	// Token: 0x0200023D RID: 573
	public static class PackObjectDatabase
	{
		// Token: 0x06000C6A RID: 3178 RVA: 0x0003E9D0 File Offset: 0x0003CBD0
		public static PackObjectDatabase.PackObjectInfo GetPackObjectInfo(Type type)
		{
			PackObjectDatabase.PackObjectInfo result;
			if (PackObjectDatabase.packObjInfoLookup.TryGetValue(type, ref result))
			{
				return result;
			}
			Type type2 = Type.GetType("Pack_" + type.Name);
			if (type2 != null)
			{
				Debug.LogError("BRUTE FORCE Pack_" + type.Name + ". This shouldn't happen.");
				type2.GetMethod("Initialize").Invoke(null, null);
			}
			if (PackObjectDatabase.packObjInfoLookup.TryGetValue(type, ref result))
			{
				return result;
			}
			return null;
		}

		// Token: 0x04000C4F RID: 3151
		public static Dictionary<Type, PackObjectDatabase.PackObjectInfo> packObjInfoLookup = new Dictionary<Type, PackObjectDatabase.PackObjectInfo>();

		// Token: 0x020003B7 RID: 951
		// (Invoke) Token: 0x0600139B RID: 5019
		public delegate SerializationFlags PackStructDelegate(IntPtr obj, PackFrame prevFrame, ref FastBitMask128 mask, ref int maskOffset, byte[] buffer, ref int bitposition, int frameId, SerializationFlags forceKeyframe);

		// Token: 0x020003B8 RID: 952
		// (Invoke) Token: 0x0600139F RID: 5023
		public delegate SerializationFlags PackObjDelegate(ref object obj, PackFrame prevFrame, ref FastBitMask128 mask, ref int maskOffset, byte[] buffer, ref int bitposition, int frameId, SerializationFlags forceKeyframe);

		// Token: 0x020003B9 RID: 953
		// (Invoke) Token: 0x060013A3 RID: 5027
		public delegate SerializationFlags PackFrameDelegate(PackFrame obj, PackFrame prevFrame, ref FastBitMask128 mask, ref int maskOffset, byte[] buffer, ref int bitposition, int frameId, SerializationFlags forceKeyframe);

		// Token: 0x020003BA RID: 954
		// (Invoke) Token: 0x060013A7 RID: 5031
		public delegate SerializationFlags UnpackFrameDelegate(PackFrame obj, ref FastBitMask128 hasContentMask, ref FastBitMask128 isCompleteMask, ref int maskOffset, byte[] buffer, ref int bitposition, int frameId, SerializationFlags forceKeyframe);

		// Token: 0x020003BB RID: 955
		// (Invoke) Token: 0x060013AB RID: 5035
		public delegate void PackCopyFrameToObjectDelegate(PackFrame src, object trg, ref FastBitMask128 mask, ref int maskOffset);

		// Token: 0x020003BC RID: 956
		// (Invoke) Token: 0x060013AF RID: 5039
		public delegate void PackCopyFrameToStructDelegate(PackFrame src, IntPtr trg, ref FastBitMask128 mask, ref int maskOffset);

		// Token: 0x020003BD RID: 957
		// (Invoke) Token: 0x060013B3 RID: 5043
		public delegate void PackSnapshotObjectDelegate(PackFrame snap, PackFrame targ, object trg, ref FastBitMask128 readyMask, ref int maskOffset);

		// Token: 0x020003BE RID: 958
		// (Invoke) Token: 0x060013B7 RID: 5047
		public delegate void PackSnapshotStructDelegate(PackFrame snap, PackFrame targ, IntPtr trg, ref FastBitMask128 readyMask, ref int maskOffset);

		// Token: 0x020003BF RID: 959
		// (Invoke) Token: 0x060013BB RID: 5051
		public delegate void PackInterpFrameToFrameDelegate(PackFrame start, PackFrame end, PackFrame trg, float ntime, ref FastBitMask128 readyMask, ref int maskOffset);

		// Token: 0x020003C0 RID: 960
		// (Invoke) Token: 0x060013BF RID: 5055
		public delegate void PackInterpFrameToObjectDelegate(PackFrame start, PackFrame end, object trg, float ntime, ref FastBitMask128 readyMask, ref int maskOffset);

		// Token: 0x020003C1 RID: 961
		// (Invoke) Token: 0x060013C3 RID: 5059
		public delegate void PackInterpFrameToStructDelegate(PackFrame start, PackFrame end, IntPtr trg, float ntime, ref FastBitMask128 readyMask, ref int maskOffset);

		// Token: 0x020003C2 RID: 962
		public class PackObjectInfo
		{
			// Token: 0x060013C6 RID: 5062 RVA: 0x0005A1F8 File Offset: 0x000583F8
			public PackObjectInfo(FastBitMask128 defaultReadyMask, PackObjectDatabase.PackObjDelegate packObjToBuffer, PackObjectDatabase.PackFrameDelegate packFrameToBuffer, PackObjectDatabase.UnpackFrameDelegate unpackFrameFromBuffer, int maxBits, Func<PackFrame> factoryFrame, Func<object, int, PackFrame[]> factoryFramesObj, PackObjectDatabase.PackCopyFrameToObjectDelegate copyFrameToObj, Action<object, PackFrame> captureObj, Action<PackFrame, PackFrame> copyFrameToFrame, PackObjectDatabase.PackSnapshotObjectDelegate snapObject, PackObjectDatabase.PackInterpFrameToFrameDelegate interpFrameToFrame, PackObjectDatabase.PackInterpFrameToObjectDelegate interpFrameToObj, int fieldCount)
			{
				this.PackObjToBuffer = packObjToBuffer;
				this.defaultReadyMask = defaultReadyMask;
				this.PackFrameToBuffer = packFrameToBuffer;
				this.UnpackFrameFromBuffer = unpackFrameFromBuffer;
				this.maxBits = maxBits;
				this.maxBytes = maxBits + 7 >> 3;
				this.FactoryFrame = factoryFrame;
				this.FactoryFramesObj = factoryFramesObj;
				this.CopyFrameToObj = copyFrameToObj;
				this.CaptureObj = captureObj;
				this.CopyFrameToFrame = copyFrameToFrame;
				this.SnapObject = snapObject;
				this.InterpFrameToFrame = interpFrameToFrame;
				this.InterpFrameToObj = interpFrameToObj;
				this.fieldCount = fieldCount;
			}

			// Token: 0x060013C7 RID: 5063 RVA: 0x0005A284 File Offset: 0x00058484
			public PackObjectInfo(FastBitMask128 defaultReadyMask, PackObjectDatabase.PackStructDelegate packStructToBuffer, PackObjectDatabase.PackFrameDelegate packFrameToBuffer, PackObjectDatabase.UnpackFrameDelegate unpackFrameFromBuffer, int maxBits, Func<PackFrame> factoryFrame, Func<IntPtr, int, PackFrame[]> factoryFramesStruct, PackObjectDatabase.PackCopyFrameToStructDelegate copyFrameToStruct, Action<IntPtr, PackFrame> captureStruct, Action<PackFrame, PackFrame> copyFrameToFrame, PackObjectDatabase.PackSnapshotStructDelegate snapStruct, PackObjectDatabase.PackInterpFrameToFrameDelegate interpFrameToFrame, PackObjectDatabase.PackInterpFrameToStructDelegate interpFrameToStruct, int fieldCount)
			{
				this.defaultReadyMask = defaultReadyMask;
				this.PackStructToBuffer = packStructToBuffer;
				this.PackFrameToBuffer = packFrameToBuffer;
				this.UnpackFrameFromBuffer = unpackFrameFromBuffer;
				this.maxBits = maxBits;
				this.maxBytes = maxBits + 7 >> 3;
				this.FactoryFrame = factoryFrame;
				this.FactoryFramesStruct = factoryFramesStruct;
				this.CopyFrameToStruct = copyFrameToStruct;
				this.CaptureStruct = captureStruct;
				this.CopyFrameToFrame = copyFrameToFrame;
				this.SnapStruct = snapStruct;
				this.InterpFrameToFrame = interpFrameToFrame;
				this.InterpFrameToStruct = interpFrameToStruct;
				this.fieldCount = fieldCount;
			}

			// Token: 0x040012C0 RID: 4800
			public readonly Type packFrameType;

			// Token: 0x040012C1 RID: 4801
			public readonly int maxBits;

			// Token: 0x040012C2 RID: 4802
			public readonly int maxBytes;

			// Token: 0x040012C3 RID: 4803
			public readonly FastBitMask128 defaultReadyMask;

			// Token: 0x040012C4 RID: 4804
			public readonly PackObjectDatabase.PackObjDelegate PackObjToBuffer;

			// Token: 0x040012C5 RID: 4805
			public readonly PackObjectDatabase.PackStructDelegate PackStructToBuffer;

			// Token: 0x040012C6 RID: 4806
			public readonly PackObjectDatabase.PackFrameDelegate PackFrameToBuffer;

			// Token: 0x040012C7 RID: 4807
			public readonly PackObjectDatabase.UnpackFrameDelegate UnpackFrameFromBuffer;

			// Token: 0x040012C8 RID: 4808
			public Func<PackFrame> FactoryFrame;

			// Token: 0x040012C9 RID: 4809
			public Func<object, int, PackFrame[]> FactoryFramesObj;

			// Token: 0x040012CA RID: 4810
			public Func<IntPtr, int, PackFrame[]> FactoryFramesStruct;

			// Token: 0x040012CB RID: 4811
			public PackObjectDatabase.PackCopyFrameToObjectDelegate CopyFrameToObj;

			// Token: 0x040012CC RID: 4812
			public PackObjectDatabase.PackCopyFrameToStructDelegate CopyFrameToStruct;

			// Token: 0x040012CD RID: 4813
			public PackObjectDatabase.PackSnapshotObjectDelegate SnapObject;

			// Token: 0x040012CE RID: 4814
			public PackObjectDatabase.PackSnapshotStructDelegate SnapStruct;

			// Token: 0x040012CF RID: 4815
			public PackObjectDatabase.PackInterpFrameToFrameDelegate InterpFrameToFrame;

			// Token: 0x040012D0 RID: 4816
			public PackObjectDatabase.PackInterpFrameToObjectDelegate InterpFrameToObj;

			// Token: 0x040012D1 RID: 4817
			public PackObjectDatabase.PackInterpFrameToStructDelegate InterpFrameToStruct;

			// Token: 0x040012D2 RID: 4818
			public Action<object, PackFrame> CaptureObj;

			// Token: 0x040012D3 RID: 4819
			public Action<IntPtr, PackFrame> CaptureStruct;

			// Token: 0x040012D4 RID: 4820
			public Action<PackFrame, PackFrame> CopyFrameToFrame;

			// Token: 0x040012D5 RID: 4821
			public readonly int fieldCount;
		}
	}
}
