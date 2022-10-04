module HonlyHelperBouncySpikes

using ..Ahorn, Maple

@mapdef Entity "HonlyHelper/BouncySpikesUp" BouncySpikesUp(x::Integer, y::Integer, width::Integer=Maple.defaultSpikeWidth, texture::String="objects/HonlyHelper/BouncySpikes/bouncer", FreezeFrameEnable::Bool=false)
@mapdef Entity "HonlyHelper/BouncySpikesDown" BouncySpikesDown(x::Integer, y::Integer, width::Integer=Maple.defaultSpikeWidth, texture::String="objects/HonlyHelper/BouncySpikes/bouncer", FreezeFrameEnable::Bool=false)
@mapdef Entity "HonlyHelper/BouncySpikesLeft" BouncySpikesLeft(x::Integer, y::Integer, height::Integer=Maple.defaultSpikeHeight, texture::String="objects/HonlyHelper/BouncySpikes/bouncer", FreezeFrameEnable::Bool=false)
@mapdef Entity "HonlyHelper/BouncySpikesRight" BouncySpikesRight(x::Integer, y::Integer, height::Integer=Maple.defaultSpikeHeight, texture::String="objects/HonlyHelper/BouncySpikes/bouncer", FreezeFrameEnable::Bool=false)

const placements = Ahorn.PlacementDict()

BouncySpikes = Dict{String, Type}(
    "up" => BouncySpikesUp,
    "down" => BouncySpikesDown,
    "left" => BouncySpikesLeft,
    "right" => BouncySpikesRight
)

BouncySpikesUnion = Union{BouncySpikesUp, BouncySpikesDown, BouncySpikesLeft, BouncySpikesRight}


    
        for (dir, entity) in BouncySpikes
            key = "Bouncy Spikes ($(uppercasefirst(dir))) (HonlyHelper)"
            placements[key] = Ahorn.EntityPlacement(
                entity,
                "rectangle",
            )
        end

directions = Dict{String, String}(
    "HonlyHelper/BouncySpikesUp" => "up",
    "HonlyHelper/BouncySpikesDown" => "down",
    "HonlyHelper/BouncySpikesLeft" => "left",
    "HonlyHelper/BouncySpikesRight" => "right",
)

offsets = Dict{String, Tuple{Integer, Integer}}(
    "up" => (4, -4),
    "down" => (4, 4),
    "left" => (-4, 4),
    "right" => (4, 4),
)

BouncySpikesOffsets = Dict{String, Tuple{Integer, Integer}}(
    "up" => (0, 0),
    "down" => (0, -0),
    "left" => (0, 0),
    "right" => (-0, 0),
)

rotations = Dict{String, Number}(
    "up" => 0,
    "right" => pi / 2,
    "down" => pi,
    "left" => pi * 3 / 2
)

resizeDirections = Dict{String, Tuple{Bool, Bool}}(
    "up" => (true, false),
    "down" => (true, false),
    "left" => (false, true),
    "right" => (false, true),
)

function Ahorn.renderSelectedAbs(ctx::Ahorn.Cairo.CairoContext, entity::BouncySpikesUnion)
    direction = get(directions, entity.name, "up")
    theta = rotations[direction] - pi / 2

    width = Int(get(entity.data, "width", 0))
    height = Int(get(entity.data, "height", 0))

    x, y = Ahorn.position(entity)
    cx, cy = x + floor(Int, width / 2) - 8 * (direction == "left"), y + floor(Int, height / 2) - 8 * (direction == "up")

    Ahorn.drawArrow(ctx, cx, cy, cx + cos(theta) * 24, cy + sin(theta) * 24, Ahorn.colors.selection_selected_fc, headLength=6)
end

function Ahorn.selection(entity::BouncySpikesUnion)
    if haskey(directions, entity.name)
        x, y = Ahorn.position(entity)

        width = Int(get(entity.data, "width", 8))
        height = Int(get(entity.data, "height", 8))

        direction = get(directions, entity.name, "up")

        ox, oy = offsets[direction]

        return Ahorn.Rectangle(x + ox - 4, y + oy - 4, width, height)
    end
end

Ahorn.minimumSize(entity::BouncySpikesUnion) = 8, 8

function Ahorn.resizable(entity::BouncySpikesUnion)
    if haskey(directions, entity.name)
        direction = get(directions, entity.name, "up")

        return resizeDirections[direction]
    end
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::BouncySpikesUnion)
    if haskey(directions, entity.name)
        direction = get(directions, entity.name, "up")
	texture_used = get(entity.data, "texture", "objects/HonlyHelper/BouncySpikes/bouncer")
        BouncySpikesOffset = BouncySpikesOffsets[direction]

        width = get(entity.data, "width", 8)
        height = get(entity.data, "height", 8)

        for ox in 0:8:width - 8, oy in 0:8:height - 8
            drawX, drawY = (ox, oy) .+ offsets[direction] .+ BouncySpikesOffset
            Ahorn.drawSprite(ctx, "$(texture_used)_$(direction)00", drawX, drawY)
        end
    end
end

end