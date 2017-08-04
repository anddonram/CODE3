using System.Runtime.Serialization;

namespace Codigo
{
    /**
     * <summary>
     * Represents a list of possible commands. 
     * Each enum must be EnumMember for the keys to work
     * </summary>
     */
    [DataContract]
    public enum CommandEnum
    {
        [EnumMember]
        CameraUp,
        [EnumMember]
        CameraDown,
        [EnumMember]
        CameraRight,
        [EnumMember]
        CameraLeft, [EnumMember]
        CameraCenter, [EnumMember]
        StartPath, [EnumMember]
        AddTile, [EnumMember]
        FinishPath, [EnumMember]
        ActInTile, [EnumMember]
        Chop, [EnumMember]
        Fight, [EnumMember]
        Build, [EnumMember]
        Heal, [EnumMember]
        Switch, [EnumMember]
        EnterCottage, [EnumMember]
        Train, [EnumMember]
        ZoomIn, [EnumMember]
        ZoomOut, [EnumMember]
        CottageOne, [EnumMember]
        CottageTwo, [EnumMember]
        CottageThree, [EnumMember]
        CottageFour, [EnumMember]
        CottageFive, [EnumMember]
        CottageSix, [EnumMember]
        Stop, [EnumMember]
        Destroy, [EnumMember]
        Select, [EnumMember]
        StopCreation, [EnumMember]
        Create, [EnumMember]
        ToggleFog, [EnumMember]
        Cottage, [EnumMember]
        Candle, [EnumMember]
        FakeTree, [EnumMember]
        Bridge, [EnumMember]
        Trap, [EnumMember]
        Rock, [EnumMember]
        Tree, [EnumMember]
        Castle, [EnumMember]
        Water, [EnumMember]
        Person,
    }
}