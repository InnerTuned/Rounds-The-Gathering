using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace ShieldsMod.Shield;

/// <summary>
/// Trigger collider at outer shield radius. While the owner is blocking and shield is active,
/// deflects enemy projectiles via Block.DoBlock (all vanilla block effects apply).
/// </summary>
public class ShieldParryTrigger : MonoBehaviour
{
    private Player _owner;
    private Block _block;
    private CircleCollider2D _trigger;
    private ShieldState _state;

    private readonly HashSet<int> _recentlyDeflected = new HashSet<int>();

    public void Init(Player owner, ShieldState state, float radius)
    {
        _owner = owner;
        _state = state;
        _block = owner.GetComponent<Block>();

        // Place on "Ignore Raycast" layer (built-in layer 2) so AI line-of-sight
        // raycasts on "Default" don't hit the shield trigger — without this, bot AI
        // sees the collider as an obstacle and refuses to shoot at shielded players.
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        _trigger = gameObject.AddComponent<CircleCollider2D>();
        _trigger.isTrigger = true;
        _trigger.radius = radius;

        var rb = gameObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.simulated = true;

        UpdateEnabled();
    }

    public void SetState(ShieldState state) => _state = state;

    public void SetRadius(float worldRadius)
    {
        if (_owner == null || _trigger == null)
            return;

        float scale = Mathf.Max(_owner.transform.localScale.x, 0.01f);
        _trigger.radius = worldRadius / scale;
    }

    public void UpdateEnabled()
    {
        if (_trigger != null)
            _trigger.enabled = _state != null && _state.IsActive;
    }

    private void LateUpdate()
    {
        if (_owner == null)
            return;

        transform.position = _owner.transform.position;
        float bodyRadius = ShieldVisual.GetBodyRadius(_owner);
        SetRadius(bodyRadius * ShieldVisual.SizeMultiplier);
        UpdateEnabled();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_owner == null || _block == null || _state == null || !_state.IsActive)
            return;

        if (!_block.IsBlocking())
            return;

        ProjectileHit projHit = other.GetComponent<ProjectileHit>()
            ?? other.GetComponentInParent<ProjectileHit>();
        if (projHit == null)
            return;

        PhotonView projView = projHit.GetComponent<PhotonView>();
        if (projView == null || !projView.IsMine)
            return;

        int projId = projHit.gameObject.GetInstanceID();
        if (_recentlyDeflected.Contains(projId))
            return;

        if (!IsEnemyProjectile(projHit))
            return;

        // Use the projectile's current position as the hit point.
        // Collider2D.ClosestPoint is not available in this Unity version.
        Vector3 hitPos = projHit.transform.position;
        Vector3 forward = projHit.transform.forward;
        if (forward.sqrMagnitude < 0.001f)
            forward = (hitPos - _owner.transform.position).normalized;

        _recentlyDeflected.Add(projId);
        _block.DoBlock(projHit.gameObject, forward, hitPos);
        SLog.Line($"Outer parry player={_owner.playerID} projectile={projHit.gameObject.name}");
    }

    private bool IsEnemyProjectile(ProjectileHit projHit)
    {
        Player spawner = projHit.ownPlayer;
        if (spawner == null)
        {
            SpawnedAttack sa = projHit.GetComponent<SpawnedAttack>();
            if (sa != null)
                spawner = sa.spawner;
        }

        if (spawner == null)
            return true;

        return spawner.playerID != _owner.playerID
            && spawner.transform.root != _owner.transform.root;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        ProjectileHit projHit = other.GetComponent<ProjectileHit>()
            ?? other.GetComponentInParent<ProjectileHit>();
        if (projHit != null)
            _recentlyDeflected.Remove(projHit.gameObject.GetInstanceID());
    }
}
