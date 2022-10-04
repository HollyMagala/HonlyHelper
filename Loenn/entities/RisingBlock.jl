module HonlyHelperRisingBlock

using ..Ahorn, Maple

@mapdef Entity "HonlyHelper/RisingBlock" RisingBlock(x::Integer, y::Integer, width::Integer=2, height::Integer=2,  tiletype::String="3")

const placements = Ahorn.PlacementDict(
    "Rising Block (Honly Helper)" => Ahorn.EntityPlacement(
        RisingBlock,
        "rectangle",
        Dict{String, Any}(),
        Ahorn.tileEntityFinalizer
    ),
)

Ahorn.editingOptions(entity::RisingBlock) = Dict{String, Any}(
    "tiletype" => Ahorn.tiletypeEditingOptions()
)

Ahorn.minimumSize(entity::RisingBlock) = 8, 8
Ahorn.resizable(entity::RisingBlock) = true, true

Ahorn.selection(entity::RisingBlock) = Ahorn.getEntityRectangle(entity)

Ahorn.renderAbs(ctx::Ahorn.Cairo.CairoContext, entity::RisingBlock, room::Maple.Room) = Ahorn.drawTileEntity(ctx, room, entity)

end