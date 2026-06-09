using System;

namespace Photon.Pun.Simple
{
	// Token: 0x020002CC RID: 716
	public interface ISerializationOptional : IOnNetSerialize
	{
		// Token: 0x17000102 RID: 258
		// (get) Token: 0x06000F62 RID: 3938
		bool IncludeInSerialization { get; }
	}
}
