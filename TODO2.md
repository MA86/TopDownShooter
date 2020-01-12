# Ideas
- make player rotate about center
- player can mount guns on certain things
    - marked by editor-man
        - if gun is on top of "mountable region", then give gun an accuracy boost
        - you can only turn the gun a certain angle 
    - only some guns are mountable
- zombie AI
- make e drop barrel (not space)

# Tasks
- DONE - create mountable region scene (inherits area 2d)
- DONE - when player collides with mountable region
    - set flag in player (OverMountableRegion)
- DONE - when player uncollides with mountable region
    - unset the flag (OverMountableRegion)
- when e is pressed (unhandled input of player)
    - if OverMountableRegion
        - DONE make player immobile
        - limit player's rotation fov 
        - DONE - set gun's Mounted flag
        - DONE move player to pos of mountable region (and face him)
    - if gun is currently mounted
        - DONE make player mobile
        - unlimit his rotation fov
        - DONE reset gun's mounted flag
- when gun's mounted flag is on
    - DONE make it fire more accurately

- consider putting most of this logic inside MountableRegion instead of Player (just think about it)


