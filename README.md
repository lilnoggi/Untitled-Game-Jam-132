# [Working Title] - Rabbit Gun Game
*You are the forest's strongest rabbit, defending your home from a greedy city planner and waves of annoying humans using an arsenal of non-lethal, veggie-themed weapons.*

## Engine Version
**Unity 6000.5.10f1**

## Controls
* **WASD:** Movement
* **Mouse Position:** Aim
* **Left-Click:** Shoot
* **Q / E:** Cycle Weapons (Water Gun, Paintball Gun, Carrot Launcher)
* **Left-Shift:** Dash

## Repository & Git Workflow
To keep the project clean and avoid merge conflicts during the jam, please adhere to the following guidelines:
* **Main Branch:** Only for stable, playable MVP builds. Do not commit directly to main.
* **Feature Branches:** Branch off main using the `feature/your-feature-name` naming convention.
* **Pull Requests:** All merges into main require a Pull Request using the provided `.github` template. Ensure your scene runs without errors before requesting a review.

## Architecture Overview
* **Data-Driven Design:** Weapon stats, enemy health, and wave configurations are decoupled into `ScriptableObjects`. Edit these data containers directly rather than tweaking prefabs.
* **Object Pooling:** Projectiles and Enemies are strictly recycled via the custom `PoolManager` to eliminate instantiation overhead. Do not use `Instantiate()` or `Destroy()` for these entities.
* **Manager Decoupling:** Core logic is split across dedicated systems (e.g., `WaveSpawner`, `WeaponManager`, `UIManager`). Avoid creating overlapping dependencies.
