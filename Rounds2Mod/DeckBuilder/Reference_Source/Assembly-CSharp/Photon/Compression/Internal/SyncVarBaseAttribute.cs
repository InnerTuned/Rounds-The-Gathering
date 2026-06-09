using System;
using System.Collections.Generic;
using UnityEngine;

namespace Photon.Compression.Internal
{
	// Token: 0x02000222 RID: 546
	[AttributeUsage(256)]
	[Serializable]
	public abstract class SyncVarBaseAttribute : Attribute
	{
		// Token: 0x06000C26 RID: 3110 RVA: 0x0003E57A File Offset: 0x0003C77A
		public virtual void Initialize(Type primitiveType)
		{
			if (this.bitCount > -1)
			{
				return;
			}
			this.bitCount = this.GetDefaultBitCount(primitiveType);
		}

		// Token: 0x06000C27 RID: 3111 RVA: 0x0003E594 File Offset: 0x0003C794
		public virtual int GetDefaultBitCount(Type fieldType)
		{
			if (fieldType == typeof(byte) || fieldType == typeof(sbyte))
			{
				return 8;
			}
			if (fieldType == typeof(ushort) || fieldType == typeof(short) || fieldType == typeof(char))
			{
				return 16;
			}
			if (fieldType == typeof(uint) || fieldType == typeof(int) || fieldType == typeof(float))
			{
				return 32;
			}
			if (fieldType == typeof(ulong) || fieldType == typeof(long) || fieldType == typeof(double))
			{
				return 64;
			}
			if (fieldType == typeof(bool))
			{
				return 1;
			}
			if (fieldType == typeof(Vector3))
			{
				return 32;
			}
			if (fieldType == typeof(Vector2))
			{
				return 32;
			}
			return 0;
		}

		// Token: 0x06000C28 RID: 3112 RVA: 0x0003E6B4 File Offset: 0x0003C8B4
		public virtual int GetMaxBits(Type fieldType)
		{
			if (fieldType == typeof(byte) || fieldType == typeof(sbyte))
			{
				return 8;
			}
			if (fieldType == typeof(ushort) || fieldType == typeof(short) || fieldType == typeof(char))
			{
				return 16;
			}
			if (fieldType == typeof(uint) || fieldType == typeof(int) || fieldType == typeof(float))
			{
				return 32;
			}
			if (fieldType == typeof(ulong) || fieldType == typeof(long) || fieldType == typeof(double))
			{
				return 64;
			}
			if (fieldType == typeof(bool))
			{
				return 1;
			}
			if (fieldType == typeof(Vector3))
			{
				return 96;
			}
			if (fieldType == typeof(Vector2))
			{
				return 64;
			}
			if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition().IsAssignableFrom(typeof(List)))
			{
				global::Debug.LogWarning("Can't get max bits needed for List<> types, as they are variable. " + fieldType.Name);
				return 2048;
			}
			global::Debug.LogWarning("Can't get bits needed for unsupported types. " + fieldType.Name);
			return 2048;
		}

		// Token: 0x06000C29 RID: 3113 RVA: 0x0003E827 File Offset: 0x0003CA27
		public bool IsKeyframe(int frameId)
		{
			return this.keyRate == KeyRate.Every || (this.keyRate != KeyRate.Never && frameId % (int)this.keyRate == 0);
		}

		// Token: 0x06000C2A RID: 3114 RVA: 0x0003E84C File Offset: 0x0003CA4C
		public bool IsForced(int frameId, SerializationFlags writeFlags)
		{
			return this.syncAs != SyncAs.Trigger && (this.keyRate == KeyRate.Every || (writeFlags & SerializationFlags.Force) != SerializationFlags.None || (this.keyRate == KeyRate.Never && (writeFlags & SerializationFlags.NewConnection) != SerializationFlags.None) || (this.keyRate != KeyRate.Never && frameId % (int)this.keyRate == 0));
		}

		// Token: 0x06000C2B RID: 3115 RVA: 0x0003E89C File Offset: 0x0003CA9C
		public bool IsForcedClass<T>(int frameId, T value, T prevValue, SerializationFlags writeFlags) where T : class
		{
			if (this.syncAs == SyncAs.Trigger)
			{
				global::Debug.LogError("Reference type " + typeof(T).Name + " cannot be set to SyncAs.Trigger. This PackAttribute setting only applies to structs.");
				return true;
			}
			return this.keyRate == KeyRate.Every || (writeFlags & SerializationFlags.Force) != SerializationFlags.None || (this.keyRate == KeyRate.Never && (writeFlags & SerializationFlags.NewConnection) != SerializationFlags.None) || (this.keyRate != KeyRate.Never && frameId % (int)this.keyRate == 0) || !value.Equals(prevValue);
		}

		// Token: 0x06000C2C RID: 3116 RVA: 0x0003E928 File Offset: 0x0003CB28
		public bool IsForced<T>(int frameId, T value, T prevValue, SerializationFlags writeFlags) where T : struct
		{
			if (this.syncAs == SyncAs.Trigger)
			{
				return !value.Equals(Activator.CreateInstance<T>());
			}
			return this.keyRate == KeyRate.Every || (writeFlags & SerializationFlags.Force) != SerializationFlags.None || (this.keyRate == KeyRate.Never && (writeFlags & SerializationFlags.NewConnection) != SerializationFlags.None) || (this.keyRate != KeyRate.Never && frameId % (int)this.keyRate == 0) || !value.Equals(prevValue);
		}

		// Token: 0x04000C45 RID: 3141
		public SyncAs syncAs;

		// Token: 0x04000C46 RID: 3142
		public KeyRate keyRate;

		// Token: 0x04000C47 RID: 3143
		public string applyCallback;

		// Token: 0x04000C48 RID: 3144
		public string snapshotCallback;

		// Token: 0x04000C49 RID: 3145
		public SetValueTiming setValueTiming = SetValueTiming.AfterCallback;

		// Token: 0x04000C4A RID: 3146
		public bool interpolate;

		// Token: 0x04000C4B RID: 3147
		public int bitCount = -1;
	}
}
