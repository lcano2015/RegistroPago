using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using RegistroPago.Models;
using RegistroPago.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using System.Drawing;
using OfficeOpenXml.Drawing;
using ClosedXML.Excel;

namespace RegistroPago.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment hostEnvironment;
        IPagoRepository pagoRepository;


        public HomeController(IWebHostEnvironment hostEnvironment, IPagoRepository pagoRepository)
        {

            this.hostEnvironment = hostEnvironment;
            this.pagoRepository = pagoRepository;

        }

        public IActionResult Index()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Create([Bind("Cip,NombreApellido,FechaVoucher,ImageFile")] Pago pago)
        {
            var erroresValidacion = new List<string>();

            try
            {
                if (ModelState.IsValid)
                {
                    if (pagoRepository.GetPagoxCip(pago.Cip) == 0)
                    {

                        string wwwRootPath = hostEnvironment.WebRootPath;
                        string fileName = Path.GetFileNameWithoutExtension(pago.ImageFile.FileName);
                        string extension = Path.GetExtension(pago.ImageFile.FileName);
                        pago.ImageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        string path = Path.Combine(wwwRootPath + "/Image/", fileName);
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await pago.ImageFile.CopyToAsync(fileStream);
                        }

                        pago.FechaRegistro = DateTime.Now;
                        pago.Estado = 1;

                        var pagoId = await pagoRepository.AddPago(pago);
                        return Ok(); // Devolver una respuesta HTTP 200 OK si la creación fue exitosa
                    }

                    else
                    {
                        erroresValidacion.Add("Existe un registro con ese CIP");
                    }
                }
                else
                {
                    // Obtener los errores de validación del modelo
                    erroresValidacion = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                }

            }
            catch (DataException ex)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                erroresValidacion.Add(ex.Message);
            }

            return Ok(erroresValidacion); // Devolver una respuesta HTTP 400 Bad Request si el modelo no es válido

        }

        public async Task<IActionResult> ListaPagos(string searchTerm, int pagina = 1, int elementosPorPagina = 6)
        {
            // Obtener todos los elementos o realizar la búsqueda
            List<Pago> listaCompleta = await pagoRepository.GetPagos();


            int idBusqueda;
            if (int.TryParse(searchTerm, out idBusqueda))
            {
                listaCompleta = listaCompleta.Where(e => e.Cip == idBusqueda).ToList();
            }



            // Calcular el total de elementos y la cantidad de páginas
            var totalElementos = listaCompleta.Count();
            var totalPaginas = (int)Math.Ceiling(totalElementos / (double)elementosPorPagina);

            // Validar el número de página
            pagina = Math.Max(1, Math.Min(pagina, totalPaginas));

            // Obtener los elementos de la página actual
            var elementosPagina = listaCompleta.Skip((pagina - 1) * elementosPorPagina).Take(elementosPorPagina);

            // Pasar los datos a la vista
            var modelo = new PaginacionViewModel
            {
                Pagos = elementosPagina,
                PaginaActual = pagina,
                TotalElementos = totalElementos,
                ElementosPorPagina = elementosPorPagina,
                TotalPaginas = totalPaginas,
                TerminoBusqueda = searchTerm
            };

            return View(modelo);
        }

        public IActionResult ExportarAExcel()
        {
            // Obtener los datos de la base de datos
            var datos = ObtenerDatosConImagenes();

            // Crear el archivo de Excel con ClosedXML
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Datos");

            // Configurar el ancho de la columna para la imagen
            worksheet.Column(6).Width = 20;

            // Escribir los encabezados


            worksheet.Cell(1, 1).Value = "PagoId";
            worksheet.Cell(1, 2).Value = "CIP";
            worksheet.Cell(1, 3).Value = "Apellidos y Nombres";
            worksheet.Cell(1, 4).Value = "Fecha de Voucher";
            worksheet.Cell(1, 5).Value = "Fecha de Registro";
            worksheet.Cell(1, 6).Value = "Estado";
            worksheet.Cell(1, 7).Value = "Imagen";

            // Escribir los datos
            int row = 2;
            foreach (var dato in datos)
            {
               

                worksheet.Cell(row, 1).Value = dato.PagoId;
                worksheet.Cell(row, 2).Value = dato.Cip;
                worksheet.Cell(row, 3).Value = dato.NombreApellido;
                worksheet.Cell(row, 4).Value = dato.FechaVoucher.ToString("yyyy-MM-dd");
                worksheet.Cell(row, 5).Value = dato.FechaRegistro.ToString("yyyy-MM-dd");
                worksheet.Cell(row, 6).Value = dato.Estado.ToString();

                string path = Path.Combine(hostEnvironment.WebRootPath + "/Image/", dato.ImageName);
                // Agregar la imagen en la celda
                var imagen = worksheet.AddPicture(path);
                imagen.MoveTo(worksheet.Cell(row, 7));
                imagen.WithSize(100, 100);

                // Ajustar el ancho y alto de la celda de la imagen
                worksheet.Column(7).Width = 100;
                worksheet.Row(row).Height = 100;

                row++;
            }


            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                stream.Seek(0, SeekOrigin.Begin);

                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var fileName = "datos.xlsx";

                // Devolver el archivo como respuesta HTTP
                return File(stream.ToArray(), contentType, fileName);
            }
            
        }






        //public IActionResult ExportarAExcel()
        //{
        //    // Obtener los datos con imágenes de tu fuente de datos
        //    var datosConImagenes = ObtenerDatosConImagenes();

        //    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;


        //    // Crear el archivo Excel utilizando la librería EPPlus
        //    using (var package = new ExcelPackage())
        //    {
        //        var worksheet = package.Workbook.Worksheets.Add("Datos");

        //        // Encabezados de columna
        //        worksheet.Cells[1, 1].Value = "PagoId";
        //        worksheet.Cells[1, 2].Value = "CIP";
        //        worksheet.Cells[1, 3].Value = "Apellidos y Nombres";
        //        worksheet.Cells[1, 4].Value = "Fecha de Voucher";
        //        worksheet.Cells[1, 5].Value = "Fecha de Registro";
        //        worksheet.Cells[1, 6].Value = "Imagen";
        //        // Establecer el ancho de columna para la columna de imagen
        //        worksheet.Column(6).Width = 50;

        //        int rowIndex = 2;

        //        // Agregar los datos con imágenes al archivo Excel
        //        foreach (var dato in datosConImagenes)
        //        {
        //            worksheet.Cells[rowIndex, 1].Value = dato.PagoId;
        //            worksheet.Cells[rowIndex, 2].Value = dato.Cip;
        //            worksheet.Cells[rowIndex, 3].Value = dato.NombreApellido;
        //            worksheet.Cells[rowIndex, 4].Value = dato.FechaVoucher.ToString("yyyy-MM-dd");
        //            worksheet.Cells[rowIndex, 5].Value = dato.FechaRegistro.ToString("yyyy-MM-dd");

        //            // Agregar la imagen a la celda correspondiente

        //            string wwwRootPath = hostEnvironment.WebRootPath;

        //            string path = Path.Combine(wwwRootPath + "/Image/", dato.ImageName);
        //            ////var rutaAbsoluta = Path.GetFullPath(dato.RutaImagen);
        //            //var imagen = Image.FromFile(path);
        //            //FileInfo fileInfo=
        //            var fileInfo = new FileInfo(path);

        //            ExcelPicture pic = worksheet.Drawings.AddPicture("imagen" + rowIndex.ToString(), fileInfo);

        //            int colIndex = 5;

        //            pic.SetPosition(rowIndex, 0, colIndex, 0);
        //            //pic.SetPosition(PixelTop, PixelLeft);
        //            int Height = 100;  
        //            int Width = 100;

        //            pic.SetSize(Height, Width);
        //            //pic.SetSize(40);  
        //            worksheet.Protection.IsProtected = false;
        //            worksheet.Protection.AllowSelectLockedCells = false;



        //            //var excelImage = worksheet.Drawings.AddPicture("img" + rowIndex.ToString(), fileInfo);
        //            //excelImage.SetPosition(rowIndex - 1, 0, 1, 0);

        //            // Cargar la imagen en la celda
        //            //var imagen = Image.FromFile(dato.RutaImagen);




        //            //var picture = worksheet.Drawings.AddPicture("imagen" + rowIndex.ToString(), fileInfo);
        //            //picture.From.Column = 6;
        //            //picture.From.Row = rowIndex;
        //            //picture.SetSize(100, 100);

        //            //worksheet.Cells[rowIndex, 6].AutoFitColumns();

        //            //// Obtener las dimensiones de la imagen
        //            //var imageWidth = picture.;
        //            //var imageHeight = fileInfo.Height;

        //            //// Establecer el ancho de la columna y la altura de la fila para ajustar la imagen
        //            //worksheet.Column(5).Width = imageWidth / OfficeOpenXml.ExcelPackage.PIXELS_PER_COLUMN;
        //            //worksheet.Row(row).Height = imageHeight / OfficeOpenXml.ExcelPackage.PIXELS_PER_ROW;


        //            //picture.SetSize(100, 100); // Establecer el tamaño de la imagen en la celda
        //            ////picture.Picture = ExcelPictureSizeMode.Zoom; // Ajustar el tamaño de la imagen para que quepa en la celda




        //            rowIndex++;
        //        }

        //        // Guardar el archivo Excel
        //        var content = package.GetAsByteArray();
        //        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "datos.xlsx");
        //    }
        //}

        private List<Pago> ObtenerDatosConImagenes()
        {
            // Aquí deberías implementar la lógica para obtener los datos con imágenes de tu fuente de datos (base de datos, servicio web, etc.)
            // Retorna una lista de objetos que contengan los datos y las rutas de las imágenes
            // Por ejemplo:
            List<Pago> listaCompleta = pagoRepository.GetPagosExcel();

            return listaCompleta;
        }

    }
}
