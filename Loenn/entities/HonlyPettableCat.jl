module HonlyPettableCatModule

using ..Ahorn, Maple

@mapdef Entity "HonlyHelper/PettableCat" HonlyPettableCat(x::Integer, y::Integer, catFlag::String = "CatHasBeenPet")

const placements = Ahorn.PlacementDict(
    "Pettable Cat (HonlyHelper)" => Ahorn.EntityPlacement(
        HonlyPettableCat
    )
)



sprite = "characters/HonlyHelper/pettableCat/spoons_idle00.png"

function Ahorn.selection(entity::HonlyPettableCat)
    x, y = Ahorn.position(entity)
    return Ahorn.Rectangle(x-8, y-8, 16, 16)
end

Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::HonlyPettableCat, room::Maple.Room) = Ahorn.drawSprite(ctx, sprite, 0, 4)

end