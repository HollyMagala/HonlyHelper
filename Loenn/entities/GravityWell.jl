module GravityWellModule

using ..Ahorn, Maple

@mapdef Entity "HonlyHelper/GravityWell" GravityWell(x::Integer, y::Integer, strength::Number=5, radius::Number=40.0, distanceScaling::String="Fixed")

const placements = Ahorn.PlacementDict(
    "Gravity Well (HonlyHelper)" => Ahorn.EntityPlacement(
        GravityWell
    )
)

const DistanceScalings = String["Linear", "Fixed"]

Ahorn.editingOptions(entity::GravityWell) = Dict{String, Any}(
  "distanceScaling" => DistanceScalings
)

sprite = "objects/HonlyHelper/GravityWell/Test.png"

function Ahorn.selection(entity::GravityWell)
    x, y = Ahorn.position(entity)
    return Ahorn.Rectangle(x - 16, y - 16, 24, 24)
end

Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::GravityWell, room::Maple.Room) = Ahorn.drawSprite(ctx, sprite, -4, -4)

end