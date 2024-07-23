# Livelox2png
Utility to extract a map with course from Livelox, saving it as a png.

## Usage
Run the built executable. Follow the prompted instructions

## Configuration
Edit the `appsettings.json` file. Livelox2png will try to match `DefaultPerson` to determine which course to draw.

## Missing features, issues & notes
* Authentication isn't implemented – activities not publicly available cannot be fetched
* No plotting of runners actual path as of yet
* No clipping of control rings
* No nice display of control numbers on reused controls (loops)