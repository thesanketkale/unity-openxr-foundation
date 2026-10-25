# ADR-0002: Analytical Projection Physics Math Over HingeJoint

## Context
Physical interactables such as levers, rotary dials, sliders, push buttons, and hinged doors are central to immersive VR simulations. Traditionally, developers implement these using Unity's PhysX components, specifically `HingeJoint` and `ConfigurableJoint`.

However, in standalone mobile VR:
1. PhysX iterative constraint solvers struggle under variable frame rates (e.g. thermal throttling dropping from 90Hz to 72Hz), causing joint oscillation, elasticity, and visual jitter.
2. Rapid hand motion can violate joint penetration constraints, causing joint disconnects or physics explosions.
3. Network synchronization of joint constraints in future multiplayer extensions introduces compounding physics desync.

## Decision
We implement all mechanical interactables using **analytical kinematic projection math** rather than `HingeJoint` or `ConfigurableJoint`:
- **Levers & Doors:** The interactor's grab pose is transformed into the hinge's local space and projected onto the 2D plane perpendicular to the rotation axis. The angle is computed using `Mathf.Atan2(y, x)` relative to a reference zero vector and strictly clamped to `[minAngle, maxAngle]`.
- **Rotary Dials:** The angular delta between consecutive frames is computed by projecting interactor motion around the dial's rotation axis, allowing detented or continuous multi-turn operation with zero joint elasticity.
- **Sliders:** The interactor pose is projected onto the 1D travel segment vector using dot product projection: `progress = Vector3.Dot(delta, axis) / length`.
- **Push Buttons:** The poke point is projected along the depression normal with a hysteresis threshold (80% down = click activation, 40% up = reset release) to prevent contact bounce.

## Alternatives Considered
1. **Unity `HingeJoint` with Rigidbody Limits:** High physics solver jitter; erratic behaviour when grabbed with fast hand tracking; requires heavy physics sub-stepping.
2. **Kinematic Rigidbody with Distance Springs:** Still exhibits spring bounce and overshoot, producing unrealistic mechanical feel for solid levers and dials.

## Consequences
- **Positive:** Zero physics solver jitter; 100% deterministic behaviour regardless of frame rate; completely stable under erratic hand tracking tracking losses; easily unit-testable without physics engine ticks.
- **Negative:** Secondary dynamic collisions (e.g., another thrown object striking a half-open door) require dedicated kinematic sweep queries.
