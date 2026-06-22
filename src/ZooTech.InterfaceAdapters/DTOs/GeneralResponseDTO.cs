using System;
using System.Collections.Generic;
using System.Text;

namespace ZooTech.InterfaceAdapters.DTOs
{
    public class GeneralResponseDTO<T>
    {
        public T? Data { get; set; }

        public string? ErrorMessage { get; set; }

        public bool Success { get; set; }

        public static GeneralResponseDTO<T> Ok(T data) => new GeneralResponseDTO<T> { Success = true, Data = data };
        public static GeneralResponseDTO<T> Fail(string errorMessage) => new GeneralResponseDTO<T> { Success = false, ErrorMessage = errorMessage };
    }
}
