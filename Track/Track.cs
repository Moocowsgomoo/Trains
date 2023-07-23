using Godot;
using System;
using System.Collections.Generic;

public enum SwitchDirection{
    NONE,
    LEFT,
    RIGHT,
    ALL
}

public partial class Track : Path2D
{
    [Export] public SwitchData entryData;
    [Export] public SwitchData exitData;

    List<Track> outboundTracks=new();

    public override void _Ready()
    {
        base._Ready();
        //GD.Print(enterSwitch.ResourcePath + " // " + enterSwitch.connection + " // " + this);
        if (entryData != null){
            GetNode<Track>(entryData.track).AddOutboundTrack(this);
        }
    }

    public void AddOutboundTrack(Track track){
        //GD.Print(this.Name + " Add Connection");
        outboundTracks.Add(track);
        outboundTracks.Sort((x,y)=>(int)(x.entryData.distance - y.entryData.distance));
    }

    public Track GetNextOutboundTrack(float distance){
        foreach(Track track in outboundTracks){
            if (track.entryData.distance < distance) continue;
            else return track;
        }
        return null;
    }

    public Track GetDestinationTrack(){
        return GetNode<Track>(exitData.track);
    }

    public float GetDistanceFromStart(){
        if (GetNode<Track>(entryData.track) == this) return 0f;
        else return entryData.distance + GetNode<Track>(entryData.track).GetDistanceFromStart();
    }
}
