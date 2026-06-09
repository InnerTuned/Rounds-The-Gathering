using System;
using emotitron;
using Photon.Utilities;

namespace Photon.Compression.Internal
{
	// Token: 0x0200021F RID: 543
	public class PackFrame_TestPackObject : PackFrame
	{
		// Token: 0x06000C13 RID: 3091 RVA: 0x0003DC60 File Offset: 0x0003BE60
		public static void Interpolate(PackFrame start, PackFrame end, PackFrame trg, float time, ref FastBitMask128 readyMask, ref int maskOffset)
		{
			PackFrame_TestPackObject packFrame_TestPackObject = start as PackFrame_TestPackObject;
			PackFrame_TestPackObject packFrame_TestPackObject2 = end as PackFrame_TestPackObject;
			PackFrame_TestPackObject packFrame_TestPackObject3 = trg as PackFrame_TestPackObject;
			FastBitMask128 mask = end.mask;
			if (mask[maskOffset])
			{
				packFrame_TestPackObject3.rotation = (packFrame_TestPackObject2.rotation - packFrame_TestPackObject.rotation) * time + packFrame_TestPackObject.rotation;
			}
			maskOffset++;
			if (mask[maskOffset])
			{
				packFrame_TestPackObject3.intoroboto = (int)((float)(packFrame_TestPackObject2.intoroboto - packFrame_TestPackObject.intoroboto) * time) + packFrame_TestPackObject.intoroboto;
			}
			maskOffset++;
		}

		// Token: 0x06000C14 RID: 3092 RVA: 0x0003DCEC File Offset: 0x0003BEEC
		public static void Interpolate(PackFrame start, PackFrame end, object trg, float time, ref FastBitMask128 readyMask, ref int maskOffset)
		{
			PackFrame_TestPackObject packFrame_TestPackObject = start as PackFrame_TestPackObject;
			PackFrame_TestPackObject packFrame_TestPackObject2 = end as PackFrame_TestPackObject;
			TestPackObject testPackObject = trg as TestPackObject;
			FastBitMask128 mask = end.mask;
			if (readyMask[maskOffset] && mask[maskOffset])
			{
				testPackObject.rotation = (packFrame_TestPackObject2.rotation - packFrame_TestPackObject.rotation) * time + packFrame_TestPackObject.rotation;
			}
			maskOffset++;
			maskOffset++;
		}

		// Token: 0x06000C15 RID: 3093 RVA: 0x0003DD5C File Offset: 0x0003BF5C
		public static void SnapshotCallback(PackFrame snapframe, PackFrame targframe, object trg, ref FastBitMask128 readyMask, ref int maskOffset)
		{
			PackFrame_TestPackObject packFrame_TestPackObject = snapframe as PackFrame_TestPackObject;
			PackFrame_TestPackObject packFrame_TestPackObject2 = targframe as PackFrame_TestPackObject;
			TestPackObject testPackObject = trg as TestPackObject;
			FastBitMask128 mask = snapframe.mask;
			FastBitMask128 mask2 = targframe.mask;
			if (readyMask[maskOffset])
			{
				float num = mask[maskOffset] ? packFrame_TestPackObject.rotation : testPackObject.rotation;
				float targ = mask2[maskOffset] ? packFrame_TestPackObject2.rotation : num;
				testPackObject.SnapshotHook(num, targ);
			}
			maskOffset++;
			maskOffset++;
		}

		// Token: 0x06000C16 RID: 3094 RVA: 0x0003DDE8 File Offset: 0x0003BFE8
		public static void Capture(object src, PackFrame trg)
		{
			TestPackObject testPackObject = src as TestPackObject;
			PackFrame_TestPackObject packFrame_TestPackObject = trg as PackFrame_TestPackObject;
			packFrame_TestPackObject.rotation = testPackObject.rotation;
			packFrame_TestPackObject.intoroboto = testPackObject.intoroboto;
		}

		// Token: 0x06000C17 RID: 3095 RVA: 0x0003DE1C File Offset: 0x0003C01C
		public static void Apply(PackFrame src, object trg, ref FastBitMask128 mask, ref int maskOffset)
		{
			PackFrame_TestPackObject packFrame_TestPackObject = src as PackFrame_TestPackObject;
			TestPackObject testPackObject = trg as TestPackObject;
			if (mask[maskOffset])
			{
				float oldrot = testPackObject.rotation;
				testPackObject.rotation = packFrame_TestPackObject.rotation;
				testPackObject.RotationHook(packFrame_TestPackObject.rotation, oldrot);
			}
			maskOffset++;
			if (mask[maskOffset])
			{
				testPackObject.intoroboto = packFrame_TestPackObject.intoroboto;
			}
			maskOffset++;
		}

		// Token: 0x06000C18 RID: 3096 RVA: 0x0003DE84 File Offset: 0x0003C084
		public static void Copy(PackFrame src, PackFrame trg)
		{
			PackFrame_TestPackObject packFrame_TestPackObject = src as PackFrame_TestPackObject;
			PackFrame_TestPackObject packFrame_TestPackObject2 = trg as PackFrame_TestPackObject;
			packFrame_TestPackObject2.rotation = packFrame_TestPackObject.rotation;
			packFrame_TestPackObject2.intoroboto = packFrame_TestPackObject.intoroboto;
		}

		// Token: 0x06000C19 RID: 3097 RVA: 0x0003DEB5 File Offset: 0x0003C0B5
		public static PackFrame Factory()
		{
			return new PackFrame_TestPackObject
			{
				mask = new FastBitMask128(2),
				isCompleteMask = new FastBitMask128(2)
			};
		}

		// Token: 0x06000C1A RID: 3098 RVA: 0x0003DED4 File Offset: 0x0003C0D4
		public static PackFrame[] Factory(object trg, int count)
		{
			PackFrame_TestPackObject[] array = new PackFrame_TestPackObject[count];
			for (int i = 0; i < count; i++)
			{
				PackFrame_TestPackObject packFrame_TestPackObject = new PackFrame_TestPackObject
				{
					mask = new FastBitMask128(2),
					isCompleteMask = new FastBitMask128(2)
				};
				array[i] = packFrame_TestPackObject;
			}
			return array;
		}

		// Token: 0x04000C39 RID: 3129
		public float rotation;

		// Token: 0x04000C3A RID: 3130
		public int intoroboto;
	}
}
