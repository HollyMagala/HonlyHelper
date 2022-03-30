module HonlyHelperTurretController
using ..Ahorn, Maple

@mapdef Trigger "HonlyHelper/TurretController" TurretController(x::Integer, y::Integer, width::Integer=Maple.defaultTriggerWidth, height::Integer=Maple.defaultTriggerHeight, turretID::String="TurretID", turretAction::String="Heli Fade In")

const placements = Ahorn.PlacementDict(
    "Turret Controller (HonlyHelper, cursed)" => Ahorn.EntityPlacement(
        TurretController,
        "rectangle",
    ),
)

const turretActions = String["HeliFadeIn", "HeliOn", "HeliLeave", "GunOnlyOn", "GunOnlyOff"]

Ahorn.editingOptions(trigger::TurretController) = Dict{String, Any}(
  "turretAction" => turretActions
)

end