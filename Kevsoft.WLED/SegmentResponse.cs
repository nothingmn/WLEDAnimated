using System.Text.Json.Serialization;

namespace Kevsoft.WLED;

public sealed class SegmentResponse
{
    /// <summary>
    /// Zero-indexed ID of the segment. May be omitted, in that case the ID will be inferred from the order of the segment objects in the seg array. As such, not included in state response.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// Name of the segment, also used to control scrolling text
    /// </summary>
    [JsonPropertyName("n")]
    public string? Name { get; set; }

    /// <summary>
    /// LED the segment starts at.
    /// </summary>
    [JsonPropertyName("start")]
    public int Start { get; set; }

    /// <summary>
    /// LED the segment stops at, not included in range. If stop is set to a lower or equal value than start (setting to 0 is recommended), the segment is invalidated and deleted.
    /// </summary>
    [JsonPropertyName("stop")]
    public int Stop { get; set; }

    /// <summary>
    /// Length of the segment (stop - start). stop has preference, so if it is included, len is ignored.
    /// </summary>
    [JsonPropertyName("len")]
    public int Length { get; set; }

    /// <summary>
    /// Grouping (how many consecutive LEDs of the same segment will be grouped to the same color)
    /// </summary>
    [JsonPropertyName("grp")]
    public int Group { get; set; }

    /// <summary>
    /// Spacing (how many LEDs are turned off and skipped between each group)
    /// </summary>
    [JsonPropertyName("spc")]
    public int Spacing { get; set; }

    /// <summary>
    /// Offset (how many LEDs to rotate the virtual start of the segments, available since 0.13.0)
    /// </summary>
    [JsonPropertyName("of")]
    public int Offset { get; set; }

    /// <summary>
    /// Array that has up to 3 color arrays as elements, the primary, secondary (background) and tertiary colors of the segment. Each color is an array of 3 or 4 bytes, which represent an RGB(W) color.
    /// </summary>
    [JsonPropertyName("col")]
    public int[][] Colors { get; set; } = null!;

    /// <summary>
    /// ID of the effect.
    /// </summary>
    [JsonPropertyName("fx")]
    public int EffectId { get; set; }

    /// <summary>
    /// Relative effect speed
    /// </summary>
    [JsonPropertyName("sx")]
    public int EffectSpeed { get; set; }

    [JsonPropertyName("ix")]
    public int EffectIntensity { get; set; }

    /// <summary>
    /// ID of the color palette
    /// </summary>
    [JsonPropertyName("pal")]
    public int ColorPaletteId { get; set; }

    /// <summary>
    /// true if the segment is selected. Selected segments will have their state (color/FX) updated by APIs that don't support segments.
    /// </summary>
    [JsonPropertyName("sel")]
    public bool Selected { get; set; }

    /// <summary>
    /// Flips the segment, causing animations to change direction.
    /// </summary>
    [JsonPropertyName("rev")]
    public bool Reverse { get; set; }

    /// <summary>
    /// Turns on and off the individual segment. (available since 0.10.0)
    /// </summary>
    [JsonPropertyName("on")]
    public bool SegmentState { get; set; }

    /// <summary>
    /// Sets the individual segment brightness (available since 0.10.0)
    /// </summary>
    [JsonPropertyName("bri")]
    public int Brightness { get; set; }

    /// <summary>
    /// Mirrors the segment (available since 0.10.2)
    /// </summary>
    [JsonPropertyName("mi")]
    public bool Mirror { get; set; }

    /// <summary>
    /// Mirrors the 2D segment in vertical dimension. (available since 0.14.0)
    /// </summary>
    [JsonPropertyName("mY")]
    public bool Mirror2D { get; set; }

    /// <summary>
    /// Transposes a segment (swaps X and Y dimensions). (available since 0.14.0)
    /// </summary>
    [JsonPropertyName("tp")]
    public bool TransposeSegment { get; set; }

    /// <summary>
    /// White spectrum color temperature (available since 0.13.0)
    /// </summary>
    [JsonPropertyName("cct")]
    public int ColorTemperature { get; set; }

    /// <summary>
    /// Loxone RGB value for primary color. Each color (RRR,GGG,BBB) is specified in the range from 0 to 100%. Only available if Loxone is compiled in.
    /// </summary>
    [JsonPropertyName("lx")]
    public string? LoxonePrimaryRGB { get; set; }

    /// <summary>
    /// Loxone RGB value for secondary color. Each color (RRR,GGG,BBB) is specified in the range from 0 to 100%. Only available if Loxone is compiled in.
    /// </summary>
    [JsonPropertyName("ly")]
    public string? LoxoneSecondaryRGB { get; set; }

    /// <summary>
    /// Individual LED control. Not included in state response (available since 0.10.2)
    /// </summary>
    [JsonPropertyName("i")]
    public Array? IndividualLEDControl { get; set; }

    /// <summary>
    /// Freezes/unfreezes the current effect
    /// </summary>
    [JsonPropertyName("frz")]
    public bool FreezeEffect { get; set; }

    /// <summary>
    /// Setting of segment field 'Expand 1D FX'. (0: Pixels, 1: Bar, 2: Arc, 3: Corner)
    /// </summary>
    [JsonPropertyName("m12")]
    public int Expand1DFX { get; set; }

    /// <summary>
    /// Setting of the sound simulation type for audio enhanced effects. (0: 'BeatSin', 1: 'WeWillRockYou', 2: '10_3', 3: '14_3') (as of 0.14.0-b1, there are these 4 types defined)
    /// </summary>
    [JsonPropertyName("si")]
    public int SoundSimulationType { get; set; }

    /// <summary>
    /// Forces loading of effect defaults (speed, intensity, etc) from effect metadata. (available since 0.14.0)
    /// </summary>
    [JsonPropertyName("fxdef")]
    public bool LoadEffectDefaults { get; set; }

    /// <summary>
    /// Assigns group or set ID to segment (not to be confused with grouping). Visual aid only (helps in UI). (available since 0.14.0)
    /// </summary>
    [JsonPropertyName("set")]
    public int SetID { get; set; }

    /// <summary>
    /// Flag to repeat current segment settings by creating segments until all available LEDs are included in automatically created segments or maximum segments reached. Will also toggle reverse on every even segment. (available since 0.13.0)
    /// </summary>
    [JsonPropertyName("rpt")]
    public bool RepeatSegmentSettings { get; set; }
}