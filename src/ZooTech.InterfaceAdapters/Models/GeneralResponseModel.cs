namespace ZooTech.InterfaceAdapters.Models
{
    public class GeneralResponseModel<T>
    {
        public T? Data { get; set; }

        public static GeneralResponseModel<T> Ok(T? data) => new() { Data = data };
    }
}
