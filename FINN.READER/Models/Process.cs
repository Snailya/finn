using FINN.SHAREDKERNEL.Dtos;

namespace FINN.READER.Models;

internal class Process
{
    /// <summary>
    ///     Subprocess of the process.
    /// </summary>
    public readonly List<Process> SubProcess = new();

    /// <summary>
    ///     Name of the layer to draw.
    /// </summary>
    public string? Layer { get; set; }

    /// <summary>
    ///     Index of the process
    /// </summary>
    public string Index { get; set; }

    /// <summary>
    ///     Name of process.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     Key of parameters
    /// </summary>
    public string RH { get; set; }

    /// <summary>
    ///     Time of process.
    /// </summary>
    public string TimeString { get; set; }

    /// <summary>
    ///     Width(m) (length in y-direction) of room.
    /// </summary>
    public Length Width { get; set; }

    /// <summary>
    ///     Lenght(m) (length in x-direction) of room.
    /// </summary>
    public Length Length { get; set; }

    /// <summary>
    ///     Speed of conveyor
    /// </summary>
    public string Speed { get; set; }

    /// <summary>
    ///     Cycle pitch.
    /// </summary>
    public string CyclePitch { get; set; }

    /// <summary>
    ///     Cycle time.
    /// </summary>
    public string CycleTime { get; set; }

    /// <summary>
    ///     Gross capacity.
    /// </summary>
    public string GrossCapacity { get; set; }


    public ProcessDto ToLayoutProcessDto()
    {
        return new ProcessDto
        {
            Layer = Layer,
            Name = Name.Replace("\n", ""),
            Line1 = $"{Name} {RH} {TimeString}".Replace("\n", ""),
            Line2 = $"{Length} x {Width}".Replace("\n", ""),
            XLength = Length.ToMillimeter(),
            YLength = Width.ToMillimeter(),
            SubProcess = SubProcess.Select(x => x.ToLayoutProcessDto())
        };
    }
}