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
    public class ProveedorFunction
    {
        private readonly ILogger<ProveedorFunction> _logger;
        private readonly IRepositorio _repositorio;

        public ProveedorFunction(ILogger<ProveedorFunction> logger, IRepositorio repositorio)
        {
            _logger = logger;
            _repositorio = repositorio;
        }

        [Function("ListarProveedor")]
        [OpenApiOperation("Proveedorlist", "Proveedor", Description = "Listar Proveedor")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IProveedor), Description = "Lista de Proveedores")]
        public async Task<HttpResponseData> ListarProveedor([HttpTrigger(AuthorizationLevel.Function, "get", Route = "ListarProveedor")] HttpRequestData req)
        {
            _logger.LogInformation("ListarProveedor");
            var res = req.CreateResponse(HttpStatusCode.OK);
            await res.WriteAsJsonAsync(_repositorio.ListarTodos<Proveedor>());
            return res;

        }
        [OpenApiOperation("Proveedorinser", "Proveedor", Description = "Insertar Proveedor")]
        [OpenApiRequestBody("application/json", typeof(IProveedor), Description = "Inserte Proveedor")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IProveedor), Description = "Proveedor Insertado")]

        [Function("InsertarProveedor")]
         public async Task<HttpResponseData> InsertarProveedor([HttpTrigger(AuthorizationLevel.Function, "post", Route = "InsertarProveedor")] HttpRequestData req)
        {
            _logger.LogInformation("InsertarProveedor");
            var data = await req.ReadFromJsonAsync<Proveedor>();
            Proveedor success = await _repositorio.Insertar<Proveedor>(data);
            if (success != null)
            {
                var respuesta = req.CreateResponse(HttpStatusCode.OK);
                return respuesta;
            }
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }
        [Function("ActualizarProveedor")]
        [OpenApiOperation("Proveedoractu", "Proveedor", Description = "Actualizar Proveedor")]
        [OpenApiRequestBody("application/json", typeof(IProveedor), Description = "Actualizar Proveedor")]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "ID del Proveedor")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IProveedor), Description = "Proveedor Actualizado")]

        public async Task<HttpResponseData> ActualizarProveedor([HttpTrigger(AuthorizationLevel.Function, "put", Route = "ActualizarProveedor/{id}")] HttpRequestData req, string id)
        {
            _logger.LogInformation("ActualizarProveedor");
            var data = await req.ReadFromJsonAsync<Proveedor>();
            data.RowKey = id;
            bool success = await _repositorio.Actualizar<Proveedor>(data);
            if (success)
            {
                var respuesta = req.CreateResponse(HttpStatusCode.OK);
                return respuesta;
            }
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }
        [Function("EliminarProveedor")]
        [OpenApiOperation("Proveedorelim", "Proveedor", Description = "Eliminar Proveedor")]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "ID del Proveedor")]
        [OpenApiResponseWithoutBody(HttpStatusCode.OK ,Description = "Proveedor Eliminado")]

        public async Task<HttpResponseData> EliminarProveedor([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "EliminarProveedor/{id}")] HttpRequestData req, string id)
        {
            _logger.LogInformation("EliminarProveedor");
            var data = new Proveedor();
            data.RowKey = id;
            bool success = await _repositorio.Eliminar<Proveedor>(data);
            if (success)
            {
                var respuesta = req.CreateResponse(HttpStatusCode.OK);
                return respuesta;
            }
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }

        [Function("ObtenerProveedor")]
        [OpenApiOperation("Proveedorobt", "Proveedor", Description = "Obtener Proveedor")]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "ID del Proveedor")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IProveedor), Description = "Obtener Proveedor")]
        public async Task<HttpResponseData> ObtenerProveedor([HttpTrigger(AuthorizationLevel.Function, "get", Route = "ObtenerProveedor/{id}")] HttpRequestData req, string id)
        {
            _logger.LogInformation("ObtenerProveedor");
            var data = new Proveedor();
            data.RowKey = id;
            var res = req.CreateResponse(HttpStatusCode.OK);
            if (_repositorio.ListarUno<Proveedor>(data).Result != null)
            {
                await res.WriteAsJsonAsync(_repositorio.ListarUno<Proveedor>(data).Result);
                return res;
            }
            var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
            var errorMensaje = new { mensaje = $"Proveedor con ID: {id} no encontrado." };
            await notFoundResponse.WriteAsJsonAsync(errorMensaje);
            return notFoundResponse;
        }
    }
}
