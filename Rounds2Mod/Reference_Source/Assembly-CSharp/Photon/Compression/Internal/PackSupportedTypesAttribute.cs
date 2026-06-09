using System;

namespace Photon.Compression.Internal
{
	// Token: 0x02000223 RID: 547
	[AttributeUsage(1024)]
	public class PackSupportedTypesAttribute : Attribute
	{
		// Token: 0x06000C2E RID: 3118 RVA: 0x0003E9BF File Offset: 0x0003CBBF
		public PackSupportedTypesAttribute(Type supportedType)
		{
			this.supportedType = supportedType;
		}

		// Token: 0x04000C4C RID: 3148
		public Type supportedType;
	}
}
