from __future__ import annotations

import math
import os
import sys
from pathlib import Path

from PIL import Image


TARGET_BLADE_SIZE = (50, 236)


def main() -> int:
    if len(sys.argv) != 2:
        print("Expected 1 arg: <project_root>", file=sys.stderr)
        return 1

    project_root = Path(sys.argv[1])
    source_path = project_root / "pack" / "images" / "steel_pipe_blade.png"
    output_path = project_root / "pack" / "images" / "steel_pipe_blade_runtime.png"

    image = Image.open(source_path).convert("RGBA")
    rotated = rotate_pipe_vertical(image)
    alpha_bbox = rotated.getchannel("A").getbbox()
    if alpha_bbox is None:
        raise RuntimeError(f"No opaque pixels found in {source_path}")

    cropped = rotated.crop(alpha_bbox)
    resized = cropped.resize(TARGET_BLADE_SIZE, Image.Resampling.LANCZOS)
    resized.save(output_path)
    print(f"Prepared {output_path} -> {resized.width}x{resized.height}")
    return 0


def rotate_pipe_vertical(image: Image.Image) -> Image.Image:
    alpha = image.getchannel("A")
    width, height = alpha.size
    pixels = alpha.load()

    total = 0.0
    mean_x = 0.0
    mean_y = 0.0
    for y in range(height):
        for x in range(width):
            value = pixels[x, y]
            if value == 0:
                continue
            weight = value / 255.0
            total += weight
            mean_x += x * weight
            mean_y += y * weight

    if total == 0.0:
        return image

    mean_x /= total
    mean_y /= total

    cov_xx = 0.0
    cov_xy = 0.0
    cov_yy = 0.0
    for y in range(height):
        for x in range(width):
            value = pixels[x, y]
            if value == 0:
                continue
            weight = value / 255.0
            dx = x - mean_x
            dy = y - mean_y
            cov_xx += dx * dx * weight
            cov_xy += dx * dy * weight
            cov_yy += dy * dy * weight

    angle = 0.5 * math.atan2(2.0 * cov_xy, cov_xx - cov_yy)
    angle_degrees = math.degrees(angle)
    rotate_degrees = 90.0 - angle_degrees
    return image.rotate(rotate_degrees, resample=Image.Resampling.BICUBIC, expand=True)


if __name__ == "__main__":
    raise SystemExit(main())
