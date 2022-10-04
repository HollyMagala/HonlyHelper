module MothModule

using ..Ahorn, Maple

@mapdef Entity "HonlyHelper/Moth" HonlyMoth(x::Integer, y::Integer)

const placements = Ahorn.PlacementDict(
    "Möth (HonlyHelper)" => Ahorn.EntityPlacement(
        HonlyMoth
    )
)



sprite = "objects/HonlyHelper/Bugs/Moth.png"

function Ahorn.selection(entity::HonlyMoth)
    x, y = Ahorn.position(entity)
    return Ahorn.Rectangle(x-2, y-2, 4, 4)
end

Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::HonlyMoth, room::Maple.Room) = Ahorn.drawSprite(ctx, sprite, 0, 0)

end