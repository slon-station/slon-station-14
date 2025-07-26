using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Goobstation.Shared.Gangs;

[RegisterComponent]
public sealed partial class GangSprayComponent : Component
{
    [DataField]
    public string GangSignPrototype = "RandomGangSignAny";

    [DataField]
    public float SprayTime = 7f;
}
