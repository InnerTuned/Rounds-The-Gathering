using System;
using emotitron;
using Photon.Utilities;
using UnityEngine;

namespace Photon.Compression.Internal
{
	// Token: 0x02000220 RID: 544
	public static class Pack_TestPackObject
	{
		// Token: 0x06000C1C RID: 3100 RVA: 0x0003DF24 File Offset: 0x0003C124
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		public static void Initialize()
		{
			if (Pack_TestPackObject.initialized)
			{
				return;
			}
			Pack_TestPackObject.isInitializing = true;
			int num = 0;
			PackObjectAttribute packObjectAttribute = typeof(TestPackObject).GetCustomAttributes(typeof(PackObjectAttribute), false)[0] as PackObjectAttribute;
			DefaultKeyRate defaultKeyRate = packObjectAttribute.defaultKeyRate;
			FastBitMask128 defaultReadyMask = new FastBitMask128(2);
			int num2 = 0;
			SyncHalfFloatAttribute syncHalfFloatAttribute = typeof(TestPackObject).GetField("rotation").GetCustomAttributes(typeof(SyncVarBaseAttribute), false)[0] as SyncHalfFloatAttribute;
			Pack_TestPackObject.rotationPacker = new PackDelegate<float>(syncHalfFloatAttribute.Pack);
			Pack_TestPackObject.rotationUnpacker = new UnpackDelegate<float>(syncHalfFloatAttribute.Unpack);
			syncHalfFloatAttribute.Initialize(typeof(float));
			if (syncHalfFloatAttribute.keyRate == KeyRate.UseDefault)
			{
				syncHalfFloatAttribute.keyRate = (KeyRate)defaultKeyRate;
			}
			if (syncHalfFloatAttribute.syncAs == SyncAs.Auto)
			{
				syncHalfFloatAttribute.syncAs = packObjectAttribute.syncAs;
			}
			if (syncHalfFloatAttribute.syncAs == SyncAs.Auto)
			{
				syncHalfFloatAttribute.syncAs = SyncAs.State;
			}
			if (syncHalfFloatAttribute.syncAs == SyncAs.Trigger)
			{
				defaultReadyMask[num2] = true;
			}
			num += 16;
			num2++;
			SyncRangedIntAttribute syncRangedIntAttribute = typeof(TestPackObject).GetField("intoroboto").GetCustomAttributes(typeof(SyncVarBaseAttribute), false)[0] as SyncRangedIntAttribute;
			Pack_TestPackObject.intorobotoPacker = new PackDelegate<int>(syncRangedIntAttribute.Pack);
			Pack_TestPackObject.intorobotoUnpacker = new UnpackDelegate<int>(syncRangedIntAttribute.Unpack);
			syncRangedIntAttribute.Initialize(typeof(int));
			if (syncRangedIntAttribute.keyRate == KeyRate.UseDefault)
			{
				syncRangedIntAttribute.keyRate = (KeyRate)defaultKeyRate;
			}
			if (syncRangedIntAttribute.syncAs == SyncAs.Auto)
			{
				syncRangedIntAttribute.syncAs = packObjectAttribute.syncAs;
			}
			if (syncRangedIntAttribute.syncAs == SyncAs.Auto)
			{
				syncRangedIntAttribute.syncAs = SyncAs.State;
			}
			if (syncRangedIntAttribute.syncAs == SyncAs.Trigger)
			{
				defaultReadyMask[num2] = true;
			}
			num += 2;
			num2++;
			Pack_TestPackObject.packObjInfo = new PackObjectDatabase.PackObjectInfo(defaultReadyMask, new PackObjectDatabase.PackObjDelegate(Pack_TestPackObject.Pack), new PackObjectDatabase.PackFrameDelegate(Pack_TestPackObject.Pack), new PackObjectDatabase.UnpackFrameDelegate(Pack_TestPackObject.Unpack), num, new Func<PackFrame>(PackFrame_TestPackObject.Factory), new Func<object, int, PackFrame[]>(PackFrame_TestPackObject.Factory), new PackObjectDatabase.PackCopyFrameToObjectDelegate(PackFrame_TestPackObject.Apply), new Action<object, PackFrame>(PackFrame_TestPackObject.Capture), new Action<PackFrame, PackFrame>(PackFrame_TestPackObject.Copy), new PackObjectDatabase.PackSnapshotObjectDelegate(PackFrame_TestPackObject.SnapshotCallback), new PackObjectDatabase.PackInterpFrameToFrameDelegate(PackFrame_TestPackObject.Interpolate), new PackObjectDatabase.PackInterpFrameToObjectDelegate(PackFrame_TestPackObject.Interpolate), 2);
			PackObjectDatabase.packObjInfoLookup.Add(typeof(TestPackObject), Pack_TestPackObject.packObjInfo);
			Pack_TestPackObject.isInitializing = false;
			Pack_TestPackObject.initialized = true;
		}

