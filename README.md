# ⚽ Football Elite 2027

Simulador de fútbol 3D **original**, desarrollado en **Unity 6 + C#** para computadora y iPhone, con perfiles gráficos y controles adaptados.

> Estado: **Fase 1 — prototipo de entrenamiento**. No contiene equipos ni jugadores de EA SPORTS FC, FIFA ni marcas licenciadas. No se presenta como juego AAA terminado.

## Fase 1: entrenamiento jugable

La escena se genera desde el Editor de Unity, sin depender de un archivo de escena binario. Incluye:

- Campo 3D con líneas reglamentarias a escala simplificada, césped, porterías y gradas.
- Futbolista controlable, carrera y orientación suave.
- Balón con física, regate, pase y tiro con fuerza y elevación diferenciadas.
- Portero de entrenamiento con seguimiento básico del balón.
- Detección de goles, marcador, tiempo y reinicio.
- Cámara de transmisión con seguimiento y zoom suave.
- Teclado y mando en PC; joystick y botones táctiles en iPhone.
- Perfiles de rendimiento iniciales: escritorio y móvil.

### Cómo abrir el proyecto

1. Instala **Unity Hub** y una versión compatible de **Unity 6 (6000.0.x)** con módulos para Windows y, si desarrollas en Mac, iOS.
2. Clona el repositorio:
   ```bash
   git clone https://github.com/FredyFuness/Football-Elite-2027.git
   ```
3. Abre la carpeta principal con **Unity Hub → Add project from disk**. Espera a que importe las dependencias.
4. En el Editor, selecciona **Football Elite → Crear escenario de entrenamiento (Fase 1)**.
5. Abre `Assets/FootballElite/Scenes/Training.unity` y pulsa **Play**.

La escena y los materiales se generan automáticamente; después de generarlos, guarda y versiona los archivos `.unity`, `.mat` y sus `.meta` en Git.

### Controles

| Acción | PC | iPhone |
|---|---|---|
| Movimiento | WASD / flechas / stick | Joystick virtual |
| Correr | Shift / gatillo RT | Mantener **SPRINT** |
| Pasar | J / botón sur del mando | **PASE** |
| Disparar | Espacio / botón este del mando | **TIRO** |
| Reiniciar sesión | R | Botón del marcador |

El tiro funciona cerca del balón. Apunta moviendo al futbolista. Los controles están preparados para un modelo 3D más detallado en fases posteriores.

### Ejecutables

**Windows:** en Unity, abre **File → Build Profiles → Windows**, agrega la escena `Training.unity` y construye el ejecutable.

**iPhone:** una aplicación nativa iOS requiere **Mac + Xcode**, además de configurar firma de Apple. El código y la escena pueden prepararse desde Windows, pero la compilación final para un iPhone físico no se realiza directamente con GitHub Pages.

**Navegador:** este repositorio contiene un proyecto de Unity, no una web HTML. Publicarlo en GitHub Pages requeriría una exportación WebGL adicional (más pesada y con límites en Safari móvil).

## Plan de desarrollo

Consulta [arquitectura](Docs/ARQUITECTURA.md) y [hoja de ruta](Docs/FASES.md). Empezamos por una mecánica de fútbol sólida y después añadimos futbolistas animados, IA de equipo, porteros avanzados, reglas, partidos y modos.

## Organización

```text
Assets/FootballElite/
  Scripts/Runtime/   # lógica de partido, jugador, balón, cámara y UI táctil
  Scripts/Editor/    # generador reproducible del escenario
  Scenes/            # escenario generado en Unity
  Materials/         # materiales generados en Unity
Packages/            # dependencias UPM
ProjectSettings/     # versión de Unity
Docs/                # arquitectura y fases
.github/workflows/    # verificaciones estáticas CI
```

Este proyecto es independiente de **NOCTURNE: Ciudad de Sombras**.
