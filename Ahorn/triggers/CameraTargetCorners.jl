module CameraTargetCornerTriggerModule

using ..Ahorn, Maple

@mapdef Trigger "HonlyHelper/CameraTargetCornerTrigger" CameraTargetCornerTrigger(x::Integer, y::Integer, width::Integer=Maple.defaultTriggerWidth, height::Integer=Maple.defaultTriggerHeight, lerpStrength::Number=0.0, positionMode::String="BottomLeft", xOnly::Bool=false, yOnly::Bool=false, deleteFlag::String="", nodes::Array{Tuple{Integer, Integer}, 1}=Tuple{Integer, Integer}[])

const placements = Ahorn.PlacementDict(
    "Camera Target Corner (HonlyHelper)" => Ahorn.EntityPlacement(
        CameraTargetCornerTrigger,
        "rectangle",
        Dict{String, Any}(),
        function(trigger)
            trigger.data["nodes"] = [(Int(trigger.data["x"]) + Int(trigger.data["width"]) + 8, Int(trigger.data["y"]))]
        end
    )
)

const positionModeOptions = String["BottomLeft", "BottomRight", "TopLeft", "TopRight"]

Ahorn.editingOptions(trigger::CameraTargetCornerTrigger) = Dict{String, Any}(
        "positionMode" => positionModeOptions
    )

function Ahorn.nodeLimits(trigger::CameraTargetCornerTrigger)
    return 1, 1
end

end