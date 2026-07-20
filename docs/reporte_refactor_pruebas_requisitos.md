# Reporte de refactorizacion de pruebas

## Alcance

Se reorganizaron las pruebas de los requisitos **Registrar Vacuno**, **Editar Registro** y
**Eliminar Fecundacion**. El objetivo fue separar datos de prueba, preparacion de dependencias,
reglas de validacion y contratos HTTP para que cada prueba tenga una sola razon de cambio.

## Cambios aplicados

### Datos y fixtures reutilizables

- `VacunoTestDataFactory` concentra comandos y entidades validas de Vacuno.
- `FecundacionTestDataFactory` concentra comandos y read models de Fecundacion.
- `VacunoMutationTestContext` configura repositorio, Unit of Work, cache, parametros del tenant
  y resolucion de referencias.
- `DeleteFecundacionTestContext` configura la transaccion y las dependencias del borrado logico.

Esto elimina duplicacion de Arrange y aplica SRP: los tests describen escenarios, mientras que
los fixtures construyen su ambiente. Los interactores dependen de interfaces simuladas, por lo
que las pruebas tambien respetan DIP.

## Cobertura funcional

### Registrar Vacuno

- Persistencia dentro de Unit of Work.
- Invalidacion de cache solo despues de una operacion correcta.
- Rechazo por codigo duplicado antes de iniciar la transaccion.
- Limites configurables por tenant y errores estructurados por campo.
- Obligatoriedad, procedencia y precio requerido para compra.
- Integracion HTTP: registro, lectura posterior, duplicidad y rechazo estructurado.

### Editar Registro

- Actualizacion de campos editables conservando el codigo.
- Rechazo cuando el vacuno no existe.
- Inmutabilidad de fecha de nacimiento y tipo de adquisicion.
- Limite parametrizado de observaciones.
- Integracion HTTP: actualizacion correcta y rechazo de fecha inmutable.

### Eliminar Fecundacion

- Eliminacion logica dentro de Unit of Work.
- Bloqueo por inexistencia o dependencias con cria.
- Cache intacta cuando falla la transaccion.
- Validacion estructurada de `id` y `razon`.
- Integracion HTTP: eliminacion y ocultamiento, inexistencia y motivo obligatorio.

## Resultado de ejecucion

- Pruebas unitarias enfocadas: **39/39 aprobadas**.
- Proyecto de integracion: **compila correctamente, 0 errores**.
- Pruebas HTTP enfocadas: **4 aprobadas y 5 bloqueadas por respuesta 500 al escribir**.

Los cinco fallos de integracion ocurren al crear datos tanto de Vacuno como de Fecundacion. Las
consultas de catalogos y los escenarios de rechazo si responden, por lo que el resultado confirma
un problema externo en la ruta de escritura/configuracion SQL del ambiente. Se mantiene visible
para no convertir una dependencia real en un falso positivo mediante datos hardcodeados o mocks
dentro de una prueba de integracion.

## Observaciones

- Persisten advertencias preexistentes fuera del alcance en `ProviderInterfaceTests` y
  `TriajeMapper`; no fueron introducidas por este refactor.
- Para obtener la suite HTTP completamente verde, el usuario SQL del entorno debe poder abrir y
  escribir en la base administrativa y en la base del tenant utilizada por la factory.
