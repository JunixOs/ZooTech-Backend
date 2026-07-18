# Reporte para DBA: parametrización del ciclo estral (módulo Celo)

## Contexto

El endpoint `GET /api/v1/celo/vacas-en-celo` calcula el estado reproductivo de cada
vaca (`En celo` / `Próximo` / `Pasó`) y los días restantes hasta el próximo celo
esperado. La regla de negocio usa tres valores numéricos que hoy están fijos en el
código fuente (`GetVacasEnCeloInteractor.cs`), sin respaldo en base de datos:

| Parámetro | Valor actual | Significado |
|---|---|---|
| `DuracionCicloDias` | 21 | Duración estándar del ciclo estral (días entre un celo y el siguiente) |
| `DiasEnCelo` | 2 | Días posteriores al último celo registrado en que la vaca se considera "En celo" |
| `DiasProximo` | 3 | Ventana de días antes del próximo celo esperado en que se considera "Próximo" |

Estos valores fueron aprobados como referencia inicial de negocio, pero no existe
ninguna tabla de catálogo que los respalde. Cualquier cambio hoy requiere modificar
código y redesplegar el backend.

## Pedido

Se solicita crear una tabla de parámetros/configuración en base de datos para que el
backend pueda leer estos valores en tiempo de ejecución en lugar de tenerlos
hardcodeados.

### Propuesta de esquema

```sql
CREATE TABLE cat_parametro_celo (
    id                  INT IDENTITY PRIMARY KEY,
    codigo              VARCHAR(50) NOT NULL UNIQUE,  -- ej: 'DURACION_CICLO_DIAS'
    valor               INT NOT NULL,
    descripcion         VARCHAR(200) NULL,
    fecha_actualizacion DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);

INSERT INTO cat_parametro_celo (codigo, valor, descripcion) VALUES
    ('DURACION_CICLO_DIAS', 21, 'Duración estándar del ciclo estral en días'),
    ('DIAS_EN_CELO',         2, 'Días posteriores al último celo en que la vaca se considera en celo'),
    ('DIAS_PROXIMO',         3, 'Ventana de días previos al próximo celo esperado');
```

Puntos abiertos para el DBA:

- ¿Estos parámetros deberían ser globales o por raza/establecimiento (`vacuno_raza_id`,
  `establecimiento_id`)? Si en el futuro el ciclo varía por raza, conviene agregar esa
  columna desde el inicio para no migrar de nuevo.
- Confirmar si `cat_parametro_celo` debe vivir en el mismo esquema que el resto de
  catálogos (`cat_*`) del módulo Celo.

## Cómo lo va a consumir el backend

- Se agrega un método al repositorio (`ICeloRepository.GetParametrosCicloAsync`) que
  lee la tabla y devuelve los tres valores.
- `GetVacasEnCeloInteractor` deja de usar `const` y los recibe inyectados desde el
  repositorio (con cache en memoria de corta duración para no consultar la tabla en
  cada request).
- Si la tabla está vacía o el parámetro no existe, el backend cae a los valores
  actuales (21/2/3) como default de seguridad.

## Impacto si no se hace

Ninguno urgente — el endpoint funciona hoy con los valores fijos. Esto es una mejora
de mantenibilidad: evita un despliegue de backend cada vez que el negocio quiera
ajustar la duración del ciclo o las ventanas de "En celo"/"Próximo".
