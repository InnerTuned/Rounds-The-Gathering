using System;
using System.Reflection;
using UnityEngine;

namespace Photon.Pun.UtilityScripts
{
	// Token: 0x0200030E RID: 782
	public static class CopyComponent
	{
		// Token: 0x0600109F RID: 4255 RVA: 0x00050110 File Offset: 0x0004E310
		public static Component ComponentCopy(this Component original, GameObject destination)
		{
			Type type = original.GetType();
			Component component = destination.AddComponent(type);
			foreach (FieldInfo fieldInfo in type.GetFields())
			{
				fieldInfo.SetValue(component, fieldInfo.GetValue(original));
			}
			return component;
		}

		// Token: 0x060010A0 RID: 4256 RVA: 0x00050158 File Offset: 0x0004E358
		public static T GetCopyOf<T>(this T comp, T other) where T : Component
		{
			if (comp == null || other == null)
			{
				return comp;
			}
			Type type = comp.GetType();
			if (type != other.GetType())
			{
				return default(T);
			}
			BindingFlags bindingFlags = 54;
			foreach (PropertyInfo propertyInfo in type.GetProperties(bindingFlags))
			{
				if (propertyInfo.CanWrite)
				{
					try
					{
						if (!Attribute.IsDefined(propertyInfo, typeof(ObsoleteAttribute)))
						{
							propertyInfo.SetValue(comp, propertyInfo.GetValue(other, null), null);
						}
					}
					catch
					{
					}
				}
			}
			foreach (FieldInfo fieldInfo in type.GetFields(bindingFlags))
			{
				fieldInfo.SetValue(comp, fieldInfo.GetValue(other));
			}
			return comp;
		}

		// Token: 0x060010A1 RID: 4257 RVA: 0x0005025C File Offset: 0x0004E45C
		public static T AddColliderCopy<T>(this GameObject go, T toAdd) where T : Collider
		{
			T t = go.AddComponent(toAdd.GetType()).GetCopyOf(toAdd) as T;
			t.sharedMaterial = toAdd.sharedMaterial;
			t.isTrigger = toAdd.isTrigger;
			return t;
		}
	}
}
