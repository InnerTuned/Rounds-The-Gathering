using System;
using Photon.Compression;
using Photon.Pun;
using UnityEngine;

namespace emotitron.Utilities.Example
{
	// Token: 0x020001EF RID: 495
	public class BasicController : MonoBehaviour
	{
		// Token: 0x060009BB RID: 2491 RVA: 0x000317C1 File Offset: 0x0002F9C1
		private void Awake()
		{
			this.rb = base.GetComponent<Rigidbody>();
			this.rb2D = base.GetComponent<Rigidbody2D>();
			this.pv = base.GetComponent<PhotonView>();
		}

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x060009BC RID: 2492 RVA: 0x000317E7 File Offset: 0x0002F9E7
		private bool IsMine
		{
			get
			{
				return this.pv == null || this.pv.IsMine;
			}
		}

		// Token: 0x060009BD RID: 2493 RVA: 0x00031804 File Offset: 0x0002FA04
		private void Start()
		{
			if (base.GetComponent<IHasTransformCrusher>() != null)
			{
				this.tc = base.GetComponent<IHasTransformCrusher>().TC;
			}
			if (!this.IsMine)
			{
				if (this.rb)
				{
					this.rb.isKinematic = true;
				}
				if (this.rb2D)
				{
					this.rb2D.isKinematic = true;
				}
			}
		}

		// Token: 0x060009BE RID: 2494 RVA: 0x00031864 File Offset: 0x0002FA64
		private void FixedUpdate()
		{
			if (this.timing == BasicController.Timing.Fixed || (this.timing == BasicController.Timing.Auto && this.rb))
			{
				this.Apply();
			}
		}

		// Token: 0x060009BF RID: 2495 RVA: 0x0003188A File Offset: 0x0002FA8A
		private void Update()
		{
			if (this.timing == BasicController.Timing.Update || (this.timing == BasicController.Timing.Auto && !this.rb))
			{
				this.Apply();
			}
		}

		// Token: 0x060009C0 RID: 2496 RVA: 0x000318B0 File Offset: 0x0002FAB0
		private void LateUpdate()
		{
			if (this.timing == BasicController.Timing.LateUpdate)
			{
				this.Apply();
			}
		}

		// Token: 0x060009C1 RID: 2497 RVA: 0x000318C4 File Offset: 0x0002FAC4
		private void SumKeys(out Vector3 move, out Vector3 turn)
		{
			move = new Vector3(0f, 0f, 0f);
			if (Input.touchCount > 0)
			{
				Vector2 rawPosition = Input.GetTouch(0).rawPosition;
				if (rawPosition.x < (float)Screen.width * 0.333f)
				{
					move.x -= 1f;
				}
				else if (rawPosition.x > (float)Screen.width * 0.666f)
				{
					move.x += 1f;
				}
				if (rawPosition.y < (float)Screen.height * 0.333f)
				{
					move.z -= 1f;
				}
				else if (rawPosition.y > (float)Screen.height * 0.666f)
				{
					move.z += 1f;
				}
			}
			if (Input.GetKey(this.moveRight))
			{
				move.x += 1f;
			}
			if (Input.GetKey(this.moveLeft))
			{
				move.x -= 1f;
			}
			if (Input.GetKey(this.moveUp))
			{
				move.y += 1f;
			}
			if (Input.GetKey(this.moveDn))
			{
				move.y -= 1f;
			}
			if (Input.GetKey(this.moveFwd))
			{
				move.z += 1f;
			}
			if (Input.GetKey(this.moveBwd))
			{
				move.z -= 1f;
			}
			move = Vector3.ClampMagnitude(move, 1f);
			turn = new Vector3(0f, 0f, 0f);
			if (Input.GetKey(this.pitchPos))
			{
				turn.x += 1f;
			}
			if (Input.GetKey(this.pitchNeg))
			{
				turn.x -= 1f;
			}
			if (Input.GetKey(this.yawPos))
			{
				turn.y += 1f;
			}
			if (Input.GetKey(this.yawNeg))
			{
				turn.y -= 1f;
			}
			if (Input.GetKey(this.rollPos))
			{
				turn.z += 1f;
			}
			if (Input.GetKey(this.rollNeg))
			{
				turn.z -= 1f;
			}
		}

