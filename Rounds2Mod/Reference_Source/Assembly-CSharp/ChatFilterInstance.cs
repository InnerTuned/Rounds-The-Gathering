using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200011C RID: 284
[Serializable]
public class ChatFilterInstance
{
	// Token: 0x0400073A RID: 1850
	public List<string> words = new List<string>();

	// Token: 0x0400073B RID: 1851
	public string category;

	// Token: 0x0400073C RID: 1852
	[TextArea]
	public string replacement;
}
