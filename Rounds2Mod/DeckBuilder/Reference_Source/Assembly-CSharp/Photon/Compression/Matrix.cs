using System;
using Photon.Utilities;
using UnityEngine;

namespace Photon.Compression
{
	// Token: 0x02000215 RID: 533
	public class Matrix
	{
		// Token: 0x06000B76 RID: 2934 RVA: 0x00003CCC File Offset: 0x00001ECC
		public Matrix()
		{
		}

		// Token: 0x06000B77 RID: 2935 RVA: 0x0003ADB8 File Offset: 0x00038FB8
		public Matrix(TransformCrusher crusher)
		{
			this.crusher = crusher;
		}

		// Token: 0x06000B78 RID: 2936 RVA: 0x0003ADC7 File Offset: 0x00038FC7
		public Matrix(TransformCrusher crusher, Vector3 position, Element rotation, Vector3 scale)
		{
			this.crusher = crusher;
			this.position = position;
			this.scale = scale;
			this.rotation = rotation;
		}

		// Token: 0x06000B79 RID: 2937 RVA: 0x0003ADEC File Offset: 0x00038FEC
		public Matrix(TransformCrusher crusher, Transform transform)
		{
			this.crusher = crusher;
			this.position = ((crusher == null || crusher.PosCrusher == null || crusher.PosCrusher.local) ? transform.localPosition : transform.position);
			this.scale = ((crusher == null || crusher.SclCrusher == null || crusher.SclCrusher.local) ? transform.localScale : transform.lossyScale);
			bool flag = crusher == null || crusher.RotCrusher == null || crusher.RotCrusher.local;
			if (crusher != null && crusher.RotCrusher != null && crusher.RotCrusher.TRSType == 2)
			{
				this.rotation = (flag ? transform.localRotation : transform.rotation);
				return;
			}
			this.rotation = (flag ? transform.localEulerAngles : transform.eulerAngles);
		}

		// Token: 0x06000B7A RID: 2938 RVA: 0x0003AF04 File Offset: 0x00039104
		public void Set(TransformCrusher crusher, Vector3 position, Element rotation, Vector3 scale)
		{
			this.crusher = crusher;
			this.position = position;
			this.scale = scale;
			this.rotation = rotation;
		}

		// Token: 0x06000B7B RID: 2939 RVA: 0x0003AF23 File Offset: 0x00039123
		[Obsolete("Use Capture() instead. Set was confusing with other usage.")]
		public void Set(TransformCrusher crusher, Transform transform)
		{
			this.Capture(crusher, transform);
		}

		// Token: 0x06000B7C RID: 2940 RVA: 0x0003AF30 File Offset: 0x00039130
		public void Capture(TransformCrusher crusher, Transform transform)
		{
			this.crusher = crusher;
			this.position = transform.position;
			this.scale = transform.localScale;
			if (crusher != null && crusher.RotCrusher != null && crusher.RotCrusher.TRSType == 2)
			{
				this.rotation = transform.rotation;
				return;
			}
			this.rotation = transform.eulerAngles;
		}

		// Token: 0x06000B7D RID: 2941 RVA: 0x0003AFA4 File Offset: 0x000391A4
		[Obsolete("Use Capture() instead. Set was confusing with other usage.")]
		public void Set(Transform transform)
		{
			this.Capture(transform);
		}

		// Token: 0x06000B7E RID: 2942 RVA: 0x0003AFB0 File Offset: 0x000391B0
		public void Capture(Transform transform)
		{
			this.position = transform.position;
			this.scale = transform.localScale;
			if (this.crusher != null && this.crusher.RotCrusher != null && this.crusher.RotCrusher.TRSType == 2)
			{
				this.rotation = transform.rotation;
				return;
			}
			this.rotation = transform.eulerAngles;
		}

		// Token: 0x06000B7F RID: 2943 RVA: 0x0003B02C File Offset: 0x0003922C
		[Obsolete("Use Capture() instead. Set was confusing with other usage.")]
		public void Set(TransformCrusher crusher, Rigidbody rb)
		{
			this.Capture(crusher, rb);
		}

		// Token: 0x06000B80 RID: 2944 RVA: 0x0003B038 File Offset: 0x00039238
		public void Capture(TransformCrusher crusher, Rigidbody rb)
		{
			this.crusher = crusher;
			this.position = rb.position;
			this.scale = rb.transform.localScale;
			if (crusher != null && crusher.RotCrusher != null && crusher.RotCrusher.TRSType == 2)
			{
				this.rotation = rb.rotation;
				return;
			}
			this.rotation = rb.rotation.eulerAngles;
		}

