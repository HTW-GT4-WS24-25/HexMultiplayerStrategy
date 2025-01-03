# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

<!-- Headers should be listed in this order: Added, Changed, Deprecated, Removed, Fixed, Security -->

## [1.3.1] - 2024-12-21

### Fixed

- Added com.unity.collections as a dependency since it is needed for the SmoothNormalsBaker to work
- Edge Detection: Fixes for masks/fills
- Edge Detection: Reduced number of shader variants
- Edge Detection: Fixed compilation error in section shader

## [1.3.0] - 2024-12-18

### Added

- Edge Detection: Added option to scale edge thickness with screen resolution
- Edge Detection: Added option to fade edge color with depth
- Edge Detection: Added section map support for particles
- Fast Outline: Added smoothed normals baker allowing for rendering smoother outlines with fewer artifacts
- Compatibility Window: Added check to detect outline overrides in the scene which break SRP batching
- GPU Instancing: Added GPU instancing option for improved performance when rendering many different outline variants at once
- Wide Outline: Added vertex animation support
- Soft Outline: Added vertex animation support

### Changed

- Edge Detection: Reorganized settings
- Edge Detection: Simplified debug view options
- Edge Detection: Changed precision of section map texture from 8 bit to 16 bit precision

### Fixed

- Compatibility Window: Fixed styling for light mode of Unity Editor
- MSAA + Soft Outline: Fixed rendering issues when MSAA is enabled for soft outline
- MSAA + Wide Outline: Fixed console errors when MSAA is enabled for wide outline (rendering artifacts are still present!)
- Surface Fill: Fixed rotation values not being applied correctly for texture patterns
- Unity 6: Fixed stencil rendering issue 

## [1.2.6] - 2024-11-27

### Fixed

- Unity 2022: Fixed an issue with profiling samplers being created every frame, potentially causing a crash in builds

## [1.2.5] - 2024-11-17

### Added

- Added BeforeRenderingTransparents as outline injection point

## [1.2.4] - 2024-11-13

### Added

- Soft outline: Added scale-with-resolution option for soft outline resulting in better performance at higher resolutions

### Fixed

- Android: Fixed graphics format not being supported

## [1.2.3] - 2024-11-09

### Fixed

- Edge Detection: Fixed edge detection not rendering on Unity 6000.0.22f1 or newer and Unity 2022.3.49f1 or newer

## [1.2.2] - 2024-11-06

### Changed

- Edge Detection: Changed default background to clear instead of white

### Fixed

- Edge Detection: Fixed masking not working
- Fixed potential UnassignedReferenceExceptions when outline/fill material was not assigned
- Fixed package samples missing scripts and materials

## [1.2.1] - 2024-11-03

### Added

- Added custom property drawer for rendering layer mask in Unity 2022

### Fixed

- Wide Outline: Fixed Wide Outline not working with render scales different from 1
- Compatibility Check: Fixed error when using compatibility check in a project using a 2D renderer

## [1.2.0] - 2024-10-25

### Added

- Wide Outline: Added alpha cutout support
- Soft Outline: Added alpha cutout support
- WebGL: Added support for WebGL (except for Soft Outline)
- iOS: Added support for iOS
- Added the SetActive method for enabling/disabling outlines through code

### Fixed

- Fixed typos

## [1.1.1] - 2024-10-12

### Fixed

- Fixed a compilation error on older version of Unity 2022.3

## [1.1.0] - 2024-10-07

### Added

- Added support for Unity 2022.3
- Added support for Unity 6 with compatibility mode enabled
- Added (experimental) support for the DOTS Hybrid Renderer
- Compatibility Check: Added new compatibility check window to see if Linework will work with your project
- Added option to create outline settings directly from within the renderer feature UI

### Removed

- Removed unused code
- Removed old 'About and Support' window

### Fixed

- Fixed various memory leaks and unnecessary memory allocations

## [1.0.0] - 2024-08-25

### Added

- Fast Outline: Added the Fast Outline effect for rendering simple outlines using vertex extrusion
- Soft Outline: Added the Soft Outline effect for rendering soft and glowy outlines
- Wide Outline: Added the Wide Outline effect for rendering consistent and smooth outlines
- Edge Detection: Added the Edge Detection effect for rendering a full-screen outline effect that applies to the whole scene
- Surface Fill: Added the Surface Fill effect for rendering screen-space fill effects and patterns