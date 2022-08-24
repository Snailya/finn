﻿using System.Text.Json.Serialization;

namespace FINN.SHAREDKERNEL.Dtos.Management;

public class BlockDefinitionDto
{
    /// <summary>
    ///     The id of the block in database.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    ///     The name of the block, used to insert block.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}