using System;
using UnityEngine;

// Token: 0x02000032 RID: 50
public class CollisionChecker : MonoBehaviour
{
	// Token: 0x060000FF RID: 255 RVA: 0x000079C9 File Offset: 0x00005BC9
	private void Awake()
	{
		this.data = base.GetComponent<CharacterData>();
	}

	// Token: 0x06000100 RID: 256 RVA: 0x000079D7 File Offset: 0x00005BD7
	private void OnCollisionEnter2D(Collision2D collision)
	{
		this.Collide(collision);
	}

	// Token: 0x06000101 RID: 257 RVA: 0x000079D7 File Offset: 0x00005BD7
	private void OnCollisionStay2D(Collision2D collision)
	{
		this.Collide(collision);
	}

	// Token: 0x06000102 RID: 258 RVA: 0x000079E0 File Offset: 0x00005BE0
	private void Collide(Collision2D collision)
	{
		if (this.collisionAction != null)
		{
			this.collisionAction.Invoke(collision);
		}
		if (Vector3.Angle(Vector3.up, collision.contacts[0].normal) > 70f)
		{
			Vector3.Angle(Vector3.up, collision.contacts[0].normal);
			return;
		}
		this.data.TouchGround(collision.contacts[0].point, collision.contacts[0].normal, collision.otherRigidbody, collision.transform);
	}

	// Token: 0x04000158 RID: 344
	private CharacterData data;

	// Token: 0x04000159 RID: 345
	public Action<Collision2D> collisionAction;
}
