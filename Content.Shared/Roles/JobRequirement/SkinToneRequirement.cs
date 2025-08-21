// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Diagnostics.CodeAnalysis;
using Content.Shared.Humanoid;
using Content.Shared.Preferences;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared._Slon.Roles;

/// <summary>
/// Requires the character's human skin tone to be less than or equal to a configured threshold (0-100).
/// See <see cref="SkinColor.HumanSkinToneFromColor"/> for mapping details.
/// </summary>
[UsedImplicitly]
[Serializable, NetSerializable]
public sealed partial class SkinToneRequirement : JobRequirement
{
    /// <summary>
    /// Maximum allowed human skin tone value (0-100).
    /// </summary>
    [DataField(required: true)]
    public int MaxHumanSkinTone { get; set; }

    public override bool Check(IEntityManager entManager,
        IPrototypeManager protoManager,
        HumanoidCharacterProfile? profile,
        IReadOnlyDictionary<string, TimeSpan> playTimes,
        [NotNullWhen(false)] out FormattedMessage? reason)
    {
        reason = null;

        if (profile is null)
            return true;

        var tone = SkinColor.HumanSkinToneFromColor(profile.Appearance.SkinColor);

        var failed = Inverted
            ? tone <= MaxHumanSkinTone
            : tone > MaxHumanSkinTone;

        if (!failed)
            return true;

        var message = Inverted
            ? Loc.GetString("role-requirements-skin-tone-inverted", ("tone", MaxHumanSkinTone))
            : Loc.GetString("role-requirements-skin-tone", ("tone", MaxHumanSkinTone));

        reason = FormattedMessage.FromMarkupPermissive(message);
        return false;
    }
}


