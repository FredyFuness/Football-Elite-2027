# Hoja de ruta por fases

| Fase | Entrega verificable | Estado |
|---|---|---|
| 0. Repositorio | Proyecto Unity, documentación, CI, separación total de NOCTURNE | ✅ |
| 1. Entrenamiento | Campo, balón, jugador, tiro, pase, portero sencillo, goles, cámara, controles móvil/PC | Implementado en código; requiere abrir y probar en Unity |
| 2. Animaciones y arte | Futbolista real riggeado, locomoción, tiros, césped PBR, público y estadio | Pendiente |
| 3. Partido 5v5 | Posesión, pases con objetivo, jugadores de apoyo, IA, cambios, porteros inteligentes | Pendiente |
| 4. Reglas | Saques, faltas básicas, fuera de juego, árbitro, marcador de tiempo | Pendiente |
| 5. Partido 11v11 | Formación, estrategias, resistencia, menú táctico, repeticiones | Pendiente |
| 6. Producto multiplataforma | Ajustes gráficos, sonido, pruebas iOS, builds PC, partidas guardadas | Pendiente |

## Criterio de cierre de Fase 1

- Unity importa sin errores de compilación.
- El comando de menú genera la escena Training.unity.
- El jugador puede moverse y correr usando PC.
- El balón se mueve mediante física, reacciona al regate, pase y tiro.
- Las dos porterías detectan un gol y reinician posiciones.
- Se ve marcador, reloj y botones táctiles.
- El mismo código puede compilarse para Windows e iOS tras instalar módulos correspondientes.

**Estado de validación:** el código se revisa con CI estática; la ejecución en Editor y compilaciones de PC/iPhone requieren Unity y sus respectivos módulos. No declarar cerrado este criterio hasta efectuar pruebas reales.
