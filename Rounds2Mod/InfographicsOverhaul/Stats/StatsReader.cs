using System.Collections.Generic;

namespace InfoOverhaul.Stats;

/// <summary>Collects player stats using the same formulas as Infoholic's GameStatusUpdate.</summary>
internal static class StatsReader
{
    internal readonly struct StatLine
    {
        public readonly string Text;
        public StatLine(string text) => Text = text;
    }

    public static List<StatLine> Collect(Player player)
    {
        var lines = new List<StatLine>(24);
        if (player == null)
            return lines;

        Gun gun = player.data?.weaponHandler?.gun;
        Block block = player.data?.block;

        lines.Add(new StatLine($"HP: {player.data.health:f0} / {player.data.maxHealth:f0}"));
        lines.Add(new StatLine($"Lives: {player.data.stats.respawns + 1:f0}"));
        lines.Add(new StatLine(block != null ? $"Block CD: {block.Cooldown():f2}s" : "Block CD: —"));
        lines.Add(new StatLine(block != null ? $"Block Count: {block.additionalBlocks + 1:f0}" : "Block Count: —"));
        lines.Add(new StatLine(FormatShieldHealth(player.playerID)));
        lines.Add(new StatLine(FormatPoisonResistance(player.playerID)));
        lines.Add(new StatLine(FormatStunResistance(player.playerID)));
        lines.Add(new StatLine(FormatLifestealResistance(player.playerID)));

        if (gun != null)
        {
            lines.Add(new StatLine($"DMG: {(gun.damage * 55f) * gun.bulletDamageMultiplier:f0}"));
            lines.Add(new StatLine($"Knockback: {gun.knockback:f2}"));
            lines.Add(new StatLine($"Life Steal: {player.data.stats.lifeSteal:f2}"));
            lines.Add(new StatLine($"Damage Grow: {gun.damageAfterDistanceMultiplier:f2}"));
            lines.Add(new StatLine($"Bullet Slow: {gun.slow:f2}"));
            lines.Add(new StatLine($"Move SPD: {player.data.stats.movementSpeed:f2}"));
            lines.Add(new StatLine($"Jump Height: {player.data.stats.jump:f2}"));
            lines.Add(new StatLine($"Player Size: {player.data.stats.sizeMultiplier:f2}"));
            lines.Add(new StatLine($"Attack SPD: {(gun.attackSpeed * gun.attackSpeedMultiplier):f2}s"));
            lines.Add(new StatLine($"Bullet SPD: {gun.projectileSpeed:f2}"));
            lines.Add(new StatLine($"Projectile SPD: {gun.projectielSimulatonSpeed:f2}"));

            var ammo = gun.GetComponentInChildren<GunAmmo>();
            if (ammo != null)
            {
                float reload = (ammo.reloadTime + ammo.reloadTimeAdd) * ammo.reloadTimeMultiplier;
                lines.Add(new StatLine($"Reload Time: {reload:f2}s"));
                lines.Add(new StatLine($"Ammo: {ammo.maxAmmo:f0}"));
            }
            else
            {
                lines.Add(new StatLine("Reload Time: —"));
                lines.Add(new StatLine("Ammo: —"));
            }

            lines.Add(new StatLine($"Bullet Gravity: {gun.gravity:f2}"));
            lines.Add(new StatLine($"Bullets: {gun.numberOfProjectiles:f0}"));
            lines.Add(new StatLine($"Bullet Range: {gun.destroyBulletAfter:f2}"));
            lines.Add(new StatLine($"Bounces: {gun.reflects:f0}"));
            lines.Add(new StatLine($"Bursts: {gun.bursts:f0}"));
        }
        else
        {
            lines.Add(new StatLine("DMG: —"));
            lines.Add(new StatLine("Knockback: —"));
            lines.Add(new StatLine($"Life Steal: {player.data.stats.lifeSteal:f2}"));
            lines.Add(new StatLine("Damage Grow: —"));
            lines.Add(new StatLine("Bullet Slow: —"));
            lines.Add(new StatLine($"Move SPD: {player.data.stats.movementSpeed:f2}"));
            lines.Add(new StatLine($"Jump Height: {player.data.stats.jump:f2}"));
            lines.Add(new StatLine($"Player Size: {player.data.stats.sizeMultiplier:f2}"));
            lines.Add(new StatLine("Attack SPD: —"));
            lines.Add(new StatLine("Bullet SPD: —"));
            lines.Add(new StatLine("Projectile SPD: —"));
            lines.Add(new StatLine("Reload Time: —"));
            lines.Add(new StatLine("Ammo: —"));
            lines.Add(new StatLine("Bullet Gravity: —"));
            lines.Add(new StatLine("Bullets: —"));
            lines.Add(new StatLine("Bullet Range: —"));
            lines.Add(new StatLine("Bounces: —"));
            lines.Add(new StatLine("Bursts: —"));
        }

        return lines;
    }

    private static string FormatShieldHealth(int playerID)
    {
        if (ShieldStatsBridge.TryGetShieldHealth(playerID, out float current, out float max))
        {
            return max > 0f
                ? $"Shield Health: {current:f0} / {max:f0}"
                : "Shield Health: None";
        }

        return ShieldStatsBridge.IsAvailable
            ? "Shield Health: — / —"
            : "Shield Health: N/A";
    }

    private static string FormatPoisonResistance(int playerID)
    {
        if (PoisonResistanceStatsBridge.TryGetPercent(playerID, out float percent))
            return $"Poison Resistance: {percent:f0}%";

        return PoisonResistanceStatsBridge.IsAvailable
            ? "Poison Resistance: 0%"
            : "Poison Resistance: N/A";
    }

    private static string FormatStunResistance(int playerID)
    {
        if (StunResistanceStatsBridge.TryGetPercent(playerID, out float percent))
            return $"Stun Resistance: {percent:f0}%";

        return StunResistanceStatsBridge.IsAvailable
            ? "Stun Resistance: 0%"
            : "Stun Resistance: N/A";
    }

    private static string FormatLifestealResistance(int playerID)
    {
        if (LifestealResistanceStatsBridge.TryGetPercent(playerID, out float percent))
            return $"Lifesteal Resistance: {percent:f0}%";

        return LifestealResistanceStatsBridge.IsAvailable
            ? "Lifesteal Resistance: 0%"
            : "Lifesteal Resistance: N/A";
    }
}
