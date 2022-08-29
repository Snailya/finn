using FINN.SHAREDKERNEL.UseCases;

namespace FINN.EXCEL.Models;

internal class Process
{
    /// <summary>
    ///     Subprocess of the process.
    /// </summary>
    public readonly List<Process> SubProcess = new();

    /// <summary>
    ///     Name of the layer to draw.
    /// </summary>
    public string Layer { get; set; } = string.Empty;

    /// <summary>
    ///     Index of the process
    /// </summary>
    public string Index { get; set; } = string.Empty;

    /// <summary>
    ///     Name of process.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     Key of parameters
    /// </summary>
    public string RH { get; set; } = string.Empty;

    /// <summary>
    ///     Time of process.
    /// </summary>
    public string TimeString { get; set; } = string.Empty;

    /// <summary>
    ///     Width(m) (length in y-direction) of room.
    /// </summary>
    public Length Width { get; set; } = Length.Zero;

    /// <summary>
    ///     Lenght(m) (length in x-direction) of room.
    /// </summary>
    public Length Length { get; set; } = Length.Zero;

    /// <summary>
    ///     Speed of conveyor
    /// </summary>
    public string Speed { get; set; } = string.Empty;

    /// <summary>
    ///     Cycle pitch.
    /// </summary>
    public string CyclePitch { get; set; } = string.Empty;

    /// <summary>
    ///     Cycle time.
    /// </summary>
    public string CycleTime { get; set; } = string.Empty;

    /// <summary>
    ///     Gross capacity.
    /// </summary>
    public string GrossCapacity { get; set; } = string.Empty;


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