using System.Xml.Serialization;

namespace AutomatedRodentTracker.Model.Behaviours
{
    public enum InteractionBehaviour
    {
        [XmlEnum(Name="Started")]
        Started,
        [XmlEnum(Name="Ended")]
        Ended,
    }
}
