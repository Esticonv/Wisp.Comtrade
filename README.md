# Wisp.Comtrade
COMTRADE parsing library for Net4.5



# Example

//using Wisp.Comtrade;

var record=new Record(@"c:\file.cfg");

var timeLine=record.GetTimeLine();

int channelIndex=0;

var analog_channel_1=record.GetAnalogPrimaryChannel(channelIndex);
