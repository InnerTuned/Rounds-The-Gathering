using System;

namespace Photon.Compression.Internal
{
	// Token: 0x02000221 RID: 545
	public class SyncRangedAttribute : SyncVarBaseAttribute, IPackSingle
	{
		// Token: 0x06000C23 RID: 3107 RVA: 0x0003E4F6 File Offset: 0x0003C6F6
		public SyncRangedAttribute(LiteFloatCompressType compression, float min, float max, bool accurateCenter)
		{
			LiteFloatCrusher.Recalculate(compression, min, max, accurateCenter, this.crusher);
		}

		// Token: 0x06000C24 RID: 3108 RVA: 0x0003E51C File Offset: 0x0003C71C
		public SerializationFlags Pack(ref float value, float preValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			uint num = (uint)this.crusher.Encode(value);
			if (!base.IsForced(frameId, writeFlags) && num == (uint)this.crusher.Encode(preValue))
			{
				return SerializationFlags.None;
			}
			this.crusher.WriteCValue(num, buffer, ref bitposition);
			return SerializationFlags.IsComplete;
		}

		// Token: 0x06000C25 RID: 3109 RVA: 0x0003E567 File Offset: 0x0003C767
		public SerializationFlags Unpack(ref float value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			value = this.crusher.ReadValue(buffer, ref bitposition);
			return SerializationFlags.IsComplete;
		}

		// Token: 0x04000C44 RID: 3140
		private LiteFloatCrusher crusher = new LiteFloatCrusher();
	}
}
