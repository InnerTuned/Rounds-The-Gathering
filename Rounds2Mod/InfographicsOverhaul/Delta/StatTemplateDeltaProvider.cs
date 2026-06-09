using System;
using System.Collections.Generic;
using UnityEngine;

namespace InfoOverhaul.Delta;

/// <summary>
/// Simulates ApplyCardStats math from a card prefab's Gun / CharacterStatModifiers / Block
/// templates. Works for vanilla cards and CustomCard mods that set stats in SetupCard.
/// </summary>
internal static class StatTemplateDeltaProvider
{
    private const float Epsilon = 0.0001f;

    public static IReadOnlyList<StatDeltaLine> Compute(Player player, CardInfo card)
    {
        if (player == null || card == null)
            return Array.Empty<StatDeltaLine>();

        var cardRoot = card.sourceCard != null ? card.sourceCard.gameObject : card.gameObject;

        var cardGun = cardRoot.GetComponent<Gun>();
        var cardPlayer = cardRoot.GetComponent<CharacterStatModifiers>();
        var cardBlock = cardRoot.GetComponentInChildren<Block>();

        if (!HasStatTemplate(cardGun, cardPlayer, cardBlock))
            return Array.Empty<StatDeltaLine>();

        Gun gun = player.data?.weaponHandler?.gun;
        Block block = player.data?.block;
        CharacterData data = player.data;
        CharacterStatModifiers stats = player.GetComponent<CharacterStatModifiers>();
        GunAmmo ammo = gun != null ? gun.GetComponentInChildren<GunAmmo>() : null;

        var lines = new List<StatDeltaLine>();

        if (cardPlayer != null && data != null && stats != null)
        {
            TryAdd(lines, "HP", data.maxHealth, data.maxHealth * cardPlayer.health, "f0");
            TryAdd(lines, "Lives", stats.respawns + 1f, stats.respawns + cardPlayer.respawns + 1f, "f0");
            TryAdd(lines, "Move SPD", stats.movementSpeed, stats.movementSpeed * cardPlayer.movementSpeed, "f2");
            TryAdd(lines, "Jump Height", stats.jump, stats.jump * cardPlayer.jump, "f2");
            TryAdd(lines, "Player Size", stats.sizeMultiplier, stats.sizeMultiplier * cardPlayer.sizeMultiplier, "f2");
            TryAdd(lines, "Life Steal", stats.lifeSteal, stats.lifeSteal + cardPlayer.lifeSteal, "f2");
        }

        if (cardBlock != null && block != null)
        {
            float cdBefore = block.Cooldown();
            float cdAfter = (block.cooldown + block.cdAdd + cardBlock.cdAdd) * (block.cdMultiplier * cardBlock.cdMultiplier);
            TryAdd(lines, "Block CD", cdBefore, cdAfter, "f2", suffix: "s");
            TryAdd(lines, "Block Count", block.additionalBlocks + 1f, block.additionalBlocks + cardBlock.additionalBlocks + 1f, "f0");
        }

        if (cardGun != null && gun != null)
        {
            float dmgBefore = (gun.damage * 55f) * gun.bulletDamageMultiplier;
            float dmgAfter = (ApplyGunDamage(gun.damage, gun.numberOfProjectiles, cardGun) * 55f)
                * (gun.bulletDamageMultiplier * cardGun.bulletDamageMultiplier);
            TryAdd(lines, "DMG", dmgBefore, dmgAfter, "f0");

            TryAdd(lines, "Knockback", gun.knockback, ApplyKnockback(gun.knockback, gun.numberOfProjectiles, cardGun), "f2");
            TryAdd(lines, "Damage Grow", gun.damageAfterDistanceMultiplier,
                gun.damageAfterDistanceMultiplier * cardGun.damageAfterDistanceMultiplier, "f2");
            TryAdd(lines, "Bullet Slow", gun.slow, gun.slow + cardGun.slow, "f2");
            TryAdd(lines, "Attack SPD", gun.attackSpeed * gun.attackSpeedMultiplier,
                (gun.attackSpeed * cardGun.attackSpeed) * gun.attackSpeedMultiplier, "f2", suffix: "s");
            TryAdd(lines, "Bullet SPD", gun.projectileSpeed, gun.projectileSpeed * cardGun.projectileSpeed, "f2");
            TryAdd(lines, "Projectile SPD", gun.projectielSimulatonSpeed,
                gun.projectielSimulatonSpeed * cardGun.projectielSimulatonSpeed, "f2");
            TryAdd(lines, "Bullet Gravity", gun.gravity, gun.gravity * cardGun.gravity, "f2");
            TryAdd(lines, "Bullets", gun.numberOfProjectiles, gun.numberOfProjectiles + cardGun.numberOfProjectiles, "f0");
            TryAdd(lines, "Bounces", gun.reflects, gun.reflects + cardGun.reflects, "f0");
            TryAdd(lines, "Bursts", gun.bursts, gun.bursts + cardGun.bursts, "f0");

            if (cardGun.destroyBulletAfter != 0f)
                TryAdd(lines, "Bullet Range", gun.destroyBulletAfter, cardGun.destroyBulletAfter, "f2");
            else
                TryAdd(lines, "Bullet Range", gun.destroyBulletAfter,
                    gun.destroyBulletAfter + cardGun.destroyBulletAfter, "f2");
        }

        if (cardGun != null && ammo != null)
        {
            float reloadBefore = (ammo.reloadTime + ammo.reloadTimeAdd) * ammo.reloadTimeMultiplier;
            float reloadAfter = (ammo.reloadTime + ammo.reloadTimeAdd + cardGun.reloadTimeAdd)
                * (ammo.reloadTimeMultiplier * cardGun.reloadTime);
            TryAdd(lines, "Reload Time", reloadBefore, reloadAfter, "f2", suffix: "s");
            TryAdd(lines, "Ammo", ammo.maxAmmo, Mathf.Clamp(ammo.maxAmmo + cardGun.ammo, 1, 90), "f0");
        }

        return lines;
    }

