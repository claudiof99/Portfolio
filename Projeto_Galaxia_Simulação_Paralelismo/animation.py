"""
animation.py - GIF Assembly

Reads all PNG frames from the frames/ directory and combines them
into an animated GIF.

No MPI needed — called by rank 0 only after the simulation ends.

"""

import os
import shutil
import subprocess
import glob

from PIL import Image

def _normalize_path(path: str) -> str:
    return path.replace("\\", "/")

def build_gif(frames_dir: str, output_path: str, fps: int = 15) -> None:
    """
    Combine all PNG frames into a single animated GIF.

    Frames are sorted lexicographically — because filenames are zero-padded
    (frame_0000000.png, frame_0001000.png, …) this is equivalent to
    numeric order and gives the correct temporal sequence.

    Args:
        frames_dir  : directory containing frame_XXXXXXX.png files
        output_path : path to write the output GIF
        fps         : frames per second for the animation
    """
    pattern = os.path.join(frames_dir, "frame_*.png")
    frames  = sorted(glob.glob(pattern))


    if not frames:
        print(f"[animation] No frames found in '{frames_dir}'. Skipping GIF.")
        return

    os.makedirs(os.path.dirname(output_path) or ".", exist_ok=True)

    print(f"[animation] Assembling GIF from {len(frames)} frames "
          f"at {fps} fps...")
    
    if shutil.which("ffmpeg") is not None:
        print("[animation] FFmpeg available. Using it for GIF creation.")
        try:
            _create_gif_with_ffmpeg(frames_dir, output_path, fps)
        except (subprocess.CalledProcessError, OSError) as exc:
            print(f"[animation] FFmpeg failed ({exc}). Falling back to PIL.")
            _create_gif_with_pil(frames, output_path, fps)
    else:
        print("[animation] FFmpeg not found. Using PIL for GIF creation.")
        _create_gif_with_pil(frames, output_path, fps)

    size_mb = os.path.getsize(output_path) / 1e6
    print(f"[animation] GIF saved: {output_path}  ({size_mb:.1f} MB, "
          f"{len(frames)} frames)")

def _create_gif_with_ffmpeg(frames_dir: str, output_path: str, fps: int = 15) -> None:
    pattern = os.path.join(frames_dir, "frame_*.png")
    frames = sorted(glob.glob(pattern))
    frames_cwd = os.path.abspath(frames_dir)
    palette = "palette.png"
    list_file = "frames_list.txt"
    output_abs = os.path.abspath(output_path)

    print(f"[animation] Assembling GIF from {len(frames)} frames "
          f"at {fps} fps using FFmpeg...")

    # Use basename-only paths and run ffmpeg from frames_dir so Windows ffmpeg
    # does not need to parse parent folders with non-ASCII characters.
    with open(os.path.join(frames_cwd, list_file), "w", encoding="utf-8") as f:
        for frame in frames:
            f.write(f"file '{os.path.basename(frame)}'\n")
            f.write(f"duration {1.0 / fps}\n")

    palette_cmd = [
        "ffmpeg", "-y",
        "-f", "concat", "-safe", "0",
        "-i", list_file,
        "-vf", "palettegen=stats_mode=diff",
        "-update", "1",
        palette,
    ]
    gif_cmd = [
        "ffmpeg", "-y",
        "-f", "concat", "-safe", "0",
        "-i", list_file,
        "-i", palette,
        "-filter_complex", "paletteuse=dither=bayer:bayer_scale=3",
        "-r", str(fps),
        "-t", str(len(frames) / fps),
        "-loop", "0",
        _normalize_path(output_abs),
    ]

    subprocess.run(palette_cmd, check=True, cwd=frames_cwd)
    subprocess.run(gif_cmd, check=True, cwd=frames_cwd)

    os.remove(os.path.join(frames_cwd, palette))
    os.remove(os.path.join(frames_cwd, list_file))

    
def _create_gif_with_pil(frames: list, output_path: str, fps: int = 15) -> None:

    print(f"[animation] Assembling GIF from {len(frames)} frames "f"at {fps} fps using PIL...")

    images = [Image.open(f).convert("RGBA") for f in frames]
    images[0].save(
        output_path, 
        save_all=True, 
        append_images=images[1:], 
        duration=int(1000/fps), 
        loop=0)
