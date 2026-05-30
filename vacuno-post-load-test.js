import http from "k6/http";
import { check, sleep } from "k6";

var BASE_URL = __ENV.BASE_URL || "https://localhost:7001";
var TENANT_ID = __ENV.TENANT_ID || "1";

export default function () {
    var codigo = "V" + String(Date.now()).slice(-6) + String(__VU).padStart(2, "0") + String(__ITER % 100).padStart(2, "0");

    var payload = {
        Codigo: codigo,
        Nombre: "Vacuno Test",
        FechaNacimiento: "2022-01-15",
        IdTipoAdquisicion: "1",
        IdRaza: "1",
        IdColor: "1",
        IdSexo: "2",
        CodigoMadre: "VACA001",
        CodigoPadre: "TORO001",
        NombreGranja: "Granja Test",
        IdDistrito: "1",
        IdDepartamento: "1",
        IdProvincia: "1",
        IdTipoUtilizacion: "1",
        FechaEspecificacion: "2023-06-01",
        Observaciones: "Prueba de concurrencia",
        DummyFile: http.file("contenido de prueba", "dummy.txt", "text/plain")
    };

    var headers = {
        "X-Tenant-Id": TENANT_ID
    };

    var res = http.post(BASE_URL + "/v1/vacunos", payload, {
        headers: headers,
        timeout: "15s"
    });

    check(res, {
        "status controlado": function (r) {
            return [201, 400, 409, 500].includes(r.status);
        },
        "sin timeout": function (r) {
            return r.status !== 0;
        },
        "tiempo menor a 2000ms": function (r) {
            return r.timings.duration < 2000;
        }
    });

    sleep(0.2);
}