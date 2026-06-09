using System;
using System.Collections.Generic;
using Photon.Pun.Simple.Pooling;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x020002BB RID: 699
	public static class GenericHitscanExt
	{
		// Token: 0x06000F48 RID: 3912 RVA: 0x0004A4F4 File Offset: 0x000486F4
		private static GameObject SetUpDebugPrimitive(this GameObject go, string name, bool createCylinderChild = false)
		{
			go.name = name;
			go.hideFlags = (HideFlags.HideInHierarchy | HideFlags.HideInInspector);
			Object.DontDestroyOnLoad(go);
			Collider component;
			if (createCylinderChild)
			{
				GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
				gameObject.GetComponent<Renderer>().material.color = Color.yellow;
				gameObject.transform.parent = go.transform;
				gameObject.transform.eulerAngles = new Vector3(90f, 0f, 0f);
				component = gameObject.GetComponent<Collider>();
			}
			else
			{
				component = go.GetComponent<Collider>();
			}
			Object.DestroyImmediate(component);
			Renderer component2 = go.GetComponent<Renderer>();
			if (component2)
			{
				component2.material.color = Color.yellow;
			}
			return go;
		}

		// Token: 0x06000F49 RID: 3913 RVA: 0x0004A598 File Offset: 0x00048798
		private static void CreateDebugPrimitives()
		{
			GenericHitscanExt.DebugSpherePrefab = GameObject.CreatePrimitive(PrimitiveType.Sphere).SetUpDebugPrimitive("DebugSpherePrefab", false);
			Pool.AddPrefabToPool(GenericHitscanExt.DebugSpherePrefab.gameObject, 2, 2, null, true);
			GenericHitscanExt.DebugCubePrefab = GameObject.CreatePrimitive(PrimitiveType.Cube).SetUpDebugPrimitive("DebugCubePrefab", false);
			Pool.AddPrefabToPool(GenericHitscanExt.DebugCubePrefab.gameObject, 2, 2, null, true);
			GenericHitscanExt.DebugCylinderPrefab = new GameObject().SetUpDebugPrimitive("DebugCylinderPrefab", true);
			Pool.AddPrefabToPool(GenericHitscanExt.DebugCylinderPrefab.gameObject, 4, 4, null, true);
		}

		// Token: 0x06000F4A RID: 3914 RVA: 0x0004A620 File Offset: 0x00048820
		public static void VisualizeHitscan(this HitscanDefinition hd, Transform origin, float duration = 0.5f)
		{
			if (GenericHitscanExt.DebugSpherePrefab == null)
			{
				GenericHitscanExt.CreateDebugPrimitives();
			}
			switch (hd.hitscanType)
			{
			case HitscanType.Raycast:
			{
				Vector3 pos = hd.useOffset ? (origin.position + origin.TransformDirection(hd.offset1) + origin.forward * hd.distance * 0.5f) : (origin.position + origin.forward * hd.distance * 0.5f);
				Pool.Spawn(GenericHitscanExt.DebugCylinderPrefab, pos, origin.rotation, new Vector3(0.1f, 0.1f, hd.distance * 0.5f), duration);
				return;
			}
			case HitscanType.SphereCast:
			{
				Vector3 pos2 = hd.useOffset ? (origin.position + origin.TransformDirection(hd.offset1)) : origin.position;
				Vector3 pos3 = hd.useOffset ? (origin.position + origin.TransformDirection(hd.offset1) + origin.forward * hd.distance * 0.5f) : (origin.position + origin.forward * hd.distance * 0.5f);
				Vector3 pos4 = hd.useOffset ? (origin.position + origin.TransformDirection(hd.offset1) + origin.forward * hd.distance) : (origin.position + origin.forward * hd.distance);
				float num = hd.radius * 2f;
				Pool.Spawn(GenericHitscanExt.DebugSpherePrefab, pos2, Quaternion.identity, new Vector3(num, num, num), duration);
				Pool.Spawn(GenericHitscanExt.DebugSpherePrefab, pos4, Quaternion.identity, new Vector3(num, num, num), duration);
				Pool.Spawn(GenericHitscanExt.DebugCylinderPrefab, pos3, origin.rotation, new Vector3(num, num, hd.distance * 0.5f), duration);
				return;
			}
			case HitscanType.CapsuleCast:
			{
				Vector3 vector = origin.position + origin.TransformDirection(hd.offset1);
				Vector3 vector2 = origin.position + origin.TransformDirection(hd.offset2);
				Vector3 vector3 = origin.forward * hd.distance;
				Vector3 b = vector3 * 0.5f;
				Vector3 pos5 = vector + b;
				Vector3 pos6 = vector2 + b;
				Vector3 vector4 = vector + vector3;
				Vector3 vector5 = vector2 + vector3;
				Vector3 pos7 = vector + (vector2 - vector) * 0.5f;
				Vector3 pos8 = vector4 + (vector5 - vector4) * 0.5f;
				float num2 = hd.radius * 2f;
				Pool.Spawn(GenericHitscanExt.DebugSpherePrefab, vector, Quaternion.identity, new Vector3(num2, num2, num2), duration);
				Pool.Spawn(GenericHitscanExt.DebugSpherePrefab, vector4, Quaternion.identity, new Vector3(num2, num2, num2), duration);
				Pool.Spawn(GenericHitscanExt.DebugSpherePrefab, vector2, Quaternion.identity, new Vector3(num2, num2, num2), duration);
				Pool.Spawn(GenericHitscanExt.DebugSpherePrefab, vector5, Quaternion.identity, new Vector3(num2, num2, num2), duration);
				Pool.Spawn(GenericHitscanExt.DebugCylinderPrefab, pos5, origin.rotation, new Vector3(num2, num2, hd.distance * 0.5f), duration);
				Pool.Spawn(GenericHitscanExt.DebugCylinderPrefab, pos6, origin.rotation, new Vector3(num2, num2, hd.distance * 0.5f), duration);
				float z = Vector3.Magnitude(vector2 - vector) * 0.5f;
				Pool.Spawn(GenericHitscanExt.DebugCylinderPrefab, pos7, Quaternion.LookRotation(vector2 - vector, Vector3.up), new Vector3(num2, num2, z), duration);
				Pool.Spawn(GenericHitscanExt.DebugCylinderPrefab, pos8, Quaternion.LookRotation(vector5 - vector4, Vector3.up), new Vector3(num2, num2, z), duration);
				return;
			}
			case HitscanType.BoxCast:
			{
				Vector3 vector6 = hd.useOffset ? (origin.position + origin.TransformDirection(hd.offset1)) : origin.position;
				Vector3 vector7 = hd.useOffset ? (origin.position + origin.TransformDirection(hd.offset1) + origin.forward * hd.distance) : (origin.position + origin.forward * hd.distance);
				Vector3 pos9 = vector6 + (vector7 - vector6) * 0.5f;
				Quaternion rot = Quaternion.Euler(origin.eulerAngles + hd.orientation);
				Vector3 scl = hd.halfExtents * 2f;
				Pool.Spawn(GenericHitscanExt.DebugCubePrefab, vector7, rot, scl, duration);
				Pool.Spawn(GenericHitscanExt.DebugCylinderPrefab, pos9, Quaternion.LookRotation(vector7 - vector6, Vector3.up), new Vector3(0.1f, 0.1f, hd.distance * 0.5f), duration);
				return;
			}
			case HitscanType.OverlapSphere:
			{
				Vector3 pos10 = hd.useOffset ? (origin.position + hd.offset1) : origin.position;
				Quaternion rotation = origin.rotation;
				float num3 = hd.radius * 2f;
				Pool.Spawn(GenericHitscanExt.DebugSpherePrefab, pos10, rotation, new Vector3(num3, num3, num3), duration);
				return;
			}
			case HitscanType.OverlapCapsule:
			{
				Quaternion rotation2 = origin.rotation;
				float num4 = hd.radius * 2f;
				Vector3 vector8 = origin.TransformPoint(hd.offset1);
				Vector3 vector9 = origin.TransformPoint(hd.offset2);
				Vector3 vector10 = vector9 - vector8;
				Vector3 pos11 = vector8 + vector10 * 0.5f;
				Pool.Spawn(GenericHitscanExt.DebugSpherePrefab, vector8, rotation2, new Vector3(num4, num4, num4), duration);
				Pool.Spawn(GenericHitscanExt.DebugSpherePrefab, vector9, rotation2, new Vector3(num4, num4, num4), duration);
				Pool.Spawn(GenericHitscanExt.DebugCylinderPrefab, pos11, Quaternion.LookRotation(vector10, Vector3.up), new Vector3(num4, num4, vector10.magnitude * 0.5f), duration);
				return;
			}
			case HitscanType.OverlapBox:
			{
				Vector3 pos12 = hd.useOffset ? (origin.position + hd.offset1) : origin.position;
				Quaternion rot2 = Quaternion.Euler(origin.eulerAngles + hd.orientation);
				Vector3 scl2 = hd.halfExtents * 2f;
				Pool.Spawn(GenericHitscanExt.DebugCubePrefab, pos12, rot2, scl2, duration);
				return;
			}
			default:
				return;
			}
		}

		// Token: 0x06000F4B RID: 3915 RVA: 0x0004ACD1 File Offset: 0x00048ED1
		public static int GenericHitscanNonAlloc(this HitscanDefinition hd, Transform origin, out RaycastHit[] rayhits, out Collider[] hits, ref int nearestIndex, bool showDebugWidgets = false, bool useSecondaryRealm = false)
		{
			hits = GenericHitscanExt.reusableColliderArray;
			rayhits = GenericHitscanExt.reusableRayHitArray;
			return hd.GenericHitscanNonAlloc(origin, ref GenericHitscanExt.reusableColliderArray, ref GenericHitscanExt.reusableRayHitArray, ref nearestIndex, showDebugWidgets, useSecondaryRealm);
		}

		// Token: 0x06000F4C RID: 3916 RVA: 0x0004ACF8 File Offset: 0x00048EF8
		public static int GenericHitscanNonAlloc(this HitscanDefinition hd, Transform origin, NetObject ownerNetObj, ref List<NetworkHit> hitscanHits, ref int nearestIndex, bool showDebugWidgets = false, bool useSecondaryRealm = false)
		{
			if (hitscanHits == null)
			{
				hitscanHits = GenericHitscanExt.reusableHitscanHitList;
			}
			int num = -1;
			nearestIndex = -1;
			int num2 = hd.GenericHitscanNonAlloc(origin, ref GenericHitscanExt.reusableColliderArray, ref GenericHitscanExt.reusableRayHitArray, ref num, showDebugWidgets, useSecondaryRealm);
			GenericHitscanExt.reusableGameObjIntDict.Clear();
			hitscanHits.Clear();
			if (num2 > 0)
			{
				for (int i = 0; i < num2; i++)
				{
					Collider collider = GenericHitscanExt.reusableColliderArray[i];
					List<NetObject> list = GenericHitscanExt.reusableNetObjectsList;
					NestedComponentUtilities.GetNestedComponentsInParents<NetObject>(collider.transform, list);
					int count = list.Count;
					if (count != 0)
					{
						bool flag = false;
						for (int j = 0; j < count; j++)
						{
							if (list[j] == ownerNetObj)
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							NetObject netObject = list[0];
							int viewID = netObject.ViewID;
							IContactGroupsAssign component = collider.GetComponent<IContactGroupsAssign>();
							int num3 = (component == null) ? 0 : component.Mask;
							int num4;
							bool flag2 = GenericHitscanExt.reusableGameObjIntDict.TryGetValue(viewID, ref num4);
							int colliderId = netObject.colliderLookup[collider];
							if (flag2)
							{
								NetworkHit networkHit = hitscanHits[num4];
								hitscanHits[num4] = new NetworkHit(networkHit.netObjId, networkHit.hitMask | num3, colliderId);
								if (i == num)
								{
									nearestIndex = num4;
								}
							}
							else
							{
								if (i == num)
								{
									nearestIndex = hitscanHits.Count;
								}
								hitscanHits.Add(new NetworkHit(viewID, num3, colliderId));
								GenericHitscanExt.reusableGameObjIntDict.Add(viewID, hitscanHits.Count - 1);
							}
						}
					}
				}
			}
			return GenericHitscanExt.reusableGameObjIntDict.Count;
		}

		// Token: 0x06000F4D RID: 3917 RVA: 0x0004AE70 File Offset: 0x00049070
		public static int GenericHitscanNonAlloc(this HitscanDefinition hd, Transform origin, ref Collider[] hits, ref RaycastHit[] rayhits, ref int nearestIndex, bool showDebugWidgets = false, bool useSecondaryRealm = false)
		{
			if (showDebugWidgets)
			{
				hd.VisualizeHitscan(origin, 0.5f);
			}
			if (hits == null)
			{
				hits = GenericHitscanExt.reusableColliderArray;
			}
			if (rayhits == null)
			{
				rayhits = GenericHitscanExt.reusableRayHitArray;
			}
			HitscanType hitscanType = hd.hitscanType;
			Vector3 vector = hd.useOffset ? origin.TransformPoint(hd.offset1) : origin.position;
			LayerMask layerMask = hd.layerMask;
			int num;
			switch (hitscanType)
			{
			case HitscanType.Raycast:
				num = Physics.RaycastNonAlloc(origin.position, origin.forward, rayhits, hd.distance, layerMask);
				break;
			case HitscanType.SphereCast:
				num = Physics.SphereCastNonAlloc(new Ray(vector, origin.forward), hd.radius, rayhits, hd.distance, layerMask);
				break;
			case HitscanType.CapsuleCast:
				num = Physics.CapsuleCastNonAlloc(origin.TransformPoint(hd.offset1), origin.TransformPoint(hd.offset2), hd.radius, origin.forward, rayhits, hd.distance, layerMask);
				break;
			case HitscanType.BoxCast:
				num = Physics.BoxCastNonAlloc(vector, hd.halfExtents, origin.forward, rayhits, Quaternion.Euler(origin.eulerAngles + hd.orientation), hd.distance, layerMask);
				break;
			case HitscanType.OverlapSphere:
				num = Physics.OverlapSphereNonAlloc(vector, hd.radius, hits, layerMask);
				break;
			case HitscanType.OverlapCapsule:
				num = Physics.OverlapCapsuleNonAlloc(origin.TransformPoint(hd.offset1), origin.TransformPoint(hd.offset2), hd.radius, hits, layerMask);
				break;
			case HitscanType.OverlapBox:
				num = Physics.OverlapBoxNonAlloc(vector, hd.halfExtents, hits, Quaternion.Euler(origin.eulerAngles + hd.orientation), layerMask);
				break;
			default:
				num = 0;
				break;
			}
			nearestIndex = -1;
			if (num == 0)
			{
				return num;
			}
			bool nearestOnly = hd.nearestOnly;
			float num2 = float.PositiveInfinity;
			if (hitscanType.IsOverlap())
			{
				nearestIndex = -1;
				return num;
			}
			if (nearestOnly)
			{
				for (int i = 0; i < num; i++)
				{
					RaycastHit raycastHit = rayhits[i];
					float distance = raycastHit.distance;
					if (distance < num2)
					{
						num2 = distance;
						nearestIndex = i;
					}
				}
				hits[0] = rayhits[nearestIndex].collider;
				return 1;
			}
			for (int j = 0; j < num; j++)
			{
				RaycastHit raycastHit2 = rayhits[j];
				float distance2 = raycastHit2.distance;
				if (distance2 < num2)
				{
					num2 = distance2;
					nearestIndex = j;
				}
				if (!nearestOnly)
				{
					hits[j] = rayhits[j].collider;
				}
			}
			if (nearestOnly)
			{
				hits[0] = rayhits[nearestIndex].collider;
				return 1;
			}
			return num;
		}

		// Token: 0x06000F4E RID: 3918 RVA: 0x0004B108 File Offset: 0x00049308
		[Obsolete("Haven't reworked this for new physx yet.")]
		public static int GenericCastNonAlloc(this Transform srcT, Collider[] hits, RaycastHit[] rayhits, float distance, float radius, int mask, Quaternion orientation, bool useOffset, Vector3 offset1, Vector3 offset2, HitscanType hitscanType)
		{
			Vector3 vector = useOffset ? (srcT.position + srcT.TransformDirection(offset1)) : srcT.position;
			int num;
			switch (hitscanType)
			{
			case HitscanType.Raycast:
				num = Physics.RaycastNonAlloc(new Ray(vector, srcT.forward), rayhits, distance, mask);
				break;
			case HitscanType.SphereCast:
				num = Physics.SphereCastNonAlloc(new Ray(vector, srcT.forward), radius, rayhits, distance, mask);
				break;
			case HitscanType.CapsuleCast:
				num = Physics.CapsuleCastNonAlloc(srcT.TransformPoint(offset1), srcT.TransformPoint(offset2), radius, srcT.forward, rayhits, distance, mask);
				break;
			case HitscanType.BoxCast:
				num = Physics.BoxCastNonAlloc(vector, offset2, srcT.forward, rayhits, orientation, distance, mask);
				break;
			case HitscanType.OverlapSphere:
				num = Physics.OverlapSphereNonAlloc(vector, radius, hits, mask);
				break;
			case HitscanType.OverlapCapsule:
				num = Physics.OverlapCapsuleNonAlloc(srcT.TransformPoint(offset1), srcT.TransformPoint(offset2), radius, hits, mask);
				break;
			case HitscanType.OverlapBox:
				num = Physics.OverlapBoxNonAlloc(vector, offset2, hits, orientation, mask);
				break;
			default:
				num = 0;
				break;
			}
			if (hitscanType.IsCast())
			{
				for (int i = 0; i < num; i++)
				{
					hits[i] = rayhits[i].collider;
				}
			}
			return num;
		}

		// Token: 0x04000E6E RID: 3694
		public static Collider[] reusableColliderArray = new Collider[64];

		// Token: 0x04000E6F RID: 3695
		public static RaycastHit[] reusableRayHitArray = new RaycastHit[64];

		// Token: 0x04000E70 RID: 3696
		public static List<NetworkHit> reusableHitscanHitList = new List<NetworkHit>();

		// Token: 0x04000E71 RID: 3697
		public static List<NetObject> reusableNetObjectsList = new List<NetObject>();

		// Token: 0x04000E72 RID: 3698
		private static GameObject DebugSpherePrefab;

		// Token: 0x04000E73 RID: 3699
		private static GameObject DebugCylinderPrefab;

		// Token: 0x04000E74 RID: 3700
		private static GameObject DebugCubePrefab;

		// Token: 0x04000E75 RID: 3701
		private static readonly Dictionary<int, int> reusableGameObjIntDict = new Dictionary<int, int>();
	}
}
