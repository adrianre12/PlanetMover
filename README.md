# PlanetMover

PlanetMover is a tool for moving planets and associated grids in Space Engineers. The command is run when the game is <ins>not</ins> running and it parses the SANDBOX_0_0_0_.sbs to perform several actions;

- Move: Move the planet and grids within a configured radius to the GPS position.
- Filter: Copies all entities except those outside the configured radius. No move is made.
- Remove: Copies all entities except those inside the configured radius. No move is made.
- Extract: Output the Entities inside the configured radius; this does not create a valid SBS. 
- Config: Create a new default config file.

PlanetMover calculates the transformation from the planet's centre to the GPS point, which is then applied to each entity.

## Config creation

In the SBS, the planet positions are not centred on the planet, but rather on a point outside the planet. The only way to find the centre is via code in-game. 
To aid in this, the Mod PlanetMoverTools adds an 'Admin Only' block, which generates the config. Place the block on the planet and then copy the config from the Custom Data.
The To position by default is set to the planet centre, thus if this were used, the planet would not move. A diff can then be used to check that PlanetMover is not causing issues.

The To position can then be updated with the X, Y and Z positions from the target position GPS.

## Notes

It is recommended that the SANDBOX_0_0_0_.sbs be copied to a directory outside of the world save as the directory is wiped with each game save.

It is necessary to delete SANDBOX_0_0_0_.sbsB5 so that it is recreated from the modified SANDBOX_0_0_0_.sbs file. This will cause a much longer start-up as it is regenerated. 
The logging will appear to freeze at the line "Keen:    Console compatibility: No" but if Taskmanger is viewed, it can be seen that the server is still working.
