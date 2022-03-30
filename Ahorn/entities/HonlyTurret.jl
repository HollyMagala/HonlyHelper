module HonlyHelperTurret

using ..Ahorn, Maple

@mapdef Entity "HonlyHelper/Turret" Turret(x::Integer, y::Integer, turretID::String = "TurretID", aimTime::Number = 2.0, cooldownTime::Number = 2.0, randomCooldownMultiplier::Number = 0.0, desiredBulletSpeed::Number = 2000.0, accelerationMultiplier::Number = 1.0)

const placements = Ahorn.PlacementDict(
    "Turret (HonlyHelper, WIP)" => Ahorn.EntityPlacement(
        Turret
    )
)

sprite = "objects/HonlyHelper/Turret/idle00.png"

function Ahorn.selection(entity::Turret)
    x, y = Ahorn.position(entity)
    return Ahorn.Rectangle(x - 16, y - 16, 24, 24)
end

Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::Turret, room::Maple.Room) = Ahorn.drawSprite(ctx, sprite, -4, -4)

end