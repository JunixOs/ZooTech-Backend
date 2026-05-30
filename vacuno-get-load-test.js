@'
    import http from "k6/http";
import { check, sleep } from "k6";

var BASE_URL = __ENV.BASE_URL || "https://localhost:7001";

export default function () {
    var res = http.get(BASE_URL + "/v1/vacunos/1", {
        timeout: "10s"
    });

    check(res, {
        "status 200 404 500": function (r) {
            return [200, 404, 500].includes(r.status);
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
'@ | Out-File -FilePath .\vacuno-get-load-test.js -Encoding ascii