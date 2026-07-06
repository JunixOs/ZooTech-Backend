# Skill: Unit Testing en Clean Architecture (Ports & Adapters)

## Objetivo
Validar la lógica de negocio pura en los Interactors de la capa de Application en total aislamiento.

## Estándares Técnicos
- **Framework:** xUnit.
- **Mocking:** NSubstitute o Moq (Preferido: NSubstitute para legibilidad).
- **Aserciones:** FluentAssertions.
- **Aislamiento:** Está PROHIBIDO instanciar clases de la capa de Infrastructure o InterfaceAdapters. Todas las dependencias (Gateway, Repositories, Services) deben ser Mocked.

## Patrón de Prueba: AAA (Arrange, Act, Assert)
1. **Arrange:** Configurar los Mocks de los puertos de salida (OutputPorts) y gateways.
2. **Act:** Llamar al método `Handle` del Interactor (InputPort).
3. **Assert:** Verificar que el Interactor llamó al método correcto del `OutputPort` (ej. `_outputPort.Ok()`) con los datos esperados.

## Reglas Críticas para ZooTech
- **Multi-tenancy:** Simular siempre un `ITenantContext` válido.
- **Domain Rules:** Validar que el Interactor use las `Domain Rules` y lance `BusinessException` cuando las reglas fallan.
- **Filtros por Defecto:** Probar explícitamente que, si los parámetros de fecha son nulos, el Interactor calcule el rango de 30 días usando `IDateTimeProvider.UtcNow`.