module HonlyFireFlyModule

using ..Ahorn, Maple

@mapdef Entity "HonlyHelper/FireFly" HonlyFireFly(x::Integer, y::Integer)

const placements = Ahorn.PlacementDict(
    "FireFly (HonlyHelper)" => Ahorn.EntityPlacement(
        HonlyFireFly
    )
)



sprite = "objects/HonlyHelper/Bugs/FireFly.png"

function Ahorn.selection(entity::HonlyFireFly)
    x, y = Ahorn.position(entity)
    return Ahorn.Rectangle(x-2, y-2, 4, 4)
end

Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::HonlyFireFly, room::Maple.Room) = Ahorn.drawSprite(ctx, sprite, 0, 0)

end