		// Token: 0x060009C2 RID: 2498 RVA: 0x00031B18 File Offset: 0x0002FD18
		private void Apply()
		{
			if (!this.IsMine)
			{
				return;
			}
			Vector3 vector;
			Vector3 vector2;
			this.SumKeys(out vector, out vector2);
			if (this.rb && !this.rb.isKinematic)
			{
				if (this.rb && this.clampToCrusher && this.tc != null)
				{
					this.rb.MovePosition(this.tc.PosCrusher.Clamp(this.rb.position));
				}
				vector *= this.moveForce * Time.deltaTime;
				if (this.moveRelative)
				{
					this.rb.AddRelativeForce(vector, 2);
				}
				else
				{
					this.rb.AddForce(vector, 2);
				}
			}
			else if (this.rb2D && !this.rb2D.isKinematic)
			{
				if (this.rb2D && this.clampToCrusher && this.tc != null)
				{
					this.rb2D.MovePosition(this.tc.PosCrusher.Clamp(this.rb2D.position));
				}
				vector *= this.moveForce * Time.deltaTime;
				if (this.moveRelative)
				{
					this.rb2D.AddRelativeForce(vector, 1);
				}
				else
				{
					this.rb2D.AddForce(vector, 1);
				}
			}
			else
			{
				Vector3 vector3 = this.rb ? this.rb.position : base.transform.position;
				if (this.moveRelative)
				{
					vector3 += base.transform.localRotation * vector * this.moveSpeed * Time.deltaTime;
				}
				else
				{
					vector3 += vector * this.moveSpeed * Time.deltaTime;
				}
				if (this.clampToCrusher && this.tc != null && this.tc.PosCrusher != null)
				{
					vector3 = this.tc.PosCrusher.Clamp(vector3);
				}
				if (this.rb)
				{
					this.rb.MovePosition(vector3);
				}
				else
				{
					base.transform.position = vector3;
				}
			}
			if (this.rb && !this.rb.isKinematic)
			{
				vector2 *= this.turnForce * Time.deltaTime;
				this.rb.AddRelativeTorque(vector2, 2);
				return;
			}
			if (this.clampToCrusher && this.tc != null && this.tc.RotCrusher.TRSType != 2)
			{
				Vector3 localEulerAngles = this.tc.RotCrusher.Clamp(base.transform.eulerAngles += vector2 * this.turnSpeed * Time.deltaTime);
				base.transform.localEulerAngles = localEulerAngles;
				return;
			}
			base.transform.rotation *= Quaternion.Euler(vector2 * this.turnSpeed * Time.deltaTime);
		}

		// Token: 0x04000B0B RID: 2827
		private PhotonView pv;

		// Token: 0x04000B0C RID: 2828
		private Rigidbody rb;

		// Token: 0x04000B0D RID: 2829
		private Rigidbody2D rb2D;

		// Token: 0x04000B0E RID: 2830
		[HideInInspector]
		public TransformCrusher TransformCrusherRef;

		// Token: 0x04000B0F RID: 2831
		private TransformCrusher tc;

		// Token: 0x04000B10 RID: 2832
		public BasicController.Timing timing = BasicController.Timing.Fixed;

		// Token: 0x04000B11 RID: 2833
		public bool moveRelative = true;

		// Token: 0x04000B12 RID: 2834
		[Space]
		public KeyCode moveLeft = KeyCode.A;

		// Token: 0x04000B13 RID: 2835
		public KeyCode moveRight = KeyCode.D;

		// Token: 0x04000B14 RID: 2836
		public KeyCode moveFwd = KeyCode.W;

		// Token: 0x04000B15 RID: 2837
		public KeyCode moveBwd = KeyCode.S;

		// Token: 0x04000B16 RID: 2838
		public KeyCode moveUp = KeyCode.Space;

		// Token: 0x04000B17 RID: 2839
		public KeyCode moveDn = KeyCode.Z;

		// Token: 0x04000B18 RID: 2840
		[Space]
		public KeyCode pitchPos = KeyCode.R;

		// Token: 0x04000B19 RID: 2841
		public KeyCode pitchNeg = KeyCode.C;

		// Token: 0x04000B1A RID: 2842
		public KeyCode yawPos = KeyCode.E;

		// Token: 0x04000B1B RID: 2843
		public KeyCode yawNeg = KeyCode.Q;

		// Token: 0x04000B1C RID: 2844
		public KeyCode rollPos = KeyCode.Alpha4;

		// Token: 0x04000B1D RID: 2845
		public KeyCode rollNeg = KeyCode.Alpha4;

		// Token: 0x04000B1E RID: 2846
		[Space]
		public bool clampToCrusher;

		// Token: 0x04000B1F RID: 2847
		public float moveSpeed = 5f;

		// Token: 0x04000B20 RID: 2848
		public float turnSpeed = 60f;

		// Token: 0x04000B21 RID: 2849
		public float moveForce = 12f;

		// Token: 0x04000B22 RID: 2850
		public float turnForce = 100f;

		// Token: 0x04000B23 RID: 2851
		public float scaleSpeed = 1f;

		// Token: 0x04000B24 RID: 2852
		private bool isMine;

		// Token: 0x020003B1 RID: 945
		public enum Timing
		{
			// Token: 0x040012A6 RID: 4774
			Auto,
			// Token: 0x040012A7 RID: 4775
			Fixed,
			// Token: 0x040012A8 RID: 4776
			Update,
			// Token: 0x040012A9 RID: 4777
			LateUpdate
		}
	}
}
