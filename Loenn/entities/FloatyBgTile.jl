module FloatyBgTileModule

using ..Ahorn, Maple

@mapdef Entity "HonlyHelper/FloatyBgTile" FloatyBgTile(x::Integer, y::Integer, width::Integer=2, height::Integer=2,  tiletype::String="3", disableSpawnOffset::Bool=false)

const placements = Ahorn.PlacementDict(
    "Floaty BG Tiles (HonlyHelper)" => Ahorn.EntityPlacement(
        FloatyBgTile,
        "rectangle",
        Dict{String, Any}()
    )
)

function bgTiletypeEditingOptions()
    validTiles = Ahorn.validTiles("bgTiles")
    tileNames = Ahorn.tileNames("bgTiles")

    return Dict{String, String}(
        tileNames[mat] => string(mat)
        for mat in validTiles if mat != Maple.tile_bg_names["Air"]
    )
end

function drawBgTileEntity(ctx::Ahorn.Cairo.CairoContext, room::Maple.Room, entity::Maple.Entity; material::Union{Char, Nothing}=nothing, alpha::Number=Ahorn.getGlobalAlpha(), blendIn::Bool=false)
    x = Int(get(entity.data, "x", 0))
    y = Int(get(entity.data, "y", 0))

    width = Int(get(entity.data, "width", 32))
    height = Int(get(entity.data, "height", 32))

    blendIn = get(entity.data, "blendin", blendIn)

    if material === nothing
        key = Ahorn.materialTileTypeKey(entity)
        tile = get(entity.data, key, "3")
        material = isa(tile, Number) ? string(tile) : tile
    end
    
    # Don't draw air versions, even though they shouldn't exist
    if material[1] in Ahorn.validTileEntityTiles("bgTiles")
        fakeTiles = Ahorn.createFakeTiles(room, x, y, width, height, material[1], blendIn=blendIn)
        objTiles = Ahorn.getObjectTiles(room, x, y, width, height)

        Ahorn.drawFakeTiles(ctx, room, fakeTiles, objTiles, false, x, y, alpha=alpha, clipEdges=true)
    end
end



Ahorn.editingOptions(entity::FloatyBgTile) = Dict{String, Any}(
    "tiletype" => bgTiletypeEditingOptions()
)

Ahorn.minimumSize(entity::FloatyBgTile) = 8, 8
Ahorn.resizable(entity::FloatyBgTile) = true, true

Ahorn.selection(entity::FloatyBgTile) = Ahorn.getEntityRectangle(entity)

Ahorn.renderAbs(ctx::Ahorn.Cairo.CairoContext, entity::FloatyBgTile, room::Maple.Room) = drawBgTileEntity(ctx, room, entity, alpha=0.4)

end