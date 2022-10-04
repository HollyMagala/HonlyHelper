module HonlyFireFlyDanceModule

using ..Ahorn, Maple

@mapdef Entity "HonlyHelper/FireFlyDance" HonlyFireFlyDance(x::Integer, y::Integer)

const placements = Ahorn.PlacementDict(
    "FireFlyDance (HonlyHelper)" => Ahorn.EntityPlacement(
        HonlyFireFlyDance
    )
)



sprite = "objects/HonlyHelper/Bugs/FireFly.png"

function Ahorn.selection(entity::HonlyFireFlyDance)
    x, y = Ahorn.position(entity)
    return Ahorn.Rectangle(x-2, y-2, 4, 4)
end

Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::HonlyFireFlyDance, room::Maple.Room) = Ahorn.drawSprite(ctx, sprite, 0, 0)

end