		// Token: 0x06000C1D RID: 3101 RVA: 0x0003E1A4 File Offset: 0x0003C3A4
		public static SerializationFlags Pack(ref object obj, PackFrame prevFrame, ref FastBitMask128 mask, ref int maskOffset, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			TestPackObject testPackObject = obj as TestPackObject;
			PackFrame_TestPackObject packFrame_TestPackObject = prevFrame as PackFrame_TestPackObject;
			SerializationFlags serializationFlags = SerializationFlags.None;
			SerializationFlags serializationFlags2 = Pack_TestPackObject.rotationPacker(ref testPackObject.rotation, packFrame_TestPackObject.rotation, buffer, ref bitposition, frameId, writeFlags);
			mask[maskOffset] = (serializationFlags2 > SerializationFlags.None);
			SerializationFlags serializationFlags3 = serializationFlags | serializationFlags2;
			maskOffset++;
			SerializationFlags serializationFlags4 = Pack_TestPackObject.intorobotoPacker(ref testPackObject.intoroboto, packFrame_TestPackObject.intoroboto, buffer, ref bitposition, frameId, writeFlags);
			mask[maskOffset] = (serializationFlags4 > SerializationFlags.None);
			SerializationFlags result = serializationFlags3 | serializationFlags4;
			maskOffset++;
			return result;
		}

		// Token: 0x06000C1E RID: 3102 RVA: 0x0003E228 File Offset: 0x0003C428
		public static SerializationFlags Pack(ref TestPackObject packable, PackFrame prevFrame, ref FastBitMask128 mask, ref int maskOffset, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			PackFrame_TestPackObject packFrame_TestPackObject = prevFrame as PackFrame_TestPackObject;
			SerializationFlags serializationFlags = SerializationFlags.None;
			SerializationFlags serializationFlags2 = Pack_TestPackObject.rotationPacker(ref packable.rotation, packFrame_TestPackObject.rotation, buffer, ref bitposition, frameId, writeFlags);
			mask[maskOffset] = (serializationFlags2 > SerializationFlags.None);
			SerializationFlags serializationFlags3 = serializationFlags | serializationFlags2;
			maskOffset++;
			SerializationFlags serializationFlags4 = Pack_TestPackObject.intorobotoPacker(ref packable.intoroboto, packFrame_TestPackObject.intoroboto, buffer, ref bitposition, frameId, writeFlags);
			mask[maskOffset] = (serializationFlags4 > SerializationFlags.None);
			SerializationFlags result = serializationFlags3 | serializationFlags4;
			maskOffset++;
			return result;
		}

		// Token: 0x06000C1F RID: 3103 RVA: 0x0003E2A8 File Offset: 0x0003C4A8
		public static SerializationFlags Pack(PackFrame obj, PackFrame prevFrame, ref FastBitMask128 mask, ref int maskOffset, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			PackFrame_TestPackObject packFrame_TestPackObject = obj as PackFrame_TestPackObject;
			PackFrame_TestPackObject packFrame_TestPackObject2 = prevFrame as PackFrame_TestPackObject;
			SerializationFlags serializationFlags = SerializationFlags.None;
			SerializationFlags serializationFlags2 = Pack_TestPackObject.rotationPacker(ref packFrame_TestPackObject.rotation, packFrame_TestPackObject2.rotation, buffer, ref bitposition, frameId, writeFlags);
			mask[maskOffset] = (serializationFlags2 > SerializationFlags.None);
			SerializationFlags serializationFlags3 = serializationFlags | serializationFlags2;
			maskOffset++;
			SerializationFlags serializationFlags4 = Pack_TestPackObject.intorobotoPacker(ref packFrame_TestPackObject.intoroboto, packFrame_TestPackObject2.intoroboto, buffer, ref bitposition, frameId, writeFlags);
			mask[maskOffset] = (serializationFlags4 > SerializationFlags.None);
			SerializationFlags result = serializationFlags3 | serializationFlags4;
			maskOffset++;
			return result;
		}

