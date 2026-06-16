namespace ZooTech.InterfaceAdapters.DTOs;

public class GeneralResponseDTO<T>
{
    public T? Data { get; set; }

    public static GeneralResponseDTO<T> Ok(T? data) => new() { Data = data };
    public static GeneralResponseDTO<T> Fail(string message) => new() { Data = default };
}