    /// <summary>Inverse of <see cref="Compute"/> — stats after removing one copy of the card from the hand.</summary>
    public static IReadOnlyList<StatDeltaLine> ComputeRemoval(Player player, CardInfo card)
    {
        if (player == null || card == null)
            return Array.Empty<StatDeltaLine>();

        var cardRoot = card.sourceCard != null ? card.sourceCard.gameObject : card.gameObject;

        var cardGun = cardRoot.GetComponent<Gun>();
        var cardPlayer = cardRoot.GetComponent<CharacterStatModifiers>();
        var cardBlock = cardRoot.GetComponentInChildren<Block>();

        if (!HasStatTemplate(cardGun, cardPlayer, cardBlock))
            return Array.Empty<StatDeltaLine>();

        Gun gun = player.data?.weaponHandler?.gun;
        Block block = player.data?.block;
        CharacterData data = player.data;
        CharacterStatModifiers stats = player.GetComponent<CharacterStatModifiers>();
        GunAmmo ammo = gun != null ? gun.GetComponentInChildren<GunAmmo>() : null;

        var lines = new List<StatDeltaLine>();

        if (cardPlayer != null && data != null && stats != null)
        {
            TryAdd(lines, "HP", data.maxHealth,
                SafeDivide(data.maxHealth, cardPlayer.health), "f0");
            TryAdd(lines, "Lives", stats.respawns + 1f,
                stats.respawns - cardPlayer.respawns + 1f, "f0");
            TryAdd(lines, "Move SPD", stats.movementSpeed,
                SafeDivide(stats.movementSpeed, cardPlayer.movementSpeed), "f2");
            TryAdd(lines, "Jump Height", stats.jump,
                SafeDivide(stats.jump, cardPlayer.jump), "f2");
            TryAdd(lines, "Player Size", stats.sizeMultiplier,
                SafeDivide(stats.sizeMultiplier, cardPlayer.sizeMultiplier), "f2");
            TryAdd(lines, "Life Steal", stats.lifeSteal,
                stats.lifeSteal - cardPlayer.lifeSteal, "f2");
        }

        if (cardBlock != null && block != null)
        {
            float cdBefore = block.Cooldown();
            float cdAfter = SafeDivide(
                cdBefore,
                (block.cdMultiplier * cardBlock.cdMultiplier));
            cdAfter = (cdAfter - block.cdAdd - cardBlock.cdAdd) / Mathf.Max(block.cdMultiplier, Epsilon);
            TryAdd(lines, "Block CD", cdBefore, cdAfter, "f2", suffix: "s");
            TryAdd(lines, "Block Count", block.additionalBlocks + 1f,
                block.additionalBlocks - cardBlock.additionalBlocks + 1f, "f0");
        }

        if (cardGun != null && gun != null)
        {
            float dmgBefore = (gun.damage * 55f) * gun.bulletDamageMultiplier;
            float dmgAfter = (InverseApplyGunDamage(gun.damage, gun.numberOfProjectiles, cardGun) * 55f)
                * SafeDivide(gun.bulletDamageMultiplier, cardGun.bulletDamageMultiplier);
            TryAdd(lines, "DMG", dmgBefore, dmgAfter, "f0");

            TryAdd(lines, "Knockback", gun.knockback,
                InverseApplyKnockback(gun.knockback, gun.numberOfProjectiles, cardGun), "f2");
            TryAdd(lines, "Damage Grow", gun.damageAfterDistanceMultiplier,
                SafeDivide(gun.damageAfterDistanceMultiplier, cardGun.damageAfterDistanceMultiplier), "f2");
            TryAdd(lines, "Bullet Slow", gun.slow, gun.slow - cardGun.slow, "f2");
            TryAdd(lines, "Attack SPD", gun.attackSpeed * gun.attackSpeedMultiplier,
                SafeDivide(gun.attackSpeed, cardGun.attackSpeed) * gun.attackSpeedMultiplier, "f2", suffix: "s");
            TryAdd(lines, "Bullet SPD", gun.projectileSpeed,
                SafeDivide(gun.projectileSpeed, cardGun.projectileSpeed), "f2");
            TryAdd(lines, "Projectile SPD", gun.projectielSimulatonSpeed,
                SafeDivide(gun.projectielSimulatonSpeed, cardGun.projectielSimulatonSpeed), "f2");
            TryAdd(lines, "Bullet Gravity", gun.gravity,
                SafeDivide(gun.gravity, cardGun.gravity), "f2");
            TryAdd(lines, "Bullets", gun.numberOfProjectiles,
                gun.numberOfProjectiles - cardGun.numberOfProjectiles, "f0");
            TryAdd(lines, "Bounces", gun.reflects, gun.reflects - cardGun.reflects, "f0");
            TryAdd(lines, "Bursts", gun.bursts, gun.bursts - cardGun.bursts, "f0");

            if (cardGun.destroyBulletAfter != 0f)
                TryAdd(lines, "Bullet Range", gun.destroyBulletAfter,
                    gun.destroyBulletAfter - cardGun.destroyBulletAfter, "f2");
            else
                TryAdd(lines, "Bullet Range", gun.destroyBulletAfter,
                    gun.destroyBulletAfter - cardGun.destroyBulletAfter, "f2");
        }

        if (cardGun != null && ammo != null)
        {
            float reloadBefore = (ammo.reloadTime + ammo.reloadTimeAdd) * ammo.reloadTimeMultiplier;
            float reloadAfter = (ammo.reloadTime + ammo.reloadTimeAdd - cardGun.reloadTimeAdd)
                * SafeDivide(ammo.reloadTimeMultiplier, cardGun.reloadTime);
            TryAdd(lines, "Reload Time", reloadBefore, reloadAfter, "f2", suffix: "s");
            TryAdd(lines, "Ammo", ammo.maxAmmo,
                Mathf.Clamp(ammo.maxAmmo - cardGun.ammo, 1, 90), "f0");
        }

        return lines;
    }