		// Token: 0x06000C20 RID: 3104 RVA: 0x0003E32C File Offset: 0x0003C52C
		public static SerializationFlags Unpack(PackFrame obj, ref FastBitMask128 mask, ref FastBitMask128 isCompleteMask, ref int maskOffset, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			PackFrame_TestPackObject packFrame_TestPackObject = obj as PackFrame_TestPackObject;
			SerializationFlags serializationFlags = SerializationFlags.None;
			if (mask[maskOffset])
			{
				SerializationFlags serializationFlags2 = Pack_TestPackObject.rotationUnpacker(ref packFrame_TestPackObject.rotation, buffer, ref bitposition, frameId, writeFlags);
				isCompleteMask[maskOffset] = ((serializationFlags2 & SerializationFlags.IsComplete) > SerializationFlags.None);
				mask[maskOffset] = (serializationFlags2 > SerializationFlags.None);
				serializationFlags |= serializationFlags2;
			}
			maskOffset++;
			if (mask[maskOffset])
			{
				SerializationFlags serializationFlags3 = Pack_TestPackObject.intorobotoUnpacker(ref packFrame_TestPackObject.intoroboto, buffer, ref bitposition, frameId, writeFlags);
				isCompleteMask[maskOffset] = ((serializationFlags3 & SerializationFlags.IsComplete) > SerializationFlags.None);
				mask[maskOffset] = (serializationFlags3 > SerializationFlags.None);
				serializationFlags |= serializationFlags3;
			}
			maskOffset++;
			return serializationFlags;
		}

		// Token: 0x06000C21 RID: 3105 RVA: 0x0003E3D4 File Offset: 0x0003C5D4
		public static SerializationFlags Pack(ref PackFrame_TestPackObject packable, PackFrame prevFrame, ref FastBitMask128 mask, ref int maskOffset, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			PackFrame_TestPackObject packFrame_TestPackObject = prevFrame as PackFrame_TestPackObject;
			SerializationFlags serializationFlags = SerializationFlags.None;
			SerializationFlags serializationFlags2 = Pack_TestPackObject.rotationPacker(ref packable.rotation, packFrame_TestPackObject.rotation, buffer, ref bitposition, frameId, writeFlags);
			mask[maskOffset] = (serializationFlags2 > SerializationFlags.None);
			SerializationFlags serializationFlags3 = serializationFlags | serializationFlags2;
			maskOffset++;
			SerializationFlags serializationFlags4 = Pack_TestPackObject.intorobotoPacker(ref packable.intoroboto, packFrame_TestPackObject.intoroboto, buffer, ref bitposition, frameId, writeFlags);
			mask[maskOffset] = (serializationFlags4 > SerializationFlags.None);
			SerializationFlags result = serializationFlags3 | serializationFlags4;
			maskOffset++;
			return result;
		}

		// Token: 0x06000C22 RID: 3106 RVA: 0x0003E454 File Offset: 0x0003C654
		public static SerializationFlags Unpack(ref PackFrame_TestPackObject packable, ref FastBitMask128 mask, ref FastBitMask128 isCompleteMask, ref int maskOffset, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			SerializationFlags serializationFlags = SerializationFlags.None;
			if (mask[maskOffset])
			{
				SerializationFlags serializationFlags2 = Pack_TestPackObject.rotationUnpacker(ref packable.rotation, buffer, ref bitposition, frameId, writeFlags);
				isCompleteMask[maskOffset] = ((serializationFlags2 & SerializationFlags.IsComplete) > SerializationFlags.None);
				mask[maskOffset] = (serializationFlags2 > SerializationFlags.None);
				serializationFlags |= serializationFlags2;
			}
			maskOffset++;
			if (mask[maskOffset])
			{
				SerializationFlags serializationFlags3 = Pack_TestPackObject.intorobotoUnpacker(ref packable.intoroboto, buffer, ref bitposition, frameId, writeFlags);
				isCompleteMask[maskOffset] = ((serializationFlags3 & SerializationFlags.IsComplete) > SerializationFlags.None);
				mask[maskOffset] = (serializationFlags3 > SerializationFlags.None);
				serializationFlags |= serializationFlags3;
			}
			maskOffset++;
			return serializationFlags;
		}

		// Token: 0x04000C3B RID: 3131
		public const int LOCAL_FIELDS = 2;

		// Token: 0x04000C3C RID: 3132
		public const int TOTAL_FIELDS = 2;

		// Token: 0x04000C3D RID: 3133
		public static PackObjectDatabase.PackObjectInfo packObjInfo;

		// Token: 0x04000C3E RID: 3134
		private static PackDelegate<float> rotationPacker;

		// Token: 0x04000C3F RID: 3135
		private static UnpackDelegate<float> rotationUnpacker;

		// Token: 0x04000C40 RID: 3136
		private static PackDelegate<int> intorobotoPacker;

		// Token: 0x04000C41 RID: 3137
		private static UnpackDelegate<int> intorobotoUnpacker;

		// Token: 0x04000C42 RID: 3138
		public static bool initialized;

		// Token: 0x04000C43 RID: 3139
		public static bool isInitializing;
	}
}
