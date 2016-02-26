# Lazy ImageViewer

A .NET image viewer run in WPF. It only supports whatever image formats that
the WPF ImageView normally works with, and it only ever displays the first
frame of GIFs.

The primary purpose of this is to view sequential images without having to touch
the mouse. Basically, it rips off a bunch of controls from IrfanView.

I just wanted to read comics lazily, okay?

Despite this, mouse click-and-drag scrolling and scrollwheel scrolling *does* in
fact scroll the image.

## Controls
Main Window

* **Left Arrow** &mdash; Previous image. Opens directory picker if the current image is
  the first image.
* **Right Arrow** &mdash; Next image. Opens directory picker if the current image is the
  last image.
* **Up/Down Arrow** &mdash; Scrolls image up/down.
* **Page Up/Down** &mdash; Scrolls image up/down in large amounts
* **Home/End** &mdash; Go to first/last image in directory
* **Ctrl+Left/Right Arrow** &mdash; Go to first/last image in directory
* **Space** &mdash; Toggle between stretch modes: Fill width, Shrink to fit width, Shrink to fit both
* **Enter** &mdash; Toggle maximize
* **Ctrl+Up** &mdash; Opens directory picker
* **Ctrl+Down** &mdash; Toggles stretch modes like **Space**.

Directory Picker

* **Left Arrow** &mdash; Exit directory
* **Right Arrow** &mdash; Enter directory
* **Up/Down Arrow** &mdash; Move selector between directories up/down
* **Page Up/Down** &mdash; Jump selector over many directories up/down
* **Ctrl+Up/Down Arrow** &mdash; Jump to first/last directory in current directory
