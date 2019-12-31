using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class NetworkedObjectId
{
    public int Id;

    public override int GetHashCode()
    {
        return Id;
    }

    public override bool Equals(object obj)
    {
        if (obj is InstantiatedObjectId) return false;
        var other = (PrimevalObjectId)obj;
        return Id == other.Id;
    }
}

/// <summary>
/// Uniquely identifies an object that existed at the moment the scene was loaded
/// </summary>
[Serializable]
public class PrimevalObjectId : NetworkedObjectId
{
    
}

/// <summary>
/// Uniquely identifies an object that was instantiated during runtime.
/// </summary>
[Serializable]
public class InstantiatedObjectId : NetworkedObjectId
{
    public string UserId;
    public int SequenceNum;

    public override int GetHashCode()
    {
        return UserId.GetHashCode() ^ SequenceNum;
    }

    public override bool Equals(object obj)
    {
        if (obj is PrimevalObjectId) return false;
        var other = (InstantiatedObjectId)obj;
        return UserId == other.UserId && SequenceNum == other.SequenceNum;
    }
}
