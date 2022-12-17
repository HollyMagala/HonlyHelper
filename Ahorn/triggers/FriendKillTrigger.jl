module HonlyHelperFriendKillTrigger
using ..Ahorn, Maple

@mapdef Trigger "HonlyHelper/FriendKillTrigger" FriendKillTrigger(x::Integer, y::Integer, width::Integer=Maple.defaultTriggerWidth, height::Integer=Maple.defaultTriggerHeight)

const placements = Ahorn.PlacementDict(
    "Friend Kill Trigger (HonlyHelper, cursed)" => Ahorn.EntityPlacement(
        FriendKillTrigger,
        "rectangle",
    ),
)

end