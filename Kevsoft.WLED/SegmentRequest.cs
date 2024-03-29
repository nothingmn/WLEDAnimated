﻿using System.Text.Json.Serialization;

namespace Kevsoft.WLED;

public sealed class SegmentRequest
{
    /// <inheritdoc cref="SegmentResponse.Id"/>
    [JsonPropertyName("id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Id { get; set; }

    /// <inheritdoc cref="SegmentResponse.Name"/>
    [JsonPropertyName("n")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Name { get; set; }

    /// <inheritdoc cref="SegmentResponse.Start"/>
    [JsonPropertyName("start")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Start { get; set; }

    /// <inheritdoc cref="SegmentResponse.Stop"/>
    [JsonPropertyName("stop")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Stop { get; set; }

    /// <inheritdoc cref="SegmentResponse.Length"/>
    [JsonPropertyName("len")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Length { get; set; }

    /// <inheritdoc cref="SegmentResponse.Group"/>
    [JsonPropertyName("grp")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Group { get; set; }

    /// <inheritdoc cref="SegmentResponse.Spacing"/>
    [JsonPropertyName("spc")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Spacing { get; set; }

    /// <inheritdoc cref="SegmentResponse.Offset"/>
    [JsonPropertyName("of")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Offset { get; set; }

    /// <inheritdoc cref="SegmentResponse.Colors"/>
    [JsonPropertyName("col")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int[][]? Colors { get; set; }

    /// <inheritdoc cref="SegmentResponse.EffectId"/>
    [JsonPropertyName("fx")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? EffectId { get; set; }

    /// <inheritdoc cref="SegmentResponse.EffectSpeed"/>
    [JsonPropertyName("sx")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? EffectSpeed { get; set; }

    /// <inheritdoc cref="SegmentResponse.EffectIntensity"/>
    [JsonPropertyName("ix")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? EffectIntensity { get; set; }


    /// <inheritdoc cref="SegmentResponse.EffectCustomSlider1"/>
    [JsonPropertyName("c1")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? EffectCustomSlider1 { get; set; }

    /// <inheritdoc cref="SegmentResponse.EffectCustomSlider2"/>
    [JsonPropertyName("c2")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? EffectCustomSlider2 { get; set; }

    /// <inheritdoc cref="SegmentResponse.EffectCustomSlider3"/>
    [JsonPropertyName("c3")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? EffectCustomSlider3 { get; set; }

    /// <inheritdoc cref="SegmentResponse.EffectOption1"/>
    [JsonPropertyName("o1")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? EffectOption1 { get; set; }

    /// <inheritdoc cref="SegmentResponse.EffectOption2"/>
    [JsonPropertyName("o2")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? EffectOption2 { get; set; }

    /// <inheritdoc cref="SegmentResponse.EffectOption3"/>
    [JsonPropertyName("o3")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? EffectOption3 { get; set; }




    /// <inheritdoc cref="SegmentResponse.ColorPaletteId"/>
    [JsonPropertyName("pal")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? ColorPaletteId { get; set; }

    /// <inheritdoc cref="SegmentResponse.Selected"/>
    [JsonPropertyName("sel")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Selected { get; set; }

    /// <inheritdoc cref="SegmentResponse.Reverse"/>
    [JsonPropertyName("rev")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Reverse { get; set; }

    /// <inheritdoc cref="SegmentResponse.SegmentState"/>
    [JsonPropertyName("on")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? SegmentState { get; set; }

    /// <inheritdoc cref="SegmentResponse.Brightness"/>
    [JsonPropertyName("bri")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Brightness { get; set; }

    /// <inheritdoc cref="SegmentResponse.Mirror"/>
    [JsonPropertyName("mi")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Mirror { get; set; }

    /// <inheritdoc cref="SegmentResponse.Mirror"/>
    [JsonPropertyName("mY")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Mirror2D { get; set; }

    /// <inheritdoc cref="SegmentResponse.TransposeSegment"/>
    [JsonPropertyName("tp")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? TransposeSegment { get; set; }

    /// <inheritdoc cref="SegmentResponse.ColorTemperature"/>
    [JsonPropertyName("cct")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? ColorTemperature { get; set; }

    /// <inheritdoc cref="SegmentResponse.LoxonePrimaryRGB"/>
    [JsonPropertyName("lx")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? LoxonePrimaryRGB { get; set; }

    /// <inheritdoc cref="SegmentResponse.LoxoneSecondaryRGB"/>
    [JsonPropertyName("ly")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? LoxoneSecondaryRGB { get; set; }

    /// <inheritdoc cref="SegmentResponse.IndividualLEDControl"/>
    [JsonPropertyName("i")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Array? IndividualLEDControl { get; set; }

    /// <inheritdoc cref="SegmentResponse.FreezeEffect"/>
    [JsonPropertyName("frz")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? FreezeEffect { get; set; }

    /// <inheritdoc cref="SegmentResponse.Expand1DFX"/>
    [JsonPropertyName("m12")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Expand1DFX { get; set; }

    /// <inheritdoc cref="SegmentResponse.SoundSimulationType"/>
    [JsonPropertyName("si")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? SoundSimulationType { get; set; }

    /// <inheritdoc cref="SegmentResponse.LoadEffectDefaults"/>
    [JsonPropertyName("fxdef")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? LoadEffectDefaults { get; set; }

    /// <inheritdoc cref="SegmentResponse.SetID"/>
    [JsonPropertyName("set")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? SetID { get; set; }

    /// <inheritdoc cref="SegmentResponse.RepeatSegmentSettings"/>
    [JsonPropertyName("rpt")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? RepeatSegmentSettings { get; set; }

    public static SegmentRequest From(SegmentResponse segmentResponse)
    {
        return new SegmentRequest
        {
            Id = segmentResponse.Id,
            Start = segmentResponse.Start,
            Stop = segmentResponse.Stop,
            Length = segmentResponse.Length,
            Group = segmentResponse.Group,
            Spacing = segmentResponse.Spacing,
            Offset = segmentResponse.Offset,
            Colors = segmentResponse.Colors,
            EffectId = segmentResponse.EffectId,
            EffectSpeed = segmentResponse.EffectSpeed,
            EffectIntensity = segmentResponse.EffectIntensity,
            ColorPaletteId = segmentResponse.ColorPaletteId,
            Selected = segmentResponse.Selected,
            Reverse = segmentResponse.Reverse,
            SegmentState = segmentResponse.SegmentState,
            Brightness = segmentResponse.Brightness,
            Mirror = segmentResponse.Mirror
        };
    }

    public static implicit operator SegmentRequest(SegmentResponse rhs) => From(rhs);
}