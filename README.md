# Simple 3D Video Player

Simple 180 SBS Video Player, mainly for Xreal Air to watch VR videos. Using VLC for Unity and Unity Video Player (each has its own issues).

## File Arguments

The following arguments can be used with the video player:

- `"Simple 3D Player.exe" --file "<path>"`: Specifies the path to the video file to be played.

## Players

The project has two players: VLC for Unity and the official video player.

VLC is the default player, which works amazing for high resolution videos, while the official may stutter heavily on those.

But VLC enforces its internal 3D viewer when a specific metadata is present in the video file, conflicting with our 3D viewer.

The only way is to strip those metadata, which can be done with `ffmpeg`:

```sh
ffmpeg -i input.mp4 -map 0 -c copy -map_metadata 0 -metadata:s:v:0 side_data_type=none -metadata:s:v:1 side_data_type=none output.mp4
```
It will only remove the side-by-side metadata from the video file, while keeping the rest of the metadata and video file intact.

Or you can use the [bash script inside the Misc folder](Misc/strip-side-metadata.bat) by simply drag-n-dropping a video file onto it.