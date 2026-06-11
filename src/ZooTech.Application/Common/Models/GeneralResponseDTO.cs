using System;
using System.Collections.Generic;
using System.Text;

namespace ZooTech.Application.Common.Models
{
    internal class GeneralResponseDTO<T>
    {
        public T? Data {  get; set; }

        public static GeneralResponseDTO<T> Ok(T? data) => new GeneralResponseDTO<T> { Data = data };
    }
}