    internal static bool HasStatTemplate(Gun cardGun, CharacterStatModifiers cardPlayer, Block cardBlock)
    {
        if (cardGun != null && GunHasChanges(cardGun))
            return true;
        if (cardPlayer != null && PlayerHasChanges(cardPlayer))
            return true;
        if (cardBlock != null && BlockHasChanges(cardBlock))
            return true;
        return false;
    }

    private static bool GunHasChanges(Gun g) =>
        !Mathf.Approximately(g.damage, 1f) ||
        !Mathf.Approximately(g.bulletDamageMultiplier, 1f) ||
        !Mathf.Approximately(g.knockback, 1f) ||
        !Mathf.Approximately(g.attackSpeed, 1f) ||
        !Mathf.Approximately(g.projectileSpeed, 1f) ||
        !Mathf.Approximately(g.projectielSimulatonSpeed, 1f) ||
        !Mathf.Approximately(g.gravity, 1f) ||
        !Mathf.Approximately(g.damageAfterDistanceMultiplier, 1f) ||
        !Mathf.Approximately(g.reloadTime, 1f) ||
        !Mathf.Approximately(g.reloadTimeAdd, 0f) ||
        g.ammo != 0 ||
        g.numberOfProjectiles != 0 ||
        g.reflects != 0 ||
        g.bursts != 0 ||
        g.slow != 0f ||
        g.destroyBulletAfter != 0f;

