using System;

namespace emotitron.Utilities
{
	// Token: 0x020001EB RID: 491
	public static class NameTypeUtils
	{
		// Token: 0x060009B5 RID: 2485 RVA: 0x00031730 File Offset: 0x0002F930
		public static int GetVitalTypeForName(string name, string[] enumNames)
		{
			for (int i = 0; i < enumNames.Length; i++)
			{
				if (name == enumNames[i])
				{
					return i;
				}
			}
			return 1;
		}
	}
}