		// Token: 0x06000B81 RID: 2945 RVA: 0x0003B0B9 File Offset: 0x000392B9
		[Obsolete("Use Capture() instead. Set was confusing with other usage.")]
		public void Set(Rigidbody rb)
		{
			this.Capture(rb);
		}

		// Token: 0x06000B82 RID: 2946 RVA: 0x0003B0C4 File Offset: 0x000392C4
		public void Capture(Rigidbody rb)
		{
			this.position = rb.position;
			if (this.crusher != null && this.crusher.RotCrusher != null && this.crusher.RotCrusher.TRSType == 2)
			{
				this.rotation = rb.rotation;
			}
			else
			{
				this.rotation = rb.rotation.eulerAngles;
			}
			this.scale = rb.transform.localScale;
		}

		// Token: 0x06000B83 RID: 2947 RVA: 0x0003B14E File Offset: 0x0003934E
		public void Clear()
		{
			this.crusher = null;
		}

		// Token: 0x06000B84 RID: 2948 RVA: 0x0003B157 File Offset: 0x00039357
		public void Compress(CompressedMatrix nonalloc)
		{
			this.crusher.Compress(nonalloc, this);
		}

		// Token: 0x06000B85 RID: 2949 RVA: 0x0003B166 File Offset: 0x00039366
		[Obsolete("Supply the transform to Apply to. Default Transform has been deprecated to allow shared TransformCrushers.")]
		public void Apply()
		{
			this.crusher.Apply(this);
		}

		// Token: 0x06000B86 RID: 2950 RVA: 0x0003B174 File Offset: 0x00039374
		public void Apply(Transform t)
		{
			if (this.crusher == null)
			{
				global::Debug.LogError("No crusher defined for this matrix. This matrix has not yet had a value assigned to it most likely, but you are trying to apply it to a transform.");
				return;
			}
			this.crusher.Apply(t, this);
		}

		// Token: 0x06000B87 RID: 2951 RVA: 0x0003B19C File Offset: 0x0003939C
		[Obsolete("Apply for Rigidbody has been replaced with Move and Set, to indicate usage of MovePosition/Rotation vs rb.position/rotation.")]
		public void Apply(Rigidbody rb)
		{
			if (this.crusher == null)
			{
				global::Debug.LogError("No crusher defined for this matrix. This matrix has not yet had a value assigned to it most likely, but you are trying to apply it to a transform.");
				return;
			}
			this.crusher.Apply(rb, this);
		}

		// Token: 0x06000B88 RID: 2952 RVA: 0x0003B1C4 File Offset: 0x000393C4
		public static Matrix Lerp(Matrix target, Matrix start, Matrix end, float t)
		{
			TransformCrusher transformCrusher = end.crusher;
			target.crusher = transformCrusher;
			target.position = Vector3.Lerp(start.position, end.position, t);
			if (transformCrusher != null && transformCrusher.RotCrusher != null)
			{
				if (transformCrusher.RotCrusher.TRSType == 2)
				{
					target.rotation = Quaternion.Slerp((Quaternion)start.rotation, (Quaternion)end.rotation, t);
				}
				else
				{
					Vector3 vector = (Vector3)start.rotation;
					Vector3 vector2 = (Vector3)end.rotation;
					float num = vector.y - vector2.y;
					float num2 = vector.z - vector2.z;
					Vector3 b = new Vector3(vector2.x, (num > 180f) ? (vector2.y + 360f) : ((num < -180f) ? (vector2.y - 360f) : vector2.y), (num2 > 180f) ? (vector2.z + 360f) : ((num2 < -180f) ? (vector2.z - 360f) : vector2.z));
					target.rotation = Vector3.Lerp(vector, b, t);
				}
			}
			else
			{
				target.rotation = end.rotation;
			}
			target.scale = Vector3.Lerp(start.scale, end.scale, t);
			return target;
		}