    private static bool PlayerHasChanges(CharacterStatModifiers s) =>
        !Mathf.Approximately(s.health, 1f) ||
        !Mathf.Approximately(s.movementSpeed, 1f) ||
        !Mathf.Approximately(s.jump, 1f) ||
        !Mathf.Approximately(s.sizeMultiplier, 1f) ||
        s.respawns != 0 ||
        s.lifeSteal != 0f;

    private static bool BlockHasChanges(Block b) =>
        !Mathf.Approximately(b.cdMultiplier, 1f) ||
        b.cdAdd != 0f ||
        b.additionalBlocks != 0;

    private static float ProjectileBlendFactor(int cardProjectiles, int currentProjectiles)
    {
        if (cardProjectiles == 0 || currentProjectiles == 1)
            return 1f;
        return cardProjectiles / (float)(cardProjectiles + currentProjectiles);
    }

    private static float ApplyGunDamage(float currentDamage, int currentProjectiles, Gun cardGun)
    {
        float blend = ProjectileBlendFactor(cardGun.numberOfProjectiles, currentProjectiles);
        return Mathf.Max(currentDamage * (1f - blend * (1f - cardGun.damage)), 0.25f);
    }

    private static float ApplyKnockback(float currentKnockback, int currentProjectiles, Gun cardGun)
    {
        float blend = ProjectileBlendFactor(cardGun.numberOfProjectiles, currentProjectiles);
        return currentKnockback * (1f - blend * (1f - cardGun.knockback));
    }

    private static float SafeDivide(float value, float divisor)
    {
        if (Mathf.Abs(divisor) < Epsilon)
            return value;
        return value / divisor;
    }

    private static float InverseApplyGunDamage(float currentDamage, int currentProjectiles, Gun cardGun)
    {
        int projBeforeCard = Mathf.Max(1, currentProjectiles - cardGun.numberOfProjectiles);
        float blend = ProjectileBlendFactor(cardGun.numberOfProjectiles, projBeforeCard);
        float factor = 1f - blend * (1f - cardGun.damage);
        if (Mathf.Abs(factor) < Epsilon)
            return currentDamage;
        return Mathf.Max(currentDamage / factor, 0.25f);
    }

    private static float InverseApplyKnockback(float currentKnockback, int currentProjectiles, Gun cardGun)
    {
        int projBeforeCard = Mathf.Max(1, currentProjectiles - cardGun.numberOfProjectiles);
        float blend = ProjectileBlendFactor(cardGun.numberOfProjectiles, projBeforeCard);
        float factor = 1f - blend * (1f - cardGun.knockback);
        if (Mathf.Abs(factor) < Epsilon)
            return currentKnockback;
        return currentKnockback / factor;
    }

    private static void TryAdd(List<StatDeltaLine> lines, string label, float before, float after,
        string format, string suffix = "")
    {
        if (Mathf.Abs(before - after) < Epsilon)
            return;

        lines.Add(new StatDeltaLine(label,
            before.ToString(format) + suffix,
            after.ToString(format) + suffix));
    }
}
