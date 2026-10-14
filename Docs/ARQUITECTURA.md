# Arquitectura — Football Elite 2027

## Tecnología

- Unity 6 (motor 3D) con Universal Render Pipeline (URP) para reducir diferencias entre PC e iOS.
- C# para sistemas de juego.
- Unity Input System para teclado, mando y pantalla táctil.
- PhysX (`Rigidbody`, `Collider` y `ForceMode.Impulse`) para el balón.
- uGUI (`Canvas`, `EventSystem`) para marcador y controles móviles.

## Capas y dependencias

```text
               +----------------------+
               |  MatchManager        |  reloj, goles, reinicio
               +----------+-----------+
                          |
      +-------------------+------------------+
      |                   |                  |
 PlayerController    BallController     GoalTrigger
      |                   |                  |
 FootballInput        PhysX             MatchManager
      |                   |
      +-- TouchJoystick   +-- GoalkeeperAI
      +-- TouchButton
                          |
                BroadcastCamera (solo visual)
                MatchHUD (solo presentación)
                GraphicsBootstrap (adaptación)
```

Los componentes se asignan desde `TrainingSceneBuilder` en Editor y se conectan por campos serializados; no necesitan buscar objetos por nombre en cada cuadro.

## Escala

- Campo 64 x 100 unidades (1 unidad ≈ 1 m).
- Pelota radio 0.22 m, masa aproximada 0.43 kg.
- Portería ancho 7.32 m; alto 2.44 m.
- Equipo inicial: un jugador y un portero rival de práctica.

## Reglas del prototipo

- Entrenamiento sin fuera de juego ni faltas (fases posteriores).
- Se puede regatear empujando el balón de manera física.
- Pase bajo y tiro elevado disponibles a corta distancia.
- Los sensores de las porterías registran goles cuando el balón cruza el área.
- El marcador permite reiniciar la sesión.

## Rendimiento

- Pantalla móvil: iluminación más ligera, 60 FPS objetivo (no garantizados), sombras limitadas, controles táctiles grandes.
- PC: más calidad, mayor resolución, mando o teclado.
- Geometría del escenario generada con materiales reutilizables y sin texturas externas pesadas.
- Las optimizaciones de instancing, LOD, animaciones y crowd se implementarán al introducir modelos 3D profesionales.

## Próximas fases

Arquitectura extensible: `TeamController`, `FootballerAnimator`, `TacticalAI`, `GoalkeeperAI`, `MatchRules`, `MatchPresentation`, `CareerMode`, `ReplaySystem`. Cada sistema entra después de validar la física y la interacción base.

## Limitaciones honestas

El generador construye una escena inicial con geometría simple. NO produce automáticamente jugadores fotorrealistas, animación mocap ni la calidad audiovisual de EA SPORTS FC. Esas mejoras requieren modelos, rig, texturas y animaciones con licencia y optimización específica por plataforma.
