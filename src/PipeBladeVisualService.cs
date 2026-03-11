using Godot;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace RegentWithASteelPipe;

internal static class PipeBladeVisualService
{
    private static readonly Vector2 OverrideVisualOffset = new(0f, 0f);

    private const float OverrideScaleMultiplier = 3f;

    private const string BladeNodePath = "SpineSword/SwordBone/ScaleContainer/Blade";

    private const string SpineSwordPath = "SpineSword";

    private const string TrailPath = "Trail";

    private const string OriginalHitboxPath = "SpineSword/Hitbox";

    private const string DetachedHitboxPath = "Hitbox";

    private const string OverrideNodeName = "PipeBladeOverride";

    private const string HitboxOffsetMeta = "regent_pipe_hitbox_offset";

    internal static void Initialize(NSovereignBladeVfx instance)
    {
        DetachHitbox(instance);
        HideOriginal(instance);
        EnsureOverride(instance);
        Update(instance);
    }

    internal static void Update(NSovereignBladeVfx instance)
    {
        HideOriginal(instance);

        Sprite2D? sourceBlade = instance.GetNodeOrNull<Sprite2D>(BladeNodePath);
        Sprite2D? overrideBlade = instance.GetNodeOrNull<Sprite2D>(OverrideNodeName);
        Control? hitbox = instance.GetNodeOrNull<Control>(DetachedHitboxPath);
        Texture2D? texture = CustomAssetService.BladeTexture;
        if (sourceBlade == null || overrideBlade == null || texture == null)
        {
            return;
        }

        if (hitbox != null && instance.HasMeta(HitboxOffsetMeta))
        {
            Vector2 hitboxOffset = instance.GetMeta(HitboxOffsetMeta).AsVector2();
            hitbox.GlobalPosition = sourceBlade.GlobalPosition + hitboxOffset;
            hitbox.Visible = true;
            hitbox.MouseFilter = Control.MouseFilterEnum.Stop;
        }

        overrideBlade.Texture = texture;
        overrideBlade.Visible = true;
        overrideBlade.GlobalPosition = hitbox != null
            ? hitbox.GlobalPosition + hitbox.Size * 0.5f + OverrideVisualOffset
            : sourceBlade.GlobalPosition;
        overrideBlade.GlobalRotation = sourceBlade.GlobalRotation;
        overrideBlade.GlobalScale = sourceBlade.GlobalScale * OverrideScaleMultiplier;
        overrideBlade.Modulate = Colors.White;
    }

    private static void EnsureOverride(NSovereignBladeVfx instance)
    {
        if (instance.GetNodeOrNull<Sprite2D>(OverrideNodeName) != null)
        {
            return;
        }

        Texture2D? texture = CustomAssetService.BladeTexture;
        if (texture == null)
        {
            return;
        }

        Sprite2D blade = new()
        {
            Name = OverrideNodeName,
            Texture = texture,
            Centered = true,
            TopLevel = true,
            Visible = true,
            Modulate = Colors.White
        };

        instance.AddChild(blade);
    }

    private static void HideOriginal(NSovereignBladeVfx instance)
    {
        if (instance.GetNodeOrNull<CanvasItem>(SpineSwordPath) is { } spineSword)
        {
            spineSword.Visible = false;
        }

        SetHidden(instance.GetNodeOrNull<Line2D>(TrailPath));
    }

    private static void DetachHitbox(NSovereignBladeVfx instance)
    {
        if (instance.GetNodeOrNull<Control>(DetachedHitboxPath) is not { } hitbox)
        {
            hitbox = instance.GetNodeOrNull<Control>(OriginalHitboxPath);
            if (hitbox == null)
            {
                return;
            }

            Sprite2D? sourceBlade = instance.GetNodeOrNull<Sprite2D>(BladeNodePath);
            Vector2 offset = sourceBlade == null ? Vector2.Zero : hitbox.GlobalPosition - sourceBlade.GlobalPosition;
            instance.SetMeta(HitboxOffsetMeta, Variant.From(offset));

            Node? parent = hitbox.GetParent();
            parent?.RemoveChild(hitbox);
            instance.AddChild(hitbox);
        }

        hitbox.Visible = true;
        hitbox.MouseFilter = Control.MouseFilterEnum.Stop;
        if (hitbox.GetNodeOrNull<NSelectionReticle>("SelectionReticle") is { } selectionReticle)
        {
            selectionReticle.Visible = false;
            selectionReticle.Modulate = Colors.Transparent;
            selectionReticle.OnDeselect();
        }
    }

    private static void SetHidden(CanvasItem? node)
    {
        if (node != null)
        {
            node.Visible = false;
        }
    }
}
