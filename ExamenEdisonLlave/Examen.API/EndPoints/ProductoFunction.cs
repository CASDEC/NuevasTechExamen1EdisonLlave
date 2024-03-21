using Examen.API.Contratos.Repositorios;
using Examen.API.Modelo;
using Examen.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net;

namespace Examen.API.EndPoints
{
    public class ProductoFunction
    {
        private readonly ILogger<ProductoFunction> _logger;
        private readonly IRepositorio _repositorio;

        public ProductoFunction(ILogger<ProductoFunction> logger, IRepositorio repositorio)
        {
            _logger = logger;
            _repositorio = repositorio;
        }

        [Function("ListarProducto")]
        [OpenApiOperation("Productolist", "Producto", Description = "Listar Producto")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IProducto), Description = "Lista de Productos")]
        public async Task<HttpResponseData> ListarProducto([HttpTrigger(AuthorizationLevel.Function, "get", Route = "ListarProducto")] HttpRequestData req)
        {
            _logger.LogInformation("ListarProducto");
            var res = req.CreateResponse(HttpStatusCode.OK);
            await res.WriteAsJsonAsync(_repositorio.ListarTodos<Producto>());
            return res;

        }
        [OpenApiOperation("Productoinser", "Producto", Description = "Insertar Producto")]
        [OpenApiRequestBody("application/json", typeof(IProducto), Description = "Inserte Producto")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IProducto), Description = "Producto Insertado")]

        [Function("InsertarProducto")]
        public async Task<HttpResponseData> InsertarProducto([HttpTrigger(AuthorizationLevel.Function, "post", Route = "InsertarProducto")] HttpRequestData req)
        {
            _logger.LogInformation("InsertarProducto");
            var data = await req.ReadFromJsonAsync<Producto>();
            Producto success = await _repositorio.Insertar<Producto>(data);
            if (success != null)
            {
                var respuesta = req.CreateResponse(HttpStatusCode.OK);
                return respuesta;
            }
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }
        [Function("ActualizarProducto")]
        [OpenApiOperation("Productoactu", "Producto", Description = "Actualizar Producto")]
        [OpenApiRequestBody("application/json", typeof(IProducto), Description = "Actualizar Producto")]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "ID del Producto")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IProducto), Description = "Producto Actualizado")]

        public async Task<HttpResponseData> ActualizarProducto([HttpTrigger(AuthorizationLevel.Function, "put", Route = "ActualizarProducto/{id}")] HttpRequestData req, string id)
        {
            _logger.LogInformation("ActualizarProducto");
            var data = await req.ReadFromJsonAsync<Producto>();
            data.RowKey = id;
            bool success = await _repositorio.Actualizar<Producto>(data);
            if (success)
            {
                var respuesta = req.CreateResponse(HttpStatusCode.OK);
                return respuesta;
            }
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }
        [Function("EliminarProducto")]
        [OpenApiOperation("Productoelim", "Producto", Description = "Eliminar Producto")]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "ID del Producto")]
        [OpenApiResponseWithoutBody(HttpStatusCode.OK, Description = "Producto Eliminado")]

        public async Task<HttpResponseData> EliminarProducto([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "EliminarProducto/{id}")] HttpRequestData req, string id)
        {
            _logger.LogInformation("EliminarProducto");
            var data = new Producto();
            data.RowKey = id;
            bool success = await _repositorio.Eliminar<Producto>(data);
            if (success)
            {
                var respuesta = req.CreateResponse(HttpStatusCode.OK);
                return respuesta;
            }
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }

        [Function("ObtenerProducto")]
        [OpenApiOperation("Productoobt", "Producto", Description = "Obtener Producto")]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "ID del Producto")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IProducto), Description = "Obtener Producto")]
        public async Task<HttpResponseData> ObtenerProducto([HttpTrigger(AuthorizationLevel.Function, "get", Route = "ObtenerProducto/{id}")] HttpRequestData req, string id)
        {
            _logger.LogInformation("ObtenerProducto");
            var data = new Producto();
            data.RowKey = id;
            var res = req.CreateResponse(HttpStatusCode.OK);
            if (_repositorio.ListarUno<Producto>(data).Result != null)
            {
                await res.WriteAsJsonAsync(_repositorio.ListarUno<Producto>(data).Result);
                return res;
            }
            var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
            var errorMensaje = new { mensaje = $"Producto con ID: {id} no encontrado." };
            await notFoundResponse.WriteAsJsonAsync(errorMensaje);
            return notFoundResponse;
        }
    }
}
