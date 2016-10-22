SimpleBMSPlayer.BASS
====================

This is the same sample for SimpleBMSPlayer. Instead of NAudio, this version uses [Un4seen BASS](http://www.un4seen.com/) to demostrate how to play a BMS file automatically.

To try it, simply build this project, grab a bass.dll depends on your platform from Un4seen website and extract it on your debug/release folder, drag the BMS/BME/BML/PMS/Bmson file(s) to the built binary to see the result.

Because of the license, it does not conatins any source/binaries from BASS. And the wrapper used in this demo is [ManagedBass.PInvoke](https://github.com/ManagedBass/ManagedBass.PInvoke).