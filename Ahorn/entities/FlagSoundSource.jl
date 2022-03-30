module HonlyHelperFlagSoundSource

using ..Ahorn, Maple

@mapdef Entity "HonlyHelper/FlagSoundSource" FlagSoundSource(x::Integer, y::Integer, sound::String = "sound", flag::String = "flag", AllowFade::Bool=false)

const placements = Ahorn.PlacementDict(
    "Flag Sound Source (HonlyHelper)" => Ahorn.EntityPlacement(
        FlagSoundSource
    )
)

function Ahorn.selection(entity::FlagSoundSource)
    x, y = Ahorn.position(entity::FlagSoundSource)
    return Ahorn.Rectangle(x - 12, y - 12, 24, 24)
end

Ahorn.editingOptions(entity::FlagSoundSource) = Dict{String, Any}(
    "sound" => EnvironmentSounds.sounds
)

Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::FlagSoundSource, room::Maple.Room) = Ahorn.drawImage(ctx, Ahorn.Assets.speaker, -12, -12)

end