using System;
using System.Collections.Generic;
using System.Reflection;
using Photon.Pun.Simple.ContactGroups;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x0200025C RID: 604
	public class ContactTrigger : MonoBehaviour, IContactTrigger, IContactable, IOnPreSimulate, IOnStateChange
	{
		// Token: 0x06000D17 RID: 3351 RVA: 0x000415CE File Offset: 0x0003F7CE
		static ContactTrigger()
		{
			ContactTrigger.FindDerivedTypesFromAssembly();
		}

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x06000D18 RID: 3352 RVA: 0x000415EA File Offset: 0x0003F7EA
		// (set) Token: 0x06000D19 RID: 3353 RVA: 0x000415F2 File Offset: 0x0003F7F2
		public bool PreventRepeats
		{
			get
			{
				return this.preventRepeats;
			}
			set
			{
				this.preventRepeats = value;
			}
		}

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x06000D1A RID: 3354 RVA: 0x000415FB File Offset: 0x0003F7FB
		public List<IContactSystem> ContactSystems
		{
			get
			{
				return this._contactSystems;
			}
		}

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x06000D1B RID: 3355 RVA: 0x00041603 File Offset: 0x0003F803
		// (set) Token: 0x06000D1C RID: 3356 RVA: 0x0004160B File Offset: 0x0003F80B
		public IContactTrigger Proxy
		{
			get
			{
				return this._proxy;
			}
			set
			{
				this._proxy = value;
			}
		}

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x06000D1D RID: 3357 RVA: 0x00041614 File Offset: 0x0003F814
		// (set) Token: 0x06000D1E RID: 3358 RVA: 0x0004161C File Offset: 0x0003F81C
		public byte Index { get; set; }

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x06000D1F RID: 3359 RVA: 0x00041625 File Offset: 0x0003F825
		public NetObject NetObj
		{
			get
			{
				return this.netObj;
			}
		}

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x06000D20 RID: 3360 RVA: 0x0004162D File Offset: 0x0003F82D
		public ISyncContact SyncContact
		{
			get
			{
				return this.syncContact;
			}
		}

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x06000D21 RID: 3361 RVA: 0x00041635 File Offset: 0x0003F835
		public IContactGroupsAssign ContactGroupsAssign
		{
			get
			{
				return this.contactGroupsAssign;
			}
		}

		// Token: 0x06000D22 RID: 3362 RVA: 0x00041640 File Offset: 0x0003F840
		public virtual void PollInterfaces()
		{
			this._contactSystems.Clear();
			NestedComponentUtilities.GetNestedComponentsInParents<IContactSystem, NetObject>(base.transform, ContactTrigger.tempFindSystems);
			int i = 0;
			int count = ContactTrigger.tempFindSystems.Count;
			while (i < count)
			{
				Type type = ContactTrigger.tempFindSystems[i].GetType();
				bool flag = false;
				foreach (Type type2 in this.ignoredSystems)
				{
					if (PunExtensions.CheckIsAssignableFrom(type, type2))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					this._contactSystems.Add(ContactTrigger.tempFindSystems[i]);
				}
				i++;
			}
			NestedComponentUtilities.GetNestedComponentsInChildren<IOnContactEvent>(base.transform, this.OnContactEventCallbacks, true, new Type[]
			{
				typeof(NetObject),
				typeof(IContactTrigger)
			});
		}

		// Token: 0x06000D23 RID: 3363 RVA: 0x00041730 File Offset: 0x0003F930
		public virtual void Awake()
		{
			if (this._proxy == null)
			{
				this._proxy = this;
			}
			this.GetAllowedTypesFromHashes();
			this.PollInterfaces();
			this.netObj = NestedComponentUtilities.GetParentComponent<NetObject>(base.transform);
			this.syncContact = base.GetComponent<ISyncContact>();
			this.contactGroupsAssign = base.GetComponent<ContactGroupAssign>();
			if (this.contactGroupsAssign == null)
			{
				IContactGroupsAssign nestedComponentInParent = NestedComponentUtilities.GetNestedComponentInParent<IContactGroupsAssign, NetObject>(base.transform);
				if (nestedComponentInParent != null && nestedComponentInParent.ApplyToChildren)
				{
					this.contactGroupsAssign = nestedComponentInParent;
				}
			}
			foreach (IOnContactEvent onContactEvent in this.OnContactEventCallbacks)
			{
				this.usedContactTypes |= onContactEvent.TriggerOn;
			}
		}

		// Token: 0x06000D24 RID: 3364 RVA: 0x000417FC File Offset: 0x0003F9FC
		protected virtual void OnEnable()
		{
			if (this.preventRepeats)
			{
				NetMasterCallbacks.RegisterCallbackInterfaces(this, true, false);
				this.triggeringHitscans.Clear();
				this.triggeringEnters.Clear();
				this.triggeringStays.Clear();
			}
		}

		// Token: 0x06000D25 RID: 3365 RVA: 0x0004182F File Offset: 0x0003FA2F
		protected virtual void OnDisable()
		{
			if (this.preventRepeats)
			{
				NetMasterCallbacks.RegisterCallbackInterfaces(this, false, true);
			}
		}

		// Token: 0x06000D26 RID: 3366 RVA: 0x00041841 File Offset: 0x0003FA41
		public void OnStateChange(ObjState newState, ObjState previousState, Transform attachmentTransform, Mount attachTo = null, bool isReady = true)
		{
			if (this.preventRepeats)
			{
				this.triggeringEnters.Clear();
			}
		}

		// Token: 0x06000D27 RID: 3367 RVA: 0x00041856 File Offset: 0x0003FA56
		private void OnTriggerEnter2D(Collider2D other)
		{
			this.Contact(other, ContactType.Enter);
		}

		// Token: 0x06000D28 RID: 3368 RVA: 0x00041856 File Offset: 0x0003FA56
		private void OnTriggerEnter(Collider other)
		{
			this.Contact(other, ContactType.Enter);
		}

		// Token: 0x06000D29 RID: 3369 RVA: 0x00041860 File Offset: 0x0003FA60
		private void OnCollisionEnter2D(Collision2D collision)
		{
			this.Contact(collision.collider, ContactType.Enter);
		}

		// Token: 0x06000D2A RID: 3370 RVA: 0x0004186F File Offset: 0x0003FA6F
		private void OnCollisionEnter(Collision collision)
		{
			this.Contact(collision.collider, ContactType.Enter);
		}

		// Token: 0x06000D2B RID: 3371 RVA: 0x0004187E File Offset: 0x0003FA7E
		private void OnTriggerStay2D(Collider2D other)
		{
			this.Contact(other, ContactType.Stay);
		}

		// Token: 0x06000D2C RID: 3372 RVA: 0x0004187E File Offset: 0x0003FA7E
		private void OnTriggerStay(Collider other)
		{
			this.Contact(other, ContactType.Stay);
		}

		// Token: 0x06000D2D RID: 3373 RVA: 0x00041888 File Offset: 0x0003FA88
		private void OnCollisionStay2D(Collision2D collision)
		{
			this.Contact(collision.collider, ContactType.Stay);
		}

		// Token: 0x06000D2E RID: 3374 RVA: 0x00041897 File Offset: 0x0003FA97
		private void OnCollisionStay(Collision collision)
		{
			this.Contact(collision.collider, ContactType.Stay);
		}

		// Token: 0x06000D2F RID: 3375 RVA: 0x000418A6 File Offset: 0x0003FAA6
		private void OnTriggerExit2D(Collider2D other)
		{
			this.Contact(other, ContactType.Exit);
		}

		// Token: 0x06000D30 RID: 3376 RVA: 0x000418A6 File Offset: 0x0003FAA6
		private void OnTriggerExit(Collider other)
		{
			this.Contact(other, ContactType.Exit);
		}

		// Token: 0x06000D31 RID: 3377 RVA: 0x000418B0 File Offset: 0x0003FAB0
		private void OnCollisionExit2D(Collision2D collision)
		{
			this.Contact(collision.collider, ContactType.Exit);
		}

		// Token: 0x06000D32 RID: 3378 RVA: 0x000418BF File Offset: 0x0003FABF
		private void OnCollisionExit(Collision collision)
		{
			this.Contact(collision.collider, ContactType.Exit);
		}

		// Token: 0x06000D33 RID: 3379 RVA: 0x000418D0 File Offset: 0x0003FAD0
		protected virtual void Contact(Component otherCollider, ContactType contactType)
		{
			IContactTrigger nestedComponentInParents = NestedComponentUtilities.GetNestedComponentInParents<IContactTrigger, NetObject>(otherCollider.transform);
			if (nestedComponentInParents == null)
			{
				return;
			}
			if (this.CheckIsNested(this, nestedComponentInParents.Proxy))
			{
				return;
			}
			if (this._proxy.NetObj == nestedComponentInParents.Proxy.NetObj)
			{
				return;
			}
			nestedComponentInParents.Proxy.OnContact(this, contactType);
			this._proxy.OnContact(nestedComponentInParents, contactType);
		}

		// Token: 0x06000D34 RID: 3380 RVA: 0x00041930 File Offset: 0x0003FB30
		protected bool CheckIsNested(IContactTrigger first, IContactTrigger second)
		{
			NetObject netObject = first.NetObj;
			NetObject netObject2 = second.NetObj;
			Transform parent;
			for (NetObject netObject3 = netObject; netObject3 != null; netObject3 = NestedComponentUtilities.GetParentComponent<NetObject>(parent))
			{
				if (netObject3 == netObject2)
				{
					return true;
				}
				parent = netObject3.transform.parent;
				if (parent == null)
				{
					break;
				}
			}
			Transform parent2;
			for (NetObject netObject3 = netObject2; netObject3 != null; netObject3 = NestedComponentUtilities.GetParentComponent<NetObject>(parent2))
			{
				if (netObject3 == netObject)
				{
					return true;
				}
				parent2 = netObject3.transform.parent;
				if (parent2 == null)
				{
					break;
				}
			}
			return false;
		}

		// Token: 0x06000D35 RID: 3381 RVA: 0x00041998 File Offset: 0x0003FB98
		public virtual void OnContact(IContactTrigger otherCT, ContactType contactType)
		{
			if (base.GetComponent<ContactProjectile>() && contactType == ContactType.Enter)
			{
				global::Debug.Log("Prj Contact");
			}
			List<IContactSystem> contactSystems = otherCT.Proxy.ContactSystems;
			int count = contactSystems.Count;
			if (count == 0)
			{
				return;
			}
			if (this.netObj != null && !this._proxy.NetObj.AllObjsAreReady)
			{
				return;
			}
			NetObject netObject = otherCT.Proxy.NetObj;
			if (netObject != null && !netObject.AllObjsAreReady)
			{
				global::Debug.Log(string.Concat(new object[]
				{
					Time.time,
					base.name,
					" ",
					netObject.photonView.OwnerActorNr,
					" Other object not ready so ignoring contact"
				}));
				return;
			}
			for (int i = 0; i < count; i++)
			{
				IContactSystem contactSystem = contactSystems[i];
				if (this.IsCompatibleSystem(contactSystem, otherCT))
				{
					if (this.preventRepeats)
					{
						switch (contactType)
						{
						case ContactType.Enter:
							if (this.triggeringEnters.Contains(contactSystem))
							{
								goto IL_1BF;
							}
							this.triggeringEnters.Add(contactSystem);
							break;
						case ContactType.Stay:
							if (this.triggeringStays.Contains(contactSystem))
							{
								goto IL_1BF;
							}
							this.triggeringStays.Add(contactSystem);
							break;
						case (ContactType)3:
							break;
						case ContactType.Exit:
							if (!this.triggeringEnters.Contains(contactSystem))
							{
								goto IL_1BF;
							}
							this.triggeringEnters.Remove(contactSystem);
							break;
						default:
							if (contactType == ContactType.Hitscan)
							{
								if (this.triggeringHitscans.Contains(contactSystem))
								{
									goto IL_1BF;
								}
								this.triggeringHitscans.Add(contactSystem);
							}
							break;
						}
					}
					if ((this.usedContactTypes & contactType) == ContactType.Undefined)
					{
						return;
					}
					ContactEvent contactEvent = new ContactEvent(contactSystem, otherCT, contactType);
					if (this.Proxy.SyncContact == null)
					{
						this.ContactCallbacks(contactEvent);
					}
					else
					{
						this.syncContact.SyncContactEvent(contactEvent);
					}
				}
				IL_1BF:;
			}
		}

		// Token: 0x06000D36 RID: 3382 RVA: 0x00041B70 File Offset: 0x0003FD70
		public virtual Consumption ContactCallbacks(ContactEvent contactEvent)
		{
			Consumption consumption = Consumption.None;
			int i = 0;
			int count = this.OnContactEventCallbacks.Count;
			while (i < count)
			{
				consumption |= this.OnContactEventCallbacks[i].OnContactEvent(contactEvent);
				if (consumption == Consumption.All)
				{
					return Consumption.All;
				}
				i++;
			}
			return consumption;
		}

		// Token: 0x06000D37 RID: 3383 RVA: 0x00041BB3 File Offset: 0x0003FDB3
		public void OnPreSimulate(int frameId, int subFrameId)
		{
			if (this.preventRepeats)
			{
				this.triggeringHitscans.Clear();
				this.triggeringStays.Clear();
			}
		}

		// Token: 0x06000D38 RID: 3384 RVA: 0x00041BD4 File Offset: 0x0003FDD4
		internal void GetAllowedTypesFromHashes()
		{
			this.ignoredSystems.Clear();
			if (this._ignoredSystems == null)
			{
				return;
			}
			foreach (Type type in ContactTrigger.contactSystemTypes)
			{
				int hashCode = type.Name.GetHashCode();
				bool flag = false;
				int[] array = this._ignoredSystems;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] == hashCode)
					{
						flag = true;
					}
				}
				if (flag)
				{
					this.ignoredSystems.Add(type);
				}
			}
		}

		// Token: 0x06000D39 RID: 3385 RVA: 0x00041C78 File Offset: 0x0003FE78
		internal static void FindDerivedTypesFromAssembly()
		{
			ContactTrigger.contactSystemTypes.Clear();
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				foreach (Type type in assemblies[i].GetTypes())
				{
					if (!type.IsAbstract && PunExtensions.CheckIsAssignableFrom(typeof(IContactSystem), type))
					{
						ContactTrigger.contactSystemTypes.Add(type);
					}
				}
			}
		}

		// Token: 0x06000D3A RID: 3386 RVA: 0x00041CEC File Offset: 0x0003FEEC
		private bool IsCompatibleSystem(IContactSystem system, IContactTrigger ct)
		{
			Type type = system.GetType();
			using (List<Type>.Enumerator enumerator = this.ignoredSystems.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (PunExtensions.CheckIsAssignableFrom(enumerator.Current, type))
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x04000CBE RID: 3262
		[Tooltip("If ITriggeringComponent has multiple colliders, they will all be capable of triggering Enter/Stay/Exit events. Enabling this prevents that, and will suppress multiple calls on the same object.")]
		[SerializeField]
		public bool preventRepeats = true;

		// Token: 0x04000CBF RID: 3263
		[SerializeField]
		[HideInInspector]
		public int[] _ignoredSystems;

		// Token: 0x04000CC0 RID: 3264
		protected List<Type> ignoredSystems = new List<Type>();

		// Token: 0x04000CC1 RID: 3265
		public List<IOnContactEvent> OnContactEventCallbacks = new List<IOnContactEvent>(1);

		// Token: 0x04000CC2 RID: 3266
		private List<IContactSystem> _contactSystems = new List<IContactSystem>(0);

		// Token: 0x04000CC3 RID: 3267
		[Tooltip("This ContactTrigger can act as a proxy of another. For example projectiles set the proxy as the shooters ContactTrigger, so projectile hits can be treated as hits by the players weapon. Default setting is 'this', indicating this isn't a proxy.")]
		public IContactTrigger _proxy;

		// Token: 0x04000CC5 RID: 3269
		protected NetObject netObj;

		// Token: 0x04000CC6 RID: 3270
		protected ISyncContact syncContact;

		// Token: 0x04000CC7 RID: 3271
		protected IContactGroupsAssign contactGroupsAssign;

		// Token: 0x04000CC8 RID: 3272
		internal ContactType usedContactTypes;

		// Token: 0x04000CC9 RID: 3273
		protected HashSet<IContactSystem> triggeringHitscans = new HashSet<IContactSystem>();

		// Token: 0x04000CCA RID: 3274
		protected HashSet<IContactSystem> triggeringEnters = new HashSet<IContactSystem>();

		// Token: 0x04000CCB RID: 3275
		protected HashSet<IContactSystem> triggeringStays = new HashSet<IContactSystem>();

		// Token: 0x04000CCC RID: 3276
		public static List<IContactSystem> tempFindSystems = new List<IContactSystem>(2);

		// Token: 0x04000CCD RID: 3277
		internal static List<Type> contactSystemTypes = new List<Type>();
	}
}
