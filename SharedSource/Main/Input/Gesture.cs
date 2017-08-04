using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Codigo
{
    [DataContract]
    /**
     * <summary>
     * This class represents an action that must be performed, for example, keep pressed a button, 
     * or only the moment it is pressed or released
     * </summary>
     */
    public enum Gesture
    {
        [EnumMember]
        Pressed,
        [EnumMember]
        Released,
        [EnumMember]
        Down,
        [EnumMember]
        Up
    }
}