		// Token: 0x06000B89 RID: 2953 RVA: 0x0003B334 File Offset: 0x00039534
		public static Matrix LerpUnclamped(Matrix target, Matrix start, Matrix end, float t)
		{
			TransformCrusher transformCrusher = end.crusher;
			target.crusher = transformCrusher;
			target.position = Vector3.LerpUnclamped(start.position, end.position, t);
			if (transformCrusher != null && transformCrusher.RotCrusher != null)
			{
				if (transformCrusher.RotCrusher.TRSType == 2)
				{
					target.rotation = Quaternion.SlerpUnclamped((Quaternion)start.rotation, (Quaternion)end.rotation, t);
				}
				else
				{
					Vector3 vector = (Vector3)start.rotation;
					Vector3 vector2 = (Vector3)end.rotation;
					float num = vector.y - vector2.y;
					float num2 = vector.z - vector2.z;
					Vector3 b = new Vector3(vector2.x, (num > 180f) ? (vector2.y + 360f) : ((num < -180f) ? (vector2.y - 360f) : vector2.y), (num2 > 180f) ? (vector2.z + 360f) : ((num2 < -180f) ? (vector2.z - 360f) : vector2.z));
					target.rotation = Vector3.LerpUnclamped(vector, b, t);
				}
			}
			else
			{
				target.rotation = end.rotation;
			}
			target.scale = Vector3.LerpUnclamped(start.scale, end.scale, t);
			return target;
		}

		// Token: 0x06000B8A RID: 2954 RVA: 0x0003B4A4 File Offset: 0x000396A4
		public static Matrix CatmullRomLerpUnclamped(Matrix target, Matrix pre, Matrix start, Matrix end, Matrix post, float t)
		{
			TransformCrusher transformCrusher = end.crusher;
			target.crusher = transformCrusher;
			target.position = CatmulRom.CatmullRomLerp(pre.position, start.position, end.position, post.position, t);
			if (transformCrusher != null && transformCrusher.RotCrusher != null)
			{
				if (transformCrusher.RotCrusher.TRSType == 2)
				{
					target.rotation = Quaternion.SlerpUnclamped((Quaternion)start.rotation, (Quaternion)end.rotation, t);
				}
				else
				{
					Vector3 vector = (Vector3)start.rotation;
					Vector3 vector2 = (Vector3)end.rotation;
					float num = vector.y - vector2.y;
					float num2 = vector.z - vector2.z;
					Vector3 b = new Vector3(vector2.x, (num > 180f) ? (vector2.y + 360f) : ((num < -180f) ? (vector2.y - 360f) : vector2.y), (num2 > 180f) ? (vector2.z + 360f) : ((num2 < -180f) ? (vector2.z - 360f) : vector2.z));
					target.rotation = Vector3.LerpUnclamped(vector, b, t);
				}
			}
			else
			{
				target.rotation = end.rotation;
			}
			target.scale = CatmulRom.CatmullRomLerp(pre.scale, start.scale, end.scale, post.scale, t);
			return target;
		}

		// Token: 0x06000B8B RID: 2955 RVA: 0x0003B630 File Offset: 0x00039830
		public static Matrix CatmullRomLerpUnclamped(Matrix target, Matrix pre, Matrix start, Matrix end, float t)
		{
			TransformCrusher transformCrusher = end.crusher;
			target.crusher = transformCrusher;
			target.position = CatmulRom.CatmullRomLerp(pre.position, start.position, end.position, t);
			if (transformCrusher != null && transformCrusher.RotCrusher != null)
			{
				if (transformCrusher.RotCrusher.TRSType == 2)
				{
					target.rotation = Quaternion.SlerpUnclamped((Quaternion)start.rotation, (Quaternion)end.rotation, t);
				}
				else
				{
					Vector3 vector = (Vector3)start.rotation;
					Vector3 vector2 = (Vector3)end.rotation;
					float num = vector.y - vector2.y;
					float num2 = vector.z - vector2.z;
					Vector3 b = new Vector3(vector2.x, (num > 180f) ? (vector2.y + 360f) : ((num < -180f) ? (vector2.y - 360f) : vector2.y), (num2 > 180f) ? (vector2.z + 360f) : ((num2 < -180f) ? (vector2.z - 360f) : vector2.z));
					target.rotation = Vector3.LerpUnclamped(vector, b, t);
				}
			}
			else
			{
				target.rotation = end.rotation;
			}
			target.scale = CatmulRom.CatmullRomLerp(pre.scale, start.scale, end.scale, t);
			return target;
		}

		// Token: 0x06000B8C RID: 2956 RVA: 0x0003B7B0 File Offset: 0x000399B0
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"MATRIX pos: ",
				this.position,
				" rot: ",
				this.rotation,
				" scale: ",
				this.scale,
				"  rottype: ",
				this.rotation.vectorType
			});
		}

		// Token: 0x04000C10 RID: 3088
		public TransformCrusher crusher;

		// Token: 0x04000C11 RID: 3089
		public Vector3 position;

		// Token: 0x04000C12 RID: 3090
		public Element rotation;

		// Token: 0x04000C13 RID: 3091
		public Vector3 scale;

		// Token: 0x04000C14 RID: 3092
		public static Matrix reusable = new Matrix();
	}
}
