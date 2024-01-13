﻿using System.Text.Json.Serialization;

namespace Kevsoft.WLED;

public sealed class InformationResponse
{
    /// <summary>
    /// Version name.
    /// </summary>
    [JsonPropertyName("ver")]
    public string VersionName { get; set; } = null!;

    /// <summary>
    /// Build ID (YYMMDDB, B = daily build index).
    /// </summary>
    [JsonPropertyName("vid")]
    public uint BuildId { get; set; }

    /// <summary>
    /// LEDs Information
    /// </summary>
    [JsonPropertyName("leds")]
    public LedsResponse Leds { get; set; } = null!;

    /// <summary>
    /// If true, an UI with only a single button for toggling sync should toggle receive+send, otherwise send only
    /// </summary>
    [JsonPropertyName("str")]
    public bool ToggleSendReceive { get; set; }

    /// <summary>
    /// Friendly name of the light.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// The UDP port for realtime packets and WLED broadcast.
    /// </summary>
    [JsonPropertyName("udpport")]
    public int UdpPort { get; set; }

    /// <summary>
    /// If true, the software is currently receiving realtime data via UDP or E1.31.
    /// </summary>
    [JsonPropertyName("live")]
    public bool Live { get; set; }

    /// <summary>
    /// Info about the realtime data source
    /// </summary>
    [JsonPropertyName("lm")]
    public string? LiveSource { get; set; }

    /// <summary>
    /// If true, the software is currently receiving realtime data via UDP or E1.31.
    /// </summary>
    [JsonPropertyName("lip")]
    public string? LiveIP { get; set; }

    /// <summary>
    /// Number of currently connected WebSockets clients. -1 indicates that WS is unsupported in this build.
    /// </summary>
    [JsonPropertyName("ws")]
    public int WebSocketClients { get; set; }

    /// <summary>
    /// Number of effects included.
    /// </summary>
    [JsonPropertyName("fxcount")]
    public byte EffectsCount { get; set; }

    /// <summary>
    /// Number of palettes configured.
    /// </summary>
    [JsonPropertyName("palcount")]
    public ushort PalettesCount { get; set; }

    /// <summary>
    /// Info about current signal strength
    /// </summary>
    [JsonPropertyName("wifi")]
    public Wifi Wifi { get; set; }

    /// <summary>
    /// Name of the platform.
    /// </summary>
    [JsonPropertyName("arch")]
    public string Arch { get; set; } = null!;

    /// <summary>
    /// Version of the underlying (Arduino core) SDK.
    /// </summary>
    [JsonPropertyName("core")]
    public string Core { get; set; } = null!;

    /// <summary>
    /// Bytes of heap memory (RAM) currently available. Problematic if less than 10k.
    /// </summary>
    [JsonPropertyName("freeheap")]
    public uint FreeHeapMemory { get; set; }

    /// <summary>
    /// Time since the last boot/reset in seconds.
    /// </summary>
    [JsonPropertyName("uptime")]
    public uint UpTime { get; set; }

    /// <summary>
    /// Used for debugging purposes only.
    /// </summary>
    [JsonPropertyName("opt")]
    public ushort Opt { get; set; }

    /// <summary>
    /// The producer/vendor of the light. Always WLED for standard installations.
    /// </summary>
    [JsonPropertyName("brand")]
    public string Brand { get; set; } = null!;

    /// <summary>
    /// The product name. Always FOSS for standard installations.
    /// </summary>
    [JsonPropertyName("product")]
    public string Product { get; set; } = null!;

    /// <summary>
    /// The origin of the build. src if a release version is compiled from source, bin for an official release image, dev for a development build (regardless of src/bin origin) and exp for experimental versions. ogn if the image is flashed to hardware by the vendor.
    /// </summary>
    [JsonPropertyName("btype")]
    public string BuildType { get; set; } = null!;

    /// <summary>
    /// The hexadecimal hardware MAC address of the light, lowercase and without colons.
    /// </summary>
    [JsonPropertyName("mac")]
    public string MacAddress { get; set; } = null!;

    /// <summary>
    /// The IP address of this instance. Empty string if not connected. (since 0.13.0)
    /// </summary>
    [JsonPropertyName("ip")]
    public string NetworkAddress { get; set; } = null!;
}

public class Wifi
{
    /// <summary>
    /// The BSSID of the currently connected network.
    /// </summary>
    [JsonPropertyName("bssid")]
    public string? WifiBSSID { get; set; }

    /// <summary>
    /// Relative signal quality of the current connection.
    /// </summary>
    [JsonPropertyName("signal")]
    public double? WifiSignalStrength { get; set; }

    /// <summary>
    /// Info about current signal strength
    /// </summary>
    [JsonPropertyName("channel")]
    public int WifiChannel { get; set; }
}

public class Leds
{
    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("pwr")]
    public int Pwr { get; set; }

    [JsonPropertyName("fps")]
    public int fps { get; set; }

    [JsonPropertyName("maxpwr")]
    public int maxpwr { get; set; }

    [JsonPropertyName("maxseg")]
    public int maxseg { get; set; }

    //[JsonPropertyName("seclc")]
    //public int seclc { get; set; }

    [JsonPropertyName("lc")]
    public int lc { get; set; }

    [JsonPropertyName("rgbw")]
    public int rgbw { get; set; }

    [JsonPropertyName("wv")]
    public int wv { get; set; }

    [JsonPropertyName("cct")]
    public int cct { get; set; }
}