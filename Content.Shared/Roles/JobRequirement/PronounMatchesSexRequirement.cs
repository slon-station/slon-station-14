// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Diagnostics.CodeAnalysis;
using Content.Shared.Preferences;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;
using Content.Shared.Humanoid;

namespace Content.Shared._Slon.Roles;

/// <summary>
/// Requires that the selected pronouns (Gender) match the character's Sex.
/// - Sex.Male   -> Gender.Male (he/him)
/// - Sex.Female -> Gender.Female (she/her)
/// Any other combinations (including Epicene/Neuter) are not allowed.
/// </summary>
[UsedImplicitly]
[Serializable, NetSerializable]
public sealed partial class PronounMatchesSexRequirement : JobRequirement
{
    public override bool Check(IEntityManager entManager,
        IPrototypeManager protoManager,
        HumanoidCharacterProfile? profile,
        IReadOnlyDictionary<string, TimeSpan> playTimes,
        [NotNullWhen(false)] out FormattedMessage? reason)
    {
        reason = null;

        if (profile is null)
            return true;

        var sex = profile.Sex;
        var gender = profile.Gender;

        var allowed = (sex == Sex.Male && gender == Gender.Male)
                   || (sex == Sex.Female && gender == Gender.Female);

        if (allowed)
            return true;

        reason = FormattedMessage.FromMarkupPermissive(
            Loc.GetString("role-requirements-pronouns-mismatch"));
        return false;
    }
}


