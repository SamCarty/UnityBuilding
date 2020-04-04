using UnityEngine;

public class BuildModeController : MonoBehaviour {
    // Build mode on and off
    private bool buildModeIsObjects;
    private InstalledObjectType buildModeInstalledObjectType;
    private TileType buildModeTile = TileType.Floor;

    public void Build(Tile tile) {
        if (buildModeIsObjects) {
            // Create the InstalledObject and assign it to the designated Tile.

            // Check legality of placing InstalledObject here.
            if (InstalledObject.CheckPlacementValidity(buildModeInstalledObjectType, tile)) {
                // Create the InstalledObject as a new pending Job.
                
                WorldController.instance.world.PlaceInstalledObject(buildModeInstalledObjectType, tile);
                
                Job job = new Job(tile,
                    (j) => {
                         tile.installedObject.SetInstalled(true);
                    });

                // TODO: probably should move to somewhere else, too easy to forget to do this!!
                tile.pendingInstalledObjectJob = job;
                                
                job.RegisterJobCancelCallback(j => { tile.pendingInstalledObjectJob = null; });
                                
                // Add the job to the World's job queue.
                WorldController.instance.world.jobQueue.Enqueue(job);
            }
        }

        else {
            // We are in Tile changing mode, not object mode.
            if (tile != null) tile.tileType = buildModeTile;
        }
    }

    public void SetModeBuildFoundation() {
        buildModeTile = TileType.Floor;
        buildModeIsObjects = false;
    }

    public void SetModeBuildInstalledObject(string type) {
        // Wall is not a TileType, it is an InstalledObject!
        buildModeIsObjects = true;

        // Try and get the Enum ObjectType from the passed in string (workaround as Unity does not allow Enums to be
        // passed into script methods from the editor.
        try {
            InstalledObjectType objectType =
                (InstalledObjectType) System.Enum.Parse(typeof(InstalledObjectType), type);
            buildModeInstalledObjectType = objectType;
        }
        catch (System.Exception) {
            Debug.LogError("MouseController - Parse cannot convert the ObjectType string to an Enum, " +
                           "check the spelling.");
        }
    }

    public void SetModeBulldoze() {
        buildModeTile = TileType.Ground;
        buildModeIsObjects = false;
    }
}