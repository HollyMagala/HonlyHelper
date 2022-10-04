module HonlyHorrorNodeModule

using ..Ahorn, Maple

@mapdef Entity "HonlyHelper/HorrorNode" HonlyHorrorNode(x::Integer, y::Integer)

const placements = Ahorn.PlacementDict(
    "HorrorNode (HonlyHelper)" => Ahorn.EntityPlacement(
        HonlyHorrorNode
    )
)



sprite = "objects/HonlyHelper/Bugs/HorrorNode.png"

function Ahorn.selection(entity::HonlyHorrorNode)
    x, y = Ahorn.position(entity)
    return Ahorn.Rectangle(x-2, y-2, 4, 4)
end

Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::HonlyHorrorNode, room::Maple.Room) = Ahorn.drawSprite(ctx, sprite, 0, 0)

end