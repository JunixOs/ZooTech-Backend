using System;
using System.Collections.Generic;
using System.Text;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosPdf.GeneratOrdenioComparationPdf;

public sealed record GenerateOrdenioComparationPdfOutput(
    byte[] Content,
    string ContentType,
    string FileName
 );

