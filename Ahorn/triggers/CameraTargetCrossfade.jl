module CameraTargetCrossfadeTriggerModule

using ..Ahorn, Maple

@mapdef Trigger "HonlyHelper/CameraTargetCrossfadeTrigger" CameraTargetCrossfadeTrigger(x::Integer, y::Integer, width::Integer=Maple.defaultTriggerWidth, height::Integer=Maple.defaultTriggerHeight, lerpStrengthA::Number=0.0, lerpStrengthB::Number=0.0, positionMode::String="NoEffect", xOnly::Bool=false, yOnly::Bool=false, deleteFlag::String="", nodes::Array{Tuple{Integer, Integer}, 1}=Tuple{Integer, Integer}[])

const placements = Ahorn.PlacementDict(
    "Camera Target Crossfade (HonlyHelper)" => Ahorn.EntityPlacement(
        CameraTargetCrossfadeTrigger,
        "rectangle",
        Dict{String, Any}(),
        function(trigger)
            trigger.data["nodes"] = [(Int(trigger.data["x"]) + Int(trigger.data["width"]) + 8, Int(trigger.data["y"])), (Int(trigger.data["x"]) + Int(trigger.data["width"]) + 8, Int(trigger.data["y"]) + 8)]
        end
    )
)

Ahorn.editingOptions(trigger::CameraTargetCrossfadeTrigger) = Dict{String, Any}(
        "positionMode" => Maple.trigger_position_modes
    )

function Ahorn.nodeLimits(trigger::CameraTargetCrossfadeTrigger)
    return 2, 2
end

end