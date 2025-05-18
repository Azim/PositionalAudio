# PositionalAudio
Mumble positional audio integration.

# Installing Mumble
https://www.mumble.info/downloads/

# Configuring Mumble

Open **Settings** -> **Audio Output** -> **Positional Audio**\
Make sure **Enable** and **Headphones** are enabled.\
Copy following settings: 

* Minimum Distance: 4.0m   (Minimum distance before the volume fallof starts)
* Maximum Distance: 100.0m (Adjust depending on how far you want to hear other players from)
* Minimum Volume: 0%       (Make sure to set to 0, otherwise you will always hear other players)
* Bloom: 35%               (If player is directly to the side of you, this determines how much of that sound is audible on the other ear, adjust to taste)

Open **Settings** -> **Plugins** \
Make sure **Link to Game and Transmit Position** is enabled.\
Scroll down and find **Link** plugin, make sure **Enable**, **PA** and **KeyEvents** are enabled for it.

Dont forget to click **Apply** before closing.


# Special thanks:
WWYDF and his [GTFO OpenPA mod](https://github.com/WWYDF/OpenPA), which i used as a reference for making this mod.

Xikrili and CharlesE2 for helping me test and troubleshoot during the development.

