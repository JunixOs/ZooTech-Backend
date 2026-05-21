# Evidencia API - Ver Vacuno

## Servicios

- Backend: `http://localhost:5085`
- Swagger UI: `http://localhost:5085/swagger`
- Frontend: `http://localhost:4200`

## Credenciales demo

- Email: `admin@zootech.com`
- Password: `Zootech2026!`

## Flujo recomendado para video corto

1. Abrir Swagger UI y ejecutar `POST /api/auth/login`.
2. Copiar el token `zootech-demo-token` o usar el token devuelto por login.
3. Probar `GET /api/vacunos?page=1&pageSize=10` con header `Authorization: Bearer zootech-demo-token`.
4. Probar `GET /api/vacunos/VAC_101` y mostrar los datos completos de Duquesa.
5. Probar `PUT /api/vacunos/VAC_101` cambiando `nombre` u `observaciones`.
6. Abrir el frontend, iniciar sesion y verificar que la pantalla de vacunos refleja el listado y detalle desde la API.

Tambien se puede importar `docs/api-evidence/zootech-vacunos.postman_collection.json` en Postman.
