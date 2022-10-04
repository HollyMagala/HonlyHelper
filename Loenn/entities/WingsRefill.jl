module HonlyHelperWingsRefill

using ..Ahorn, Maple

@mapdef Entity "HonlyHelper/WingsRefill" WingsUpgrade(x::Integer, y::Integer, oneUse::Bool = false, feathers::Integer = 1)

const placements = Ahorn.PlacementDict(
    "Wings Refill (HonlyHelper)" => Ahorn.EntityPlacement(
        WingsUpgrade
    )
)

sprite = "objects/refill/idle00"

function Ahorn.selection(entity::WingsUpgrade)
    x, y = Ahorn.position(entity)
    return Ahorn.getSpriteRectangle(sprite, x, y)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::WingsUpgrade, room::Maple.Room)
    Ahorn.drawSprite(ctx, sprite, 0, 0)
end

end