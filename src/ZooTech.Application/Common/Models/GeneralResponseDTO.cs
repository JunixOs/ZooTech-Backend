namespace ZooTech.Application.Common.Models
{
    public class GeneralResponseDTO<T>
    {
        public T? Data { get; set; }

        public static GeneralResponseDTO<T> Ok(T? data) => new() { Data = data };
    }
}
